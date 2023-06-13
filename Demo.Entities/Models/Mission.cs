using System;
using System.Collections.Generic;

namespace Demo.Entities.Models;

public partial class Mission
{
    public long MissionId { get; set; }

    public long ThemeId { get; set; }

    public long CityId { get; set; }

    public long CountryId { get; set; }

    public string Title { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? Deadline { get; set; }

    public bool MissionType { get; set; }

    public int? TotalSeat { get; set; }

    public int? AvailableSeat { get; set; }

    public bool? Status { get; set; }

    public string? OrganizationName { get; set; }

    public string? OrganizationDetail { get; set; }

    public string? Avaibility { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<FavoriteMission> FavoriteMissions { get; set; } = new List<FavoriteMission>();

    public virtual ICollection<GoalMission> GoalMissions { get; set; } = new List<GoalMission>();

    public virtual ICollection<MissionDocument> MissionDocuments { get; set; } = new List<MissionDocument>();

    public virtual ICollection<MissionInvite> MissionInvites { get; set; } = new List<MissionInvite>();

    public virtual ICollection<MissionMedium> MissionMedia { get; set; } = new List<MissionMedium>();

    public virtual ICollection<MissionRating> MissionRatings { get; set; } = new List<MissionRating>();

    public virtual ICollection<MissionSkill> MissionSkills { get; set; } = new List<MissionSkill>();

    public virtual ICollection<NotificationMessage> NotificationMessages { get; set; } = new List<NotificationMessage>();

    public virtual ICollection<Story> Stories { get; set; } = new List<Story>();

    public virtual ICollection<TimeMission> TimeMissions { get; set; } = new List<TimeMission>();

    public virtual ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();
}
