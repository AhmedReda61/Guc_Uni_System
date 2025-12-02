using System;
using System.Collections.Generic;

namespace Guc_Uni_System.Models;

public partial class EmployeePhone
{
    public int EmpId { get; set; }

    public string PhoneNum { get; set; } = null!;

    public virtual Employee Emp { get; set; } = null!;
}
