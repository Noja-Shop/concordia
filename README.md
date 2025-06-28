# Noja Group Buying E-Commerce API

Noja API built with .NET 9, implementing Clean Architecture principles and SOLID principles.

#### Architecture Overview
This solution follows Clean Architecture with clear separation of concerns:

src/
- Noja.API/              # Presentation Layer (Controllers, Endpoints)
- Noja.Application/      # Application Layer (Services, DTOs, Business Logic)
- Noja.Core/            # Domain Layer (Entities, Interfaces, Enums)
- Noja.Infrastructure/   # Infrastructure Layer (Data, Authentication, External Services)

## Key Architectural Patterns
- Clean Architecture - Dependency inversion and separation of concerns
- Repository Pattern - Data access abstraction
- Service Layer Pattern - Business logic encapsulation
- DTO Pattern - Data transfer and API contracts
- Dependency Injection - Loose coupling and testability

#### Features
###### Multi-user
* Customers - Browse and purchase products
* Admins - Platform management and oversight
* User registration and authentication
* User profile management


###### Authentication & Authorization
* JWT-based authentication
* Role-based access control
* Token blacklisting

###### Product Management
* Product CRUD operations
* Product categories
* Stock management
* Admin-only product creation/modification

#### Stack 
* .NET 9
* ASP.NET Core Web API - RESTful API framework
* Entity Framework Core - ORM with PostgreSQL(Primary database)
* ASP.NET Core Identity - User management
* JWT Bearer Authentication - Stateless authentication  

### Quick Start
Make sure you have PostgreSQL, .NET 9 and Git.

#### Installation
1. Clone the repository
Create a folder then clone the repo git clone https://github.com/Noja-Shop/concordia.git cd src

2. Setup Database
create a postgresql database createdb NojaDB

3. Configure Connection String

// src/Noja.API/appsettings.json
{
  "ConnectionStrings": {
    "DbConnectionString": "Host=localhost;Database=noja_db;Username=your_user;Password=your_password"
  },
  "Jwt": {
    "Secret": "your-super-secret-key-here-minimum-32-characters",
    "ValidIssuer": "NojaAPI",
    "ValidAudience": "NojaClient"
  }
}

4. Run Migrations
dotnet ef migrations add [named_migration] --project ../Noja.Infrastructure --startup-project ../Noja.API

dotnet ef database update

5. Start the Application
dotnet run

6. Access API Documentation
* https://localhost:5129/scalar - You'll have to check it at the terminal, it might be different

### Project Structure
src/

*Noja.API/
-Controllers/           # API Controllers
-Endpoints/            # Route definitions
-Program.cs            # Application entry point

*Noja.Application/
-Models/               # DTOs and ViewModels
-Services/             # Business logic
-ApplicationDI.cs      # DI registration

*Noja.Core/
-Entity/               # Domain entities
-Enums/               # Domain enums
-Interfaces/          # Contracts

*Noja.Infrastructure/
-Data/                # DbContext and configurations
-Authentication/      # JWT implementation
-Repositories/        # Data access
-InfrastructureDI.cs  # DI registration

#### API Response Format
All API Endpoints return a consistent response format:
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { /* Response data */ },
  "errors": null
}


