using Auction.WebApi;
using Auction.WebApi.Infrastructure;
using Serilog;

var builder = WebApplication.CreateSlimBuilder(args);

var configurationRoot = builder.Configuration as IConfigurationRoot;

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddSerilog(logger);

builder.Services.AddWebServices(configurationRoot);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseFeatures();
app.UseAuthentication();

app.Run();
