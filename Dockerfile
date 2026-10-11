# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Clinic.Api/Clinic.Api.csproj", "src/Clinic.Api/"]
COPY ["src/Clinic.Application/Clinic.Application.csproj", "src/Clinic.Application/"]
COPY ["src/Clinic.Domain/Clinic.Domain.csproj", "src/Clinic.Domain/"]
COPY ["src/Clinic.Infrastructure/Clinic.Infrastructure.csproj", "src/Clinic.Infrastructure/"]
COPY ["src/Clinic.Persistence/Clinic.Persistence.csproj", "src/Clinic.Persistence/"]
RUN dotnet restore "./src/Clinic.Api/Clinic.Api.csproj"
COPY . .
WORKDIR "/src/src/Clinic.Api"
RUN dotnet build "./Clinic.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Clinic.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clinic.Api.dll"]