using Guc_Uni_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Guc_Uni_System.Pages
{
    public class HRDashboardModel : PageModel
    {
        private readonly UniversityHrManagementSystemContext _context;

        public HRDashboardModel(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        // --- INPUT PROPERTIES ---
        // We reuse these properties for different forms to keep code clean
        [BindProperty] public int LeaveRequestId { get; set; }

        [BindProperty] public int EmpId { get; set; } // Used for Deductions & Payroll

        [BindProperty] public DateTime PayStart { get; set; }
        [BindProperty] public DateTime PayEnd { get; set; }


        public IActionResult OnGet()
        {
            // 1. Security Check
            var role = HttpContext.Session.GetString("user_role");
            if (role != "HR")
                return RedirectToPage("/Login");

            return Page();
        }

        // --- LEAVE APPROVAL HANDLERS ---

        // Req #2: Approve Annual/Accidental
        public IActionResult OnPostApproveAnnualAcc()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;

            _context.Database.ExecuteSqlRaw("EXEC HR_approval_an_acc @request_ID={0}, @HR_ID={1}",
                LeaveRequestId, myId);

            TempData["Msg"] = $"Annual/Accidental Request {LeaveRequestId} Processed.";
            return RedirectToPage();
        }

        // Req #3: Approve Unpaid
        public IActionResult OnPostApproveUnpaid()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            _context.Database.ExecuteSqlRaw("EXEC HR_approval_unpaid @request_ID={0}, @HR_ID={1}",
                LeaveRequestId, myId);

            TempData["Msg"] = $"Unpaid Request {LeaveRequestId} Processed.";
            return RedirectToPage();
        }

        // Req #4: Approve Compensation
        public IActionResult OnPostApproveComp()
        {
            int myId = HttpContext.Session.GetInt32("user_id").Value;
            _context.Database.ExecuteSqlRaw("EXEC HR_approval_comp @request_ID={0}, @HR_ID={1}",
                LeaveRequestId, myId);

            TempData["Msg"] = $"Compensation Request {LeaveRequestId} Processed.";
            return RedirectToPage();
        }

        // --- DEDUCTION HANDLERS ---

        // Req #5: Missing Hours
        public IActionResult OnPostAddMissingHours()
        {
            _context.Database.ExecuteSqlRaw("EXEC Deduction_hours @employee_ID={0}", EmpId);
            TempData["Msg"] = $"Missing Hours Deduction Calculated for Emp {EmpId}.";
            return RedirectToPage();
        }

        // Req #6: Missing Days
        public IActionResult OnPostAddMissingDays()
        {
            _context.Database.ExecuteSqlRaw("EXEC Deduction_days @employee_id={0}", EmpId);
            TempData["Msg"] = $"Missing Days Deduction Calculated for Emp {EmpId}.";
            return RedirectToPage();
        }

        // Req #7: Unpaid Leave Deduction
        public IActionResult OnPostAddUnpaidDeduction()
        {
            _context.Database.ExecuteSqlRaw("EXEC Deduction_unpaid @employee_ID={0}", EmpId);
            TempData["Msg"] = $"Unpaid Leave Deduction Calculated for Emp {EmpId}.";
            return RedirectToPage();
        }

        // --- PAYROLL HANDLER ---

        // Req #8: Generate Payroll
        public IActionResult OnPostGeneratePayroll()
        {
            _context.Database.ExecuteSqlRaw("EXEC Add_Payroll @employee_ID={0}, @from={1}, @to={2}",
                EmpId, PayStart, PayEnd);

            TempData["Msg"] = $"Payroll Generated for Emp {EmpId}.";
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}