using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sachssoft.Sasopuls.ViewModels
{
    public sealed class ViewModelFactoryRegistry : IEnumerable<IViewModelFactory>
    {
        private readonly IReadOnlyDictionary<Type, IViewModelFactory> _factories;

        public ViewModelFactoryRegistry(IEnumerable<IViewModelFactory> factories)
        {
            if (factories is null)
                throw new ArgumentNullException(nameof(factories));

            var dict = new Dictionary<Type, IViewModelFactory>();

            foreach (var factory in factories)
            {
                if (factory is null)
                    throw new ArgumentException("Factory collection contains null.", nameof(factories));

                if (dict.ContainsKey(factory.ModelType))
                    throw new ArgumentException(
                        $"Duplicate factory for model type {factory.ModelType.Name}",
                        nameof(factories));

                dict.Add(factory.ModelType, factory);
            }

            _factories = new ReadOnlyDictionary<Type, IViewModelFactory>(dict);
        }

        public IEnumerable<IViewModelFactory> AvailableFactories => _factories.Values;

        // 1.1.2
        public bool TryCreateInstance(Type viewModelType, object? model, out ViewModelBase? viewModel)
        {
            if (viewModelType is null)
                throw new ArgumentNullException(nameof(viewModelType));

            foreach (var factory in _factories.Values)
            {
                if (factory is IViewModelTypeProvider provider &&
                    provider.ViewModelType == viewModelType)
                {
                    viewModel = factory.Build(model);
                    return true;
                }
            }

            viewModel = null;
            return false;
        }

        // 1.1.2
        public bool TryCreateInstance(Type viewModelType, out ViewModelBase? viewModel)
        {
            return TryCreateInstance(viewModelType, null, out viewModel);
        }

        // 1.1.2
        public bool TryCreateInstance<TViewModel>(out ViewModelBase? viewModel)
            where TViewModel : ViewModelBase
        {
            return TryCreateInstance(typeof(TViewModel), null, out viewModel);
        }

        // 1.1.2
        public bool TryCreateInstance<TViewModel>(object? model, out ViewModelBase? viewModel)
            where TViewModel : ViewModelBase
        {
            return TryCreateInstance(typeof(TViewModel), model, out viewModel);
        }

        // Baut ein ViewModel anhand des übergebenen Models.  
        // Sucht eine passende Factory über den ModelType (inkl. Vererbung via IsAssignableFrom).  
        // Wirft eine Exception, wenn keine passende Factory registriert ist.
        public ViewModelBase Build(object model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var modelType = model.GetType();

            foreach (var factory in _factories.Values)
            {
                if (factory.ModelType.IsAssignableFrom(modelType))
                {
                    return factory.Build(model);
                }
            }

            throw new InvalidOperationException(
                $"No factory registered for model type {modelType.Name}");
        }

        public ModelViewModelBase<TModel> Build<TModel>(TModel model) => Build(model);

        public bool CanResolve(Type modelType)
        {
            if (modelType is null)
                throw new ArgumentNullException(nameof(modelType));

            return _factories.ContainsKey(modelType);
        }

        public bool CanResolve<TModel>() => CanResolve(typeof(TModel));

        IEnumerator<IViewModelFactory> IEnumerable<IViewModelFactory>.GetEnumerator() => _factories.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _factories.Values.GetEnumerator();
    }
}