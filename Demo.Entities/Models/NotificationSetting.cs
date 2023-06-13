using System;
using System.Collections.Generic;

namespace Demo.Entities.Models;

public partial class NotificationSetting
{
    public long NotificationSettingId { get; set; }

    public long UserId { get; set; }

    public string? NotificationName { get; set; }

    public virtual Users User { get; set; } = null!;
}
