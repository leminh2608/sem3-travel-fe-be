using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace KarnelTravels.API.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken);
    Task<ApiResponse<string>> ForgotPasswordAsync(string email);
    Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
    Task<ApiResponse<AuthResponse>> AdminLoginAsync(LoginRequest request);
    Task<ApiResponse<string>> LogoutAsync(string userId);
    Task<ApiResponse<UserProfileDto>> GetCurrentUserAsync(string userId);
    Task<ApiResponse<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}

public class AuthService : IAuthService
{
    private readonly KarnelTravelsDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(KarnelTravelsDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        if (user.IsLocked)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Account is locked"
            };
        }

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new ApiResponse<AuthResponse>
        {
            Success = true,
            Message = "Login successful",
            Data = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Avatar = user.Avatar,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600
            }
        };
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Email already exists"
            };
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.User,
            IsEmailVerified = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new ApiResponse<AuthResponse>
        {
            Success = true,
            Message = "Registration successful",
            Data = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Token = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600
            }
        };
    }

    public async Task<ApiResponse<AuthResponse>> AdminLoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == UserRole.Admin);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Invalid admin credentials"
            };
        }

        var token = GenerateJwtToken(user);
        var permissions = GetPermissions(user.Role);

        return new ApiResponse<AuthResponse>
        {
            Success = true,
            Message = "Admin login successful",
            Data = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Avatar = user.Avatar,
                Token = token,
                ExpiresIn = 3600,
                Permissions = permissions
            }
        };
    }

    public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Invalid refresh token"
            };
        }

        var newToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new ApiResponse<AuthResponse>
        {
            Success = true,
            Data = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = 3600
            }
        };
    }

    public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Email not found"
            };
        }

        // In production, send email with reset link
        return new ApiResponse<string>
        {
            Success = true,
            Message = "Password reset link has been sent to your email"
        };
    }

    public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // In production, validate token from email
        return new ApiResponse<string>
        {
            Success = true,
            Message = "Password has been reset successfully"
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "KarnelTravelsSecretKey2026!@#$%^&*()"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "KarnelTravels",
            audience: _configuration["Jwt:Audience"] ?? "KarnelTravels",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    private List<string> GetPermissions(UserRole role)
    {
        return role switch
        {
            UserRole.Admin => new List<string> { "ManageUsers", "ManageBookings", "ManageContent", "ViewReports" },
            UserRole.Moderator => new List<string> { "ManageContent", "ViewReports" },
            UserRole.Staff => new List<string> { "ManageBookings" },
            _ => new List<string>()
        };
    }

    public async Task<ApiResponse<string>> LogoutAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Invalid user"
            };
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            };
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _context.SaveChangesAsync();

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Logout successful"
        };
    }

    public async Task<ApiResponse<UserProfileDto>> GetCurrentUserAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
        {
            return new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = "Invalid user"
            };
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = "User not found"
            };
        }

        return new ApiResponse<UserProfileDto>
        {
            Success = true,
            Message = "Success",
            Data = new UserProfileDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<ApiResponse<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Invalid user"
            };
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            };
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Current password is incorrect"
            };
        }

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "New passwords do not match"
            };
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return new ApiResponse<string>
        {
            Success = true,
            Message = "Password changed successfully"
        };
    }
}
