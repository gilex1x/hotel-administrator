using hotel_admin_api.Models;
using SQLite;

namespace hotel_admin_api.Repositories;

public sealed class SqliteUserRepository : IUserRepository
{
    private readonly SQLiteConnection _database;

    public SqliteUserRepository(SQLiteConnection database)
    {
        _database = database;
    }

    public User? FindByUsername(string username)
    {
        return _database.Table<User>().FirstOrDefault(user => user.Username == username);
    }
}
