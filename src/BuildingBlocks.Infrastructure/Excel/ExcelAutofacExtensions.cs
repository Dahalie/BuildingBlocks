using Autofac;
using BuildingBlocks.Application.Excel;

namespace BuildingBlocks.Infrastructure.Excel;

public static class ExcelAutofacExtensions
{
    public static ContainerBuilder AddExcel(this ContainerBuilder builder)
    {
        builder.RegisterType<ExcelFileReader>().As<IExcelReader>().SingleInstance().IfNotRegistered(typeof(IExcelReader));
        builder.RegisterType<ExcelFileWriter>().As<IExcelWriter>().SingleInstance().IfNotRegistered(typeof(IExcelWriter));

        return builder;
    }
}
