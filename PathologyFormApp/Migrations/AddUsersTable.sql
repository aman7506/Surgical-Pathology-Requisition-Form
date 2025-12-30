-- Create Users table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NOT NULL,
    FullName NVARCHAR(100),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Create index on Username
CREATE UNIQUE INDEX IX_Users_Username ON Users(Username);

-- Insert default admin user (password: admin123)
INSERT INTO Users (Username, Password, Role, FullName, IsActive, CreatedAt)
VALUES ('admin', 'admin123', 'Doctor', 'System Administrator', 1, GETUTCDATE()); 