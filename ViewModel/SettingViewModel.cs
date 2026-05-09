using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Apis.Calendar.v3;
using Mystic_ToDo_MAUI_.Services;
using Mystic_ToDo_MAUI_.Services.API;
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
        private readonly GoogleCalendarAPIHelper _googleHelper;
        private readonly GoogleTaskSyncService _syncService;

        private CancellationTokenSource _cts;

        // -----------------------------
        // properties and supporting structures
        // -----------------------------
        [ObservableProperty]
        private bool isDarkModeEnabled;

        [ObservableProperty]
        private bool alarmNotificationsEnabled;

        [ObservableProperty]
        private bool repeatNotificationsEnabled;

        [ObservableProperty]
        private bool googleSyncEnabled;
        partial void OnGoogleSyncEnabledChanged(bool value)
        {
            Preferences.Default.Set("GoogleSyncEnabled", value);
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
     

        // -----------------------------
        // Constructor
        // -----------------------------

        public SettingViewModel
            (
                AppState appState,
                GoogleCalendarAPIHelper googleHelper,
                GoogleTaskSyncService syncService
            )
        {
            _appState = appState;
            _googleHelper = googleHelper;
            _syncService = syncService;
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




        public ICommand BackupDatabaseCommand { get; }
        public ICommand ResetDatabaseCommand { get; }


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

            // Load Settings from Preferences file 


            // Load Google Sync State
            GoogleSyncEnabled =
                Preferences.Default.Get("GoogleSyncEnabled", false);

            if (GoogleSyncEnabled)
            {
                IsGoogleConnected = true;
                //GoogleAccountEmail = "Google Account Connected";
                GoogleAccountEmail =
                    Preferences.Default.Get(
                        "GoogleAccountEmail",
                        "Not Connected");
            }
        }

    }
}
