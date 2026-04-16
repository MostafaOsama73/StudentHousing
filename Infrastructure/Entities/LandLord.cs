using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class LandLord
{
    public Guid LandLordId { get; set; }
    public string? UserId { get; set; }
    public string CompanyName { get; set; }
    public string NationalId { get; set; }
    public string PropertyOwnerShipProof { get; set; }
    public string VerificationStatus { get; set; }
    
    public virtual User? User { get; set; }
}
