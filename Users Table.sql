CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Salt NVARCHAR(128) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    SelectedPersonality NVARCHAR(50) NOT NULL DEFAULT 'Softie Bebe',
    PhoneNumber NVARCHAR(20) NULL,
    ProfilePictureUrl NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    LastLogin DATETIME2 NULL,
    
    CONSTRAINT CHK_Users_Email_Format CHECK (Email LIKE '%_@_%._%'),
    CONSTRAINT CHK_Users_Age CHECK (DATEDIFF(YEAR, DateOfBirth, GETDATE()) >= 13)
);