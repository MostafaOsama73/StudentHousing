using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class Room
{
    public Guid RoomId { get; set; }
    public Guid HousingUnitId { get; set; }
    public RoomType RoomType { get; set; }
    public int NumberOfBeds { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
        
    public virtual HousingUnit? HousingUnit { get; set; }

}
