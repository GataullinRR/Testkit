using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace TBDecoding
{
    class StepValidator
    {
        class DesiredValue
        {
            public string Key;
            public List<double> DesiredListOfDoubles;
            public string DesiredString;
            public bool Validated { get; private set; }

            public void SetValidated()
            {
                Validated = true;
            }
        }

        Dictionary<string, Dictionary<string, DesiredValue>> _desiredValues = 
            new Dictionary<string, Dictionary<string, DesiredValue>>();

        public bool SuppressValidation { get; set; }
        public bool SuppressBadKeys { get; set; }

        string _currentSession = null;
        public void BeginNewSession(string name)
        {
            _desiredValues.Add(name, new Dictionary<string, DesiredValue>());
            _currentSession = name;
        }
        public void BeginSession(string name)
        {
            _currentSession = name;
        }

        public void Store(string key, List<double> values)
        {
            _desiredValues[_currentSession].Add(key, new DesiredValue() { DesiredListOfDoubles = values } );
        }
        public void Store(string key, string values, char valuesSeparator)
        {
            Store(key, ParsingUtils.ParseDoubles(values, valuesSeparator));
        }

        public void Store(string key, string value)
        {
            _desiredValues[_currentSession].Add(key, new DesiredValue() { DesiredString = value });
        }

        public void Validate(string key, List<double> actualValues)
        {
            if (SuppressValidation || SuppressBadKeys && !_desiredValues[_currentSession].ContainsKey(key))
            {
                return;
            }

            var desired = _desiredValues[_currentSession][key];
            if (!desired.Validated)
            {
                var result = ArrayUtils.Compare(desired.DesiredListOfDoubles, actualValues, 1.0E-6);
                if (!result.IsMatch)
                {
                    throw new Exception();
                }

                desired.SetValidated();
            }
        }
        public void Validate(string key, string actualValue)
        {
            if (SuppressValidation || SuppressBadKeys && !_desiredValues[_currentSession].ContainsKey(key))
            {
                return;
            }

            var desired = _desiredValues[_currentSession][key];
            if (!desired.Validated)
            {
                var result = ArrayUtils.Compare(desired.DesiredString, actualValue);
                if (!result.IsMatch)
                {
                    throw new Exception();
                }

                desired.SetValidated();
            }
        }
    }
}
