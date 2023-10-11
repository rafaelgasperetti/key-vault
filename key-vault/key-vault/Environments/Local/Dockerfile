#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
RUN rm -rf /app
COPY . ./src
RUN dotnet user-secrets -p /app/src/key-vault/key-vault.csproj set "Kestrel:Certificates:Development:Password" "key-vault"
RUN dotnet restore /app/src/key-vault/key-vault.csproj
RUN dotnet build /app/src/key-vault/key-vault.csproj --configuration Debug --no-cache --output /app/build
RUN dotnet publish /app/src/key-vault/key-vault.csproj --configuration Debug --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY ./key-vault/Environments/Local/key-vault.pfx /usr/local/share/ca-certificates/
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=key-vault
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/usr/local/share/ca-certificates/key-vault.pfx
RUN apt-get update && apt-get install -y curl default-mysql-client ca-certificates && update-ca-certificates
HEALTHCHECK --interval=3s --timeout=1s --retries=3 CMD curl -f -k https://localhost/health --fail
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "key-vault.dll"]