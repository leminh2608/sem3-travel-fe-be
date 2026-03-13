using System.Text.Json;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TouristSpotsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public TouristSpotsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TouristSpotDto>>>> GetTouristSpots(
        [FromQuery] string? search,
        [FromQuery] string? region,
        [FromQuery] string? type,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder = "ASC",
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.TouristSpots.Where(s => s.IsActive).AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(s => s.Name.Contains(search) || s.City.Contains(search));

        if (!string.IsNullOrEmpty(region))
            query = query.Where(s => s.Region == region);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(s => s.Type == type);

        query = sortBy?.ToLower() switch
        {
            "name" => sortOrder == "DESC" ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            "rating" => sortOrder == "DESC" ? query.OrderByDescending(s => s.Rating) : query.OrderBy(s => s.Rating),
            _ => query.OrderBy(s => s.Name)
        };

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = items.Select(s => new TouristSpotDto
        {
            SpotId = s.Id,
            Name = s.Name,
            Description = s.Description,
            Region = s.Region,
            Type = s.Type,
            Address = s.Address,
            City = s.City,
            Latitude = s.Latitude,
            Longitude = s.Longitude,
            Images = string.IsNullOrEmpty(s.Images) ? null : JsonSerializer.Deserialize<List<string>>(s.Images),
            TicketPrice = s.TicketPrice,
            BestTime = s.BestTime,
            Rating = s.Rating,
            ReviewCount = s.ReviewCount,
            IsFeatured = s.IsFeatured,
            CreatedAt = s.CreatedAt
        }).ToList();

        return Ok(new ApiResponse<List<TouristSpotDto>>
        {
            Success = true,
            Data = result,
            Pagination = new PaginationInfo
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TouristSpotDto>>> GetTouristSpot(Guid id)
    {
        var spot = await _context.TouristSpots.FindAsync(id);

        if (spot == null || !spot.IsActive)
        {
            return NotFound(new ApiResponse<TouristSpotDto>
            {
                Success = false,
                Message = "Tourist spot not found"
            });
        }

        return Ok(new ApiResponse<TouristSpotDto>
        {
            Success = true,
            Data = new TouristSpotDto
            {
                SpotId = spot.Id,
                Name = spot.Name,
                Description = spot.Description,
                Region = spot.Region,
                Type = spot.Type,
                Address = spot.Address,
                City = spot.City,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                Images = string.IsNullOrEmpty(spot.Images) ? null : JsonSerializer.Deserialize<List<string>>(spot.Images),
                TicketPrice = spot.TicketPrice,
                BestTime = spot.BestTime,
                Rating = spot.Rating,
                ReviewCount = spot.ReviewCount,
                IsFeatured = spot.IsFeatured,
                CreatedAt = spot.CreatedAt
            }
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<ActionResult<ApiResponse<TouristSpotDto>>> CreateTouristSpot([FromBody] CreateTouristSpotRequest request)
    {
        var spot = new TouristSpot
        {
            Name = request.Name,
            Description = request.Description,
            Region = request.Region,
            Type = request.Type,
            Address = request.Address,
            City = request.City,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Images = request.Images != null ? JsonSerializer.Serialize(request.Images) : null,
            TicketPrice = request.TicketPrice,
            BestTime = request.BestTime,
            IsFeatured = request.IsFeatured,
            IsActive = true
        };

        _context.TouristSpots.Add(spot);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<TouristSpotDto>
        {
            Success = true,
            Message = "Tourist spot created successfully",
            Data = new TouristSpotDto
            {
                SpotId = spot.Id,
                Name = spot.Name,
                Description = spot.Description,
                Region = spot.Region,
                Type = spot.Type,
                Address = spot.Address,
                City = spot.City,
                Rating = spot.Rating,
                ReviewCount = spot.ReviewCount,
                IsFeatured = spot.IsFeatured,
                CreatedAt = spot.CreatedAt
            }
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<ActionResult<ApiResponse<TouristSpotDto>>> UpdateTouristSpot(Guid id, [FromBody] CreateTouristSpotRequest request)
    {
        var spot = await _context.TouristSpots.FindAsync(id);

        if (spot == null)
        {
            return NotFound(new ApiResponse<TouristSpotDto>
            {
                Success = false,
                Message = "Tourist spot not found"
            });
        }

        spot.Name = request.Name;
        spot.Description = request.Description;
        spot.Region = request.Region;
        spot.Type = request.Type;
        spot.Address = request.Address;
        spot.City = request.City;
        spot.Latitude = request.Latitude;
        spot.Longitude = request.Longitude;
        spot.Images = request.Images != null ? JsonSerializer.Serialize(request.Images) : null;
        spot.TicketPrice = request.TicketPrice;
        spot.BestTime = request.BestTime;
        spot.IsFeatured = request.IsFeatured;
        spot.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<TouristSpotDto>
        {
            Success = true,
            Message = "Tourist spot updated successfully",
            Data = new TouristSpotDto
            {
                SpotId = spot.Id,
                Name = spot.Name,
                Description = spot.Description,
                Region = spot.Region,
                Type = spot.Type,
                City = spot.City,
                Rating = spot.Rating,
                IsFeatured = spot.IsFeatured
            }
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteTouristSpot(Guid id)
    {
        var spot = await _context.TouristSpots.FindAsync(id);

        if (spot == null)
        {
            return NotFound(new ApiResponse<string>
            {
                Success = false,
                Message = "Tourist spot not found"
            });
        }

        spot.IsActive = false;
        spot.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Tourist spot deleted successfully"
        });
    }
}
