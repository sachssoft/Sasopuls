using System;
using System.Collections.Generic;
using System.Text;

namespace Sachssoft.Sasopuls
{
    public abstract class NamedType : IEquatable<NamedType>
    {
        protected NamedType(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public override string ToString() => Name;

        public override bool Equals(object? obj)
            => obj is NamedType other && Name == other.Name && GetType() == other.GetType();

        public bool Equals(NamedType? other)
            => other != null && Name == other.Name && GetType() == other.GetType();

        public override int GetHashCode()
            => HashCode.Combine(Name, GetType());
    }
}
