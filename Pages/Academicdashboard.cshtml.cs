using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;

// NOTE: You must replace 'YourAppNamespace' with the actual namespace of your Razor Pages project
namespace YourAppNamespace.Pages.Academic
{
    // --- DATA MODELS (Matching Database Schema) ---
    // These models represent the C# objects used to hold data retrieved from your SQL database.

    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DepartmentName { get; set; }
        public string OfficialDayOff { get; set; }
        public int AnnualBalance { get; set; }
        public int AccidentalBalance { get; set; }
        public decimal Salary { get; set; }
    }

    public class PerformanceRecord
    {
        public string Semester { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }
    }

    public class AttendanceRecord
    {
        public DateTime Date { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public TimeSpan TotalDuration { get; set; } // calculated: check_out_time - check_in_time
    }

    public class PayrollRecord
    {
        public DateTime PaymentDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal DeductionsAmount { get; set; }
        public decimal FinalSalaryAmount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class DeductionRecord
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public int AttendanceID { get; set; }
    }

    public class LeaveStatus
    {
        public int RequestId { get; set; }
        public string LeaveType { get; set; } // Annual or Accidental
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumDays { get; set; }
        public string FinalApprovalStatus { get; set; }
    }

    // --- MOCK DATA ACCESS LAYER (TO BE REPLACED) ---
    // This class simulates the interaction with your SQL database.

    public static class DataAccess
    {
        private static readonly List<Employee> MockEmployees = new List<Employee>
        {
            new Employee { EmployeeId = 101, FirstName = "Jane", LastName = "Doe", AnnualBalance = 20, AccidentalBalance = 6, DepartmentName = "Media Engineering" }
        };

        // Requirement 1 / Summary
        public static Employee GetEmployeeDetails(int employeeId)
        {
            // REPLACE with: SQL query to get Employee data by ID
            return MockEmployees.FirstOrDefault(e => e.EmployeeId == employeeId);
        }

        // Requirement 2: Retrieve Performance
        public static List<PerformanceRecord> GetPerformanceRecords(int employeeId, string semesterFilter)
        {
            // REPLACE with: SQL query to join Employee and Performance tables, filtering by emp_ID and semester
            var records = new List<PerformanceRecord>
            {
                new PerformanceRecord { Semester = "W25", Rating = 5, Comments = "Outstanding research and teaching." },
                new PerformanceRecord { Semester = "S24", Rating = 4, Comments = "Excellent student feedback." },
                new PerformanceRecord { Semester = "W24", Rating = 3, Comments = "Meets expectations. Committee work needed." }
            }.Where(r => employeeId == 101).ToList();

            if (!string.IsNullOrEmpty(semesterFilter))
            {
                records = records.Where(r => r.Semester.Equals(semesterFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return records;
        }

        // Requirement 3: Retrieve Attendance
        public static List<AttendanceRecord> GetAttendanceRecords(int employeeId, DateTime monthStart, DateTime monthEnd)
        {
            // REPLACE with: SQL query to get Attendance records for the month, excluding the employee's official_day_off.
            var today = DateTime.Today;
            var records = new List<AttendanceRecord>
            {
                new AttendanceRecord { Date = today.AddDays(-5), CheckInTime = new TimeSpan(9, 0, 0), CheckOutTime = new TimeSpan(17, 0, 0), TotalDuration = new TimeSpan(8, 0, 0) },
                new AttendanceRecord { Date = today.AddDays(-4), CheckInTime = new TimeSpan(9, 15, 0), CheckOutTime = new TimeSpan(17, 30, 0), TotalDuration = new TimeSpan(8, 15, 0) },
                new AttendanceRecord { Date = today.AddDays(-3), CheckInTime = new TimeSpan(10, 0, 0), CheckOutTime = new TimeSpan(18, 0, 0), TotalDuration = new TimeSpan(8, 0, 0) },
            }.Where(r => employeeId == 101).ToList();

            return records;
        }

        // Requirement 4: Retrieve Payroll
        public static PayrollRecord GetLastMonthPayroll(int employeeId)
        {
            // REPLACE with: SQL query to get the last Payroll record for the employee
            return new PayrollRecord
            {
                PaymentDate = DateTime.Today.AddDays(-1),
                BaseSalary = 15000.00m,
                BonusAmount = 500.00m,
                DeductionsAmount = 150.00m,
                FinalSalaryAmount = 15350.00m,
                FromDate = DateTime.Today.AddMonths(-1).Date,
                ToDate = DateTime.Today.AddDays(-30).Date
            };
        }

        // Requirement 5: Fetch Deductions
        public static List<DeductionRecord> GetDeductionsByAttendance(int employeeId, DateTime startDate, DateTime endDate)
        {
            // REPLACE with: SQL query to join Deduction and Attendance tables, filtering by emp_ID and date range
            var records = new List<DeductionRecord>
            {
                new DeductionRecord { Date = new DateTime(2025, 11, 10), Type = "Late Check-In", Amount = 50.00m, AttendanceID = 451 },
                new DeductionRecord { Date = new DateTime(2025, 11, 15), Type = "Missing Hours", Amount = 100.00m, AttendanceID = 460 }
            }.Where(r => employeeId == 101).ToList();

            return records.Where(r => r.Date >= startDate && r.Date <= endDate).ToList();
        }

        // Requirement 6: Apply for Annual Leave
        public static void SubmitAnnualLeave(int employeeId, DateTime startDate, DateTime endDate, int replacementEmpId)
        {
            // REPLACE with: SQL Stored Procedure call to check balance and insert into Leave and Annual_Leave tables.
            if (employeeId == replacementEmpId)
            {
                throw new Exception("Replacement Employee ID cannot be your own ID.");
            }
            // Logic to insert the request goes here...
            System.Diagnostics.Debug.WriteLine($"Annual Leave Request from {employeeId} submitted.");
        }

        // Requirement 7: Retrieve Leave Status
        public static List<LeaveStatus> GetLeaveStatuses(int employeeId, DateTime monthStart, DateTime monthEnd)
        {
            // REPLACE with: SQL query to get status of Annual and Accidental leaves for the employee during the current month
            var records = new List<LeaveStatus>
            {
                new LeaveStatus { RequestId = 201, LeaveType = "Annual", StartDate = DateTime.Today.AddDays(15), EndDate = DateTime.Today.AddDays(18), NumDays = 4, FinalApprovalStatus = "Pending" },
                new LeaveStatus { RequestId = 202, LeaveType = "Accidental", StartDate = DateTime.Today.AddDays(5), EndDate = DateTime.Today.AddDays(5), NumDays = 1, FinalApprovalStatus = "Approved" }
            }.Where(r => employeeId == 101).ToList();

            return records.Where(r => r.StartDate >= monthStart && r.EndDate <= monthEnd).ToList();
        }
    }


    // --- ACADEMIC DASHBOARD PAGE MODEL ---

    public class AcademicDashboardModel : PageModel
    {
        // Properties for all data displayed on the dashboard
        [BindProperty] // Required for summary display and POST requests
        public Employee EmployeeDetails { get; set; }

        public List<PerformanceRecord> PerformanceRecords { get; set; }
        public List<AttendanceRecord> AttendanceRecords { get; set; }
        public PayrollRecord LastPayroll { get; set; }
        public List<DeductionRecord> Deductions { get; set; }
        public List<LeaveStatus> LeaveStatuses { get; set; }

        // Input properties for filtering and submission (Req 2, 5, 6)
        [BindProperty(SupportsGet = true)]
        public string SelectedSemester { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DeductionStartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DeductionEndDate { get; set; }

        // Leave Request Inputs (Req 6)
        [BindProperty]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(7);

        [BindProperty]
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(10);

        [BindProperty]
        public int ReplacementEmpId { get; set; }

        // Status Messages
        [TempData]
        public string StatusMessage { get; set; }
        [TempData]
        public bool IsSuccess { get; set; }

        private int? CurrentEmployeeId => HttpContext.Session.GetInt32("EmployeeId");

        // Main GET handler for page loading and data fetching (Req 1-5, 7)
        public IActionResult OnGet(string tab)
        {
            if (CurrentEmployeeId == null)
            {
                // Must redirect if not authenticated (Req 1: Login)
                return RedirectToPage("/Login");
            }

            // Always load employee details for the summary view and leave balances (Req 1)
            EmployeeDetails = DataAccess.GetEmployeeDetails(CurrentEmployeeId.Value);

            if (EmployeeDetails == null)
            {
                StatusMessage = "Error: Employee details could not be loaded.";
                return Page();
            }

            // Load data based on the active tab (which is passed as a query string)
            switch (tab?.ToLowerInvariant())
            {
                case "performance":     // Req 2
                    LoadPerformanceRecords();
                    break;
                case "attendance":      // Req 3
                    LoadAttendanceRecords();
                    break;
                case "payroll":         // Req 4
                    LoadLastPayroll();
                    break;
                case "deductions":      // Req 5
                    LoadDeductionRecords();
                    break;
                case "leavestatus":     // Req 7
                    LoadLeaveStatuses();
                    break;
                    // 'summary' and 'annualleave' tabs don't require complex GET data loading beyond employee details.
            }

            return Page();
        }

        // POST handler for Annual Leave Submission (Req 6)
        public IActionResult OnPostSubmitAnnualLeave()
        {
            if (CurrentEmployeeId == null) return RedirectToPage("/Login");

            // Redirect back to the annual leave tab after submission/error
            var redirectPage = RedirectToPage("/Academic/AcademicDashboard", new { tab = "annualleave" });

            if (!ModelState.IsValid)
            {
                StatusMessage = "Error: Please check the form data. All fields are required.";
                IsSuccess = false;
                return redirectPage;
            }

            if (StartDate > EndDate || StartDate < DateTime.Today)
            {
                StatusMessage = "Error: Start date must be today or in the future, and cannot be after the end date.";
                IsSuccess = false;
                return redirectPage;
            }

            try
            {
                // Call the mock DB method (replace with your SP execution)
                DataAccess.SubmitAnnualLeave(
                    CurrentEmployeeId.Value,
                    StartDate,
                    EndDate,
                    ReplacementEmpId
                );

                IsSuccess = true;
                StatusMessage = $"Success! Annual Leave request submitted for {StartDate:d} to {EndDate:d}.";
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                StatusMessage = $"Submission failed: {ex.Message}";
            }

            return redirectPage;
        }

        // --- Data Loading Helper Methods ---

        private void LoadPerformanceRecords()
        {
            // Req 2: Retrieve performance, using the optional SelectedSemester filter
            PerformanceRecords = DataAccess.GetPerformanceRecords(CurrentEmployeeId.Value, SelectedSemester);
            if (PerformanceRecords == null || PerformanceRecords.Count == 0)
            {
                StatusMessage = "No performance records found.";
            }
        }

        private void LoadAttendanceRecords()
        {
            // Req 3: Retrieve attendance records for the current month
            DateTime monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

            AttendanceRecords = DataAccess.GetAttendanceRecords(CurrentEmployeeId.Value, monthStart, monthEnd);
            if (AttendanceRecords == null || AttendanceRecords.Count == 0)
            {
                StatusMessage = "No attendance records available for the current month.";
            }
        }

        private void LoadLastPayroll()
        {
            // Req 4: Retrieve last month's payroll details
            LastPayroll = DataAccess.GetLastMonthPayroll(CurrentEmployeeId.Value);
            if (LastPayroll == null)
            {
                StatusMessage = "Last month's payroll is not yet processed or retrieved.";
            }
        }

        private void LoadDeductionRecords()
        {
            // Req 5: Fetch deductions based on the date range from the query string
            if (DeductionStartDate.HasValue && DeductionEndDate.HasValue)
            {
                Deductions = DataAccess.GetDeductionsByAttendance(CurrentEmployeeId.Value, DeductionStartDate.Value, DeductionEndDate.Value);
            }
            else
            {
                // Default to last 30 days if no filter applied on initial load
                Deductions = DataAccess.GetDeductionsByAttendance(CurrentEmployeeId.Value, DateTime.Today.AddDays(-30), DateTime.Today);
            }

            if (Deductions == null || Deductions.Count == 0)
            {
                StatusMessage = "No attendance deductions found for the selected period.";
            }
        }

        private void LoadLeaveStatuses()
        {
            // Req 7: Retrieve leave status for the current month
            DateTime monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

            LeaveStatuses = DataAccess.GetLeaveStatuses(CurrentEmployeeId.Value, monthStart, monthEnd);

            if (LeaveStatuses == null || LeaveStatuses.Count == 0)
            {
                StatusMessage = "No leave requests found for the current month.";
            }
        }
    }
}
