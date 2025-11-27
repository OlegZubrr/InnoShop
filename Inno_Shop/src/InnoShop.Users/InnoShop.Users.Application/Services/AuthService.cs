using AutoMapper;
using InnoShop.Users.Application.DTOs;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Application.Interfaces.Services;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using InnoShop.Users.Domain.Exceptions;

namespace InnoShop.Users.Application.Services;

public class AuthService : IAuthService
{
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IEmailService emailService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        if (!user.IsActive) throw new UserDeactivatedException();

        if (!user.IsEmailConfirmed) throw new EmailNotConfirmedException();

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var userDto = _mapper.Map<UserResponseDto>(user);

        return new LoginResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            User = userDto
        };
    }

    public async Task<UserResponseDto> RegisterAsync(UserCreateDto createDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(createDto.Email);
        if (existingUser != null) throw new UserAlreadyExistsException(createDto.Email);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = createDto.FullName,
            Email = createDto.Email,
            PasswordHash = passwordHash,
            Role = UserRole.User,
            IsActive = true,
            IsEmailConfirmed = false,
            EmailConfirmationToken = _tokenService.GenerateEmailConfirmationToken(),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        await _emailService.SendEmailConfirmationAsync(user.Email, user.EmailConfirmationToken!, user.FullName);

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        var user = await _userRepository.GetByEmailConfirmationTokenAsync(token);

        if (user == null) throw new InvalidTokenException("email confirmation");

        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return true;

        user.PasswordResetToken = _tokenService.GeneratePasswordResetToken();
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        await _emailService.SendPasswordResetAsync(user.Email, user.PasswordResetToken!, user.FullName);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(resetPasswordDto.Token);

        if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
            throw new InvalidTokenException("password reset");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }
}