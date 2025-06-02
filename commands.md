# Commands

## .NET EF Tools

```shell
dotnet tool list -g

dotnet tool install --global dotnet-ef
```

## EF Tools - Add Migrations

```shell
dotnet ef migrations add InitialCreate \
  --project Excuses.Persistence.EFCore \
  --startup-project Excuses.WebApi

dotnet ef migrations add InitialCreate \
  --project Excuses.Persistence.EFCore \
  --startup-project Excuses.WebApi     \
  --output-dir Infrastructure/Migrations

dotnet ef migrations add InitialCreate --project Excuses.Persistence.EFCore --startup-project Excuses.WebApi
```

## EF Tools - Update Database

```shell
dotnet ef database update \
  --project Excuses.Persistence.EFCore \
  --startup-project Excuses.WebApi

dotnet ef database update --project Excuses.Persistence.EFCore --startup-project Excuses.WebApi
```

## Docker - SQL Server

```shell
docker run --name excuses-sqlserver \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest

docker run --name excuses-sqlserver -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

```

## Docker - Azure Container Registry

```shell
docker build -f ./Excuses.WebApi/Dockerfile -t neurothrone/excuses-webapi .

az login
az acr login --name excusesregistry

docker tag neurothrone/excuses-webapi:latest excusesregistry.azurecr.io/neurothrone/excuses-webapi:latest

docker push excusesregistry.azurecr.io/neurothrone/excuses-webapi:latest
````

## Docker Compose

```shell
docker compose up -d
docker compose up -d --build

docker compose down
````

## Azure CLI - Create Service Principal

```shell
$subscription = "your-subscription"
$rg = "your-resource-group"
az ad sp create-for-rbac --name excuses-sp --role contributor --scopes /subscriptions/$subscription/resourceGroups/$rg --json-auth --output json
```