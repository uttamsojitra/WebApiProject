using System;
using System.Collections.Generic;

namespace Demo.Entities.Models;

public partial class MissionInvite
{
    public long MissionInviteId { get; set; }

    public long MissionId { get; set; }

    public long FromUserId { get; set; }

    public long ToUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Users FromUser { get; set; } = null!;

    public virtual Mission Mission { get; set; } = null!;

    public virtual Users ToUser { get; set; } = null!;
}
