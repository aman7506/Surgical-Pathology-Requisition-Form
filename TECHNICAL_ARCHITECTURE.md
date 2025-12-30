# Technical Architecture - Surgical Pathology Requisition Form

## 🏛️ System Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────┐       │
│  │ Razor Views │  │  ViewModels  │  │  JavaScript  │       │
│  │  (.cshtml)  │  │    (C#)      │  │  (site.js)   │       │
│  └─────────────┘  └──────────────┘  └──────────────┘       │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                          │
│  ┌─────────────────────────────────────────────────────┐    │
│  │              MVC Controllers                         │    │
│  │  • AccountController (Authentication)               │    │
│  │  • PathologyController (Main CRUD)                  │    │
│  │  • PathologyFormController (Advanced Operations)    │    │
│  │  • SpecimenController (Lookup Management)           │    │
│  │  • HomeController (Dashboard)                       │    │
│  └─────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Business Logic Layer                      │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │    Models    │  │  Identity    │  │  Validation  │      │
│  │  (Domain)    │  │   (Auth)     │  │   (Rules)    │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Data Access Layer                         │
│  ┌──────────────────┐              ┌──────────────────┐     │
│  │ Entity Framework │              │     Dapper       │     │
│  │  Core (ORM)      │              │  (Stored Procs)  │     │
│  │  • LINQ Queries  │              │  • Raw SQL       │     │
│  │  • Migrations    │              │  • Parameters    │     │
│  └──────────────────┘              └──────────────────┘     │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      Database Layer                          │
│                   SQL Server (pathology_db)                  │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Tables: PathologyForm, FormHistory, SpecimenTypes,  │   │
│  │          AspNetUsers, AspNetRoles, etc.              │   │
│  │  Stored Procedures: sp_ManagePathologyForm, etc.     │   │
│  │  Constraints: PKs, FKs, Indexes, Validations         │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

---

## 🔄 Request Flow

### Typical HTTP Request Lifecycle

```
1. Browser Request
   ↓
2. ASP.NET Core Middleware Pipeline
   ↓
3. Authentication Middleware (checks cookie)
   ↓
4. Authorization Middleware (checks role/policy)
   ↓
5. Routing Middleware (maps to controller/action)
   ↓
6. Controller Action Execution
   ↓
7. Model Binding (form data → C# objects)
   ↓
8. Model Validation (Data Annotations)
   ↓
9. Business Logic Execution
   ↓
10. Data Access (EF Core or Dapper)
    ↓
11. Database Query/Command
    ↓
12. Result Processing
    ↓
13. View Rendering (Razor Engine)
    ↓
14. HTML Response to Browser
```

---

## 🗂️ Data Model Architecture

### Entity Relationship Diagram

```
┌─────────────────────────┐
│   AspNetUsers (User)    │
│ ────────────────────────│
│ ▪ Id (PK)              │
│ ▪ UserName             │
│ ▪ Email                │
│ ▪ FullName             │
│ ▪ Role                 │──┐
│ ▪ IsActive             │  │
│ ▪ CreatedAt            │  │
└─────────────────────────┘  │
          │                  │
          │ 1:N              │ 1:N
          ▼                  ▼
┌──────────────────────────────────┐
│  PathologyRequisitionForm        │
│ ─────────────────────────────────│
│ ▪ UHID (PK)                      │
│ ▪ LabRefNo                       │
│ ▪ Date                           │
│ ▪ Name, Age, Gender              │
│ ▪ CRNo, OPD_IPD, IPDNo          │
│ ▪ Consultant                     │
│ ▪ ClinicalDiagnosis              │
│ ▪ DateTimeOfCollection           │
│ ▪ [Obstetric/Gynae Fields]       │
│ ▪ [Specimen Fields]              │
│ ▪ [Examination Fields]           │
│ ▪ GrossDescription               │
│ ▪ MicroscopicExamination         │
│ ▪ Impression                     │
│ ▪ Pathologist                    │
│ ▪ SignatureImage                 │
│ ▪ Status (Enum)                  │
│ ▪ CreatedById (FK → User)        │
│ ▪ ReviewedById (FK → User)       │
│ ▪ ReviewedAt                     │
│ ▪ CreatedAt, UpdatedAt           │
└──────────────────────────────────┘
          │
          │ 1:N
          ▼
┌─────────────────────────┐
│    FormHistory          │
│ ────────────────────────│
│ ▪ Id (PK)              │
│ ▪ FormUHID (FK)        │
│ ▪ UserId (FK)          │
│ ▪ Action               │
│ ▪ Comments             │
│ ▪ Timestamp            │
└─────────────────────────┘

┌─────────────────────────┐
│   SpecimenTypes         │
│ ────────────────────────│
│ ▪ Id (PK)              │
│ ▪ Name                 │
│ ▪ Description          │
│ ▪ IsActive             │
└─────────────────────────┘
```

### Key Relationships
- **User → PathologyRequisitionForm**: One-to-Many (CreatedBy)
- **User → PathologyRequisitionForm**: One-to-Many (ReviewedBy)
- **PathologyRequisitionForm → FormHistory**: One-to-Many
- **User → FormHistory**: One-to-Many

---

## 🔐 Authentication & Authorization Architecture

### Identity Framework Integration

```csharp
ASP.NET Core Identity Stack:
├── IdentityUser (Base)
│   └── User (Custom Extension)
│       ├── FullName
│       ├── Role
│       ├── IsActive
│       └── CreatedAt
│
├── IdentityRole (Roles)
│   ├── Doctor
│   └── Nurse
│
└── IdentityDbContext
    └── PathologyContext (Custom)
```

### Authorization Policies

```csharp
// Defined in Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireDoctorRole", policy =>
        policy.RequireRole("Doctor"));
    
    options.AddPolicy("RequireNurseRole", policy =>
        policy.RequireRole("Nurse"));
});
```

### Cookie-Based Authentication Flow

```
1. User submits credentials
   ↓
2. AccountController validates against AspNetUsers
   ↓
3. SignInManager creates authentication cookie
   ↓
4. Cookie stored in browser (HTTP-only, Secure)
   ↓
5. Subsequent requests include cookie
   ↓
6. Authentication middleware validates cookie
   ↓
7. User.Identity.IsAuthenticated = true
   ↓
8. Authorization checks role/policy
   ↓
9. Access granted/denied
```

---

## 💾 Data Access Strategies

### Dual Data Access Approach

#### 1. Entity Framework Core (Primary)
**Used For**:
- CRUD operations on main entities
- Navigation property loading
- LINQ queries
- Migration management

**Example**:
```csharp
var form = await _context.PathologyRequisitionForms
    .Include(f => f.CreatedBy)
    .Include(f => f.ReviewedBy)
    .Include(f => f.FormHistory)
    .FirstOrDefaultAsync(f => f.UHID == uhid);
```

#### 2. Dapper (Secondary)
**Used For**:
- Stored procedure execution
- Complex queries requiring optimization
- Bulk operations
- Direct SQL commands

**Example**:
```csharp
await connection.QueryAsync<PathologyRequisitionForm>(
    "sp_ManagePathologyForm",
    parameters,
    commandType: CommandType.StoredProcedure
);
```

### Why Both?
- **EF Core**: Provides abstraction, type safety, change tracking
- **Dapper**: Offers performance for complex operations, direct control

---

## 🎨 Frontend Architecture

### Razor View Structure

```
Views/
├── Shared/
│   ├── _Layout.cshtml          # Master layout
│   ├── _LoginPartial.cshtml    # User info partial
│   ├── _ValidationScriptsPartial.cshtml
│   └── Error.cshtml
│
├── Account/
│   ├── Login.cshtml
│   └── Register.cshtml
│
├── Pathology/
│   ├── Index.cshtml            # List view with pagination
│   ├── Details.cshtml          # Read-only detail view
│   ├── Create.cshtml           # Form creation
│   ├── Edit.cshtml             # Form editing
│   └── Delete.cshtml           # Delete confirmation
│
└── Home/
    └── Index.cshtml            # Dashboard
```

### Client-Side Technologies

```javascript
// jQuery (via Bootstrap dependencies)
// - AJAX form submissions
// - Dynamic UI updates
// - Client-side validation

// Bootstrap 5
// - Responsive grid system
// - Form components
// - Modal dialogs
// - Navigation components

// Vanilla JavaScript (site.js)
// - Custom form interactions
// - File upload handling
// - Dynamic field management
```

---

## 🔄 State Management

### Form Lifecycle States

```
┌─────────┐
│  Draft  │ ← Initial state when Nurse creates form
└────┬────┘
     │ Nurse clicks "Submit"
     ▼
┌──────────────────┐
│ NurseSubmitted   │ ← Waiting for Doctor review
└────┬─────────────┘
     │ Doctor completes review
     ▼
┌──────────────────┐
│ DoctorReviewed   │ ← Final state, form complete
└──────────────────┘

Additional States (if implemented):
• Rejected
• Archived
• Deleted (soft delete)
```

### State Transitions (Stored Procedure Logic)

```sql
-- Draft → NurseSubmitted
UPDATE PathologyForm 
SET Status = 'NurseSubmitted', UpdatedAt = GETDATE()
WHERE UHID = @UHID AND Status = 'Draft'

-- NurseSubmitted → DoctorReviewed
UPDATE PathologyForm
SET Status = 'DoctorReviewed',
    ReviewedById = @UserId,
    ReviewedAt = GETDATE(),
    UpdatedAt = GETDATE()
WHERE UHID = @UHID AND Status = 'NurseSubmitted'
```

---

## 📦 Dependency Injection

### Service Registration (Program.cs)

```csharp
// DbContext (Scoped)
builder.Services.AddDbContext<PathologyContext>(options =>
    options.UseSqlServer(connectionString));

// Identity (Scoped)
builder.Services.AddIdentity<User, IdentityRole>(options => { })
    .AddEntityFrameworkStores<PathologyContext>()
    .AddDefaultTokenProviders();

// MVC with Razor Runtime Compilation
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
```

### DI in Controllers

```csharp
public class PathologyController : Controller
{
    private readonly PathologyContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<PathologyController> _logger;

    public PathologyController(
        PathologyContext context,
        UserManager<User> userManager,
        ILogger<PathologyController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }
}
```

---

## 🔧 Configuration Management

### Configuration Hierarchy

```
1. appsettings.json (Base)
   ↓
2. appsettings.Development.json (Development override)
   ↓
3. User Secrets (Development only, not in source control)
   ↓
4. Environment Variables (Production)
```

### Key Configuration Sections

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."  // Database connection
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

---

## 🛡️ Security Architecture

### Security Layers

1. **Transport Security**
   - HTTPS enforcement
   - TLS 1.2+ required
   - Certificate validation

2. **Authentication Security**
   - Password hashing (Identity default: PBKDF2)
   - Salt generation per user
   - Secure cookie storage (HTTP-only, Secure flag)

3. **Authorization Security**
   - Role-based access control
   - Policy-based authorization
   - Resource-based authorization (form ownership)

4. **Data Security**
   - SQL injection prevention (parameterized queries)
   - XSS prevention (Razor encoding)
   - CSRF protection (anti-forgery tokens)

5. **Database Security**
   - Encrypted connections (TrustServerCertificate)
   - Least privilege principle for DB user
   - Foreign key constraints
   - Cascade delete restrictions

---

## 📊 Database Migration Strategy

### EF Core Migrations Workflow

```
1. Model Change in C#
   ↓
2. Add-Migration CommandName
   ↓
3. Migration file generated (Up/Down methods)
   ↓
4. Review migration code
   ↓
5. Update-Database
   ↓
6. Migration applied to database
   ↓
7. __EFMigrationsHistory table updated
```

### Migration Files (14 Total)

1. **InitialCreate** - Base schema
2. **AddOPDNoField** - Patient tracking enhancement
3. **AddSpecimenAndObsGynaeFields** - Clinical data expansion
4. **FinalDatabaseSetupV2** - Complete schema refinement
5. **AddSignatureImageColumn** - Digital signature support
6. **AddFormHistoryAndUserRoles** - Audit trail
7. **SeedRolesAndUsers** - Initial data

---

## 🚀 Performance Considerations

### Optimization Strategies

1. **Database Optimization**
   - Indexed columns: UHID, CreatedById, ReviewedById, Status
   - Foreign key indexes
   - Stored procedures for complex operations

2. **Query Optimization**
   - Eager loading with `.Include()` to prevent N+1 queries
   - AsNoTracking() for read-only queries
   - Pagination to limit result sets

3. **Caching Strategy** (Future Enhancement)
   - Distributed cache for user sessions
   - Memory cache for lookup data (SpecimenTypes)
   - Output caching for static pages

4. **Frontend Optimization**
   - Bundling and minification (production)
   - CDN for static libraries
   - Lazy loading for large forms

---

## 📈 Scalability Architecture

### Current State: Monolithic

```
┌──────────────────────────────┐
│   Single ASP.NET Core App    │
│  ┌────────────────────────┐  │
│  │  Presentation Layer    │  │
│  ├────────────────────────┤  │
│  │  Business Logic        │  │
│  ├────────────────────────┤  │
│  │  Data Access          │  │
│  └────────────────────────┘  │
└──────────────────────────────┘
              ↕
┌──────────────────────────────┐
│       SQL Server DB          │
└──────────────────────────────┘
```

### Future Scalability Options

1. **Horizontal Scaling**
   - Multiple app servers behind load balancer
   - Shared SQL Server database
   - Distributed session state (Redis)

2. **Vertical Scaling**
   - Increase server resources (CPU, RAM)
   - Database performance tuning

3. **Microservices** (Long-term)
   - Form Management Service
   - User/Authentication Service
   - Specimen Management Service
   - Reporting Service

---

## 🧪 Testing Architecture

### Testing Layers (Recommended)

```
┌─────────────────────────┐
│   UI Tests              │  ← Selenium/Playwright
├─────────────────────────┤
│   Integration Tests     │  ← WebApplicationFactory
├─────────────────────────┤
│   Unit Tests            │  ← xUnit/NUnit/MSTest
└─────────────────────────┘
```

### Test Coverage Goals

- **Controllers**: Input validation, authorization, business logic
- **Models**: Data validation, relationships
- **Stored Procedures**: CRUD operations, transactions, error handling
- **Views**: Rendering, form validation

---

## 📁 File Organization Best Practices

### Current Structure (MVC Pattern)

```
Models/          ← Domain entities, DbContext
Controllers/     ← HTTP request handlers
Views/           ← UI templates
ViewModels/      ← UI-specific data transfer objects
Scripts/         ← Database scripts
Migrations/      ← EF Core schema changes
wwwroot/         ← Static assets
```

### Separation of Concerns

- **Controllers**: Handle HTTP, delegate to services
- **Models**: Represent data structure and relationships
- **ViewModels**: Shape data for specific views
- **DbContext**: Manage database operations
- **Stored Procedures**: Complex database logic

---

## 🔮 Technology Stack Rationale

### Why ASP.NET Core 8.0?
- **Long-term support** (LTS release)
- **Performance**: Fastest web framework benchmarks
- **Cross-platform**: Windows, Linux, macOS
- **Modern C#**: Latest language features
- **Excellent tooling**: Visual Studio, VS Code

### Why Entity Framework Core?
- **Code-first approach**: Models define schema
- **LINQ support**: Type-safe queries
- **Change tracking**: Automatic update detection
- **Migration system**: Version-controlled schema changes

### Why Dapper?
- **Performance**: Micro-ORM, minimal overhead
- **Flexibility**: Direct SQL control when needed
- **Compatibility**: Works with existing stored procedures

### Why SQL Server?
- **Enterprise-grade**: ACID compliance, reliability
- **Integration**: Seamless with Visual Studio
- **Tools**: SSMS, Azure Data Studio
- **Security**: Row-level security, encryption, auditing

---

## 🎯 Design Patterns Used

1. **MVC (Model-View-Controller)**
   - Separation of presentation and business logic

2. **Repository Pattern** (via DbContext)
   - Abstraction over data access

3. **Unit of Work** (via DbContext)
   - Transactional consistency

4. **Dependency Injection**
   - Loose coupling, testability

5. **ViewModel Pattern**
   - UI-specific data representation

6. **Factory Pattern** (Identity)
   - User creation, role management

---

## 📝 Code Quality Standards

### Naming Conventions
- **Controllers**: `{Entity}Controller` (e.g., PathologyController)
- **Models**: PascalCase entities (e.g., PathologyRequisitionForm)
- **Variables**: camelCase (e.g., formHistory)
- **Private fields**: _camelCase (e.g., _context)
- **Database**: PascalCase tables, camelCase columns

### Code Organization
- **One class per file**
- **Logical grouping** (Controllers/, Models/, etc.)
- **Async/await** for I/O operations
- **Exception handling** with try-catch in controllers
- **Validation** at multiple layers (client, server, database)

---

*Last Updated: December 30, 2025*
*Version: 1.0*
