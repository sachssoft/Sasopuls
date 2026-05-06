using Sachssoft.Sasopuls.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sachssoft.Sasopuls.Inspector.Constraints
{
    public record EnumField<T> 
        where T : struct, Enum
    {
        public required T Value { get; init; }

        public ILocalized? Title { get; init; }

        public ILocalized? Description { get; init; }
    }
}
