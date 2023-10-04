#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
RUN rm -rf /app

FROM ubuntu:latest
WORKDIR /app
RUN mkdir -p test-results

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
COPY . ./src
RUN dotnet user-secrets -p /app/src/key-vault/key-vault.csproj set "Kestrel:Certificates:Development:Password" "key-vault"
RUN dotnet restore /app/src/key-vault/key-vault.csproj

RUN dotnet build /app/src/key-vault/key-vault.csproj --configuration Release --no-cache --output /app/build
RUN dotnet test /app/src/key-vault/key-vault.csproj --configuration Release --logger "html;logfilename=/app/test-results/results.html;verbosity=detailed"

RUN dotnet publish /app/src/key-vault/key-vault.csproj --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl
HEALTHCHECK --interval=3s --timeout=1s --retries=3 CMD curl -f -k https://localhost/health --fail
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "key-vault.dll"]