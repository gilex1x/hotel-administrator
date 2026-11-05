using System;
using SQLite;

namespace hotel_admin_api.Models
{
    [Table("Hotels")]
    public class Hotel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<int> Reviews { get; set; } = new List<int>();

        //Add average review score property that calculates the average review score based on the reviews list.

    }
}
