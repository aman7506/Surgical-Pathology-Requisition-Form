# GitHub Deployment Workflow - Surgical Pathology Requisition Form

## Enterprise-Grade GitHub Setup and Deployment Guide

**Project**: Surgical Pathology Requisition Form  
**Version**: 1.0  
**Target**: Healthcare / Hospital Information Systems  
**Classification**: Production-Ready Medical Software

---

## Table of Contents

1. [Git Repository Initialization](#git-repository-initialization)
2. [GitIgnore Configuration](#gitignore-configuration)
3. [Repository Structure](#repository-structure)
4. [GitHub Repository Creation](#github-repository-creation)
5. [Initial Commit and Push](#initial-commit-and-push)
6. [Branch Strategy](#branch-strategy)
7. [Deployment Workflows](#deployment-workflows)

---

## Git Repository Initialization

### Step 1: Verify Git Installation

Open PowerShell and execute:

```powershell
git --version
```

Expected output: `git version 2.x.x`

If Git is not installed, download from: https://git-scm.com/download/win

### Step 2: Configure Git Identity

```powershell
git config --global user.name "Your Full Name"
git config --global user.email "your.email@hospital.com"
```

Verify configuration:

```powershell
git config --list
```

### Step 3: Navigate to Project Directory

```powershell
cd "e:\Aman Project Files\Surgical Pathology Requisition Form"
```

### Step 4: Initialize Git Repository

```powershell
git init
```

Expected output: `Initialized empty Git repository in e:/Aman Project Files/Surgical Pathology Requisition Form/.git/`

---

## GitIgnore Configuration

### Step 5: Create Comprehensive .gitignore File

Create `.gitignore` in the project root with the following content:

```gitignore
# ASP.NET Core Build Results
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio Files
.vs/
*.suo
*.user
*.userosscache
*.sln.docstates
*.userprefs

# Build Results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Roslyn Cache Directories
*.ide/
*.ide-shm
*.ide-wal

# MSTest Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*

# NuGet Packages
*.nupkg
*.snupkg
**/packages/*
!**/packages/build/
*.nuget.props
*.nuget.targets

# Entity Framework Core
Migrations/
!Migrations/*.cs

# User-Specific Files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# User-Specific Files (Mono Auto Generated)
mono_crash.*

# Windows Image File Caches
Thumbs.db
ehthumbs.db

# Folder Config File
Desktop.ini

# Recycle Bin
$RECYCLE.BIN/

# VS Code Directories
.vscode/
!.vscode/tasks.json
!.vscode/launch.json
!.vscode/extensions.json

# JetBrains Rider
.idea/
*.sln.iml

# Publish Outputs (IMPORTANT FOR HEALTHCARE DATA SECURITY)
publish/
publish_output/
publish_win_x64/
PublishProfiles/

# File Uploads (CRITICAL - Contains Patient Data)
wwwroot/uploads/
**/wwwroot/uploads/*

# Database Files (SQLite if used)
*.db
*.db-shm
*.db-wal

# Configuration Files with Sensitive Data
appsettings.Production.json
appsettings.Staging.json
appsettings.*.json
!appsettings.json
!appsettings.Development.json

# Connection Strings and Secrets
secrets.json
*.pfx
*.p12

# Log Files
*.log
logs/
Logs/

# IIS Configuration
web.config
!web.config.example

# Backup Files
*.bak
*.backup
*_backup.*

# Temporary Files
*.tmp
*.temp
~$*

# Package Lock Files (Optional - Team Decision)
package-lock.json
yarn.lock

# Node Modules (if frontend tooling)
node_modules/

# macOS
.DS_Store

# Linux
*~

# Application Insights
ApplicationInsights.config

# NCrunch
_NCrunch_*
.*crunch*.local.xml
nCrunchTemp_*

# MightyMoose
*.mm.*
AutoTest.Net/

# Web Workbench
.sass-cache/

# Installshield
[Ee]xpress/

# DocProject
DocProject/buildhelp/
DocProject/Help/*.HxT
DocProject/Help/*.HxC
DocProject/Help/*.hhc
DocProject/Help/*.hhk
DocProject/Help/*.hhp
DocProject/Help/Html2
DocProject/Help/html

# Click-Once
publish.xml

# Publish Web Output
*.[Pp]ublish.xml
*.azurePubxml

# NuGet Symbol Packages
*.snupkg

# Azure Stream Analytics Local Run Output
ASALocalRun/

# MSBuild Binary Log
*.binlog

# NVidia Nsight GPU Debugger
*.nvuser

# MFractors (Xamarin productivity tool)
FodyWeavers.xsd

# Backup & Report Files from Converting Old Project Files
_UpgradeReport_Files/
Backup*/
UpgradeLog*.XML
UpgradeLog*.htm
ServiceFabricBackup/
*.rptproj.bak

# SQL Server Files
*.mdf
*.ldf
*.ndf

# Business Intelligence Projects
*.rdl.data
*.bim.layout
*.bim_*.settings
*.rptproj.rsuser
*- [Bb]ackup.rdl
*- [Bb]ackup ([0-9]).rdl
*- [Bb]ackup ([0-9][0-9]).rdl

# Microsoft Fakes
FakesAssemblies/

# GhostDoc Plugin
*.GhostDoc.xml

# Node.js Tools for Visual Studio
.ntvs_analysis.dat
node_modules/

# Visual Studio 6 Build Log
*.plg

# Visual Studio 6 Workspace Options
*.opt

# Visual Studio 6 Auto-generated Workspace
*.vbw

# Visual Studio LightSwitch Build Output
**/*.HTMLClient/GeneratedArtifacts
**/*.DesktopClient/GeneratedArtifacts
**/*.DesktopClient/ModelManifest.xml
**/*.Server/GeneratedArtifacts
**/*.Server/ModelManifest.xml
_Pvt_Extensions

# Paket Dependency Manager
.paket/paket.exe
paket-files/

# FAKE (F# Make)
.fake/

# CodeRush Personal Settings
.cr/personal

# Python Tools for Visual Studio
__pycache__/
*.pyc

# Cake
tools/**
!tools/packages.config

# Tabs Studio
*.tss

# Telerik's JustMock
*.jmconfig

# BizTalk Build Output
*.btp.cs
*.btm.cs
*.odx.cs
*.xsd.cs

# OpenCover UI Analysis Results
OpenCover/

# Azure Stream Analytics Local Run Output
ASALocalRun/

# MSBuild Binary and Structured Log
*.binlog

# NVidia Nsight GPU Debugger Configuration
*.nvuser

# MFractors (Xamarin productivity tool) Working Folder
.mfractor/

# Local History for Visual Studio
.localhistory/

# BeatPulse Healthcheck Temp Database
healthchecksdb

# Backup Folder for Package Reference Convert Tool
MigrationBackup/

# Ionide (cross platform F# VS Code tools) Working Folder
.ionide/

# Fody - Auto-generated XML Schema
FodyWeavers.xsd
```

Execute to create the file:

```powershell
# Create .gitignore using the content above
New-Item -Path ".gitignore" -ItemType File -Force
# Copy the content above into this file manually or using an editor
```

---

## Repository Structure

### Step 6: Organize Repository Structure

Your repository should follow this enterprise-standard structure:

```
Surgical-Pathology-Requisition-Form/
├── .gitignore
├── README.md
├── LICENSE
├── CHANGELOG.md
│
├── src/
│   └── PathologyFormApp/
│       ├── Controllers/
│       ├── Models/
│       ├── Views/
│       ├── ViewModels/
│       ├── Data/
│       ├── wwwroot/
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── PathologyFormApp.csproj
│
├── database/
│   ├── migrations/
│   ├── scripts/
│   │   ├── 01_CreateDatabase.sql
│   │   ├── 02_CreateUsersAndRoles.sql
│   │   ├── 03_StoredProcedures.sql
│   │   └── 04_SeedData.sql
│   └── schema/
│       └── database-schema.sql
│
├── docs/
│   ├── README.md
│   ├── COMPLETE_PROJECT_GUIDE.md
│   ├── TECHNICAL_ARCHITECTURE.md
│   ├── DEPLOYMENT_GUIDE.md
│   ├── API_REFERENCE.md
│   ├── USER_MANUAL.md
│   └── SECURITY_COMPLIANCE.md
│
├── deployment/
│   ├── iis/
│   │   ├── web.config.example
│   │   └── iis-setup-guide.md
│   ├── linux/
│   │   ├── pathologyapp.service.example
│   │   ├── nginx.conf.example
│   │   └── linux-deployment-guide.md
│   ├── azure/
│   │   ├── azure-deploy.sh
│   │   └── azure-setup-guide.md
│   └── docker/
│       ├── Dockerfile
│       └── docker-compose.yml
│
├── config/
│   ├── appsettings.Production.example.json
│   ├── appsettings.Staging.example.json
│   └── configuration-guide.md
│
└── tests/
    └── (Future: Unit and Integration Tests)
```

### Step 7: Restructure Your Current Project

Execute the following commands:

```powershell
# Create directory structure
New-Item -ItemType Directory -Path "src" -Force
New-Item -ItemType Directory -Path "database\migrations" -Force
New-Item -ItemType Directory -Path "database\scripts" -Force
New-Item -ItemType Directory -Path "database\schema" -Force
New-Item -ItemType Directory -Path "docs" -Force
New-Item -ItemType Directory -Path "deployment\iis" -Force
New-Item -ItemType Directory -Path "deployment\linux" -Force
New-Item -ItemType Directory -Path "deployment\azure" -Force
New-Item -ItemType Directory -Path "deployment\docker" -Force
New-Item -ItemType Directory -Path "config" -Force
New-Item -ItemType Directory -Path "tests" -Force

# Move application to src folder
Move-Item -Path "PathologyFormApp" -Destination "src\" -Force

# Move SQL scripts to database folder
Copy-Item -Path "src\PathologyFormApp\Scripts\CreateDatabase.sql" -Destination "database\scripts\01_CreateDatabase.sql" -Force
Copy-Item -Path "src\PathologyFormApp\Scripts\CreateUsersAndRoles.sql" -Destination "database\scripts\02_CreateUsersAndRoles.sql" -Force
Copy-Item -Path "src\PathologyFormApp\Scripts\UpdateStoredProcedure.sql" -Destination "database\scripts\03_StoredProcedures.sql" -Force

# Move documentation files
Move-Item -Path "*.md" -Destination "docs\" -Force

# Create example configuration files
Copy-Item -Path "src\PathologyFormApp\appsettings.json" -Destination "config\appsettings.Production.example.json" -Force
```

### Step 8: Create Example Configuration Files

Create `config/appsettings.Production.example.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_PRODUCTION_SERVER;Database=pathology_db;User Id=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;Encrypt=True;TrustServerCertificate=False;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "yourhospital.com"
}
```

Create `deployment/iis/web.config.example`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\PathologyFormApp.dll" 
                  stdoutLogEnabled="true" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
      <security>
        <requestFiltering>
          <requestLimits maxAllowedContentLength="52428800" />
        </requestFiltering>
      </security>
    </system.webServer>
  </location>
</configuration>
```

---

## GitHub Repository Creation

### Step 9: Create GitHub Account (if needed)

Navigate to: https://github.com and create an account using your hospital or professional email.

### Step 10: Create New Repository

1. Log in to GitHub
2. Click the "+" icon in the top right corner
3. Select "New repository"
4. Fill in repository details:

```
Repository name: Surgical-Pathology-Requisition-Form
Description: Enterprise-grade ASP.NET Core 8.0 MVC application for managing surgical pathology requisition forms in hospital environments. Features role-based workflows, audit trails, and compliance-ready documentation.
Visibility: Private (Recommended for healthcare applications)
Initialize: Do NOT initialize with README, .gitignore, or license (we already have these)
```

5. Click "Create repository"

### Step 11: Link Local Repository to GitHub

GitHub will display commands. Execute in PowerShell:

```powershell
# Add remote repository
git remote add origin https://github.com/YOUR_USERNAME/Surgical-Pathology-Requisition-Form.git

# Verify remote
git remote -v
```

---

## Initial Commit and Push

### Step 12: Stage Files for Commit

```powershell
# Add all files to staging area
git add .

# Verify staged files
git status
```

Review the output carefully. Ensure:
- No files from `bin/`, `obj/`, `publish/` directories
- No `appsettings.Production.json` (only example files)
- No files from `wwwroot/uploads/`
- No connection strings or passwords

### Step 13: Create Initial Commit

```powershell
git commit -m "Initial commit: Surgical Pathology Requisition Form v1.0

- ASP.NET Core 8.0 MVC application
- Role-based workflow (Nurse, Doctor)
- 60+ medical fields
- Entity Framework Core + Dapper
- ASP.NET Core Identity authentication
- Audit trail and form history
- File upload capability
- Production-ready deployment configurations
- Comprehensive documentation

Medical Disclaimer: This software is intended for use by trained medical professionals in hospital environments."
```

### Step 14: Create Main Branch and Push

```powershell
# Rename master to main (GitHub standard)
git branch -M main

# Push to GitHub
git push -u origin main
```

Enter your GitHub credentials when prompted.

---

## Branch Strategy

### Step 15: Create Development Branch

```powershell
# Create and switch to development branch
git checkout -b development

# Push development branch to GitHub
git push -u origin development
```

### Step 16: Create Feature Branch Example

```powershell
# Create feature branch
git checkout -b feature/add-barcode-scanning

# After completing feature
git add .
git commit -m "Feature: Add barcode scanning for specimen tracking

- Implement barcode reader integration
- Add specimen barcode field to PathologyForm model
- Update database schema with barcode column
- Add barcode validation logic
- Update UI to display barcode scanner input"

git push -u origin feature/add-barcode-scanning
```

### Recommended Branch Strategy for Healthcare Applications

```
main (production-ready)
  ↓
development (testing & integration)
  ↓
feature/feature-name (individual features)
hotfix/issue-description (critical fixes)
release/version-number (release preparation)
```

---

## Deployment Workflows

### Step 17: Tagging Releases

When ready for production deployment:

```powershell
# Create annotated tag
git tag -a v1.0.0 -m "Release v1.0.0: Initial production release

Features:
- Complete pathology form management
- Role-based access control
- Audit trail functionality
- File upload and management
- Comprehensive documentation
- IIS, Linux, and Azure deployment support

Tested on:
- Windows Server 2019 with IIS
- Ubuntu 22.04 with Nginx
- Azure App Service

Medical Compliance: HIPAA considerations documented"

# Push tag to GitHub
git push origin v1.0.0
```

### Step 18: Create GitHub Release

1. Navigate to your repository on GitHub
2. Click "Releases" in the right sidebar
3. Click "Create a new release"
4. Select the tag `v1.0.0`
5. Fill in release details:

```
Release title: v1.0.0 - Production Release

Description:
Initial production release of the Surgical Pathology Requisition Form application.

**New Features:**
- Complete surgical pathology requisition form management
- Role-based workflow: Nurse (data entry) → Doctor (review)
- 60+ medical and clinical fields
- Audit trail with complete form history
- Secure file upload and management
- ASP.NET Core Identity authentication
- Production deployment configurations for IIS, Linux, and Azure

**Technical Details:**
- ASP.NET Core 8.0 MVC
- Entity Framework Core 8.0.2
- SQL Server database
- Bootstrap 5 responsive UI

**Documentation:**
- Complete installation guide
- Deployment instructions for multiple platforms
- Technical architecture documentation
- API reference
- Security and compliance guidelines

**Deployment Packages:**
- Windows IIS deployment
- Linux deployment
- Azure App Service deployment

**Medical Disclaimer:**
This software is designed for use by trained medical professionals in hospital environments. Ensure compliance with local healthcare regulations and data protection laws.
```

6. Upload deployment packages (optional)
7. Click "Publish release"

---

## Commit Message Standards for Medical Projects

Use this format for commit messages:

```
Type: Brief summary (50 chars or less)

Detailed explanation (wrap at 72 characters):
- What was changed
- Why it was changed
- Impact on medical workflow or data
- Testing performed
- Compliance considerations (if applicable)

Medical Disclaimer: (if applicable)
Any relevant medical or compliance notes
```

### Commit Message Types

- `Feature`: New functionality
- `Fix`: Bug fix
- `Security`: Security improvement
- `Database`: Database schema or migration changes
- `Docs`: Documentation updates
- `Config`: Configuration changes
- `Refactor`: Code refactoring without feature changes
- `Test`: Adding or updating tests
- `Deploy`: Deployment-related changes

### Examples

```powershell
# Feature commit
git commit -m "Feature: Add digital signature support for pathologist reports

- Implement signature pad component
- Add SignatureImage field to PathologyForm model
- Update database schema with signature storage
- Add signature validation logic
- Ensure signature is captured before form completion
- Update doctor review workflow to require signature

Medical Compliance: Digital signatures comply with 21 CFR Part 11 requirements for electronic records"

# Security fix
git commit -m "Security: Fix SQL injection vulnerability in search functionality

- Replace string concatenation with parameterized queries
- Add input validation for search parameters
- Implement query parameter whitelist
- Add security logging for search operations

Security Impact: Critical - prevents unauthorized database access
Testing: Penetration testing performed, no vulnerabilities found"

# Database migration
git commit -m "Database: Add indexes for performance optimization

- Add index on PathologyForm.Status column
- Add composite index on PathologyForm (CreatedById, CreatedAt)
- Add index on FormHistory.Timestamp column

Performance Impact: Query performance improved by 60% on form listing
Medical Data: No changes to medical data structure or content"
```

---

## Security Checklist Before Pushing to GitHub

Execute before every commit:

```powershell
# Search for potential secrets
git grep -i "password"
git grep -i "connectionstring"
git grep -i "secret"
git grep -i "apikey"

# Check staged files
git diff --cached

# Check for large files
git ls-files | xargs ls -lh | sort -k5 -hr | head -20
```

Ensure none of these are committed:
- [ ] Password in any file
- [ ] Database connection strings with real credentials
- [ ] API keys or service tokens
- [ ] SSL certificates or private keys
- [ ] Patient data or test records
- [ ] Environment-specific configurations
- [ ] Large binary files (uploads, compiled outputs)

---

## Production Deployment from GitHub

### For Windows Server (IIS)

```powershell
# On production server, clone repository
git clone https://github.com/YOUR_USERNAME/Surgical-Pathology-Requisition-Form.git
cd Surgical-Pathology-Requisition-Form

# Checkout specific release tag
git checkout v1.0.0

# Navigate to application
cd src/PathologyFormApp

# Publish application
dotnet publish -c Release -o C:\inetpub\PathologyApp

# Copy production configuration (prepared separately, not in Git)
Copy-Item C:\SecureConfig\appsettings.Production.json C:\inetpub\PathologyApp\appsettings.json -Force
```

### For Linux Server (Nginx)

```bash
# Clone repository
git clone https://github.com/YOUR_USERNAME/Surgical-Pathology-Requisition-Form.git
cd Surgical-Pathology-Requisition-Form

# Checkout release tag
git checkout v1.0.0

# Publish for Linux
cd src/PathologyFormApp
dotnet publish -c Release -r linux-x64 --self-contained false -o /var/www/pathologyapp

# Copy production configuration (stored securely, not in Git)
sudo cp /secure/appsettings.Production.json /var/www/pathologyapp/appsettings.json
```

### For Azure App Service

```bash
# Clone and publish
git clone https://github.com/YOUR_USERNAME/Surgical-Pathology-Requisition-Form.git
cd Surgical-Pathology-Requisition-Form
git checkout v1.0.0

cd src/PathologyFormApp
dotnet publish -c Release -o ./publish
cd publish
zip -r ../deploy.zip *

# Deploy to Azure
az webapp deploy \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --src-path ../deploy.zip \
  --type zip
```

---

## Ongoing Maintenance

### Updating Application After Deployment

```powershell
# On production server
cd Surgical-Pathology-Requisition-Form

# Fetch latest changes
git fetch origin

# View available tags
git tag

# Checkout new version
git checkout v1.1.0

# Rebuild and deploy
cd src/PathologyFormApp
dotnet publish -c Release -o C:\inetpub\PathologyApp

# Restart application (IIS)
Restart-WebAppPool -Name "PathologyAppPool"
```

---

## Summary

You have successfully:
1. Initialized Git repository with comprehensive .gitignore
2. Organized project structure following enterprise standards
3. Created GitHub repository
4. Committed and pushed code securely (no credentials)
5. Implemented branch strategy
6. Created tagged releases
7. Prepared for production deployment from GitHub

Your code is now:
- Safely version controlled
- Professionally organized
- Ready for team collaboration
- Prepared for enterprise deployment
- Compliant with healthcare security standards

Next steps:
- Review DEPLOYMENT_GUIDE.md for detailed deployment procedures
- Review SECURITY_COMPLIANCE.md for healthcare compliance
- Implement CI/CD pipeline (GitHub Actions) for automated deployments

