using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Sachssoft.Sasopuls.Models;
using System;

namespace Sachssoft.Sasopuls.Views;

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

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        //if (Localized == null)
        //    return string.Empty;

        //var key = Localized.Key;

        //var target = serviceProvider.GetService(typeof(IProvideValueTarget))
        //    as IProvideValueTarget;

        //if (target?.TargetObject is StyledElement styledElement)
        //{
        //    if (styledElement.TryGetResource(key, null, out var value))
        //    {
        //        if (value is string text)
        //            return text;
        //    }
        //}

        //return Localized.Fallback ?? string.Empty;

        if (Localized == null)
            return string.Empty;

        return new DynamicResourceExtension(Localized.Key);
    }
}