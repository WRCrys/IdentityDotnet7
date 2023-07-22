using System.ComponentModel.DataAnnotations;

namespace IdentityDotnet7.Api.Dto
{
    public class ResetPasswordDto
    {
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
