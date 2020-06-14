using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Interfaces
{
    public interface IFeatured<TBase>
    {
        T TryGetFeature<T>() where T : TBase;
    }
}
