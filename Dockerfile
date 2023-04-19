FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS="http://+:5000"

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG GITHUB_PAT
COPY ["src/Play.Catalog.Contracts/Play.Catalog.Contracts.csproj", "src/Play.Catalog.Contracts/"]
COPY ["src/Play.Catalog.Service/Play.Catalog.Service.csproj", "src/Play.Catalog.Service/"]
COPY ["src/Play.Catalog.Service/nuget.config", "src/Play.Catalog.Service/"]

RUN dotnet restore "src/Play.Catalog.Service/Play.Catalog.Service.csproj"

COPY ["./src", "./src"]
WORKDIR "/src/Play.Catalog.Service"
RUN dotnet publish "Play.Catalog.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Play.Catalog.Service.dll"]
