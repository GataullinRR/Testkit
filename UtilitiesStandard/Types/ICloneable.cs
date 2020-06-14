using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Types
{
    public interface ICloneable<T>
    {
        T Clone();
    }
}
