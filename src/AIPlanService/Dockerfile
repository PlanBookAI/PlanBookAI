FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8090

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AIPlanService.csproj", "./"]
RUN dotnet restore "AIPlanService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AIPlanService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AIPlanService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIPlanService.dll"]
