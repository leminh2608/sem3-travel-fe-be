using System.Security.Claims;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public AdminController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    #region User Management

    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<List<UserProfileDto>>>> GetAllUsers([FromQuery] PagedRequest request)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(u => u.FullName.Contains(request.Search) || u.Email.Contains(request.Search));
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserProfileDto
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Avatar = u.Avatar,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                IsEmailVerified = u.IsEmailVerified,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<UserProfileDto>>
        {
            Success = true,
            Data = users,
            Pagination = new PaginationInfo
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = totalCount
            }
        });
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetUserById(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
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

    [HttpPut("users/{id}/lock")]
    public async Task<ActionResult<ApiResponse<string>>> LockUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            });
        }

        user.IsLocked = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "User has been locked"
        });
    }

    [HttpPut("users/{id}/unlock")]
    public async Task<ActionResult<ApiResponse<string>>> UnlockUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            });
        }

        user.IsLocked = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "User has been unlocked"
        });
    }

    [HttpPut("users/{id}/role")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateUserRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            });
        }

        if (!Enum.TryParse<UserRole>(request.Role, true, out var newRole))
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "Invalid role"
            });
        }

        user.Role = newRole;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = $"User role updated to {newRole}"
        });
    }

    [HttpDelete("users/{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "User not found"
            });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "User deleted successfully"
        });
    }

    #endregion

    #region Dashboard & Reports

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResponse<DashboardDto>>> GetDashboard()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalBookings = await _context.Bookings.CountAsync();
        var totalRevenue = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed)
            .SumAsync(b => b.FinalAmount);

        var recentBookings = await _context.Bookings
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .Select(b => new BookingSummaryDto
            {
                BookingId = b.Id,
                CustomerName = b.User != null ? b.User.FullName : "N/A",
                TourName = b.TourPackage != null ? b.TourPackage.Name : "N/A",
                TotalPrice = b.FinalAmount,
                Status = b.Status.ToString(),
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        var newUsersThisMonth = await _context.Users
            .CountAsync(u => u.CreatedAt.Month == DateTime.UtcNow.Month && u.CreatedAt.Year == DateTime.UtcNow.Year);

        return Ok(new ApiResponse<DashboardDto>
        {
            Success = true,
            Data = new DashboardDto
            {
                TotalUsers = totalUsers,
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                NewUsersThisMonth = newUsersThisMonth,
                RecentBookings = recentBookings
            }
        });
    }

    #endregion

    #region Content Management

    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<StatisticsDto>>> GetStatistics()
    {
        var stats = new StatisticsDto
        {
            TotalTours = await _context.TourPackages.CountAsync(),
            TotalTouristSpots = await _context.TouristSpots.CountAsync(),
            TotalHotels = await _context.Hotels.CountAsync(),
            TotalRestaurants = await _context.Restaurants.CountAsync(),
            TotalResorts = await _context.Resorts.CountAsync(),
            TotalTransports = await _context.Transports.CountAsync()
        };

        return Ok(new ApiResponse<StatisticsDto>
        {
            Success = true,
            Data = stats
        });
    }

    #endregion

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? Guid.Empty.ToString());
    }
}

// Additional DTOs for Admin
public class UpdateRoleRequest
{
    public string Role { get; set; } = string.Empty;
}

public class DashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public int NewUsersThisMonth { get; set; }
    public List<BookingSummaryDto> RecentBookings { get; set; } = new();
}

public class BookingSummaryDto
{
    public Guid BookingId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string TourName { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class StatisticsDto
{
    public int TotalTours { get; set; }
    public int TotalTouristSpots { get; set; }
    public int TotalHotels { get; set; }
    public int TotalRestaurants { get; set; }
    public int TotalResorts { get; set; }
    public int TotalTransports { get; set; }
}
