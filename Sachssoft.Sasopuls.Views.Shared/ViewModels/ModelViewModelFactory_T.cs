using System;

namespace Sachssoft.Sasopuls.ViewModels
{

    /// <summary>
    /// Factory for creating ViewModels and optionally providing their underlying Models.
    /// </summary>
    public sealed class ModelViewModelFactory<TModel> : IViewModelFactory
        where TModel : class
    {
        private readonly Func<TModel, ModelViewModelBase<TModel>> _viewModelFactory;
        private readonly TModel? _model;

        private ModelViewModelFactory(
            Func<TModel, ModelViewModelBase<TModel>> viewModelFactory,
            Func<TModel>? modelFactory = null,
            TModel? sourceModel = null)
        {
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));

            if (modelFactory != null)
            {
                _model = modelFactory();
            }
            else
            {
                _model = sourceModel;
            }
        }

        public Type ModelType { get; } = typeof(TModel);

        public static ModelViewModelFactory<TModel> Create(Func<TModel, ModelViewModelBase<TModel>> viewModelFactory)
        {
            return new ModelViewModelFactory<TModel>(viewModelFactory);
        }

        public static ModelViewModelFactory<TModel> Create(Func<TModel, ModelViewModelBase<TModel>> viewModelFactory,
                                                      Func<TModel> modelFactory)
        {
            return new ModelViewModelFactory<TModel>(viewModelFactory, modelFactory: modelFactory);
        }

        public static ModelViewModelFactory<TModel> Create(Func<TModel, ModelViewModelBase<TModel>> viewModelFactory,
                                                          TModel sourceModel)
        {
            return new ModelViewModelFactory<TModel>(viewModelFactory, sourceModel: sourceModel);
        }

        public ModelViewModelBase<TModel> Build(TModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            return _viewModelFactory(model);
        }

        public TModel? ProvideModel() => _model;

        ViewModelBase IViewModelFactory.Build(object? model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (model is not TModel typedModel)
                throw new ArgumentException(
                    $"Invalid model type. Expected {typeof(TModel).Name}, got {model.GetType().Name}",
                    nameof(model));

            return Build(typedModel);
        }

        object? IViewModelFactory.ProvideModel() => _model;
    }
}