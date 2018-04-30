using System;
using System.Collections.Generic;

namespace TwitterUtil.Util
{
    public class DoOrder : IComparer<string>
    {
        public int Compare(string x, string y) => string.Compare(x, y, StringComparison.Ordinal);
    }
}