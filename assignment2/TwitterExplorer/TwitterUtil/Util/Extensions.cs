using System;

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

        public static T ToEnum<T>(this string src) where T : struct, IConvertible
        {
            return (T) Enum.Parse(typeof(T), src, true);
        }
    }
}