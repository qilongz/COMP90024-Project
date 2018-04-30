using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterUtil.Util
{
    public static class Extension
    {
        public static string Pack(this string txt)
        {
            return '"' + txt
                       .Replace("\t", @"\t")
                       .Replace("\r", @"\r")
                       .Replace("\n", @"\n")
                       .Replace("\"", "\"\"") + '"';
        }
    }
}
