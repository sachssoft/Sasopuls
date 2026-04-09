using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sachssoft.Sasofly.Pulse.ViewModels
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

        public ViewModelBase Build(object model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var modelType = model.GetType();

            if (!_factories.TryGetValue(modelType, out var factory))
                throw new InvalidOperationException(
                    $"No factory registered for model type {modelType.Name}");

            return factory.Build(model);
        }

        public ViewModelBase<TModel> Build<TModel>(TModel model) => Build(model);

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