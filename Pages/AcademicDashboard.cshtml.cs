using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;

// NOTE: Updated the namespace to Guc_Uni_System
namespace Guc_Uni_System.Pages.Academic
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
        public TimeSpan TotalDuration { get; set; } // calculated: check_out_time - check_in
    }

    public class Payroll
    {
        public int PayrollId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal FinalSalaryAmount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal BonusAmount { get; set; }
        public decimal DeductionsAmount { get; set; }
    }

    public class Deduction
    {
        public int DeductionID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public int? AttendanceID { get; set; }
    }

    public class LeaveStatus
    {
        public int RequestId { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumDays { get; set; }
        public string FinalApprovalStatus { get; set; }
    }

    // --- DATA ACCESS LAYER MOCKUP (Replace with actual DB access) ---
    // In a real application, this class would contain methods to connect to the SQL database.
    public static class DataAccess
    {
        public static Employee GetEmployeeDetails(int employeeId)
        {
            // Mock Data for Employee Details (Req 1)
            return new Employee
            {
                EmployeeId = employeeId,
                FirstName = "Omar",
                LastName = "Ali",
                Email = "omar.ali@guc.edu.eg",
                DepartmentName = "Media Engineering",
                OfficialDayOff = "Friday",
                AnnualBalance = 21,
                AccidentalBalance = 5,
                Salary = 15000.00m
            };
        }

        public static List<PerformanceRecord> GetPerformanceRecords(int employeeId, string semesterFilter)
        {
            // Mock Data for Performance (Req 2)
            var records = new List<PerformanceRecord>
            {
                new PerformanceRecord { Semester = "W25", Rating = 5, Comments = "Outstanding performance and commitment to research." },
                new PerformanceRecord { Semester = "S24", Rating = 4, Comments = "Exceeded expectations in teaching quality." },
                new PerformanceRecord { Semester = "W24", Rating = 3, Comments = "Met all primary responsibilities." }
            };

            if (string.IsNullOrWhiteSpace(semesterFilter))
            {
                return records;
            }
            return records.Where(r => r.Semester.Equals(semesterFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public static List<AttendanceRecord> GetAttendanceRecords(int employeeId, DateTime monthStart)
        {
            // Mock Data for Attendance (Req 3)
            return new List<AttendanceRecord>
            {
                new AttendanceRecord { Date = monthStart.AddDays(1), CheckInTime = new TimeSpan(9, 0, 0), CheckOutTime = new TimeSpan(17, 0, 0), TotalDuration = new TimeSpan(8, 0, 0) },
                new AttendanceRecord { Date = monthStart.AddDays(2), CheckInTime = new TimeSpan(9, 15, 0), CheckOutTime = new TimeSpan(16, 45, 0), TotalDuration = new TimeSpan(7, 30, 0) },
                new AttendanceRecord { Date = monthStart.AddDays(3), CheckInTime = new TimeSpan(8, 45, 0), CheckOutTime = new TimeSpan(17, 15, 0), TotalDuration = new TimeSpan(8, 30, 0) }
            };
        }

        public static Payroll GetLastPayroll(int employeeId)
        {
            // Mock Data for Last Payroll (Req 4)
            return new Payroll
            {
                PayrollId = 101,
                PaymentDate = DateTime.Today.AddDays(-5),
                FromDate = DateTime.Today.AddMonths(-1).AddDays(-5),
                ToDate = DateTime.Today.AddDays(-5),
                BaseSalary = 12000.00m,
                BonusAmount = 3000.00m,
                DeductionsAmount = 500.00m,
                FinalSalaryAmount = 14500.00m // 12000 + 3000 - 500
            };
        }

        public static List<Deduction> GetDeductionsByAttendance(int employeeId, DateTime startDate, DateTime endDate)
        {
            // Mock Data for Deductions (Req 5)
            var allDeductions = new List<Deduction>
            {
                new Deduction { DeductionID = 1, Date = new DateTime(2025, 11, 10), Amount = 250.00m, Type = "Late Check-In", AttendanceID = 456 },
                new Deduction { DeductionID = 2, Date = new DateTime(2025, 11, 15), Amount = 500.00m, Type = "Missing Hours", AttendanceID = 789 },
                new Deduction { DeductionID = 3, Date = new DateTime(2025, 12, 1), Amount = 100.00m, Type = "Early Check-Out", AttendanceID = 1011 }
            };

            return allDeductions
                .Where(d => d.Date >= startDate && d.Date <= endDate)
                .ToList();
        }

        public static bool SubmitAnnualLeave(int employeeId, DateTime startDate, DateTime endDate, int replacementEmpId)
        {
            // Mock implementation for Req 6
            // In a real application, this would insert a request into the Leave_Request table.
            if (replacementEmpId == 9999) return false; // Mock failure for demonstration
            return true; // Mock success
        }

        public static List<LeaveStatus> GetLeaveStatuses(int employeeId, DateTime monthStart, DateTime monthEnd)
        {
            // Mock Data for Leave Status (Req 7)
            var allStatuses = new List<LeaveStatus>
            {
                new LeaveStatus { RequestId = 1, LeaveType = "Annual", StartDate = new DateTime(2025, 12, 10), EndDate = new DateTime(2025, 12, 12), NumDays = 3, FinalApprovalStatus = "Pending HR" },
                new LeaveStatus { RequestId = 2, LeaveType = "Accidental", StartDate = new DateTime(2025, 12, 20), EndDate = new DateTime(2025, 12, 20), NumDays = 1, FinalApprovalStatus = "Approved" },
                new LeaveStatus { RequestId = 3, LeaveType = "Unpaid", StartDate = new DateTime(2025, 11, 25), EndDate = new DateTime(2025, 11, 28), NumDays = 4, FinalApprovalStatus = "Rejected" }
            };

            return allStatuses
                .Where(s => s.StartDate >= monthStart && s.EndDate <= monthEnd)
                .ToList();
        }
    }

    // --- RAZOR PAGE MODEL ---

    public class AcademicDashboardModel : PageModel
    {
        // Static Employee ID for demonstration purposes (replace with actual authentication)
        private const int CurrentEmployeeIdMock = 1001;

        // Public properties to hold the data for the view
        public Employee EmployeeDetails { get; set; }
        public List<PerformanceRecord> PerformanceRecords { get; set; }
        public List<AttendanceRecord> AttendanceRecords { get; set; }
        public Payroll LastPayroll { get; set; }
        public List<Deduction> Deductions { get; set; }
        public List<LeaveStatus> LeaveStatuses { get; set; }


        // Properties for user input/state
        [BindProperty(SupportsGet = true)]
        public string SelectedSemester { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DeductionStartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? DeductionEndDate { get; set; }

        [BindProperty] // Required for POST form submission (Req 6)
        public DateTime StartDate { get; set; }

        [BindProperty] // Required for POST form submission (Req 6)
        public DateTime EndDate { get; set; }

        [BindProperty] // Required for POST form submission (Req 6)
        public int ReplacementEmpId { get; set; }

        // General message properties for the view
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ActiveTab { get; set; } = "summary"; // Default tab

        public bool IsSuccess { get; set; } = true;

        // Helper to get the employee ID
        private int? CurrentEmployeeId => CurrentEmployeeIdMock;

        // --- HANDLERS ---

        /// <summary>
        /// Handles GET requests. Determines the active tab and loads the corresponding data.
        /// </summary>
        public IActionResult OnGet()
        {
            if (CurrentEmployeeId == null)
            {
                // Redirect to login if not authenticated
                return RedirectToPage("/Login");
            }

            // Set active tab from QueryString if available. It defaults to "summary" if not provided.
            ActiveTab = HttpContext.Request.Query["tab"].ToString() ?? "summary";

            // Load required data based on the active tab and filters
            LoadEmployeeDetails();

            switch (ActiveTab)
            {
                case "performance":
                    // SelectedSemester is automatically bound via [BindProperty(SupportsGet = true)]
                    LoadPerformanceRecords();
                    break;
                case "attendance":
                    LoadAttendanceRecords();
                    break;
                case "payroll":
                    LoadLastPayroll();
                    break;
                case "deductions":
                    LoadDeductionRecords();
                    break;
                case "leavestatus":
                    LoadLeaveStatuses();
                    break;
                    // "summary" and "annualleave" only require EmployeeDetails, which is already loaded.
            }

            return Page();
        }

        /// <summary>
        /// Handles POST requests for submitting an Annual Leave request (Req 6).
        /// </summary>
        public IActionResult OnPostSubmitAnnualLeave()
        {
            // Ensure employee details are loaded for balance check and display
            LoadEmployeeDetails();
            ActiveTab = "annualleave"; // Keep the user on the leave application tab

            if (!ModelState.IsValid)
            {
                StatusMessage = "Error: Please check the form inputs and try again.";
                IsSuccess = false;
                return Page();
            }

            // Validation checks
            if (StartDate > EndDate)
            {
                StatusMessage = "Error: Start Date must be before or the same as End Date.";
                IsSuccess = false;
                return Page();
            }

            // Calculate number of days requested (inclusive)
            var numDays = (EndDate - StartDate).Days + 1;

            if (numDays > 14)
            {
                StatusMessage = $"Error: Annual leave requests are limited to a maximum of 14 consecutive days. You requested {numDays} days.";
                IsSuccess = false;
                return Page();
            }

            if (numDays > EmployeeDetails.AnnualBalance)
            {
                StatusMessage = $"Error: You only have {EmployeeDetails.AnnualBalance} annual leave days remaining. You requested {numDays} days.";
                IsSuccess = false;
                return Page();
            }

            // Submit the request (Req 6 - using mock data access)
            bool success = DataAccess.SubmitAnnualLeave(CurrentEmployeeId.Value, StartDate, EndDate, ReplacementEmpId);

            if (success)
            {
                StatusMessage = $"Success: Annual Leave request ({numDays} days) submitted successfully for approval.";
                IsSuccess = true;
                // Redirect to leave status tab to show the new request status
                return RedirectToPage(new { tab = "leavestatus" });
            }
            else
            {
                StatusMessage = "Error: Failed to submit the annual leave request. Please check the Replacement Employee ID or contact HR.";
                IsSuccess = false;
                return Page();
            }
        }

        // --- HELPER METHODS FOR DATA LOADING ---

        private void LoadEmployeeDetails()
        {
            // Req 1: Retrieve basic employee details and balances
            EmployeeDetails = DataAccess.GetEmployeeDetails(CurrentEmployeeId.Value);

            if (EmployeeDetails == null)
            {
                StatusMessage = "Error: Employee details could not be loaded.";
                IsSuccess = false;
            }
        }

        private void LoadPerformanceRecords()
        {
            // Req 2: Retrieve performance records, optionally filtered by semester
            PerformanceRecords = DataAccess.GetPerformanceRecords(CurrentEmployeeId.Value, SelectedSemester);
        }

        private void LoadAttendanceRecords()
        {
            // Req 3: Retrieve attendance records for the current month
            DateTime monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            AttendanceRecords = DataAccess.GetAttendanceRecords(CurrentEmployeeId.Value, monthStart);
        }

        private void LoadLastPayroll()
        {
            // Req 4: Retrieve last month's payroll details
            LastPayroll = DataAccess.GetLastPayroll(CurrentEmployeeId.Value);
        }

        private void LoadDeductionRecords()
        {
            // Req 5: Set default date range if not provided via query string
            if (!DeductionStartDate.HasValue)
            {
                DeductionStartDate = DateTime.Today.AddDays(-30);
            }
            if (!DeductionEndDate.HasValue)
            {
                DeductionEndDate = DateTime.Today;
            }

            // Fetch deductions using the determined date range
            Deductions = DataAccess.GetDeductionsByAttendance(CurrentEmployeeId.Value, DeductionStartDate.Value, DeductionEndDate.Value);
        }

        private void LoadLeaveStatuses()
        {
            // Req 7: Retrieve leave status for the current month
            DateTime monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

            LeaveStatuses = DataAccess.GetLeaveStatuses(CurrentEmployeeId.Value, monthStart, monthEnd);
        }

        /// <summary>
        /// Helper method to determine if a tab is active for CSS styling in the Razor View.
        /// </summary>
        /// <param name="tabName">The name of the tab to check (e.g., "summary").</param>
        /// <returns>The string "active" if the tab matches ActiveTab, otherwise an empty string.</returns>
        public string GetActiveClass(string tabName)
        {
            // The ActiveTab property is set in OnGet() from the query string or defaults to "summary".
            return ActiveTab.Equals(tabName, StringComparison.OrdinalIgnoreCase) ? "active" : "";
        }
    }
}