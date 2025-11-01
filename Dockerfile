# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["PHR.sln", "./"]
COPY ["PHR.Api/PHR.Api.csproj", "PHR.Api/"]
COPY ["PHR.Application/PHR.Application.csproj", "PHR.Application/"]
COPY ["PHR.Domain/PHR.Domain.csproj", "PHR.Domain/"]
COPY ["PHR.Infrastructure/PHR.Infrastructure.csproj", "PHR.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "PHR.sln"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/PHR.Api"
RUN dotnet build "PHR.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PHR.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create directory for database
RUN mkdir -p /app/data

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Copy published output
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:8080/api/Health || exit 1

ENTRYPOINT ["dotnet", "PHR.Api.dll"]
