using Autofac.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Nop.Web;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile(NopConfigurationDefaults.AppSettingsFilePath, true, true);
        if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
        {
            var path = string.Format(NopConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
            builder.Configuration.AddJsonFile(path, true, true);
        }
        builder.Configuration.AddEnvironmentVariables();

        //load application settings
        builder.Services.ConfigureApplicationSettings(builder);

        var appSettings = Singleton<AppSettings>.Instance;
        var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;
        if (useAutofac)
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        else
        {
            builder.Host.UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = false;
                options.ValidateOnBuild = true;
            });
        }

        //add services to the application and configure service provider
        builder.Services.ConfigureApplicationServices(builder);

        // ── OpenTelemetry ─────────────────────────────────────────────────────
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService("nopcommerce", serviceVersion: "1.0.0"))
                .AddAspNetCoreInstrumentation(opts => { opts.RecordException = true; })
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation(opts =>
                {
                    opts.SetDbStatementForText = true;
                    opts.RecordException = true;
                })
                .AddSource("NopCommerce.Catalog")
                .AddProcessor(new Nop.Services.Infrastructure.PiiSanitiserProcessor())
                .AddConsoleExporter()
                .AddOtlpExporter(opts =>
                {
                    opts.Endpoint = new Uri("http://otel-collector:4317");
                    opts.Protocol = OtlpExportProtocol.Grpc;
                }))
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService("nopcommerce", serviceVersion: "1.0.0"))
                .AddAspNetCoreInstrumentation()
                .AddMeter("NopCommerce.Metrics")
                .AddOtlpExporter(opts =>
                {
                    opts.Endpoint = new Uri("http://otel-collector:4318/v1/metrics");
                    opts.Protocol = OtlpExportProtocol.HttpProtobuf;
                }));
        // ── End OpenTelemetry ─────────────────────────────────────────────────

        var app = builder.Build();

        //configure the application HTTP request pipeline
        app.ConfigureRequestPipeline();
        await app.PublishAppStartedEventAsync();
        await app.RunAsync();
    }
}