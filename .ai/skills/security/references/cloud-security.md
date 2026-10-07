# Cloud Security (Azure)
Azure security controls: identity, network isolation, key management and monitoring. Load this when hardening an Azure deployment.

### Azure Security Best Practices

```csharp
// File: infrastructure/security-baseline.bicep

// 1. Azure Security Center
resource securityCenter 'Microsoft.Security/pricings@2023-01-01' = {
  name: 'VirtualMachines'
  properties: {
    pricingTier: 'Standard'
  }
}

// 2. Azure Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: 'application-keyvault'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'premium'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
  }
}

// 3. Azure SQL Database with encryption
resource sqlServer 'Microsoft.Sql/servers@2023-02-01-preview' = {
  name: 'application-sqlserver'
  location: location
  properties: {
    administratorLogin: adminUsername
    administratorLoginPassword: adminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Disabled'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-02-01-preview' = {
  parent: sqlServer
  name: 'applicationdb'
  location: location
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
  }
  sku: {
    name: 'S0'
    tier: 'Standard'
  }
}

// Enable Transparent Data Encryption (TDE)
resource tde 'Microsoft.Sql/servers/databases/transparentDataEncryption@2023-02-01-preview' = {
  parent: sqlDatabase
  name: 'current'
  properties: {
    state: 'Enabled'
  }
}

// 4. Azure Monitor and Log Analytics
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'application-logs'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 90
  }
}

// 5. Azure Private Link
resource privateEndpoint 'Microsoft.Network/privateEndpoints@2023-04-01' = {
  name: 'sqlserver-private-endpoint'
  location: location
  properties: {
    subnet: {
      id: subnetId
    }
    privateLinkServiceConnections: [
      {
        name: 'sqlserver-connection'
        properties: {
          privateLinkServiceId: sqlServer.id
          groupIds: [
            'sqlServer'
          ]
        }
      }
    ]
  }
}
```

### Managed Identity for Azure Resources

```csharp
// File: {ApplicationName}.Services.API/Program.cs

using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Use Managed Identity to access Azure resources
var credential = new DefaultAzureCredential();

// Access Key Vault using Managed Identity
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["KeyVault:Url"]!),
    credential);

// Access Azure Storage using Managed Identity
builder.Services.AddSingleton<BlobServiceClient>(sp =>
{
    var storageUrl = builder.Configuration["Storage:Url"];
    return new BlobServiceClient(new Uri(storageUrl!), credential);
});

var app = builder.Build();
app.Run();
```

---

