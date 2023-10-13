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

Add Nugget source locations

`dotnet nuget add source C:\Projects\Microservices\dotnetacademy\my-microservices-project\packages`

## docker

Run mongo container

`docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo`

Run docker compose

`docker-compose up`
