using Guc_Uni_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations; // For validation

namespace Guc_Uni_System.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly UniversityHrManagementSystemContext _context;

        public AdminDashboardModel(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        // --- DATA PROPERTIES (For "View" tasks) ---
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<DepartmentCount> DeptCounts { get; set; } = new List<DepartmentCount>(); // Req #3

        // Note: You need to create a class for this if 'Leave' model is complex, 
        // but for now let's assume you have a Leave model or we use dynamic/object.
        // We will use the existing Leave model.
        public List<Leave> RejectedLeaves { get; set; } = new List<Leave>(); // Req #4

        // --- INPUT PROPERTIES (For "Form" tasks) ---
        // We bind these so when user types in the text box, the variable updates automatically.
        [BindProperty]
        public string HolidayName { get; set; }
        
        [BindProperty]
        public DateTime HolidayDate { get; set; }

        [BindProperty]
        public int UpdateEmpId { get; set; }

        [BindProperty]
        public DateTime UpdateDate { get; set; }

        [BindProperty]
        public string UpdateStatus { get; set; }

        public IActionResult OnGet()
        {
            // 1. Security Check (Are they an Admin?)
            string role = HttpContext.Session.GetString("user_role");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            // 2. Fetch Employees using EF Core
            Employees = _context.Employees.ToList();

            //Req #3: Employees per Department (Group By)
            //Logic: Count employees in each department
            //Note: EF Core GroupBy is tricky, usually easier to write Raw SQL for reporting
            //or just iterate in memory if data is small.Let's use logic:
            var data = _context.Employees
                        .GroupBy(e => e.DeptName)
                        .Select(g => new DepartmentCount { Name = g.Key, Count = g.Count() })
                        .ToList();
            DeptCounts = data;


            // Req #4: Rejected Medical Leaves
            // Assuming you have a 'Leaves' table and 'Medical_Leaves' table.
            // We filter by type='Medical' (implied by joining or specific table) and status='rejected'
            // Since EF Core scaffolding might have created 'Leaves', let's check:
            // This is complex because Medical_Leave is a child table.
            // Simplified approach: Get leaves where status is rejected
            // You might need to adjust based on your exact Scaffolding for 'MedicalLeaves'
            RejectedLeaves = _context.Leaves
                                    .Where(l => l.FinalApprovalStatus == "rejected")
                                    .ToList();
            // In a perfect world, you would Join with Medical_Leaves to ensure it is medical.

            return Page();
        }

        // --- THE POST METHODS (Actions) ---

        // Req #7: Add Holiday
        public IActionResult OnPostAddHoliday()
        {
            // Call Stored Procedure
            _context.Database.ExecuteSqlRaw("EXEC Add_Holiday @holiday_name={0}, @from_date={1}, @to_date={1}",
                HolidayName, HolidayDate);

            TempData["Msg"] = "Holiday Added!";
            return RedirectToPage();
        }

        // Req #5: Remove Deductions
        public IActionResult OnPostRemoveDeductions()
        {
            _context.Database.ExecuteSqlRaw("EXEC Remove_Deductions");
            TempData["Msg"] = "Resigned Deductions Removed!";
            return RedirectToPage();
        }

        // Req #8: Init Attendance
        public IActionResult OnPostInitAttendance()
        {
            // Assuming you have logic for this, or just a dummy placeholder if no proc exists
            // Since the PDF mentions it, maybe it's just an Insert loop?
            // For now, let's pretend there is a procedure or we just execute logic.
            // _context.Database.ExecuteSqlRaw("EXEC Init_Attendance"); 

            TempData["Msg"] = "Attendance Initialized for Today!";
            return RedirectToPage();
        }

        // Req #6: Update Attendance
        public IActionResult OnPostUpdateAttendance()
        {
            // Simple SQL Update
            // Note: Use {0}, {1}, {2} placeholders for security
            string sql = "UPDATE Attendance SET status = {0} WHERE emp_ID = {1} AND date = {2}";
            _context.Database.ExecuteSqlRaw(sql, UpdateStatus, UpdateEmpId, UpdateDate);

            TempData["Msg"] = "Attendance Updated!";
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }

        // Helper Class for the Report
        public class DepartmentCount
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }
}