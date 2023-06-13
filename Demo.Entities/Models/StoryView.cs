using System;
using System.Collections.Generic;

namespace Demo.Entities.Models;

public partial class StoryView
{
    public long ViewId { get; set; }

    public long StoryId { get; set; }

    public long UserId { get; set; }

    public virtual Story Story { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
