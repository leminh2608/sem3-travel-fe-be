using System.Security.Claims;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public UsersController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
    {
        var userId = GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = "User not found"
            });
        }

        return Ok(new ApiResponse<UserProfileDto>
        {
            Success = true,
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
        });
    }

    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new ApiResponse<UserProfileDto>
            {
                Success = false,
                Message = "User not found"
            });
        }

        if (!string.IsNullOrEmpty(request.FullName)) user.FullName = request.FullName;
        if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
        if (request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth;
        if (request.Gender != null) user.Gender = request.Gender;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<UserProfileDto>
        {
            Success = true,
            Message = "Profile updated successfully",
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
        });
    }

    [HttpGet("addresses")]
    public async Task<ActionResult<ApiResponse<List<AddressDto>>>> GetAddresses()
    {
        var userId = GetUserId();
        var addresses = await _context.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync();

        var result = addresses.Select(a => new AddressDto
        {
            AddressId = a.Id,
            AddressLine = a.AddressLine,
            Ward = a.Ward,
            District = a.District,
            City = a.City,
            Country = a.Country,
            IsDefault = a.IsDefault
        }).ToList();

        return Ok(new ApiResponse<List<AddressDto>>
        {
            Success = true,
            Data = result
        });
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<ApiResponse<AddressDto>>> CreateAddress([FromBody] CreateAddressRequest request)
    {
        var userId = GetUserId();

        if (request.IsDefault)
        {
            var existingDefaults = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();

            foreach (var addr in existingDefaults)
            {
                addr.IsDefault = false;
            }
        }

        var address = new Address
        {
            UserId = userId,
            AddressLine = request.AddressLine,
            Ward = request.Ward,
            District = request.District,
            City = request.City,
            Country = request.Country,
            IsDefault = request.IsDefault
        };

        _context.Addresses.Add(address);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<AddressDto>
        {
            Success = true,
            Message = "Address created successfully",
            Data = new AddressDto
            {
                AddressId = address.Id,
                AddressLine = address.AddressLine,
                Ward = address.Ward,
                District = address.District,
                City = address.City,
                Country = address.Country,
                IsDefault = address.IsDefault
            }
        });
    }

    [HttpDelete("addresses/{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteAddress(Guid id)
    {
        var userId = GetUserId();
        var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (address == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "Address not found"
            });
        }

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Address deleted successfully"
        });
    }

    [HttpPut("change-password")]
    public async Task<ActionResult<ApiResponse<string>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetUserId();
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            });
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "Current password is incorrect"
            });
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Password changed successfully"
        });
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? Guid.Empty.ToString());
    }
}
