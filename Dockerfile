FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore GhostNetwork.Gateway.Api/GhostNetwork.Gateway.Api.csproj
WORKDIR /src/GhostNetwork.Gateway.Api
RUN dotnet build GhostNetwork.Gateway.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish GhostNetwork.Gateway.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GhostNetwork.Gateway.Api.dll"]
