FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["InnoShop.Products.API/InnoShop.Products.API.csproj", "InnoShop.Products.API/"]
COPY ["InnoShop.Products.Application/InnoShop.Products.Application.csproj", "InnoShop.Products.Application/"]
COPY ["InnoShop.Products.Domain/InnoShop.Products.Domain.csproj", "InnoShop.Products.Domain/"]
COPY ["InnoShop.Products.Infrastructure/InnoShop.Products.Infrastructure.csproj", "InnoShop.Products.Infrastructure/"]

RUN dotnet restore "InnoShop.Products.API/InnoShop.Products.API.csproj"

COPY . .

WORKDIR "/src/InnoShop.Products.API"
RUN dotnet build "InnoShop.Products.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InnoShop.Products.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN mkdir -p /app/data

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "InnoShop.Products.API.dll"]