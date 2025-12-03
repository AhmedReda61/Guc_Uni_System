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
            if (id == "admin" && password == "admin123")
            {
                return new UserLoginResult { IsSuccess = true, Role = "Admin", UserId = 0 };
            }

            // 2. DATABASE CHECK (Using SQL Functions)
            if (int.TryParse(id, out int empId))
            {
                // A. Check if they are HR
                try
                {
                    bool isHr = _context.Database
                        .SqlQueryRaw<bool>("SELECT dbo.HRLoginValidation({0}, {1})", empId, password)
                        .AsEnumerable() // Execute immediately
                        .FirstOrDefault();

                    if (isHr)
                    {
                        return new UserLoginResult
                        {
                            IsSuccess = true,
                            Role = "HR",
                            UserId = empId
                        };
                    }

                    // B. Check if they are Academic (Non-HR)
                    bool isAcademic = _context.Database
                        .SqlQueryRaw<bool>("SELECT dbo.EmployeeLoginValidation({0}, {1})", empId, password)
                        .AsEnumerable()
                        .FirstOrDefault();

                    if (isAcademic)
                    {
                        return new UserLoginResult
                        {
                            IsSuccess = true,
                            Role = "Academic",
                            UserId = empId
                        };
                    }
                }
                catch (Exception ex)
                {
                    // If the database connection fails or function doesn't exist
                    return new UserLoginResult { IsSuccess = false, ErrorMessage = "Database Error: " + ex.Message };
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