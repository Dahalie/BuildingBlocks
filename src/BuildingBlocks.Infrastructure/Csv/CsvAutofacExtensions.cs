using Autofac;
using BuildingBlocks.Application.Csv;

namespace BuildingBlocks.Infrastructure.Csv;

public static class CsvAutofacExtensions
{
    public static ContainerBuilder AddCsv(this ContainerBuilder builder)
    {
        builder.RegisterType<CsvFileReader>().As<ICsvReader>().SingleInstance().IfNotRegistered(typeof(ICsvReader));
        builder.RegisterType<CsvFileWriter>().As<ICsvWriter>().SingleInstance().IfNotRegistered(typeof(ICsvWriter));

        return builder;
    }
}
