FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App
EXPOSE 8080
EXPOSE 8081

# Copy everything
COPY . .
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "ThreadService.API.dll"]

#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081

#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["ThreadService.API/ThreadService.API.csproj", "ThreadService.API/"]
#RUN dotnet restore "./ThreadService.API/ThreadService.API.csproj"
#COPY . .
#WORKDIR "/src/ThreadService.API"
#RUN dotnet build "./ThreadService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./ThreadService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "ThreadService.API.dll"]