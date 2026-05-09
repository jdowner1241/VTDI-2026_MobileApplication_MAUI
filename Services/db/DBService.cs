using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services.db
{
    public class DBService
    {
        private SQLiteAsyncConnection? _database;

        public SQLiteAsyncConnection Database =>
            _database ?? throw new InvalidOperationException(
                "Database not initialized.");

        public async Task InitializeAsync()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "mystic_todo.db");

            _database = new SQLiteAsyncConnection(dbPath);

            await Task.CompletedTask;
        }

        public async Task CloseAsync()
        {
            if (_database != null)
            {
                await _database.CloseAsync();

                _database = null;
            }
        }
    }
}
