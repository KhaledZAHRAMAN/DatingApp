using System.ComponentModel.DataAnnotations;

namespace API;

public class RegisterDto
{
    [Required]
    public required string UserName { get; set; }
    public required string Password { get; set; }
}