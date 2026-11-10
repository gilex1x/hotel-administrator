using System.ComponentModel.DataAnnotations;

namespace hotel_admin_api.DTOs;

public sealed class HotelWriteRequest : IValidatableObject
{
    private const int MaxImageBytes = 400 * 1024;
    private const string JpegDataUriPrefix = "data:image/jpeg;base64,";

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(240)]
    public string Address { get; set; } = string.Empty;

    [Required]
    public List<int> Reviews { get; set; } = [];

    public string? ImageDataUri { get; set; }

    public bool RemoveImage { get; set; }

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

        if (ImageDataUri is not null)
        {
            var maxEncodedLength = JpegDataUriPrefix.Length + ((MaxImageBytes + 2) / 3 * 4);
            if (ImageDataUri.Length > maxEncodedLength ||
                !ImageDataUri.StartsWith(JpegDataUriPrefix, StringComparison.Ordinal))
            {
                yield return new ValidationResult(
                    "The image must be a JPEG of 400 KB or less.",
                    [nameof(ImageDataUri)]);
                yield break;
            }

            byte[]? imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(ImageDataUri[JpegDataUriPrefix.Length..]);
            }
            catch (FormatException)
            {
                imageBytes = null;
            }

            if (imageBytes is null)
            {
                yield return new ValidationResult("The image data is invalid.", [nameof(ImageDataUri)]);
                yield break;
            }

            if (imageBytes.Length > MaxImageBytes ||
                imageBytes.Length < 3 ||
                imageBytes[0] != 0xFF ||
                imageBytes[1] != 0xD8 ||
                imageBytes[2] != 0xFF)
            {
                yield return new ValidationResult(
                    "The image must be a valid JPEG of 400 KB or less.",
                    [nameof(ImageDataUri)]);
            }
        }

        if (ImageDataUri is not null && RemoveImage)
        {
            yield return new ValidationResult(
                "An image cannot be uploaded and removed at the same time.",
                [nameof(ImageDataUri), nameof(RemoveImage)]);
        }
    }
}
