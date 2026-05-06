namespace Sachssoft.Sasopuls.Inspector
{
    public class InspectorPropertyChangedEventArgs
    {
        public InspectorPropertyChangedEventArgs(IInspectorProperty property)
        {
            Property = property;
        }

        public IInspectorProperty Property { get; }

        public IInspectorSchema Schema => Property.Schema;
    }
}
