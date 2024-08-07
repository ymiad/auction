FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Auction.WebApi/Auction.WebApi.csproj", "src/Auction.WebApi/"]
COPY ["src/Auction.Application/Auction.Application.csproj", "src/Auction.Application/"]
COPY ["src/Auction.Domain/Auction.Domain.csproj", "src/Auction.Domain/"]
COPY ["src/Auction.Infrastructure/Auction.Infrastructure.csproj", "src/Auction.Infrastructure/"]
RUN dotnet restore "./src/Auction.WebApi/Auction.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Auction.WebApi"
RUN dotnet build "./Auction.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Auction.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auction.WebApi.dll"]