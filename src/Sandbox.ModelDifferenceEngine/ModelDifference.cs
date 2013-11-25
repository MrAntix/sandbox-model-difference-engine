using System;
using System.Collections.Generic;

namespace Sandbox.ModelDifferenceEngine
{
    public class ModelDifference
    {
        readonly IDictionary<Type, Func<object, object, bool>> _comparisons;

        public ModelDifference()
        {
            _comparisons = new Dictionary<Type, Func<object, object, bool>>();
        }

        public ModelSnapshot Snapshot(object data)
        {
            return new ModelSnapshot(data, _comparisons);
        }

        public ModelDifference RegisterComparison<T, TProp>(
            Func<T, TProp> exp)
        {
            _comparisons.Add(typeof (T), (a, b) => CompareEquals(a, b, exp));

            return this;
        }

        static bool CompareEquals<T, TProp>(
            object a, object b,
            Func<T, TProp> exp)
        {
            if (a == b) return true;
            if (a == null || b == null) return false;

            return Equals(exp((T) a), exp((T) b));
        }
    }
}