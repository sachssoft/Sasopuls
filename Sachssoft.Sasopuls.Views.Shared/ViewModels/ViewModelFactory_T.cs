using Sachssoft.Sasofly.Pulse.ViewModels;

using System;

/// <summary>
/// Factory for creating ViewModels and optionally providing their underlying Models.
/// </summary>
public sealed class ViewModelFactory<TModel> : IViewModelFactory
    where TModel : class
{
    private readonly Func<TModel, ViewModelBase<TModel>> _viewModelFactory;
    private readonly TModel? _model;

    private ViewModelFactory(
        Func<TModel, ViewModelBase<TModel>> viewModelFactory,
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

    public static ViewModelFactory<TModel> Create(Func<TModel, ViewModelBase<TModel>> viewModelFactory)
    {
        return new ViewModelFactory<TModel>(viewModelFactory);
    }

    public static ViewModelFactory<TModel> Create(Func<TModel, ViewModelBase<TModel>> viewModelFactory,
                                                  Func<TModel> modelFactory)
    {
        return new ViewModelFactory<TModel>(viewModelFactory, modelFactory: modelFactory);
    }

    public static ViewModelFactory<TModel> Create(Func<TModel, ViewModelBase<TModel>> viewModelFactory,
                                                      TModel sourceModel)
    {
        return new ViewModelFactory<TModel>(viewModelFactory, sourceModel: sourceModel);
    }

    public ViewModelBase<TModel> Build(TModel model)
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