using hotel_admin_api.Models;
using SQLite;

namespace hotel_admin_api.Repositories;

public sealed class SqliteHotelRepository : IHotelRepository
{
    private readonly SQLiteConnection _database;

    public SqliteHotelRepository(SQLiteConnection database)
    {
        _database = database;
    }

    public IReadOnlyList<Hotel> GetAll()
    {
        return _database.Table<Hotel>().ToList();
    }

    public Hotel? GetById(int id)
    {
        return _database.Find<Hotel>(id);
    }

    public Hotel Add(Hotel hotel)
    {
        _database.Insert(hotel);
        return hotel;
    }

    public bool Update(Hotel hotel)
    {
        return _database.Update(hotel) > 0;
    }

    public bool Delete(int id)
    {
        return _database.Delete<Hotel>(id) > 0;
    }
}
