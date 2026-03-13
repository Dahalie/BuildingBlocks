using BuildingBlocks.Application.Csv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.Csv;

public static class CsvMicrosoftExtensions
{
    public static IServiceCollection AddCsv(this IServiceCollection services)
    {
        services.TryAddSingleton<ICsvReader, CsvFileReader>();
        services.TryAddSingleton<ICsvWriter, CsvFileWriter>();

        return services;
    }
}
