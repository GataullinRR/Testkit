using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedT.Types
{
    public class NodeParameter : ParameterBase
    {
        ParameterBase[] _children = new ParameterBase[0];
        public ParameterBase[] Children
        {
            get => _children;
            set
            {
                foreach (var child in _children)
                {
                    child.Parrent = null;
                }
                _children = value;
                foreach (var child in _children)
                {
                    child.Parrent = this;
                }
            }
        } 

        public IEnumerable<LeafParameter> GetAllLeafs()
        {
            foreach (var child in Children)
            {
                if (child is LeafParameter leaf)
                {
                    yield return leaf;
                }
                else if (child is NodeParameter node)
                {
                    foreach (var leaf2 in node.GetAllLeafs())
                    {
                        yield return leaf2;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public IEnumerable<Difference> CompareWith(NodeParameter tree)
        {
            var aLeafs = GetAllLeafs();
            var bLeafs = tree.GetAllLeafs().ToList();
            foreach (var aLeaf in aLeafs)
            {
                var bLeaf = bLeafs.FirstOrDefault(b => b.GetFullPath() == aLeaf.GetFullPath());

                if (bLeaf != null)
                {
                    if (bLeaf.Value != aLeaf.Value)
                    {
                        yield return new Difference()
                        {
                            Kind = DiferenceKind.ValueMismatch,
                            P1 = aLeaf,
                            P2 = bLeaf
                        };
                    }
                    
                    bLeafs.Remove(bLeaf);
                }
                else
                {
                    yield return new Difference()
                    {
                        Kind = DiferenceKind.P2Missing,
                        P1 = aLeaf,
                        P2 = null
                    };
                }
            }

            foreach (var bLeaf in bLeafs)
            {
                yield return new Difference()
                {
                    Kind = DiferenceKind.P2Created,
                    P1 = null,
                    P2 = bLeaf
                };
            }
        }

        //public Dictionary<string, string?> AsDictionary()
        //{
        //    var dictionary = new Dictionary<string, string?>();
        //    ddd(this);

        //    return dictionary;

        //    void ddd(ParameterBase root, string ns = null)
        //    {
        //        ns = ns == null
        //            ? root.Name
        //            : ns;
        //        if (root is NodeParameter node)
        //        {
        //            var counts = new Dictionary<string, int>();
        //            foreach (var child in node.Children)
        //            {
        //                var key = ns + "." + child.Name;
        //                counts[key] = counts.GetValueOrDefault(key, 0) + 1;

        //                ddd(child, key + $"[{counts[key] - 1}]");
        //            }
        //        }
        //        else if (root is LeafParameter leaf)
        //        {
        //            dictionary.Add(ns, leaf.Value);
        //        }
        //    }
        //}

        //public static NodeParameter FromDictionary(IDictionary<string, string> dictionary)
        //{
        //    dictionary.Keys.SelectMany()

        //}

        public static NodeParameter? TryDeserialize(string jsonSerialized)
        {
            try
            {
                return jsonSerialized == null
                    ? null
                    : JsonConvert.DeserializeObject<NodeParameter>(jsonSerialized, new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.All
                    });
            }
            catch
            {
                return null;
            }
        }
    }
}
