using System.Text.Json;
using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransportsController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public TransportsController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<TransportDto>>>> GetTransports(
        [FromQuery] string? type,
        [FromQuery] string? fromCity,
        [FromQuery] string? toCity,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Transports.Where(t => t.IsActive).AsQueryable();

        if (!string.IsNullOrEmpty(type))
            query = query.Where(t => t.Type == type);

        if (!string.IsNullOrEmpty(fromCity))
            query = query.Where(t => t.FromCity == fromCity);

        if (!string.IsNullOrEmpty(toCity))
            query = query.Where(t => t.ToCity == toCity);

        if (minPrice.HasValue)
            query = query.Where(t => t.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(t => t.Price <= maxPrice.Value);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = items.Select(t => new TransportDto
        {
            TransportId = t.Id,
            Type = t.Type,
            Provider = t.Provider,
            FromCity = t.FromCity,
            ToCity = t.ToCity,
            Route = t.Route,
            DepartureTime = t.DepartureTime.HasValue ? t.DepartureTime.Value.ToString(@"hh\:mm") : null,
            ArrivalTime = t.ArrivalTime.HasValue ? t.ArrivalTime.Value.ToString(@"hh\:mm") : null,
            DurationMinutes = t.DurationMinutes,
            Price = t.Price,
            AvailableSeats = t.AvailableSeats,
            Amenities = string.IsNullOrEmpty(t.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(t.Amenities),
            Images = string.IsNullOrEmpty(t.Images) ? null : JsonSerializer.Deserialize<List<string>>(t.Images),
            IsFeatured = t.IsFeatured
        }).ToList();

        return Ok(new ApiResponse<List<TransportDto>>
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
    public async Task<ActionResult<ApiResponse<TransportDto>>> GetTransport(Guid id)
    {
        var transport = await _context.Transports.FindAsync(id);

        if (transport == null || !transport.IsActive)
        {
            return NotFound(new ApiResponse<TransportDto>
            {
                Success = false,
                Message = "Transport not found"
            });
        }

        return Ok(new ApiResponse<TransportDto>
        {
            Success = true,
            Data = new TransportDto
            {
                TransportId = transport.Id,
                Type = transport.Type,
                Provider = transport.Provider,
                FromCity = transport.FromCity,
                ToCity = transport.ToCity,
                Route = transport.Route,
                DepartureTime = transport.DepartureTime.HasValue ? transport.DepartureTime.Value.ToString(@"hh\:mm") : null,
                ArrivalTime = transport.ArrivalTime.HasValue ? transport.ArrivalTime.Value.ToString(@"hh\:mm") : null,
                DurationMinutes = transport.DurationMinutes,
                Price = transport.Price,
                AvailableSeats = transport.AvailableSeats,
                Amenities = string.IsNullOrEmpty(transport.Amenities) ? null : JsonSerializer.Deserialize<List<string>>(transport.Amenities),
                Images = string.IsNullOrEmpty(transport.Images) ? null : JsonSerializer.Deserialize<List<string>>(transport.Images),
                IsFeatured = transport.IsFeatured
            }
        });
    }
}
