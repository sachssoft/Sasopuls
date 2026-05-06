using System;
using System.Numerics;

namespace Sachssoft.Sasopuls.Inspector.Constraints
{
    public class RangeConstraint<T> : IInspectorConstraint
        where T : struct, INumber<T>
    {
        public T MinValue { get; init; }
        public T MaxValue { get; init; }

        public ValidationResult Validate(object? value, IInspectorProperty property)
        {
            if (value is not T typed)
            {
                return ValidationResult.Fail(
                    new InvalidOperationException($"Expected {typeof(T).Name}"));
            }

            if (typed < MinValue || typed > MaxValue)
            {
                return ValidationResult.Fail(
                    new ArgumentOutOfRangeException(property.Name),
                    suggestion: MinValue);
            }

            return ValidationResult.Success();
        }
    }
}