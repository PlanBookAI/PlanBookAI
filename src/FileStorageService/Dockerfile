FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8086

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PlanbookAI.FileStorageService.csproj", "."]
RUN dotnet restore "./PlanbookAI.FileStorageService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PlanbookAI.FileStorageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlanbookAI.FileStorageService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Create storage directory
RUN mkdir -p /app/data/files

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8086
ENV FILE_STORAGE_ROOT=/app/data/files
ENV MAX_UPLOAD_MB=50
ENV ALLOWED_MIME=image/jpeg,image/png,image/gif,image/webp,image/bmp,application/pdf,text/plain,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet

ENTRYPOINT ["dotnet", "PlanbookAI.FileStorageService.dll"]