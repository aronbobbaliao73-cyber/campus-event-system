IF DB_ID('CampusEvents') IS NULL
    CREATE DATABASE CampusEvents;
GO

USE CampusEvents;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

CREATE TABLE dbo.Users
(
    UserId        INT IDENTITY(1,1)  NOT NULL,
    FirstName     NVARCHAR(100)      NOT NULL,
    LastName      NVARCHAR(100)      NOT NULL,
    Email         NVARCHAR(255)      NOT NULL,
    PasswordHash  VARBINARY(256)     NULL,
    Role          VARCHAR(20)        NOT NULL CONSTRAINT DF_Users_Role DEFAULT ('Student'),
    CreatedAt     DATETIME2(0)       NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_Email CHECK
    (
        Email LIKE '_%@univ.edu.ph'
        AND Email NOT LIKE '% %'
        AND Email NOT LIKE '%@%@%'
    ),
    CONSTRAINT CK_Users_Role CHECK (Role IN ('Student', 'Admin'))
);

CREATE TABLE dbo.Events
(
    EventId        INT IDENTITY(1,1)  NOT NULL,
    Title          NVARCHAR(200)      NOT NULL,
    Description    NVARCHAR(2000)     NULL,
    Venue          NVARCHAR(200)      NOT NULL,
    StartDateTime  DATETIME2(0)       NOT NULL,
    EndDateTime    DATETIME2(0)       NOT NULL,
    Capacity       INT                NOT NULL,
    Status         VARCHAR(20)        NOT NULL CONSTRAINT DF_Events_Status DEFAULT ('Published'),
    CreatedBy      INT                NOT NULL,
    CreatedAt      DATETIME2(0)       NOT NULL CONSTRAINT DF_Events_CreatedAt DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Events PRIMARY KEY CLUSTERED (EventId),
    CONSTRAINT FK_Events_Users_CreatedBy FOREIGN KEY (CreatedBy)
        REFERENCES dbo.Users (UserId)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT CK_Events_Capacity CHECK (Capacity > 0),
    CONSTRAINT CK_Events_Status CHECK (Status IN ('Draft', 'Published', 'Cancelled', 'Completed')),
    CONSTRAINT CK_Events_Dates CHECK (EndDateTime > StartDateTime)
);

CREATE TABLE dbo.Registrations
(
    RegistrationId  INT IDENTITY(1,1)  NOT NULL,
    UserId          INT                NOT NULL,
    EventId         INT                NOT NULL,
    Status          VARCHAR(20)        NOT NULL CONSTRAINT DF_Registrations_Status DEFAULT ('Registered'),
    RegisteredAt    DATETIME2(0)       NOT NULL CONSTRAINT DF_Registrations_RegisteredAt DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_Registrations PRIMARY KEY CLUSTERED (RegistrationId),
    CONSTRAINT FK_Registrations_Users FOREIGN KEY (UserId)
        REFERENCES dbo.Users (UserId)
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
    CONSTRAINT FK_Registrations_Events FOREIGN KEY (EventId)
        REFERENCES dbo.Events (EventId)
        ON DELETE CASCADE
        ON UPDATE NO ACTION,
    CONSTRAINT UQ_Registrations_User_Event UNIQUE (UserId, EventId),
    CONSTRAINT CK_Registrations_Status CHECK (Status IN ('Registered', 'Cancelled', 'Attended'))
);

CREATE NONCLUSTERED INDEX IX_Events_CreatedBy
    ON dbo.Events (CreatedBy);

CREATE NONCLUSTERED INDEX IX_Registrations_UserId
    ON dbo.Registrations (UserId);

CREATE NONCLUSTERED INDEX IX_Registrations_EventId
    ON dbo.Registrations (EventId)
    INCLUDE (UserId, Status, RegisteredAt);

INSERT INTO dbo.Users (FirstName, LastName, Email, Role) VALUES
    ('Admin', 'User', 'admin@univ.edu.ph', 'Admin'),
    ('Juan', 'Dela Cruz', 'juan.delacruz@univ.edu.ph', 'Student'),
    ('Maria', 'Santos', 'maria.santos@univ.edu.ph', 'Student');

INSERT INTO dbo.Events (Title, Description, Venue, StartDateTime, EndDateTime, Capacity, CreatedBy) VALUES
    ('Tech Talk 2026', 'Industry speakers on web development.', 'Main Auditorium', '2026-11-05T14:00:00', '2026-11-05T17:00:00', 100, 1),
    ('Career Fair', 'Meet partner companies.', 'Gymnasium', '2026-11-20T09:00:00', '2026-11-20T16:00:00', 300, 1);

INSERT INTO dbo.Registrations (UserId, EventId) VALUES (2, 1), (3, 1), (2, 2);

COMMIT TRANSACTION;
GO