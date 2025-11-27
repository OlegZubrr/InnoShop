using System.Security.Claims;
using InnoShop.Users.Application.DTOs;
using InnoShop.Users.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Users.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IUserService _userService;

    public AuthController(
        IAuthService authService,
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _userService = userService;
        _logger = logger;
    }


    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserCreateDto createDto)
    {
        var user = await _authService.RegisterAsync(createDto);
        _logger.LogInformation("User registered successfully: {Email}", createDto.Email);

        return Ok(new
        {
            message = "Registration successful. Please check your email to confirm your account.",
            user
        });
    }


    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var response = await _authService.LoginAsync(loginDto);
        _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);
        return Ok(response);
    }


    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
        await _authService.ConfirmEmailAsync(token);
        _logger.LogInformation("Email confirmed successfully");
        return Ok(new { message = "Email confirmed successfully. You can now log in." });
    }


    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
        _logger.LogInformation("Password reset requested for: {Email}", forgotPasswordDto.Email);

        return Ok(new { message = "If the email exists, a password reset link has been sent." });
    }


    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        await _authService.ResetPasswordAsync(resetPasswordDto);
        _logger.LogInformation("Password reset successfully");
        return Ok(new { message = "Password has been reset successfully" });
    }


    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId)) return Unauthorized(new { message = "Invalid token" });

        var user = await _userService.GetByIdAsync(Guid.Parse(userId));
        _logger.LogInformation("Current user retrieved: {UserId}", userId);

        return Ok(user);
    }
}