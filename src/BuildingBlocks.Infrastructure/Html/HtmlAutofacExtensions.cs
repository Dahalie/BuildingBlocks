using Autofac;
using BuildingBlocks.Application.Html;

namespace BuildingBlocks.Infrastructure.Html;

public static class HtmlAutofacExtensions
{
    public static ContainerBuilder AddHtml(this ContainerBuilder builder)
    {
        builder.RegisterType<ScribanTemplateRenderer>().As<ITemplateRenderer>().SingleInstance().IfNotRegistered(typeof(ITemplateRenderer));
        builder.RegisterType<GanssHtmlSanitizer>().As<IHtmlSanitizer>().SingleInstance().IfNotRegistered(typeof(IHtmlSanitizer));
        builder.RegisterType<HtmlToTextConverter>().As<IHtmlToTextConverter>().SingleInstance().IfNotRegistered(typeof(IHtmlToTextConverter));

        return builder;
    }
}
