using System;
using System.Text.Json;
using SQLite;

namespace hotel_admin_api.Models
{
    [Table("Hotels")]
    public class Hotel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ImageDataUri { get; set; }

        [Ignore]
        public List<int> Reviews { get; set; } = new List<int>();

        [Column("Reviews")]
        [System.Text.Json.Serialization.JsonIgnore]
        public string ReviewsJson
        {
            get => JsonSerializer.Serialize(Reviews);
            set => Reviews = string.IsNullOrWhiteSpace(value)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(value) ?? new List<int>();
        }

        [Ignore]
        public double AverageReviewScore
        {
            get
            {
                if (Reviews.Count == 0) return 0;
                return Reviews.Average();
            }
        }
    }
}
