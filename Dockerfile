FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/MusicMasterShop/Presentation/MusicMasterShop.WebApi/MusicMasterShop.WebApi.csproj", "src/MusicMasterShop/Presentation/MusicMasterShop.WebApi/"]
COPY ["src/MusicMasterShop/Application/MusicMasterShop.Application/MusicMasterShop.Application.csproj", "src/MusicMasterShop/Application/MusicMasterShop.Application/"]
COPY ["src/MusicMasterShop/Infrastructure/MusicMasterShop.InfraData/MusicMasterShop.InfraData.csproj", "src/MusicMasterShop/Infrastructure/MusicMasterShop.InfraData/"]
COPY ["src/MusicMasterShop/Domain/MusicMasterShop.Domain/MusicMasterShop.Domain.csproj", "src/MusicMasterShop/Domain/MusicMasterShop.Domain/"]

RUN dotnet restore "src/MusicMasterShop/Presentation/MusicMasterShop.WebApi/MusicMasterShop.WebApi.csproj"

COPY . .

WORKDIR "/src/src/MusicMasterShop/Presentation/MusicMasterShop.WebApi"
RUN dotnet publish "MusicMasterShop.WebApi.csproj" \
    --configuration "$BUILD_CONFIGURATION" \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MusicMasterShop.WebApi.dll"]
