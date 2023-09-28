#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app/build
COPY . ./
RUN dotnet restore "key-vault.csproj"
RUN dotnet build "key-vault.csproj" -c Release -o /app/build
RUN dotnet publish "key-vault.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish
WORKDIR /app/publish
ENTRYPOINT ["dotnet", "key-vault.dll"]