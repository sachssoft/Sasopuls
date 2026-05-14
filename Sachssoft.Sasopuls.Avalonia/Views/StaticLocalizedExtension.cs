using Avalonia;
using Avalonia.Markup.Xaml;
using Sachssoft.Sasopuls.Models;
using System;

namespace Sachssoft.Sasopuls.Views
{
    public class StaticLocalizedExtension : MarkupExtension
    {
        public ILocalized? Localized { get; }

        public StaticLocalizedExtension() { }

        public StaticLocalizedExtension(ILocalized localized)
        {
            Localized = localized;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Localized == null)
                return string.Empty;

            var target = serviceProvider.GetService(typeof(IProvideValueTarget))
                as IProvideValueTarget;

            if (target?.TargetObject is StyledElement styledElement &&
                styledElement.TryGetResource(Localized.Key, null, out var value) &&
                value is string text)
            {
                return text;
            }

            return Localized.Fallback ?? string.Empty;
        }
    }
}
