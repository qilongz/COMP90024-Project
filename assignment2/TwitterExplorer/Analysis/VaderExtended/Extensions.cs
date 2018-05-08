using System.Linq;

namespace VaderExtended
{
    internal static class Extensions
    {
        /// <summary>
        /// Determine if word is ALL CAPS
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsUpper(this string word)
        {
            return !word.Any(char.IsLower);
        }

       
        
    }
}
