using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KarnelTravels.API.Entities;

public class Review : BaseEntity
{
    public int Rating { get; set; } // 1-5

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(2000)]
    public string? Content { get; set; }

    public string? Images { get; set; } // JSON array

    public ReviewType Type { get; set; }

    // Foreign keys
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    public Guid? TouristSpotId { get; set; }

    [ForeignKey("TouristSpotId")]
    public virtual TouristSpot? TouristSpot { get; set; }

    public Guid? HotelId { get; set; }

    [ForeignKey("HotelId")]
    public virtual Hotel? Hotel { get; set; }

    public Guid? RestaurantId { get; set; }

    [ForeignKey("RestaurantId")]
    public virtual Restaurant? Restaurant { get; set; }

    public Guid? ResortId { get; set; }

    [ForeignKey("ResortId")]
    public virtual Resort? Resort { get; set; }

    public Guid? TourPackageId { get; set; }

    [ForeignKey("TourPackageId")]
    public virtual TourPackage? TourPackage { get; set; }

    public Guid? BookingId { get; set; }

    [ForeignKey("BookingId")]
    public virtual Booking? Booking { get; set; }
}

public enum ReviewType
{
    TouristSpot = 0,
    Hotel = 1,
    Restaurant = 2,
    Resort = 3,
    Tour = 4
}
