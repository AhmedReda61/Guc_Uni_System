using Guc_Uni_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Guc_Uni_System.Pages.Admin
{
    public class AdminPartTwoModel : PageModel
    {
        private readonly UniversityHrManagementSystemContext _context;

        public AdminPartTwoModel(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        // --- Data Holders ---
        public List<AllEmployeeAttendance> AttendanceRecords { get; set; } = new List<AllEmployeeAttendance>();
        public List<AllPerformance> PerformanceRecords { get; set; } = new List<AllPerformance>();

        // --- Form Inputs ---
        [BindProperty]
        public int TargetEmployeeId { get; set; }

        [BindProperty]
        public int NewEmployeeId { get; set; }

        [BindProperty]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime ToDate { get; set; } = DateTime.Today;

        // --- UI Messages ---
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;

        public void OnGet()
        {
        }

        // 1. Fetch Yesterday's Attendance
        public void OnPostViewYesterdayAttendance()
        {
            try
            {
                // 'allEmployeeAttendance' is a VIEW based on your SQL
                AttendanceRecords = _context.AllEmployeeAttendances.ToList();
                IsSuccess = true;
                Message = "Attendance retrieved successfully.";
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Message = "Error: " + ex.Message;
            }
        }

        // 2. Fetch Winter Performance
        public void OnPostViewWinterPerformance()
        {
            try
            {
                // 'allPerformance' is a VIEW based on your SQL
                PerformanceRecords = _context.AllPerformances.ToList();
                IsSuccess = true;
                Message = "Performance retrieved successfully.";
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Message = "Error: " + ex.Message;
            }
        }

        // 3. Remove Holiday Attendance
        public IActionResult OnPostRemoveHolidayAttendance()
        {
            return ExecuteDbAction("EXEC Remove_Holiday", "Holiday attendance records cleaned.");
        }

        // 4. Remove Unattended DayOff
        public IActionResult OnPostRemoveUnattendedDayOff()
        {
            if (TargetEmployeeId <= 0)
            {
                IsSuccess = false;
                Message = "Invalid ID";
                return Page();
            }
            // Parameter name in your SQL is @employee_ID
            return ExecuteDbAction($"EXEC Remove_DayOff @employee_ID={TargetEmployeeId}", "DayOff removed.");
        }

        // 5. Remove Approved Leaves
        public IActionResult OnPostRemoveApprovedLeaves()
        {
            if (TargetEmployeeId <= 0)
            {
                IsSuccess = false;
                Message = "Invalid ID";
                return Page();
            }
            // Parameter name in your SQL is @employee_id
            return ExecuteDbAction($"EXEC Remove_Approved_Leaves @employee_id={TargetEmployeeId}", "Approved leaves removed.");
        }

        // 6. Replace Employee
        public IActionResult OnPostReplaceEmployee()
        {
            if (TargetEmployeeId <= 0 || NewEmployeeId <= 0)
            {
                IsSuccess = false;
                Message = "Invalid IDs";
                return Page();
            }

            // Parameters match your SQL: @Emp1_ID, @Emp2_ID, @from_date, @to_date
            string sql = $"EXEC Replace_employee @Emp1_ID={TargetEmployeeId}, @Emp2_ID={NewEmployeeId}, @from_date='{FromDate:yyyy-MM-dd}', @to_date='{ToDate:yyyy-MM-dd}'";
            return ExecuteDbAction(sql, "Employee replaced successfully.");
        }

        // 7. Update Employment Status
        public IActionResult OnPostUpdateDailyStatus()
        {
            if (TargetEmployeeId <= 0)
            {
                IsSuccess = false;
                Message = "Invalid ID";
                return Page();
            }
            // Parameter name in your SQL is @Employee_ID
            return ExecuteDbAction($"EXEC Update_Employment_Status @Employee_ID={TargetEmployeeId}", "Status updated.");
        }

        // --- Helper Method ---
        private IActionResult ExecuteDbAction(string sql, string successMsg)
        {
            try
            {
                _context.Database.ExecuteSqlRaw(sql);
                IsSuccess = true;
                Message = successMsg;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Message = "SQL Error: " + (ex.InnerException?.Message ?? ex.Message);
            }
            return Page();
        }
    }
}