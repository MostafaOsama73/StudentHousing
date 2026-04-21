using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class LandLord
{
    public Guid LandLordId { get; set; }
    public string FullName { get; set; }
    public string? UserId { get; set; }
    public string? CompanyName { get; set; }
    public string NationalId { get; set; }
    public string PropertyOwnerShipProof { get; set; }
    public string VerificationStatus { get; set; }
    
    // Audit timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Approval tracking
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }
    
    public virtual User? User { get; set; }
    public virtual ICollection<HousingUnit>? HousingUnits { get; set; }
}
