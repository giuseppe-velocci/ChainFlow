using System.Text.RegularExpressions;

namespace ChainFlow.Helpers
{
    internal static class FilenameSanitizer
    {
        private static readonly char[] _whitelist = {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',

            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z',

            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
            'u', 'v', 'w', 'x', 'y', 'z',

            '-', '_'
        };

        private static readonly int _maxLen = 64;
        private static readonly char altChar = '_';
        private static readonly string altString = altChar.ToString();
        private static readonly Regex replaceRegex = new("_{2,}");

        public static string Sanitize(string filename) 
        {
            var sanitizedString = string.Join("", filename.Select(x => _whitelist.Contains(x) ? x : altChar));
            return sanitizedString?.Any() is true ?
                Normalize(sanitizedString)
                : "_";
        }

        private static string Normalize(string sanitizedString)
        {
            var output = replaceRegex.Replace(sanitizedString, "");
            if (output?.Any() is true)
            {
                return output.Length > _maxLen ?
                    output[.._maxLen] : 
                    output;
            }
            else
            {
                return altString;
            }
        }
    }
}
