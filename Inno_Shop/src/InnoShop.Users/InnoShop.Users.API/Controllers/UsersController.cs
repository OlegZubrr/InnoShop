using System.Security.Claims;
using AutoMapper;
using InnoShop.Users.Application.DTOs;
using InnoShop.Users.Application.Events;
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
    private readonly IMessageBus _messageBus;
    private readonly IUserService _userService;

    public UsersController(
        IUserService userService,
        IMapper mapper,
        ILogger<UsersController> logger,
        IMessageBus messageBus)
    {
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _messageBus = messageBus;
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
        var user = await _userService.GetByIdAsync(id);

        await _userService.DeleteAsync(id);
        _logger.LogInformation("User deleted: {Id}", id);

        var deleteEvent = new UserDeletedEvent
        {
            UserId = user!.Id,
            Email = user.Email,
            DeletedAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(deleteEvent, "user.deleted");
        _logger.LogInformation("UserDeletedEvent published for user: {Id}", id);

        return NoContent();
    }

    [HttpPatch("{id:guid}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        await _userService.DeactivateAsync(id);
        _logger.LogInformation("User deactivated: {Id}", id);

        var deactivateEvent = new UserDeactivatedEvent
        {
            UserId = user!.Id,
            Email = user.Email,
            DeactivatedAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(deactivateEvent, "user.deactivated");
        _logger.LogInformation("UserDeactivatedEvent published for user: {Id}", id);

        return Ok(new { message = "User deactivated successfully" });
    }

    [HttpPatch("{id:guid}/activate")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        await _userService.ActivateAsync(id);
        _logger.LogInformation("User activated: {Id}", id);

        var activateEvent = new UserActivatedEvent
        {
            UserId = user!.Id,
            Email = user.Email,
            ActivatedAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(activateEvent, "user.activated");
        _logger.LogInformation("UserActivatedEvent published for user: {Id}", id);

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

    [HttpPatch("{id:guid}/role")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateUserRoleDto updateRoleDto)
    {
        await _userService.UpdateRoleAsync(id, updateRoleDto.Role);
        _logger.LogInformation("User role updated: {Id} to {Role}", id, updateRoleDto.Role);

        return Ok(new { message = $"User role updated to {updateRoleDto.Role}" });
    }
}