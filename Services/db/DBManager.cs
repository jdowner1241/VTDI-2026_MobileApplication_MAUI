using Mystic_ToDo_MAUI_.Model.db.tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services.db
{
    public class DBManager<T> where T : new()
    {

        private string defaultDBPath = Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db");
        private readonly SQLiteAsyncConnection _database;

        public DBManager() 
        {
            _database = new SQLiteAsyncConnection(defaultDBPath);
            _database.CreateTableAsync<T>().Wait();  // Creates the table for type T if it doesn't exist
        }

        // Get DB path 
        public string GetDBPath() => defaultDBPath;

        // Create or Update
        public Task<int> SaveAsync(T item)
        {
            return _database.InsertOrReplaceAsync(item);
        }

        // Read All 
        public Task<List<T>> GetAllAsync()
        {
            return _database.Table<T>().ToListAsync();
        }

        // Read by ID
        public Task<T> GetByIdAsync(int id)
        {
            return _database.FindAsync<T>(id);
        }

        // Delete 
        public Task<int> DeleteAsync(T item)
        {
            return _database.DeleteAsync(item);
        }


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
