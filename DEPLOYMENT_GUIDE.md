# Deployment Guide - Surgical Pathology Requisition Form

## 🚀 Deployment Overview

This guide covers deploying the Surgical Pathology Requisition Form application to various environments, from local development to production hosting.

---

## 📋 Pre-Deployment Checklist

### Required Software & Services
- [ ] **.NET 8.0 Runtime** (or SDK for self-contained)
- [ ] **SQL Server** (2019+ recommended)
- [ ] **IIS** (Windows Server) or equivalent web server
- [ ] **SSL Certificate** (for HTTPS in production)
- [ ] **Backup Storage** (for database and file backups)

### Configuration Requirements
- [ ] Database server accessible from web server
- [ ] Connection strings configured
- [ ] File upload directory writable
- [ ] Environment-specific appsettings.json
- [ ] Initial admin user credentials ready

---

## 🏠 Local Development Deployment

### Using Visual Studio

1. **Open Solution**
   ```
   File → Open → Project/Solution
   Navigate to: PathologyFormApp.csproj
   ```

2. **Configure Database**
   - Update `appsettings.Development.json`
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=pathology_db;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Apply Migrations**
   ```powershell
   # Package Manager Console
   Update-Database
   ```

4. **Run Application**
   - Press `F5` (Debug) or `Ctrl+F5` (Run without debugging)
   - Browser opens automatically to https://localhost:7123

### Using VS Code / Command Line

1. **Install Dependencies**
   ```powershell
   cd "e:\Aman Project Files\Surgical Pathology Requisition Form\PathologyFormApp"
   dotnet restore
   ```

2. **Configure Database**
   - Edit `appsettings.Development.json` as above

3. **Apply Migrations**
   ```powershell
   dotnet ef database update
   ```

4. **Run Application**
   ```powershell
   dotnet run
   
   # Or with specific profile
   dotnet run --launch-profile https
   ```

5. **Access Application**
   - Navigate to: https://localhost:7123
   - Or: http://localhost:5123

---

## 🏢 Windows Server / IIS Deployment

### Option 1: Framework-Dependent Deployment

#### Step 1: Prepare Server

1. **Install .NET 8.0 Hosting Bundle**
   ```
   Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   Select: "Hosting Bundle" for Windows
   Install and restart server
   ```

2. **Install IIS Features**
   ```powershell
   # Run as Administrator
   Install-WindowsFeature -Name Web-Server -IncludeManagementTools
   Install-WindowsFeature -Name Web-Asp-Net45
   ```

3. **Verify Installation**
   ```powershell
   dotnet --info
   # Should show .NET 8.0 runtime
   ```

#### Step 2: Publish Application

1. **Using Visual Studio**
   ```
   Right-click Project → Publish
   Target: Folder
   Configuration: Release
   Target Framework: net8.0
   Deployment Mode: Framework-dependent
   Target Runtime: Portable
   Publish
   ```

2. **Using Command Line**
   ```powershell
   cd PathologyFormApp
   dotnet publish -c Release -o ./publish
   ```

#### Step 3: Copy Files to Server

```powershell
# Copy publish folder to server
Copy-Item -Path "./publish/*" -Destination "C:\inetpub\PathologyApp" -Recurse -Force
```

#### Step 4: Configure IIS

1. **Create Application Pool**
   ```
   IIS Manager → Application Pools → Add Application Pool
   Name: PathologyAppPool
   .NET CLR Version: No Managed Code
   Managed Pipeline Mode: Integrated
   Start application pool immediately: ✓
   ```

2. **Configure Application Pool Identity**
   ```
   Advanced Settings → Process Model → Identity
   Select: ApplicationPoolIdentity (or custom service account)
   
   # Grant database access to this identity
   ```

3. **Create Website**
   ```
   IIS Manager → Sites → Add Website
   Site Name: PathologyApp
   Application Pool: PathologyAppPool
   Physical Path: C:\inetpub\PathologyApp
   Binding: 
     - Type: https
     - IP: All Unassigned
     - Port: 443
     - Host name: pathology.yourdomain.com
     - SSL Certificate: [Select your certificate]
   ```

4. **Configure Permissions**
   ```powershell
   # Grant read access to application files
   icacls "C:\inetpub\PathologyApp" /grant "IIS_IUSRS:(OI)(CI)R" /T
   
   # Grant write access to uploads folder
   icacls "C:\inetpub\PathologyApp\wwwroot\uploads" /grant "IIS_IUSRS:(OI)(CI)M" /T
   ```

#### Step 5: Configure Application

1. **Update appsettings.json**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=PROD_SQL_SERVER;Database=pathology_db;User Id=pathology_user;Password=STRONG_PASSWORD;Encrypt=True;TrustServerCertificate=False;"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Warning",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "pathology.yourdomain.com"
   }
   ```

2. **Update web.config** (if needed)
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
       </system.webServer>
     </location>
   </configuration>
   ```

3. **Create Logs Directory**
   ```powershell
   New-Item -Path "C:\inetpub\PathologyApp\logs" -ItemType Directory
   icacls "C:\inetpub\PathologyApp\logs" /grant "IIS_IUSRS:(OI)(CI)M" /T
   ```

#### Step 6: Database Setup

1. **Execute Database Scripts**
   ```sql
   -- In SQL Server Management Studio on production server
   -- 1. Execute CreateDatabase.sql
   -- 2. Execute CreateUsersAndRoles.sql
   -- 3. Verify tables and stored procedures created
   ```

2. **Configure Database User**
   ```sql
   USE [master]
   GO
   
   -- Create SQL login
   CREATE LOGIN [pathology_user] WITH PASSWORD = 'STRONG_PASSWORD'
   GO
   
   USE [pathology_db]
   GO
   
   -- Create database user
   CREATE USER [pathology_user] FOR LOGIN [pathology_user]
   GO
   
   -- Grant permissions
   ALTER ROLE [db_datareader] ADD MEMBER [pathology_user]
   ALTER ROLE [db_datawriter] ADD MEMBER [pathology_user]
   GRANT EXECUTE TO [pathology_user]
   GO
   ```

#### Step 7: Start Application

```powershell
# Restart IIS
iisreset

# Or restart just the application pool
Restart-WebAppPool -Name "PathologyAppPool"
```

#### Step 8: Verify Deployment

1. **Test URL**
   ```
   https://pathology.yourdomain.com
   ```

2. **Check Application Event Viewer**
   ```
   Event Viewer → Application
   Look for ASP.NET Core errors
   ```

3. **Check stdout Logs**
   ```
   C:\inetpub\PathologyApp\logs\
   ```

---

### Option 2: Self-Contained Deployment

**Advantages**: No .NET runtime required on server
**Disadvantages**: Larger deployment size

#### Publish Command
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish_win_x64
```

#### Deploy
Follow same IIS steps as above, but:
- No need to install .NET Hosting Bundle
- Update web.config to use .exe instead of dll:
  ```xml
  <aspNetCore processPath=".\PathologyFormApp.exe" arguments="" ... />
  ```

---

## 🐧 Linux Deployment (Ubuntu/Debian)

### Prerequisites

```bash
# Install .NET 8.0 Runtime
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-8.0

# Verify
dotnet --info
```

### Step 1: Publish Application

```powershell
# On development machine
dotnet publish -c Release -r linux-x64 --self-contained false -o ./publish_linux
```

### Step 2: Copy to Server

```bash
# Using SCP
scp -r ./publish_linux/* user@server:/var/www/pathologyapp/

# Or using rsync
rsync -avz ./publish_linux/ user@server:/var/www/pathologyapp/
```

### Step 3: Configure Service

1. **Create Service File**
   ```bash
   sudo nano /etc/systemd/system/pathologyapp.service
   ```

2. **Service Configuration**
   ```ini
   [Unit]
   Description=Surgical Pathology Requisition Form App
   After=network.target

   [Service]
   Type=notify
   User=www-data
   WorkingDirectory=/var/www/pathologyapp
   ExecStart=/usr/bin/dotnet /var/www/pathologyapp/PathologyFormApp.dll
   Restart=always
   RestartSec=10
   KillSignal=SIGINT
   SyslogIdentifier=pathologyapp
   Environment=ASPNETCORE_ENVIRONMENT=Production
   Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

   [Install]
   WantedBy=multi-user.target
   ```

3. **Enable and Start Service**
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl enable pathologyapp
   sudo systemctl start pathologyapp
   sudo systemctl status pathologyapp
   ```

### Step 4: Configure Nginx Reverse Proxy

1. **Install Nginx**
   ```bash
   sudo apt-get install nginx
   ```

2. **Create Nginx Configuration**
   ```bash
   sudo nano /etc/nginx/sites-available/pathologyapp
   ```

3. **Nginx Configuration**
   ```nginx
   server {
       listen 80;
       listen [::]:80;
       server_name pathology.yourdomain.com;
       return 301 https://$server_name$request_uri;
   }

   server {
       listen 443 ssl http2;
       listen [::]:443 ssl http2;
       server_name pathology.yourdomain.com;

       ssl_certificate /etc/ssl/certs/pathology.crt;
       ssl_certificate_key /etc/ssl/private/pathology.key;
       ssl_protocols TLSv1.2 TLSv1.3;
       ssl_ciphers HIGH:!aNULL:!MD5;

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

       # File upload size limit
       client_max_body_size 50M;
   }
   ```

4. **Enable Site**
   ```bash
   sudo ln -s /etc/nginx/sites-available/pathologyapp /etc/nginx/sites-enabled/
   sudo nginx -t
   sudo systemctl restart nginx
   ```

### Step 5: Configure Firewall

```bash
sudo ufw allow 'Nginx Full'
sudo ufw status
```

---

## ☁️ Cloud Deployment Options

### Azure App Service

#### Step 1: Create Resources

```bash
# Install Azure CLI
# https://docs.microsoft.com/en-us/cli/azure/install-azure-cli

# Login
az login

# Create resource group
az group create --name PathologyAppRG --location eastus

# Create App Service Plan
az appservice plan create \
  --name PathologyAppPlan \
  --resource-group PathologyAppRG \
  --sku B1 \
  --is-linux

# Create Web App
az webapp create \
  --name pathologyapp \
  --resource-group PathologyAppRG \
  --plan PathologyAppPlan \
  --runtime "DOTNETCORE:8.0"
```

#### Step 2: Configure Database

```bash
# Create Azure SQL Server
az sql server create \
  --name pathologydbserver \
  --resource-group PathologyAppRG \
  --location eastus \
  --admin-user sqladmin \
  --admin-password YourStrongPassword123!

# Create Database
az sql db create \
  --name pathology_db \
  --server pathologydbserver \
  --resource-group PathologyAppRG \
  --service-objective S0

# Configure firewall
az sql server firewall-rule create \
  --server pathologydbserver \
  --resource-group PathologyAppRG \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

#### Step 3: Deploy Application

```bash
# Publish and zip
dotnet publish -c Release -o ./publish
cd publish
Compress-Archive -Path * -DestinationPath ../app.zip

# Deploy
az webapp deploy \
  --resource-group PathologyAppRG \
  --name pathologyapp \
  --src-path ../app.zip \
  --type zip
```

#### Step 4: Configure Connection String

```bash
# Set connection string
az webapp config connection-string set \
  --name pathologyapp \
  --resource-group PathologyAppRG \
  --connection-string-type SQLAzure \
  --settings DefaultConnection="Server=tcp:pathologydbserver.database.windows.net,1433;Database=pathology_db;User ID=sqladmin;Password=YourStrongPassword123!;Encrypt=true;"
```

### AWS Elastic Beanstalk

1. **Install EB CLI**
   ```bash
   pip install awsebcli
   ```

2. **Initialize EB**
   ```bash
   eb init -p "64bit Amazon Linux 2 v2.5.0 running .NET Core" -r us-east-1 pathologyapp
   ```

3. **Create Environment**
   ```bash
   eb create pathologyapp-env
   ```

4. **Deploy**
   ```bash
   dotnet publish -c Release -o ./publish
   cd publish
   zip -r ../app.zip *
   eb deploy
   ```

---

## 🔄 Database Migration in Production

### Option 1: Manual Migration (Recommended for Production)

```powershell
# Generate SQL script from migrations
dotnet ef migrations script -o migration.sql

# Review and test SQL script
# Then execute in production SSMS
```

### Option 2: Automatic Migration (Use with Caution)

Update `Program.cs` to apply migrations on startup:

```csharp
// Add this before app.Run()
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PathologyContext>();
    if (app.Environment.IsProduction())
    {
        // Optionally apply migrations automatically
        // db.Database.Migrate();
    }
}
```

⚠️ **Warning**: Automatic migrations in production can cause downtime. Always backup first!

---

## 🔒 Production Security Checklist

### Application Security
- [ ] HTTPS enforced (no HTTP access)
- [ ] Strong SSL/TLS certificate installed
- [ ] Connection strings encrypted
- [ ] Debug mode disabled
- [ ] Detailed errors disabled
- [ ] HSTS enabled
- [ ] CORS configured appropriately

### Database Security
- [ ] Strong SQL user password
- [ ] Least privilege database permissions
- [ ] SQL Server firewall configured
- [ ] Encrypted connections enforced
- [ ] Regular backups scheduled
- [ ] Point-in-time restore tested

### Server Security
- [ ] Windows Updates enabled
- [ ] Firewall configured (only 80/443 open)
- [ ] Antivirus installed
- [ ] Remote Desktop secured
- [ ] Admin passwords strong
- [ ] Audit logging enabled

### Application Configuration
- [ ] Default admin password changed
- [ ] File upload limits set
- [ ] Session timeout configured
- [ ] Rate limiting implemented (future)
- [ ] Logging configured for monitoring

---

## 📊 Monitoring & Logging

### Application Insights (Azure)

```bash
# Install package
dotnet add package Microsoft.ApplicationInsights.AspNetCore

# Configure in Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Logging Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "EventLog": {
      "LogLevel": {
        "Default": "Warning"
      }
    }
  }
}
```

### Health Checks

Add to `Program.cs`:

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PathologyContext>();

app.MapHealthChecks("/health");
```

---

## 🔄 Update/Upgrade Process

### Application Update

```powershell
# 1. Backup current application
Copy-Item -Path "C:\inetpub\PathologyApp" -Destination "C:\Backups\PathologyApp_$(Get-Date -Format 'yyyyMMdd')" -Recurse

# 2. Stop application pool
Stop-WebAppPool -Name "PathologyAppPool"

# 3. Deploy new version
Copy-Item -Path "./publish/*" -Destination "C:\inetpub\PathologyApp" -Recurse -Force

# 4. Start application pool
Start-WebAppPool -Name "PathologyAppPool"

# 5. Verify deployment
Invoke-WebRequest -Uri "https://pathology.yourdomain.com/health"
```

### Database Update

```sql
-- 1. Backup database
BACKUP DATABASE [pathology_db] 
TO DISK = 'C:\Backups\pathology_db_$(Get-Date -Format 'yyyyMMdd').bak'
WITH COMPRESSION;

-- 2. Execute migration script
-- (Run migration.sql generated earlier)

-- 3. Verify schema version
SELECT * FROM __EFMigrationsHistory ORDER BY MigrationId DESC;
```

---

## 🆘 Rollback Procedures

### Application Rollback

```powershell
# Stop application pool
Stop-WebAppPool -Name "PathologyAppPool"

# Restore previous version
Remove-Item -Path "C:\inetpub\PathologyApp\*" -Recurse -Force
Copy-Item -Path "C:\Backups\PathologyApp_20241230\*" -Destination "C:\inetpub\PathologyApp" -Recurse

# Start application pool
Start-WebAppPool -Name "PathologyAppPool"
```

### Database Rollback

```sql
-- Restore from backup
USE [master]
GO

ALTER DATABASE [pathology_db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
GO

RESTORE DATABASE [pathology_db]
FROM DISK = 'C:\Backups\pathology_db_20241230.bak'
WITH REPLACE

ALTER DATABASE [pathology_db] SET MULTI_USER
GO
```

---

## 📋 Post-Deployment Verification

### Checklist

1. **Application Access**
   - [ ] Homepage loads
   - [ ] HTTPS certificate valid
   - [ ] Login page accessible

2. **Authentication**
   - [ ] Can login with seeded users
   - [ ] Role-based access working
   - [ ] Logout functional

3. **Core Features**
   - [ ] Create new pathology form
   - [ ] Save form as draft
   - [ ] Edit existing form
   - [ ] Submit for review
   - [ ] Doctor can review forms
   - [ ] File upload works
   - [ ] Form history tracks changes

4. **Database**
   - [ ] Connection successful
   - [ ] Data persists correctly
   - [ ] Stored procedures execute
   - [ ] Foreign keys enforced

5. **Performance**
   - [ ] Page load < 2 seconds
   - [ ] No timeout errors
   - [ ] File uploads complete
   - [ ] Search/filter responsive

6. **Security**
   - [ ] HTTP redirects to HTTPS
   - [ ] Authentication required
   - [ ] Unauthorized access denied
   - [ ] SQL injection prevention

---

## 📞 Support Contacts

### Deployment Issues
- **Application Errors**: Check stdout logs
- **Database Errors**: Check SQL Server logs
- **IIS Errors**: Check Event Viewer

### Resources
- **ASP.NET Core Deployment**: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/
- **IIS Hosting**: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/
- **Linux Hosting**: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx

---

*Last Updated: December 30, 2025*
*Deployment Guide Version: 1.0*
