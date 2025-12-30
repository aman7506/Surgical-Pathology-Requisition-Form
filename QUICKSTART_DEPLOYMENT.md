# QUICKSTART ENTERPRISE DEPLOYMENT GUIDE
## From Local Development to Production in 10 Steps

**Project**: Surgical Pathology Requisition Form  
**Target**: Hospital Production Environment  
**Timeline**: Can be completed in 1-2 days with preparation

---

## Overview

This guide provides the exact commands and steps to take your application from local development to:
1. GitHub repository (version control)
2. Production deployment (Windows/Linux/Azure)
3. Enterprise-ready with full documentation

**No prior DevOps experience required** - every command is provided.

---

## PART 1: GITHUB SETUP (30 Minutes)

### Step 1: Initialize Git Repository

Open PowerShell in your project directory:

```powershell
cd "e:\Aman Project Files\Surgical Pathology Requisition Form"

# Configure Git (one-time setup)
git config --global user.name "Your Name"
git config --global user.email "your.email@hospital.com"

# Initialize repository
git init
```

### Step 2: Create .gitignore

Create `.gitignore` file in project root:

```powershell
@"
# ASP.NET Core
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/
.vs/
publish/
publish_output/
publish_win_x64/

# User Uploads - CRITICAL (Contains Patient Data)
wwwroot/uploads/

# Configuration with Secrets
appsettings.Production.json
appsettings.Staging.json

# Database
*.db
*.mdf
*.ldf

# Logs
*.log

# Temporary
*.tmp
*.bak
*.backup
"@ | Out-File -FilePath ".gitignore" -Encoding UTF8
```

### Step 3: Create GitHub Repository

1. Go to https://github.com
2. Click "+" → "New repository"
3. Name: `Surgical-Pathology-Requisition-Form`
4. Visibility: **Private** (recommended for healthcare)
5. DO NOT initialize with README
6. Click "Create repository"

### Step 4: Connect and Push

```powershell
# Add all files
git add .

# Create first commit
git commit -m "Initial commit: Surgical Pathology Requisition Form v1.0

- ASP.NET Core 8.0 MVC application
- Role-based workflow (Nurse, Doctor)
- 60+ medical fields for pathology documentation
- Entity Framework Core with SQL Server
- Production-ready with comprehensive documentation

Medical Disclaimer: Designed for use by trained medical professionals"

# Add remote (replace YOUR_USERNAME with your GitHub username)
git remote add origin https://github.com/YOUR_USERNAME/Surgical-Pathology-Requisition-Form.git

# Push to GitHub
git branch -M main
git push -u origin main
```

**DONE!** Your code is now on GitHub.

---

## PART 2: PRODUCTION DEPLOYMENT

Choose your deployment target:

### Option A: Windows Server / IIS (Most Common for Hospitals)
### Option B: Linux / Nginx  
### Option C: Azure App Service (Cloud)

---

## OPTION A: WINDOWS SERVER IIS DEPLOYMENT (90 Minutes)

### Step 1: Install Prerequisites on Server

Execute on Windows Server (PowerShell as Administrator):

```powershell
# Install IIS
Install-WindowsFeature -Name Web-Server -IncludeManagementTools

# Download .NET 8.0 Hosting Bundle
# Go to: https://dotnet.microsoft.com/download/dotnet/8.0
# Download "ASP.NET Core Runtime 8.0.x - Windows Hosting Bundle"
# Install the downloaded file

# Restart IIS
iisreset
```

### Step 2: Prepare Production Environment

```powershell
# Create directories
New-Item -ItemType Directory -Path "C:\inetpub\PathologyApp" -Force
New-Item -ItemType Directory -Path "C:\inetpub\PathologyApp\logs" -Force
New-Item -ItemType Directory -Path "C:\PathologyApp\SecureConfig" -Force

# Set permissions
icacls "C:\inetpub\PathologyApp" /grant "BUILTIN\IIS_IUSRS:(OI)(CI)R" /T
```

### Step 3: Publish Application

On your development machine:

```powershell
cd "e:\Aman Project Files\Surgical Pathology Requisition Form\PathologyFormApp"

# Clean and publish
dotnet clean
dotnet publish -c Release -o "./publish"

# Create ZIP for transfer
Compress-Archive -Path "./publish/*" -DestinationPath "PathologyApp-Production.zip" -Force
```

Transfer `PathologyApp-Production.zip` to server (via RDP, network share, or secure file transfer).

### Step 4: Deploy to Server

On server:

```powershell
# Extract to IIS directory
Expand-Archive -Path "PathologyApp-Production.zip" -DestinationPath "C:\inetpub\PathologyApp" -Force
```

### Step 5: Configure Production appsettings.json

On server, create `C:\PathologyApp\SecureConfig\appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=pathology_db;User Id=pathology_user;Password=YOUR_STRONG_PASSWORD;Encrypt=True;TrustServerCertificate=False;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "pathology.yourhospital.com"
}
```

Copy to application:

```powershell
Copy-Item "C:\PathologyApp\SecureConfig\appsettings.Production.json" "C:\inetpub\PathologyApp\appsettings.json" -Force
```

### Step 6: Create SQL Server Database

On SQL Server (SSMS):

```sql
-- Create database
CREATE DATABASE [pathology_db]
GO

-- Create user
CREATE LOGIN [pathology_user] WITH PASSWORD = 'YOUR_STRONG_PASSWORD'
GO

USE [pathology_db]
GO

CREATE USER [pathology_user] FOR LOGIN [pathology_user]
GO

-- Grant permissions
ALTER ROLE [db_datareader] ADD MEMBER [pathology_user]
ALTER ROLE [db_datawriter] ADD MEMBER [pathology_user]
GRANT EXECUTE TO [pathology_user]
GO

-- Execute your database scripts
-- Run: CreateDatabase.sql
-- Run: CreateUsersAndRoles.sql
-- Run: UpdateStoredProcedure.sql
```

### Step 7: Create IIS Application Pool and Website

```powershell
Import-Module WebAdministration

# Create App Pool
New-WebAppPool -Name "PathologyAppPool"
Set-ItemProperty -Path "IIS:\AppPools\PathologyAppPool" -Name "managedRuntimeVersion" -Value ""

# Create Website
New-Website -Name "PathologyApp" `
  -PhysicalPath "C:\inetpub\PathologyApp" `
  -ApplicationPool "PathologyAppPool" `
  -Port 80

# Note: Configure HTTPS with SSL certificate separately
# For now, testing on HTTP is fine
```

### Step 8: Configure Firewall

```powershell
New-NetFirewallRule -DisplayName "PathologyApp HTTP" -Direction Inbound -LocalPort 80 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "PathologyApp HTTPS" -Direction Inbound -LocalPort 443 -Protocol TCP -Action Allow
```

### Step 9: Start Application

```powershell
Start-Website -Name "PathologyApp"
iisreset
```

### Step 10: Test Deployment

Open browser and navigate to:
```
http://YOUR_SERVER_IP
```

You should see the login page.

Test login with default credentials:
- Nurse: `nurse@hospital.com` / `Nurse@123`
- Doctor: `doctor@hospital.com` / `Doctor@123`

**IMPORTANT**: Change these passwords immediately in production!

---

## OPTION B: LINUX / NGINX DEPLOYMENT (90 Minutes)

### Step 1: Install Prerequisites

On Ubuntu server:

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install .NET 8.0
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y aspnetcore-runtime-8.0

# Install Nginx
sudo apt install -y nginx

# Verify installations
dotnet --list-runtimes
nginx -v
```

### Step 2: Prepare Application Directory

```bash
# Create directories
sudo mkdir -p /var/www/pathologyapp
sudo mkdir -p /var/www/pathologyapp/logs
sudo mkdir -p /var/pathologyapp/secureconfig

# Create application user
sudo useradd -r -s /bin/false pathologyapp

# Set ownership
sudo chown -R pathologyapp:www-data /var/www/pathologyapp
```

### Step 3: Publish and Transfer Application

On development machine:

```powershell
cd "PathologyFormApp"
dotnet publish -c Release -r linux-x64 --self-contained false -o "./publish-linux"

# Create archive
tar -czf PathologyApp-Linux.tar.gz -C ./publish-linux .
```

Transfer to Linux server using SCP:

```bash
scp PathologyApp-Linux.tar.gz user@your-server:/tmp/
```

### Step 4: Extract on Server

```bash
# Extract
sudo tar -xzf /tmp/PathologyApp-Linux.tar.gz -C /var/www/pathologyapp

# Set permissions
sudo chown -R pathologyapp:www-data /var/www/pathologyapp
sudo chmod +x /var/www/pathologyapp/PathologyFormApp

# Clean up
rm /tmp/PathologyApp-Linux.tar.gz
```

### Step 5: Configure Production Settings

```bash
sudo nano /var/pathologyapp/secureconfig/appsettings.Production.json
```

Add:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=pathology_db;User Id=pathology_user;Password=YOUR_STRONG_PASSWORD;Encrypt=True;TrustServerCertificate=False;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "pathology.yourhospital.com"
}
```

Copy to application:

```bash
sudo cp /var/pathologyapp/secureconfig/appsettings.Production.json /var/www/pathologyapp/appsettings.json
sudo chown pathologyapp:www-data /var/www/pathologyapp/appsettings.json
sudo chmod 600 /var/www/pathologyapp/appsettings.json
```

### Step 6: Create Systemd Service

```bash
sudo nano /etc/systemd/system/pathologyapp.service
```

Add:

```ini
[Unit]
Description=Pathology Requisition Form Application
After=network.target

[Service]
Type=notify
User=pathologyapp
Group=www-data
WorkingDirectory=/var/www/pathologyapp
ExecStart=/usr/bin/dotnet /var/www/pathologyapp/PathologyFormApp.dll
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

Enable and start:

```bash
sudo systemctl daemon-reload
sudo systemctl enable pathologyapp
sudo systemctl start pathologyapp
sudo systemctl status pathologyapp
```

### Step 7: Configure Nginx

```bash
sudo nano /etc/nginx/sites-available/pathologyapp
```

Add:

```nginx
server {
    listen 80;
    server_name pathology.yourhospital.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    client_max_body_size 50M;
}
```

Enable site:

```bash
sudo ln -s /etc/nginx/sites-available/pathologyapp /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

### Step 8: Configure Firewall

```bash
sudo ufw allow 'Nginx Full'
sudo ufw enable
sudo ufw statusStep 9: Setup Database

Execute the same SQL commands as in Windows deployment on your SQL Server.

### Step 10: Test Deployment

```bash
curl http://YOUR_SERVER_IP
```

**SUCCESS!** Application should respond.

---

## OPTION C: AZURE APP SERVICE (60 Minutes)

### Step 1: Install Azure CLI

On Windows:

```powershell
Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi
Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'
```

### Step 2: Login to Azure

```bash
az login
```

### Step 3: Create Resources

```bash
# Create resource group
az group create --name PathologyAppRG --location eastus

# Create App Service Plan
az appservice plan create \
  --name PathologyAppPlan \
  --resource-group PathologyAppRG \
  --sku P1V2 \
  --is-linux


# Create Web App
az webapp create \
  --name pathologyapp \
  --resource-group PathologyAppRG \
  --plan PathologyAppPlan \
  --runtime "DOTNETCORE:8.0"
```

### Step 4: Create Azure SQL Database

```bash
az sql server create \
  --name pathologydbserver \
  --resource-group PathologyAppRG \
  --location eastus \
  --admin-user sqladmin \
  --admin-password "YourStrongP@ssw0rd!"

az sql db create \
  --resource-group PathologyAppRG \
  --server pathologydbserver \
  --name pathology_db \
  --service-objective S1
```

### Step 5: Configure Firewall

```bash
az sql server firewall-rule create \
  --resource-group PathologyAppRG \
  --server pathologydbserver \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

### Step 6: Set Connection String

```bash
az webapp config connection-string set \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:pathologydbserver.database.windows.net,1433;Database=pathology_db;User ID=sqladmin;Password=YourStrongP@ssw0rd!;Encrypt=True;"
```

### Step 7: Deploy Application

```bash
cd PathologyFormApp
dotnet publish -c Release -o ./publish
cd publish
zip -r ../deploy.zip .

az webapp deploy \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --src-path ../deploy.zip \
  --type zip
```

### Step 8: Configure HTTPS

```bash
az webapp update \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --https-only true
```

### Step 9: Get Application URL

```bash
az webapp show \
  --name pathologyapp \
  --resource-group PathologyAppRG \
  --query defaultHostName \
  --output tsv
```

### Step 10: Test

Navigate to the URL from previous command in your browser.

---

## POST-DEPLOYMENT CHECKLIST

### Security (Critical - Do This First)

- [ ] Change default user passwords
- [ ] Configure SSL/TLS certificate
- [ ] Update connection string with strong password
- [ ] Restrict database access to application server IP only
- [ ] Enable database encryption (TDE)
- [ ] Configure firewall (only ports 80, 443)
- [ ] Review and secure file upload directory
- [ ] Disable detailed error messages in production
- [ ] Configure secure session timeout

### Operational

- [ ] Configure database backup schedule
- [ ] Set up application logging
- [ ] Configure monitoring and alerts
- [ ] Document administrator contacts
- [ ] Create runbook for common issues
- [ ] Schedule regular security updates
- [ ] Plan disaster recovery procedure

### Compliance (Healthcare Specific)

- [ ] Review HIPAA compliance requirements
- [ ] Document data retention policies
- [ ] Configure audit logging
- [ ] Establish access control procedures
- [ ] Create user training materials
- [ ] Perform security risk assessment
- [ ] Document business continuity plan

---

## TROUBLESHOOTING

### Application Won't Start

**Windows IIS**:
```powershell
# Check logs
Get-Content "C:\inetpub\PathologyApp\logs\stdout_*.log" -Tail 50

# Check Event Viewer
Get-EventLog -LogName Application -Newest 20 | Where-Object { $_.Source -like "*PathologyApp*" }
```

**Linux**:
```bash
sudo journalctl -u pathologyapp.service -n 50
```

**Azure**:
```bash
az webapp log tail --resource-group PathologyAppRG --name pathologyapp
```

### Database Connection Fails

1. Verify connection string is correct
2. Test database connectivity from application server
3. Check firewall allows connection
4. Verify database user has correct permissions
5. Check SQL Server is running and accessible

### Login Not Working

1. Verify users were created (check AspNetUsers table)
2. Re-run CreateUsersAndRoles.sql script
3. Check application logs for authentication errors
4. Verify password meets complexity requirements

---

## DOCUMENTATION REFERENCE

Your project now includes complete enterprise documentation:

1. **README.md**: Project overview, features, quick start
2. **GITHUB_DEPLOYMENT_WORKFLOW.md**: Complete Git and GitHub setup
3. **ENTERPRISE_DEPLOYMENT_GUIDE.md**: Detailed production deployment instructions
4. **COMPLETE_PROJECT_GUIDE.md**: All-in-one technical reference
5. **This file (QUICK START)**: Step-bystep deployment guide

---

## SUPPORT

### For Technical Issues
- Check documentation in `/docs` folder
- Review application logs
- Consult ENTERPRISE_DEPLOYMENT_GUIDE.md troubleshooting section

### For Security Issues
- Report immediately to security team
- Do NOT post security issues publicly
- Document incident according to facility procedures

### For Medical/Clinical Issues
- Contact clinical leadership
- Follow facility quality assurance procedures
- Document according to medical record retention policies

---

## SUCCESS CRITERIA

You have successfully deployed when:

- [ ] Application accessible via HTTPS
- [ ] Can login with test credentials
- [ ] Can create a pathology form
- [ ] Form saves to database
- [ ] Can upload a file
- [ ] Audit trail records actions in FormHistory table
- [ ] Role-based access working (Nurse cannot access Doctor functions)
- [ ] Application logs are being written
- [ ] Database backups are configured


**CONGRATULATIONS!** Your Surgical Pathology Requisition Form is now live in production.

---

*Quickstart Guide - Enterprise Healthcare Software Deployment*  
*Last Updated: December 2025*
