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

        [BindProperty] public int LeaveRequestId { get; set; }

        [BindProperty] public int EmpId { get; set; } // Used for Deductions & Payroll

        [BindProperty] public DateTime PayStart { get; set; }
        [BindProperty] public DateTime PayEnd { get; set; }


        public IActionResult OnGet()
        {
            // Session access rarely throws, but good practice to keep safe
            try
            {
                var role = HttpContext.Session.GetString("user_role");
                if (role != "HR")
                    return RedirectToPage("/Login");
            }
            catch (Exception)
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }

        // 2) Approve Annual/Accidental
        public IActionResult OnPostApproveAnnualAcc()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").GetValueOrDefault();

                _context.Database.ExecuteSqlRaw("EXEC HR_approval_an_acc @request_ID={0}, @HR_ID={1}",
                    LeaveRequestId, myId);

                TempData["Msg"] = $"Annual/Accidental Request {LeaveRequestId} Processed Successfully.";
            }
            catch (Exception ex)
            {
                // Capture the SQL error message and show it to the user
                TempData["Msg"] = $"Error Processing Request {LeaveRequestId}: {ex.Message}";
            }
            return RedirectToPage();
        }

        // 3) Approve Unpaid
        public IActionResult OnPostApproveUnpaid()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").GetValueOrDefault();

                _context.Database.ExecuteSqlRaw("EXEC HR_approval_unpaid @request_ID={0}, @HR_ID={1}",
                    LeaveRequestId, myId);

                TempData["Msg"] = $"Unpaid Request {LeaveRequestId} Processed Successfully.";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = $"Error Processing Unpaid Request {LeaveRequestId}: {ex.Message}";
            }
            return RedirectToPage();
        }

        // 4) Approve Compensation
        public IActionResult OnPostApproveComp()
        {
            try
            {
                int myId = HttpContext.Session.GetInt32("user_id").GetValueOrDefault();

                _context.Database.ExecuteSqlRaw("EXEC HR_approval_comp @request_ID={0}, @HR_ID={1}",
                    LeaveRequestId, myId);

                TempData["Msg"] = $"Compensation Request {LeaveRequestId} Processed Successfully.";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = $"Error Processing Comp Request {LeaveRequestId}: {ex.Message}";
            }
            return RedirectToPage();
        }

        // 5) Missing Hours
        public IActionResult OnPostAddMissingHours()
        {
            try
            {
                _context.Database.ExecuteSqlRaw("EXEC Deduction_hours @employee_ID={0}", EmpId);
                TempData["Msg"] = $"Missing Hours Deduction Calculated for Emp {EmpId}.";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = $"Error Adding Missing Hours for Emp {EmpId}: {ex.Message}";
            }
            return RedirectToPage();
        }

        // 6) Missing Days
        public IActionResult OnPostAddMissingDays()
        {
            try
            {
                _context.Database.ExecuteSqlRaw("EXEC Deduction_days @employee_id={0}", EmpId);
                TempData["Msg"] = $"Missing Days Deduction Calculated for Emp {EmpId}.";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = $"Error Adding Missing Days for Emp {EmpId}: {ex.Message}";
            }
            return RedirectToPage();
        }

        // 7) Unpaid Leave Deduction
        public IActionResult OnPostAddUnpaidDeduction()
        {
            try
            {
                _context.Database.ExecuteSqlRaw("EXEC Deduction_unpaid @employee_ID={0}", EmpId);
                TempData["Msg"] = $"Unpaid Leave Deduction Calculated for Emp {EmpId}.";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = $"Error Adding Unpaid Deduction for Emp {EmpId}: {ex.Message}";
            }
            return RedirectToPage();
        }

        // 8) Generate Payroll
        public IActionResult OnPostGeneratePayroll()
        {
            try
            {
                _context.Database.ExecuteSqlRaw("EXEC Add_Payroll @employee_ID={0}, @from={1}, @to={2}",
                    EmpId, PayStart, PayEnd);

                TempData["Msg"] = $"Payroll Generated Successfully for Emp {EmpId}.";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = $"Error Generating Payroll for Emp {EmpId}: {ex.Message}";
            }
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}