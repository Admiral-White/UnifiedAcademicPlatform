# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install netcat for service waiting
RUN apt-get update && apt-get install -y netcat-openbsd && rm -rf /var/lib/apt/lists/*

# Use the .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["src/Services/Authentication/UAP.Authentication.API/UAP.Authentication.API.csproj", "src/Services/Authentication/UAP.Authentication.API/"]
RUN dotnet restore "src/Services/Authentication/UAP.Authentication.API/UAP.Authentication.API.csproj"

# Copy all source code
COPY . .
WORKDIR "/src/src/Services/Authentication/UAP.Authentication.API"
RUN dotnet build "UAP.Authentication.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "UAP.Authentication.API.csproj" -c Release -o /app/publish

# Final stage - create the runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UAP.Authentication.API.dll"]