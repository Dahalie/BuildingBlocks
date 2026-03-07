using System.Globalization;
using BuildingBlocks.Application.Csv;
using CsvHelper;

namespace BuildingBlocks.Infrastructure.Csv;

public class CsvFileReader : ICsvReader
{
    public async Task<IReadOnlyList<T>> ReadAsync<T>(Stream stream, CancellationToken ct = default)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = new List<T>();

        await foreach (var record in csv.GetRecordsAsync<T>(ct))
            records.Add(record);

        return records;
    }
}
