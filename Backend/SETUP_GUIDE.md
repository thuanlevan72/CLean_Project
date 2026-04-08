# Todo App Backend - Setup Guide

## What Was Implemented

A complete .NET Core 10 Web API backend with PostgreSQL database support for the Todo App.

### Project Structure

```
Backend/TodoApi/
├── Controllers/
│   └── TodoController.cs          # REST API endpoints
├── Data/
│   └── AppDbContext.cs            # Entity Framework DbContext
├── DTOs/
│   ├── CreateTodoDto.cs           # DTO for creating todos
│   ├── UpdateTodoDto.cs           # DTO for updating todos
│   └── TodoDto.cs                 # DTO for todo responses
├── Models/
│   └── Todo.cs                    # Todo entity model
├── Repositories/
│   ├── ITodoRepository.cs         # Repository interface
│   └── TodoRepository.cs          # Repository implementation
├── Services/
│   ├── ITodoService.cs            # Service interface
│   └── TodoService.cs             # Service implementation
├── Program.cs                     # Application configuration
├── appsettings.json               # Configuration with PostgreSQL connection
└── TodoApi.csproj                 # Project file with dependencies
```

### API Endpoints

| Method | Endpoint                | Description            |
| ------ | ----------------------- | ---------------------- |
| GET    | `/api/todo`             | Get all todos          |
| GET    | `/api/todo/{id}`        | Get todo by ID         |
| POST   | `/api/todo`             | Create new todo        |
| PUT    | `/api/todo/{id}`        | Update todo            |
| DELETE | `/api/todo/{id}`        | Delete todo            |
| PATCH  | `/api/todo/{id}/toggle` | Toggle todo completion |

### Dependencies Installed

- **Npgsql.EntityFrameworkCore.PostgreSQL** - PostgreSQL provider for Entity Framework Core
- **Microsoft.EntityFrameworkCore.Design** - EF Core design-time tools
- **Microsoft.EntityFrameworkCore.Tools** - EF Core CLI tools
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI documentation

## Prerequisites

1. **.NET 10 SDK** - Already installed (version 10.0.102)
2. **PostgreSQL** - Must be installed and running

## Database Setup

### 1. Create PostgreSQL Database

Connect to PostgreSQL and create the database:

```sql
CREATE DATABASE todo_db;
```

### 2. Update Connection String

Edit [`Backend/TodoApi/appsettings.json`](Backend/TodoApi/appsettings.json) and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=todo_db;Username=postgres;Password=YOUR_ACTUAL_PASSWORD"
  }
}
```

Replace `YOUR_ACTUAL_PASSWORD` with your PostgreSQL password.

### 3. Create Database Migration

Run the following commands to create and apply the database migration:

```bash
cd Backend/TodoApi
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This will create the `Todos` table with the following schema:

```sql
CREATE TABLE "Todos" (
    "Id" SERIAL PRIMARY KEY,
    "Text" VARCHAR(500) NOT NULL,
    "Completed" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NULL
);
```

## Running the Backend

### Option 1: Using dotnet run

```bash
cd Backend/TodoApi
dotnet run
```

The API will be available at:

- **HTTPS**: https://localhost:7000
- **HTTP**: http://localhost:5000

### Option 2: Using dotnet watch (for development)

```bash
cd Backend/TodoApi
dotnet watch run
```

This will automatically reload the application when code changes are detected.

## Testing the API

### Using Swagger UI

Once the application is running, open your browser and navigate to:

```
https://localhost:7000/swagger
```

This will open the Swagger UI where you can test all API endpoints interactively.

### Using curl

**Get all todos:**

```bash
curl https://localhost:7000/api/todo
```

**Create a todo:**

```bash
curl -X POST https://localhost:7000/api/todo \
  -H "Content-Type: application/json" \
  -d '{"text": "Buy groceries"}'
```

**Toggle todo completion:**

```bash
curl -X PATCH https://localhost:7000/api/todo/1/toggle
```

**Delete a todo:**

```bash
curl -X DELETE https://localhost:7000/api/todo/1
```

## Frontend Integration

The backend is configured to accept requests from `http://localhost:3000` (the React frontend).

### Update Frontend API Calls

Replace the local state management in [`Frontend/src/App.js`](Frontend/src/App.js) with API calls:

```javascript
const API_BASE = "http://localhost:5000/api";

// Fetch all todos
const fetchTodos = async () => {
  const response = await fetch(`${API_BASE}/todo`);
  const data = await response.json();
  setTodos(data);
};

// Create todo
const addTodo = async (text) => {
  const response = await fetch(`${API_BASE}/todo`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ text }),
  });
  const newTodo = await response.json();
  setTodos([...todos, newTodo]);
};

// Toggle todo
const toggleTodo = async (id) => {
  const response = await fetch(`${API_BASE}/todo/${id}/toggle`, {
    method: "PATCH",
  });
  const updatedTodo = await response.json();
  setTodos(todos.map((todo) => (todo.id === id ? updatedTodo : todo)));
};

// Delete todo
const deleteTodo = async (id) => {
  await fetch(`${API_BASE}/todo/${id}`, {
    method: "DELETE",
  });
  setTodos(todos.filter((todo) => todo.id !== id));
};
```

## CORS Configuration

The backend is configured to allow requests from:

- `http://localhost:3000` (React development server)

If you need to allow other origins, update the CORS policy in [`Backend/TodoApi/Program.cs`](Backend/TodoApi/Program.cs):

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://your-production-domain.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

## Troubleshooting

### Database Connection Issues

1. **Error: "Connection refused"**
   - Ensure PostgreSQL is running
   - Check the connection string in `appsettings.json`
   - Verify PostgreSQL is listening on the correct port (default: 5432)

2. **Error: "Database does not exist"**
   - Create the database: `CREATE DATABASE todo_db;`

3. **Error: "Authentication failed"**
   - Check username and password in connection string
   - Verify PostgreSQL user has access to the database

### Build Errors

1. **Error: "Missing reference"**
   - Run `dotnet restore` to restore NuGet packages
   - Run `dotnet build` to rebuild the project

2. **Error: "Migration already exists"**
   - Run `dotnet ef migrations remove` to remove the last migration
   - Then run `dotnet ef migrations add InitialCreate` again

### CORS Issues

If the frontend cannot connect to the backend:

1. Check that the backend is running
2. Verify the CORS policy allows the frontend origin
3. Check browser console for CORS errors
4. Ensure the frontend is using the correct API URL

## Development Workflow

1. **Start PostgreSQL** (if not already running)
2. **Run the backend**:
   ```bash
   cd Backend/TodoApi
   dotnet watch run
   ```
3. **Run the frontend** (in a separate terminal):
   ```bash
   cd Frontend
   npm start
   ```
4. **Test the integration** by creating, toggling, and deleting todos in the frontend

## Production Deployment

For production deployment, consider:

1. **Use environment variables** for connection strings
2. **Enable HTTPS** with proper SSL certificates
3. **Configure proper CORS** origins
4. **Set up logging** to a file or external service
5. **Use connection pooling** for database connections
6. **Consider Docker** containerization for easier deployment

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Npgsql Documentation](https://www.npgsql.org/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
