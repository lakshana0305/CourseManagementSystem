IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Courses] (
    [CourseId] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Instructor] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY ([CourseId])
);

CREATE TABLE [Enrollments] (
    [EnrollmentId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_Enrollments] PRIMARY KEY ([EnrollmentId])
);

CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260925065255_InitialCreate', N'10.0.12');

COMMIT;
GO

