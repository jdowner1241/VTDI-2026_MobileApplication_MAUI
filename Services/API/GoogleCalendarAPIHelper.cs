using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Mystic_ToDo_MAUI_.Services.API
{
    public class GoogleCalendarAPIHelper
    {
        private CalendarService _calendarService;

        public GoogleCalendarAPIHelper()
        {
        }

        public async Task<CalendarService> GetCalendarServiceAsync()
        {
            if (_calendarService != null)
                return _calendarService;

            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("credentials.json");

                // Use FromStream instead of the nonexistent FromJson
                var secrets = GoogleClientSecrets.FromStream(stream).Secrets;

                var credPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    ".credentials/calendar.json");

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                     secrets,
                     new[] { CalendarService.Scope.Calendar },
                     Preferences.Get("GoogleUserEmail", "default"),
                     CancellationToken.None,
                     new FileDataStore(credPath, true));

                _calendarService = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Mystic_ToDo_MAUI",
                });
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"OAuth transport warning: {ex.Message}");
            }

            if (_calendarService == null)
            {
                throw new InvalidOperationException("Google Calendar service failed to initialize.");
            }
            return _calendarService;
        }
    }
}

