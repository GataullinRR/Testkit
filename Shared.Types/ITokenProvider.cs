using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Types
{
    public interface ITokenProvider
    {
        string Token { get; set; }
    }
}
