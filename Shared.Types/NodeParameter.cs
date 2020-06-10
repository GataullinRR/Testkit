using Newtonsoft.Json;
using System.Collections.Generic;

namespace Shared.Types
{
    public class NodeParameter : ParameterBase
    {
        public ParameterBase[] Children { get; set; } = new ParameterBase[0];

        public Dictionary<string, string?> AsDictionary()
        {
            var dictionary = new Dictionary<string, string?>();
            ddd(this);

            return dictionary;

            void ddd(ParameterBase root, string ns = null)
            {
                ns = ns == null
                    ? root.Name
                    : ns;
                if (root is NodeParameter node)
                {
                    var counts = new Dictionary<string, int>();
                    foreach (var child in node.Children)
                    {
                        var key = ns + "." + child.Name;
                        counts[key] = counts.GetValueOrDefault(key, 0) + 1;

                        ddd(child, key + $"[{counts[key] - 1}]");
                    }
                }
                else if (root is LeafParameter leaf)
                {
                    dictionary.Add(ns, leaf.Value);
                }
            }
        }

        public static NodeParameter Deserialize(string jsonSerialized)
        {
            return JsonConvert.DeserializeObject<NodeParameter>(jsonSerialized, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }
    }
}
