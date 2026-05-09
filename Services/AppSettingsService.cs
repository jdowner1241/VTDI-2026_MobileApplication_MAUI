using Mystic_ToDo_MAUI_.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Mystic_ToDo_MAUI_.Services
{
    public class AppSettingsService
    {
        private readonly string _settingsPath;

        public AppSettingsService()
        {
            _settingsPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "appsettings.json");
        }

        public async Task<AppSettingsModel> LoadSettingsAsync()
        {
            try
            {
                if (!File.Exists(_settingsPath))
                {
                    var defaultSettings = new AppSettingsModel();

                    await SaveSettingsAsync(defaultSettings);

                    return defaultSettings;
                }

                var json = await File.ReadAllTextAsync(_settingsPath);

                return JsonSerializer.Deserialize<AppSettingsModel>(json)
                       ?? new AppSettingsModel();
            }
            catch
            {
                return new AppSettingsModel();
            }
        }

        public async Task SaveSettingsAsync(AppSettingsModel settings)
        {
            var json = JsonSerializer.Serialize(
                settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(_settingsPath, json);
        }
    }
}
