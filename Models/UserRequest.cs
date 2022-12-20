using System.ComponentModel.DataAnnotations;
namespace Etape1.Models;

public class UserRequest
{
    [Required(ErrorMessage ="Email is required.")]
    [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.com", ErrorMessage ="Invalid Email.")]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    [Required(ErrorMessage ="Password is required.")]
    public string Password { get; set; }
    [Required(ErrorMessage ="Role is required.")]
    public string Role { get; set; }
}