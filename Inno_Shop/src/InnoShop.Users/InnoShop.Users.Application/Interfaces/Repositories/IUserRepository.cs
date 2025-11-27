using InnoShop.Users.Domain.Entities;

namespace InnoShop.Users.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByEmailConfirmationTokenAsync(string token);
    Task<User?> GetByPasswordResetTokenAsync(string token);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<bool> IsExistsAsync(Guid id);
    Task SaveChangesAsync();
}