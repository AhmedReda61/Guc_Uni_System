using System;
using System.Collections.Generic;

namespace Guc_Uni_System.Models;

public partial class AllPerformance
{
    public int PerformanceId { get; set; }

    public int? Rating { get; set; }

    public string? Comments { get; set; }

    public string? Semester { get; set; }

    public int? EmpId { get; set; }
}
