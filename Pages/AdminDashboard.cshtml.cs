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

        public List<AllEmployeeProfile> Employees { get; set; } = new List<AllEmployeeProfile>();
        public List<NoEmployeeDept> DeptCounts { get; set; } = new List<NoEmployeeDept>();
        public List<AllRejectedMedical> RejectedMedicalLeaves { get; set; } = new List<AllRejectedMedical>();

        // We bind these so when user types in the text box, the variable updates automatically.
        [BindProperty]
        public string HolidayName { get; set; }
        
        [BindProperty]
        public DateTime From { get; set; }

        [BindProperty]
        public DateTime To { get; set; }

        [BindProperty]
        public int UpdateEmpId { get; set; }

        [BindProperty]
        public DateTime check_in { get; set; }

        [BindProperty]
        public DateTime check_out { get; set; }
        public IActionResult OnGet()
        {
            // 1. Security Check (Are they an Admin?)
            string role = HttpContext.Session.GetString("user_role");
            if (role != "Admin")
            {
                return RedirectToPage("/Login");
            }

            Employees = _context.AllEmployeeProfiles.ToList();

            DeptCounts = _context.NoEmployeeDepts.ToList();

            RejectedMedicalLeaves = _context.AllRejectedMedicals.ToList();

            //var query = from l in _context.Leaves
            //            join m in _context.MedicalLeaves on l.RequestId equals m.RequestId
            //            where l.FinalApprovalStatus == "rejected"
            //            select new RejectedMedicalInfo
            //            {
            //                RequestId = l.RequestId,
            //                EmployeeId = m.EmpId,
            //                StartDate = l.StartDate.GetValueOrDefault(), // .Value needed if Date is nullable in DB
            //                Status = l.FinalApprovalStatus,
            //                Details = m.DisabilityDetails
            //            };

            //var query = from em in _context.Employees
            //            select new e
            //            {
            //                name = em.FirstName
            //            };

            return Page();
        }

        public IActionResult OnPostAddHoliday()
        {
            _context.Database.ExecuteSqlRaw("EXEC Add_Holiday @holiday_name={0}, @from_date={1}, @to_date={2}",
                HolidayName , From, To);

            TempData["Msg"] = "Holiday Added!";
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveDeductions()
        {
            _context.Database.ExecuteSqlRaw("EXEC Remove_Deductions");
            TempData["Msg"] = "Resigned Deductions Removed!";
            return RedirectToPage();
        }

        public IActionResult OnPostInitAttendance()
        {
            _context.Database.ExecuteSqlRaw("EXEC Initiate_Attendance");

            TempData["Msg"] = "Attendance Initialized for Today!";
            return RedirectToPage();
        }

        public IActionResult OnPostUpdateAttendance()
        {
            
            _context.Database.ExecuteSqlRaw("EXEC Update_Attendance @Employee_id={0}, @check_in_time={1}, @check_out_time={2}",
                UpdateEmpId, check_in, check_out);

            TempData["Msg"] = "Attendance Updated!";
            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}