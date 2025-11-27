using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Application.Interfaces.Services;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Exceptions;

namespace InnoShop.Users.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new UserNotFoundException(id);

        return user;
    }

    public async Task<User> CreateAsync(User user)
    {
        var existing = await _userRepository.GetByEmailAsync(user.Email);
        if (existing != null)
            throw new UserAlreadyExistsException(user.Email);

        user.Id = Guid.NewGuid();

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new UserNotFoundException(id);

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _userRepository.IsExistsAsync(id);
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new UserNotFoundException(id);

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new UserNotFoundException(id);

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<User?> UpdateAsync(Guid id, User updatedUser)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
            throw new UserNotFoundException(id);


        if (existingUser.Email != updatedUser.Email)
        {
            var userWithEmail = await _userRepository.GetByEmailAsync(updatedUser.Email);
            if (userWithEmail != null)
                throw new UserAlreadyExistsException(updatedUser.Email);
        }

        existingUser.FullName = updatedUser.FullName;
        existingUser.Email = updatedUser.Email;
        existingUser.Role = updatedUser.Role;
        existingUser.IsActive = updatedUser.IsActive;

        await _userRepository.UpdateAsync(existingUser);

        await _userRepository.SaveChangesAsync();

        return existingUser;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null) throw new UserNotFoundException(userId);

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash)) throw new InvalidCredentialsException();

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }
}