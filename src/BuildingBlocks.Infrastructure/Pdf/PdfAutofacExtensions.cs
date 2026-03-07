using Autofac;
using BuildingBlocks.Application.Pdf;

namespace BuildingBlocks.Infrastructure.Pdf;

public static class PdfAutofacExtensions
{
    public static ContainerBuilder AddHtmlToPdfConverter(this ContainerBuilder builder)
    {
        builder.RegisterType<PuppeteerHtmlToPdfConverter>().As<IHtmlToPdfConverter>().SingleInstance().IfNotRegistered(typeof(IHtmlToPdfConverter));

        return builder;
    }
}
