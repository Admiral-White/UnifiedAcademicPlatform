FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/CourseCatalog/UAP.CourseCatalog.API/UAP.CourseCatalog.API.csproj", "src/Services/CourseCatalog/UAP.CourseCatalog.API/"]
RUN dotnet restore "src/Services/CourseCatalog/UAP.CourseCatalog.API/UAP.CourseCatalog.API.csproj"

COPY . .
WORKDIR "/src/src/Services/CourseCatalog/UAP.CourseCatalog.API"
RUN dotnet build "UAP.CourseCatalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UAP.CourseCatalog.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UAP.CourseCatalog.API.dll"]