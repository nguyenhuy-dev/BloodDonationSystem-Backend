using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Event
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; }
    public int MaxOfDonor { get; set; }
    public double EstimatedVolume { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public DateTime EventTime { get; set; }
    public bool EventType { get; set; }
    public bool IsExpired { get; set; }

    public int BloodTypeId { get; set; }
    public BloodComponent BloodComponent { get; set; }
    public Guid CreateBy { get; set; }
    public Guid UpdateBy { get; set; }
    public int FacilityId { get; set; }

    [ForeignKey("BloodTypeId")]
    public virtual BloodType BloodType { get; set; }

    [ForeignKey("CreateBy")]
    public virtual User Creator { get; set; }

    [ForeignKey("UpdateBy")]
    public virtual User Updater { get; set; }

    [ForeignKey("FacilityId")]
    public virtual Facility Facility { get; set; }

    public virtual ICollection<BloodRegistration> BloodRegistrations { get; set; }
}

