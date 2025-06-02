# Excuses API - Cloud-Based .NET Application

This project is a backend excuse generator built with modern .NET technologies, EF Core, Azure SQL, Azure Functions, and
Docker.

## About the Application

The application serves excuses via a RESTful Web API and Azure Functions, backed by a SQL Server database. It is
containerized and supports both local development and cloud deployment via Docker Compose and Azure App Services.

Functionality includes:

- A Web API built with .NET and Minimal APIs
- A SQL Server database storing excuses
- An Azure Function with full CRUD access to the same database
- Docker support for both the API and database
- GitHub Actions for CI/CD to Azure Container Registry

## Tech Stack & Tools

- **Framework:** .NET 8 (Minimal API + Azure Functions)
- **Database:** SQL Server (local via Docker and cloud via Azure SQL)
- **ORM:** Entity Framework Core
- **Containerization:** Docker, Docker Compose
- **Cloud:** Azure App Service, Azure SQL, Azure Container Registry
- **CI/CD:** GitHub Actions
- **Other:** Visual Studio, Postman/.http files, Azure CLI

## Run & Test the Application with Docker

### 1. Clone the Repository

```bash
git clone https://github.com/neurothrone/azure-excuses
cd azure-excuses
```

### 2. Start SQL Server Locally

```bash
docker run --name excuses-sqlserver \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Run Docker Compose

```bash
docker compose up -d --build
```

To stop:

```bash
docker compose down
```

## EF Core Migrations

### Add Migrations

```bash
dotnet ef migrations add InitialCreate \
  --project Excuses.Persistence.EFCore \
  --startup-project Excuses.WebApi
```

### Update the Database

```bash
dotnet ef database update \
  --project Excuses.Persistence.EFCore \
  --startup-project Excuses.WebApi
```

## Docker & Azure Deployment

### Build and Push Container Image

```bash
docker build -f ./Excuses.WebApi/Dockerfile -t neurothrone/excuses-webapi .

docker tag neurothrone/excuses-webapi:latest \
  excusesregistry.azurecr.io/neurothrone/excuses-webapi:latest

docker push excusesregistry.azurecr.io/neurothrone/excuses-webapi:latest
```

### Azure CLI: Create Service Principal

```bash
$subscription = "your-subscription"
$rg = "your-resource-group"

az ad sp create-for-rbac \
  --name excuses-sp \
  --role contributor \
  --scopes /subscriptions/$subscription/resourceGroups/$rg \
  --json-auth \
  --output json
```

## Infrastructure Considerations: Containers vs Virtual Machines

When deploying backend applications like this one, a common decision is whether to use lightweight containers or
traditional virtual machines (VMs). This project was developed using Docker containers, which offer several advantages
in terms of speed, efficiency, and cost.

### Cost and Capacity Comparison

Using Azure as a baseline:

- A typical Azure Linux VM with sufficient resources for a web API and database might cost around 600–900 SEK/month per
  instance depending on vCPU, RAM, and disk.
- In contrast, running a containerized setup with Azure App Service and Azure SQL can reduce costs significantly. Basic
  tiers for container hosting and a small Azure SQL instance cost approximately 350–500 SEK/month in total.

Containers are more resource-efficient because they share the host OS and are quicker to start, scale, and stop. This
efficiency leads to lower infrastructure overhead and better utilization compared to VMs.

### Pros and Cons

| Feature             | Containers                              | Virtual Machines                           |
|---------------------|-----------------------------------------|--------------------------------------------|
| Boot Time           | Seconds                                 | Minutes                                    |
| Resource Efficiency | High (shared OS kernel)                 | Lower (dedicated OS)                       |
| Isolation           | Strong (but shares kernel)              | Full OS-level isolation                    |
| Portability         | Very high (runs anywhere with Docker)   | More complex (image size, OS dependencies) |
| Complexity          | Requires Docker and orchestration tools | Simpler with classic VM management         |

For most modern backend services, containers provide a better developer experience and lower operational costs.

## Storage Considerations: Hot and Archived Data

In this project, excuses are stored in a SQL Server database. Assuming a storage volume of 25 GB, with data being
"hot" (frequently accessed) for two weeks and then archived for six months, storage costs become an important factor.

### Azure Storage Strategy

- **Hot Phase (2 weeks)**: Data can reside in an Azure SQL Database (Basic or S0 tier), optimized for frequent
  reads/writes.
    - Estimated cost: ~50–100 SEK/month depending on DTUs and region.
- **Archived Phase (6 months)**: Data can be exported and stored in Azure Blob Storage (Cool or Archive tier).
    - Estimated cost: ~5–10 SEK/month for 25 GB in the Cool tier or even less in the Archive tier.

### Comparison with Other Providers

To arrive at a more precise estimation of storage costs and usage across cloud providers, we compare hot and archived
data storage for 25 GB, assuming the data is hot for 2 weeks and archived for 6 months. Prices are based on regions
closest to Sweden (e.g., Stockholm, Sweden Central, Finland).

| Provider  | Hot Storage (DB, 2 weeks)                                     | Archived Storage (6 months)                  | Total Monthly Avg (approx.) |
|-----------|---------------------------------------------------------------|----------------------------------------------|-----------------------------|
| **Azure** | Azure SQL Database Basic Tier (~65 SEK/month)                 | Blob Storage Archive Tier (~3 SEK/month)     | ~68 SEK/month               |
| **AWS**   | Amazon RDS (SQL Server, db.t3.micro + 25 GB) (~120 SEK/month) | S3 Glacier Flexible Retrieval (~4 SEK/month) | ~124 SEK/month              |
| **GCP**   | Cloud SQL (SQL Server, db-f1-micro + 25 GB) (~95 SEK/month)   | Cloud Storage Archive (~3 SEK/month)         | ~98 SEK/month               |

- **Azure** is the most cost-effective if Blob Archive storage is used after the hot phase.
- **AWS** is typically more expensive due to the higher base cost of RDS for SQL Server.
- **GCP** is priced somewhere in between and benefits from competitive archive-tier pricing.

All providers offer automatic lifecycle rules to move data from hot to cold storage, reducing manual maintenance.
However, the transition is not always instantaneous and may involve delays or retrieval fees when accessing archive
data.

In summary, for workloads like this where data is actively used for a short period, then archived Azure offers the
lowest monthly cost, especially if combining Azure SQL with Archive Blob storage.
