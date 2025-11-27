using System.Security.Claims;
using AutoMapper;
using InnoShop.Users.Application.DTOs;
using InnoShop.Users.Application.Interfaces.Services;
using InnoShop.Users.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Users.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UsersController(
        IUserService userService,
        IMapper mapper,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        var dto = _mapper.Map<IEnumerable<UserResponseDto>>(users);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && currentUserId != id.ToString()) return Forbid();

        var user = await _userService.GetByIdAsync(id);
        var dto = _mapper.Map<UserResponseDto>(user);
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserCreateDto userCreateDto)
    {
        var userEntity = _mapper.Map<User>(userCreateDto);

        userEntity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password);

        var createdUser = await _userService.CreateAsync(userEntity);
        var dto = _mapper.Map<UserResponseDto>(createdUser);

        _logger.LogInformation("User created: {Email}", createdUser.Email);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto userUpdateDto)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        if (!isAdmin && currentUserId != id.ToString()) return Forbid();

        var userEntity = _mapper.Map<User>(userUpdateDto);


        await _userService.UpdateAsync(id, userEntity);
        _logger.LogInformation("User updated: {Id}", id);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteAsync(id);
        _logger.LogInformation("User deleted: {Id}", id);
        return NoContent();
    }

    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _userService.DeactivateAsync(id);
        _logger.LogInformation("User deactivated: {Id}", id);

        // TODO: Отправить событие в RabbitMQ для скрытия продуктов пользователя
        // await _messageBus.PublishAsync(new UserDeactivatedEvent { UserId = id });

        return Ok(new { message = "User deactivated successfully" });
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _userService.ActivateAsync(id);
        _logger.LogInformation("User activated: {Id}", id);

        // TODO: Отправить событие в RabbitMQ для восстановления продуктов пользователя
        // await _messageBus.PublishAsync(new UserActivatedEvent { UserId = id });

        return Ok(new { message = "User activated successfully" });
    }

    [HttpPost("{id:guid}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto changePasswordDto)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId != id.ToString()) return Forbid();

        await _userService.ChangePasswordAsync(
            id,
            changePasswordDto.CurrentPassword,
            changePasswordDto.NewPassword
        );

        _logger.LogInformation("Password changed for user: {Id}", id);
        return Ok(new { message = "Password changed successfully" });
    }
}