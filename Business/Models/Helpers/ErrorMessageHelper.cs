using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Models.Helpers;

public static class ErrorMessageHelper
{
    public static string InvalidCredentials => "Invalid email or password.";
    public static string UserAlreadyExists => "User already exists.";
    public static string UserNotFound => "User not found.";
    public static string UserIsDeleted => "User account is deleted.";
}
