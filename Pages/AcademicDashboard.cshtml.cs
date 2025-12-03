using Guc_Uni_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Guc_Uni_System.Pages
{
    // Placeholder models to simulate EF Core entities for required data
    public class MyAttendanceRecord
    {
        public DateTime Date { get; set; }
        public TimeSpan CheckIn { get; set; }
        public TimeSpan CheckOut { get; set; }
        public string Status { get; set; }
    }
    public class MyLastPayroll
    {
        public DateTime PaymentDate { get; set; }
        public decimal FinalSalaryAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal DeductionsAmount { get; set; }
    }
    public class MyAnnualAccidentalLeaveStatus
    {
        public int RequestId { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
    public class PerformanceRecord
    {
        public int PerformanceId { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
    }
    public class DeductionRecord
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }


    public class AcademicDashboardModel : PageModel
    {
        private readonly UniversityHrManagementSystemContext _context;

        public AcademicDashboardModel(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        // Data retrieved on page load (OnGet)
        public MyAttendanceRecord LastPayroll { get; set; } = new MyAttendanceRecord(); // Req 4
        public List<MyAttendanceRecord> AttendanceRecords { get; set; } = new List<MyAttendanceRecord>(); // Req 3
        public List<MyAnnualAccidentalLeaveStatus> CurrentLeaves { get; set; } = new List<MyAnnualAccidentalLeaveStatus>(); // Req 7

        // Data retrieved on form submission (OnPost)
        public List<PerformanceRecord> PerformanceResults { get; set; } = new List<PerformanceRecord>(); // Req 2
        public List<DeductionRecord> DeductionResults { get; set; } = new List<DeductionRecord>(); // Req 5


        // Bind Properties for Form Submissions

        // Req 2: Performance
        [BindProperty]
        public string PerformanceSemester { get; set; }

        // Req 5: Deductions
        [BindProperty]
        public DateTime DeductionFrom { get; set; }

        [BindProperty]
        public DateTime DeductionTo { get; set; }

        // Req 6: Annual Leave
        [BindProperty]
        [Required(ErrorMessage = "Start Date is required.")]
        public DateTime AnnualFrom { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "End Date is required.")]
        public DateTime AnnualTo { get; set; }

        [BindProperty]
        public string AnnualReason { get; set; }


        public IActionResult OnGet()
        {
            // 1. Security Check (Are they an Academic Employee?)
            string role = HttpContext.Session.GetString("user_role");
            string empIdString = HttpContext.Session.GetString("user_id");

            if (role != "Academic Employee" || string.IsNullOrEmpty(empIdString))
            {
                return RedirectToPage("/Login");
            }
            int empId = int.Parse(empIdString);

            // 3. Retrieve attendance records for the current month
            // Assuming there is a function/view 'View_My_Attendance_Current_Month' that takes empId
            // Using ExecuteSqlRaw to simulate calling a function or a view dynamically
            AttendanceRecords = _context.MyAttendanceRecords.FromSqlRaw("SELECT * FROM View_My_Attendance_Current_Month({0})", empId).ToList();

            // 4. Retrieve last month's payroll details
            // Assuming 'View_My_Last_Payroll' is a view/function
            LastPayroll = _context.MyLastPayrolls.FromSqlRaw("SELECT * FROM View_My_Last_Payroll({0})", empId).FirstOrDefault();

            // 7. Retrieve the status of annual/accidental leaves during the current month
            // Assuming 'View_My_Current_Annual_Accidental_Leaves' is a view/function
            CurrentLeaves = _context.MyAnnualAccidentalLeaveStatuses.FromSqlRaw("SELECT * FROM View_My_Current_Annual_Accidental_Leaves({0})", empId).ToList();


            // Check if there are any results from a previous OnPost (Req 2 or 5)
            // Storing results in TempData is a common pattern for Post/Redirect/Get (PRG)
            if (TempData.ContainsKey("PerformanceResults"))
            {
                PerformanceResults = (List<PerformanceRecord>)TempData["PerformanceResults"];
            }
            if (TempData.ContainsKey("DeductionResults"))
            {
                DeductionResults = (List<DeductionRecord>)TempData["DeductionResults"];
            }

            return Page();
        }

        // Req 2: Handler for viewing performance
        public IActionResult OnPostViewPerformance()
        {
            string empIdString = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrEmpty(empIdString)) return RedirectToPage("/Login");
            int empId = int.Parse(empIdString);

            // Assuming a stored function/view 'View_My_Performance' that returns performance records
            var results = _context.PerformanceRecords
                .FromSqlRaw("SELECT * FROM View_My_Performance @emp_id={0}, @semester={1}", empId, PerformanceSemester)
                .ToList();

            // Use TempData to pass the results back to OnGet after redirect
            TempData["PerformanceResults"] = results;
            TempData["Msg"] = $"Performance for semester '{PerformanceSemester}' retrieved.";
            return RedirectToPage();
        }

        // Req 5: Handler for fetching deductions
        public IActionResult OnPostViewDeductions()
        {
            string empIdString = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrEmpty(empIdString)) return RedirectToPage("/Login");
            int empId = int.Parse(empIdString);

            // Assuming a stored function/view 'Get_Attendance_Deductions'
            var results = _context.DeductionRecords
                .FromSqlRaw("SELECT * FROM Get_Attendance_Deductions @emp_id={0}, @from_date={1}, @to_date={2}",
                    empId, DeductionFrom, DeductionTo)
                .ToList();

            // Use TempData to pass the results back to OnGet after redirect
            TempData["DeductionResults"] = results;
            TempData["Msg"] = "Deductions retrieved for the specified period.";
            return RedirectToPage();
        }

        // Req 6: Handler for applying annual leave
        public IActionResult OnPostApplyAnnualLeave()
        {
            if (!ModelState.IsValid)
            {
                // Re-fetch OnGet data if form validation fails (not necessary for this mock, but good practice)
                return OnGet();
            }

            string empIdString = HttpContext.Session.GetString("user_id");
            if (string.IsNullOrEmpty(empIdString)) return RedirectToPage("/Login");
            int empId = int.Parse(empIdString);

            // Assuming a stored procedure 'Apply_Annual_Leave' exists
            // Reason is optional in the DB but included here for completeness
            _context.Database.ExecuteSqlRaw("EXEC Apply_Annual_Leave @emp_id={0}, @start_date={1}, @end_date={2}, @reason={3}",
                empId, AnnualFrom, AnnualTo, AnnualReason ?? (object)DBNull.Value);

            TempData["Msg"] = "Annual Leave Application Submitted!";
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}