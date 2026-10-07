# Dockerfile Patterns
Multi-stage .NET Dockerfiles, base-image choice, and production image hardening. Load this when writing or reviewing a Dockerfile.

### Multi-Stage Build for .NET
```dockerfile
# File: src/{ApplicationName}.Services.API/Dockerfile

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["global.json", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]

# Copy all project files
COPY ["src/{ApplicationName}.Services.API/{ApplicationName}.Services.API.csproj", "src/{ApplicationName}.Services.API/"]
COPY ["src/{ApplicationName}.Domain.{Domain}/{ApplicationName}.Domain.{Domain}.csproj", "src/{ApplicationName}.Domain.{Domain}/"]
COPY ["src/{ApplicationName}.Data/{ApplicationName}.Data.csproj", "src/{ApplicationName}.Data/"]
COPY ["src/{ApplicationName}.Entities.{Domain}/{ApplicationName}.Entities.{Domain}.csproj", "src/{ApplicationName}.Entities.{Domain}/"]
COPY ["src/{ApplicationName}.Models.{Domain}/{ApplicationName}.Models.{Domain}.csproj", "src/{ApplicationName}.Models.{Domain}/"]

# Restore dependencies
RUN dotnet restore "src/{ApplicationName}.Services.API/{ApplicationName}.Services.API.csproj"

# Copy all source code
COPY . .

# Build application
WORKDIR "/src/src/{ApplicationName}.Services.API"
RUN dotnet build "{ApplicationName}.Services.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "{ApplicationName}.Services.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create non-root user
RUN addgroup --gid 1000 appgroup && \
    adduser --uid 1000 --gid 1000 --disabled-password --gecos "" appuser

# Copy published app
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "{ApplicationName}.Services.API.dll"]
```

### Docker Compose for Local Development
```yaml
# docker-compose.yml
version: '3.8'

services:
  postgres:
    image: postgres:17-alpine
    container_name: {applicationname}-postgres
    environment:
      POSTGRES_USER: ${POSTGRES_USER:-postgres}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-postgres}
      POSTGRES_DB: ${POSTGRES_DB:-{applicationname}}
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    container_name: {applicationname}-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build:
      context: .
      dockerfile: src/{ApplicationName}.Services.API/Dockerfile
    container_name: {applicationname}-api
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__DefaultConnection=Host=postgres;Database={applicationname};Username=postgres;Password=postgres
      - ConnectionStrings__Redis=redis:6379
    ports:
      - "5000:8080"
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

volumes:
  postgres-data:
  redis-data:
```

### .dockerignore
```
# .dockerignore
**/.git
**/.gitignore
**/.vs
**/.vscode
**/bin
**/obj
**/*.user
**/node_modules
**/npm-debug.log
**/.DS_Store
**/Thumbs.db
**/*.md
!README.md
**/test-results
**/.claude
```
