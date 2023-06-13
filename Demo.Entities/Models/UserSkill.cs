﻿using System;
using System.Collections.Generic;

namespace Demo.Entities.Models;

public partial class UserSkill
{
    public long UserSkillId { get; set; }

    public long UserId { get; set; }

    public int SkillId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Skill Skill { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
