#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
RUN rm -rf /app
RUN apt-get update --yes
RUN apt-get --yes install curl openssl
RUN dotnet dev-certs https --clean

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
COPY . ./src
RUN dotnet dev-certs https -ep /app/src/key-vault/key-vault.pfx -p key-vault
