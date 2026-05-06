namespace Sachssoft.Sasopuls.Inspector
{
    public class InspectorPropertyChangingEventArgs
    {
        public InspectorPropertyChangingEventArgs(IInspectorProperty property)
        {
            Property = property;
        }

        public bool Cancel { get; set; }

        public IInspectorProperty Property { get; }

        public IInspectorSchema Schema => Property.Schema;
    }
}
