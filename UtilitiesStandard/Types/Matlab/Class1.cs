using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace Utilities.Types.Matlab
{
    public interface IMatlabCodeEntity
    {
        string GenerateCode();
    }
    public class MatlabVariable : IMatlabCodeEntity
    {
        readonly StringBuilder _code = new StringBuilder();

        public MatlabVariable(string name, string value)
        {
            _code.Append($"{name} = [{value}]");
        }

        public MatlabVariable Transpose()
        {
            _code.Append("'");

            return this;
        }

        public string GenerateCode()
        {
            return _code.ToString() + ";";
        }
    }

    public class MatlabCodeGenerator
    {
        readonly List<IMatlabCodeEntity> _entities = new List<IMatlabCodeEntity>();

        public MatlabVariable AddVariable(string name, IEnumerable<double> value)
        {
            var variable = new MatlabVariable(name, MatlabUtils.ToArray(value).Aggregate());
            _entities.Add(variable);

            return variable;
        }
        public MatlabVariable AddVariable(string name, IEnumerable<IEnumerable<double>> value)
        {
            var variable = new MatlabVariable(name, MatlabUtils.ToArray(value).Aggregate());
            _entities.Add(variable);

            return variable;
        }
        public MatlabVariable AddVariable(string name, IEnumerable<int> value)
        {
            var variable = new MatlabVariable(name, MatlabUtils.ToArray(value).Aggregate());
            _entities.Add(variable);

            return variable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Matlab code string</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var entity in _entities)
            {
                sb.AppendLine(entity.GenerateCode());
            }

            return sb.ToString();
        }
    }
}
