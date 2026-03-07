using System.Globalization;
using BuildingBlocks.Application.Csv;
using CsvHelper;

namespace BuildingBlocks.Infrastructure.Csv;

public class CsvFileWriter : ICsvWriter
{
    public async Task WriteAsync<T>(IEnumerable<T> records, Stream destination, CancellationToken ct = default)
    {
        await using var writer = new StreamWriter(destination, leaveOpen: true);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await csv.WriteRecordsAsync(records, ct);
    }
}
