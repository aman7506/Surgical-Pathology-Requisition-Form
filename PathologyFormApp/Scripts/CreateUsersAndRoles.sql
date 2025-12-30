USE [pathology_db]
GO

-- Create roles if they don't exist
IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'DOCTOR')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (NEWID(), 'Doctor', 'DOCTOR', NEWID())
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'NURSE')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (NEWID(), 'Nurse', 'NURSE', NEWID())
END
GO

-- Create nurse users (password: Nurse@123)
IF NOT EXISTS (SELECT * FROM [dbo].[AspNetUsers] WHERE [UserName] = 'nurse1')
BEGIN
    DECLARE @Nurse1Id NVARCHAR(450) = NEWID()
    DECLARE @NurseRoleId NVARCHAR(450) = (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'NURSE')
    
    INSERT INTO [dbo].[AspNetUsers] (
        [Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
        [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
        [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], 
        [LockoutEnabled], [AccessFailedCount], [FullName], [Role], [IsActive], [CreatedAt]
    )
    VALUES (
        @Nurse1Id,
        'nurse1',
        'NURSE1',
        'nurse1@hospital.com',
        'NURSE1@HOSPITAL.COM',
        1,
        'AQAAAAIAAYagAAAAEGJQTqXn12sPQLqcFddhv6oAHGrcmhoEYqZ8t5PrlfqngoQlXqlwZhOhrv/tmEFBCA==', -- Password: Nurse@123
        NEWID(),
        NEWID(),
        '',
        0,
        0,
        NULL,
        1,
        0,
        'Nurse Sarah Johnson',
        'Nurse',
        1,
        GETUTCDATE()
    )

    -- Assign nurse role
    INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
    VALUES (@Nurse1Id, @NurseRoleId)
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[AspNetUsers] WHERE [UserName] = 'nurse2')
BEGIN
    DECLARE @Nurse2Id NVARCHAR(450) = NEWID()
    DECLARE @NurseRoleId2 NVARCHAR(450) = (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'NURSE')
    
    INSERT INTO [dbo].[AspNetUsers] (
        [Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
        [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
        [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], 
        [LockoutEnabled], [AccessFailedCount], [FullName], [Role], [IsActive], [CreatedAt]
    )
    VALUES (
        @Nurse2Id,
        'nurse2',
        'NURSE2',
        'nurse2@hospital.com',
        'NURSE2@HOSPITAL.COM',
        1,
        'AQAAAAIAAYagAAAAEGJQTqXn12sPQLqcFddhv6oAHGrcmhoEYqZ8t5PrlfqngoQlXqlwZhOhrv/tmEFBCA==', -- Password: Nurse@123
        NEWID(),
        NEWID(),
        '',
        0,
        0,
        NULL,
        1,
        0,
        'Nurse Maria Rodriguez',
        'Nurse',
        1,
        GETUTCDATE()
    )

    -- Assign nurse role
    INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
    VALUES (@Nurse2Id, @NurseRoleId2)
END
GO

-- Create additional doctor user
IF NOT EXISTS (SELECT * FROM [dbo].[AspNetUsers] WHERE [UserName] = 'doctor1')
BEGIN
    DECLARE @Doctor1Id NVARCHAR(450) = NEWID()
    DECLARE @DoctorRoleId NVARCHAR(450) = (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'DOCTOR')
    
    INSERT INTO [dbo].[AspNetUsers] (
        [Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], 
        [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], 
        [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], 
        [LockoutEnabled], [AccessFailedCount], [FullName], [Role], [IsActive], [CreatedAt]
    )
    VALUES (
        @Doctor1Id,
        'doctor1',
        'DOCTOR1',
        'doctor1@hospital.com',
        'DOCTOR1@HOSPITAL.COM',
        1,
        'AQAAAAIAAYagAAAAEGJQTqXn12sPQLqcFddhv6oAHGrcmhoEYqZ8t5PrlfqngoQlXqlwZhOhrv/tmEFBCA==', -- Password: Doctor@123
        NEWID(),
        NEWID(),
        '',
        0,
        0,
        NULL,
        1,
        0,
        'Dr. John Smith',
        'Doctor',
        1,
        GETUTCDATE()
    )

    -- Assign doctor role
    INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
    VALUES (@Doctor1Id, @DoctorRoleId)
END
GO

-- Ensure admin user has doctor role
IF NOT EXISTS (SELECT * FROM [dbo].[AspNetUserRoles] ur 
               INNER JOIN [dbo].[AspNetUsers] u ON ur.UserId = u.Id 
               INNER JOIN [dbo].[AspNetRoles] r ON ur.RoleId = r.Id 
               WHERE u.UserName = 'admin' AND r.NormalizedName = 'DOCTOR')
BEGIN
    DECLARE @AdminUserId NVARCHAR(450) = (SELECT [Id] FROM [dbo].[AspNetUsers] WHERE [UserName] = 'admin')
    DECLARE @DoctorRoleId2 NVARCHAR(450) = (SELECT [Id] FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'DOCTOR')
    
    INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
    VALUES (@AdminUserId, @DoctorRoleId2)
END
GO

PRINT 'Users and roles created successfully!'
PRINT ''
PRINT '=== Login Credentials ==='
PRINT 'Admin User:'
PRINT '  Username: admin'
PRINT '  Password: Admin@123'
PRINT '  Role: Doctor'
PRINT ''
PRINT 'Nurse Users:'
PRINT '  Username: nurse1'
PRINT '  Password: Nurse@123'
PRINT '  Role: Nurse'
PRINT ''
PRINT '  Username: nurse2'
PRINT '  Password: Nurse@123'
PRINT '  Role: Nurse'
PRINT ''
PRINT 'Doctor User:'
PRINT '  Username: doctor1'
PRINT '  Password: Doctor@123'
PRINT '  Role: Doctor'
PRINT ''
PRINT '=== Role-Based Access ==='
PRINT 'Nurses can:'
PRINT '  - Create new forms'
PRINT '  - Fill basic patient information'
PRINT '  - Submit forms for doctor review'
PRINT '  - Manage specimen types'
PRINT ''
PRINT 'Doctors can:'
PRINT '  - View all forms'
PRINT '  - Review nurse-submitted forms'
PRINT '  - Edit and complete forms'
PRINT '  - Manage specimen types'
PRINT '  - Access all features' 