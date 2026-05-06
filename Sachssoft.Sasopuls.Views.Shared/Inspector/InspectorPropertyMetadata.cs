using Sachssoft.Sasopuls.Models;
using System;
using System.Collections.Generic;

namespace Sachssoft.Sasopuls.Inspector
{
    public class InspectorPropertyMetadata
    {

        #region 

        public ILocalized? Title { get; init; }

        public ILocalized? Description { get; init; }

        public InspectorPropertyCategory Category { get; init; }
            = InspectorPropertyCategory.Miscellaneous;

        public IReadOnlyList<IInspectorConstraint> Constraints { get; init; }
            = Array.Empty<IInspectorConstraint>();

        public InspectorValueKind ValueKind { get; init; }
            = InspectorValueKind.Auto;

        #endregion

        internal void Setup(IInspectorProperty property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            OnLoad(property);
        }

        protected virtual void OnLoad(IInspectorProperty property)
        {
        }
    }
}
