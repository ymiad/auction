using Auction.WebApi.Features;

namespace Auction.WebApi.Infrastructure;

public static class WebAppExtensions
{
    public static WebApplication UseFeatures(this WebApplication app)
    {
        app.LotsFeature();
        app.UsersFeature();
        app.AccountsFeature();
        app.UserBetsFeature();
        app.UseAuthentication();

        return app;
    }
}
