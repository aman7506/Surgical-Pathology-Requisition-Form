# Complete Project Guide - Surgical Pathology Requisition Form

**Version**: 1.0 | **Date**: December 30, 2025 | **Status**: Production-Ready

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [Project Structure](#project-structure)
4. [Database Architecture](#database-architecture)
5. [Security & Authentication](#security--authentication)
6. [Installation & Setup](#installation--setup)
7. [Deployment Guide](#deployment-guide)
8. [User Workflows](#user-workflows)
9. [API Reference](#api-reference)
10. [Troubleshooting](#troubleshooting)

---

## Project Overview

### What is it?
A comprehensive ASP.NET Core 8.0 MVC web application for managing surgical pathology requisition forms in healthcare facilities. Digitizes the complete workflow from specimen collection (Nurses) to pathologist review (Doctors).

### Key Statistics
- **60+ Fields**: Complete patient, specimen, and examination data
- **2 User Roles**: Nurse (data entry), Doctor (review)
- **3 Workflow States**: Draft → NurseSubmitted → DoctorReviewed
- **100% Feature Complete**: All CRUD operations, audit trail, file uploads
- **Production Ready**: Full deployment configurations included

### Business Value
- **Eliminates paper-based inefficiency**
- **Centralized digital storage with complete audit trail**
- **Clear role-based workflow**
- **Instant search and retrieval**
- **Regulatory compliance ready**

---

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 (MVC)
- **Language**: C# (.NET 8.0)
- **ORM**: Entity Framework Core 8.0.2
- **Micro-ORM**: Dapper 2.1.66 (for stored procedures)
- **Authentication**: ASP.NET Core Identity 8.0.2
- **Database**: Microsoft SQL Server

### Frontend
- **View Engine**: Razor (.cshtml)
- **CSS Framework**: Bootstrap 5
- **JavaScript**: jQuery + Vanilla JS
- **Features**: Responsive design, client-side validation

### Key NuGet Packages
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.2" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.2" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.2" />
<PackageReference Include="Dapper" Version="2.1.66" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="8.0.0" />
```

---

## Project Structure

```
e:\Aman Project Files\Surgical Pathology Requisition Form\
│
├── PathologyFormApp\                     # Main Application
│   ├── Controllers\                      # 5 MVC Controllers
│   │   ├── AccountController.cs          # Authentication (4.3 KB)
│   │   ├── HomeController.cs             # Dashboard (10.2 KB)
│   │   ├── PathologyController.cs        # Main CRUD (16.2 KB, 391 lines)
│   │   ├── PathologyFormController.cs    # Advanced operations (18.7 KB)
│   │   └── SpecimenController.cs         # Lookup management (9.2 KB)
│   │
│   ├── Models\                           # 9 Domain Models
│   │   ├── PathologyRequisitionForm.cs   # 184 lines, 60+ fields
│   │   ├── PathologyContext.cs           # EF DbContext
│   │   ├── User.cs                       # Extended Identity
│   │   ├── FormStatus.cs                 # Workflow enum
│   │   ├── SpecimenType.cs               # Lookup data
│   │   ├── FormHistory.cs                # Audit trail
│   │   ├── PaginatedList.cs              # Pagination helper
│   │   ├── UploadedFileInfo.cs           # File metadata
│   │   └── ErrorViewModel.cs             # Error handling
│   │
│   ├── ViewModels\                       # 4 View-Specific Models
│   │   ├── NurseFormViewModel.cs         # Nurse data entry
│   │   ├── DoctorFormViewModel.cs        # Doctor review
│   │   ├── DoctorReviewViewModel.cs      # Review workflow
│   │   └── LoginViewModel.cs             # Authentication
│   │
│   ├── Views\                            # Razor Views (22 views)
│   │   ├── Pathology\                    # 7 CRUD views
│   │   ├── Account\                      # 2 auth views
│   │   ├── Home\                         # 2 dashboard views
│   │   ├── PathologyForm\                # 3 advanced views
│   │   ├── Specimen\                     # 1 lookup view
│   │   └── Shared\                       # 5 layouts/partials
│   │
│   ├── Migrations\                       # 14 EF Core Migrations
│   ├── Scripts\                          # 4 SQL Scripts
│   │   ├── CreateDatabase.sql            # Complete DB setup (350 lines)
│   │   ├── CreateUsersAndRoles.sql       # User seeding
│   │   ├── RestoreDatabase.sql           # DB restore
│   │   └── UpdateStoredProcedure.sql     # SP maintenance
│   │
│   ├── wwwroot\                          # Static Files
│   │   ├── css\                          # Stylesheets
│   │   ├── js\                           # JavaScript
│   │   ├── lib\                          # Third-party libs
│   │   ├── images\                       # Image assets
│   │   └── uploads\                      # File upload storage
│   │
│   ├── Program.cs                        # Application entry (72 lines)
│   ├── appsettings.json                  # Configuration
│   └── PathologyFormApp.csproj           # Project file (95 lines)
│
└── Documentation\                        # All .md files
```

---

## Database Architecture

### Main Tables

#### 1. PathologyForm
**Primary Key**: UHID (Unique Hospital ID)

**Key Fields** (60+ total):
```sql
-- Patient Information
UHID, LabRefNo, Date, Name, Age, Gender, CRNo, OPD_IPD, IPDNo, Consultant

-- Clinical Details
ClinicalDiagnosis, DateTimeOfCollection

-- Obstetric/Gynecological Data
MensesOnset, LastingDays, Character, LMP, Gravida, Para, 
MenopauseAge, MenopauseYears, MensesCycle, HormoneTherapy

-- Investigation Findings
XRayUSGCTMRIFindings, LaboratoryFindings, OperativeFindings, 
PostOperativeDiagnosis, PreviousHPCytReport

-- Specimen Details
DateTimeOfReceivingSpecimen, NoOfSpecimenReceived, SpecimenNo, 
SpecimenName, MicroSectionNo, SpecialStains

-- Examination Results
GrossDescription, MicroscopicExamination, Impression, Advice
Pathologist, PathologistDate, SignatureImage

-- Workflow Fields
Status, CreatedById, ReviewedById, ReviewedAt, CreatedAt, UpdatedAt
DateTimeOfProcessing, UploadedFilePath
```

#### 2. FormHistory (Audit Trail)
```sql
Id (PK), FormUHID (FK), UserId (FK), Action, Comments, Timestamp
```
Tracks: CREATE, UPDATE, REVIEW, DELETE actions

#### 3. SpecimenTypes (Lookup)
```sql
Id (PK), Name, Description, IsActive
```

#### 4. AspNetUsers (Extended Identity)
```sql
Id (PK), UserName, Email, PasswordHash
FullName, Role, IsActive, CreatedAt  -- Custom fields
```

### Entity Relationships
```
User (1) ──creates──> (N) PathologyForm
User (1) ──reviews──> (N) PathologyForm
PathologyForm (1) ──has──> (N) FormHistory
User (1) ──performs──> (N) FormHistory
```

### Stored Procedures

**sp_ManagePathologyForm**: Main CRUD with transaction support
**sp_GetFormHistory**: Retrieve audit trail
**sp_GetFormsByStatus**: Filter forms by status/user

---

## Security & Authentication

### Authentication System
- **Identity Framework**: ASP.NET Core Identity
- **Method**: Cookie-based (7-day expiration, sliding window)
- **Password Policy**: Min 8 chars, digit, uppercase, lowercase, special char

### Authorization
```csharp
// Role-Based Policies
[Authorize(Roles = "Nurse")]   // Nurse actions
[Authorize(Roles = "Doctor")]  // Doctor actions
```

### Security Layers
1. **Transport**: HTTPS/TLS encryption
2. **Authentication**: Password hashing (PBKDF2)
3. **Authorization**: Role-based + resource-based
4. **Data Protection**: SQL injection prevention, XSS protection, CSRF tokens
5. **Database**: Encrypted connections, least privilege

### Default Users (Seeded)
```
Nurse: nurse@hospital.com / Password123!
Doctor: doctor@hospital.com / Password123!
```
⚠️ **Change in production!**

---

## Installation & Setup

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+
- Visual Studio 2022 or VS Code

### Quick Start (5 Steps)

#### 1. Configure Database
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=pathology_db;User Id=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=True;"
  }
}
```

#### 2. Create Database
Execute in SQL Server Management Studio:
```sql
-- Run Scripts/CreateDatabase.sql
-- Run Scripts/CreateUsersAndRoles.sql
```

Or use EF migrations:
```powershell
dotnet ef database update
```

#### 3. Restore Packages
```powershell
cd "PathologyFormApp"
dotnet restore
```

#### 4. Build & Run
```powershell
dotnet build
dotnet run
```

#### 5. Access Application
- HTTPS: https://localhost:7123
- HTTP: http://localhost:5123

---

## Deployment Guide

### Windows Server / IIS

#### Step 1: Install Prerequisites
```powershell
# Install .NET 8.0 Hosting Bundle
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0

# Verify installation
dotnet --info
```

#### Step 2: Publish Application
```powershell
cd PathologyFormApp
dotnet publish -c Release -o ./publish
```

#### Step 3: Create IIS Site
1. **Create App Pool**: PathologyAppPool (No Managed Code)
2. **Create Website**: 
   - Physical Path: C:\inetpub\PathologyApp
   - Port: 443 (HTTPS)
   - SSL Certificate required

#### Step 4: Set Permissions
```powershell
icacls "C:\inetpub\PathologyApp" /grant "IIS_IUSRS:(OI)(CI)R" /T
icacls "C:\inetpub\PathologyApp\wwwroot\uploads" /grant "IIS_IUSRS:(OI)(CI)M" /T
```

#### Step 5: Update Configuration
Update `appsettings.json` with production connection string.

#### Step 6: Start Site
```powershell
iisreset
```

### Linux / Nginx

#### Publish for Linux
```powershell
dotnet publish -c Release -r linux-x64 --self-contained false -o ./publish_linux
```

#### Create Systemd Service
```ini
[Unit]
Description=Pathology App
After=network.target

[Service]
Type=notify
User=www-data
WorkingDirectory=/var/www/pathologyapp
ExecStart=/usr/bin/dotnet /var/www/pathologyapp/PathologyFormApp.dll
Restart=always
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

#### Configure Nginx
```nginx
server {
    listen 443 ssl;
    server_name pathology.yourdomain.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
    }
    
    client_max_body_size 50M;
}
```

### Cloud Deployment (Azure)

```bash
# Create resources
az group create --name PathologyAppRG --location eastus
az webapp create --name pathologyapp --resource-group PathologyAppRG --runtime "DOTNETCORE:8.0"

# Deploy
az webapp deploy --resource-group PathologyAppRG --name pathologyapp --src-path app.zip --type zip

# Configure connection string
az webapp config connection-string set --name pathologyapp --connection-string-type SQLAzure --settings DefaultConnection="..."
```

---

## User Workflows

### Nurse Workflow
```
1. Login with nurse credentials
   ↓
2. Dashboard → Create New Form
   ↓
3. Fill Patient Information (Name, Age, Gender, UHID, etc.)
   ↓
4. Record Specimen Details (Collection time, type, count)
   ↓
5. Add Clinical Information (Diagnosis, consultant)
   ↓
6. Option A: Save as Draft (Status: Draft)
   Option B: Submit for Review (Status: NurseSubmitted)
```

### Doctor Workflow
```
1. Login with doctor credentials
   ↓
2. Dashboard → View Submitted Forms (Status: NurseSubmitted)
   ↓
3. Select Form → Review patient/specimen data
   ↓
4. Perform Examination → Enter findings
   ↓
5. Add Gross Description
   ↓
6. Add Microscopic Examination
   ↓
7. Provide Impression and Advice
   ↓
8. Add Digital Signature (optional)
   ↓
9. Complete Review (Status: DoctorReviewed)
```

### Form States
```
[Draft] → [NurseSubmitted] → [DoctorReviewed]
  ↓           (editable)           (final)
Can edit
by Nurse
```

---

## API Reference

### Authentication Endpoints
```
POST /Account/Login        - User login
POST /Account/Logout       - User logout
GET  /Account/Register     - Registration page (if enabled)
```

### Pathology Form Endpoints
```
GET  /Pathology/Index                  - List all forms (paginated)
GET  /Pathology/Details/{uhid}         - View form details
GET  /Pathology/Create                 - Create form page
POST /Pathology/Create                 - Submit new form
GET  /Pathology/Edit/{uhid}            - Edit form page
POST /Pathology/Edit/{uhid}            - Update form
GET  /Pathology/Delete/{uhid}          - Delete confirmation
POST /Pathology/Delete/{uhid}          - Delete form
POST /Pathology/DeleteUploadedFile     - Remove file attachment
```

### Advanced Operations
```
GET  /PathologyForm/*      - Role-specific views
POST /PathologyForm/*      - Advanced submissions
```

### Specimen Management
```
GET  /Specimen/Index       - Manage specimen types
```

---

## Troubleshooting

### Issue: Database Connection Failed
**Symptoms**: "Cannot open database" error

**Solutions**:
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure database exists: `SELECT name FROM sys.databases`
4. Test connection: `sqlcmd -S SERVER -U USER -P PASSWORD`
5. Check firewall settings

### Issue: Login Not Working
**Symptoms**: "Invalid login attempt"

**Solutions**:
1. Verify users exist: `SELECT * FROM AspNetUsers`
2. Re-run `CreateUsersAndRoles.sql`
3. Check password meets complexity requirements
4. Verify Role assignments in `AspNetUserRoles`

### Issue: File Upload Errors
**Symptoms**: "Cannot save file"

**Solutions**:
1. Check `wwwroot/uploads` folder exists
2. Verify write permissions: `icacls wwwroot\uploads`
3. Check disk space
4. Verify `web.config` file size limits

### Issue: Migration Errors
**Symptoms**: EF Core migration failures

**Solutions**:
```powershell
# Clean and re-apply
dotnet ef database drop --force
dotnet ef database update

# Or use SQL scripts directly
# Execute Scripts/CreateDatabase.sql
```

### Issue: IIS Deployment Errors
**Symptoms**: 500 Internal Server Error

**Solutions**:
1. Check Event Viewer → Application logs
2. Enable stdout logging in `web.config`:
   ```xml
   <aspNetCore stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" />
   ```
3. Verify .NET 8.0 Hosting Bundle installed
4. Check App Pool identity has database access

### Debug Mode
Enable detailed errors (Development only):
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  }
}
```

---

## Configuration Reference

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;User Id=...;Password=..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Program.cs Key Configuration
```csharp
// DbContext
builder.Services.AddDbContext<PathologyContext>(options =>
    options.UseSqlServer(connectionString));

// Identity with password policy
builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
});

// Authorization policies
builder.Services.AddAuthorization(options => {
    options.AddPolicy("RequireDoctorRole", policy => policy.RequireRole("Doctor"));
    options.AddPolicy("RequireNurseRole", policy => policy.RequireRole("Nurse"));
});

// Cookie settings
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});
```

---

## Production Checklist

### Pre-Deployment
- [ ] Change default database credentials
- [ ] Update production connection strings
- [ ] Obtain SSL certificate
- [ ] Configure firewall (only 80/443 open)
- [ ] Set up database backups
- [ ] Test deployment in staging environment

### Security
- [ ] HTTPS enforced (no HTTP)
- [ ] Strong passwords for all users
- [ ] Debug mode disabled
- [ ] Detailed errors disabled
- [ ] Database user has least privileges
- [ ] Connection strings encrypted/secured

### Performance
- [ ] Enable response compression
- [ ] Configure output caching
- [ ] Set up CDN for static files
- [ ] Database indexes verified
- [ ] Connection pooling configured

### Monitoring
- [ ] Application logging configured
- [ ] Health checks endpoint: `/health`
- [ ] Error tracking enabled
- [ ] Performance monitoring set up
- [ ] Backup verification scheduled

---

## Quick Reference

### Project Info
- **Location**: `e:\Aman Project Files\Surgical Pathology Requisition Form\`
- **Database**: 172.1.3.189:pathology_db
- **Main Entry**: PathologyFormApp/Program.cs
- **Config**: appsettings.json

### Key Commands
```powershell
# Build
dotnet build

# Run
dotnet run

# Migrations
dotnet ef migrations add MigrationName
dotnet ef database update

# Publish
dotnet publish -c Release -o ./publish

# Clean
dotnet clean
```

### Default URLs
- Local HTTPS: https://localhost:7123
- Local HTTP: http://localhost:5123
- Health Check: /health

---

## Contact & Support

### Documentation Files
- README.md - Complete documentation
- TECHNICAL_ARCHITECTURE.md - Architecture details
- DEPLOYMENT_GUIDE.md - Deployment instructions
- PROJECT_SUMMARY.md - Executive overview
- **COMPLETE_PROJECT_GUIDE.md** - This file (all-in-one)

### External Resources
- ASP.NET Core Docs: https://docs.microsoft.com/en-us/aspnet/core/
- EF Core Docs: https://docs.microsoft.com/en-us/ef/core/
- Identity Docs: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity

---

*Complete Project Guide - Last Updated: December 30, 2025*
*All content in a single comprehensive file*
