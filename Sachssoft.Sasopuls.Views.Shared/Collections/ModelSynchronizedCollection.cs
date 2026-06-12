using Sachssoft.Sasopuls.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Sachssoft.Sasopuls.Collections
{
    // New feature: Version 2.0

    public class ModelSynchronizedCollection<TViewModel, TModel> : ObservableCollection<TViewModel>
        where TViewModel : ModelViewModelBase<TModel>
    {
        private readonly IList<TModel> _models;
        private readonly object _syncRoot = new();
        private readonly Func<TModel, TViewModel> _factory;

        public ModelSynchronizedCollection(IList<TModel> synchronizable, Func<TModel, TViewModel> factory)
        {
            _models = synchronizable ?? throw new ArgumentNullException(nameof(synchronizable));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            foreach (var model in _models)
            {
                // Direkter Zugriff auf die interne Liste,
                // damit keine Synchronisation zurück ins Model erfolgt.
                Items.Add(factory(model));
            }
        }

        public void Synchronize()
        {
            lock (_syncRoot)
            {
                Items.Clear();
                foreach (var model in _models)
                {
                    Items.Add(_factory(model));
                }
            }
        }

        protected override void InsertItem(int index, TViewModel item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (_syncRoot)
            {
                _models.Insert(index, item.Model);
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (_syncRoot)
            {
                _models.RemoveAt(index);
                base.RemoveItem(index);
            }
        }

        protected override void ClearItems()
        {
            lock (_syncRoot)
            {
                _models.Clear();
                base.ClearItems();
            }
        }

        protected override void SetItem(int index, TViewModel item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (_syncRoot)
            {
                _models[index] = item.Model;
                base.SetItem(index, item);
            }
        }
    }
}