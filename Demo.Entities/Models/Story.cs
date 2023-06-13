using System;
using System.Collections.Generic;

namespace Demo.Entities.Models;

public partial class Story
{
    public long StoryId { get; set; }

    public long UserId { get; set; }

    public long MissionId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Mission Mission { get; set; } = null!;

    public virtual ICollection<NotificationMessage> NotificationMessages { get; set; } = new List<NotificationMessage>();

    public virtual ICollection<StoryMedium> StoryMedia { get; set; } = new List<StoryMedium>();

    public virtual ICollection<StoryView> StoryViews { get; set; } = new List<StoryView>();

    public virtual Users User { get; set; } = null!;
}
