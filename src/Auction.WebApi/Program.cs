using Auction.WebApi;
using Auction.WebApi.Features;
using Auction.WebApi.Infrastructure;

var builder = WebApplication.CreateSlimBuilder(args);

var configurationRoot = builder.Configuration as IConfigurationRoot;

builder.Services.AddAuctionExtensions(configurationRoot);

builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.LotsFeature();
app.MapRazorPages();
app.MapEndpoints();

app.Run();
