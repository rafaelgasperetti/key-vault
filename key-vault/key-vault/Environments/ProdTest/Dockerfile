#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
RUN rm -rf /app
COPY . ./src
RUN dotnet user-secrets -p /app/src/key-vault/key-vault/key-vault.csproj set "Kestrel:Certificates:Development:Password" "key-vault"

FROM base AS build-env
WORKDIR /app
#Env
ENV APIEnvironment__Environment=$APIEnvironment__Environment
ENV APIEnvironment__DatabaseHost=key-vault-pipe-test.db
ENV APIEnvironment__DatabasePort=3306
ENV APIEnvironment__JWTIssuer=https://key-vault-pipe-test.key-vault
ENV APIEnvironment__JWTAudience=key-vault-pipe-test.key-vault
ENV APIEnvironment__KeyVaultAPIUrl=https://key-vault-pipe-test.key-vault
#Tests
RUN dotnet restore /app/src/key-vault-test/key-vault-test.csproj
RUN dotnet test /app/src/key-vault-test/key-vault-test.csproj --configuration Release
#App
RUN dotnet restore /app/src/key-vault/key-vault/key-vault.csproj
RUN dotnet build /app/src/key-vault/key-vault/key-vault.csproj --configuration Release --no-cache --output /app/build
RUN dotnet publish /app/src/key-vault/key-vault/key-vault.csproj --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y curl
HEALTHCHECK --interval=3s --timeout=1s --retries=3 CMD curl -f -k https://localhost/health --fail
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "key-vault.dll"]