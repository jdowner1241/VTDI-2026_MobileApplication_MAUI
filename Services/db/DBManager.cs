using Mystic_ToDo_MAUI_.Model.db.tables;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services.db
{
    public class DBManager<T> where T : new()
    {

        private SQLiteAsyncConnection? _database;
        private string? _dbPath;

        public DBManager() 
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db");
            _database = new SQLiteAsyncConnection(_dbPath);
        }

        public async Task InitializeAsync()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db");
            _database = new SQLiteAsyncConnection(_dbPath);
            var wasCreated = await _database.CreateTableAsync<T>();
            Debug.WriteLine($"Database : {typeof(T).Name}. Creation Status : {wasCreated}");
        }


        // Get DB path 
        public string GetDBPath() => _dbPath  ?? string.Empty;

        private void EnsureInitialized()
        {
            if (_database == null)
                throw new InvalidOperationException("Database not initialized. Call InitializeAsync() first.");
        }


        // insert new record
        public async Task<int> InsertAsync(T item)
        {
            EnsureInitialized();
            var result = 0;

            if (_database != null)
            {
                Debug.WriteLine($"[DBManager] Inserting {typeof(T).Name}: {item}");
                result = await _database.InsertAsync(item);
                Debug.WriteLine($"[DBManager] Insert result: {result}");
            }
    
            return result;
        }

        // Update existing record
        public async Task<int> UpdateAsync(T item)
        {
            EnsureInitialized();
            var result = 0;

            if (_database != null)
            {
                Debug.WriteLine($"[DBManager] Updating {typeof(T).Name}: {item}");
                result = await _database.UpdateAsync(item);
                Debug.WriteLine($"[DBManager] Update result: {result}");
            }

            return result;
        }

        // Convenience method: decide insert vs update
        public async Task<int> SaveAsync(T item, int id)
        {
            EnsureInitialized();
            Debug.WriteLine($"[DBManager] Save called for {typeof(T).Name} with Id={id}");

            var existing = await _database.FindAsync<T>(id);
            if (existing == null)
            {
                Debug.WriteLine($"[DBManager] No existing record found. Inserting new.");
                return await _database.InsertAsync(item);
                
            }
            else
            {
                Debug.WriteLine($"[DBManager] Existing record found. Updating.");
                return await _database.UpdateAsync(item);
                
            }
        }


        // Read All 
        public Task<List<T>> GetAllAsync()
        {
            EnsureInitialized();
            return _database.Table<T>().ToListAsync();
 
        }

        // Read by ID
        public Task<T> GetByIdAsync(int id)
        {
            EnsureInitialized();
            return _database.FindAsync<T>(id);
        }

        // Delete 
        public async Task<int> DeleteAsync(T item)
        {
            EnsureInitialized();
            var result = 0;

            if (_database != null) 
            {
                Debug.WriteLine($"[DBManager] Deleting {typeof(T).Name}: {item}");
                result = await _database.DeleteAsync(item);
                Debug.WriteLine($"[DBManager] Delete result: {result}");
            }
            return result; 
        }

        //public Task<int> SaveAsync(T item) => _database!.InsertOrReplaceAsync(item);
        //public Task<List<T>> GetAllAsync() => _database!.Table<T>().ToListAsync();
        //public Task<T> GetByIdAsync(int id) => _database!.FindAsync<T>(id);
        //public Task<int> DeleteAsync(T item) => _database!.DeleteAsync(item);


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
            EnsureInitialized();


            if (_database != null) 
            {
                var existing = await _database.Table<T>().CountAsync();
                if (existing == 0)
                {
                    await _database.InsertAllAsync(defaultItems);
                }
            }    
        }

    }
   // Closer for Class

}
// Closer for Namespace
