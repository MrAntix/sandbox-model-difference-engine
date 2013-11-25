using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sandbox.ModelDifferenceEngine
{
    public class ModelSnapshot
    {
        readonly IDictionary<Type, Func<object, object, bool>> _comparisons;
        readonly IDictionary<string, object> _dictionary;

        public ModelChange[] GetChanges(object data)
        {
            var args = new ToDictionaryArguments(_comparisons, _dictionary);

            ToDictionary(
                data, null, string.Empty,
                args
                );

            return args.Changes.ToArray();
        }

        public ModelSnapshot(
            object data,
            IDictionary<Type, Func<object, object, bool>> comparisons)
        {
            _comparisons = comparisons;
            _dictionary = ToDictionary(data, comparisons);
        }

        public static IDictionary<string, object> ToDictionary(
            object data,
            IDictionary<Type, Func<object, object, bool>> comparers)
        {
            var args = new ToDictionaryArguments(comparers);

            ToDictionary(
                data, null, string.Empty,
                args
                );

            return args.Dictionary;
        }

        public static IDictionary<string, object> ToDictionary(
            object data)
        {
            return ToDictionary(data, new Dictionary<Type, Func<object, object, bool>>());
        }

        static void ToDictionary(
            object data, Type dataType, string path,
            ToDictionaryArguments args)
        {
            dataType = dataType ??
                       (data == null ? null : data.GetType());

            object oldData = null;
            var list = GetList(data, dataType);

            if (args.Dictionary.ContainsKey(path))
            {
                oldData = args.Dictionary[path];

                if (list == null
                    && !args.AreEqual(oldData, data, dataType))
                {
                    args.Changes.Add(
                        new ModelChange(path, oldData, data)
                        );
                }
            }
            else
            {
                if (args.Changes != null)
                {
                    args.Changes.Add(
                        new ModelChange(path, null, data)
                        );

                    return;
                }

                args.Dictionary.Add(path, list ?? data);
            }

            if (Equals(data, null)) return;

            if (dataType.IsPrimitive 
                || dataType == typeof (string)
                || args.Visited.Contains(data)) return;

            args.Visited.Add(data);

            if (list == null)
            {
                CheckProperties(
                    data, dataType, path,
                    args);
            }
            else if (oldData == null)
                CheckList(
                    list,
                    path, args);

            else
                CompareLists(
                    list,
                    (IEnumerable) oldData,
                    path,
                    args);
        }

        static IEnumerable<object> GetList(
            object data, Type dataType)
        {
            if (dataType == typeof (string)) return null;

            var list = data as IEnumerable;
            return list == null ? null : list.Cast<object>().ToArray();
        }

        static void CheckList(
            IEnumerable dataList, string path,
            ToDictionaryArguments args)
        {
            var i = 0;
            foreach (var data in dataList)
            {
                var pathIndex = string.Concat(path, "[", i++, "]");
                ToDictionary(
                    data, null, pathIndex,
                    args);
            }
        }

        static void CompareLists(
            IEnumerable<object> dataList,
            IEnumerable oldDataList,
            string path,
            ToDictionaryArguments args)
        {
            var newDataList = new List<object>(dataList);
            Type dataType = null;

            var i = 0;
            foreach (var oldData in oldDataList)
            {
                if (dataType == null) dataType = oldData.GetType();

                var newData = Find(oldData, dataType, newDataList, args);

                var pathIndex = string.Concat(path, "[", i++, "]");
                ToDictionary(
                    newData, null, pathIndex,
                    args);

                if (newData != null)
                    newDataList.Remove(newData);
            }

            foreach (var newData in newDataList)
            {
                if (dataType == null) dataType = newData.GetType();

                var pathIndex = string.Concat(path, "[", i++, "]");
                ToDictionary(
                    newData, null, pathIndex,
                    args);
            }
        }

        static object Find(
            object data, Type dataType,
            IEnumerable<object> list,
            ToDictionaryArguments args)
        {
            return list
                .FirstOrDefault(match => args.AreEqual(match, data, dataType));
        }

        static void CheckProperties(
            object data, Type dataType, string path,
            ToDictionaryArguments args)
        {
            var prefix = string.Concat(path, ".");
            foreach (var propertyInfo in dataType.GetProperties())
            {
                ToDictionary(
                    propertyInfo.GetValue(data), propertyInfo.PropertyType,
                    string.Concat(prefix, propertyInfo.Name),
                    args);
            }
        }

        class ToDictionaryArguments
        {
            readonly IDictionary<string, object> _dictionary;
            readonly ICollection<object> _visited;
            readonly IDictionary<Type, Func<object, object, bool>> _comparers;
            readonly ICollection<ModelChange> _changes;

            public ToDictionaryArguments(
                IDictionary<Type, Func<object, object, bool>> comparers,
                IDictionary<string, object> dictionary)
            {
                _comparers = comparers;
                _dictionary = dictionary;
                _visited = new Collection<object>();
                _changes = new Collection<ModelChange>();
            }

            public ToDictionaryArguments(
                IDictionary<Type, Func<object, object, bool>> comparers)
            {
                _comparers = comparers;
                _dictionary = new Dictionary<string, object>();
                _visited = new Collection<object>();
            }

            public IDictionary<string, object> Dictionary
            {
                get { return _dictionary; }
            }

            public ICollection<object> Visited
            {
                get { return _visited; }
            }

            public ICollection<ModelChange> Changes
            {
                get { return _changes; }
            }

            public bool AreEqual(object a, object b, Type type)
            {
                if (a == b) return true;

                if (a == null) return b == null;
                if (b == null) return false;

                type = type ?? a.GetType();

                return _comparers.ContainsKey(type)
                           ? _comparers[type](a, b)
                           : Equals(a, b);
            }
        }
    }
}