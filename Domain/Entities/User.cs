using Microsoft.AspNetCore.Identity;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class User : IdentityUser    
{
    //public string Name { get; set; }

    //public string AccountStatus { get; set; }

    //public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }
    public string? ProfileImage { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Pending;
    public virtual List<Notification> Notifications { get; set; } = null!;
}

