# Enterprise Production Deployment Guide
## Surgical Pathology Requisition Form

**Classification**: Healthcare Information System  
**Compliance Requirements**: HIPAA, Healthcare Data Security Standards  
**Deployment Environments**: Windows Server (IIS), Linux (Nginx), Azure App Service

---

## Table of Contents

1. [Pre-Deployment Requirements](#pre-deployment-requirements)
2. [Windows Server IIS Deployment](#windows-server-iis-deployment)
3. [Linux Nginx Deployment](#linux-nginx-deployment)
4. [Azure App Service Deployment](#azure-app-service-deployment)
5. [Database Setup](#database-setup)
6. [Security Hardening](#security-hardening)
7. [Post-Deployment Verification](#post-deployment-verification)
8. [Monitoring and Maintenance](#monitoring-and-maintenance)

---

## Pre-Deployment Requirements

### Infrastructure Requirements

**Hardware Specifications (Minimum)**:
- CPU: 4 cores (8 recommended for production)
- RAM: 8 GB (16 GB recommended)
- Storage: 100 GB SSD
- Network: 100 Mbps dedicated

**Software Requirements**:
- Operating System: Windows Server 2019/2022 or Ubuntu 20.04/22.04 LTS
- .NET 8.0 Runtime or Hosting Bundle
- SQL Server 2019 or later
- SSL/TLS Certificate (valid, not self-signed for production)

###Security Requirements

- [ ] Firewall configured (only ports 80, 443 open to public)
- [ ] SSL/TLS certificate obtained and validated
- [ ] Database server secured with strong authentication
- [ ] Backup system configured
- [ ] Monitoring and logging infrastructure ready
- [ ] Incident response plan documented

### Compliance Checklist

- [ ] HIPAA Security Rule compliance verified
- [ ] Data encryption at rest and in transit configured
- [ ] Access control policies documented
- [ ] Audit logging enabled
- [  ] Business Associate Agreement (BAA) with hosting provider (if cloud)
- [ ] Risk assessment completed
- [ ] Disaster recovery plan documented

---

## Windows Server IIS Deployment

### Environment: Windows Server 2019/2022

### Step 1: Install Required Components

Open PowerShell as Administrator:

```powershell
# Install IIS with required features
Install-WindowsFeature -Name Web-Server -IncludeManagementTools
Install-WindowsFeature -Name Web-Asp-Net45
Install-WindowsFeature -Name Web-Net-Ext45
Install-WindowsFeature -Name Web-ISAPI-Ext
Install-WindowsFeature -Name Web-ISAPI-Filter
Install-WindowsFeature -Name Web-Http-Redirect
Install-WindowsFeature -Name Web-Custom-Logging
Install-WindowsFeature -Name Web-Log-Libraries

# Restart to apply changes
Restart-Computer -Force
```

### Step 2: Install .NET 8.0 Hosting Bundle

```powershell
# Download .NET 8.0 Hosting Bundle
Invoke-WebRequest -Uri "https://download.visualstudio.microsoft.com/download/pr/[latest-version]/dotnet-hosting-8.0-win.exe" -OutFile "$env:TEMP\dotnet-hosting.exe"

# Install silently
Start-Process -FilePath "$env:TEMP\dotnet-hosting.exe" -ArgumentList "/quiet", "/install" -Wait

# Restart IIS
iisreset

# Verify installation
dotnet --info
```

### Step 3: Create Application Directory Structure

```powershell
# Create application directory
New-Item -ItemType Directory -Path "C:\inetpub\PathologyApp" -Force
New-Item -ItemType Directory -Path "C:\inetpub\PathologyApp\logs" -Force
New-Item -ItemType Directory -Path "C:\PathologyApp\Backups" -Force
New-Item -ItemType Directory -Path "C:\PathologyApp\SecureConfig" -Force

# Set permissions
icacls "C:\inetpub\PathologyApp" /grant "BUILTIN\IIS_IUSRS:(OI)(CI)R" /T
icacls "C:\inetpub\PathologyApp\logs" /grant "BUILTIN\IIS_IUSRS:(OI)(CI)M" /T
icacls "C:\inetpub\PathologyApp\wwwroot\uploads" /grant "BUILTIN\IIS_IUSRS:(OI)(CI)M" /T
```

### Step 4: Publish Application

On development machine:

```powershell
cd "src\PathologyFormApp"

# Clean previous builds
dotnet clean

# Publish for production
dotnet publish -c Release -o "./publish" --self-contained false

# Create deployment package
Compress-Archive -Path "./publish/*" -DestinationPath "./PathologyApp-v1.0.0.zip" -Force
```

Transfer `PathologyApp-v1.0.0.zip` to production server using secure method (SFTP, encrypted transfer).

On production server:

```powershell
# Extract deployment package
Expand-Archive -Path "PathologyApp-v1.0.0.zip" -DestinationPath "C:\inetpub\PathologyApp" -Force
```

### Step 5: Configure Production Settings

Create `C:\PathologyApp\SecureConfig\appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PROD_SQL_SERVER;Database=pathology_db;User Id=pathology_app_user;Password=STRONG_SECURE_PASSWORD;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=False;Connection Timeout=30;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "EventLog": {
      "LogLevel": {
        "Default": "Warning"
      },
      "SourceName": "PathologyApp"
    }
  },
  "AllowedHosts": "pathology.yourhospital.com",
  "ApplicationSettings": {
    "MaxFileUploadSizeMB": 50,
    "SessionTimeoutMinutes": 30,
    "EnableDetailedErrors": false,
    "RequireHttps": true
  }
}
```

Copy configuration to application directory:

```powershell
Copy-Item "C:\PathologyApp\SecureConfig\appsettings.Production.json" "C:\inetpub\PathologyApp\appsettings.json" -Force
```

### Step 6: Create IIS Application Pool

```powershell
# Import IIS module
Import-Module WebAdministration

# Create Application Pool
New-WebAppPool -Name "PathologyAppPool"

# Configure Application Pool
Set-ItemProperty -Path "IIS:\AppPools\PathologyAppPool" -Name "managedRuntimeVersion" -Value ""
Set-ItemProperty -Path "IIS:\AppPools\PathologyAppPool" -Name "managedPipelineMode" -Value "Integrated"
Set-ItemProperty -Path "IIS:\AppPools\PathologyAppPool" -Name "startMode" -Value "AlwaysRunning"
Set-ItemProperty -Path "IIS:\AppPools\PathologyAppPool" -Name "processModel.identityType" -Value "ApplicationPoolIdentity"
Set-ItemProperty -Path "IIS:\AppPools\PathologyAppPool" -Name "processModel.idleTimeout" -Value "00:00:00"
Set-ItemProperty -Path "IIS:\AppPools\PathologyAppPool" -Name "recycling.periodicRestart.time" -Value "00:00:00"

# Start Application Pool
Start-WebAppPool -Name "PathologyAppPool"
```

### Step 7: Create IIS Website

```powershell
# Remove Default Website (if exists)
Remove-Website -Name "Default Web Site" -ErrorAction SilentlyContinue

# Create new website
New-Website -Name "PathologyApp" `
  -PhysicalPath "C:\inetpub\PathologyApp" `
  -ApplicationPool "PathologyAppPool" `
  -Port 80 `
  -HostHeader "pathology.yourhospital.com"

# Add HTTPS binding (requires SSL certificate)
New-WebBinding -Name "PathologyApp" `
  -Protocol "https" `
  -Port 443 `
  -HostHeader "pathology.yourhospital.com" `
  -SslFlags 1

# Bind SSL certificate (replace thumbprint with your certificate)
$cert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object { $_.Subject -like "*pathology.yourhospital.com*" }
New-Item -Path "IIS:\SslBindings\0.0.0.0!443" -Value $cert -SSLFlags 1
```

### Step 8: Configure web.config

Ensure `C:\inetpub\PathologyApp\web.config` contains:

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
          <environmentVariable name="ASPNETCORE_HTTPS_PORT" value="443" />
        </environmentVariables>
      </aspNetCore>
      <httpProtocol>
        <customHeaders>
          <add name="X-Content-Type-Options" value="nosniff" />
          <add name="X-Frame-Options" value="SAMEORIGIN" />
          <add name="X-XSS-Protection" value="1; mode=block" />
          <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
          <remove name="X-Powered-By" />
        </customHeaders>
      </httpProtocol>
      <security>
        <requestFiltering>
          <requestLimits maxAllowedContentLength="52428800" />
        </requestFiltering>
      </security>
      <rewrite>
        <rules>
          <rule name="HTTPS Redirect" stopProcessing="true">
            <match url="(.*)" />
            <conditions>
              <add input="{HTTPS}" pattern="^OFF$" />
            </conditions>
            <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Permanent" />
          </rule>
        </rules>
      </rewrite>
    </system.webServer>
  </location>
</configuration>
```

### Step 9: Configure Windows Firewall

```powershell
# Allow HTTP (port 80)
New-NetFirewallRule -DisplayName "PathologyApp HTTP" -Direction Inbound -LocalPort 80 -Protocol TCP -Action Allow

# Allow HTTPS (port 443)
New-NetFirewallRule -DisplayName "PathologyApp HTTPS" -Direction Inbound -LocalPort 443 -Protocol TCP -Action Allow

# Block all other inbound connections (if not already configured)
Set-NetFirewallProfile -Profile Domain,Public,Private -DefaultInboundAction Block -DefaultOutboundAction Allow
```

### Step 10: Grant Database Access to IIS Application Pool

Execute on SQL Server:

```sql
USE [master]
GO

-- Create login for IIS Application Pool Identity
CREATE LOGIN [IIS APPPOOL\PathologyAppPool] FROM WINDOWS
GO

USE [pathology_db]
GO

-- Create database user
CREATE USER [IIS APPPOOL\PathologyAppPool] FOR LOGIN [IIS APPPOOL\PathologyAppPool]
GO

-- Grant permissions
ALTER ROLE [db_datareader] ADD MEMBER [IIS APPPOOL\PathologyAppPool]
ALTER ROLE [db_datawriter] ADD MEMBER [IIS APPPOOL\PathologyAppPool]
GRANT EXECUTE TO [IIS APPPOOL\PathologyAppPool]
GO
```

### Step 11: Start Application

```powershell
# Start website
Start-Website -Name "PathologyApp"

# Restart IIS for all changes to take effect
iisreset

# Verify application pool is running
Get-WebAppPoolState -Name "PathologyAppPool"
```

### Step 12: Verify IIS Deployment

```powershell
# Test HTTP to HTTPS redirect
Invoke-WebRequest -Uri "http://pathology.yourhospital.com" -MaximumRedirection 0

# Test HTTPS
Invoke-WebRequest -Uri "https://pathology.yourhospital.com"

# Check application logs
Get-Content "C:\inetpub\PathologyApp\logs\stdout_*.log" -Tail 50
```

---

## Linux Nginx Deployment

### Environment: Ubuntu 22.04 LTS

### Step 1: Update System and Install Prerequisites

```bash
# Update package list
sudo apt update
sudo apt upgrade -y

# Install required packages
sudo apt install -y curl wget apt-transport-https software-properties-common
```

### Step 2: Install .NET 8.0 Runtime

```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Update package list
sudo apt update

# Install .NET 8.0 runtime
sudo apt install -y aspnetcore-runtime-8.0

# Verify installation
dotnet --list-runtimes
```

### Step 3: Install and Configure Nginx

```bash
# Install Nginx
sudo apt install -y nginx

# Start and enable Nginx
sudo systemctl start nginx
sudo systemctl enable nginx

# Verify Nginx is running
sudo systemctl status nginx
```

### Step 4: Create Application Directory

```bash
# Create application directory
sudo mkdir -p /var/www/pathologyapp
sudo mkdir -p /var/www/pathologyapp/logs
sudo mkdir -p /var/pathologyapp/backups
sudo mkdir -p /var/pathologyapp/secureconfig

# Create dedicated user for application
sudo useradd -r -s /bin/false pathologyapp

# Set ownership
sudo chown -R pathologyapp:www-data /var/www/pathologyapp
sudo chmod -R 755 /var/www/pathologyapp

# Set permissions for uploads directory (will be created by application)
sudo mkdir -p /var/www/pathologyapp/wwwroot/uploads
sudo chown -R pathologyapp:www-data /var/www/pathologyapp/wwwroot/uploads
sudo chmod -R 775 /var/www/pathologyapp/wwwroot/uploads
```

### Step 5: Deploy Application Files

On development machine:

```bash
# Publish for Linux
dotnet publish -c Release -r linux-x64 --self-contained false -o ./publish-linux

# Create deployment archive
tar -czf PathologyApp-v1.0.0-linux.tar.gz -C ./publish-linux .
```

Transfer archive to Linux server using SCP:

```bash
scp PathologyApp-v1.0.0-linux.tar.gz user@production-server:/tmp/
```

On production server:

```bash
# Extract application
sudo tar -xzf /tmp/PathologyApp-v1.0.0-linux.tar.gz -C /var/www/pathologyapp

# Set permissions
sudo chown -R pathologyapp:www-data /var/www/pathologyapp
sudo chmod +x /var/www/pathologyapp/PathologyFormApp.dll

# Remove temporary archive
rm /tmp/PathologyApp-v1.0.0-linux.tar.gz
```

### Step 6: Configure Production Settings

Create `/var/pathologyapp/secureconfig/appsettings.Production.json`:

```bash
sudo nano /var/pathologyapp/secureconfig/appsettings.Production.json
```

Content:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-sql-server.domain.com;Database=pathology_db;User Id=pathology_app_user;Password=STRONG_SECURE_PASSWORD;Encrypt=True;TrustServerCertificate=False;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "pathology.yourhospital.com"
}
```

Copy to application directory:

```bash
sudo cp /var/pathologyapp/secureconfig/appsettings.Production.json /var/www/pathologyapp/appsettings.json
sudo chown pathologyapp:www-data /var/www/pathologyapp/appsettings.json
sudo chmod 600 /var/www/pathologyapp/appsettings.json
```

### Step 7: Create Systemd Service

```bash
sudo nano /etc/systemd/system/pathologyapp.service
```

Content:

```ini
[Unit]
Description=Surgical Pathology Requisition Form Application
Documentation=https://github.com/yourusername/Surgical-Pathology-Requisition-Form
After=network.target

[Service]
Type=notify
User=pathologyapp
Group=www-data
WorkingDirectory=/var/www/pathologyapp
ExecStart=/usr/bin/dotnet /var/www/pathologyapp/PathologyFormApp.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=pathologyapp
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment=ASPNETCORE_URLS=http://localhost:5000

# Security settings
PrivateTmp=true
NoNewPrivileges=true
ProtectSystem=strict
ProtectHome=true
ReadWritePaths=/var/www/pathologyapp/wwwroot/uploads
ReadWritePaths=/var/www/pathologyapp/logs

[Install]
WantedBy=multi-user.target
```

Enable and start service:

```bash
# Reload systemd daemon
sudo systemctl daemon-reload

# Enable service to start on boot
sudo systemctl enable pathologyapp.service

# Start service
sudo systemctl start pathologyapp.service

# Verify service is running
sudo systemctl status pathologyapp.service

# View logs
sudo journalctl -u pathologyapp.service -f
```

### Step 8: Configure Nginx Reverse Proxy

Remove default site:

```bash
sudo rm /etc/nginx/sites-enabled/default
```

Create Nginx configuration:

```bash
sudo nano /etc/nginx/sites-available/pathologyapp
```

Content:

```nginx
# HTTP to HTTPS redirect
server {
    listen 80;
    listen [::]:80;
    server_name pathology.yourhospital.com;
    
    # Redirect all HTTP to HTTPS
    return 301 https://$server_name$request_uri;
}

# HTTPS server
server {
    listen 443 ssl http/2;
    listen [::]:443 ssl http2;
    server_name pathology.yourhospital.com;

    # SSL Certificate (update paths to your certificate)
    ssl_certificate /etc/ssl/certs/pathology.yourhospital.com.crt;
    ssl_certificate_key /etc/ssl/private/pathology.yourhospital.com.key;

    # SSL Security Settings
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers 'ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384';
    ssl_prefer_server_ciphers off;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;

    # Security Headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Referrer-Policy "strict-origin-when-cross-origin" always;

    # File upload size limit (50 MB)
    client_max_body_size 50M;

    # Logging
    access_log /var/log/nginx/pathologyapp-access.log;
    error_log /var/log/nginx/pathologyapp-error.log;

    # Proxy settings
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Real-IP $remote_addr;
        
        # Timeouts
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # Static files
    location ~* \.(jpg|jpeg|png|gif|ico|css|js|woff|woff2|ttf|svg)$ {
        proxy_pass http://localhost:5000;
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

Enable site and test configuration:

```bash
# Create symbolic link to enable site
sudo ln -s /etc/nginx/sites-available/pathologyapp /etc/nginx/sites-enabled/

# Test Nginx configuration
sudo nginx -t

# Reload Nginx
sudo systemctl reload nginx
```

### Step 9: Configure Firewall (UFW)

```bash
# Allow SSH (if not already allowed)
sudo ufw allow OpenSSH

# Allow HTTP
sudo ufw allow 'Nginx HTTP'

# Allow HTTPS
sudo ufw allow 'Nginx HTTPS'

# Enable firewall
sudo ufw enable

# Verify firewall status
sudo ufw status
```

### Step 10: Install and Configure SSL Certificate

Using Certbot (Let's Encrypt):

```bash
# Install Certbot
sudo apt install -y certbot python3-certbot-nginx

# Obtain and install certificate
sudo certbot --nginx -d pathology.yourhospital.com

# Follow prompts to configure HTTPS

# Test automatic renewal
sudo certbot renew --dry-run
```

Certificates will be automatically renewed by Certbot.

### Step 11: Verify Linux Deployment

```bash
# Check application service
sudo systemctl status pathologyapp.service

# Check Nginx
sudo systemctl status nginx

# View application logs
sudo journalctl -u pathologyapp.service -n 50

# View Nginx logs
sudo tail -f /var/log/nginx/pathologyapp-access.log
sudo tail -f /var/log/nginx/pathologyapp-error.log

# Test application
curl -I https://pathology.yourhospital.com
```

---

## Azure App Service Deployment

### Prerequisites

- Azure account with active subscription
- Azure CLI installed
- SQL Server on Azure or accessible SQL Server

### Step 1: Install Azure CLI

On Windows:

```powershell
Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi
Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'
```

On Linux:

```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### Step 2: Login to Azure

```bash
# Login interactively
az login

# Set subscription (if multiple subscriptions)
az account set --subscription "YOUR_SUBSCRIPTION_ID"

# Verify current subscription
az account show
```

### Step 3: Create Resource Group

```bash
az group create \
  --name PathologyAppRG \
  --location eastus \
  --tags Environment=Production Application=PathologyForm Compliance=HIPAA
```

### Step 4: Create App Service Plan

```bash
az appservice plan create \
  --name PathologyAppPlan \
  --resource-group PathologyAppRG \
  --sku P1V2 \
  --is-linux \
  --location eastus
```

Plan tiers:
- P1V2: Production (recommended minimum)
- P2V2: High traffic
- P3V2: Enterprise

### Step 5: Create Web App

```bash
az webapp create \
  --name pathologyapp \
  --resource-group PathologyAppRG \
  --plan PathologyAppPlan \
  --runtime "DOTNETCORE:8.0"
```

### Step 6: Create Azure SQL Database (if needed)

```bash
# Create SQL Server
az sql server create \
  --name pathologydbserver \
  --resource-group PathologyAppRG \
  --location eastus \
  --admin-user sqladmin \
  --admin-password "YourStrongP@ssw0rd123!" \
  --enable-public-network true

# Create database
az sql db create \
  --resource-group PathologyAppRG \
  --server pathologydbserver \
  --name pathology_db \
  --service-objective S1 \
  --backup-storage-redundancy Zone

# Configure firewall to allow Azure services
az sql server firewall-rule create \
  --resource-group PathologyAppRG \
  --server pathologydbserver \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

### Step 7: Configure Connection String

```bash
az webapp config connection-string set \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:pathologydbserver.database.windows.net,1433;Database=pathology_db;User ID=sqladmin;Password=YourStrongP@ssw0rd123!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

### Step 8: Configure Application Settings

```bash
# Set environment
az webapp config appsettings set \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --settings ASPNETCORE_ENVIRONMENT=Production

# Configure logging
az webapp log config \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --application-logging filesystem \
  --level warning \
  --web-server-logging filesystem

# Enable HTTPS only
az webapp update \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --https-only true
```

### Step 9: Deploy Application

Prepare deployment package:

```bash
cd src/PathologyFormApp

# Publish application
dotnet publish -c Release -o ./publish

# Create deployment ZIP
cd publish
zip -r ../pathologyapp-deploy.zip .
cd ..
```

Deploy to Azure:

```bash
az webapp deploy \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --src-path pathologyapp-deploy.zip \
  --type zip
```

### Step 10: Configure Custom Domain and SSL

```bash
# Add custom domain
az webapp config hostname add \
  --webapp-name pathologyapp \
  --resource-group PathologyAppRG \
  --hostname pathology.yourhospital.com

# Bind SSL certificate (managed certificate)
az webapp config ssl create \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --hostname pathology.yourhospital.com

az webapp config ssl bind \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --certificate-thumbprint <thumbprint-from-previous-command> \
  --ssl-type SNI
```

### Step 11: Configure Auto-Scaling (Optional)

```bash
# Enable autoscale
az monitor autoscale create \
  --resource-group PathologyAppRG \
  --resource pathologyapp \
  --resource-type Microsoft.Web/serverfarms \
  --name PathologyAppAutoscale \
  --min-count 2 \
  --max-count 10 \
  --count 2

# Add scale rule based on CPU
az monitor autoscale rule create \
  --resource-group PathologyAppRG \
  --autoscale-name PathologyAppAutoscale \
  --condition "Percentage CPU > 70 avg 5m" \
  --scale out 1
```

### Step 12: Verify Azure Deployment

```bash
# Get application URL
az webapp show \
  --name pathologyapp \
  --resource-group PathologyAppRG \
  --query defaultHostName \
  --output tsv

# View logs
az webapp log tail \
  --resource-group PathologyAppRG \
  --name pathologyapp

# Check application status
az webapp browse \
  --resource-group PathologyAppRG \
  --name pathologyapp
```

---

## Database Setup

### Production Database Configuration

Execute on production SQL Server:

```sql
-- Create database
CREATE DATABASE [pathology_db]
GO

-- Use database
USE [pathology_db]
GO

-- Execute schema creation scripts
-- (Upload and execute your CreateDatabase.sql script)

-- Create application user
CREATE LOGIN [pathology_app_user] WITH PASSWORD = 'STRONG_SECURE_PASSWORD'
GO

CREATE USER [pathology_app_user] FOR LOGIN [pathology_app_user]
GO

-- Grant minimum required permissions
ALTER ROLE [db_datareader] ADD MEMBER [pathology_app_user]
ALTER ROLE [db_datawriter] ADD MEMBER [pathology_app_user]
GRANT EXECUTE TO [pathology_app_user]
GO

-- Seed initial users (execute CreateUsersAndRoles.sql)
-- Execute stored procedure scripts
```

### Database Backup Configuration

```sql
-- Configure full backup (daily at 2 AM)
USE [master]
GO

EXEC msdb.dbo.sp_add_job
    @job_name = N'PathologyDB_FullBackup',
    @enabled = 1

EXEC msdb.dbo.sp_add_jobstep
    @job_name = N'PathologyDB_FullBackup',
    @step_name = N'Backup Database',
    @subsystem = N'TSQL',
    @command = N'BACKUP DATABASE [pathology_db] TO DISK = N''C:\PathologyApp\Backups\pathology_db_full.bak'' WITH COMPRESSION, INIT',
    @retry_attempts = 3,
    @retry_interval = 5

EXEC msdb.dbo.sp_add_schedule
    @schedule_name = N'Daily_2AM',
    @freq_type = 4,
    @freq_interval = 1,
    @active_start_time = 020000

EXEC msdb.dbo.sp_attach_schedule
    @job_name = N'PathologyDB_FullBackup',
    @schedule_name = N'Daily_2AM'
```

---

## Security Hardening

### Application Security

1. **Disable Detailed Errors in Production**:

Ensure in `appsettings.json`:

```json
{
  "DetailedErrors": false,
  "HostingEnvironment": "Production"
}
```

2. **Enable HSTS**:

In `Program.cs`:

```csharp
app.UseHsts();
```

3. **Configure Security Headers**:

Already configured in `web.config` and Nginx configuration above.

### Database Security

```sql
-- Enable Transparent Data Encryption (TDE)
USE [master]
GO

CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'STRONG_MASTER_KEY_PASSWORD'
GO

CREATE CERTIFICATE PathologyDBCert WITH SUBJECT = 'Pathology Database Certificate'
GO

USE [pathology_db]
GO

CREATE DATABASE ENCRYPTION KEY
WITH ALGORITHM = AES_256
ENCRYPTION BY SERVER CERTIFICATE PathologyDBCert
GO

ALTER DATABASE [pathology_db]
SET ENCRYPTION ON
GO
```

### Network Security

- Restrict database access to application server IP only
- Use private network/VNet for Azure
- Configure SQL Server firewall rules
- Enable SSL/TLS for database connections

---

## Post-Deployment Verification

### Comprehensive Testing Checklist

```powershell
# Test HTTPS enforcement
curl -I http://pathology.yourhospital.com
# Should redirect to HTTPS

# Test HTTPS
curl -I https://pathology.yourhospital.com
# Should return 200 OK

# Test application health
curl https://pathology.yourhospital.com/health

# Test authentication
# Navigate to https://pathology.yourhospital.com
# Attempt to access protected pages
# Verify redirect to login

# Test database connectivity
# Login with seeded user
# Create test pathology form
# Verify data saved to database

# Test file upload
# Upload test document
# Verify file saved correctly
# Verify file can be downloaded

# Test audit trail
# Perform actions
# Verify FormHistory table updated

# Test role-based access
# Login as Nurse
# Verify Nurse can create forms
# Verify Nurse cannot access Doctor-only features
# Login as Doctor
# Verify Doctor can review forms
```

### Performance Testing

```bash
# Install Apache Bench
sudo apt install apache2-utils

# Test concurrent users
ab -n 1000 -c 10 https://pathology.yourhospital.com/

# Analyze results
# Look for:
# - Requests per second
# - Time per request
# - Failed requests (should be 0)
```

---

## Monitoring and Maintenance

### Application Logging

View logs:

**Windows IIS**:
```powershell
Get-Content "C:\inetpub\PathologyApp\logs\stdout_*.log" -Tail 100 -Wait
```

**Linux**:
```bash
sudo journalctl -u pathologyapp.service -f
```

**Azure**:
```bash
az webapp log tail --resource-group PathologyAppRG --name pathologyapp
```

### Health Monitoring

Implement health check endpoint in application.

Configure monitoring:

**Azure Application Insights**:
```bash
az monitor app-insights component create \
  --app pathologyapp-insights \
  --location eastus \
  --resource-group PathologyAppRG \
  --application-type web
```

### Backup Strategy

- Database: Daily full backup, hourly differential
- Application files: Weekly backup
- Configuration: Version controlled in Git
- Uploaded files: Daily backup to secure storage

### Update Procedure

```powershell
# 1. Backup current version
# 2. Test new version in staging
# 3. Schedule maintenance window
# 4. Deploy new version
# 5. Verify deployment
# 6. Monitor for 24 hours
# 7. Update documentation
```

---

## Disaster Recovery Plan

### Recovery Time Objective (RTO): 4 hours  
### Recovery Point Objective (RPO): 1 hour

1. **Database restore from backup**
2. **Redeploy application from Git repository**
3. **Restore configuration from secure storage**
4. **Verify all services operational**
5. **Notify users of restoration**

---

## Summary

This guide provided complete production deployment instructions for:
- Windows Server with IIS
- Linux with Nginx
- Azure App Service

All deployments include:
- Security hardening
- SSL/TLS configuration
- Database setup
- Monitoring and logging
- Backup strategies
- Verification procedures

Your Surgical Pathology Requisition Form is now enterprise-ready for hospital deployment.

