using Sachssoft.Sasopuls.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sachssoft.Sasopuls.Inspector
{
    public sealed class InspectorValueKind : NamedType
    {
        //
        public static readonly InspectorValueKind Auto = new(nameof(Auto));

        //
        public static readonly InspectorValueKind Text = new(nameof(Text));
        public static readonly InspectorValueKind Spinner = new(nameof(Spinner));
        public static readonly InspectorValueKind Range = new(nameof(Range));
        public static readonly InspectorValueKind Dropdown = new(nameof(Dropdown));
        public static readonly InspectorValueKind List = new(nameof(List));
        public static readonly InspectorValueKind Checkbox = new(nameof(Checkbox));
        public static readonly InspectorValueKind Switch = new(nameof(Switch));

        public InspectorValueKind(string name) : base(name)
        {
        }
    }
}
