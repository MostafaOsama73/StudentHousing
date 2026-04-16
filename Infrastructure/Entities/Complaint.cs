using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class Complaint
{
    public Guid ComplaintId { get; set; }
    public Guid StudentId { get; set; }
    public Guid LandLordId { get; set; }
    public string Description { get; set; }

    public virtual Student? Student { get; set; }
    public virtual LandLord? LandLord { get; set; }
}
