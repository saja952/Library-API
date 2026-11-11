# Library Management API

A RESTful Library Management API built using ASP.NET Core
Web API, .NET 10, Entity Framework Core, and PostgreSQL.

The project follows Clean Architecture principles and
uses the Repository and Service patterns.

## Technologies

- C# / .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- xUnit
- NSubstitute
- Git and GitHub

## Architecture

The solution contains four main projects:

### Library.Domain
Contains the domain entities:
- Author
- Book

### Library.Application
Contains:
- DTOs
- Repository interfaces
- Service interfaces
- Business logic
- ServiceResult for error handling

### Library.Infrastructure
Contains:
- EF Core DbContext
- Repository implementations
- Database migrations
- PostgreSQL configuration

### Library.Api
Contains:
- BooksController
- AuthorsController
- Dependency Injection configuration
- HTTP endpoints

## API Endpoints

### Books

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/books | Get all books |
| GET | /api/books/{id} | Get book by ID |
| POST | /api/books | Create a book |
| PUT | /api/books/{id} | Update a book |
| DELETE | /api/books/{id} | Delete a book |

### Authors

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/authors | Get all authors |
| GET | /api/authors/{id} | Get author by ID |
| POST | /api/authors | Create an author |
| GET | /api/authors/{id}/books | Get books by author |

## Validation Rules

### Author
- Name is required.
- Maximum name length is 100 characters.

### Book
- Title is required.
- Maximum title length is 200 characters.
- ISBN must contain exactly 13 digits.
- ISBN must be unique.
- Published year must be between 1450 and the current year.
- Author must exist.

## HTTP Status Codes

- 200 OK
- 201 Created
- 204 No Content
- 400 Bad Request
- 404 Not Found
- 409 Conflict

## Getting Started

### Requirements

- .NET 10 SDK
- PostgreSQL
- Git

### Clone the Repository

git clone https://github.com/saja952/Library-API.git

cd Library-API

### Configure the Database

Create a PostgreSQL database named LibraryDb.

Configure the connection string using .NET User Secrets
from the Library.Api project directory.

Do not commit database passwords to GitHub.

### Apply Migrations

dotnet ef database update --project src/Library.Infrastructure --startup-project src/Library.Api

### Run the API

dotnet run --project src/Library.Api

## Unit Testing

Unit tests are written using xUnit and NSubstitute.

The tests cover:
- Book retrieval
- Book validation
- Duplicate ISBN handling
- Author existence
- Book update and deletion
- Author retrieval and creation

Run all tests:

dotnet test

## Design Patterns

- Clean Architecture
- Repository Pattern
- Service Pattern
- Dependency Injection
- DTO Pattern
- Asynchronous Programming
- Structured Logging
