using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Utilities;

namespace Utilities.Extensions
{
    public static class CharEx
    {
        public static int AsInt(this char c)
        {
            return Convert.ToInt32(c);
        }


        static readonly Regex _regexCyrlics = new Regex(@"\p{IsCyrillic}", RegexOptions.Compiled);
        public static bool IsCyrylic(this char s)
        {
            return _regexCyrlics.IsMatch(s.ToString());
        }
    }
}
