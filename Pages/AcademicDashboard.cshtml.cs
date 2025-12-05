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

        public bool IsDean { get; set; }
        public bool IsViceDean { get; set; }
        public bool IsPresident { get; set; }
        public bool IsVicePresident { get; set; }
        public bool IsHeadOfDept { get; set; }

        public List<Performance> MyPerformances { get; set; } = new List<Performance>();
        public List<Attendance> MyAttendances { get; set; } = new List<Attendance>();
        public List<Payroll> MyPayrolls { get; set; } = new List<Payroll>();
        public List<Deduction> MyDeductions { get; set; } = new List<Deduction>();
        public List<LeaveStatusDto> MyLeavesStatus { get; set; } = new List<LeaveStatusDto>();

        [BindProperty(SupportsGet = true)]
        public string TargetSemester { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TargetMonth { get; set; } = DateTime.Now.Month;

        // Annual Leave
        [BindProperty] public DateTime AnnualStart { get; set; }
        [BindProperty] public DateTime AnnualEnd { get; set; }
        [BindProperty] public int ReplacementId { get; set; }

        // Accidental Leave
        [BindProperty] public DateTime AccidentalStart { get; set; }
        [BindProperty] public DateTime AccidentalEnd { get; set; }

        // Medical Leave
        [BindProperty] public DateTime MedicalStart { get; set; }
        [BindProperty] public DateTime MedicalEnd { get; set; }
        [BindProperty] public string MedicalType { get; set; }
        [BindProperty] public bool InsuranceStatus { get; set; }
        [BindProperty] public string DisabilityDetails { get; set; }
        [BindProperty] public string MedicalDocDesc { get; set; }
        [BindProperty] public string MedicalFileName { get; set; }

        // Unpaid Leave
        [BindProperty] public DateTime UnpaidStart { get; set; }
        [BindProperty] public DateTime UnpaidEnd { get; set; }
        [BindProperty] public string UnpaidDocDesc { get; set; }
        [BindProperty] public string UnpaidFileName { get; set; }

        // Compensation Leave
        [BindProperty] public DateTime CompDate { get; set; }
        [BindProperty] public DateTime CompOriginalDate { get; set; }
        [BindProperty] public string CompReason { get; set; }
        [BindProperty] public int CompReplacementId { get; set; }

        // Management (Dean/Prez) Inputs
        [BindProperty] public int ManageRequestId { get; set; }
        [BindProperty] public int ManageReplacementId { get; set; }
        [BindProperty] public int EvalEmpId { get; set; }
        [BindProperty] public int EvalRating { get; set; }
        [BindProperty] public string EvalComment { get; set; }
        [BindProperty] public string EvalSemester { get; set; }


        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "Academic")
                return RedirectToPage("/Login");

            int myId = HttpContext.Session.GetInt32("user_id").GetValueOrDefault();

            // 1.1) Detect Roles
            try
            {
                var employee = _context.Employees
                    .Include(e => e.RoleNames)
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
            }
            catch (Exception ex)
            {
                // Log error safely without crashing
                Console.WriteLine("Error fetching roles: " + ex.Message);
            }

            // 1.2) Performance
            try
            {
                if (!string.IsNullOrEmpty(TargetSemester))
                {
                    MyPerformances = _context.Performances
                        .FromSqlRaw("SELECT * FROM dbo.MyPerformance({0}, {1})", myId, TargetSemester)
                        .ToList();
                }
            }
            catch { }

            // 1.3) My Attendance
            try
            {
                MyAttendances = _context.Attendances
                    .FromSqlRaw("SELECT * FROM dbo.MyAttendance({0})", myId)
                    .ToList();
            }
            catch { }

            // 1.4) Last Month Payroll
            try
            {
                MyPayrolls = _context.Payrolls
                    .FromSqlRaw("SELECT * FROM dbo.Last_month_payroll({0})", myId)
                    .ToList();
            }
            catch { }

            // 1.5) Deductions
            try
            {
                MyDeductions = _context.Deductions
                    .FromSqlRaw("SELECT * FROM dbo.Deductions_Attendance({0}, {1})", myId, TargetMonth)
                    .ToList();
            }
            catch { }

            // 1.7) Leave Status
            try
            {
                var leavesData = _context.Database.SqlQueryRaw<LeaveStatusDto>(
                                @"SELECT 
                                    request_ID AS RequestId, 
                                    date_of_request AS DateOfRequest, 
                                    final_approval_status AS Status 
                                  FROM dbo.status_leaves({0})",
                                myId).ToList();
                MyLeavesStatus = leavesData;
            }
            catch { }

            return Page();
        }

        // 1.6) Annual Leave
        public IActionResult OnPostApplyAnnual()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").Value;
                _context.Database.ExecuteSqlRaw("EXEC Submit_annual @employee_ID={0}, @replacement_emp={1}, @start_date={2}, @end_date={3}",
                    myId, ReplacementId, AnnualStart, AnnualEnd);

                TempData["Msg"] = "Annual Leave Submitted Successfully!";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToPage();
        }

        // 2.1) Accidental Leave
        public IActionResult OnPostApplyAccidental()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").Value;
                _context.Database.ExecuteSqlRaw("EXEC Submit_accidental @employee_ID={0}, @start_date={1}, @end_date={2}",
                    myId, AccidentalStart, AccidentalEnd);

                TempData["Msg"] = "Accidental Leave Submitted Successfully!";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToPage();
        }

        // 2.2) Medical Leave
        public IActionResult OnPostApplyMedical()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").Value;
                _context.Database.ExecuteSqlRaw("EXEC Submit_medical @employee_ID={0}, @start_date={1}, @end_date={2}, @medical_type={3}, @insurance_status={4}, @disability_details={5}, @document_description={6}, @file_name={7}",
                    myId, MedicalStart, MedicalEnd, MedicalType, InsuranceStatus, DisabilityDetails, MedicalDocDesc, MedicalFileName);

                TempData["Msg"] = "Medical Leave Submitted Successfully!";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToPage();
        }

        // 2.3) Unpaid Leave
        public IActionResult OnPostApplyUnpaid()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").Value;
                _context.Database.ExecuteSqlRaw("EXEC Submit_unpaid @employee_ID={0}, @start_date={1}, @end_date={2}, @document_description={3}, @file_name={4}",
                    myId, UnpaidStart, UnpaidEnd, UnpaidDocDesc, UnpaidFileName);

                TempData["Msg"] = "Unpaid Leave Submitted Successfully!";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToPage();
        }

        // 2.4) Compensation Leave
        public IActionResult OnPostApplyComp()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").Value;
                _context.Database.ExecuteSqlRaw("EXEC Submit_compensation @employee_ID={0}, @compensation_date={1}, @reason={2}, @date_of_original_workday={3}, @rep_emp_id={4}",
                    myId, CompDate, CompReason, CompOriginalDate, CompReplacementId);

                TempData["Msg"] = "Compensation Leave Submitted Successfully!";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToPage();
        }

        // 2.5) Approve Unpaid (Dean/Prez)
        public IActionResult OnPostApproveUnpaid()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").Value;
                _context.Database.ExecuteSqlRaw("EXEC Upperboard_approve_unpaids @request_ID={0}, @upperboard_ID={1}",
                    ManageRequestId, myId);

                TempData["Msg"] = "Unpaid Leave Processed.";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToPage();
        }

        // 2.6) Approve Annual (Dean/Prez)
        public IActionResult OnPostApproveAnnual()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").Value;
                _context.Database.ExecuteSqlRaw("EXEC Upperboard_approve_annual @request_ID={0}, @Upperboard_ID={1}, @replacement_ID={2}",
                    ManageRequestId, myId, ManageReplacementId);

                TempData["Msg"] = "Annual Leave Processed.";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToPage();
        }

        // 2.7) Evaluation (Dean)
        public IActionResult OnPostEvaluate()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").Value;
                _context.Database.ExecuteSqlRaw("EXEC Dean_andHR_Evaluation @employee_ID={0}, @rating={1}, @comment={2}, @semester={3}",
                    EvalEmpId, EvalRating, EvalComment, EvalSemester);

                TempData["Msg"] = "Evaluation Submitted Successfully!";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Error: " + ex.Message;
            }
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        public class LeaveStatusDto
        {
            public int RequestId { get; set; }
            public DateOnly DateOfRequest { get; set; }
            public string Status { get; set; }
        }
    }
}