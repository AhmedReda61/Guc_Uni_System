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

        // Data holders for the Views
        public List<AttendanceViewModel> AttendanceRecords { get; set; } = new List<AttendanceViewModel>();
        public List<PerformanceViewModel> PerformanceRecords { get; set; } = new List<PerformanceViewModel>();

        // Inputs for Forms
        [BindProperty]
        public int TargetEmployeeId { get; set; }

        [BindProperty]
        public int NewEmployeeId { get; set; } // For "Replace Employee"

        // New Inputs required by Milestone 2 Schema
        [BindProperty]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime ToDate { get; set; } = DateTime.Today;

        // UI Feedback
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;

        public void OnGet()
        {
        }

        // 1. Fetch attendance records for all employees for yesterday.
        // CORRECTED: Uses SELECT because 'allEmployeeAttendance' is a VIEW
        public void OnPostViewYesterdayAttendance()
        {
            try
            {
                AttendanceRecords = _context.Database
                    .SqlQuery<AttendanceViewModel>($"SELECT * FROM allEmployeeAttendance")
                    .ToList();

                IsSuccess = true;
                Message = "Attendance for yesterday retrieved successfully.";
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Message = "Error: " + ex.Message;
            }
        }

        // 2. Fetch details for the performance of all employees in all Winter semesters.
        // CORRECTED: Uses SELECT because 'allPerformance' is a VIEW
        public void OnPostViewWinterPerformance()
        {
            try
            {
                PerformanceRecords = _context.Database
                    .SqlQuery<PerformanceViewModel>($"SELECT * FROM allPerformance")
                    .ToList();

                IsSuccess = true;
                Message = "Winter performance records retrieved successfully.";
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Message = "Error: " + ex.Message;
            }
        }

        // 3. Remove attendance records for all employees during official holidays
        // CORRECTED Name: Remove_Holiday
        public IActionResult OnPostRemoveHolidayAttendance()
        {
            return ExecuteDbAction("EXEC Remove_Holiday", "Holiday attendance records cleaned.");
        }

        // 4. Remove unattended dayoff for a certain employee (Current Month)
        // CORRECTED Name: Remove_DayOff, Param: @Employee_id
        public IActionResult OnPostRemoveUnattendedDayOff()
        {
            if (TargetEmployeeId <= 0)
            {
                IsSuccess = false;
                Message = "Please enter a valid Employee ID.";
                return Page();
            }

            return ExecuteDbAction($"EXEC Remove_DayOff @Employee_id={TargetEmployeeId}",
                $"Unattended day-offs removed for Employee {TargetEmployeeId}.");
        }

        // 5. Remove approved leaves for a certain employee from attendance
        // CORRECTED Name: Remove_Approved_Leaves, Param: @Employee_id
        public IActionResult OnPostRemoveApprovedLeaves()
        {
            if (TargetEmployeeId <= 0)
            {
                IsSuccess = false;
                Message = "Please enter a valid Employee ID.";
                return Page();
            }

            return ExecuteDbAction($"EXEC Remove_Approved_Leaves @Employee_id={TargetEmployeeId}",
                $"Approved leaves removed from attendance for Employee {TargetEmployeeId}.");
        }

        // 6. Replace another employee
        // CORRECTED Name: Replace_employee
        // ADDED Params: @from_date, @to_date
        public IActionResult OnPostReplaceEmployee()
        {
            if (TargetEmployeeId <= 0 || NewEmployeeId <= 0)
            {
                IsSuccess = false;
                Message = "Please enter valid IDs for both employees.";
                return Page();
            }

            // Using formatted strings for dates to ensure SQL compatibility (YYYY-MM-DD)
            string sql = $"EXEC Replace_employee @Emp1_ID={TargetEmployeeId}, @Emp2_ID={NewEmployeeId}, @from_date='{FromDate:yyyy-MM-dd}', @to_date='{ToDate:yyyy-MM-dd}'";

            return ExecuteDbAction(sql, $"Employee {TargetEmployeeId} replaced by {NewEmployeeId}.");
        }

        // 7. Update employment_status daily based on leaves/active
        // CORRECTED Name: Update_Employment_Status, Added Param: @Employee_ID
        public IActionResult OnPostUpdateDailyStatus()
        {
            if (TargetEmployeeId <= 0)
            {
                IsSuccess = false;
                Message = "Please enter a valid Employee ID.";
                return Page();
            }

            return ExecuteDbAction($"EXEC Update_Employment_Status @Employee_ID={TargetEmployeeId}",
                $"Employment status updated for Employee {TargetEmployeeId}.");
        }

        // Helper function
        private IActionResult ExecuteDbAction(string sql, string successMessage)
        {
            try
            {
                _context.Database.ExecuteSqlRaw(sql);
                IsSuccess = true;
                Message = successMessage;
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                Message = "Operation failed: " + (ex.InnerException?.Message ?? ex.Message);
            }
            return Page();
        }

        // View Models
        public class AttendanceViewModel
        {
            // Ensure these match the columns in your 'allEmployeeAttendance' view
            public int? Employee_id { get; set; } // SQL might return Employee_id (case sensitive)
            public DateTime? start_date { get; set; } // View usually returns 'start_date' for attendance date
            public string? status { get; set; }
            public TimeSpan? start_time { get; set; } // check-in
            public TimeSpan? end_time { get; set; }   // check-out
        }

        public class PerformanceViewModel
        {
            // Ensure these match columns in 'allPerformance' view
            public int? Employee_id { get; set; }
            public string? semester_code { get; set; }
            public string? course_name { get; set; }
            public int? missing_hours { get; set; }
            public int? rating { get; set; }
        }
    }
}