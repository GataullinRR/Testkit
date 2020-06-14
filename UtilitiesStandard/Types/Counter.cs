using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public class CounterSettings
    {
        internal bool Statement { get; private set; } = true;
        internal bool DoBreakAfterCount { get; private set; } = false;

        public CounterSettings If(bool statement)
        {
            Statement &= statement;

            return this;
        }

        public CounterSettings BreakAfterCount()
        {
            DoBreakAfterCount = true;

            return this;
        }
    }

    public class Counter
    {
        #region ##### StaticHelpers #####

        public enum MergeMode
        {
            ADD,
        }

        internal static Counter Merge(IEnumerable<Counter> counters, MergeMode mergeMode)
        {
            var merged = new Counter();
            foreach (var counter in counters)
            {
                foreach (var counterInfo in counter._counters)
                {
                    merged.addCounterIfNotExist(counterInfo.Key);
                    switch (mergeMode)
                    {
                        case MergeMode.ADD:
                            merged._counters[counterInfo.Key] += counterInfo.Value;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            return merged;
        }

        #endregion

        #region ##### Global ######

        readonly static Dictionary<string, Dictionary<string, List<object>>> _globalValues = new Dictionary<string, Dictionary<string, List<object>>>();
        readonly static Dictionary<string, Dictionary<string, int>> _globalCounters = new Dictionary<string, Dictionary<string, int>>();

        public static void GAddItem(string arrayId, object value, [CallerFilePath]string owner = "")
        {
            addArrayIfNotExist(arrayId, owner);
            _globalValues[owner][arrayId].Add(value);
        }
        static void addArrayIfNotExist(string arrayId, string owner)
        {
            if (_globalValues.NotContainsKey(owner))
            {
                _globalValues.Add(owner, new Dictionary<string, List<object>>());
            }
            if (_globalValues[owner].NotContainsKey(arrayId))
            {
                _globalValues[owner].Add(arrayId, new List<object>());
            }
        }

        public static void GIncrement(string counterId, CounterSettings settings, [CallerFilePath]string owner = "")
        {
            if (settings.Statement)
            {
                GIncrement(counterId, owner);
                if (settings.DoBreakAfterCount)
                {
                    Debugger.Break();
                }
            }
        }

        public static void GIncrement(string counterId, [CallerFilePath]string owner = "")
        {
            addCounterIfNotExist(counterId, owner);
            _globalCounters[owner][counterId] += 1;
        }
        public static void GDecrement(string counterId, [CallerFilePath]string owner = "")
        {
            addCounterIfNotExist(counterId, owner);
            _globalCounters[owner][counterId] -= 1;
        }
        public static void GReset(string counterId, [CallerFilePath]string owner = "")
        {
            addCounterIfNotExist(counterId, owner);
            _globalCounters[owner][counterId] = 0;
        }

        public static void GIncrementIf(string counterId, bool statement, [CallerFilePath]string owner = "")
        {
            if (statement)
            {
                GIncrement(counterId, owner);
            }
        }
        public static void GDecrementIf(string counterId, bool statement, [CallerFilePath]string owner = "")
        {
            if (statement)
            {
                GDecrement(counterId, owner);
            }
        }
        public static void GResetIf(string counterId, bool statement, [CallerFilePath]string owner = "")
        {
            if (statement)
            {
                GReset(counterId, owner);
            }
        }

        static void addCounterIfNotExist(string counter, string owner)
        {
            if (_globalCounters.NotContainsKey(owner))
            {
                _globalCounters.Add(owner, new Dictionary<string, int>());
            }
            if (_globalCounters[owner].NotContainsKey(counter))
            {
                _globalCounters[owner].Add(counter, 0);
            }
        }

        /// <summary>
        /// Merges all counters of the <paramref name="counter"/> to global counter. 
        /// If there are global counters with the same name, they will be replaced.
        /// </summary>
        /// <param name="counter">Source</param>
        /// <param name="owner">Destination</param>
        public static void GMerge(Counter counter, [CallerFilePath]string owner = "")
        {
            foreach (var counterInfo in counter._counters)
            {
                addCounterIfNotExist(counterInfo.Key, owner);
                _globalCounters[owner][counterInfo.Key] = counterInfo.Value;
            }
        }

        public static void GWipe([CallerFilePath]string owner = "")
        {
            _globalCounters.Remove(owner);
            _globalValues.Remove(owner);
        }

        public static void GDump([CallerFilePath]string owner = "")
        {
            var str = GToString(owner);
            Console.WriteLine(str);
        }

        public static string GToString([CallerFilePath]string owner = "")
        {
            var sb = new StringBuilder();
            if (_globalValues.ContainsKey(owner))
            {
                foreach (var values in _globalValues[owner].OrderBy(kvp => kvp.Key))
                {
                    sb.AppendLine();
                    sb.Append(values.Key + ":");
                    foreach (var value in values.Value)
                    {
                        sb.Append(value + " ");
                    }
                }
            }
            if (_globalCounters.ContainsKey(owner))
            {
                sb.AppendLine();
                sb.Append(_globalCounters[owner].OrderBy(kvp => kvp.Key).AsMultilineString());
            }

            return sb.ToString();
        }

        #endregion

        #region ##### Instance ######

        readonly string _name;
        readonly Dictionary<string, int> _counters = new Dictionary<string, int>();

        public int this[string counter]
        {
            get
            {
                addCounterIfNotExist(counter);
                return _counters[counter];
            }
            set
            {
                addCounterIfNotExist(counter);
                _counters[counter] = value;
            }
        }

        public Counter() : this(null)
        {

        }
        public Counter(string name)
        {
            _name = name;
        }

        public void Increment(string counter)
        {
            addCounterIfNotExist(counter);
            _counters[counter] += 1;
        }
        public void Decrement(string counter)
        {
            addCounterIfNotExist(counter);
            _counters[counter] -= 1;
        }
        public void Reset(string counter)
        {
            addCounterIfNotExist(counter);
            _counters[counter] = 0;
        }

        void addCounterIfNotExist(string counter)
        {
            if (counter == null || !_counters.ContainsKey(counter))
            {
                _counters.Add(counter, 0);
            }
        }

        public void Dump()
        {
            var str = ToString();
            Console.WriteLine(str);
        }

        public override string ToString()
        {
            return new Enumerable<string>
            {
                $"Name:{_name}",
                _counters.OrderBy(c => c.Key).AsMultilineString()
            }.AsMultilineString();
        }

        #endregion
    }
}
