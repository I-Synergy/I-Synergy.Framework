# How To Delete Azure Devops Project Wiki Through CLI

To work with azure devops CLI, you need to install the extension on top of az cli. The following command will install the azure devops cli extension.

```powershell
$  az extension add --name azure-devops
```

You can run the following command to check the list of installed extensions.
```powershell
$ az extension list

[
  {
    "experimental": false,
    "extensionType": "whl",
    "name": "azure-devops",
    "path": "/home/dro/.azure/cliextensions/azure-devops",
    "preview": false,
    "version": "0.26.0"
  }
]
```
The following command can be used to access the help section.

```powershell
$ az devops -h         # Top level
$ az devops wiki -h    # Wiki help
```
Set the organization and project where you want to delete the wiki.

```powershell
$ az devops configure --defaults organization=https://dev.azure.com/{ORG-NAME} project={PROJECT-NAME}
```
Before running the azure devops related commands you should authenticate with azure either through az login or az devops login command.

```powershell
$ az login   # AAD/MSA authentication
$ az devops login # PAT token based authentication
```
Get the project wiki metadata. The output of the below command is the list of JSON objects.

```powershell
$ az devops wiki list 
```
The value in the id field is the repository ID. Using the repository ID I tried deleting the repo but again it throws the following error.

```powershell
$ az devops wiki delete --wiki {id}
Are you sure you want to delete this wiki? (y/n): y
Wiki delete operation is not supported on wikis of type 'ProjectWiki'.
```
The only solution is to delete the underlying git repository mapped to the wiki. The az devops wiki list command will also give you the repositoryId for the underlying git repository.

Run the following azure devops repos command to remove the git repository.

```powershell
$ az repos delete --id {repositoryId}
Are you sure you want to delete this repository? (y/n): y
Deleted repository {repositoryId}
```
To validate, you can navigate to the azure devops web UI wiki section. This time it will not prompt you with a new page the moment you click wiki.