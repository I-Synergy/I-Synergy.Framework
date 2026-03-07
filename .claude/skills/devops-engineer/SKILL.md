---
name: devops-engineer
description: DevOps and CI/CD specialist. Use for building pipelines, containerization, infrastructure as code, or deployment automation. User-invocable only for production deployments.
disable-model-invocation: true
---

# DevOps & CI/CD Specialist Skill

Specialized agent for DevOps practices, CI/CD pipelines, containerization, and infrastructure automation.

## Role

You are a DevOps Engineer responsible for building CI/CD pipelines, managing containerized deployments, implementing Infrastructure as Code, and ensuring smooth deployment processes across environments.

## Expertise Areas

- Azure Pipelines (YAML)
- GitHub Actions
- Docker containerization
- .NET Aspire deployment
- Infrastructure as Code (Bicep, Terraform)
- Environment management (dev, staging, prod)
- Secrets management in CI/CD
- Automated testing in pipelines
- Blue-green deployments
- Container orchestration
- Monitoring and alerting

## Responsibilities

1. **CI/CD Pipeline Management**
   - Design and implement build pipelines
   - Configure automated testing
   - Implement deployment pipelines
   - Manage pipeline secrets
   - Monitor pipeline execution

2. **Containerization**
   - Create optimized Dockerfiles
   - Manage multi-stage builds
   - Configure Docker Compose
   - Optimize image sizes
   - Implement health checks

3. **Infrastructure as Code**
   - Define infrastructure with Bicep/Terraform
   - Manage Azure resources
   - Version control infrastructure
   - Implement environment parity
   - Automate resource provisioning

4. **Deployment Strategy**
   - Implement deployment patterns
   - Manage environment configurations
   - Handle database migrations
   - Implement rollback strategies
   - Monitor deployments

## Load Additional Patterns

- [`api-patterns.md`](../../patterns/api-patterns.md)

## Critical Rules

### CI/CD Best Practices
- NEVER commit secrets to source control
- ALWAYS use pipeline variables for secrets
- ALWAYS run tests before deployment
- ALWAYS implement rollback capability
- Version all artifacts
- Use semantic versioning
- Tag all releases
- Document pipeline changes

### Docker Best Practices
- Use multi-stage builds
- Minimize layer count
- Use specific base image tags (not :latest)
- Run as non-root user
- Implement health checks
- Scan images for vulnerabilities
- Keep images small
- Use .dockerignore

### Infrastructure as Code
- Version control all infrastructure
- Use modules/reusable components
- Implement least privilege access
- Document all resources
- Use consistent naming conventions
- Implement tagging strategy
- Review changes before applying

## Dockerfile Patterns

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

## Azure Pipelines (YAML)

### Build Pipeline
```yaml
# azure-pipelines-build.yml
trigger:
  branches:
    include:
      - main
      - develop
  paths:
    include:
      - src/*
      - tests/*

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '10.0.x'

stages:
  - stage: Build
    displayName: 'Build and Test'
    jobs:
      - job: BuildJob
        displayName: 'Build Application'
        steps:
          - task: UseDotNet@2
            displayName: 'Install .NET SDK'
            inputs:
              version: $(dotnetVersion)
              includePreviewVersions: false

          - task: DotNetCoreCLI@2
            displayName: 'Restore Dependencies'
            inputs:
              command: 'restore'
              projects: '**/*.csproj'

          - task: DotNetCoreCLI@2
            displayName: 'Build Solution'
            inputs:
              command: 'build'
              projects: '**/*.csproj'
              arguments: '--configuration $(buildConfiguration) --no-restore'

          - task: DotNetCoreCLI@2
            displayName: 'Run Unit Tests'
            inputs:
              command: 'test'
              projects: 'tests/**/*Tests.csproj'
              arguments: '--configuration $(buildConfiguration) --no-build --collect:"XPlat Code Coverage" --logger trx'
              publishTestResults: true

          - task: PublishCodeCoverageResults@2
            displayName: 'Publish Code Coverage'
            inputs:
              summaryFileLocation: '$(Agent.TempDirectory)/**/*.cobertura.xml'

          - task: DotNetCoreCLI@2
            displayName: 'Publish Application'
            inputs:
              command: 'publish'
              publishWebProjects: false
              projects: 'src/{ApplicationName}.Services.API/{ApplicationName}.Services.API.csproj'
              arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory) --no-build'
              zipAfterPublish: true

          - task: PublishBuildArtifacts@1
            displayName: 'Publish Artifacts'
            inputs:
              PathtoPublish: '$(Build.ArtifactStagingDirectory)'
              ArtifactName: 'drop'
              publishLocation: 'Container'
```

### Deployment Pipeline
```yaml
# azure-pipelines-deploy.yml
trigger: none

resources:
  pipelines:
    - pipeline: build
      source: '{ApplicationName}-Build'
      trigger:
        branches:
          include:
            - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  - group: '{ApplicationName}-Production' # Variable group in Azure DevOps

stages:
  - stage: DeployToStaging
    displayName: 'Deploy to Staging'
    jobs:
      - deployment: DeployStaging
        displayName: 'Deploy to Staging Environment'
        environment: 'staging'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: DownloadBuildArtifacts@1
                  inputs:
                    buildType: 'specific'
                    project: '$(System.TeamProject)'
                    pipeline: '{ApplicationName}-Build'
                    buildVersionToDownload: 'latest'
                    downloadType: 'single'
                    artifactName: 'drop'
                    downloadPath: '$(System.ArtifactsDirectory)'

                - task: AzureWebApp@1
                  displayName: 'Deploy to Azure Web App'
                  inputs:
                    azureSubscription: '$(AzureSubscription)'
                    appType: 'webAppLinux'
                    appName: '$(StagingWebAppName)'
                    package: '$(System.ArtifactsDirectory)/drop/**/*.zip'

                - task: AzureCLI@2
                  displayName: 'Run Database Migrations'
                  inputs:
                    azureSubscription: '$(AzureSubscription)'
                    scriptType: 'bash'
                    scriptLocation: 'inlineScript'
                    inlineScript: |
                      # Install dotnet-ef
                      dotnet tool install --global dotnet-ef

                      # Run migrations
                      dotnet ef database update \
                        --project src/{ApplicationName}.Data \
                        --startup-project src/{ApplicationName}.Services.API \
                        --connection "$(StagingDbConnectionString)"

  - stage: DeployToProduction
    displayName: 'Deploy to Production'
    dependsOn: DeployToStaging
    condition: succeeded()
    jobs:
      - deployment: DeployProduction
        displayName: 'Deploy to Production Environment'
        environment: 'production'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: DownloadBuildArtifacts@1
                  inputs:
                    buildType: 'specific'
                    project: '$(System.TeamProject)'
                    pipeline: '{ApplicationName}-Build'
                    buildVersionToDownload: 'latest'
                    downloadType: 'single'
                    artifactName: 'drop'
                    downloadPath: '$(System.ArtifactsDirectory)'

                - task: AzureWebApp@1
                  displayName: 'Deploy to Azure Web App (Slot)'
                  inputs:
                    azureSubscription: '$(AzureSubscription)'
                    appType: 'webAppLinux'
                    appName: '$(ProductionWebAppName)'
                    deployToSlotOrASE: true
                    resourceGroupName: '$(ResourceGroupName)'
                    slotName: 'staging'
                    package: '$(System.ArtifactsDirectory)/drop/**/*.zip'

                - task: AzureAppServiceManage@0
                  displayName: 'Swap Staging to Production'
                  inputs:
                    azureSubscription: '$(AzureSubscription)'
                    action: 'Swap Slots'
                    webAppName: '$(ProductionWebAppName)'
                    resourceGroupName: '$(ResourceGroupName)'
                    sourceSlot: 'staging'
                    targetSlot: 'production'
```

## GitHub Actions

### Build Workflow
```yaml
# .github/workflows/build.yml
name: Build and Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  DOTNET_VERSION: '10.0.x'
  BUILD_CONFIGURATION: 'Release'

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v2

      - name: Setup .NET
        uses: actions/setup-dotnet@v2
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
          global-json-file: global.json

      - name: Restore dependencies
        run: dotnet restore

      - name: Build solution
        run: dotnet build --configuration ${{ env.BUILD_CONFIGURATION }} --no-restore

      - name: Run tests
        run: dotnet test --configuration ${{ env.BUILD_CONFIGURATION }} --no-build --logger trx --collect:"XPlat Code Coverage"

      - name: Upload coverage to Codecov
        uses: codecov/codecov-action@v2
        with:
          files: coverage.cobertura.xml

      - name: Publish application
        run: dotnet publish src/{ApplicationName}.Services.API/{ApplicationName}.Services.API.csproj --configuration ${{ env.BUILD_CONFIGURATION }} --output ./publish

      - name: Upload artifact
        uses: actions/upload-artifact@v2
        with:
          name: published-app
          path: ./publish
```

### Docker Build and Push
```yaml
# .github/workflows/docker.yml
name: Docker Build and Push

on:
  push:
    branches: [ main ]
    tags: [ 'v*.*.*' ]

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  build-and-push:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write

    steps:
      - name: Checkout code
        uses: actions/checkout@v2

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v1

      - name: Log in to Container Registry
        uses: docker/login-action@v1
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Extract metadata
        id: meta
        uses: docker/metadata-action@v5
        with:
          images: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}
          tags: |
            type=ref,event=branch
            type=semver,pattern={{version}}
            type=semver,pattern={{major}}.{{minor}}
            type=sha

      - name: Build and push Docker image
        uses: docker/build-push-action@v5
        with:
          context: .
          file: ./src/{ApplicationName}.Services.API/Dockerfile
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
          cache-from: type=gha
          cache-to: type=gha,mode=max

      - name: Scan image for vulnerabilities
        uses: aquasecurity/trivy-action@master
        with:
          image-ref: ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ steps.meta.outputs.version }}
          format: 'sarif'
          output: 'trivy-results.sarif'

      - name: Upload Trivy results to GitHub Security
        uses: github/codeql-action/upload-sarif@v1
        with:
          sarif_file: 'trivy-results.sarif'
```

## Infrastructure as Code (Bicep)

### Azure Resources
```bicep
// infrastructure/main.bicep
@description('The name of the application')
param applicationName string = '{applicationname}'

@description('The environment (dev, staging, prod)')
param environment string = 'dev'

@description('The location for resources')
param location string = resourceGroup().location

var uniqueSuffix = uniqueString(resourceGroup().id)
var appServicePlanName = '${applicationName}-plan-${environment}'
var webAppName = '${applicationName}-api-${environment}-${uniqueSuffix}'
var postgresServerName = '${applicationName}-postgres-${environment}-${uniqueSuffix}'
var redisName = '${applicationName}-redis-${environment}-${uniqueSuffix}'
var keyVaultName = '${applicationName}-kv-${environment}-${uniqueSuffix}'

// App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: environment == 'prod' ? 'P1v1' : 'B1'
    tier: environment == 'prod' ? 'Premiumv1' : 'Basic'
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

// Web App
resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webAppName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      healthCheckPath: '/health'
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment
        }
        {
          name: 'KeyVault__VaultUri'
          value: keyVault.properties.vaultUri
        }
      ]
    }
  }
}

// PostgreSQL Flexible Server
resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2023-03-01-preview' = {
  name: postgresServerName
  location: location
  sku: {
    name: environment == 'prod' ? 'Standard_D2s_v1' : 'Standard_B1ms'
    tier: environment == 'prod' ? 'GeneralPurpose' : 'Burstable'
  }
  properties: {
    version: '17'
    storage: {
      storageSizeGB: environment == 'prod' ? 128 : 32
    }
    backup: {
      backupRetentionDays: environment == 'prod' ? 30 : 7
      geoRedundantBackup: environment == 'prod' ? 'Enabled' : 'Disabled'
    }
    highAvailability: {
      mode: environment == 'prod' ? 'ZoneRedundant' : 'Disabled'
    }
  }
}

// Redis Cache
resource redis 'Microsoft.Cache/redis@2023-08-01' = {
  name: redisName
  location: location
  properties: {
    sku: {
      name: environment == 'prod' ? 'Standard' : 'Basic'
      family: 'C'
      capacity: environment == 'prod' ? 1 : 0
    }
    enableNonSslPort: false
    minimumTlsVersion: '1.2'
  }
}

// Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
  }
}

// Key Vault Access Policy for Web App
resource keyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: subscription().tenantId
        objectId: webApp.identity.principalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
  }
}

// Outputs
output webAppName string = webApp.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output postgresServerName string = postgresServer.name
output redisName string = redis.name
output keyVaultName string = keyVault.name
```

### Deploy with Bicep
```bash
# Create resource group
az group create --name rg-{applicationname}-prod --location eastus

# Deploy infrastructure
az deployment group create \
  --resource-group rg-{applicationname}-prod \
  --template-file infrastructure/main.bicep \
  --parameters applicationName={applicationname} environment=prod
```

## Secrets Management in Pipelines

### Azure Pipelines Variable Groups
```yaml
# Reference variable group
variables:
  - group: '{ApplicationName}-Secrets'

# Use secrets
steps:
  - task: AzureCLI@2
    inputs:
      scriptType: 'bash'
      scriptLocation: 'inlineScript'
      inlineScript: |
        echo "Connection String: $(DbConnectionString)"
    env:
      DB_CONNECTION: $(DbConnectionString)
```

### GitHub Actions Secrets
```yaml
steps:
  - name: Deploy
    env:
      DB_CONNECTION: ${{ secrets.DB_CONNECTION_STRING }}
      API_KEY: ${{ secrets.EXTERNAL_API_KEY }}
    run: |
      echo "Deploying with secrets"
```

## Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!)
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

## Common DevOps Pitfalls

### ❌ Avoid These Mistakes

1. **Secrets in Source Control**
   - ❌ Committing secrets to Git
   - ✅ Use pipeline variables or Key Vault

2. **No Automated Testing**
   - ❌ Deploying without running tests
   - ✅ Always run tests in pipeline

3. **Using :latest Tag**
   - ❌ `FROM mcr.microsoft.com/dotnet/sdk:latest`
   - ✅ `FROM mcr.microsoft.com/dotnet/sdk:10.0`

4. **No Rollback Strategy**
   - ❌ Direct deployment to production
   - ✅ Blue-green or slot deployment

5. **Large Docker Images**
   - ❌ Single-stage build with SDK in final image
   - ✅ Multi-stage build with runtime-only final image

6. **Running as Root**
   - ❌ Default root user in container
   - ✅ Create and use non-root user

## DevOps Checklist

### CI/CD Pipeline
- [ ] Build pipeline triggers on commits
- [ ] Automated tests run in pipeline
- [ ] Code coverage measured
- [ ] Artifacts published
- [ ] Deployment pipeline configured
- [ ] Environment variables managed
- [ ] Secrets secured in Key Vault or pipeline variables

### Docker
- [ ] Multi-stage Dockerfile
- [ ] .dockerignore configured
- [ ] Non-root user configured
- [ ] Health check implemented
- [ ] Image size optimized
- [ ] Security scanning configured

### Infrastructure
- [ ] Infrastructure as Code defined
- [ ] Resource naming consistent
- [ ] Tagging strategy implemented
- [ ] High availability for production
- [ ] Backup strategy defined
- [ ] Monitoring configured

### Security
- [ ] No secrets in source control
- [ ] HTTPS enforced
- [ ] Minimal container privileges
- [ ] Image vulnerability scanning
- [ ] Network security configured

## Checklist Before Completion

- [ ] CI/CD pipeline tested end-to-end
- [ ] Docker images build successfully
- [ ] Health checks functional
- [ ] Infrastructure deployed via IaC
- [ ] Secrets managed securely
- [ ] Automated tests passing
- [ ] Rollback strategy tested
- [ ] Documentation complete
- [ ] Monitoring and alerts configured
- [ ] Team trained on deployment process
