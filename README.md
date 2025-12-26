### Multi-Tenant Product Management <br />

**Project Overview** <br />

<p>This is a Multi-Tenant SaaS application built with ASP.NET Core (.NET 9) and Angular 17, with Domain-Driven Design (DDD) architecture
and subdomain-based tenant isolation system. This application demonstrates subscription management, role-based access control, and theme customization.
This system is designed as an enterprise-style SaaS platform where multiple tenants
operate under a single application instance with strict isolation.  Each tenant has its own <b>isolated users, data, and subscription limits</b>.</p>



**Demo Video:** [Watch Project Demo](https://drive.google.com/file/d/1B_i5x1hAcZys3HpIYwODT1XdfcmrZi1m/view?usp=sharing)



---


**Screenshots** <br />
Authentication:<br />

<img width="3816" height="927" alt="image" src="https://github.com/user-attachments/assets/cb54ccaa-532f-4443-9eab-aa75013e37fb" />

Tenant Dashboard <br />
<img width="1907" height="950" alt="Beta_dashboard" src="https://github.com/user-attachments/assets/36315b78-2c4b-4961-b8e0-335e0fd0784e" />

Product Management View <br />
<img width="1908" height="953" alt="Alpha_page" src="https://github.com/user-attachments/assets/2d26751b-6366-4600-b30c-13324997fd9e" />

Subscription Plans <br />

<img width="3466" height="965" alt="image(1)" src="https://github.com/user-attachments/assets/3267be79-543b-4d4f-9a76-7ed5eaacb119" />


Theme Customization <br />

<img width="1912" height="961" alt="Theme_page" src="https://github.com/user-attachments/assets/24678b31-369d-4057-9c40-68c81c791002" />

Tenants Management and User List <br />

<img width="3664" height="767" alt="image(2)" src="https://github.com/user-attachments/assets/228c4f24-d5da-4536-bb9b-6a88d1044113" />


---
###  Features Added
- Tenant wise user Authentication and Authrization. <br />
- CRUD operations for Products, including image upload (file path based). <br />
- Subdomain-based tenant isolation. <br />
- Global query filters for automatic tenant check <br />
- Repository Pattern + Unit of Work implementation. <br />
- Entity Framework Core ORM <br />
- 3-tier role system (SuperAdmin, Admin, User) <br />
- 4 subscription plans (Free, Basic, Premium, Enterprise) <br />
- Automatic limit enforcement (products/users per plan) <br />
- Upgrade subscription by random math solving <br />
- Theme customization functionality <br />
- Role management (promote/demote) <br />
- Tenant creation (SuperAdmin only) <br />

###  Authentication & Authorization
- Tenant-wise authentication
- JWT-based authentication
- Role-based access control 

### Role System
- **SuperAdmin** – Platform-level control
- **Admin** – Tenant-level management
- **User** – Limited access

###  Multi-Tenancy
- Subdomain-based tenant resolution
- Global query filters for automatic tenant isolation
- Tenant creation restricted to SuperAdmin

###  Product Management
- Product CRUD operations
- Image upload (file-path based)
- Tenant-specific data access

### Subscription Management
- Subscription plans:
  - Free
  - Basic
  - Premium
  - Enterprise
- Automatic enforcement:
  - Product limits
  - User limits
- Subscription upgrade using math challenge

###  Customization
- Tenant-wise theme customization
- UI branding per tenant

###  User Management
- View users per tenant
- Register users under a tenant
- Role promotion & demotion

---


Each tenant:
- Accesses the system via its own **subdomain**
- Has isolated data using **TenantId filtering**
- Operates under a **subscription plan**
- Manages users, roles, products, and themes independently
- Each tenant has its own isolated users.
  


  

---





##  Architecture Overview

### Backend Architecture
- Domain-Driven Design (DDD)
- Clean Architecture
  - API Layer
  - Application Layer
  - Infrastructure Layer
- Repository Pattern
- Unit of Work
- Global Query Filters for multi-tenancy
- Entity Framework Core ORM

### Frontend Architecture
- Angular 17 (Without Standalone)
- JWT Auth Guards
- Tenant-aware API communication
- Dynamic theming

---


###  Configure Subdomain Support For Windows and Setup Instructions

To enable subdomain-based tenancy locally:
1. Open **Notepad as Administrator**
2. File → Open: `C:\Windows\System32\drivers\etc\hosts`
3. Add these lines at the end:
```
127.0.0.1 alpha.localhost
127.0.0.1 beta.localhost
```

1. Setup Instructions: <br />
  ```git clone <your-repo-url>``` <br />

2. Update connection string in ```appsettings.json``` to connecting the database. <br />
   -> Open ```appsettings.json``` in the ```API``` project. and here update the server name and database name to point your local server.

3. ```Add-Migration InitialCreate -StartupProject TenantStore.API -Project TenantStore.Infrastructure```  (For Visual Studio) <br />
   ```dotnet ef migrations add InitialCreate -p TenantStore.Infrastructure -s TenantStore.API```           (For Visual Studio Code) <br />
             

3. ```Update-Database -StartupProject TenantStore.API -Project TenantStore.Infrastructure```                (For Visual Studio) <br />
   ```dotnet ef database update -p TenantStore.Infrastructure -s TenantStore.API```                        (For Visual Studio Code) <br />
   

Restore the project```dotnet restore``` then build the project ```dotnet build``` and run. For frontend: ``` cd tenant-store-ui``` then ```npm install ```  <br />

---


## Test Application (Seeded Data)


Each user is **strictly bound to a specific tenant (subdomain)** and **cannot access other tenants**.

| Email | Password | Role | Tenant | Login Subdomain | Access Level |
|------|---------|------|--------|-----------------|--------------|
| superadmin@tenantstore.com | SuperAdmin@123 | SuperAdmin | Platform | alpha.localhost | Full system access, create tenants (Alpha tenant only) |
| admin@alpha.com | Admin@123 | Admin | Alpha | alpha.localhost | Manage Alpha tenant |
| admin@beta.com | Admin@123 | Admin | Beta | beta.localhost | Manage Beta tenant |
| user@alpha.com | User@123 | User | Alpha | alpha.localhost | Limited access (Alpha tenant only) |

---

## Technologies Used: <br />
ASP.NET Core Web API (.NET 9) <br />
Entity Framework Core <br />
SQL Server <br />
Angular 17 <br />
JWT Bearer (Authentication) <br/>

---

# Author

**Pritom Sarkar**  
Software Engineer & Researcher  

Bangladesh 🇧🇩  


- LinkedIn: https://www.linkedin.com/in/pritom-sarkar-2867a2187/



