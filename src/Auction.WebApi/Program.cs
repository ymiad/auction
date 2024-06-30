using Auction.WebApi;
using Auction.WebApi.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);

var configurationRoot = builder.Configuration as IConfigurationRoot;

builder.Services.AddWebServices(configurationRoot);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseFeatures();
app.UseAuthentication();

app.Run();
