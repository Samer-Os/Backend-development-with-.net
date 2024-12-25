# Enhanced User Management API

A robust REST API for user management with comprehensive validation, authentication, and logging capabilities.

## Features

- Complete CRUD operations for user management
- Input validation using Data Annotations
- Custom API response wrapper
- Secure password hashing
- API key authentication
- Comprehensive logging with Serilog
- Swagger documentation
- Soft delete functionality

## Getting Started

1. Clone the repository
```bash
git clone https://github.com/yourusername/user-management-api.git
```

2. Install dependencies
```bash
dotnet restore
```

3. Run the application
```bash
dotnet run
```

## API Endpoints

- GET /api/User - Get all active users
- GET /api/User/{id} - Get user by ID
- POST /api/User - Create new user
- PUT /api/User/{id} - Update existing user
- DELETE /api/User/{id} - Soft delete user

## Authentication

Include the X-API-Key header in all requests:
```
X-API-Key: your-secret-api-key
```

## Development Notes

- This project was developed and debugged using GitHub Copilot
- Includes comprehensive error handling and logging
- Uses structured logging with Serilog
- Implements middleware for authentication and request logging

## Validation

The API implements comprehensive validation including:
- Required fields
- Email format validation
- Username length requirements
- Password complexity requirements
- Duplicate email/username checking

## Middleware

- LoggingMiddleware: Logs request/response details with timing
- AuthenticationMiddleware: Implements API key authentication

---

Made with ❤️ using .NET 6 and GitHub Copilot