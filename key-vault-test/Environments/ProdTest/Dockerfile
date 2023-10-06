#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
RUN rm -rf /app
COPY . ./src
RUN dotnet user-secrets -p /app/src/key-vault/key-vault/key-vault.csproj set "Kestrel:Certificates:Development:Password" "key-vault"

FROM base AS build-env
WORKDIR /app
#Env
ARG APIEnvironment__Environment
ARG APIEnvironment__DatabaseHost
ARG APIEnvironment__DatabasePort
ARG APIEnvironment__DatabaseUser
ARG APIEnvironment__DatabasePassword
ARG APIEnvironment__EncryptValues
ARG APIEnvironment__Secret
ARG APIEnvironment__JWTIssuer
ARG APIEnvironment__JWTAudience
ARG APIEnvironment__KeyVaultAPIUrl

ENV APIEnvironment__Environment=${APIEnvironment__Environment}
ENV APIEnvironment__DatabaseHost=${APIEnvironment__DatabaseHost}
ENV APIEnvironment__DatabasePort=${APIEnvironment__DatabasePort}
ENV APIEnvironment__DatabaseUser=${APIEnvironment__DatabaseUser}
ENV APIEnvironment__DatabasePassword=${APIEnvironment__DatabasePassword}
ENV APIEnvironment__EncryptValues=${APIEnvironment__EncryptValues}
ENV APIEnvironment__Secret=${APIEnvironment__Secret}
ENV APIEnvironment__JWTIssuer=${APIEnvironment__JWTIssuer}
ENV APIEnvironment__JWTAudience=${APIEnvironment__JWTAudience}
ENV APIEnvironment__KeyVaultAPIUrl=${APIEnvironment__KeyVaultAPIUrl}
#Tests
RUN dotnet restore /app/src/key-vault-test/key-vault-test.csproj
RUN dotnet build /app/src/key-vault-test/key-vault-test.csproj --configuration Release --no-cache --output /app/build
RUN dotnet test /app/src/key-vault-test/key-vault-test.csproj --configuration Release