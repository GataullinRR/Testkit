using System.Collections.Generic;
using System.Threading.Tasks;

namespace Runner
{
    public interface ICookieStorage 
    {
        Task SetAsync(string name, CookieValue? value);
        Task<CookieValue> GetAsync(string name);
    }
}
