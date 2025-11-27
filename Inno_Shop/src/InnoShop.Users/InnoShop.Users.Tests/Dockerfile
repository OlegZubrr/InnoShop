FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/InnoShop.Users/InnoShop.Users.Tests/InnoShop.Users.Tests.csproj", "src/InnoShop.Users/InnoShop.Users.Tests/"]
RUN dotnet restore "src/InnoShop.Users/InnoShop.Users.Tests/InnoShop.Users.Tests.csproj"
COPY . .
WORKDIR "/src/src/InnoShop.Users/InnoShop.Users.Tests"
RUN dotnet build "./InnoShop.Users.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InnoShop.Users.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InnoShop.Users.Tests.dll"]
