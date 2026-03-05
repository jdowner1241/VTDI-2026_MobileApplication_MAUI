using Mystic_ToDo_MAUI_.Model.db.tables;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services.db
{
    public class DBManager<T> where T : new()
    {

        private SQLiteAsyncConnection? _database;
        private string? _dbPath;

        public DBManager() 
        {
            
        }

        public async Task InitializeAsync()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db");
            _database = new SQLiteAsyncConnection(_dbPath);
            await _database.CreateTableAsync<T>();
        }


        // Get DB path 
        public string GetDBPath() => _dbPath  ?? string.Empty;

        //// Create or Update
        //public Task<int> SaveAsync(T item)
        //{
        //    return _database.InsertOrReplaceAsync(item);
        //}

        //// Read All 
        //public Task<List<T>> GetAllAsync()
        //{
        //    return _database.Table<T>().ToListAsync();
        //}

        //// Read by ID
        //public Task<T> GetByIdAsync(int id)
        //{
        //    return _database.FindAsync<T>(id);
        //}

        //// Delete 
        //public Task<int> DeleteAsync(T item)
        //{
        //    return _database.DeleteAsync(item);
        //}

        public Task<int> SaveAsync(T item) => _database!.InsertOrReplaceAsync(item);
        public Task<List<T>> GetAllAsync() => _database!.Table<T>().ToListAsync();
        public Task<T> GetByIdAsync(int id) => _database!.FindAsync<T>(id);
        public Task<int> DeleteAsync(T item) => _database!.DeleteAsync(item);


        //// Intialize DB and create table if not exists
        //public async Task IntializeDatabase() 
        //{
        //    await _database.CreateTableAsync<TaskList_RepeatTag>();
        //    await   CreateTableAsync<Task>();
        //    await db.CreateTableAsync<Category>();

        //}


        // Seed default value 
        public async Task SeedDefaultsAsync(IEnumerable<T> defaultItems)
        {
            var existing = await _database.Table<T>().CountAsync();
            if (existing == 0)
            {
                await _database.InsertAllAsync(defaultItems);
            }
        }


    }
}
