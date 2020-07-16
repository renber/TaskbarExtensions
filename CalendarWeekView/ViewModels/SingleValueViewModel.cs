using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWeekView.ViewModels
{
    class SingleValueViewModel<T> : ViewModelBase
    {
        public T Value { get; }
        public string Description { get; }

        public SingleValueViewModel(T value, string description)
        {
            Value = value;
            Description = description;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
