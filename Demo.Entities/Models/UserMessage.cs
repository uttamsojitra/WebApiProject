using System;
using System.Collections.Generic;

namespace Demo.Entities.Models;

public partial class UserMessage
{
    public long MessageId { get; set; }

    public long UserId { get; set; }

    public string? Email { get; set; }

    public string? Message { get; set; }

    public string? Subject { get; set; }

    public virtual Users User { get; set; } = null!;
}
