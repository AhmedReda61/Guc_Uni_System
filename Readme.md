# Google Gemini

## University HR Management System

### Overview

The **University HR Management System** is a full-stack web application designed to streamline and automate the complex administrative workflows of a university environment. It serves as a centralized platform for managing employee records, attendance, payroll, leave requests, and performance evaluations.

The system features **role-based access control** with distinct dashboards for Admins, HR Personnel, and Academic Staff (Lecturers, Teaching Assistants, Deans, etc.), ensuring secure and efficient data management across all departments.

---

## Key Features

### 🔐 Role-Based Access Control

- **Secure Authentication**: Custom logic validates credentials against SQL Server records, distinguishing between HR and Academic staff.
- **Dynamic Dashboards**: The UI adapts automatically based on the user's role (Admin, HR, Academic, Dean, President).

### 🛠️ Admin Module

- **System Management**: Initialize daily attendance records and add official holidays.
- **Data Oversight**: View comprehensive lists of all employees, track active vs. resigned status, and view rejected leaves.
- **Maintenance**: Automated cleaning of resigned employee deductions and updating of document statuses.

### 💼 HR Module

- **Payroll Automation**: Generate monthly payrolls with automatic calculations for base salary, bonuses (overtime), and deductions.
- **Leave Management**: Final approval authority for annual, accidental, and unpaid leaves based on strict balance and policy checks.
- **Deduction Handling**: Automatic calculation and application of penalties for missing hours or days.

### 🎓 Academic Module

- **Self-Service Portal**: View personal attendance, payroll history, and performance reports.
- **Leave Applications**: Apply for annual, medical, accidental, or compensation leaves with automatic replacement checks.
- **Management Functions**: Deans and Presidents can approve leave requests and submit performance evaluations for their department members.

---

## Tech Stack

### Backend

- **Framework**: ASP.NET Core 8.0 (Razor Pages)
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core (Database-First approach)
- **Architecture**: Service-based architecture with Separation of Concerns (SoC)

### Frontend

- **Framework**: Bootstrap 5 (Responsive Design)
- **Styling**: Custom CSS overrides for modern UI components
- **Interactive Elements**: Bootstrap Modals, Accordions, and Tabs for organized data presentation

### Database Logic

- **Stored Procedures**: 40+ complex procedures handling business logic (e.g., `Submit_annual`, `Add_Payroll`, `HR_approval_comp`).
- **Functions**: Custom scalar and table-valued functions for calculating salaries, overtime bonuses, and retrieving status reports.
- **Triggers & Views**: Automated data consistency checks and read-optimized views for dashboards.

---

## Installation & Setup

### Prerequisites

- Visual Studio 2022 (with ASP.NET and web development workload)
- Microsoft SQL Server
- .NET 8.0 SDK

### Steps

#### 1. Clone the Repository

```bash
git clone https://github.com/YourUsername/University-HR-System.git
```

#### 2. Database Setup

- Open SQL Server Management Studio (SSMS).
- Run the provided `Script.sql` script to create the database, tables, and stored procedures.
- Insert initial dummy data (Employees, Departments, Roles) to test the system.

#### 3. Configure Connection

Open `appsettings.json` in the project root and update the connection string:

```json
"ConnectionStrings": {
  "MyDbConnection": "Server=YOUR_SERVER_NAME;Database=University_HR_ManagementSystem;Trusted_Connection=True;"
}
```

#### 4. Run the Application

- Open the solution file (`.sln`) in Visual Studio.
- Press **F5** or click the **Green Play** button to build and launch the server.
- The browser will open to the Home Page.

---

## Usage Guide

- **Login**: Use the hardcoded Admin credentials (`admin / admin123`) or a valid Employee ID/Password from your database.
- **Navigation**: Use the top navigation bar to switch between the Home page, All Data Viewer, and your specific Dashboard.
- **All Data Viewer**: A utility page available to verify database content directly from the UI without opening SSMS.

---

## Contributors

- Ahmed Reda
- Add Team Member Name
- Add Team Member Name
