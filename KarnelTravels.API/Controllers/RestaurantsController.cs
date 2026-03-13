using System.Text.Json;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public RestaurantsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RestaurantDto>>>> GetRestaurants(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] string? cuisineType,
        [FromQuery] string? priceLevel,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Restaurants.Where(r => r.IsActive).AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.Name.Contains(search) || r.City.Contains(search));

        if (!string.IsNullOrEmpty(city))
            query = query.Where(r => r.City == city);

        if (!string.IsNullOrEmpty(cuisineType))
            query = query.Where(r => r.CuisineType == cuisineType);

        if (!string.IsNullOrEmpty(priceLevel))
            query = query.Where(r => r.PriceLevel == priceLevel);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = items.Select(r => new RestaurantDto
        {
            RestaurantId = r.Id,
            Name = r.Name,
            Description = r.Description,
            Address = r.Address,
            City = r.City,
            CuisineType = r.CuisineType,
            PriceLevel = r.PriceLevel,
            Style = r.Style,
            OpeningTime = r.OpeningTime,
            ClosingTime = r.ClosingTime,
            ContactPhone = r.ContactPhone,
            Images = string.IsNullOrEmpty(r.Images) ? null : JsonSerializer.Deserialize<List<string>>(r.Images),
            HasReservation = r.HasReservation,
            Rating = r.Rating,
            ReviewCount = r.ReviewCount,
            IsFeatured = r.IsFeatured
        }).ToList();

        return Ok(new ApiResponse<List<RestaurantDto>>
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
    public async Task<ActionResult<ApiResponse<RestaurantDto>>> GetRestaurant(Guid id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);

        if (restaurant == null || !restaurant.IsActive)
        {
            return NotFound(new ApiResponse<RestaurantDto>
            {
                Success = false,
                Message = "Restaurant not found"
            });
        }

        return Ok(new ApiResponse<RestaurantDto>
        {
            Success = true,
            Data = new RestaurantDto
            {
                RestaurantId = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Address = restaurant.Address,
                City = restaurant.City,
                CuisineType = restaurant.CuisineType,
                PriceLevel = restaurant.PriceLevel,
                Style = restaurant.Style,
                OpeningTime = restaurant.OpeningTime,
                ClosingTime = restaurant.ClosingTime,
                ContactPhone = restaurant.ContactPhone,
                Images = string.IsNullOrEmpty(restaurant.Images) ? null : JsonSerializer.Deserialize<List<string>>(restaurant.Images),
                HasReservation = restaurant.HasReservation,
                Rating = restaurant.Rating,
                ReviewCount = restaurant.ReviewCount,
                IsFeatured = restaurant.IsFeatured
            }
        });
    }
}
