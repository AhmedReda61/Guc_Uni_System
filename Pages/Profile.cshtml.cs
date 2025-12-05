using Guc_Uni_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Guc_Uni_System.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly UniversityHrManagementSystemContext _context;

        public ProfileModel(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        public Employee CurrentEmployee { get; set; }

        // This was missing and caused the error
        public string DashboardUrl { get; set; }

        public IActionResult OnGet()
        {
            // 1. Get Role from Session
            string role = HttpContext.Session.GetString("user_role");

            if (string.IsNullOrEmpty(role))
            {
                return RedirectToPage("/Login");
            }

            // 2. Logic for ADMIN (Who doesn't exist in the database)
            if (role == "Admin")
            {
                // Set the Back Button to point to Admin Dashboard
                DashboardUrl = "/Admin/AdminDashboard";

                // Create a "Fake" Profile so the page doesn't crash
                CurrentEmployee = new Employee
                {
                    EmployeeId = 0,
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@guc.edu.eg",
                    EmploymentStatus = "Active",
                    TypeOfContract = "System Access",
                    Address = "Server Room 1",
                    DeptName = "IT Administration",
                    YearsOfExperience = 99,
                    OfficialDayOff = "None",
                    Salary = 0,
                    AnnualBalance = 0,
                    AccidentalBalance = 0,
                    NationalId = "00000000000000",
                    Gender = "N/A"
                };
                return Page();
            }

            // 3. Logic for ACADEMIC EMPLOYEE (Who exists in the database)
            else
            {
                // Set the Back Button to point to Academic Dashboard
                DashboardUrl = "/AcademicDashboard";

                int? userId = HttpContext.Session.GetInt32("user_id");
                if (userId == null) return RedirectToPage("/Login");

                CurrentEmployee = _context.Employees
                    .FirstOrDefault(e => e.EmployeeId == userId);

                if (CurrentEmployee == null) return RedirectToPage("/Login");

                return Page();
            }
        }
    }
}