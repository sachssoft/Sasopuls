using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Sachssoft.Sasopuls.Models;
using System;

namespace Sachssoft.Sasopuls.Views
{

    public class LocalizedExtension : MarkupExtension
    {
        public ILocalized? Localized { get; }

        public LocalizedExtension()
        {
        }

        public LocalizedExtension(ILocalized localized)
        {
            Localized = localized;
        }

        // Hinweis:
        // Diese Vorprüfung ist nicht zuverlässig, da TryGetResource nur den aktuellen Resource-Scope prüft.
        // Resources können z. B. durch Theme- oder Dictionary-Wechsel später verfügbar werden,
        // werden hier aber fälschlich als "nicht vorhanden" bewertet.
        // Daher kann es zu inkonsistentem Verhalten zwischen Initial- und Runtime-Auflösung kommen.
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Localized == null)
                return string.Empty;

            var key = Localized.Key;

            var target = serviceProvider.GetService(typeof(IProvideValueTarget))
                as IProvideValueTarget;

            if (target?.TargetObject is StyledElement styledElement)
            {
                if (styledElement.TryGetResource(key, null, out var value))
                {
                    return new DynamicResourceExtension(key);
                }
            }

            return Localized.Fallback ?? string.Empty;
        }
    }
}