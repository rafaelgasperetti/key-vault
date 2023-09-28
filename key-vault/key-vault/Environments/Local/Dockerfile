#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
RUN dotnet new webapi -o app
RUN dotnet dev-certs https --clean && dotnet dev-certs https -ep /https/aspnetapp.pfx -p WH2mYa97fRrxVEGd && dotnet dev-certs https
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["key-vault/key-vault.csproj", "key-vault/"]
RUN dotnet restore "key-vault/key-vault.csproj"
COPY . .
WORKDIR "/src/key-vault"
RUN dotnet build "key-vault.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "key-vault.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "key-vault.dll"]