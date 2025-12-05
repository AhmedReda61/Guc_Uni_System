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
        public string DepartmentName { get; set; }
        public string RoleName { get; set; }

        public IActionResult OnGet()
        {
            // 1. Get User ID from Session
            // Note: Ensure your Login page sets "user_id"
            int? userId = HttpContext.Session.GetInt32("user_id");

            // FALLBACK FOR TESTING (Remove this in production)
            if (userId == null || userId == 0) userId = 1;

            if (userId == null) return RedirectToPage("/Login");

            // 2. Fetch Employee Details
            CurrentEmployee = _context.Employees
                .FirstOrDefault(e => e.EmployeeId == userId);

            if (CurrentEmployee == null) return RedirectToPage("/Login");

            // 3. Fetch Extra Info (Dept & Role) logic if needed
            // Assuming Department is directly in Employee table or View
            // If you used the View 'AllEmployeeProfile', you could fetch from there too.

            return Page();
        }
    }
}