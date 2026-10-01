# Examination Management API

This project is an ASP.NET Core Web API for managing academic courses, exam marks, and result calculation using SQL Server and ADO.NET.

## Project version and technology

- .NET SDK / Target Framework: .NET 10.0
- ASP.NET Core: 10.0
- Database: SQL Server
- Data access: Microsoft.Data.SqlClient 7.0.1
- OpenAPI UI: Scalar.AspNetCore
- Project type: ASP.NET Core Web API

## Main features

- Course CRUD operations
- Student exam mark entry
- Result calculation with pass/fail and grade logic
- Pagination for course listing
- Exception handling middleware
- Dependency injection for services
- Soft delete pattern for mark cancellation

## Project structure

- Controllers/ - API endpoints
- Services/ - Business logic and SQL logic
- Interfaces/ - Contracts for services
- Models/ - Request and response models
- Middleware/ - Custom exception handling
- Properties/ - Launch settings
- Program.cs - application startup and dependency registration

## Prerequisites

Before running the project, make sure you have:

- .NET 10 SDK installed
- SQL Server running locally or on a reachable server
- SQL Server Management Studio or Azure Data Studio for database setup
- Administrator or DB user access to create tables and insert sample data

## Required configuration file

This project reads the SQL Server connection string from appsettings.json.

Create a file named appsettings.json in the project root with the following structure:

```json
{
  "ConnectionStrings": {
    "ExamDb": "Server=localhost;Database=ExamDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
  }
}
```

If you are using Windows authentication, use this instead:

```json
{
  "ConnectionStrings": {
    "ExamDb": "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

Important:
- The application specifically looks for the key name ExamDb.
- If the file is missing or the key is incorrect, the application will fail at startup.

## Database setup instructions

Use the SQL script provided in the sample file named:

sample table and insert query.txt

This file contains the table creation script and sample dummy data for the app.

### Table creation script

```sql
CREATE TABLE TEST_ACD_COURSE (
    COURSENO INT NOT NULL PRIMARY KEY,
    COURSECODE NVARCHAR(50) NOT NULL,
    COURSENAME NVARCHAR(200) NOT NULL,
    MININTERNAL DECIMAL(18,2) NULL,
    MINEXTERNAL DECIMAL(18,2) NULL,
    MINTOTAL DECIMAL(18,2) NULL
);

CREATE TABLE ACD_STUD_REST_MARK (
    IDNO INT NOT NULL,
    SESSIONNO INT NOT NULL,
    COURSENO INT NOT NULL,
    SEMESTERNO INT NOT NULL,
    INTERNAL DECIMAL(18,2) NULL,
    [EXTERNAL] DECIMAL(18,2) NULL,
    TOTALMARK DECIMAL(18,2) NULL,
    PASSFAIL NVARCHAR(20) NULL,
    GRADE NVARCHAR(10) NULL,
    CANCEL BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_ACD_STUD_REST_MARK PRIMARY KEY (IDNO, SESSIONNO, COURSENO, SEMESTERNO)
);
```

### Sample dummy data

```sql
INSERT INTO TEST_ACD_COURSE (COURSENO, COURSECODE, COURSENAME, MININTERNAL, MINEXTERNAL, MINTOTAL)
VALUES
(101, 'CSE101', 'Computer Fundamentals', 20.00, 30.00, 50.00),
(102, 'CSE102', 'Database Systems', 20.00, 30.00, 50.00),
(103, 'CSE103', 'Web Programming', 20.00, 30.00, 50.00),
(104, 'CSE104', 'Operating Systems', 20.00, 30.00, 50.00);

INSERT INTO ACD_STUD_REST_MARK (IDNO, SESSIONNO, COURSENO, SEMESTERNO, INTERNAL, [EXTERNAL], TOTALMARK, PASSFAIL, GRADE, CANCEL)
VALUES
(1, 2024, 101, 1, 18.00, 35.00, 53.00, 'Pass', 'P', 0),
(1, 2024, 102, 1, 22.00, 28.00, 50.00, 'Pass', 'P', 0),
(2, 2024, 101, 1, 15.00, 25.00, 40.00, 'Fail', 'F', 0),
(2, 2024, 103, 1, 21.00, 32.00, 53.00, 'Pass', 'P', 0);
```

## Run the project

Open a terminal in the project root and run:

```powershell
dotnet restore
dotnet build
dotnet run
```

The API will start and run on a local ASP.NET Core port, typically:

- http://localhost:5159
- or the port configured in launchSettings.json

## Check the app

You can test the API using browser or curl.

### Quick health check

```powershell
curl http://localhost:5159/api/exam
```

### Database connectivity check

```powershell
curl http://localhost:5159/api/exam/ping-db
```

## API endpoints

### Course endpoints

- GET /api/course
- GET /api/course/{courseno}
- POST /api/course
- PUT /api/course/{courseno}
- DELETE /api/course/{courseno}

### Exam endpoints

- GET /api/exam
- GET /api/exam/hello
- GET /api/exam/ping-db
- GET /api/exam/studentresult
- POST /api/exam/mark-entry
- PUT /api/exam/mark-entry/{idno}/{courseno}/{sessionno}
- DELETE /api/exam/mark-entry/{idno}/{courseno}/{sessionno}

### Exam result endpoints

- GET /api/examresult
- POST /api/examresult/calculate

## Notes for use

- The project uses raw SQL and does not use Entity Framework Core.
- Table names must match exactly the names used in the SQL queries.
- The column [EXTERNAL] is wrapped in brackets because EXTERNAL is a reserved SQL Server keyword.
- Make sure the database and user credentials in appsettings.json are valid before running the app.

## Developer note

This project is intended as a backend demonstration for examination management logic and can be used as a learning base for building more complete academic ERP modules.
