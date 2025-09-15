# ConfigChangeTracker API

## Description
This API was created to create, modify, and verify the correctness of business logic in the domain of configuration rules.  
It supports operations to create (POST), update (PUT), retrieve by ID (GET), and list or filter changes.  
The API ensures proper validation and logging for all operations.

## Used technologies
- .NET 8  
- C#  
- ASP.NET Core Web API  
- In-memory persistence (no database)  
- Serilog for logging  
- Swagger / OpenAPI for documentation  
- Postman for testing  
- xUnit for unit and integration tests

## Setup / How to Run
1. Open the solution in Visual Studio.  
2. Restore NuGet packages.  
3. Run the project (F5) – the API will start on `http://localhost:{port}`.  
4. Use Swagger at `http://localhost:{port}/swagger` to test endpoints interactively.  
5. Alternatively, use Postman to send HTTP requests.

## Endpoints
- **POST** `/api/configchange` – Create a new configuration change.  
- **PUT** `/api/configchange/{id}` – Update an existing configuration change.  
- **GET** `/api/configchange/{id}` – Retrieve a specific change by ID.  
- **GET** `/api/configchange` – Retrieve all configuration changes.  
- **GET** `/api/configchange/list?type={type}&from={from}&to={to}` – Retrieve filtered list of changes.  
- **DELETE** `/api/configchange/{id}` – Delete a configuration change.  
- **GET** `/api/health` – Health check endpoint, returns API status.

## Validation & Logging
- Input validation is performed using model annotations and `ModelState`.  
- Invalid requests return `400 Bad Request` with error details.  
- Critical changes trigger warnings in logs.  
- Logging system was implemented using Serilog, outputting both to console and to `Logs/app-.txt`.
- Logging system records all critical changes, warnings, and information messages, 
- so it allows us to trace any issues or unexpected behavior and understand the root cause of problems.
- Correctness of the API was verified using Postman, by sending various POST, PUT, GET, and DELETE requests.

## Postman Collection
- A Postman collection named REST API is provided for testing all API endpoints.  
- You can import the file `REST_API.postman_collection.json` into Postman to quickly send requests and verify functionality.

### Using Base URL Variable
Since each developer might run the API on a different local port, we use a Postman environment variable `{{baseUrl}}` for all requests.

#### Steps:
1. Import the Postman collection (`REST_API.postman_collection.json`).  
2. Create a new environment in Postman, e.g., named **Local**.  
3. Add a variable:  
   - **Name:** `baseUrl`  
   - **Value:** `http://localhost:5258` (or whatever port your Visual Studio project is running on)  
4. Select the **Local** environment in Postman.  
5. All requests in the collection use `{{baseUrl}}` in the URL, for example: {{baseUrl}}/api/configchange
6. You can now run any request regardless of the port your API is using.
 
## Tests
- Unit and integration tests are implemented using xUnit.  
- Tests cover CRUD operations and validation scenarios.

## Assumptions & Decisions
- In-memory storage is used instead of a database for simplicity.  
- `ConfigChange.Id` is generated automatically when creating new changes.  
- Logging is considered a form of “external integration” for monitoring critical changes.  
- An interface (`IConfigChangeStorage`) was used to keep the code cleaner and more maintainable.  
  This approach makes it easier to add additional controllers in the future or change the storage implementation without affecting the controllers.  
- Storage logic is separated from the controllers: the interface defines the methods, the storage class implements them, and controllers only handle the request logic.

## Examples
- Creating a new change:
```json
{
  "id": "6f9619ff-8b86-d011-b42d-00cf4fc964ff",
  "ruleName": "new_change",
  "changeType": "add",
  "isCritical": true,
  "changedAt": "2025-09-15T11:57:58.160Z"
}