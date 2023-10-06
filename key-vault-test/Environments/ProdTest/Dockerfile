#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
RUN rm -rf /app
COPY . ./src
RUN dotnet user-secrets -p /app/src/key-vault/key-vault/key-vault.csproj set "Kestrel:Certificates:Development:Password" "key-vault"

FROM base AS build-env
WORKDIR /app
#Env
#ARG env_from_dockerfile
#ENV env_from_dockerfile=${env_from_dockerfile}
#Tests
RUN dotnet restore /app/src/key-vault-test/key-vault-test.csproj
RUN dotnet build /app/src/key-vault-test/key-vault-test.csproj --configuration Release --no-cache --output /app/build
RUN dotnet test /app/src/key-vault-test/key-vault-test.csproj --configuration Release

#FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
#WORKDIR /app
#ENTRYPOINT ["dotnet", "test"]