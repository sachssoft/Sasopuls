using Sachssoft.Sasopuls.Basic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Sachssoft.Sasopuls
{
    /// <summary>
    /// Base class for objects that track a "dirty" state.
    /// Supports delayed notifications, freeze/unfreeze,
    /// child dirty propagation, and AOT/trimming friendly.
    /// </summary>
    public class NotifyObject : IDisposable, INotifyPropertyChangedContext, INotifyPropertyChangingContext
    {
        private event PropertyChangedEventHandler? _propertyChangedBridge;
        private event PropertyChangingEventHandler? _propertyChangingBridge;

        private readonly List<NotifyObject> _childDirtyObjects = new();
        private bool _isDirty;
        private bool _isFrozen;

        private readonly object _delayLock = new();
        private int _delayCount;
        private HashSet<string>? _delayedProperties;
        private CancellationTokenSource? _delayCts;
        private int _delayMilliseconds = 100; // Standard-Verzögerung

        public event EventHandler? DirtyActivated;

        public event EventHandler<PropertyChangingContextEventArgs>? PropertyChanging;
        public event EventHandler<PropertyChangedContextEventArgs>? PropertyChanged;

        #region Event Bridge

        private void OnPropertyChangedContext(object? sender, PropertyChangedContextEventArgs e)
        {
            _propertyChangedBridge?.Invoke(sender, new PropertyChangedEventArgs(e.PropertyName));
        }

        private void OnPropertyChangingContext(object? sender, PropertyChangingContextEventArgs e)
        {
            _propertyChangingBridge?.Invoke(sender, new PropertyChangingEventArgs(e.PropertyName));
        }

        // === INotifyPropertyChanged ===
        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                if (_propertyChangedBridge == null)
                    PropertyChanged += OnPropertyChangedContext;

                _propertyChangedBridge += value;
            }
            remove
            {
                _propertyChangedBridge -= value;

                if (_propertyChangedBridge == null)
                    PropertyChanged -= OnPropertyChangedContext;
            }
        }

        // === INotifyPropertyChanging ===
        event PropertyChangingEventHandler? INotifyPropertyChanging.PropertyChanging
        {
            add
            {
                if (_propertyChangingBridge == null)
                    PropertyChanging += OnPropertyChangingContext;

                _propertyChangingBridge += value;
            }
            remove
            {
                _propertyChangingBridge -= value;

                if (_propertyChangingBridge == null)
                    PropertyChanging -= OnPropertyChangingContext;
            }
        }
        #endregion

        #region Dirty State

        /// <summary>Indicates whether the object or its children have been modified.</summary>
        public bool IsDirty
        {
            get => _isDirty;
            protected set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    if (!_isFrozen)
                    {
                        if (_isDirty)
                            DirtyActivated?.Invoke(this, EventArgs.Empty);
                        RaisePropertyChanged(nameof(IsDirty));
                    }
                }
            }
        }

        /// <summary>Indicates whether notifications are currently frozen.</summary>
        public bool IsFrozen
        {
            get => _isFrozen;
            set
            {
                if (_isFrozen != value)
                {
                    _isFrozen = value;
                    RaisePropertyChanged(nameof(IsFrozen));
                }
            }
        }

        /// <summary>Marks the object as dirty.</summary>
        public void MarkDirty() => IsDirty = true;

        /// <summary>Resets the dirty state of this object and all registered child objects.</summary>
        public void ResetDirty()
        {
            var wasFrozen = IsFrozen;
            IsFrozen = true;

            IsDirty = false;

            foreach (var child in _childDirtyObjects)
                child.ResetDirty();

            IsFrozen = wasFrozen;
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
            PropertyChangeContext? context = null,
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
                        RaisePropertyChanging(propertyName, context);
                        RaisePropertyChanged(propertyName, context);
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
        protected void SetAndMarkDirty<T>(
            ref T? field, 
            T? value, 
            PropertyChangeContext? context = null, 
            [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T?>.Default.Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName, context);
                MarkDirty();
            }
        }

        // Setzt ein Feld, feuert PropertyChanged und markiert Dirty
        // Speziell für Model Property (MVVM)
        protected void SetAndMarkDirty<T>(
            Func<T?> getter, 
            Action<T?> setter, 
            T? value, 
            PropertyChangeContext? context = null, 
            [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T?>.Default.Equals(getter(), value))
            {
                setter(value);
                RaisePropertyChanged(propertyName, context);
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
            PropertyChangeContext? context = null,
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
                        RaisePropertyChanged(propertyName, context);
                        MarkDirty();
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(nullStoreBehavior), nullStoreBehavior, null);
                }
            }

            if (!EqualityComparer<T?>.Default.Equals(store.Get(propertyName, fallback), value))
            {
                store.Set(propertyName, value);
                RaisePropertyChanged(propertyName, context);
                MarkDirty();
            }
        }

        // Setzt ein Feld, feuert PropertyChanging und PropertyChanged
        protected bool SetAndNotify<T>(
            ref T? field, 
            T? value, 
            PropertyChangeContext? context = null,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T?>.Default.Equals(field, value)) return false;

            RaisePropertyChanging(propertyName, context);
            field = value;
            RaisePropertyChanged(propertyName, context);
            return true;
        }

        // Setzt ein Feld, feuert PropertyChanging und PropertyChanged
        // Speziell für Model Property (MVVM)
        protected bool SetAndNotify<T>(
            Func<T?> getter, 
            Action<T?> setter, 
            T? value, 
            PropertyChangeContext? context = null,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T?>.Default.Equals(getter(), value)) return false;

            RaisePropertyChanging(propertyName, context);
            setter(value);
            RaisePropertyChanged(propertyName, context);
            return true;
        }

        // Setzt ein Feld, feuert PropertyChanging und PropertyChanged
        // Speziell für Model Property (MVVM)
        protected bool SetAndNotify<T>(
            IPropertyStore? store,
            T? value,
            T? fallback = default,
            NullStoreBehavior nullStoreBehavior = NullStoreBehavior.ThrowException,
            PropertyChangeContext? context = null,
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
                        RaisePropertyChanging(propertyName, context);
                        RaisePropertyChanged(propertyName, context);
                        return true;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(nullStoreBehavior), nullStoreBehavior, null);
                }
            }

            if (EqualityComparer<T?>.Default.Equals(store.Get(propertyName, fallback), value))
                return false;

            RaisePropertyChanging(propertyName, context);
            store.Set(propertyName, value);
            RaisePropertyChanged(propertyName, context);
            return true;
        }

        // Setzt ein Feld direkt ohne Dirty, feuert nur Events
        protected void SetDirectly<T>(
            ref T? field, 
            T? value, 
            PropertyChangeContext? context = null, 
            [CallerMemberName] string propertyName = "")
        {
            RaisePropertyChanging(propertyName, context);
            field = value;
            RaisePropertyChanged(propertyName, context);
        }

        // Setzt ein Feld direkt ohne Dirty, feuert nur Events
        // Speziell für Model Property (MVVM)
        protected void SetDirectly<T>(
            Action<T?> setter, 
            T? value,
            PropertyChangeContext? context = null, 
            [CallerMemberName] string propertyName = "")
        {
            RaisePropertyChanging(propertyName, context);
            setter(value);
            RaisePropertyChanged(propertyName, context);
        }

        // Setzt ein Feld direkt ohne Dirty, feuert nur Events
        // Speziell für Model Property (MVVM)
        protected void SetDirectly<T>(
            IPropertyStore? store,
            T? value,
            NullStoreBehavior nullStoreBehavior = NullStoreBehavior.ThrowException,
            PropertyChangeContext? context = null,
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
                        RaisePropertyChanging(propertyName, context);
                        RaisePropertyChanged(propertyName, context);
                        return;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(nullStoreBehavior), nullStoreBehavior, null);
                }
            }

            RaisePropertyChanging(propertyName, context);
            store.Set(propertyName, value);
            RaisePropertyChanged(propertyName, context);
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
        protected void RaisePropertyChanged(string propertyName, PropertyChangeContext? context = null)
        {
            if (_isFrozen) return;

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
                    OnPropertyChanged(new PropertyChangedContextEventArgs(propertyName, context));
                }
            }
        }

        // Protected, feuert PropertyChanging
        protected void RaisePropertyChanging(string propertyName, PropertyChangeContext? context = null)
        {
            if (_isFrozen) return;
            OnPropertyChanging(new PropertyChangingContextEventArgs(propertyName, context));
        }

        // Verzögerte Properties auslösen
        private void FlushDelayedProperties()
        {
            lock (_delayLock)
            {
                if (_delayedProperties != null)
                {
                    foreach (var prop in _delayedProperties)
                        OnPropertyChanged(new PropertyChangedContextEventArgs(prop));

                    _delayedProperties.Clear();
                    _delayCts = null;
                }
            }
        }

        #endregion

        #region Protected Event Invokers

        protected virtual void OnPropertyChanging(PropertyChangingContextEventArgs e)
        {
            PropertyChanging?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedContextEventArgs e)
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
                _prevFreeze = obj.IsFrozen;
                obj.IsFrozen = true;
            }

            public void Dispose() => _obj.IsFrozen = _prevFreeze;
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