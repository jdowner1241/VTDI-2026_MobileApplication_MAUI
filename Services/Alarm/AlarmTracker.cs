using Microsoft.Maui.Controls.PlatformConfiguration;
using CommunityToolkit.Maui;
using Mystic_ToDo_MAUI_.Model.db.tables;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services.Alarm
{
    public class AlarmTracker
    {
        private readonly TaskRepo _taskRepo;
        private readonly IDispatcherTimer _timer;

        public AlarmTracker(TaskRepo taskRepo, IDispatcherTimer timer)
        {
            _taskRepo = taskRepo;
            _timer = timer;
            _timer.Interval = TimeSpan.FromMinutes(1);
            _timer.Tick += Timer_Tick;

        }

        public void Start() => _timer.Start();

        private async void Timer_Tick(object sender, EventArgs e) 
        {
            var now = DateTime.Now;
            var dueTasks = await _taskRepo.GetAllTasksAsync(now);

            foreach (var task in dueTasks)
            {
                // Show Notification
                await ShowToastNotification(task);

                // Update task in database using Repeat or Non-Repeat method
                await HandleRepeatAsync(task);

            }
        }

        private async Task ShowToastNotification(TaskList task)
        {
            var message = $"Task '{task.Title}' is Triggered!";

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                // Windows fallback → use DisplayAlert
                await Application.Current.MainPage.DisplayAlertAsync("Task", message, "OK");

                // Play a system sound (Windows only)
                System.Media.SystemSounds.Exclamation.Play();
            }
            else
            {
                // Mobile platforms → use Toast
                var toast = CommunityToolkit.Maui.Alerts.Toast.Make(
                    message,
                    CommunityToolkit.Maui.Core.ToastDuration.Short
                );
                await toast.Show();
            }

        }

        private async Task HandleRepeatAsync(TaskList task)
        {
            if (task.Task_RepeatList != null)
            {
                // Calculate next alarm info
                var nextRepeatInfo = _taskRepo.GetNextAlarm(task.Task_RepeatList);

                if (nextRepeatInfo != null)
                {
                    // Update task repeat info with new due date
                    task.Task_RepeatList = nextRepeatInfo;

                    // Persist changes
                    await _taskRepo.UpdateTaskRepeatAsync(task);
                }
                else
                {
                    await _taskRepo.UpdateTaskNonRepeatAsync(task, true);
                }

            }
        }


    }
}
