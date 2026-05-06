using System;
using System.Collections.Generic;
using System.Text;

namespace Sachssoft.Sasopuls.Inspector
{
    public interface IInspectorConstraint
    {
        ValidationResult Validate(object? value, IInspectorProperty property);
    }
}
