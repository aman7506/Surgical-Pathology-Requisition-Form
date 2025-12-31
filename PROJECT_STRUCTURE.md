# Project Structure Guide
## Surgical Pathology Requisition Form - Complete Code Organization

**Purpose**: Complete guide to project structure showing where every code file is located and what it contains.

---

## 📁 Complete Directory Structure

```
Surgical-Pathology-Requisition-Form/
│
├── 📄 README.md                              ← Main project documentation
├── 📄 START_HERE.md                          ← Your first file to read
├── 📄 LICENSE                                ← MIT License with healthcare terms
├── 📄 .gitignore                             ← Git ignore rules
├── 📄 Dockerfile                             ← Docker container definition
├── 📄 docker-compose.yml                     ← Full stack deployment
├── 📄 .dockerignore                          ← Docker build exclusions
│
├── 📁 .github/                               ← GitHub configuration
│   └── workflows/                            ← CI/CD automation
│       ├── build.yml                         ← Build & test workflow
│       └── release.yml                       ← Release automation
│
├── 📁 Documentation/                         ← All documentation files
│   ├── QUICKSTART_DEPLOYMENT.md              ← Fast deployment guide
│   ├── DOCUMENTATION_INDEX.md                ← Master navigation
│   ├── COMPLETE_PROJECT_GUIDE.md             ← All-in-one reference
│   ├── TECHNICAL_ARCHITECTURE.md             ← System architecture
│   ├── ENTERPRISE_DEPLOYMENT_GUIDE.md        ← Production deployment
│   ├── GITHUB_DEPLOYMENT_WORKFLOW.md         ← Git setup guide
│   ├── DOCKER_DEPLOYMENT_GUIDE.md            ← Container deployment
│   ├── MSI_INSTALLER_GUIDE.md                ← Windows installer
│   ├── PROJECT_SUMMARY.md                    ← Executive summary
│   └── PROJECT_STRUCTURE.md                  ← This file
│
└── 📁 PathologyFormApp/                      ← MAIN APPLICATION (all code here)
    │
    ├── 📄 Program.cs                         ← Application entry point & configuration
    ├── 📄 PathologyFormApp.csproj            ← Project file with dependencies
    ├── 📄 appsettings.json                   ← Development configuration
    ├── 📄 appsettings.Development.json       ← Dev environment settings
    │
    ├── 📁 Controllers/                       ← MVC Controllers (Business Logic)
    │   ├── AccountController.cs              ← Login/Logout (187 lines)
    │   ├── HomeController.cs                 ← Dashboard (391 lines)
    │   ├── PathologyController.cs            ← Main CRUD operations (560 lines)
    │   ├── PathologyFormController.cs        ← Advanced operations (678 lines)
    │   └── SpecimenController.cs             ← Specimen type management (298 lines)
    │
    ├── 📁 Models/                            ← Data Models (Database entities)
    │   ├── PathologyRequisitionForm.cs       ← Main form model (184 lines, 60+ fields)
    │   ├── PathologyContext.cs               ← EF Core DbContext (58 lines)
    │   ├── User.cs                           ← Extended Identity user (23 lines)
    │   ├── FormStatus.cs                     ← Workflow enum (Draft/Submitted/Reviewed)
    │   ├── FormHistory.cs                    ← Audit trail model
    │   ├── SpecimenType.cs                   ← Lookup data model
    │   ├── PaginatedList.cs                  ← Pagination helper
    │   ├── UploadedFileInfo.cs               ← File metadata model
    │   └── ErrorViewModel.cs                 ← Error handling model
    │
    ├── 📁 ViewModels/                        ← View-specific models
    │   ├── NurseFormViewModel.cs             ← Nurse data entry view
    │   ├── DoctorFormViewModel.cs            ← Doctor review view
    │   ├── DoctorReviewViewModel.cs          ← Review workflow view
    │   └── LoginViewModel.cs                 ← Authentication view
    │
    ├── 📁 Views/                             ← Razor Views (User Interface)
    │   │
    │   ├── 📁 Shared/                        ← Shared layout and components
    │   │   ├── _Layout.cshtml                ← Main layout template
    │   │   ├── _ValidationScriptsPartial.cshtml  ← Validation scripts
    │   │   ├── Error.cshtml                  ← Error page
    │   │   └── _LoginPartial.cshtml          ← Login status partial
    │   │
    │   ├── 📁 Account/                       ← Authentication views
    │   │   ├── Login.cshtml                  ← Login page
    │   │   └── Register.cshtml               ← Registration page
    │   │
    │   ├── 📁 Home/                          ← Dashboard views
    │   │   ├── Index.cshtml                  ← Main dashboard
    │   │   └── Privacy.cshtml                ← Privacy policy
    │   │
    │   ├── 📁 Pathology/                     ← Main form CRUD views
    │   │   ├── Index.cshtml                  ← List all forms (with pagination)
    │   │   ├── Details.cshtml                ← View form details
    │   │   ├── Create.cshtml                 ← Create new form
    │   │   ├── Edit.cshtml                   ← Edit existing form
    │   │   ├── Delete.cshtml                 ← Delete confirmation
    │   │   └── _FormFields.cshtml            ← Reusable form fields partial
    │   │
    │   ├── 📁 PathologyForm/                 ← Advanced views
    │   │   ├── NurseForm.cshtml              ← Nurse-specific form
    │   │   ├── DoctorReview.cshtml           ← Doctor review interface
    │   │   └── FormHistory.cshtml            ← Audit trail view
    │   │
    │   └── 📁 Specimen/                      ← Specimen management views
    │       └── Index.cshtml                  ← Specimen type list
    │
    ├── 📁 wwwroot/                           ← Static files (CSS, JS, images)
    │   │
    │   ├── 📁 css/                           ← Stylesheets
    │   │   ├── site.css                      ← Main stylesheet
    │   │   └── bootstrap.min.css             ← Bootstrap framework
    │   │
    │   ├── 📁 js/                            ← JavaScript files
    │   │   ├── site.js                       ← Main application JavaScript
    │   │   └── jquery.min.js                 ← jQuery library
    │   │
    │   ├── 📁 lib/                           ← Third-party libraries
    │   │   ├── bootstrap/                    ← Bootstrap 5 files
    │   │   ├── jquery/                       ← jQuery files
    │   │   └── jquery-validation/            ← Validation library
    │   │
    │   ├── 📁 images/                        ← Image assets
    │   │   └── logo.png                      ← Hospital logo
    │   │
    │   └── 📁 uploads/                       ← User uploaded files (NOT in Git)
    │       └── (patient documents, specimen images)
    │
    ├── 📁 Migrations/                        ← Entity Framework migrations
    │   ├── 20240321000000_InitialCreate.cs   ← Initial database schema
    │   ├── 20240322000000_AddFormHistory.cs  ← Add audit trail
    │   ├── 20240323000000_AddFileUpload.cs   ← Add file upload support
    │   ├── ... (14 migrations total)
    │   └── PathologyContextModelSnapshot.cs  ← Current model snapshot
    │
    ├── 📁 Scripts/                           ← SQL Database scripts
    │   ├── CreateDatabase.sql                ← Complete database setup (350 lines)
    │   ├── CreateUsersAndRoles.sql           ← User seeding with default accounts
    │   ├── UpdateStoredProcedure.sql         ← Stored procedure updates
    │   └── RestoreDatabase.sql               ← Database restore script
    │
    ├── 📁 Properties/                        ← Project properties
    │   └── launchSettings.json               ← Debug launch settings
    │
    └── 📁 Data/                              ← (Optional) Data access layer
        └── (Future: Repository pattern classes)
```

---

## 🎯 **Key Code Files Explained**

### **1. Program.cs** (Entry Point)
**Location**: `PathologyFormApp/Program.cs`
**Lines**: 72
**Purpose**: Application startup and configuration

**What's Inside**:
```csharp
// Database configuration
builder.Services.AddDbContext<PathologyContext>(...);

// Identity setup
builder.Services.AddIdentity<User, IdentityRole>(...);

// Authorization policies
builder.Services.AddAuthorization(...);
  - RequireDoctorRole
  - RequireNurseRole

// Cookie authentication
builder.Services.ConfigureApplicationCookie(...);

// MVC controllers and views
builder.Services.AddControllersWithViews();

// Default route: Pathology/Index
app.MapControllerRoute(name: "default", pattern: "{controller=Pathology}/{action=Index}/{id?}");
```

---

### **2. PathologyRequisitionForm.cs** (Main Model)
**Location**: `PathologyFormApp/Models/PathologyRequisitionForm.cs`
**Lines**: 184
**Purpose**: Core data model for pathology forms

**What's Inside** (60+ fields):
```csharp
// Primary Key
public string UHID { get; set; }

// Patient Information
public string Name { get; set; }
public int Age { get; set; }
public string Gender { get; set; }
public string CRNo { get; set; }

// Clinical Details
public string ClinicalDiagnosis { get; set; }
public DateTime DateTimeOfCollection { get; set; }

// Specimen Information
public string SpecimenName { get; set; }
public int NoOfSpecimenReceived { get; set; }

// Examination Results
public string GrossDescription { get; set; }
public string MicroscopicExamination { get; set; }
public string Impression { get; set; }

// Workflow
public FormStatus Status { get; set; }
public string CreatedById { get; set; }
public string ReviewedById { get; set; }

// Navigation Properties
public User CreatedBy { get; set; }
public User ReviewedBy { get; set; }
public ICollection<FormHistory> History { get; set; }
```

---

### **3. PathologyController.cs** (Main CRUD)
**Location**: `PathologyFormApp/Controllers/PathologyController.cs`
**Lines**: 560
**Purpose**: Main form operations (Create, Read, Update, Delete)

**Key Methods**:
```csharp
// GET: Pathology/Index
public async Task<IActionResult> Index(string searchString, int? pageNumber)
  → Lists all forms with pagination and search

// GET: Pathology/Details/5
public async Task<IActionResult> Details(string id)
  → View single form details

// GET: Pathology/Create
public IActionResult Create()
  → Show create form page

// POST: Pathology/Create
public async Task<IActionResult> Create(PathologyRequisitionForm form, IFormFile uploadedFile)
  → Save new form to database

// GET: Pathology/Edit/5
public async Task<IActionResult> Edit(string id)
  → Show edit form page

// POST: Pathology/Edit/5
public async Task<IActionResult> Edit(string id, PathologyRequisitionForm form)
  → Update existing form

// GET: Pathology/Delete/5
public async Task<IActionResult> Delete(string id)
  → Show delete confirmation

// POST: Pathology/Delete/5
public async Task<IActionResult> DeleteConfirmed(string id)
  → Delete form from database
```

---

### **4. PathologyContext.cs** (Database Context)
**Location**: `PathologyFormApp/Models/PathologyContext.cs`
**Lines**: 58
**Purpose**: Entity Framework Core database context

**What's Inside**:
```csharp
public class PathologyContext : IdentityDbContext<User>
{
    // Database tables
    public DbSet<PathologyRequisitionForm> PathologyForms { get; set; }
    public DbSet<FormHistory> FormHistory { get; set; }
    public DbSet<SpecimenType> SpecimenTypes { get; set; }

    // Relationships configuration
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // User → PathologyForm (CreatedBy)
        builder.Entity<PathologyRequisitionForm>()
            .HasOne(p => p.CreatedBy)
            .WithMany(u => u.CreatedForms)
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // User → PathologyForm (ReviewedBy)
        builder.Entity<PathologyRequisitionForm>()
            .HasOne(p => p.ReviewedBy)
            .WithMany()
            .HasForeignKey(p => p.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);

        // PathologyForm → FormHistory
        builder.Entity<FormHistory>()
            .HasOne(h => h.Form)
            .WithMany(p => p.History)
            .HasForeignKey(h => h.FormUHID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

### **5. AccountController.cs** (Authentication)
**Location**: `PathologyFormApp/Controllers/AccountController.cs`
**Lines**: 187
**Purpose**: User login and logout

**Key Methods**:
```csharp
// GET: Account/Login
public IActionResult Login()
  → Show login page

// POST: Account/Login
public async Task<IActionResult> Login(LoginViewModel model)
  → Authenticate user
  → Check role (Nurse or Doctor)
  → Redirect to appropriate dashboard

// POST: Account/Logout
public async Task<IActionResult> Logout()
  → Sign out user
  → Redirect to login page
```

---

### **6. Views Structure**

#### **Main Layout** (`Views/Shared/_Layout.cshtml`)
- Navigation bar with role-based menu
- Bootstrap 5 styling
- jQuery and validation scripts
- Responsive design

#### **Form Views** (`Views/Pathology/`)
- `Index.cshtml`: List view with search, filter, pagination
- `Create.cshtml`: Form creation with 60+ fields
- `Edit.cshtml`: Form editing
- `Details.cshtml`: Read-only view with all data
- `Delete.cshtml`: Deletion confirmation

#### **Dashboard** (`Views/Home/Index.cshtml`)
- Nurse dashboard: Recent forms, quick create
- Doctor dashboard: Pending reviews, completed reviews

---

### **7. Database Scripts**

#### **CreateDatabase.sql** (350 lines)
**Location**: `PathologyFormApp/Scripts/CreateDatabase.sql`

**Contents**:
```sql
-- Create database
CREATE DATABASE pathology_db;

-- Create PathologyForm table (60+ columns)
CREATE TABLE PathologyForm (
    UHID NVARCHAR(50) PRIMARY KEY,
    LabRefNo NVARCHAR(50),
    Name NVARCHAR(200),
    Age INT,
    Gender NVARCHAR(10),
    -- ... 55+ more fields
    Status NVARCHAR(50),
    CreatedById NVARCHAR(450),
    ReviewedById NVARCHAR(450),
    CreatedAt DATETIME2,
    UpdatedAt DATETIME2
);

-- Create FormHistory table
CREATE TABLE FormHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FormUHID NVARCHAR(50),
    UserId NVARCHAR(450),
    Action NVARCHAR(50),
    Comments NVARCHAR(MAX),
    Timestamp DATETIME2
);

-- Create stored procedures
CREATE PROCEDURE sp_ManagePathologyForm ...
CREATE PROCEDURE sp_GetFormHistory ...
CREATE PROCEDURE sp_GetFormsByStatus ...
```

#### **CreateUsersAndRoles.sql**
**Location**: `PathologyFormApp/Scripts/CreateUsersAndRoles.sql`

**Creates**:
- Roles: `Doctor`, `Nurse`
- Default users:
  - `nurse@hospital.com` / `Nurse@123`
  - `doctor@hospital.com` / `Doctor@123`

---

## 🔍 **Code Flow Explanation**

### **Creating a New Form (Nurse Workflow)**

1. **User clicks "Create New"**
   - Route: `GET /Pathology/Create`
   - Controller: `PathologyController.Create()` (GET method)
   - View: `Views/Pathology/Create.cshtml`

2. **User fills form and submits**
   - Route: `POST /Pathology/Create`
   - Controller: `PathologyController.Create(PathologyRequisitionForm form)` (POST method)
   - Database: Saves to `PathologyForm` table via `PathologyContext`
   - Audit: Creates entry in `FormHistory` table
   - File Upload: Saves to `wwwroot/uploads/`
   - Redirect: Back to Index page

3. **Form appears in list**
   - Route: `GET /Pathology/Index`
   - Controller: `PathologyController.Index()`
   - View: `Views/Pathology/Index.cshtml`
   - Database: Queries with `_context.PathologyForms.ToListAsync()`

---

### **Reviewing a Form (Doctor Workflow)**

1. **Doctor logs in**
   - Route: `POST /Account/Login`
   - Controller: `AccountController.Login(LoginViewModel model)`
   - Authentication: `SignInManager.PasswordSignInAsync()`
   - Redirect: To dashboard based on role

2. **Doctor views submitted forms**
   - Route: `GET /Pathology/Index?status=NurseSubmitted`
   - Controller: `PathologyController.Index()` with filter
   - View: `Views/Pathology/Index.cshtml`
   - Database: Queries forms where `Status == "NurseSubmitted"`

3. **Doctor reviews and completes**
   - Route: `GET /Pathology/Edit/{UHID}`
   - Controller: `PathologyController.Edit()` (GET)
   - View: `Views/Pathology/Edit.cshtml`
   - User adds: Gross Description, Microscopic Examination, Impression
   - Submit: `POST /Pathology/Edit/{UHID}`
   - Database: Updates form, sets `Status = "DoctorReviewed"`
   - Audit: Adds review entry to `FormHistory`

---

## 📊 **Technology Stack by Layer**

### **Presentation Layer**
- **Location**: `Views/` folder
- **Technology**: Razor (.cshtml), HTML5, Bootstrap 5, jQuery
- **Files**: 22 view files
- **Purpose**: User interface

### **Business Logic Layer**
- **Location**: `Controllers/` folder
- **Technology**: C# ASP.NET Core MVC
- **Files**: 5 controller classes
- **Purpose**: Handle user requests, business rules

### **Data Access Layer**
- **Location**: `Models/PathologyContext.cs`
- **Technology**: Entity Framework Core 8.0.2 + Dapper 2.1.66
- **Files**: 1 context, 9 models, 4 view models
- **Purpose**: Database operations

### **Database Layer**
- **Location**: SQL Server (external)
- **Scripts**: `Scripts/` folder (4 SQL files)
- **Migrations**: `Migrations/` folder (14 files)
- **Purpose**: Data storage

---

## 📈 **File Statistics**

| Category | File Count | Total Lines |
|----------|------------|-------------|
| Controllers | 5 | ~2,100 |
| Models | 9 | ~500 |
| ViewModels | 4 | ~200 |
| Views | 22 | ~3,000 |
| Migrations | 14 | ~1,500 |
| SQL Scripts | 4 | ~700 |
| JavaScript | 2 | ~300 |
| CSS | 2 | ~500 |
| Configuration | 3 | ~150 |
| **TOTAL CODE** | **65** | **~9,000** |
| Documentation | 12 | ~12,000 (50,000 words) |

---

## 🎯 **Quick Reference**

### **Need to find...**

**Authentication code?**  
→ `Controllers/AccountController.cs`

**Form creation logic?**  
→ `Controllers/PathologyController.cs` → `Create()` methods

**Database schema?**  
→ `Models/PathologyRequisitionForm.cs` and `Scripts/CreateDatabase.sql`

**Form UI?**  
→ `Views/Pathology/Create.cshtml` and `Edit.cshtml`

**Workflow logic?**  
→ `Models/FormStatus.cs` and controller authorization

**File upload?**  
→ `Controllers/PathologyController.cs` → `Create()` POST method

**Database queries?**  
→ `Models/PathologyContext.cs` and controller LINQ queries

**Styling?**  
→ `wwwroot/css/site.css`

**JavaScript?**  
→ `wwwroot/js/site.js`

---

**This is your complete CODE MAP!** Every file location and purpose documented. 🗺️✨

Open any file to see the actual code!
