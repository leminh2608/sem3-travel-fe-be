using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarnelTravels.API.Entities;

public class Restaurant : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    public string CuisineType { get; set; } = string.Empty; // Vietnamese, Chinese, Japanese, Korean, Italian, Seafood

    public string PriceLevel { get; set; } = "Budget"; // Budget, MidRange, HighEnd

    public string Style { get; set; } = "Restaurant"; // Restaurant, Cafe, Bar

    [MaxLength(50)]
    public string? OpeningTime { get; set; }

    [MaxLength(50)]
    public string? ClosingTime { get; set; }

    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    public string? Images { get; set; } // JSON array

    public string? Menu { get; set; } // JSON array of menu items

    public bool HasReservation { get; set; } = true;

    public double Rating { get; set; } = 0;

    public int ReviewCount { get; set; } = 0;

    public bool IsFeatured { get; set; } = false;

    // Navigation properties
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
