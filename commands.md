# Commands

## dotnet

Create web api project

`dotnet new webapi -n <name>`

Create class library project

`dotnet new classlib -n <name>`

Add Nuget package to a project

`dotnet add package <name>`

Create Nuget package

`dotnet pack -o <output path>`

Create Nuget package with version

`dotnet pack -p:PackageVersion=1.0.0 -o <output path>`

`dotnet pack -p:PackageVersion=1.0.0 -o C:\Projects\Microservices\my-microservices-project\packages`

Add Nugget source locations

`dotnet nuget add source C:\Projects\Microservices\dotnetacademy\my-microservices-project\packages`

Dotnet code generator

`dotnet tool install -g dotnet-aspnet-codegenerator --version 7.0.0`

Dotnet code generator for identity
`dotnet aspnet-codegenerator identity --files "Account.Register"`

## docker

Run mongo container

`docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo`

Run docker compose

`docker-compose up`
