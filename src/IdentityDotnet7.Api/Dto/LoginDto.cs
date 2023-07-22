using System.ComponentModel.DataAnnotations;

namespace IdentityDotnet7.Api.Dto;

public class LoginDto
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}