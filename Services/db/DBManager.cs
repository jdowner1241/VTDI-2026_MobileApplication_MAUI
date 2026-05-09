using CommunityToolkit.Mvvm.ComponentModel;
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

        private readonly DBService _dbService;
        private SQLiteAsyncConnection Database => _dbService.Database;
        public bool IsInitialized { get; private set; }

        public DBManager(DBService dbService) 
        {
            _dbService = dbService;
            //_dbPath = Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db");
            //_database = new SQLiteAsyncConnection(_dbPath);
        }
        public async Task InitializeTableAsync()
        {
            EnsureInitialized();

            await Database.CreateTableAsync<T>();

            Debug.WriteLine($"[DBManager] Table created: {typeof(T).Name}");
        }


        //public async Task InitializeAsync()
        //{
        //    _dbPath = Path.Combine(FileSystem.AppDataDirectory, "mystic_todo.db");
        //    _database = new SQLiteAsyncConnection(_dbPath);
        //    var wasCreated = await _database.CreateTableAsync<T>();
        //    Debug.WriteLine($"Database : {typeof(T).Name}. Creation Status : {wasCreated}");

        //    IsInitialized = true;
        //}


        // Get DB path 
        //public string GetDBPath() => _dbPath  ?? string.Empty;

        private void EnsureInitialized()
        {
            if (_dbService == null)
                throw new InvalidOperationException("DBService not injected.");

            // Database property will throw if not ready anyway
        }


        // insert new record
        public async Task<int> InsertAsync(T item)
        {
            EnsureInitialized();
            var result = 0;

            if (Database != null)
            {
                Debug.WriteLine($"[DBManager] Inserting {typeof(T).Name}: {item}");
                result = await Database.InsertAsync(item);
                Debug.WriteLine($"[DBManager] Insert result: {result}");
            }
    
            return result;
        }

        // Update existing record
        public async Task<int> UpdateAsync(T item)
        {
            EnsureInitialized();
            var result = 0;

            if (Database != null)
            {
                Debug.WriteLine($"[DBManager] Updating {typeof(T).Name}: {item}");
                result = await Database.UpdateAsync(item);
                Debug.WriteLine($"[DBManager] Update result: {result}");
            }

            return result;
        }

        // Convenience method: decide insert vs update
        public async Task<int> SaveAsync(T item, int id)
        {
            EnsureInitialized();
            Debug.WriteLine($"[DBManager] Save called for {typeof(T).Name} with Id={id}");

            var existing = await Database.FindAsync<T>(id);
            if (existing == null)
            {
                Debug.WriteLine($"[DBManager] No existing record found. Inserting new.");
                return await Database.InsertAsync(item);
                
            }
            else
            {
                Debug.WriteLine($"[DBManager] Existing record found. Updating.");
                return await Database.UpdateAsync(item);
                
            }
        }


        // Read All 
        public Task<List<T>> GetAllAsync()
        {
            EnsureInitialized();
            return Database.Table<T>().ToListAsync();
 
        }

        // Read by ID
        public Task<T> GetByIdAsync(int id)
        {
            EnsureInitialized();
            return Database.FindAsync<T>(id);
        }

        // Delete 
        public async Task<int> DeleteAsync(T item)
        {
            EnsureInitialized();
            var result = 0;

            if (Database != null) 
            {
                Debug.WriteLine($"[DBManager] Deleting {typeof(T).Name}: {item}");
                result = await Database.DeleteAsync(item);
                Debug.WriteLine($"[DBManager] Delete result: {result}");
            }
            return result; 
        }

      

        // Seed default value 
        public async Task SeedDefaultsAsync(IEnumerable<T> defaultItems)
        {
            EnsureInitialized();


            if (Database != null) 
            {
                var existing = await Database.Table<T>().CountAsync();
                if (existing == 0)
                {
                    await Database.InsertAllAsync(defaultItems);
                    Debug.WriteLine($"[DBManager] Seeded {typeof(T).Name} with defaults.");
                }
            }    
        }

    }
   // Closer for Class

}
// Closer for Namespace
