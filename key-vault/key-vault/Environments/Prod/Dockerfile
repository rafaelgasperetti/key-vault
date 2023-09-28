#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS copy
WORKDIR /app
COPY . ./src

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app/src
RUN dotnet build "key-vault.csproj" -c Release -o /app/build
RUN dotnet publish "key-vault.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "key-vault.dll"]