namespace BuildingBlocks.Application.Pdf;

public interface IHtmlToPdfConverter
{
    Task ConvertAsync(string htmlContent, Stream destination, CancellationToken ct = default);
}
