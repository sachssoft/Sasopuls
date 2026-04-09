using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Sachssoft.Sasofly.Pulse
{
    /// <summary>
    /// Base class for objects that track a "dirty" state.
    /// Supports delayed notifications, freeze/unfreeze,
    /// child dirty propagation, and AOT/trimming friendly.
    /// </summary>
    public class NotifyObject : IDisposable, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private readonly List<NotifyObject> _childDirtyObjects = new();
        private bool _dirty;
        private bool _freeze;

        private readonly object _delayLock = new();
        private int _delayCount;
        private HashSet<string>? _delayedProperties;
        private CancellationTokenSource? _delayCts;
        private int _delayMilliseconds = 100; // Standard-Verzögerung

        public event EventHandler? DirtyActivated;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

        #region Dirty State

        /// <summary>Indicates whether the object or its children have been modified.</summary>
        public bool Dirty
        {
            get => _dirty;
            protected set
            {
                if (_dirty != value)
                {
                    _dirty = value;
                    if (!_freeze)
                    {
                        if (_dirty)
                            DirtyActivated?.Invoke(this, EventArgs.Empty);
                        RaisePropertyChanged(nameof(Dirty));
                    }
                }
            }
        }

        /// <summary>Indicates whether notifications are currently frozen.</summary>
        public bool Freeze
        {
            get => _freeze;
            set
            {
                if (_freeze != value)
                {
                    _freeze = value;
                    RaisePropertyChanged(nameof(Freeze));
                }
            }
        }

        /// <summary>Marks the object as dirty.</summary>
        public void MarkDirty() => Dirty = true;

        /// <summary>Resets the dirty state of this object and all registered child objects.</summary>
        public void ResetDirty()
        {
            var wasFrozen = Freeze;
            Freeze = true;

            Dirty = false;

            foreach (var child in _childDirtyObjects)
                child.ResetDirty();

            Freeze = wasFrozen;
            OnDirtyReset();
        }

        /// <summary>Hook method for derived classes when dirty state is reset.</summary>
        protected virtual void OnDirtyReset() { }

        #endregion

        #region Child Dirty Propagation

        /// <summary>Registers a child object for dirty state propagation.</summary>
        public void RegisterDirtyObject(NotifyObject obj)
        {
            if (obj == null || _childDirtyObjects.Contains(obj)) return;

            _childDirtyObjects.Add(obj);
            obj.DirtyActivated += Child_DirtyActivated;
        }

        /// <summary>Unregisters a child object.</summary>
        public void UnregisterDirtyObject(NotifyObject obj)
        {
            if (_childDirtyObjects.Remove(obj))
                obj.DirtyActivated -= Child_DirtyActivated;
        }

        // Kind-Dirty-Ereignis behandelt und markiert das aktuelle Objekt dirty
        private void Child_DirtyActivated(object? sender, EventArgs e) => MarkDirty();

        #endregion

        #region Property Helpers

        protected T? GetFromStore<T>(
            IPropertyStore? store,
            T? fallback = default,
            NullStoreBehavior nullStoreBehavior = NullStoreBehavior.Ignore,
            [CallerMemberName] string propertyName = ""
        )
        {
            if (store == null)
            {
                switch (nullStoreBehavior)
                {
                    case NullStoreBehavior.Ignore:
                        return fallback;

                    case NullStoreBehavior.ThrowException:
                        throw new ArgumentNullException(nameof(store), $"Store cannot be null for property {propertyName}");

                    case NullStoreBehavior.NotifyOnly:
                        // Nur Events feuern, Value gibt es nicht → fallback
                        RaisePropertyChanging(propertyName);
                        RaisePropertyChanged(propertyName);
                        return fallback;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(nullStoreBehavior), nullStoreBehavior, null);
                }
            }

            return store.Get(propertyName, fallback);
        }

        // ---------------------------
        // Für Felder (bestehende Methoden)
        // ---------------------------

        // Setzt ein Feld, feuert PropertyChanged und markiert Dirty
        protected void SetAndMarkDirty<T>(ref T? field, T? value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T?>.Default.Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName);
                MarkDirty();
            }
        }

        // Setzt ein Feld, feuert PropertyChanged und markiert Dirty
        // Speziell für Model Property (MVVM)
        protected void SetAndMarkDirty<T>(Func<T?> getter, Action<T?> setter, T? value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T?>.Default.Equals(getter(), value))
            {
                setter(value);
                RaisePropertyChanged(propertyName);
                MarkDirty();
            }
        }

        // Setzt ein Feld, feuert PropertyChanged und markiert Dirty
        // Speziell für Model Property (MVVM)
        protected void SetAndMarkDirty<T>(
            IPropertyStore? store,
            T? value,
            T? fallback = default,
            NullStoreBehavior nullStoreBehavior = NullStoreBehavior.ThrowException,
            [CallerMemberName] string propertyName = ""
        )
        {
            if (store == null)
            {
                switch (nullStoreBehavior)
                {
                    case NullStoreBehavior.Ignore:
                        return;

                    case NullStoreBehavior.ThrowException:
                        throw new ArgumentNullException(nameof(store), $"Store cannot be null for property {propertyName}");

                    case NullStoreBehavior.NotifyOnly:
                        RaisePropertyChanged(propertyName);
                        MarkDirty();
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(nullStoreBehavior), nullStoreBehavior, null);
                }
            }

            if (!EqualityComparer<T?>.Default.Equals(store.Get(propertyName, fallback), value))
            {
                store.Set(propertyName, value);
                RaisePropertyChanged(propertyName);
                MarkDirty();
            }
        }

        // Setzt ein Feld, feuert PropertyChanging und PropertyChanged
        protected bool SetAndNotify<T>(ref T? field, T? value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T?>.Default.Equals(field, value)) return false;

            RaisePropertyChanging(propertyName);
            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        // Setzt ein Feld, feuert PropertyChanging und PropertyChanged
        // Speziell für Model Property (MVVM)
        protected bool SetAndNotify<T>(Func<T?> getter, Action<T?> setter, T? value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T?>.Default.Equals(getter(), value)) return false;

            RaisePropertyChanging(propertyName);
            setter(value);
            RaisePropertyChanged(propertyName);
            return true;
        }

        // Setzt ein Feld, feuert PropertyChanging und PropertyChanged
        // Speziell für Model Property (MVVM)
        protected bool SetAndNotify<T>(
            IPropertyStore? store,
            T? value,
            T? fallback = default,
            NullStoreBehavior nullStoreBehavior = NullStoreBehavior.ThrowException,
            [CallerMemberName] string propertyName = "")
        {
            if (store == null)
            {
                switch (nullStoreBehavior)
                {
                    case NullStoreBehavior.Ignore:
                        return false;

                    case NullStoreBehavior.ThrowException:
                        throw new ArgumentNullException(nameof(store), $"Store cannot be null for property {propertyName}");

                    case NullStoreBehavior.NotifyOnly:
                        RaisePropertyChanging(propertyName);
                        RaisePropertyChanged(propertyName);
                        return true;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(nullStoreBehavior), nullStoreBehavior, null);
                }
            }

            if (EqualityComparer<T?>.Default.Equals(store.Get(propertyName, fallback), value))
                return false;

            RaisePropertyChanging(propertyName);
            store.Set(propertyName, value);
            RaisePropertyChanged(propertyName);
            return true;
        }

        // Setzt ein Feld direkt ohne Dirty, feuert nur Events
        protected void SetDirectly<T>(ref T? field, T? value, [CallerMemberName] string propertyName = "")
        {
            RaisePropertyChanging(propertyName);
            field = value;
            RaisePropertyChanged(propertyName);
        }

        // Setzt ein Feld direkt ohne Dirty, feuert nur Events
        // Speziell für Model Property (MVVM)
        protected void SetDirectly<T>(Action<T?> setter, T? value, [CallerMemberName] string propertyName = "")
        {
            RaisePropertyChanging(propertyName);
            setter(value);
            RaisePropertyChanged(propertyName);
        }

        // Setzt ein Feld direkt ohne Dirty, feuert nur Events
        // Speziell für Model Property (MVVM)
        protected void SetDirectly<T>(
            IPropertyStore? store,
            T? value,
            NullStoreBehavior nullStoreBehavior = NullStoreBehavior.ThrowException,
            [CallerMemberName] string propertyName = "")
        {
            if (store == null)
            {
                switch (nullStoreBehavior)
                {
                    case NullStoreBehavior.Ignore:
                        return;

                    case NullStoreBehavior.ThrowException:
                        throw new ArgumentNullException(nameof(store), $"Store cannot be null for property {propertyName}");

                    case NullStoreBehavior.NotifyOnly:
                        RaisePropertyChanging(propertyName);
                        RaisePropertyChanged(propertyName);
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(nullStoreBehavior), nullStoreBehavior, null);
                }
            }

            RaisePropertyChanging(propertyName);
            store.Set(propertyName, value);
            RaisePropertyChanged(propertyName);
        }

        #endregion

        #region Delay and Suppress Notify

        // Temporär alle Benachrichtigungen einfrieren (Freeze + kein Dirty Event)
        public IDisposable SuppressNotify() => new NotifySuppressor(this);

        // Verzögerte Benachrichtigungen für die angegebene Zeit
        public IDisposable DelayNotify(int milliseconds = 100)
        {
            _delayMilliseconds = milliseconds;
            Interlocked.Increment(ref _delayCount);
            return new DelayScope(this);
        }

        // Protected, damit abgeleitete Klassen direkt Benachrichtigungen feuern können
        protected void RaisePropertyChanged(string propertyName)
        {
            if (_freeze) return;

            lock (_delayLock)
            {
                if (_delayCount > 0)
                {
                    _delayedProperties ??= new HashSet<string>();
                    _delayedProperties.Add(propertyName);

                    // Timer neu starten
                    _delayCts?.Cancel();
                    _delayCts = new CancellationTokenSource();
                    var token = _delayCts.Token;

                    Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(_delayMilliseconds, token);
                            if (!token.IsCancellationRequested)
                                FlushDelayedProperties();
                        }
                        catch (TaskCanceledException) { }
                    });
                }
                else
                {
                    OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        // Protected, feuert PropertyChanging
        protected void RaisePropertyChanging(string propertyName)
        {
            if (_freeze) return;
            OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

        // Verzögerte Properties auslösen
        private void FlushDelayedProperties()
        {
            lock (_delayLock)
            {
                if (_delayedProperties != null)
                {
                    foreach (var prop in _delayedProperties)
                        OnPropertyChanged(new PropertyChangedEventArgs(prop));

                    _delayedProperties.Clear();
                    _delayCts = null;
                }
            }
        }

        #endregion

        #region Protected Event Invokers

        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #endregion

        #region Dispose

        // Dispose, entfernt Child-Events und leert die Liste
        public void Dispose()
        {
            OnDisposed();

            foreach (var child in _childDirtyObjects)
            {
                child.DirtyActivated -= Child_DirtyActivated;
                child.Dispose();
            }

            _childDirtyObjects.Clear();
        }

        protected virtual void OnDisposed() { }

        #endregion

        #region Underlying Classes

        // Hilfsklasse für SuppressNotify
        private class NotifySuppressor : IDisposable
        {
            private readonly NotifyObject _obj;
            private readonly bool _prevFreeze;

            public NotifySuppressor(NotifyObject obj)
            {
                _obj = obj;
                _prevFreeze = obj.Freeze;
                obj.Freeze = true;
            }

            public void Dispose() => _obj.Freeze = _prevFreeze;
        }

        // Hilfsklasse für DelayNotify
        private class DelayScope : IDisposable
        {
            private readonly NotifyObject _obj;
            private bool _disposed;

            public DelayScope(NotifyObject obj)
            {
                _obj = obj;
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;

                if (Interlocked.Decrement(ref _obj._delayCount) == 0)
                    _obj.FlushDelayedProperties();
            }
        }

        #endregion
    }
}