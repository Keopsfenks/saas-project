# Base image (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image (SDK)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Proje dosyalarını kopyala
COPY ["../WebApi/WebApi.csproj", "WebApi/"]
COPY ["../core/Application/Application.csproj", "core/Application/"]
COPY ["../core/Domain/Domain.csproj", "core/Domain/"]
COPY ["../external/Infrastructure/Infrastructure.csproj", "external/Infrastructure/"]

# Bağımlılıkları yükle
RUN dotnet restore "WebApi/WebApi.csproj"

# Kalan tüm dosyaları kopyala ve projeyi derle
COPY . .
WORKDIR "/src/WebApi"
RUN dotnet build "WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
