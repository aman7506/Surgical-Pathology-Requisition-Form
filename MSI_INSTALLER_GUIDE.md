# MSI Installer Creation Guide
## Surgical Pathology Requisition Form - Windows Installer Package

**Purpose**: Create a professional MSI installer for deploying the Surgical Pathology Requisition Form to Windows Server environments.

---

## Overview

An MSI (Microsoft Installer) package allows hospitals to deploy your application with a professional installation wizard that:
- Installs all required components
- Configures IIS automatically
- Sets up database connections
- Creates necessary permissions
- Provides uninstall capability

---

## Option 1: WiX Toolset (Professional Installer - Recommended)

### Prerequisites

1. **Install WiX Toolset**:
   - Download from: https://wixtoolset.org/
   - Version: WiX v3.11.2 or later
   - Includes: WiX Toolset Build Tools + Visual Studio Extension

2. **Install WiX Extension for Visual Studio**:
   - Open Visual Studio 2022
   - Extensions → Manage Extensions
   - Search for "WiX"
   - Install "WiX Toolset Visual Studio Extension"

### Step 1: Create WiX Installer Project

1. Open Visual Studio 2022
2. File → New → Project
3. Search for "Setup Project for WiX"
4. Name: `PathologyFormInstaller`
5. Location: Your solution directory

### Step 2: Create Product.wxs File

Create `Product.wxs` in your WiX project:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi"
     xmlns:util="http://schemas.microsoft.com/wix/UtilExtension"
     xmlns:iis="http://schemas.microsoft.com/wix/IISExtension">
  
  <Product Id="*" 
           Name="Surgical Pathology Requisition Form" 
           Language="1033" 
           Version="1.0.0.0" 
           Manufacturer="Your Hospital/Organization" 
           UpgradeCode="PUT-GUID-HERE">
    
    <Package InstallerVersion="200" 
             Compressed="yes" 
             InstallScope="perMachine" 
             Description="Hospital Pathology Form Management System"
             Comments="Enterprise healthcare application for surgical pathology requisition forms" />

    <MajorUpgrade DowngradeErrorMessage="A newer version is already installed." />
    
    <MediaTemplate EmbedCab="yes" />

    <!-- Prerequisites -->
    <PropertyRef Id="NETFRAMEWORK48" />
    <Condition Message="This application requires .NET 8.0 Runtime. Please install it first.">
      <![CDATA[Installed OR NETFRAMEWORK48]]>
    </Condition>

    <!-- Installation Directory -->
    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFiles64Folder">
        <Directory Id="INSTALLFOLDER" Name="PathologyFormApp" />
      </Directory>
      
      <!-- IIS wwwroot -->
      <Directory Id="INETPUB">
        <Directory Id="WWWROOT" Name="wwwroot">
          <Directory Id="APPFOLDER" Name="PathologyApp" />
        </Directory>
      </Directory>
    </Directory>

    <!-- Application Files Component -->
    <ComponentGroup Id="ProductComponents" Directory="APPFOLDER">
      <Component Id="AppFiles" Guid="PUT-GUID-HERE">
        <File Id="MainDLL" Source="$(var.SolutionDir)PathologyFormApp\bin\Release\net8.0\publish\PathologyFormApp.dll" />
        <File Id="MainEXE" Source="$(var.SolutionDir)PathologyFormApp\bin\Release\net8.0\publish\PathologyFormApp.exe" />
        <!-- Add all other published files -->
      </Component>
    </ComponentGroup>

    <!-- IIS Configuration -->
    <ComponentGroup Id="IISComponents" Directory="APPFOLDER">
      <Component Id="IISAppPool" Guid="PUT-GUID-HERE">
        <iis:WebAppPool Id="PathologyAppPool" Name="PathologyAppPool" ManagedRuntimeVersion="v0.0" />
      </Component>
      
      <Component Id="IISWebSite" Guid="PUT-GUID-HERE">
        <iis:WebSite Id="PathologyWebSite" 
                     Description="Pathology Requisition Form" 
                     Directory="APPFOLDER">
          <iis:WebAddress Id="AllUnassigned" Port="80" />
        </iis:WebSite>
      </Component>
    </ComponentGroup>

    <!-- Features -->
    <Feature Id="ProductFeature" Title="Pathology Form Application" Level="1">
      <ComponentGroupRef Id="ProductComponents" />
      <ComponentGroupRef Id="IISComponents" />
    </Feature>

    <!-- Custom Actions -->
    <CustomAction Id="PublishApp" 
                  Directory="INSTALLFOLDER" 
                  Execute="deferred" 
                  Impersonate="no" 
                  Return="check"
                  ExeCommand='dotnet publish "$(var.SolutionDir)PathologyFormApp\PathologyFormApp.csproj" -c Release -o "[APPFOLDER]"' />

    <InstallExecuteSequence>
      <Custom Action="PublishApp" Before="InstallFiles">NOT Installed</Custom>
    </InstallExecuteSequence>

  </Product>
</Wix>
```

### Step 3: Build MSI

1. Build the WiX project in Release mode
2. MSI file will be in: `bin\Release\PathologyFormInstaller.msi`

---

## Option 2: Advanced Installer (GUI-Based - Easier)

### Download and Install

1. Download Advanced Installer: https://www.advancedinstaller.com/
2. Free edition is sufficient for basic MSI creation

### Create Project

1. Launch Advanced Installer
2. New Project → Simple → Windows Application
3. Project Name: `Surgical Pathology Requisition Form`

### Configure Project

#### 1. Product Details
- Product Name: `Surgical Pathology Requisition Form`
- Version: `1.0.0`
- Publisher: `Your Hospital/Organization`
- Install Location: `C:\inetpub\PathologyApp`

#### 2. Add Files
- Files and Folders → Add Files
- Select all files from: `PathologyFormApp\bin\Release\net8.0\publish\`
- Destination: `[APPDIR]`

#### 3. Prerequisites
- Prerequisites → Add Prerequisite
- Select: `.NET Runtime 8.0`
- Download URL: Official Microsoft download link

#### 4. IIS Configuration
- IIS → Add Web Site
- Name: `PathologyApp`
- Port: `80`
- App Pool: `PathologyAppPool`
- .NET Version: `No Managed Code`

#### 5. Registry
- Registry → Add Registry Value
- HKLM\SOFTWARE\PathologyApp
- Values:
  - InstallPath (String): `[APPDIR]`
  - Version (String): `1.0.0`

#### 6. Shortcuts
- Shortcuts → Add Shortcut
- Target: Documentation folder
- Name: `Pathology App Documentation`

### Build MSI

1. Project → Build
2. MSI will be created in project output folder

---

## Option 3: Simple Self-Extracting Archive (Quick Solution)

If you just need a quick deployable package:

### Using 7-Zip

```powershell
# Install 7-Zip (if not installed)
# Download from: https://www.7-zip.org/

# Navigate to publish folder
cd PathologyFormApp\bin\Release\net8.0\publish

# Create self-extracting archive
& "C:\Program Files\7-Zip\7z.exe" a -sfx7z.sfx PathologyFormSetup.exe *

# This creates PathologyFormSetup.exe which extracts and can run setup scripts
```

---

## MSI Installer Features to Include

### 1. Database Configuration Dialog

Create custom dialog for database setup:
- SQL Server address
- Database name
- SQL Authentication credentials
- Test connection button

### 2. IIS Setup Options

- Automatic IIS feature installation
- App Pool configuration
- HTTPS binding setup
- SSL certificate selection

### 3. Backup Existing Installation

Before upgrade:
- Backup database
- Backup configuration files
- Backup uploaded files

### 4. Post-Install Configuration

- Open configuration wizard in browser
- Allow initial admin user creation
- Database migration execution

---

## Testing Your MSI

### Test Checklist

- [ ] Fresh Windows Server installation
- [ ] Install prerequisites (.NET 8.0)
- [ ] Run MSI installer
- [ ] Verify IIS configuration
- [ ] Test database connection
- [ ] Access application in browser
- [ ] Test all features
- [ ] Test uninstall process
- [ ] Test upgrade installation

### Testing Environments

1. **Clean VM**: Windows Server 2019/2022
2. **Existing Installation**: Test upgrade path
3. **Different SQL Server versions**: Test compatibility

---

## Distribution

### Digital Signature (Recommended for Healthcare)

Sign your MSI for security:

```powershell
# Get code signing certificate from trusted CA
# Then sign the MSI

signtool sign /f "YourCertificate.pfx" /p "password" /t http://timestamp.digicert.com PathologyFormInstaller.msi
```

### Checksums

Generate checksums for verification:

```powershell
# SHA256 checksum
Get-FileHash PathologyFormInstaller.msi -Algorithm SHA256 | Format-List

# Create checksum file
Get-FileHash PathologyFormInstaller.msi -Algorithm SHA256 | Out-File PathologyFormInstaller.msi.sha256
```

---

## Alternative: Docker Container (Modern Approach)

Instead of MSI, consider Docker for easier deployment:

### Create Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PathologyFormApp/PathologyFormApp.csproj", "PathologyFormApp/"]
RUN dotnet restore "PathologyFormApp/PathologyFormApp.csproj"
COPY . .
WORKDIR "/src/PathologyFormApp"
RUN dotnet build "PathologyFormApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PathologyFormApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PathologyFormApp.dll"]
```

### Build and Run

```powershell
# Build Docker image
docker build -t pathologyapp:1.0 .

# Run container
docker run -d -p 80:80 -p 443:443 --name pathologyapp pathologyapp:1.0

# Save image for distribution
docker save pathologyapp:1.0 -o pathologyapp-v1.0.tar

# Load on target server
docker load -i pathologyapp-v1.0.tar
```

---

## Recommended Approach for Your Project

For a hospital environment, I recommend:

**Phase 1: Immediate Deployment**
- Use manual deployment following QUICKSTART_DEPLOYMENT.md
- Or use Advanced Installer for quick MSI creation

**Phase 2: Professional Distribution**
- Create WiX-based MSI installer
- Add database configuration wizard
- Include automatic IIS setup
- Add digital signature

**Phase 3: Modern Deployment**
- Containerize with Docker
- Use Docker Compose for full stack
- Implement Kubernetes for enterprise scale

---

## Next Steps

1. **Commit LICENSE file to Git**:
```powershell
git add LICENSE
git commit -m "Add MIT License with healthcare-specific terms"
git push origin main
```

2. **Choose MSI approach** based on your needs

3. **Test installer** in clean environment

Would you like me to:
1. Create the WiX project files?
2. Set up a Docker container?
3. Create installation scripts?
4. Something else?
