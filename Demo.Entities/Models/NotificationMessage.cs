using System;
using System.Collections.Generic;

namespace Demo.Entities.Models;

public partial class NotificationMessage
{
    public long NotificationId { get; set; }

    public long UserId { get; set; }

    public long? MissionId { get; set; }

    public long? StoryId { get; set; }

    public string? MessageType { get; set; }

    public string? Message { get; set; }

    public bool? Read { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Mission? Mission { get; set; }

    public virtual Story? Story { get; set; }

    public virtual Users User { get; set; } = null!;
}
