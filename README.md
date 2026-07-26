# Exam API Demo

ASP.NET Core Web API project demonstrating RESTful API development
with SQL Server, built as part of a transition from ASP.NET Web Forms
to modern .NET development.

## Tech Stack
- ASP.NET Core Web API (.NET 10)
- SQL Server / ADO.NET
- Scalar (OpenAPI)

## Features
- Full CRUD APIs for Course and Exam Mark management
- Exam result calculation with component-level pass/fail logic
- Dependency injection with service layer pattern
- Global exception handling middleware
- Pagination support
- Soft delete pattern for exam records

## Project Structure
- Controllers/ — API controllers (ExamController, CourseController, ExamResultController)
- Services/ — Business logic and DB access layer
- Interfaces/ — Service contracts
- Models/ — Request/response models
- Middleware/ — Global exception handling

## API Endpoints
### Course
- GET /api/course — Get all courses (paginated)
- GET /api/course/{courseno} — Get course by ID
- POST /api/course — Create course
- PUT /api/course/{courseno} — Update course
- DELETE /api/course/{courseno} — Delete course

### Exam Mark Entry
- GET /api/exam/results — Get student results
- POST /api/exam/mark-entry — Insert mark entry
- PUT /api/exam/mark-entry/{idno}/{courseno}/{sessionno} — Update marks
- DELETE /api/exam/mark-entry/{idno}/{courseno}/{sessionno} — Cancel mark entry (soft delete)

### Exam Result
- GET /api/examresult — Get student results by session
- POST /api/examresult/calculate — Calculate and update pass/fail and grade