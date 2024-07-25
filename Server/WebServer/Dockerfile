#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebServer/WebServer.csproj", "WebServer/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["BuisnesLogic/BuisnesLogic.csproj", "BuisnesLogic/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["DomainModel/DomainModel.csproj", "DomainModel/"]
COPY ["DataBase/DataBase.csproj", "DataBase/"]
RUN dotnet restore "./WebServer/WebServer.csproj"
COPY . .
WORKDIR "/src/WebServer"
RUN dotnet build "./WebServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebServer.dll"]