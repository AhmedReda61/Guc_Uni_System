using Guc_Uni_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static Guc_Uni_System.Pages.AcademicDashboardModel;

namespace Guc_Uni_System.Pages
{
    public class allModel : PageModel
    {
        private readonly UniversityHrManagementSystemContext _context;

        public allModel(UniversityHrManagementSystemContext context)
        {
            _context = context;
        }

        public List<Employee> employees { get; set; } = new List<Employee>();
        public List<NoEmployeeDept> deps { get; set; } = new List<NoEmployeeDept>();
        public List<Role> roles { get; set; } = new List<Role>();
        public List<AnnualLeave> ann_leaves { get; set; } = new List<AnnualLeave>();
        public List<AccidentalLeave> acc_leaves { get; set; } = new List<AccidentalLeave>();
        public List<MedicalLeave> medicals_leaves { get; set; } = new List<MedicalLeave>();
        public List<UnpaidLeave> unpaid_leaves { get; set; } = new List<UnpaidLeave>();
        public List<CompensationLeave> comp_leaves { get; set; } = new List<CompensationLeave>();
        public List<Payroll> payrolls { get; set; } = new List<Payroll>();
        public List<Attendance> attendances { get; set; } = new List<Attendance>();
        public List<Deduction> deductions { get; set; } = new List<Deduction>();
        public List<Performance> performances { get; set; } = new List<Performance>();
        public List<Holiday> holidays { get; set; } = new List<Holiday>();


        public IActionResult OnGet()
        {

            employees = _context.Employees.Include(e => e.RoleNames).ToList();
            deps = _context.NoEmployeeDepts.ToList();
            roles = _context.Roles.ToList();
            ann_leaves = _context.AnnualLeaves
                   .Include(l => l.Request)
                   .ToList();

            acc_leaves = _context.AccidentalLeaves
                .Include(l => l.Request)
                .ToList();

            medicals_leaves = _context.MedicalLeaves
                .Include(l => l.Request)
                .ToList();

            unpaid_leaves = _context.UnpaidLeaves
                .Include(l => l.Request)
                .ToList();

            comp_leaves = _context.CompensationLeaves
                .Include(l => l.Request)
                .ToList();
            payrolls = _context.Payrolls.ToList();
            attendances = _context.Attendances.ToList();
            deductions = _context.Deductions.ToList();
            performances = _context.Performances.ToList();

            holidays = _context.Database.SqlQueryRaw<Holiday>(
                                @"SELECT *
                                  FROM Holiday"
                                ).ToList();


            return Page();
        }
    }

    public class Holiday
    {
        public int holiday_ID { get; set; }
        public string holiday_name { get; set; }
        public DateOnly? from_date { get; set; }
        public DateOnly? to_date { get; set; }
    }
}
