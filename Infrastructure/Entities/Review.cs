using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class Review
{
    public Guid ReviewId { get; set; }
    public Guid StudentId { get; set; }
    public Guid HousingUnitId { get; set; }

    //[AllowedValues(1,2, 3, 4, 5)]
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime ReviewDate { get; set; }     
    
    public HousingUnit? HousingUnit { get; set; }
    public Student? Student { get; set; }

}
