FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["InnoShop.Users.API/InnoShop.Users.API.csproj", "InnoShop.Users.API/"]
COPY ["InnoShop.Users.Application/InnoShop.Users.Application.csproj", "InnoShop.Users.Application/"]
COPY ["InnoShop.Users.Domain/InnoShop.Users.Domain.csproj", "InnoShop.Users.Domain/"]
COPY ["InnoShop.Users.Infrastructure/InnoShop.Users.Infrastructure.csproj", "InnoShop.Users.Infrastructure/"]

RUN dotnet restore "InnoShop.Users.API/InnoShop.Users.API.csproj"

COPY . .

WORKDIR "/src/InnoShop.Users.API"
RUN dotnet build "InnoShop.Users.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InnoShop.Users.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN mkdir -p /app/data

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "InnoShop.Users.API.dll"]