using Sachssoft.Sasopuls.Models;

namespace Sachssoft.Sasopuls.Inspector
{
    // Abgeleitet erlaubt z.B. erweiterte Eigenschaften
    public class InspectorPropertyCategory : NamedType
    {
        public static readonly InspectorPropertyCategory Miscellaneous = new(nameof(Miscellaneous));

        public InspectorPropertyCategory(string name) : base(name)
        {
        }

        public ILocalized? Title { get; init; }

        public ILocalized? Description { get; init; }

        public ILocalized? Icon { get; init; }

        public bool IsToggleable { get; init; }
    }
}
