using Guc_Uni_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Guc_Uni_System.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly UniversityHrManagementSystemContext _context;

        public AdminDashboardModel(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        public List<Employee> Employees { get; set; } = new List<Employee>();

        public IActionResult OnGet()
        {
            // 1. Security Check (Are they an Admin?)
            string role = HttpContext.Session.GetString("user_role");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            // 2. Fetch Employees using EF Core
            // We use .Include() to get the Department name if it's a related table
            // If Department is just a string column in Employee, remove .Include
            Employees = _context.Employees.ToList();

            return Page();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}