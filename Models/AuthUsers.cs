using System.ComponentModel.DataAnnotations;
namespace Etape1.Models;

public class AuthUsers
{
    [Required(ErrorMessage ="Email is required.")]
    [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.com", ErrorMessage ="Invalid Email.")]
    public string Email { get; set; }
    
    [DataType(DataType.Password)]
    [Required(ErrorMessage ="Password is required.")]
    public string Password { get; set; }
}