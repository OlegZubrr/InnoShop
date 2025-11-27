using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailConfirmationTokenAsync(string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);
    }

    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == token
                                      && u.PasswordResetTokenExpiry > DateTime.UtcNow);
    }
}