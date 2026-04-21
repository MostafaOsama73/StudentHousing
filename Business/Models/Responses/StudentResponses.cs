using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.Responses;

public class StudentResponse
{
    public Guid StudentId { get; set; }
    public string? UserId { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PreferredArea { get; set; }
    public string NationalId { get; set; }

    
    public DateTime CreatedAt { get; set; } 
    public DateTime? UpdatedAt { get; set; }

    public bool IsVerified { get; set; } 
    public VerificationStatus? VerificationStatus { get; set; }  

}

public class StudentIndexedResponse : GenericIndexedResponse<StudentResponse>
{
}
