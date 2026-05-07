using System;
using System.Collections.Generic;
using System.Text;

namespace Sachssoft.Sasopuls.Inspector
{
    public interface IInspectorSchema : IEnumerable<IInspectorProperty>
    {
        object Owner { get; }

        IReadOnlyDictionary<string, IInspectorProperty> Properties { get; }

        bool TryGet(string name, out IInspectorProperty? property);
    }
}
