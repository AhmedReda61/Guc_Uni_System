using Guc_Uni_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Guc_Uni_System.Services
{
    public class AuthService
    {
        private readonly UniversityHrManagementSystemContext _context;

        public AuthService(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        public UserLoginResult ValidateUser(string id, string password)
        {
            // 1. SIMPLE ADMIN CHECK
            if (id == "a" && password == "a")
            {
                return new UserLoginResult { IsSuccess = true, Role = "Admin", UserId = 0 };
            }

            // 2. DATABASE CHECK
            if (int.TryParse(id, out int empId))
            {
                // Notice we include 'Roles' (or whatever the list is named in Employee.cs)
                // CHECK YOUR Employee.cs: If the list is named 'RoleNames', change 'e.Roles' to 'e.RoleNames'

                var user = _context.Employees
                    .Include(e => e.RoleNames)
                    .FirstOrDefault(e => e.EmployeeId == empId && e.Password == password);

                if (user != null)
                {
                    // Now we check the Role table directly
                    // Adjust 'RoleName' if your Role.cs property is named 'Name' or 'Title'
                    bool isHr = user.RoleNames.Any(r => r.RoleName.Contains("HR"));

                    return new UserLoginResult
                    {
                        IsSuccess = true,
                        Role = isHr ? "HR" : "Academic",
                        UserId = empId
                    };
                }
            }

            return new UserLoginResult { IsSuccess = false, ErrorMessage = "Invalid Credentials" };
        }
    }

    public class UserLoginResult
    {
        public bool IsSuccess { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public string ErrorMessage { get; set; }
    }
}