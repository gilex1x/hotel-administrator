using System.ComponentModel.DataAnnotations;

namespace hotel_admin_api.DTOs;

public sealed class HotelWriteRequest : IValidatableObject
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(240)]
    public string Address { get; set; } = string.Empty;

    [Required]
    public List<int> Reviews { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            yield return new ValidationResult("Name is required.", [nameof(Name)]);
        }

        if (string.IsNullOrWhiteSpace(Address))
        {
            yield return new ValidationResult("Address is required.", [nameof(Address)]);
        }

        if (Reviews is null)
        {
            yield return new ValidationResult("Reviews are required.", [nameof(Reviews)]);
            yield break;
        }

        if (Reviews.Any(review => review is < 1 or > 5))
        {
            yield return new ValidationResult("Each review must be between 1 and 5.", [nameof(Reviews)]);
        }
    }
}
