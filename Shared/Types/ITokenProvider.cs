using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharedT.Types
{
    public interface ITokenProvider
    {
        Task<string?> GetTokenAsync();
    }
}
