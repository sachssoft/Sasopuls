using System;
using System.Threading;

namespace Sachssoft.Sasopuls.Messaging
{
    /// <summary>
    /// Central dispatcher for CanExecute updates.
    /// Prevents event storms in large MVVM systems.
    /// </summary>
    public sealed class CommandDispatcher
    {
        private readonly object _lock = new();
        private event Action? _updateRequested;

        /// <summary>
        /// Raised when any command rule changes state.
        /// Commands subscribe to this event instead of individual rules.
        /// </summary>
        public event Action? UpdateRequested
        {
            add
            {
                lock (_lock)
                    _updateRequested += value;
            }
            remove
            {
                lock (_lock)
                    _updateRequested -= value;
            }
        }

        /// <summary>
        /// Triggers a global CanExecute refresh.
        /// </summary>
        public void NotifyChanged()
        {
            _updateRequested?.Invoke();
        }

        /// <summary>
        /// Optional: batched notification (future extension hook)
        /// </summary>
        public void NotifyChangedDeferred(int delayMs = 1)
        {
            // minimal batching without timer dependencies
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(delayMs);
                _updateRequested?.Invoke();
            });
        }
    }
}