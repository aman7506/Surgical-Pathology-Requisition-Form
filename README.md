# Surgical Pathology Requisition Form

Enterprise-grade ASP.NET Core 8.0 MVC application for managing surgical pathology requisition forms in hospital and clinical laboratory environments.

---

## Project Status

**Version**: 1.0.0  
**Status**: Production-Ready  
**Classification**: Healthcare Information System  
**Compliance**: HIPAA-Ready Architecture

---

## Table of Contents

- [Overview](#overview)
- [Medical Context](#medical-context)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Usage](#usage)
- [Deployment](#deployment)
- [Security](#security)
- [Documentation](#documentation)
- [License](#license)
- [Medical Disclaimer](#medical-disclaimer)

---

## Overview

This application digitizes and streamlines the surgical pathology requisition workflow in healthcare facilities. It replaces traditional paper-based forms with a secure, auditable electronic system that ensures data integrity, regulatory compliance, and improved operational efficiency.

### Problem Statement

Traditional paper-based pathology requisition forms present significant challenges:

- Risk of data loss or damage
- Illegible handwriting leading to errors
- Difficulty in tracking form status
- No audit trail for regulatory compliance
- Inefficient retrieval and reporting
- Lack of integration with laboratory information systems

### Solution

This application provides:

- Role-based digital workflow (Nurse data entry, Doctor review and diagnosis)
- Comprehensive 60+ field medical data capture
- Complete audit trail for regulatory compliance
- Secure file attachment for specimen images and reports
- Real-time status tracking
- Search and retrieval capabilities
- HIPAA-ready security architecture

---

## Medical Context

### What is Surgical Pathology?

Surgical pathology is a medical specialty focused on the examination and diagnosis of tissue specimens removed during surgery or biopsy procedures. The requisition form is a critical document that:

1. Records patient demographic and clinical information
2. Documents specimen collection details
3. Provides clinical history to assist pathologist diagnosis
4. Tracks specimen handling and processing
5. Records pathologist findings and diagnosis

### Clinical Workflow

```
Patient → Surgical Procedure → Specimen Collection (Nurse)
                                        ↓
                          Requisition Form Creation (Nurse)
                                        ↓
                            Laboratory Processing
                                        ↓
                    Microscopic Examination (Pathologist)
                                        ↓
                    Diagnosis and Reporting (Doctor)
                                        ↓
                            Medical Record
```

### Regulatory Importance

Pathology requisition forms are legal medical documents subject to:
- Healthcare regulations (HIPAA, CLIA)
- Quality assurance requirements
- Accreditation standards (CAP, TJC)
- Medical record retention policies

---

## Features

### Core Functionality

- **Complete Form Management**: 60+ medical and clinical data fields
- **Role-Based Workflow**: Distinct interfaces for Nurses and Doctors
- **Audit Trail**: Complete history of all form actions
- **File Uploads**: Support for specimen images and related documents
- **Search and Filter**: Advanced query capabilities
- **Status Tracking**: Draft → Submitted → Reviewed workflow
- **Responsive Design**: Accessible on desktop and mobile devices

### User Roles

#### Nurse Role
- Create new requisition forms
- Enter patient demographics
- Record specimen collection details
- Document clinical information
- Save drafts or submit for pathologist review
- View own submitted forms

#### Doctor / Pathologist Role
- Review nurse-submitted forms
- Enter gross specimen description
- Document microscopic examination findings
- Provide diagnostic impression
- Add clinical advice and recommendations
- Digital signature capability
- Complete final review

### Workflow States

```
Draft: Initial form creation, editable by Nurse
  ↓
NurseSubmitted: Submitted for pathologist review, locked for editing
  ↓
DoctorReviewed: Pathologist completed diagnosis, final state
```

---

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# with .NET 8.0
- **Database**: Microsoft SQLServer 2019+
- **ORM**: Entity Framework Core 8.0.2
- **Micro-ORM**: Dapper 2.1.66 (for stored procedures)
- **Authentication**: ASP.NET Core Identity 8.0.2

### Frontend
- **View Engine**: Razor (.cshtml)
- **CSS Framework**: Bootstrap 5
- **JavaScript**: jQuery + Vanilla JavaScript
- **Responsive**: Mobile-first design

### Database
- **RDBMS**: Microsoft SQL Server
- **Migrations**: Entity Framework Core Code-First
- **Stored Procedures**: Complex operations and reporting
- **Audit**: Complete form history tracking

### Security
- **Authentication**: Cookie-based with ASP.NET Core Identity
- **Authorization**: Role-based access control
- **Password Policy**: Configurable complexity requirements
- **Encryption**: HTTPS/TLS for transport, TDE for data at rest
- **Protection**: CSRF, XSS, SQL injection prevention

---

## System Requirements

### Minimum Requirements

**Server**:
- CPU: 4 cores
- RAM: 8 GB
- Storage: 100 GB SSD
- OS: Windows Server 2019+ or Ubuntu 20.04+ LTS
- Network: 100 Mbps

**Software**:
- .NET 8.0 Runtime or Hosting Bundle
- SQL Server 2019+ (or compatible)
- IIS 10+ (Windows) or Nginx (Linux)
- SSL/TLS Certificate

**Client (Browser)**:
- Chrome 90+
- Firefox 88+
- Edge 90+
- Safari 14+

### Recommended Specifications

**Production Server**:
- CPU: 8 cores
- RAM: 16 GB
- Storage: 250 GB SSD (RAID 1)
- OS: Windows Server 2022 or Ubuntu 22.04 LTS
- Network: 1 Gbps
- Load Balancer (high availability)

---

## Installation

### Quick Start (Development)

#### Prerequisites

1. Install .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
2. Install SQL Server: https://www.microsoft.com/sql-server/sql-server-downloads
3. Install Git: https://git-scm.com/downloads

#### Clone Repository

```powershell
git clone https://github.com/yourusername/Surgical-Pathology-Requisition-Form.git
cd Surgical-Pathology-Requisition-Form
```

#### Configure Database

Update `src/PathologyFormApp/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=pathology_db;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

#### Create Database

Execute SQL scripts in order:

```powershell
# Using SSMS, execute:
# 1. database/scripts/01_CreateDatabase.sql
# 2. database/scripts/02_CreateUsersAndRoles.sql
# 3. database/scripts/03_StoredProcedures.sql
```

Or use Entity Framework migrations:

```powershell
cd src/PathologyFormApp
dotnet ef database update
```

#### Run Application

```powershell
cd src/PathologyFormApp
dotnet restore
dotnet build
dotnet run
```

Navigate to: `https://localhost:7123`

#### Default Login Credentials

```
Nurse Account:
Username: nurse@hospital.com
Password: Nurse@123

Doctor Account:
Username: doctor@hospital.com
Password: Doctor@123
```

**IMPORTANT**: Change these credentials immediately in production.

---

## Usage

### Nurse Workflow

1. **Login** with nurse credentials
2. **Navigate to Dashboard** to view existing forms
3. **Create New Form**:
   - Click "Create New" button
   - Enter patient information (UHID, Name, Age, Gender, etc.)
   - Record specimen collection details
   - Add clinical diagnosis and history
   - Optional: Upload related documents or images
4. **Save Options**:
   - Save as Draft: Form remains editable
   - Submit for Review: Locks form and notifies pathologist
5. **Track Status** on dashboard

### Doctor / Pathologist Workflow

1. **Login** with doctor credentials
2. **View Submitted Forms** on dashboard
3. **Select Form** to review
4. **Review Patient and Specimen Information**
5. **Enter Examination Findings**:
   - Gross specimen description
   - Microscopic examination details
   - Special stains (if applicable)
6. **Provide Diagnosis**:
   - Diagnostic impression
   - Clinical advice and recommendations
   - Optionally add digital signature
7. **Complete Review**: Changes status to DoctorReviewed

### Form Search and Filtering

- Search by UHID, patient name, or lab reference number
- Filter by status (Draft, Submitted, Reviewed)
- Filter by date range
- Sort by creation date, review date, etc.

---

## Deployment

This application supports multiple deployment scenarios:

### Windows Server with IIS

Complete step-by-step guide: [ENTERPRISE_DEPLOYMENT_GUIDE.md](docs/ENTERPRISE_DEPLOYMENT_GUIDE.md#windows-server-iis-deployment)

Quick summary:
1. Install .NET 8.0 Hosting Bundle
2. Publish application: `dotnet publish -c Release`
3. Configure IIS Application Pool
4. Create IIS Website with HTTPS binding
5. Configure production database connection
6. Verify deployment

### Linux with Nginx

Complete step-by-step guide: [ENTERPRISE_DEPLOYMENT_GUIDE.md](docs/ENTERPRISE_DEPLOYMENT_GUIDE.md#linux-nginx-deployment)

Quick summary:
1. Install .NET 8.0 Runtime
2. Install and configure Nginx
3. Create systemd service
4. Configure reverse proxy
5. Install SSL certificate
6. Verify deployment

### Azure App Service

Complete step-by-step guide: [ENTERPRISE_DEPLOYMENT_GUIDE.md](docs/ENTERPRISE_DEPLOYMENT_GUIDE.md#azure-app-service-deployment)

Quick summary:
1. Create Azure resources (Resource Group, App Service Plan, Web App)
2. Create or configure Azure SQL Database
3. Deploy application using Azure CLI
4. Configure connection strings
5. Enable HTTPS and custom domain
6. Verify deployment

### Docker (Containerized Deployment)

See: [deployment/docker/README.md](deployment/docker/README.md)

---

## Security

### Authentication

- ASP.NET Core Identity with bcrypt password hashing
- Cookie-based authentication with 7-day sliding expiration
- Configurable password complexity requirements
- Account lockout after failed login attempts

### Authorization

- Role-based access control (Nurse, Doctor)
- Resource-based authorization (users can only edit own drafts)
- Policy-based authorization for sensitive operations

### Data Protection

- HTTPS/TLS enforced for all connections
- Database connections encrypted
- Parameterized queries prevent SQL injection
- Razor encoding prevents XSS attacks
- Anti-forgery tokens prevent CSRF
- File upload validation and sanitization

### Compliance Features

- Complete audit trail in FormHistory table
- User action timestamps
- Data encryption at rest (SQL Server TDE)
- Secure configuration management
- HIPAA-ready architecture (requires additional operational controls)

### Production Security Checklist

- [ ] Change default user passwords
- [ ] Configure strong password policy
- [ ] Enable HTTPS with valid SSL certificate
- [ ] Restrict database access to application server
- [ ] Configure firewall (only ports 80, 443 open)
- [ ] Enable SQL Server TDE
- [ ] Configure regular database backups
- [ ] Implement monitoring and alerting
- [ ] Review and restrict file upload locations
- [ ] Disable detailed error messages
- [ ] Configure secure session management

---

## Documentation

### Available Documentation

- **[README.md](README.md)**: This file - project overview and quick start
- **[GITHUB_DEPLOYMENT_WORKFLOW.md](docs/GITHUB_DEPLOYMENT_WORKFLOW.md)**: Git setup and GitHub deployment
- **[ENTERPRISE_DEPLOYMENT_GUIDE.md](docs/ENTERPRISE_DEPLOYMENT_GUIDE.md)**: Production deployment for IIS, Linux, Azure
- **[TECHNICAL_ARCHITECTURE.md](docs/TECHNICAL_ARCHITECTURE.md)**: System architecture and design
- **[COMPLETE_PROJECT_GUIDE.md](docs/COMPLETE_PROJECT_GUIDE.md)**: Comprehensive all-in-one guide
- **[SECURITY_COMPLIANCE.md](docs/SECURITY_COMPLIANCE.md)**: Security implementation and compliance

### Getting Help

1. Check relevant documentation file
2. Review troubleshooting section in deployment guides
3. Check application logs
4. Review GitHub issues
5. Contact system administrator

---

## Project Structure

```
Surgical-Pathology-Requisition-Form/
├── src/
│   └── PathologyFormApp/           # Main application
│       ├── Controllers/            # MVC controllers (5)
│       ├── Models/                 # Domain models (9)
│       ├── ViewModels/             # View-specific models (4)
│       ├── Views/                  # Razor views
│       ├── wwwroot/                # Static files
│       ├── Program.cs              # Application entry point
│       └── PathologyFormApp.csproj # Project file
│
├── database/
│   ├── scripts/                    # SQL scripts for setup
│   └── schema/                     # Database schema documentation
│
├── docs/                           # All documentation
│
├── deployment/
│   ├── iis/                        # IIS deployment configs
│   ├── linux/                      # Linux deployment configs
│   ├── azure/                      # Azure deployment scripts
│   └── docker/                     # Docker configurations
│
├── config/                         # Example configuration files
│
├── .gitignore                      # Git ignore rules
├── README.md                       # This file
└── LICENSE                         # License information
```

---

## Contributing

This is a healthcare application. All contributions must:

1. Follow medical data handling best practices
2. Maintain HIPAA compliance architecture
3. Include appropriate testing
4. Update documentation
5. Follow code quality standards

See CONTRIBUTING.md for detailed guidelines.

---

## Versioning

This project uses Semantic Versioning 2.0.0:
- MAJOR version for incompatible API changes
- MINOR version for backwards-compatible functionality
- PATCH version for backwards-compatible bug fixes

Current version: **1.0.0**

---

## Authors and Acknowledgments

### Development Team
- Project Lead: [Your Name]
- Backend Development: [Team Names]
- UI/UX Design: [Designer Names]
- Database Architecture: [DBA Names]
- Quality Assurance: [QA Names]

### Medical Consultants
- Clinical Workflow: [Pathologist Name]
- Regulatory Compliance: [Compliance Officer Name]

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

For commercial use in healthcare facilities, additional terms may apply. Contact the development team for licensing inquiries.

---

## Medical Disclaimer

**IMPORTANT - READ CAREFULLY**

This software is designed as a tool for trained medical professionals in hospital and clinical laboratory environments. It is intended to assist in the documentation and management of surgical pathology requisition forms.

### Limitations and Responsibilities

1. **Not a Diagnostic Tool**: This application does NOT perform medical diagnosis. All diagnostic determinations must be made by qualified pathologists.

2. **Professional Judgment**: The software does not replace professional medical judgment. All medical decisions must be made by licensed healthcare providers.

3. **Data Accuracy**: Users are responsible for ensuring accuracy and completeness of all entered data. The software provides data validation but cannot verify medical correctness.

4. **Regulatory Compliance**: While the application is designed with HIPAA-ready architecture, achieving full regulatory compliance requires appropriate operational controls, policies, and procedures implemented by the healthcare facility.

5. **System Validation**: Healthcare facilities must perform appropriate system validation and qualification according to their quality management system requirements before clinical use.

6. **Training Required**: All users must receive appropriate training on both the software and relevant medical/laboratory procedures before use.

7. **No Warranty**: This software is provided "AS IS" without warranty of any kind. See LICENSE file for complete warranty disclaimer.

### Use Restrictions

This software must only be used:
- By trained and authorized healthcare personnel
- In facilities with appropriate quality management systems
- In compliance with all applicable laws and regulations
- With appropriate backup and disaster recovery procedures
- Under supervision of qualified medical leadership

### Liability

The developers, contributors, and distributors of this software assume no liability for:
- Medical errors or misdiagnoses
- Data loss or corruption
- System failures or downtime
- Regulatory non-compliance
- Any damages arising from use or inability to use this software

### Reporting Issues

Critical issues that may affect patient safety must be reported immediately to:
- Your facility's quality assurance department
- Your facility's IT security team
- The software development team

For software bugs and feature requests, use the GitHub issue tracker.

---

## Support and Contact

### Technical Support
- GitHub Issues: https://github.com/yourusername/Surgical-Pathology-Requisition-Form/issues
- Email: support@yourhospital.com

### Security Vulnerabilities
For security issues, please email: security@yourhospital.com  
**Do not post security vulnerabilities in public issues**

### Commercial Inquiries  
Email: business@yourhospital.com

---

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for adetailed history of changes.

### Version 1.0.0 (Initial Release)
- Complete pathology requisition form management
- Role-based workflow (Nurse, Doctor)
- 60+ medical and clinical data fields
- Audit trail functionality
- File upload and management
- ASP.NET Core Identity authentication
- Production deployment support (IIS, Linux, Azure)
- Comprehensive documentation

---

## Acknowledgments

- ASP.NET Core Team for the excellent framework
- Bootstrap Team for the UI framework
- Entity Framework Team for the ORM
- Healthcare IT community for best practices guidance
- Open source community for tools and libraries

---

**Project Repository**: https://github.com/yourusername/Surgical-Pathology-Requisition-Form  
**Documentation**: [docs/](docs/)  
**License**: MIT (with healthcare use considerations)  
**Status**: Production-Ready  
**Version**: 1.0.0

---

*This README follows enterprise healthcare software documentation standards.*  
*Last Updated: December 2025*
