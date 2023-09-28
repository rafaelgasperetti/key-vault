#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app/src
COPY . ./
RUN dotnet build "src/key-vault.csproj" -c Release -o /app/build
RUN dotnet publish "src/key-vault.csproj" -c Release -o /app/publish

# Final image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "key-vault.dll"]