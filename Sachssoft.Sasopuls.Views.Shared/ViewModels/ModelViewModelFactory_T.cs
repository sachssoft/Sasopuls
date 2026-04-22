using System;

namespace Sachssoft.Sasopuls.ViewModels
{

    /// <summary>
    /// Factory for creating ViewModels and optionally providing their underlying Models.
    /// </summary>
    public sealed class ModelViewModelFactory<TModel> :
        IViewModelFactory, IViewModelFactoryContextProvider, IViewModelTypeProvider, IViewModelCreator
        where TModel : class
    {
        private readonly Func<TModel, ModelViewModelBase<TModel>> _viewModelFactory;
        private readonly Func<TModel>? _modelFactory;
        private readonly TModel? _model;
        private readonly IViewModelFactoryContext? _context;

        private ModelViewModelFactory(
            Func<TModel, ModelViewModelBase<TModel>> viewModelFactory,
            Func<TModel>? modelFactory = null,
            TModel? sourceModel = null,
            IViewModelFactoryContext? context = null)
        {
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
            _modelFactory = modelFactory;
            _context = context;

            if (_modelFactory != null)
            {
                _model = _modelFactory();
            }
            else
            {
                _model = sourceModel;
            }
        }

        public Type ModelType { get; } = typeof(TModel);

        public Type ViewModelType { get; } = typeof(ModelViewModelBase<TModel>);

        public IViewModelFactoryContext? Context => _context;

        public static ModelViewModelFactory<TModel> Create(Func<TModel, ModelViewModelBase<TModel>> viewModelFactory)
        {
            return new ModelViewModelFactory<TModel>(viewModelFactory);
        }

        public static ModelViewModelFactory<TModel> Create(Func<TModel, ModelViewModelBase<TModel>> viewModelFactory, IViewModelFactoryContext? context)
        {
            return new ModelViewModelFactory<TModel>(viewModelFactory, context: context);
        }

        public static ModelViewModelFactory<TModel> Create(Func<TModel, ModelViewModelBase<TModel>> viewModelFactory,
                                                      Func<TModel> modelFactory)
        {
            return new ModelViewModelFactory<TModel>(viewModelFactory, modelFactory: modelFactory);
        }

        public static ModelViewModelFactory<TModel> Create(Func<TModel, ModelViewModelBase<TModel>> viewModelFactory,
                                                      Func<TModel> modelFactory, IViewModelFactoryContext? context)
        {
            return new ModelViewModelFactory<TModel>(viewModelFactory, modelFactory: modelFactory, context: context);
        }

        public static ModelViewModelFactory<TModel> Create(Func<TModel, ModelViewModelBase<TModel>> viewModelFactory,
                                                          TModel sourceModel)
        {
            return new ModelViewModelFactory<TModel>(viewModelFactory, sourceModel: sourceModel);
        }

        public static ModelViewModelFactory<TModel> Create(Func<TModel, ModelViewModelBase<TModel>> viewModelFactory,
                                                          TModel sourceModel, IViewModelFactoryContext? context)
        {
            return new ModelViewModelFactory<TModel>(viewModelFactory, sourceModel: sourceModel, context: context);
        }

        // Version 1.1.2
        public ModelViewModelBase<TModel> CreateFromModel()
        {
            if (_model is null)
                throw new InvalidOperationException(
                    "No stored model is available. Ensure a model was provided during factory creation.");

            return _viewModelFactory(_model);
        }

        // Version 1.1.2
        public ModelViewModelBase<TModel> CreateViewModel()
        {
            if (_modelFactory is null)
                throw new InvalidOperationException(
                    $"{nameof(ModelViewModelFactory<TModel>)} cannot create a ViewModel because no model factory was provided. " +
                    "Use a factory overload that provides a model factory.");

            var model = _modelFactory();

            return _viewModelFactory(model);
        }

        // Version 1.1.2
        ViewModelBase IViewModelCreator.CreateViewModel() => CreateViewModel();

        public ModelViewModelBase<TModel> Build(TModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            return _viewModelFactory(model);
        }

        // Gibt das bereitgestellte oder intern gespeicherte Model zurück, falls vorhanden.
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