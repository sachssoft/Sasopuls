using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Sachssoft.Sasopuls.Inspector.Constraints
{
    public class EnumConstraint<T> : IInspectorConstraint
        where T : struct, Enum
    {
        public IReadOnlyList<EnumField<T>> Values { get; init; } = Array.Empty<EnumField<T>>();

        public bool IsFlags => typeof(T).IsDefined(typeof(FlagsAttribute), false);

        public ValidationResult Validate(object? value, IInspectorProperty property)
        {
            if (value is not T typed)
            {
                return ValidationResult.Fail(
                    new InvalidOperationException($"Expected enum {typeof(T).Name}"));
            }

            if (Values.Count == 0)
                return ValidationResult.Success();

            if (!IsFlags)
            {
                if (!Values.Any(v => EqualityComparer<T>.Default.Equals(v.Value, typed)))
                {
                    return ValidationResult.Fail(
                        new InvalidOperationException($"Value '{typed}' is not allowed"));
                }

                return ValidationResult.Success();
            }

            // FLAGS MODE
            ulong allowedBits = 0;

            foreach (var v in Values)
                allowedBits |= Convert.ToUInt64(v.Value);

            ulong actual = Convert.ToUInt64(typed);

            if ((actual & ~allowedBits) != 0)
            {
                return ValidationResult.Fail(
                    new InvalidOperationException($"Value '{typed}' contains invalid flags"));
            }

            return ValidationResult.Success();
        }
    }
}
