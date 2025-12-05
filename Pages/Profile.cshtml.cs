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
        public string DashboardUrl { get; set; } // Dynamic Back Link

        public IActionResult OnGet()
        {
            // 1. Check Role
            string role = HttpContext.Session.GetString("user_role");
            if (string.IsNullOrEmpty(role)) return RedirectToPage("/Login");

            // 2. Set Back Button Destination
            if (role == "Admin")
            {
                DashboardUrl = "/Admin/AdminDashboard";
                // Admin doesn't have a profile in DB, but we prevent crash if they type URL
                CurrentEmployee = new Employee { FirstName = "Admin", LastName = "User", Email = "admin@system.local", EmploymentStatus = "System", DeptName = "IT" };
                return Page();
            }
            else
            {
                DashboardUrl = "/AcademicDashboard"; // Academic Employee
            }

            // 3. Fetch Real Employee Data
            int? userId = HttpContext.Session.GetInt32("user_id");
            if (userId == null) return RedirectToPage("/Login");

            CurrentEmployee = _context.Employees
                .FirstOrDefault(e => e.EmployeeId == userId);

            if (CurrentEmployee == null) return RedirectToPage("/Login");

            return Page();
        }
    }
}