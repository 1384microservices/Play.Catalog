# Play Catalog
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

### Clone
Create a project folder on your box. **D:\Projects\Play Economy** is a good idea but you can choose whatever fits your needs.

For Windows boxes you have to issue this command in a powershell window: `New-Item -ItemType Directory -Path 'D:\Projects\Play Economy'`. Switch to that directory: `Set-Locatin -Path 'D:\Projects\Play Economy'`. Clone this repository to your box: `https://github.com/1384microservices/Play.Catalog.git`.

### Run database storage
To get up and running you need a MongoDB database. We'll use a containerized deployment for this by issuing this command: `docker run --rm -d --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo`

### Run service
Within your service repository root folder (ie `D:\Projects\Play Economy`) start the service by issuing `dotnet run --Project .\src\Play.Catalog.Service\Play.Catalog.Service.csproj`

### Publish changes



[^wsl]:[You need to have WSL upfront](https://learn.microsoft.com/en-us/windows/wsl/)