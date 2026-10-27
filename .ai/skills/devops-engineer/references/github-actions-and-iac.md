# GitHub Actions & Infrastructure as Code
GitHub Actions workflows and Bicep infrastructure templates. Load this when writing a CI/CD workflow or provisioning Azure resources as code.

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

