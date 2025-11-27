using InnoShop.Users.Domain.Entities;

namespace InnoShop.Users.Application.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(Guid id, User updatedUser);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> ActivateAsync(Guid id);
    Task<bool> IsExistsAsync(Guid id);

    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}