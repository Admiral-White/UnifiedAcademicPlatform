FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/StudentAcademic/UAP.StudentAcademic.API/UAP.StudentAcademic.API.csproj", "src/Services/StudentAcademic/UAP.StudentAcademic.API/"]
RUN dotnet restore "src/Services/StudentAcademic/UAP.StudentAcademic.API/UAP.StudentAcademic.API.csproj"

COPY . .
WORKDIR "/src/src/Services/StudentAcademic/UAP.StudentAcademic.API"
RUN dotnet build "UAP.StudentAcademic.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UAP.StudentAcademic.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UAP.StudentAcademic.API.dll"]