using BuildingBlocks.Application.Pdf;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace BuildingBlocks.Infrastructure.Pdf;

public class PuppeteerHtmlToPdfConverter : IHtmlToPdfConverter, IAsyncDisposable
{
    private IBrowser? _browser;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task ConvertAsync(string htmlContent, Stream destination, CancellationToken ct = default)
    {
        var browser = await GetBrowserAsync();

        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(htmlContent, new NavigationOptions { WaitUntil = [WaitUntilNavigation.Networkidle0] });

        var pdfBytes = await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "20mm",
                Bottom = "20mm",
                Left = "15mm",
                Right = "15mm"
            }
        });

        await destination.WriteAsync(pdfBytes, ct);
    }

    private async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser is { IsClosed: false })
            return _browser;

        await _semaphore.WaitAsync();
        try
        {
            if (_browser is { IsClosed: false })
                return _browser;

            var fetcher = new BrowserFetcher();
            await fetcher.DownloadAsync();

            _browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            return _browser;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
            _browser = null;
        }

        _semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}
