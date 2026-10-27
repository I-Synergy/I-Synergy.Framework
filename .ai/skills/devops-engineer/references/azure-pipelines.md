# Azure Pipelines (YAML)
azure-pipelines.yml build, test and deploy stages, service connections, and pipeline variables. Load this when writing or reviewing an Azure DevOps pipeline.

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

