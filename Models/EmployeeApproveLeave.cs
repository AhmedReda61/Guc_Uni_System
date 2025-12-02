using System;
using System.Collections.Generic;

namespace Guc_Uni_System.Models;

public partial class EmployeeApproveLeave
{
    public int Emp1Id { get; set; }

    public int LeaveId { get; set; }

    public string? Status { get; set; }

    public virtual Employee Emp1 { get; set; } = null!;

    public virtual Leave Leave { get; set; } = null!;
}
