using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Guc_Uni_System.Models;

public partial class UniversityHrManagementSystemContext : DbContext
{
    public UniversityHrManagementSystemContext()
    {
    }

    public UniversityHrManagementSystemContext(DbContextOptions<UniversityHrManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccidentalLeave> AccidentalLeaves { get; set; }

    public virtual DbSet<AllEmployeeAttendance> AllEmployeeAttendances { get; set; }

    public virtual DbSet<AllEmployeeProfile> AllEmployeeProfiles { get; set; }

    public virtual DbSet<AllPerformance> AllPerformances { get; set; }

    public virtual DbSet<AllRejectedMedical> AllRejectedMedicals { get; set; }

    public virtual DbSet<AnnualLeave> AnnualLeaves { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<CompensationLeave> CompensationLeaves { get; set; }

    public virtual DbSet<Deduction> Deductions { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeApproveLeave> EmployeeApproveLeaves { get; set; }

    public virtual DbSet<EmployeePhone> EmployeePhones { get; set; }

    public virtual DbSet<EmployeeReplaceEmployee> EmployeeReplaceEmployees { get; set; }

    public virtual DbSet<Leave> Leaves { get; set; }

    public virtual DbSet<MedicalLeave> MedicalLeaves { get; set; }

    public virtual DbSet<NoEmployeeDept> NoEmployeeDepts { get; set; }

    public virtual DbSet<Payroll> Payrolls { get; set; }

    public virtual DbSet<Performance> Performances { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<UnpaidLeave> UnpaidLeaves { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:MyDbConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccidentalLeave>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Accident__18D0B537FD671CF0");

            entity.ToTable("Accidental_Leave");

            entity.Property(e => e.RequestId)
                .ValueGeneratedNever()
                .HasColumnName("request_ID");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");

            entity.HasOne(d => d.Emp).WithMany(p => p.AccidentalLeaves)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Accidenta__emp_I__693CA210");

            entity.HasOne(d => d.Request).WithOne(p => p.AccidentalLeave)
                .HasForeignKey<AccidentalLeave>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accidenta__reque__68487DD7");
        });

        modelBuilder.Entity<AllEmployeeAttendance>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("allEmployeeAttendance");

            entity.Property(e => e.AttendanceId)
                .ValueGeneratedOnAdd()
                .HasColumnName("attendance_ID");
            entity.Property(e => e.CheckInTime).HasColumnName("check_in_time");
            entity.Property(e => e.CheckOutTime).HasColumnName("check_out_time");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TotalDuration).HasColumnName("total_duration");
        });

        modelBuilder.Entity<AllEmployeeProfile>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("allEmployeeProfiles");

            entity.Property(e => e.AccidentalBalance).HasColumnName("accidental_balance");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.AnnualBalance).HasColumnName("annual_balance");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EmployeeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("employee_ID");
            entity.Property(e => e.EmploymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employment_status");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.OfficialDayOff)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("official_day_off");
            entity.Property(e => e.TypeOfContract)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_of_contract");
            entity.Property(e => e.YearsOfExperience).HasColumnName("years_of_experience");
        });

        modelBuilder.Entity<AllPerformance>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("allPerformance");

            entity.Property(e => e.Comments)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comments");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");
            entity.Property(e => e.PerformanceId).HasColumnName("performance_ID");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Semester)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("semester");
        });

        modelBuilder.Entity<AllRejectedMedical>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("allRejectedMedicals");

            entity.Property(e => e.DisabilityDetails)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("disability_details");
            entity.Property(e => e.EmpId).HasColumnName("Emp_ID");
            entity.Property(e => e.InsuranceStatus).HasColumnName("insurance_status");
            entity.Property(e => e.RequestId).HasColumnName("request_ID");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<AnnualLeave>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Annual_L__18D0B5378FF60A8C");

            entity.ToTable("Annual_Leave");

            entity.Property(e => e.RequestId)
                .ValueGeneratedNever()
                .HasColumnName("request_ID");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");
            entity.Property(e => e.ReplacementEmp).HasColumnName("replacement_emp");

            entity.HasOne(d => d.Emp).WithMany(p => p.AnnualLeaveEmps)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Annual_Le__emp_I__6477ECF3");

            entity.HasOne(d => d.ReplacementEmpNavigation).WithMany(p => p.AnnualLeaveReplacementEmpNavigations)
                .HasForeignKey(d => d.ReplacementEmp)
                .HasConstraintName("FK__Annual_Le__repla__656C112C");

            entity.HasOne(d => d.Request).WithOne(p => p.AnnualLeave)
                .HasForeignKey<AnnualLeave>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Annual_Le__reque__6383C8BA");
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PK__Attendan__20D7AD30BE899575");

            entity.ToTable("Attendance");

            entity.Property(e => e.AttendanceId).HasColumnName("attendance_ID");
            entity.Property(e => e.CheckInTime).HasColumnName("check_in_time");
            entity.Property(e => e.CheckOutTime).HasColumnName("check_out_time");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Absent")
                .HasColumnName("status");
            entity.Property(e => e.TotalDuration)
                .HasComputedColumnSql("(datediff(minute,[check_in_time],[check_out_time]))", false)
                .HasColumnName("total_duration");

            entity.HasOne(d => d.Emp).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Attendanc__emp_I__03F0984C");
        });

        modelBuilder.Entity<CompensationLeave>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Compensa__18D0B5379571E97A");

            entity.ToTable("Compensation_Leave");

            entity.Property(e => e.RequestId)
                .ValueGeneratedNever()
                .HasColumnName("request_ID");
            entity.Property(e => e.DateOfOriginalWorkday).HasColumnName("date_of_original_workday");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");
            entity.Property(e => e.Reason)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("reason");
            entity.Property(e => e.ReplacementEmp).HasColumnName("replacement_emp");

            entity.HasOne(d => d.Emp).WithMany(p => p.CompensationLeaveEmps)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Compensat__emp_I__75A278F5");

            entity.HasOne(d => d.ReplacementEmpNavigation).WithMany(p => p.CompensationLeaveReplacementEmpNavigations)
                .HasForeignKey(d => d.ReplacementEmp)
                .HasConstraintName("FK__Compensat__repla__76969D2E");

            entity.HasOne(d => d.Request).WithOne(p => p.CompensationLeave)
                .HasForeignKey<CompensationLeave>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compensat__reque__74AE54BC");
        });

        modelBuilder.Entity<Deduction>(entity =>
        {
            entity.HasKey(e => new { e.DeductionId, e.EmpId }).HasName("PK__Deductio__10DEF40503813CCC");

            entity.ToTable("Deduction");

            entity.Property(e => e.DeductionId)
                .ValueGeneratedOnAdd()
                .HasColumnName("deduction_ID");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.AttendanceId).HasColumnName("attendance_ID");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.UnpaidId).HasColumnName("unpaid_ID");

            entity.HasOne(d => d.Attendance).WithMany(p => p.Deductions)
                .HasForeignKey(d => d.AttendanceId)
                .HasConstraintName("FK__Deduction__atten__0B91BA14");

            entity.HasOne(d => d.Emp).WithMany(p => p.Deductions)
                .HasForeignKey(d => d.EmpId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Deduction__emp_I__06CD04F7");

            entity.HasOne(d => d.Unpaid).WithMany(p => p.Deductions)
                .HasForeignKey(d => d.UnpaidId)
                .HasConstraintName("FK__Deduction__unpai__0A9D95DB");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Name).HasName("PK__Departme__72E12F1A3EECE34E");

            entity.ToTable("Department");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.BuildingLocation)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("building_location");

            entity.HasMany(d => d.RoleNames).WithMany(p => p.DepartmentNames)
                .UsingEntity<Dictionary<string, object>>(
                    "RoleExistsInDepartment",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleName")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Role_exis__Role___5CD6CB2B"),
                    l => l.HasOne<Department>().WithMany()
                        .HasForeignKey("DepartmentName")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Role_exis__depar__5BE2A6F2"),
                    j =>
                    {
                        j.HasKey("DepartmentName", "RoleName").HasName("PK__Role_exi__90D2B2969929FF93");
                        j.ToTable("Role_existsIn_Department");
                        j.IndexerProperty<string>("DepartmentName")
                            .HasMaxLength(50)
                            .IsUnicode(false)
                            .HasColumnName("department_name");
                        j.IndexerProperty<string>("RoleName")
                            .HasMaxLength(50)
                            .IsUnicode(false)
                            .HasColumnName("Role_name");
                    });
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Document__9666E8ACC438FB72");

            entity.ToTable("Document");

            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("file_name");
            entity.Property(e => e.MedicalId).HasColumnName("medical_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.UnpaidId).HasColumnName("unpaid_ID");

            entity.HasOne(d => d.Emp).WithMany(p => p.Documents)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Document__emp_ID__7A672E12");

            entity.HasOne(d => d.Medical).WithMany(p => p.Documents)
                .HasForeignKey(d => d.MedicalId)
                .HasConstraintName("FK__Document__medica__7B5B524B");

            entity.HasOne(d => d.Unpaid).WithMany(p => p.Documents)
                .HasForeignKey(d => d.UnpaidId)
                .HasConstraintName("FK__Document__unpaid__7C4F7684");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C52E0BA8F840466E");

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.AccidentalBalance).HasColumnName("accidental_balance");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.AnnualBalance).HasColumnName("annual_balance");
            entity.Property(e => e.DeptName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("dept_name");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.EmergencyContactName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("emergency_contact_name");
            entity.Property(e => e.EmergencyContactPhone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("emergency_contact_phone");
            entity.Property(e => e.EmploymentStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employment_status");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.LastWorkingDate).HasColumnName("last_working_date");
            entity.Property(e => e.NationalId)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("national_ID");
            entity.Property(e => e.OfficialDayOff)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("official_day_off");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Salary)
                .HasComputedColumnSql("([dbo].[HRSalary_calculation]([employee_id]))", false)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salary");
            entity.Property(e => e.TypeOfContract)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type_of_contract");
            entity.Property(e => e.YearsOfExperience).HasColumnName("years_of_experience");

            entity.HasOne(d => d.DeptNameNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DeptName)
                .HasConstraintName("FK__Employee__dept_n__5070F446");

            entity.HasMany(d => d.RoleNames).WithMany(p => p.Emps)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleName")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Employee___role___59063A47"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmpId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Employee___emp_I__5812160E"),
                    j =>
                    {
                        j.HasKey("EmpId", "RoleName").HasName("PK__Employee__0506608288AFFF80");
                        j.ToTable("Employee_Role");
                        j.IndexerProperty<int>("EmpId").HasColumnName("emp_ID");
                        j.IndexerProperty<string>("RoleName")
                            .HasMaxLength(50)
                            .IsUnicode(false)
                            .HasColumnName("role_name");
                    });
        });

        modelBuilder.Entity<EmployeeApproveLeave>(entity =>
        {
            entity.HasKey(e => new { e.Emp1Id, e.LeaveId }).HasName("PK__Employee__01AC16ECFC9015AD");

            entity.ToTable("Employee_Approve_Leave");

            entity.Property(e => e.Emp1Id).HasColumnName("Emp1_ID");
            entity.Property(e => e.LeaveId).HasColumnName("leave_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("status");

            entity.HasOne(d => d.Emp1).WithMany(p => p.EmployeeApproveLeaves)
                .HasForeignKey(d => d.Emp1Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee___Emp1___160F4887");

            entity.HasOne(d => d.Leave).WithMany(p => p.EmployeeApproveLeaves)
                .HasForeignKey(d => d.LeaveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee___leave__17036CC0");
        });

        modelBuilder.Entity<EmployeePhone>(entity =>
        {
            entity.HasKey(e => new { e.EmpId, e.PhoneNum }).HasName("PK__Employee__2CC0848A3CE1C56E");

            entity.ToTable("Employee_Phone");

            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.PhoneNum)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone_num");

            entity.HasOne(d => d.Emp).WithMany(p => p.EmployeePhones)
                .HasForeignKey(d => d.EmpId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee___emp_i__534D60F1");
        });

        modelBuilder.Entity<EmployeeReplaceEmployee>(entity =>
        {
            entity.HasKey(e => new { e.TableId, e.Emp1Id, e.Emp2Id }).HasName("PK__Employee__A0B8B21CDD36E0BA");

            entity.ToTable("Employee_Replace_Employee");

            entity.Property(e => e.TableId)
                .ValueGeneratedOnAdd()
                .HasColumnName("table_id");
            entity.Property(e => e.Emp1Id).HasColumnName("Emp1_ID");
            entity.Property(e => e.Emp2Id).HasColumnName("Emp2_ID");
            entity.Property(e => e.FromDate).HasColumnName("from_date");
            entity.Property(e => e.ToDate).HasColumnName("to_date");

            entity.HasOne(d => d.Emp1).WithMany(p => p.EmployeeReplaceEmployeeEmp1s)
                .HasForeignKey(d => d.Emp1Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee___Emp1___123EB7A3");

            entity.HasOne(d => d.Emp2).WithMany(p => p.EmployeeReplaceEmployeeEmp2s)
                .HasForeignKey(d => d.Emp2Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employee___Emp2___1332DBDC");
        });

        modelBuilder.Entity<Leave>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Leave__18D0B5376FBCB274");

            entity.ToTable("Leave");

            entity.Property(e => e.RequestId).HasColumnName("request_ID");
            entity.Property(e => e.DateOfRequest).HasColumnName("date_of_request");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FinalApprovalStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending")
                .HasColumnName("final_approval_status");
            entity.Property(e => e.NumDays)
                .HasComputedColumnSql("(datediff(day,[start_date],[end_date])+(1))", false)
                .HasColumnName("num_days");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
        });

        modelBuilder.Entity<MedicalLeave>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Medical___18D0B5379B8D4F2D");

            entity.ToTable("Medical_Leave");

            entity.Property(e => e.RequestId)
                .ValueGeneratedNever()
                .HasColumnName("request_ID");
            entity.Property(e => e.DisabilityDetails)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("disability_details");
            entity.Property(e => e.EmpId).HasColumnName("Emp_ID");
            entity.Property(e => e.InsuranceStatus).HasColumnName("insurance_status");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.Emp).WithMany(p => p.MedicalLeaves)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Medical_L__Emp_I__6E01572D");

            entity.HasOne(d => d.Request).WithOne(p => p.MedicalLeave)
                .HasForeignKey<MedicalLeave>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Medical_L__reque__6C190EBB");
        });

        modelBuilder.Entity<NoEmployeeDept>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("NoEmployeeDept");

            entity.Property(e => e.Department)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumberOfEmployees).HasColumnName("Number of Employees");
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payroll__3214EC2759EA2E14");

            entity.ToTable("Payroll");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BonusAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("bonus_amount");
            entity.Property(e => e.Comments)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("comments");
            entity.Property(e => e.DeductionsAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("deductions_amount");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");
            entity.Property(e => e.FinalSalaryAmount)
                .HasColumnType("decimal(10, 1)")
                .HasColumnName("final_salary_amount");
            entity.Property(e => e.FromDate).HasColumnName("from_date");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.ToDate).HasColumnName("to_date");

            entity.HasOne(d => d.Emp).WithMany(p => p.Payrolls)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Payroll__emp_ID__7F2BE32F");
        });

        modelBuilder.Entity<Performance>(entity =>
        {
            entity.HasKey(e => e.PerformanceId).HasName("PK__Performa__8C2B1B88475FB8A1");

            entity.ToTable("Performance");

            entity.Property(e => e.PerformanceId).HasColumnName("performance_ID");
            entity.Property(e => e.Comments)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comments");
            entity.Property(e => e.EmpId).HasColumnName("emp_ID");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Semester)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("semester");

            entity.HasOne(d => d.Emp).WithMany(p => p.Performances)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Performan__emp_I__0F624AF8");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleName).HasName("PK__Role__783254B027A13D4E");

            entity.ToTable("Role");

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
            entity.Property(e => e.AccidentalBalance).HasColumnName("accidental_balance");
            entity.Property(e => e.AnnualBalance).HasColumnName("annual_balance");
            entity.Property(e => e.BaseSalary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("base_salary");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.PercentageOvertime)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("percentage_overtime");
            entity.Property(e => e.PercentageYoe)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("percentage_YOE");
            entity.Property(e => e.Rank).HasColumnName("rank");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<UnpaidLeave>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Unpaid_L__18D0B537A6443626");

            entity.ToTable("Unpaid_Leave");

            entity.Property(e => e.RequestId)
                .ValueGeneratedNever()
                .HasColumnName("request_ID");
            entity.Property(e => e.EmpId).HasColumnName("Emp_ID");

            entity.HasOne(d => d.Emp).WithMany(p => p.UnpaidLeaves)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Unpaid_Le__Emp_I__71D1E811");

            entity.HasOne(d => d.Request).WithOne(p => p.UnpaidLeave)
                .HasForeignKey<UnpaidLeave>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Unpaid_Le__reque__70DDC3D8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
