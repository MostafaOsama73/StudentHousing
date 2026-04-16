using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities;

public class Notification
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public bool IsSeen { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
