using System.Text;
using System.Web;
using IdentityDotnet7.Api.Configuration.EmailSender;
using IdentityDotnet7.Api.Configuration.JwtTokenGenerator;
using IdentityDotnet7.Api.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityDotnet7.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMailService _mailService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IJwtTokenGenerator jwtTokenGenerator, IMailService mailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _mailService = mailService;
        }
        
        /*
         * We can use design patterns to write a clean code
         * Repository pattern and clean code architecture and CQRS or any other options could be useful
         * To keep the example simple, the significant methods could be implemented in this exercise
         */
        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterDto dto)
        {
            // Check user has been registered already
            var exist = await _userManager.FindByEmailAsync(dto.Email);

            if (exist != null)
                return BadRequest(new { message = $"{exist.Email} has been registered already! "});

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(new { message = "Registration failed"});
            
            // Check if users has not been registered with role user, then the user will be added to the User role
            if (!await _userManager.IsInRoleAsync(user, "User"))
                await _userManager.AddToRoleAsync(user, "User");

            // Generate token
            var token = HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(user));
            
            //Generate Confirmation link
            var confirmationLink = new StringBuilder($"https://localhost:7130/api/account/confirmEmail?token={token}&userId={user.Id}");

            //Send email
            var status = _mailService.Send(user.Email, "Email Confirmation", confirmationLink.ToString(), false);

            if (status)
                return StatusCode(StatusCodes.Status201Created, new { message = "Confirmation link has been sent to your email address." });

            return StatusCode(StatusCodes.Status400BadRequest);
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(LoginDto dto)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);

            if (signInResult.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);

                if (user.EmailConfirmed)
                {
                    var token = _jwtTokenGenerator.GenerateToken(user!);

                    return Ok(new { email = dto.Email, token });
                }
            }
            
            return BadRequest();
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null || string.IsNullOrEmpty(token))
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                var tokenJwt = _jwtTokenGenerator.GenerateToken(user);

                return Ok(new { email = user.Email, tokenJwt });
            }

            return Forbid();
        }

        [HttpGet("ResendConfirmationLink")]
        public async Task<IActionResult> ResendConfirmationLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null)
            {
                var token = HttpUtility.UrlDecode(await _userManager.GenerateEmailConfirmationTokenAsync(user));

                var confirmationLink = new StringBuilder($"https://localhost:7130/api/account/confirmEmail?token={token}&userId={user.Id}");

                var status = _mailService.Send(user.Email, "Email Confirmation", confirmationLink.ToString(), false);

                if (status)
                    return StatusCode(StatusCodes.Status201Created, new { message = "Confirmation link has been sent to your email address." });
            }

            return BadRequest();
        }

        [HttpGet("ResetPasswordLink")]
        public async Task<IActionResult> ResetPasswordLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null || user.EmailConfirmed is not false)
            {
                var token = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));

                var passwordResetLink = new StringBuilder($"https://localhost:7130/api/account/resetPassword?token={token}&userId={user.Id}");

                var status = _mailService.Send(user.Email, "Reset Password", passwordResetLink.ToString(), false);

                if (status)
                    return StatusCode(StatusCodes.Status201Created, new { message = "Reset password link has been sent to your email address." });
            }

            return BadRequest();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token, string userId, ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return NotFound();

            var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);

            if (result.Succeeded)
                return Ok(new { message = "Your password has been updated." });

            return BadRequest();
        }
    }
}
