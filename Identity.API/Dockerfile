FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80 

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["Identity.API/Identity.API.csproj", "Identity.API/"]
COPY ["Core/Core.csproj", "Core/"]

# Clear NuGet cache before restoring to save space and avoid cache conflicts
RUN dotnet nuget locals all --clear

# Restore dependencies
RUN dotnet restore "./Identity.API/Identity.API.csproj"

# Copy the rest of the source code
COPY . .

# Set the working directory for build
WORKDIR "/src/Identity.API"

# Build the project
RUN dotnet build "./Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Clear NuGet cache again after build to reduce final image size
RUN dotnet nuget locals all --clear

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Identity.API.dll"]
