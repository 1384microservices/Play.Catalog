# Play Economy Catalog service
Play economy game items catalog service.

## About
This service implements items catalog service REST api.
It is built only for playground and this code should not be used in production.

### Endpoints

### Architecture

## Run

## Contribute
### Prerequisites
* Install [winget](https://learn.microsoft.com/en-us/windows/package-manager/winget/)
* Install git: `winget install --id Git.Git --source winget`
* Install dotnet 6 (or greater) SDK: `winget install --d Microsoft.DotNet.SDK.6`
* Install docker[^wsl]: `winget install --id Docker.DockerDesktop`
* Install visual studio code: `winget install --id VisualStudioCode --source winget`

### Build development infrastructure.
This service needs some infrastructure services like MongoDB to run. The infrastructure stack is already configured at [Play.Infrastructure](https://github.com/1384microservices/Play.Infrastructure) repository.

### Install Play.Common
This services needs Play.Common nuget package. To make it available on your machine run first instructions from [Play.Common](https://github.com/1384microservices/Play.Common) repository.

### Clone source
Create a project folder on your box. **D:\Projects\Play Economy** is a good idea but you can choose whatever fits your needs. For Windows boxes you have to issue this command in a powershell window: `New-Item -ItemType Directory -Path 'D:\Projects\Play Economy'`. Switch to this directory: `Set-Locatin -Path 'D:\Projects\Play Economy'`. 

Clone this repository to your box: `git clone https://github.com/1384microservices/Play.Catalog.git`.

### Run service
Within your service repository root folder (ie `D:\Projects\Play Economy`) start the service by issuing `dotnet run --Project .\src\Play.Catalog.Service\Play.Catalog.Service.csproj`

### Pack and publish contracts
```powershell
# Change with your package version.
$version="1.0.2"

# Change with your organisation name.
$owner="1384microservices"

# Change with your GitHub Personal Access Token
$gh_pat="[type your PAT here]"

# Change with your repository URL
$repositoryUrl="https://github.com/$owner/Play.Catalog"

# Build package
dotnet pack .\src\Play.Catalog.Contracts\ --configuration Release -p:PackageVersion=$version -p:RepositoryUrl=$repositoryUrl -o ..\packages\

# Publish package
dotnet nuget push ..\packages\Play.Catalog.Contracts.$version.nupkg --api-key $gh_pat --source "github"

```



[^wsl]:[You need to have WSL upfront](https://learn.microsoft.com/en-us/windows/wsl/)