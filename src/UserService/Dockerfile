# ==================== OPTIMIZED DOCKERFILE ====================
# Build stage - tối ưu layer caching
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release

# Install dependencies first (layer caching)
WORKDIR /src
COPY UserService.csproj ./
RUN dotnet restore --no-cache

# Copy source code (invalidates cache only when source changes)
COPY . ./

# Build và publish trong một layer
RUN dotnet publish UserService.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    --self-contained false

# ==================== RUNTIME STAGE ====================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8082

# Set entry point
ENTRYPOINT ["dotnet", "UserService.dll"]