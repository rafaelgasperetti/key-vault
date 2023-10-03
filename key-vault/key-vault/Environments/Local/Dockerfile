#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
COPY . ./src
RUN apt-get update --yes
RUN apt-get --yes install curl openssl
RUN dotnet restore /app/src/key-vault/key-vault.csproj
RUN dotnet build /app/src/key-vault/key-vault.csproj --configuration Debug --no-cache --output /app/build
RUN dotnet dev-certs https
RUN dotnet publish /app/src/key-vault/key-vault.csproj --configuration Debug --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
HEALTHCHECK --interval=3s --timeout=1s --retries=3 CMD curl -f -k https://localhost/health --fail
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "key-vault.dll"]