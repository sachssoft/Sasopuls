using System;

namespace Sachssoft.Sasopuls.Models
{
    public class SortSequenceItem<T>
    {
        private readonly string _name;
        private readonly Func<T, IComparable> _selector;

        public SortSequenceItem(
            string name,
            Func<T, IComparable> selector
        )
        {
            _name = name;
            _selector = selector;
        }

        public string Name => _name;

        public Func<T, IComparable> Selector => _selector;
    }
}
