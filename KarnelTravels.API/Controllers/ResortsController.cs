using System.Text.Json;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResortsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public ResortsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ResortDto>>>> GetResorts(
        [FromQuery] string? search,
        [FromQuery] string? location,
        [FromQuery] int? starRating,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Resorts.Where(r => r.IsActive).AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.Name.Contains(search) || r.City.Contains(search));

        if (!string.IsNullOrEmpty(location))
            query = query.Where(r => r.LocationType == location);

        if (starRating.HasValue)
            query = query.Where(r => r.StarRating == starRating.Value);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = items.Select(r => new ResortDto
        {
            ResortId = r.Id,
            Name = r.Name,
            Description = r.Description,
            Address = r.Address,
            City = r.City,
            LocationType = r.LocationType,
            StarRating = r.StarRating,
            Images = string.IsNullOrEmpty(r.Images) ? null : JsonSerializer.Deserialize<List<string>>(r.Images),
            MinPrice = r.MinPrice,
            MaxPrice = r.MaxPrice,
            Amenities = string.IsNullOrEmpty(r.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(r.Amenities),
            Rating = r.Rating,
            ReviewCount = r.ReviewCount,
            IsFeatured = r.IsFeatured
        }).ToList();

        return Ok(new ApiResponse<List<ResortDto>>
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
    public async Task<ActionResult<ApiResponse<ResortDto>>> GetResort(Guid id)
    {
        var resort = await _context.Resorts.FindAsync(id);

        if (resort == null || !resort.IsActive)
        {
            return NotFound(new ApiResponse<ResortDto>
            {
                Success = false,
                Message = "Resort not found"
            });
        }

        return Ok(new ApiResponse<ResortDto>
        {
            Success = true,
            Data = new ResortDto
            {
                ResortId = resort.Id,
                Name = resort.Name,
                Description = resort.Description,
                Address = resort.Address,
                City = resort.City,
                LocationType = resort.LocationType,
                StarRating = resort.StarRating,
                Images = string.IsNullOrEmpty(resort.Images) ? null : JsonSerializer.Deserialize<List<string>>(resort.Images),
                MinPrice = resort.MinPrice,
                MaxPrice = resort.MaxPrice,
                Amenities = string.IsNullOrEmpty(resort.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(resort.Amenities),
                Rating = resort.Rating,
                ReviewCount = resort.ReviewCount,
                IsFeatured = resort.IsFeatured
            }
        });
    }

    [HttpGet("{id}/rooms")]
    public async Task<ActionResult<ApiResponse<List<HotelRoomDto>>>> GetResortRooms(Guid id)
    {
        var rooms = await _context.ResortRooms.Where(r => r.ResortId == id && r.IsActive).ToListAsync();

        var result = rooms.Select(r => new HotelRoomDto
        {
            RoomId = r.Id,
            RoomType = r.RoomType,
            Description = r.Description,
            MaxOccupancy = r.MaxOccupancy,
            PricePerNight = r.PricePerNight,
            BedType = r.BedType,
            RoomAmenities = string.IsNullOrEmpty(r.RoomAmenities) ? null : JsonSerializer.Deserialize<List<string>>(r.RoomAmenities),
            Images = string.IsNullOrEmpty(r.Images) ? null : JsonSerializer.Deserialize<List<string>>(r.Images),
            TotalRooms = r.TotalRooms,
            AvailableRooms = r.AvailableRooms
        }).ToList();

        return Ok(new ApiResponse<List<HotelRoomDto>>
        {
            Success = true,
            Data = result
        });
    }
}
