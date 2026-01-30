# Developer Evaluation Project

`READ CAREFULLY`

## Instructions
**The test below will have up to 7 calendar days to be delivered from the date of receipt of this manual.**

- The code must be versioned in a public Github repository and a link must be sent for evaluation once completed
- Upload this template to your repository and start working from it
- Read the instructions carefully and make sure all requirements are being addressed
- The repository must provide instructions on how to configure, execute and test the project
- Documentation and overall organization will also be taken into consideration

## Use Case
**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

* Sale number
* Date when the sale was made
* Customer
* Total sale amount
* Branch where the sale was made
* Products
* Quantities
* Unit prices
* Discounts
* Total amount for each item
* Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:
* SaleCreated
* SaleModified
* SaleCancelled
* ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

* Purchases above 4 identical items have a 10% discount
* Purchases between 10 and 20 identical items have a 20% discount
* It's not possible to sell above 20 identical items
* Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:
   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview
This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates. 

See [Overview](/.doc/overview.md)

## Tech Stack
This section lists the key technologies used in the project, including the backend, testing, frontend, and database components. 

See [Tech Stack](/.doc/tech-stack.md)

## Frameworks
This section outlines the frameworks and libraries that are leveraged in the project to enhance development productivity and maintainability. 

See [Frameworks](/.doc/frameworks.md)

## API Structure
This section includes links to the detailed documentation for the different API resources:
- [API General](/.doc/general-api.md)
- [Products API](/.doc/products-api.md)
- [Carts API](/.doc/carts-api.md)
- [Sales API](/.doc/sales-api.md)
- [Users API](/.doc/users-api.md)
- [Auth API](/.doc/auth-api.md)

## Project Structure
This section describes the overall structure and organization of the project files and directories.

See [Project Structure](/.doc/project-structure.md)

## Getting Started

### Prerequisites
- [Docker](https://www.docker.com/) and Docker Compose
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (for the Angular frontend)
- Git

### 1. Clone the Repository
```bash
git clone <repository-url>
cd developer-evaluation
```

### 2. Start Infrastructure Services
The project requires PostgreSQL, MongoDB, and Redis. Start them using Docker Compose:

```bash
cd template/backend
docker-compose up -d ambev.developerevaluation.database ambev.developerevaluation.nosql ambev.developerevaluation.cache
```

This starts:
- **PostgreSQL 13** (port 5432): Main relational database
- **MongoDB 8.0** (port 27017): NoSQL database for event store
- **Redis 7.4.1** (port 6379): Cache layer

Default credentials:
| Service    | User        | Password     | Database/DB            |
|------------|-------------|--------------|------------------------|
| PostgreSQL | `developer` | `ev@luAt10n` | `developer_evaluation` |
| MongoDB    | `developer` | `ev@luAt10n` | `developer_evaluation` |
| Redis      | -           | `ev@luAt10n` | -                      |

### 3. Configure Connection Strings
Check the mapped ports from Docker:
```bash
docker-compose ps
```

Update `template/backend/src/Ambev.DeveloperEvaluation.WebApi/appsettings.Development.json` with the correct ports:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=<POSTGRES_PORT>;Database=developer_evaluation;Username=developer;Password=ev@luAt10n",
    "Redis": "localhost:<REDIS_PORT>,password=ev@luAt10n",
    "MongoDB": "mongodb://developer:ev%40luAt10n@localhost:<MONGO_PORT>/developer_evaluation?authSource=admin"
  }
}
```

### 4. Apply Database Migrations
```bash
cd template/backend
dotnet ef database update --startup-project src/Ambev.DeveloperEvaluation.WebApi --project src/Ambev.DeveloperEvaluation.ORM
```

### 5. Run the API
```bash
cd template/backend/src/Ambev.DeveloperEvaluation.WebApi
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5119`
- HTTPS: `https://localhost:7181`

### 6. Access Swagger
Open your browser and navigate to:
```
http://localhost:5119/swagger
```

Swagger provides an interactive UI to test all API endpoints. Use the "Authorize" button to insert a JWT token for authenticated requests.

### 7. Running Tests

```bash
cd template/backend

# Run all tests
dotnet test

# Run only unit tests
dotnet test tests/Ambev.DeveloperEvaluation.Unit

# Run only integration tests
dotnet test tests/Ambev.DeveloperEvaluation.Integration
```

### 8. Frontend (Angular)
```bash
cd template/frontend
npm install
ng serve
```
The frontend will be available at `http://localhost:4200`.