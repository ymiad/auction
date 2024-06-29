using Auction.WebApi;
using Auction.WebApi.Features;
using Auction.WebApi.Infrastructure;
using CleanArchitecture.Web.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateSlimBuilder(args);

var configurationRoot = builder.Configuration as IConfigurationRoot;

builder.Services.AddAuctionExtensions(configurationRoot);
builder.Services.AddHttpContextAccessor();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Auth Demo Api enabled with JWT Bearer",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Bearer {your JWT token}."
            });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.LotsFeature();
app.UsersFeature();
app.AccountsFeature();
app.UserBetsFeature();
app.UseAuthentication();
app.MapEndpoints();

app.Run();
