using System.ComponentModel.DataAnnotations;

namespace hotel_admin_api.DTOs;

public sealed class HotelListQuery : IValidatableObject
{
    [StringLength(120)]
    public string? Name { get; set; }

    [Range(1, 5)]
    public double? Rating { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Rating.HasValue && !double.IsFinite(Rating.Value))
        {
            yield return new ValidationResult("Rating must be a finite number.", [nameof(Rating)]);
        }
    }
}
