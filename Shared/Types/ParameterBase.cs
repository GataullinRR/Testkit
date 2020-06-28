using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace SharedT.Types
{
    public abstract class ParameterBase
    {
        public string Name { get; set; }
        public NodeParameter? Parrent { get; set; }

        public string GetFullPath()
        {
            return Parrent == null 
                ? Name 
                : Parrent.GetFullPath() + "." + Name;
        }

        public IEnumerable<NodeParameter> GetParrents()
        {
            return getParrents().Reverse();
        }
        IEnumerable<NodeParameter> getParrents()
        {
            if (Parrent != null)
            {
                yield return Parrent;
                foreach (var parrent in Parrent.GetParrents())
                {
                    yield return parrent;
                }
            }
        }

        public string SerializeToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
        }
    }
}
