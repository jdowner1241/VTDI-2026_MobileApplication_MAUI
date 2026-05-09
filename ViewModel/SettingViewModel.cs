using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Calendar.v3;
using Mystic_ToDo_MAUI_.Model;
using Mystic_ToDo_MAUI_.Services;
using Mystic_ToDo_MAUI_.Services.API;
using Mystic_ToDo_MAUI_.Services.db;
using Mystic_ToDo_MAUI_.Services.ThemeHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Mystic_ToDo_MAUI_.ViewModel
{
    public partial class SettingViewModel : BaseViewModel
    {
        // -----------------------------
        // Repositories for database access
        // -----------------------------
        private readonly AppState _appState;
        private readonly DBService _dbService;
        private readonly DBInitializer _dbInitializer;
        private readonly AppSettingsService _settingsService;
        private readonly ThemeSwitcher _themeSwitcher;
        private readonly GoogleCalendarAPIHelper _googleHelper;
        private readonly GoogleTaskSyncService _syncService;

        private CancellationTokenSource _cts;

        // -----------------------------
        // properties and supporting structures
        // -----------------------------
        [ObservableProperty]
        private bool isDarkModeEnabled;
        partial void OnIsDarkModeEnabledChanged(bool value)
        {
            if (_isInitializing)
                return;

            if (value)
            {
                _themeSwitcher.SetTheme("DarkTheme");
            }
            else
            {
                _themeSwitcher.SetTheme("LightTheme");
            }

            _ = SaveSettingsAsync();
        }

        [ObservableProperty]
        private bool alarmNotificationsEnabled;
        partial void OnAlarmNotificationsEnabledChanged(bool value)
        {
            if (_isInitializing)
                return;

            _ = SaveSettingsAsync();
        }

        [ObservableProperty]
        private bool repeatNotificationsEnabled;
        partial void OnRepeatNotificationsEnabledChanged(bool value)
        {
            if (_isInitializing)
                return;

            _ = SaveSettingsAsync();
        }

        [ObservableProperty]
        private bool googleSyncEnabled;
        partial void OnGoogleSyncEnabledChanged(bool value)
        {
            if (_isInitializing)
                return;

            Preferences.Default.Set("GoogleSyncEnabled", value);
            _ = SaveSettingsAsync();
        }


        [ObservableProperty]
        private bool isGoogleConnected;

        [ObservableProperty]
        private bool isSyncingGoogle;

        [ObservableProperty]
        private string googleAccountEmail = "Not Connected";

        [ObservableProperty]
        private double syncProgress;

        [ObservableProperty]
        private string syncProgressText;

        // -----------------------------
        // Flags
        // -----------------------------
        private SemaphoreSlim _syncLock = new(1, 1);
        private bool _isInitializing;

        // -----------------------------
        // Constructor
        // -----------------------------

        public SettingViewModel
            (
                AppState appState,
                DBInitializer dbInitializer,
                DBService dbService,
                GoogleCalendarAPIHelper googleHelper,
                GoogleTaskSyncService syncService,
                AppSettingsService settingsService,
                ThemeSwitcher themeSwitcher
            )
        {
            _appState = appState;
            _dbInitializer = dbInitializer;
            _dbService = dbService;
            _googleHelper = googleHelper;
            _syncService = syncService;

            _settingsService = settingsService;
            _themeSwitcher = themeSwitcher;
        }

        // -----------------------------
        // Methods
        // -----------------------------

        [RelayCommand]
        private async Task ConnectGoogleAsync()
        {
            try
            {
                IsSyncingGoogle = true;

                var service = await _googleHelper.GetCalendarServiceAsync();

                if (service != null)
                {
                    var calendar = await service.Calendars.Get("primary").ExecuteAsync();

                    GoogleAccountEmail = calendar.Summary;

                    IsGoogleConnected = true;

                    GoogleSyncEnabled = true;

                    Preferences.Default.Set("GoogleSyncEnabled", true);

                    Preferences.Default.Set("GoogleAccountEmail", GoogleAccountEmail);

                    Debug.WriteLine($"Connected Google Account: {GoogleAccountEmail}");

                    await SaveSettingsAsync();

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Google connect failed: {ex.Message}");

                await Shell.Current.DisplayAlertAsync(
                    "Connection Error",
                    ex.Message,
                    "OK");
            }
            finally
            {
                IsSyncingGoogle = false;
            }
        }

        [RelayCommand]
        private async Task SyncNowAsync()
        {
            if (!GoogleSyncEnabled)
            {
                await Shell.Current.DisplayAlertAsync(
                    "Google Sync Disabled",
                    "Enable Google Sync first.",
                    "OK");

                return;
            }

            if (IsSyncingGoogle)
                return;

            try
            {
                IsSyncingGoogle = true;

                await LoadGoogleEventsAsync();

                await Shell.Current.DisplayAlertAsync(
                    "Sync Complete",
                    "Google Calendar synced successfully.",
                    "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sync failed: {ex.Message}");

                await Shell.Current.DisplayAlertAsync(
                    "Sync Error",
                    ex.Message,
                    "OK");
            }
            finally
            {
                IsSyncingGoogle = false;
            }
        }

        [RelayCommand]
        private async Task DisconnectGoogleAsync()
        {
            GoogleSyncEnabled = false;

            IsGoogleConnected = false;

            GoogleAccountEmail = "Not Connected";

            Preferences.Default.Remove("GoogleSyncEnabled");

            await Shell.Current.DisplayAlertAsync(
                "Disconnected",
                "Google Calendar disconnected.",
                "OK");

            await SaveSettingsAsync();
        }


        // Google API integration for Fetching Calendar Events
        public async Task LoadGoogleEventsAsync()
        {

            await _syncLock.WaitAsync();

            _cts = new CancellationTokenSource();

            try
            {
                var service = await _googleHelper.GetCalendarServiceAsync();

                var request = service.Events.List("primary");

                request.TimeMin = DateTime.Now;
                request.TimeMax = DateTime.Now.AddMonths(1);
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.OrderBy =
                    EventsResource.ListRequest.OrderByEnum.StartTime;

                var eventsResp = await request.ExecuteAsync(_cts.Token);

                if (eventsResp.Items != null)
                {
                    var total = eventsResp.Items.Count;
                    var current = 0;

                    foreach (var ev in eventsResp.Items)
                    {
                        _cts.Token.ThrowIfCancellationRequested();

                        await _syncService.SaveGoogleEventAsync(ev);

                        current++;

                        SyncProgress = (double)current / total;

                        SyncProgressText = $"{current} / {total} Synced";
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Sync cancelled.");
            }
            finally
            {
                IsSyncingGoogle = false;

                SyncProgress = 0;

                SyncProgressText = string.Empty;

                _syncLock.Release();
            }
        }


        [RelayCommand]
        private async Task BackupDatabaseAsync()
        {
            try
            {
                var dbPath = Path.Combine(
                    FileSystem.AppDataDirectory,
                    "mystic_todo.db");

                if (!File.Exists(dbPath))
                {
                    await Shell.Current.DisplayAlertAsync(
                        "Backup Failed",
                        "Database file not found.",
                        "OK");

                    return;
                }

                // Create backup folder
                var backupFolder = Path.Combine(
                    FileSystem.AppDataDirectory,
                    "Backups");

                Directory.CreateDirectory(backupFolder);

                // Timestamped backup name
                var backupFile = Path.Combine(
                    backupFolder,
                    $"mystic_todo_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

                // Copy DB
                File.Copy(dbPath, backupFile, true);

                await Shell.Current.DisplayAlertAsync(
                    "Backup Complete",
                    $"Database backup created successfully.\n\n{backupFile}",
                    "OK");

                Debug.WriteLine($"Backup created: {backupFile}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Backup error: {ex.Message}");

                await Shell.Current.DisplayAlertAsync(
                    "Backup Error",
                    ex.Message,
                    "OK");
            }
        }

        [RelayCommand]
        private async Task ResetDatabaseAsync()
        {
            var confirm = await Shell.Current.DisplayAlertAsync(
                "Reset Database",
                "This will delete ALL local data and recreate the database.\n\nContinue?",
                "Yes",
                "No");

            if (!confirm)
                return;

            try
            {
                IsLoading = true;

                var dbPath = Path.Combine(
                    FileSystem.AppDataDirectory,
                    "mystic_todo.db");

                // STOP DB USAGE FIRST (IMPORTANT)
                _cts?.Cancel(); // stop any running operations
                await Task.Delay(200);

                // CLOSE CONNECTION (CRITICAL FIX)
                await _dbService.CloseAsync();
                await Task.Delay(300);

                // Delete old DB
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);

                    Debug.WriteLine("Old database deleted.");
                }

                // Recreate schema + seed
                await _dbInitializer.DBInitializerAsync();

                await Shell.Current.DisplayAlertAsync(
                       "Reset Complete",
                       "Database recreated successfully.\nPlease restart the app for best stability.",
                       "OK");

                Debug.WriteLine("Database reset completed.");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Reset DB error: {ex.Message}");

                await Shell.Current.DisplayAlertAsync(
                    "Reset Error",
                    ex.Message,
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }


        private async Task SaveSettingsAsync()
        {
            var settings = new AppSettingsModel
            {
                IsDarkModeEnabled = IsDarkModeEnabled,
                AlarmNotificationsEnabled = AlarmNotificationsEnabled,
                RepeatNotificationsEnabled = RepeatNotificationsEnabled,
                GoogleSyncEnabled = GoogleSyncEnabled,
                GoogleAccountEmail = GoogleAccountEmail
            };

            await _settingsService.SaveSettingsAsync(settings);
        }


        // Refresh Other UI elements
        public void RefreshUI()
        {

        }

        public void CancelSync()
        {
            _cts?.Cancel();
        }

        // -----------------------------
        // Startup and Cleanup
        // -----------------------------
        public async Task LoadDataAsync()
        {
            await _appState.WaitUntilReady();

            _isInitializing = true;

            // Load Settings from Preferences file 
            var settings = await _settingsService.LoadSettingsAsync();

            IsDarkModeEnabled = settings.IsDarkModeEnabled;
            AlarmNotificationsEnabled = settings.AlarmNotificationsEnabled;
            RepeatNotificationsEnabled = settings.RepeatNotificationsEnabled;

            // Load Google Sync State
            GoogleSyncEnabled = settings.GoogleSyncEnabled;
            GoogleAccountEmail = settings.GoogleAccountEmail;

            IsGoogleConnected =
                GoogleSyncEnabled &&
                !string.IsNullOrWhiteSpace(GoogleAccountEmail) &&
                GoogleAccountEmail != "Not Connected";

            //// Load Google Sync State
            //GoogleSyncEnabled =
            //    Preferences.Default.Get("GoogleSyncEnabled", false);

            //if (GoogleSyncEnabled)
            //{
            //    IsGoogleConnected = true;
            //    //GoogleAccountEmail = "Google Account Connected";
            //    GoogleAccountEmail =
            //        Preferences.Default.Get(
            //            "GoogleAccountEmail",
            //            "Not Connected");
            //}
            _isInitializing = false;
        }

    }
}
