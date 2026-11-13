using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Azure;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var connectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddAzureClientsCore();
    builder.Services.AddOptions<AzureMonitorExporterOptions>()
        .Configure<AzureComponentFactory>((options, factory) =>
        {
            options.ConnectionString = connectionString;
            options.Credential = factory.CreateTokenCredential(builder.Configuration.GetSection("AzureMonitorExporter"));
        });

    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
