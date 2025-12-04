using Guc_Uni_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Guc_Uni_System.Pages.Admin
{
    public class AdminDashboardModel : PageModel
    {
        private readonly UniversityHrManagementSystemContext _context;

        public AdminDashboardModel(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        // ==========================================
        // PROPERTIES (Part 1 & Part 2 Merged)
        // ==========================================

        // -- Part 1 Data Holders (Loaded on Get) --
        public List<AllEmployeeProfile> Employees { get; set; } = new List<AllEmployeeProfile>();
        public List<NoEmployeeDept> DeptCounts { get; set; } = new List<NoEmployeeDept>();
        public List<AllRejectedMedical> RejectedMedicalLeaves { get; set; } = new List<AllRejectedMedical>();

        // -- Part 2 Data Holders (Loaded via AJAX) --
        public List<AllEmployeeAttendance> AttendanceRecords { get; set; } = new List<AllEmployeeAttendance>();
        public List<AllPerformance> PerformanceRecords { get; set; } = new List<AllPerformance>();

        // -- Inputs (Merged) --
        [BindProperty]
        public int TargetEmployeeId { get; set; } // Used for generic ID inputs

        [BindProperty]
        public int NewEmployeeId { get; set; } // For Replace Employee

        [BindProperty]
        public string HolidayName { get; set; } // Part 1

        [BindProperty]
        public DateTime FromDate { get; set; } = DateTime.Today; // Shared Date Input

        [BindProperty]
        public DateTime ToDate { get; set; } = DateTime.Today; // Shared Date Input

        [BindProperty]
        public DateTime CheckIn { get; set; } // Part 1 Update Attendance

        [BindProperty]
        public DateTime CheckOut { get; set; } // Part 1 Update Attendance

        // -- UI Messages --
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;

        // ==========================================
        // HTTP GET (Security & Initial Data)
        // ==========================================
        public IActionResult OnGet()
        {
            // 1. Security Check (From Part 1)
            string role = HttpContext.Session.GetString("user_role");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            // 2. Load Part 1 Static Data
            try
            {
                Employees = _context.AllEmployeeProfiles.ToList();
                DeptCounts = _context.NoEmployeeDepts.ToList();
                RejectedMedicalLeaves = _context.AllRejectedMedicals.ToList();
            }
            catch (Exception ex)
            {
                // If DB fails on load, we can log it, but page should still render
                Console.WriteLine(ex.Message);
            }

            return Page();
        }

        // ==========================================
        // PART 1 ACTIONS (Converted to AJAX/JSON)
        // ==========================================

        public IActionResult OnPostAddHoliday()
        {
            return ExecuteDbAction(
                "EXEC Add_Holiday @holiday_name={0}, @from_date={1}, @to_date={2}",
                "Holiday Added Successfully!",
                HolidayName, FromDate, ToDate);
        }

        public IActionResult OnPostRemoveDeductions()
        {
            return ExecuteDbAction("EXEC Remove_Deductions", "Resigned Deductions Removed!");
        }

        public IActionResult OnPostInitAttendance()
        {
            return ExecuteDbAction("EXEC Initiate_Attendance", "Attendance Initialized for Today!");
        }

        public IActionResult OnPostUpdateAttendance()
        {
            if (TargetEmployeeId <= 0) return InvalidIdResponse();

            return ExecuteDbAction(
                "EXEC Update_Attendance @Employee_id={0}, @check_in_time={1}, @check_out_time={2}",
                "Attendance Updated Manually!",
                TargetEmployeeId, CheckIn, CheckOut);
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        // ==========================================
        // PART 2 FETCH METHODS (AJAX Data Return)
        // ==========================================

        public IActionResult OnPostViewYesterdayAttendance()
        {
            try
            {
                var records = _context.AllEmployeeAttendances.ToList();
                return new JsonResult(new { type = "data", category = "attendance", data = records });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { type = "message", success = false, message = "Error: " + ex.Message });
            }
        }

        public IActionResult OnPostViewWinterPerformance()
        {
            try
            {
                var records = _context.AllPerformances.ToList();
                return new JsonResult(new { type = "data", category = "performance", data = records });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { type = "message", success = false, message = "Error: " + ex.Message });
            }
        }

        // ==========================================
        // PART 2 ACTIONS (AJAX/JSON)
        // ==========================================

        public IActionResult OnPostRemoveHolidayAttendance()
        {
            return ExecuteDbAction("EXEC Remove_Holiday", "Holiday attendance records cleaned.");
        }

        public IActionResult OnPostRemoveUnattendedDayOff()
        {
            if (TargetEmployeeId <= 0) return InvalidIdResponse();
            return ExecuteDbAction($"EXEC Remove_DayOff @employee_ID={TargetEmployeeId}", "Unattended DayOff removed.");
        }

        public IActionResult OnPostRemoveApprovedLeaves()
        {
            if (TargetEmployeeId <= 0) return InvalidIdResponse();
            return ExecuteDbAction($"EXEC Remove_Approved_Leaves @employee_id={TargetEmployeeId}", "Approved leaves removed.");
        }

        public IActionResult OnPostReplaceEmployee()
        {
            if (TargetEmployeeId <= 0 || NewEmployeeId <= 0)
            {
                return new JsonResult(new { type = "message", success = false, message = "Please enter valid IDs." });
            }

            // Using formatted string for complex parameters
            string sql = $"EXEC Replace_employee @Emp1_ID={TargetEmployeeId}, @Emp2_ID={NewEmployeeId}, @from_date='{FromDate:yyyy-MM-dd}', @to_date='{ToDate:yyyy-MM-dd}'";
            return ExecuteDbAction(sql, "Employee replaced successfully.");
        }

        public IActionResult OnPostUpdateDailyStatus()
        {
            if (TargetEmployeeId <= 0) return InvalidIdResponse();
            return ExecuteDbAction($"EXEC Update_Employment_Status @Employee_ID={TargetEmployeeId}", "Employment status updated.");
        }

        // ==========================================
        // HELPERS
        // ==========================================
        private IActionResult InvalidIdResponse()
        {
            return new JsonResult(new { type = "message", success = false, message = "Please enter a valid Employee ID." });
        }

        // Overloaded helper to handle both simple strings and parameterized queries
        private IActionResult ExecuteDbAction(string sql, string successMsg, params object[] parameters)
        {
            try
            {
                if (parameters != null && parameters.Length > 0)
                    _context.Database.ExecuteSqlRaw(sql, parameters);
                else
                    _context.Database.ExecuteSqlRaw(sql);

                return new JsonResult(new { type = "message", success = true, message = successMsg });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { type = "message", success = false, message = "SQL Error: " + (ex.InnerException?.Message ?? ex.Message) });
            }
        }
    }
}