using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class BloodRegistration
{
    [Key]
    public int Id { get; set; }

    [Required]
    public RegistrationStatus? Status { get; set; }

    public DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }

    public int? VolunteerId { get; set; }
    public int? HealthId { get; set; }
    public int? BloodProcedureId { get; set; }
    public Guid MemberId { get; set; }
    public Guid? StaffId { get; set; }
    public int EventId { get; set; }

    [ForeignKey("VolunteerId")]
    public virtual Volunteer Volunteer { get; set; }

    [ForeignKey("HealthId")]
    public virtual HealthProcedure HealthProcedure { get; set; }

    [ForeignKey("BloodProcedureId")]
    public virtual BloodProcedure BloodProcedure { get; set; }

    [ForeignKey("MemberId")]
    public virtual User Member { get; set; }

    [ForeignKey("StaffId")]
    public virtual User Staff { get; set; }

    [ForeignKey("EventId")]
    public virtual Event Event { get; set; }

    public virtual BloodInventory BloodInventory { get; set; }
}