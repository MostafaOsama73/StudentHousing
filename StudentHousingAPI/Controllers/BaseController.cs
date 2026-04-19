using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StudentHousingAPI.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected string GetUserId()
    {
        return User.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? string.Empty;
    }
    
    protected string GetloggedId()
    {
        return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }


}
