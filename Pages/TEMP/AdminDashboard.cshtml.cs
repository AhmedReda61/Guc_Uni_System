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

       
        public List<AllEmployeeProfile> Employees { get; set; } = new List<AllEmployeeProfile>();
        public List<NoEmployeeDept> DeptCounts { get; set; } = new List<NoEmployeeDept>();
        public List<AllRejectedMedical> RejectedMedicalLeaves { get; set; } = new List<AllRejectedMedical>();
        public List<AllEmployeeAttendance> AttendanceRecords { get; set; } = new List<AllEmployeeAttendance>();
        public List<AllPerformance> PerformanceRecords { get; set; } = new List<AllPerformance>();

    
        [BindProperty]
        public int TargetEmployeeId { get; set; }

        [BindProperty]
        public int NewEmployeeId { get; set; }

        [BindProperty]
        public string HolidayName { get; set; }

        [BindProperty]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime ToDate { get; set; } = DateTime.Today;

        [BindProperty]
        public DateTime CheckIn { get; set; }

        [BindProperty]
        public DateTime CheckOut { get; set; }

        public IActionResult OnGet()
        {
            string role = HttpContext.Session.GetString("user_role");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            try
            {
                Employees = _context.AllEmployeeProfiles.ToList();
                DeptCounts = _context.NoEmployeeDepts.ToList();
                RejectedMedicalLeaves = _context.AllRejectedMedicals.ToList();
                //AttendanceRecords = _context.AllEmployeeAttendances.ToList();
                //PerformanceRecords = _context.AllPerformances.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Page();
        }

        // 2.1)
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

        // 2.2)
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

        // 1.5)
        public IActionResult OnPostRemoveDeductions()
        {
            return ExecuteDbAction("EXEC Remove_Deductions", "Resigned Deductions Removed!");
        }

        // 1.6)
        public IActionResult OnPostUpdateAttendance()
        {
            return ExecuteDbAction(
                "EXEC Update_Attendance @Employee_id={0}, @check_in_time={1}, @check_out_time={2}",
                "Attendance Updated Manually!",
                TargetEmployeeId, CheckIn, CheckOut);
        }

        // 1.7)
        public IActionResult OnPostAddHoliday()
        {
            return ExecuteDbAction(
                "EXEC Add_Holiday @holiday_name={0}, @from_date={1}, @to_date={2}",
                "Holiday Added Successfully!",
                HolidayName, FromDate, ToDate);
        }

        // 1.8)
        public IActionResult OnPostInitAttendance()
        {
            return ExecuteDbAction("EXEC Initiate_Attendance", "Attendance Initialized for Today!");
        }

       // 2.3)
        public IActionResult OnPostRemoveHolidayAttendance()
        {
            return ExecuteDbAction("EXEC Remove_Holiday", "Holiday attendance records cleaned.");
        }

        // 2.4)
        public IActionResult OnPostRemoveUnattendedDayOff()
        {
            return ExecuteDbAction($"EXEC Remove_DayOff @employee_ID={TargetEmployeeId}", "Unattended DayOff removed.");
        }

        // 2.5)
        public IActionResult OnPostRemoveApprovedLeaves()
        {
            return ExecuteDbAction($"EXEC Remove_Approved_Leaves @employee_id={TargetEmployeeId}", "Approved leaves removed.");
        }

        // 2.6)
        public IActionResult OnPostReplaceEmployee()
        {
            string sql = $"EXEC Replace_employee @Emp1_ID={TargetEmployeeId}, @Emp2_ID={NewEmployeeId}, @from_date='{FromDate:yyyy-MM-dd}', @to_date='{ToDate:yyyy-MM-dd}'";
            return ExecuteDbAction(sql, "Employee replaced successfully.");
        }

        // 2.7)
        public IActionResult OnPostUpdateDailyStatus()
        {
            return ExecuteDbAction($"EXEC Update_Employment_Status @Employee_ID={TargetEmployeeId}", "Employment status updated.");
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

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