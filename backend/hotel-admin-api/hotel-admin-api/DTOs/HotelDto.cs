namespace hotel_admin_api.DTOs;

public sealed record HotelDto(
    int Id,
    string Name,
    string Address,
    IReadOnlyList<int> Reviews,
    double AverageReviewScore,
    string? ImageUrl);
