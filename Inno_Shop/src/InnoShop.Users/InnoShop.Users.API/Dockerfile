FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/InnoShop.Users/InnoShop.Users.API/InnoShop.Users.API.csproj", "src/InnoShop.Users/InnoShop.Users.API/"]
COPY ["src/InnoShop.Users/InnoShop.Users.Domain/InnoShop.Users.Domain.csproj", "src/InnoShop.Users/InnoShop.Users.Domain/"]
COPY ["src/InnoShop.Users/InnoShop.Users.Application/InnoShop.Users.Application.csproj", "src/InnoShop.Users/InnoShop.Users.Application/"]
COPY ["src/InnoShop.Users/InnoShop.Users.Infrastructure/InnoShop.Users.Infrastructure.csproj", "src/InnoShop.Users/InnoShop.Users.Infrastructure/"]
RUN dotnet restore "src/InnoShop.Users/InnoShop.Users.API/InnoShop.Users.API.csproj"
COPY . .
WORKDIR "/src/src/InnoShop.Users/InnoShop.Users.API"
RUN dotnet build "./InnoShop.Users.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./InnoShop.Users.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InnoShop.Users.API.dll"]
