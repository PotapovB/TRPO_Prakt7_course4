using System;
using System.Collections.Generic;

namespace TRPO_Prakt7.Models;

public partial class Task
{
    public int? Id { get; set; }

    public string? Title { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public string? Priority { get; set; } = null!;

    public DateTime? Deadline { get; set; }

    public bool? IsCompleted { get; set; }
}
