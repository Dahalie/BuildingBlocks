using BuildingBlocks.Application.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BuildingBlocks.Infrastructure.Excel;

public static class ExcelMicrosoftExtensions
{
    public static IServiceCollection AddExcel(this IServiceCollection services)
    {
        services.TryAddSingleton<IExcelReader, ExcelFileReader>();
        services.TryAddSingleton<IExcelWriter, ExcelFileWriter>();

        return services;
    }
}
