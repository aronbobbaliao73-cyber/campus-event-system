# Online Campus Event Management System

## Team Roster

| Member | Name | Assigned Role |
|---|---|---|
| Member 1 | Aron Bob Baliao | Systems Architect & Prompt Lead |
| Member 2 | Charm Bueneventura | Frontend Engineer |
| Member 3 | Sharmaine Bicomong | Database & Backend Engineer |

## Task 1

### Prompt Used

ROLE
You are a Lead Systems Architect with 15 years of experience designing web applications for universities and delivering working prototypes under tight deadlines.

CONTEXT
A team of three 4th-year BSIT students has 3 hours to build a working prototype of an Online Campus Event Management System. Students must be able to view upcoming campus events and register for an event. Administrators must be able to view the list of registered attendees for each event. The team is new to generative AI tools, so the design must be simple enough to build and explain within the time limit. The frontend is plain HTML, CSS and JavaScript, the backend is C# (.NET), and the database is SQL Server.

TASK
Produce an overall system design that includes:
1. A short list of functional requirements for students and administrators.
2. A high-level architecture (frontend, backend, database) with the responsibility of each layer.
3. The main data entities and how they relate to each other.
4. A list of backend API endpoints with the HTTP method and purpose of each.
5. A suggested folder structure for the project.
6. A realistic division of work for three team members within 3 hours.

CONSTRAINTS
- Do not use third-party state management libraries such as Redux.
- Do not use frontend frameworks such as React, Angular or Vue.
- Do not propose microservices, message queues, or cloud infrastructure.
- Do not include features beyond viewing events, registering for an event, and viewing attendees.
- Keep the whole design achievable by three students in 3 hours.
- Use clear headings and keep the answer under 700 words.

### AI Output

Online Campus Event Management System: Design
1. Functional Requirements

Students

View a list of upcoming events (title, date, venue, available slots).
Register for an event by entering name, student number, and email.
See a confirmation or error message (full event, duplicate registration).

Administrators

Pick an event and view the list of registered attendees.

Login is out of scope for the prototype. The admin page is a separate page that is not linked from the student pages.

2. High-Level Architecture
Layer	Technology	Responsibility
Frontend	HTML, CSS, JavaScript (fetch)	Render events, submit the registration form, render the attendee table. State lives in plain JS variables.
Backend	ASP.NET Core Web API (C#)	Validate input, enforce rules (capacity, no duplicates), return JSON. Also serves the static frontend from wwwroot, so there are no CORS issues.
Database	SQL Server + Entity Framework Core	Store events and registrations.

Everything runs as one project and one process, so there is nothing to deploy beyond dotnet run.

3. Data Entities

Event: Id, Title, Description, EventDate, Venue, Capacity

Registration: Id, EventId (FK), StudentName, StudentNumber, Email, RegisteredAt

Relationship: one Event has many Registrations (1:N). A unique index on (EventId, StudentNumber) prevents duplicate sign-ups.

Student details are stored directly on Registration instead of a separate Student table. This avoids accounts and login and keeps the schema to two tables.

4. API Endpoints
Method	Route	Purpose
GET	/api/events	List upcoming events with remaining slots
GET	/api/events/{id}	Get one event's details
POST	/api/events/{id}/registrations	Register a student (returns 400 if full, 409 if duplicate)
GET	/api/events/{id}/registrations	List attendees for an event (admin page)
5. Folder Structure
CampusEvents/
├── CampusEvents.csproj
├── Program.cs
├── appsettings.json
├── Controllers/
│   └── EventsController.cs
├── Models/
│   ├── Event.cs
│   └── Registration.cs
├── Data/
│   └── AppDbContext.cs
├── Dtos/
│   └── RegisterRequest.cs
├── Database/
│   └── schema-and-seed.sql
└── wwwroot/
    ├── index.html          (event list)
    ├── register.html       (registration form)
    ├── admin.html          (attendee list)
    ├── css/
    │   └── styles.css
    └── js/
        ├── api.js          (shared fetch helpers)
        ├── events.js
        ├── register.js
        └── admin.js
6. Division of Work (3 Hours)
Time	Member A: Database & Backend	Member B: Frontend (Student)	Member C: Frontend (Admin) & Integration
0:00–0:20	Agree on entities and API contract together (this document)		
0:20–1:15	Create SQL tables and sample data, AppDbContext, models	index.html, styles.css, event list rendering	admin.html layout, attendee table, event dropdown
1:15–2:00	Build the four endpoints and validation rules	register.html, form submission, error and success messages	admin.js calling the attendees endpoint, api.js helpers
2:00–2:30	Test endpoints with browser or Postman	Connect pages to the live API	Connect admin page, test full flow
2:30–3:00	All together: end-to-end test, fix bugs, rehearse a 5-minute demo and explanation		

Risk controls

Freeze the API contract in the first 20 minutes so frontend work can start immediately with the agreed JSON shapes.
Seed 3–5 sample events so the demo never depends on typed data.
If time runs short, drop styling polish first. Never drop the duplicate and capacity checks, because they are the key rules to explain.

### Manual Grounding Evaluation

PASTE_EVALUATION_HERE

## Task 3

### Prompt Used

## Task 3

### Prompt Used

PASTE_TASK3_PROMPT_HERE

### AI Output Summary

The AI proposed a 3NF schema with Users, Events and Registrations. Registrations is the many-to-many link between Users and Events, with a unique (UserId, EventId) pair to block duplicate sign-ups. Foreign keys use NO ACTION for Events.CreatedBy and CASCADE for Registrations. The AI also noted that a CHECK constraint cannot enforce event capacity, so the registration API must check seat availability.

### Entity-Relationship Diagram

```mermaid
erDiagram
    USERS ||--o{ EVENTS : creates
    USERS ||--o{ REGISTRATIONS : makes
    EVENTS ||--o{ REGISTRATIONS : receives

    USERS {
        int UserId PK
        nvarchar FirstName
        nvarchar LastName
        nvarchar Email UK
        varbinary PasswordHash
        varchar Role
        datetime2 CreatedAt
    }
    EVENTS {
        int EventId PK
        nvarchar Title
        nvarchar Description
        nvarchar Venue
        datetime2 StartDateTime
        datetime2 EndDateTime
        int Capacity
        varchar Status
        int CreatedBy FK
        datetime2 CreatedAt
    }
    REGISTRATIONS {
        int RegistrationId PKS
        int UserId FK
        int EventId FK
        varchar Status
        datetime2 RegisteredAt
    }
```

### SQL Script

See `/database/schema.sql`.