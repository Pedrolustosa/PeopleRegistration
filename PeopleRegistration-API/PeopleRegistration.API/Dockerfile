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
COPY ["PeopleRegistration.API/PeopleRegistration.API.csproj", "PeopleRegistration.API/"]
COPY ["PeopleRegistration.Application/PeopleRegistration.Application.csproj", "PeopleRegistration.Application/"]
COPY ["PeopleRegistration.Domain/PeopleRegistration.Domain.csproj", "PeopleRegistration.Domain/"]
COPY ["PeopleRegistration.Infrastructure.IoC/PeopleRegistration.Infrastructure.IoC.csproj", "PeopleRegistration.Infrastructure.IoC/"]
COPY ["PeopleRegistration.Infrastructure/PeopleRegistration.Infrastructure.csproj", "PeopleRegistration.Infrastructure/"]
RUN dotnet restore "./PeopleRegistration.API/PeopleRegistration.API.csproj"
COPY . .
WORKDIR "/src/PeopleRegistration.API"
RUN dotnet build "./PeopleRegistration.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PeopleRegistration.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PeopleRegistration.API.dll"]