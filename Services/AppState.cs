using System;
using System.Collections.Generic;
using System.Text;

namespace Mystic_ToDo_MAUI_.Services
{
    public class AppState
    {
        //public bool IsInitialized { get; set; }

        private TaskCompletionSource _initTcs = new();

        public Task WaitUntilReady() => _initTcs.Task;

        public void MarkReady() => _initTcs.TrySetResult();
    }
}
