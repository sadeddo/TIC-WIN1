using System.ComponentModel.DataAnnotations;


namespace Etape1.Models;

public class Users
{
    public Guid Id { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    public string Role { get; set; }
}