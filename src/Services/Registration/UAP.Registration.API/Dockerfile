FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Registration/UAP.Registration.API/UAP.Registration.API.csproj", "src/Services/Registration/UAP.Registration.API/"]
COPY ["src/Services/Registration/UAP.Registration.Application/UAP.Registration.Application.csproj", "src/Services/Registration/UAP.Registration.Application/"]
COPY ["src/Shared/UAP.Shared.Infrastructure/UAP.Shared.Infrastructure.csproj", "src/Shared/UAP.Shared.Infrastructure/"]
COPY ["src/Shared/UAP.SharedKernel/UAP.SharedKernel.csproj", "src/Shared/UAP.SharedKernel/"]
COPY ["src/Shared/UAP.Shared.Contracts/UAP.Shared.Contracts.csproj", "src/Shared/UAP.Shared.Contracts/"]
COPY ["src/Services/Registration/UAP.Registration.Domain/UAP.Registration.Domain.csproj", "src/Services/Registration/UAP.Registration.Domain/"]
COPY ["src/Services/Registration/UAP.Registration.Infrastructure/UAP.Registration.Infrastructure.csproj", "src/Services/Registration/UAP.Registration.Infrastructure/"]
RUN dotnet restore "src/Services/Registration/UAP.Registration.API/UAP.Registration.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Registration/UAP.Registration.API"
RUN dotnet build "./UAP.Registration.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UAP.Registration.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UAP.Registration.API.dll"]
