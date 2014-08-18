using System;

namespace Hanno.Validation.Rules.Common
{
    public class RangeRule<T> : CompositeValidationRule<T> where T : IComparable<T>
    {
        public RangeRule()
            : base(new GreaterThanRule<T>(),
                   new LowerThanRule<T>())
        {
        }

        public T LowerBound
        {
            get { return ((GreaterThanRule<T>) base.Rules[0]).GreaterThanValue; }
            set { ((GreaterThanRule<T>) base.Rules[0]).GreaterThanValue = value; }
        }

        public T HigherBound
        {
            get { return ((LowerThanRule<T>)base.Rules[1]).LowerThanValue; }
            set { ((LowerThanRule<T>)base.Rules[1]).LowerThanValue = value; }
        }
    }
}