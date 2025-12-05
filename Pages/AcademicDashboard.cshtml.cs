using Guc_Uni_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Guc_Uni_System.Pages
{
    public class AcademicDashboardModel : PageModel
    {
        private readonly UniversityHrManagementSystemContext _context;

        public AcademicDashboardModel(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        // --- ROLE FLAGS ---
        public bool IsDean { get; set; }
        public bool IsViceDean { get; set; }
        public bool IsPresident { get; set; }
        public bool IsVicePresident { get; set; }
        public bool IsHeadOfDept { get; set; }

        // --- FILTER INPUTS ---
        [BindProperty(SupportsGet = true)]
        public string TargetSemester { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TargetMonth { get; set; } = DateTime.Now.Month;

        // --- DATA PROPERTIES ---
        public List<Performance> MyPerformances { get; set; } = new List<Performance>();
        public List<Attendance> MyAttendances { get; set; } = new List<Attendance>();
        public List<Payroll> MyPayrolls { get; set; } = new List<Payroll>();
        public List<Deduction> MyDeductions { get; set; } = new List<Deduction>();
        public List<LeaveStatusDto> MyLeavesStatus { get; set; } = new List<LeaveStatusDto>();

        // --- FORM INPUTS ---
        [BindProperty] public DateTime AnnualStart { get; set; } = DateTime.Today;
        [BindProperty] public DateTime AnnualEnd { get; set; } = DateTime.Today;
        [BindProperty] public int ReplacementId { get; set; }

        [BindProperty] public DateTime AccidentalStart { get; set; } = DateTime.Today;
        [BindProperty] public DateTime AccidentalEnd { get; set; } = DateTime.Today;

        [BindProperty] public DateTime MedicalStart { get; set; } = DateTime.Today;
        [BindProperty] public DateTime MedicalEnd { get; set; } = DateTime.Today;
        [BindProperty] public string MedicalType { get; set; }
        [BindProperty] public bool InsuranceStatus { get; set; }
        [BindProperty] public string DisabilityDetails { get; set; }
        [BindProperty] public string MedicalDocDesc { get; set; }
        [BindProperty] public string MedicalFileName { get; set; }

        [BindProperty] public DateTime UnpaidStart { get; set; } = DateTime.Today;
        [BindProperty] public DateTime UnpaidEnd { get; set; } = DateTime.Today;
        [BindProperty] public string UnpaidDocDesc { get; set; }
        [BindProperty] public string UnpaidFileName { get; set; }

        [BindProperty] public DateTime CompDate { get; set; } = DateTime.Today;
        [BindProperty] public DateTime CompOriginalDate { get; set; } = DateTime.Today;
        [BindProperty] public string CompReason { get; set; }
        [BindProperty] public int CompReplacementId { get; set; }

        [BindProperty] public int ManageRequestId { get; set; }
        [BindProperty] public int ManageReplacementId { get; set; }
        [BindProperty] public int EvalEmpId { get; set; }
        [BindProperty] public int EvalRating { get; set; }
        [BindProperty] public string EvalComment { get; set; }
        [BindProperty] public string EvalSemester { get; set; }

        // --- ON GET (Restored Original Logic) ---
        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "Academic")
                return RedirectToPage("/Login");

            int myId = HttpContext.Session.GetInt32("user_id").GetValueOrDefault();

            // 1. DETECT ROLES (Using your colleague's Include logic)
            var employee = _context.Employees
                .Include(e => e.RoleNames) // Using the navigation property
                .FirstOrDefault(e => e.EmployeeId == myId);

            if (employee != null && employee.RoleNames != null)
            {
                var myRoleStrings = employee.RoleNames.Select(r => r.RoleName).ToList();

                IsDean = myRoleStrings.Contains("Dean");
                IsViceDean = myRoleStrings.Contains("Vice Dean");
                IsPresident = myRoleStrings.Contains("President");
                IsVicePresident = myRoleStrings.Contains("Vice President");
            }

            IsHeadOfDept = IsDean || IsViceDean || IsPresident || IsVicePresident;

            // 2. Fetch Data Views (Using your original FromSqlRaw logic)
            try
            {
                MyPerformances = _context.Performances
                    .FromSqlRaw("SELECT * FROM dbo.MyPerformance({0}, {1})", myId, TargetSemester) // Hardcoding semester or make it dynamic
                    .ToList();
            }
            catch { }

            // 2. My Attendance (Current Month)
            try
            {
                MyAttendances = _context.Attendances
                    .FromSqlRaw("SELECT * FROM dbo.MyAttendance({0})", myId)
                    .ToList();

                MyPayrolls = _context.Payrolls
                    .FromSqlRaw("SELECT * FROM dbo.Last_month_payroll({0})", myId)
                    .ToList();

                MyDeductions = _context.Deductions
                    .FromSqlRaw("SELECT * FROM dbo.Deductions_Attendance({0}, {1})", myId, TargetMonth)
                    .ToList();

                var leavesData = _context.Database.SqlQueryRaw<LeaveStatusDto>(
                        @"SELECT 
                            request_ID AS RequestId, 
                            date_of_request AS DateOfRequest, 
                            final_approval_status AS Status 
                            FROM dbo.status_leaves({0})",
                        myId).ToList();
                MyLeavesStatus = leavesData;
            }
            catch
            {
                // Suppress errors for now as per instructions
            }

            return Page();
        }

        // --- ACTION HANDLERS ---

        public IActionResult OnPostApplyAnnual()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            ExecuteDb($"EXEC Submit_annual @employee_ID={myId}, @replacement_emp={ReplacementId}, @start_date='{AnnualStart:yyyy-MM-dd}', @end_date='{AnnualEnd:yyyy-MM-dd}'");
            TempData["Msg"] = "Annual Leave Submitted!";
            return RedirectToPage();
        }

        public IActionResult OnPostApplyAccidental()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            ExecuteDb($"EXEC Submit_accidental @employee_ID={myId}, @start_date='{AccidentalStart:yyyy-MM-dd}', @end_date='{AccidentalEnd:yyyy-MM-dd}'");
            TempData["Msg"] = "Accidental Leave Submitted!";
            return RedirectToPage();
        }

        public IActionResult OnPostApplyMedical()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            int insuranceBit = InsuranceStatus ? 1 : 0;
            ExecuteDb($"EXEC Submit_medical @employee_ID={myId}, @start_date='{MedicalStart:yyyy-MM-dd}', @end_date='{MedicalEnd:yyyy-MM-dd}', @medical_type='{MedicalType}', @insurance_status={insuranceBit}, @disability_details='{DisabilityDetails}', @document_description='{MedicalDocDesc}', @file_name='{MedicalFileName}'");
            TempData["Msg"] = "Medical Leave Submitted!";
            return RedirectToPage();
        }

        public IActionResult OnPostApplyUnpaid()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            ExecuteDb($"EXEC Submit_unpaid @employee_ID={myId}, @start_date='{UnpaidStart:yyyy-MM-dd}', @end_date='{UnpaidEnd:yyyy-MM-dd}', @document_description='{UnpaidDocDesc}', @file_name='{UnpaidFileName}'");
            TempData["Msg"] = "Unpaid Leave Submitted!";
            return RedirectToPage();
        }

        public IActionResult OnPostApplyComp()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            ExecuteDb($"EXEC Submit_compensation @employee_ID={myId}, @compensation_date='{CompDate:yyyy-MM-dd}', @reason='{CompReason}', @date_of_original_workday='{CompOriginalDate:yyyy-MM-dd}', @rep_emp_id={CompReplacementId}");
            TempData["Msg"] = "Compensation Leave Submitted!";
            return RedirectToPage();
        }

        public IActionResult OnPostApproveUnpaid()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            ExecuteDb($"EXEC Upperboard_approve_unpaids @request_ID={ManageRequestId}, @upperboard_ID={myId}");
            TempData["Msg"] = "Unpaid Leave Processed (if authorized)";
            return RedirectToPage();
        }

        public IActionResult OnPostApproveAnnual()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            ExecuteDb($"EXEC Upperboard_approve_annual @request_ID={ManageRequestId}, @Upperboard_ID={myId}, @replacement_ID={ManageReplacementId}");
            TempData["Msg"] = "Annual Leave Processed (if authorized)";
            return RedirectToPage();
        }

        public IActionResult OnPostEvaluate()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            ExecuteDb($"EXEC Dean_andHR_Evaluation @employee_ID={EvalEmpId}, @rating={EvalRating}, @comment='{EvalComment}', @semester='{EvalSemester}'");
            TempData["Msg"] = "Evaluation Submitted!";
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        private void ExecuteDb(string sql)
        {
            try { _context.Database.ExecuteSqlRaw(sql); }
            catch { }
        }

        public class LeaveStatusDto
        {
            public int RequestId { get; set; }
            public DateTime DateOfRequest { get; set; }
            public string Status { get; set; }
        }
    }
}