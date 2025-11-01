using Microsoft.EntityFrameworkCore;
using HopewellClinicApi.Data;
using HopewellClinicApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace HopewellClinicApi.Services
{
    public interface IPasswordResetService
    {
        Task<string> GenerateResetTokenAsync(Guid userId);
        Task<bool> ValidateResetTokenAsync(string token);
        Task<ApplicationUser?> GetUserByResetTokenAsync(string token);
        Task MarkTokenAsUsedAsync(string token);
        Task CleanupExpiredTokensAsync();
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly HopewellDbContext _context;
        private readonly ILogger<PasswordResetService> _logger;

        public PasswordResetService(HopewellDbContext context, ILogger<PasswordResetService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GenerateResetTokenAsync(Guid userId)
        {
            try
            {
                // Generate a secure random token
                var tokenBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(tokenBytes);
                }
                var token = Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");

                // Set expiration time (1 hour from now)
                var expiresAt = DateTime.UtcNow.AddHours(1);

                // Invalidate any existing tokens for this user
                var existingTokens = await _context.PasswordResetTokens
                    .Where(prt => prt.UserId == userId && !prt.IsUsed)
                    .ToListAsync();

                foreach (var existingToken in existingTokens)
                {
                    existingToken.IsUsed = true;
                }

                // Create new token
                var resetToken = new PasswordResetToken
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Token = token,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                };

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Generated password reset token for user {UserId}", userId);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating reset token for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            try
            {
                var resetToken = await _context.PasswordResetTokens
                    .FirstOrDefaultAsync(prt => prt.Token == token && !prt.IsUsed);

                if (resetToken == null)
                {
                    _logger.LogWarning("Reset token not found or already used: {Token}", token);
                    return false;
                }

                if (resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    _logger.LogWarning("Reset token expired: {Token}", token);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating reset token: {Token}", token);
                return false;
            }
        }

        public async Task<ApplicationUser?> GetUserByResetTokenAsync(string token)
        {
            try
            {
                var resetToken = await _context.PasswordResetTokens
                    .Include(prt => prt.User)
                    .FirstOrDefaultAsync(prt => prt.Token == token && !prt.IsUsed);

                if (resetToken == null || resetToken.ExpiresAt < DateTime.UtcNow)
                {
                    return null;
                }

                return resetToken.User;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by reset token: {Token}", token);
                return null;
            }
        }

        public async Task MarkTokenAsUsedAsync(string token)
        {
            try
            {
                var resetToken = await _context.PasswordResetTokens
                    .FirstOrDefaultAsync(prt => prt.Token == token);

                if (resetToken != null)
                {
                    resetToken.IsUsed = true;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Marked reset token as used: {Token}", token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking token as used: {Token}", token);
                throw;
            }
        }

        public async Task CleanupExpiredTokensAsync()
        {
            try
            {
                var expiredTokens = await _context.PasswordResetTokens
                    .Where(prt => prt.ExpiresAt < DateTime.UtcNow.AddDays(-1)) // Clean up tokens older than 1 day
                    .ToListAsync();

                if (expiredTokens.Any())
                {
                    _context.PasswordResetTokens.RemoveRange(expiredTokens);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cleaned up {Count} expired password reset tokens", expiredTokens.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired tokens");
                throw;
            }
        }
    }
}




