using SQLite;
using Okada.Models;

namespace Okada.Services;

public class SQLiteService
{
    private SQLiteAsyncConnection _db;

    public async Task InitAsync()
    {
        if (_db != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "okada.db");
        _db = new SQLiteAsyncConnection(dbPath);

        await _db.CreateTableAsync<User>();
        await _db.CreateTableAsync<Trip>();
        await _db.CreateTableAsync<Earning>();
    }

    public async Task AddUserAsync(User user)
    {
        await InitAsync();
        await _db.InsertOrReplaceAsync(user);
    }

    public async Task<User> GetUserAsync(string phoneNumber)
    {
        await InitAsync();
        return await _db.Table<User>().Where(u => u.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetUsersAsync()
    {
        await InitAsync();
        return await _db.Table<User>().ToListAsync();
    }

    public async Task DeleteAllUsersAsync()
    {
        await InitAsync();
        await _db.DeleteAllAsync<User>();
    }

    public async Task AddTripAsync(Trip trip)
    {
        await InitAsync();
        await _db.InsertAsync(trip);
    }

    public async Task<List<Trip>> GetTripsAsync(string phoneNumber)
    {
        await InitAsync();
        return await _db.Table<Trip>().Where(t => t.PhoneNumber == phoneNumber).OrderByDescending(t => t.Date).ToListAsync();
    }

    public async Task AddEarningAsync(Earning earning)
    {
        await InitAsync();
        await _db.InsertAsync(earning);
    }

    public async Task<List<Earning>> GetEarningsAsync(string phoneNumber)
    {
        await InitAsync();
        return await _db.Table<Earning>().Where(e => e.PhoneNumber == phoneNumber).OrderByDescending(e => e.Date).ToListAsync();
    }

}
