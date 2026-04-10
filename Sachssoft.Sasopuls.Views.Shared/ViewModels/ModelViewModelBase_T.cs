using System;
using System.Threading.Tasks;

namespace Sachssoft.Sasopuls.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels in MVVM architecture.
    /// Encapsulates the underlying model and provides lifecycle hooks.
    /// </summary>
    public abstract class ModelViewModelBase<TModel> : ViewModelBase, ITrackableViewModel, IDisposable
    {
        // Das zugrunde liegende Model (wird bewusst gekapselt)
        private readonly TModel _model;
        private bool _isReloading = false;

        protected ModelViewModelBase(TModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        // Zugriff nur für abgeleitete ViewModels (nicht für die View!)
        // -> MVVM-Regel: View darf niemals direkt auf das Model zugreifen
        protected TModel Model => _model;

        /// <summary>
        /// Indicates whether ReloadAsync is currently running
        /// </summary>
        // Zeigt an, ob gerade ReloadAsync läuft
        public bool IsReloading
        {
            get => _isReloading;
            private set => SetAndNotify(ref _isReloading, value);
        }

        /// <summary>
        /// Loads data from the model into the ViewModel (synchronous)
        /// </summary>
        // Wird verwendet, um Daten vom Model in das ViewModel zu laden
        protected virtual void LoadFromModel() { }

        /// <summary>
        /// Loads data from the model into the ViewModel (asynchronous)
        /// </summary>
        // Async-Version von LoadFromModel
        protected virtual Task LoadFromModelAsync() => Task.CompletedTask;

        /// <summary>
        /// Saves data from the ViewModel back into the model
        /// </summary>
        // Schreibt Änderungen vom ViewModel zurück ins Model
        protected virtual void SaveToModel() { }

        /// <summary>
        /// Reloads data synchronously
        /// </summary>
        public void Reload()
        {
            LoadFromModel();
        }

        /// <summary>
        /// Reloads data asynchronously
        /// </summary>
        public async Task ReloadAsync()
        {
            if (IsReloading)
                return;

            try
            {
                IsReloading = true;
                await LoadFromModelAsync().ConfigureAwait(false);
            }
            finally
            {
                IsReloading = false;
            }
        }

        /// <summary>
        /// Persists data back to the model
        /// </summary>
        public void Persist()
        {
            SaveToModel();
        }

        /// <summary>
        /// Resets the ViewModel to its original model state
        /// </summary>
        // Setzt das ViewModel zurück auf den ursprünglichen Zustand des Models
        public void Reset()
        {
            Reload();
        }

        /// <summary>
        /// Resets the ViewModel asynchronously to its original model state
        /// </summary>
        public Task ResetAsync()
        {
            return ReloadAsync();
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        protected override void OnDisposed()
        {
            base.OnDisposed();

            // Falls das Model IDisposable implementiert, ebenfalls freigeben
            if (_model is IDisposable disposable)
                disposable.Dispose();
        }
    }
}