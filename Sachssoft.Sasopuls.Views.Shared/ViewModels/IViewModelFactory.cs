using System;

namespace Sachssoft.Sasopuls.ViewModels
{
    /// <summary>
    /// Factory interface for creating ViewModels and providing their underlying Models.
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        /// The type of the underlying model associated with this factory.
        /// </summary>
        // Gibt den Typ des Models an, das dieses ViewModel benötigt oder verwendet.
        Type ModelType { get; }

        /// <summary>
        /// Builds a new ViewModel instance from a given model.
        /// </summary>
        /// <param name="model">
        /// Optional model to associate with the ViewModel. Can be null if the ViewModel does not require a model.
        /// </param>
        /// <returns>A new ViewModelBase instance.</returns>
        // Baut ein ViewModel aus dem angegebenen Model
        ViewModelBase Build(object? model);

        /// <summary>
        /// Provides the underlying model, either a source instance or generated via a model provider.
        /// </summary>
        /// <returns>An object representing the model, or null if none is needed.</returns>
        // Liefert das Model: entweder SourceModel oder über ModelProvider erzeugt
        object? ProvideModel();
    }
}