using System;
using System.Collections.Generic;

namespace Demo.Entities.Models;

public partial class Comment
{
    public long CommentId { get; set; }

    public string? Comments { get; set; }

    public long UserId { get; set; }

    public long MissionId { get; set; }

    public string? ApprovalStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Mission Mission { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
