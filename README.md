# 🛒 E-Commerce Web Application

A full-featured E-Commerce web application developed using **ASP.NET Core MVC** with a clean **N-Tier Architecture**.  
The application provides robust functionality for both customers and administrators, including secure authentication, external logins, role-based access control, Stripe integration, full order lifecycle management, and much more.

[![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-blueviolet)](https://dotnet.microsoft.com/)
[![Live Demo](https://img.shields.io/badge/Live%20Demo-Click%20Here-brightgreen)](http://etechno.runasp.net/)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Abdelrahman%20Zagloul-blue?logo=linkedin)](https://www.linkedin.com/in/abdelrahman-zagloul/)

---

## 📦 Project Features

### 👥 Authentication & Authorization
- User registration & login using **ASP.NET Identity**
- External login providers:
  - Google
  - GitHub
  - Facebook
  - LinkedIn
  - Microsoft
- Role-based access control (Admin / User)
- Lock & unlock user accounts (by Admin)
- Forgot / Reset password
- Profile update & password change
- Cookie-based authentication

### 🛍️ Product & Category Management
- Full CRUD for products and categories
- Show products dynamically based on category
- Upload and preview product images
- AJAX-powered DataTables for fast and searchable listings

### 🛒 Shopping & Order System
- Dynamic "Add to Cart" functionality
- Manage cart items (quantity, remove, etc.)
- Secure payment via **Stripe**
- Order tracking & status updates (Pending → Shipped → Delivered)
- Order management for both admin and customers
- Email notifications:
  - After payment (to customer and admin)
  - After shipping (to customer)
  - After delivery (to customer)

### 📊 Admin Dashboard
- Overview of:
  - Total sales
  - Orders
  - Products
  - Registered users
- Admin-specific layout
- Role and user management
- Manage all product data and categories

### 🔔 Notifications & Interactivity
- **Toastr** notifications for actions
- **SweetAlert2** for delete confirmations
- **AJAX + DataTables** for dynamic UI
- **Email System** via SMTP

---

## 💡 Architecture Overview

This project uses a layered **N-Tier Architecture** for clean code separation and maintainability:

- **🧩 Model Layer**  
  Domain entities and ViewModels used across the system.  
  ➤ *Includes: `Models`, `ViewModels`*

- **📂 Data Access Layer**  
  Handles all database logic using **Entity Framework Core**.  
  Implements the **Repository Pattern** and **Unit of Work Pattern**.  
  ➤ *Includes: `ApplicationDbContext`, `Configurations`, `Repositories`, `IUnitOfWork`, `DbInitializer`, `Migrations`*

- **⚙️ Service Layer**  
  Contains the business logic and bridges the Web & Data layers.  
  ➤ *Handles payment, user profile logic, order processing, etc.*

- **🧰 Utility Layer**  
  Shared helpers and application-wide constants.  
  ➤ *Includes: `EmailSender`, `StaticDetails`, `SessionManager`, `CookieHelper`, `ApplicationConstants`*

- **🌐 Web Layer**  
  The UI layer built with **ASP.NET Core MVC**.  
  ➤ *Includes: Controllers, Views, Razor Pages, `wwwroot`, and Areas (Admin/User)*

---

## 🔧 Technologies & Tools
- ASP.NET Core 8.0
- Entity Framework Core
- ASP.NET Identity
- N-Tier Architecture
- Repository Pattern + Unit of Work
- Stripe API for payments
- SMTP for Email
- Bootstrap 5
- Toastr & SweetAlert2
- DataTables + AJAX
- Razor Pages for Identity
- Cookie Authentication
- OAuth 2.0 (Google, GitHub, Facebook, etc.)

---

## 🧪 Design Patterns & Best Practices
- ✅ N-Tier Architecture
- ✅ Repository Pattern
- ✅ Unit of Work Pattern
- ✅ Dependency Injection
- ✅ Separation of Concerns (SoC)
- ✅ SOLID Principles
- ✅ Clean Code & Scalability

---

## 🛠️ Setup Instructions

1. **Clone the repository**:

   ```bash
   git clone https://github.com/Abdelrahman-Zagloul/Etechno-Ecommerce.git

2. Add your `appsettings.json` (not included in repo) with OAuth credentials:

   ```json
   {
   "ConnectionStrings": {
    "DefaultConnection": "YourDatabaseConnectionString"
    }
     "Authentication": {
       "Google": {
         "ClientId": "your-google-client-id",
         "ClientSecret": "your-google-secret"
       },
       "GitHub": {
         "ClientId": "your-github-client-id",
         "ClientSecret": "your-github-secret"
       },
       "Facebook": {
         "AppId": "your-facebook-app-id",
         "AppSecret": "your-facebook-secret"
       },
       "LinkedIn": {
         "ClientId": "your-linkedin-client-id",
         "ClientSecret": "your-linkedin-secret"
       },
       "Microsoft": {
         "ClientId": "your-microsoft-client-id",
         "ClientSecret": "your-microsoft-secret"
       }
     },
     "Email": {
       "SMTP": "smtp.example.com",
       "Port": 587,
       "Username": "your-email@example.com",
       "Password": "your-password"
     },
    "Stripe": {
       "PublishableKey":"Your PublishableKey",
       "Secretkey": "Your Secretkey"
      }
   ```

3. Apply Migrations:

   ```bash
   dotnet ef database update
   ```

4. Run the project:

   ```bash
   dotnet run
   ```

---

## 👨‍💻 Author

Abdelrahman Zaglol
.NET Developer | Computer Science Student
[LinkedIn](https://www.linkedin.com/in/abdelrahman-zagloul/)

---

## 📄 License

This project is licensed under the MIT License.

