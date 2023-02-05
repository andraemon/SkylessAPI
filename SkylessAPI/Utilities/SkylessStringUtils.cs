using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkylessAPI.Utilities
{
    public static class SkylessStringUtils
    {
        /// <summary>
        /// Matches all valid tokens in a string, and provides grouping of their internal elements.
        /// </summary>
        public static Regex TokenRegex { get; } = new Regex(@"\[([^:\]]+):([^\]]+)\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches a token value, and provides grouping of its internal elements.
        /// </summary>
        public static Regex TokenValueRegex { get; } = new Regex(@"^(?:\(([^\)]+)\))?([^\(]+)(?:\(([^\)]+)\))?$", RegexOptions.Compiled);

        /// <summary>
        /// Replaces raw mod IDs with offset IDs in all dynamic tokens.
        /// If an ID in a token cannot be parsed, it will be left as-is.
        /// </summary>
        /// <param name="text">The text to replace tokens within.</param>
        /// <param name="offset">The ID offset of the mod from which the string originated.</param>
        public static string ReplaceModIDsInTokens(this string text, int offset)
            => text == null ? null : TokenRegex.Replace(text, (Match match) => 
                $"[{match.Groups[1].Value}:{match.Groups[2].Value.ReplaceModIDInTokenValue(offset)}]");

        /// <summary>
        /// If there is a raw mod ID in the token, replaces it with an offset ID.
        /// If an ID cannot be parsed, no replacements will be made.
        /// </summary>
        /// <param name="tokenValue">The token to replace IDs within.</param>
        /// <param name="offset">The ID offset of the mod from which the token originated.</param>
        public static string ReplaceModIDInTokenValue(this string tokenValue, int offset)
        {
            if (tokenValue == null) return null;

            var match = TokenValueRegex.Match(tokenValue);
            
            if (match.Success)
            {
                var guid = match.Groups[1].Value;
                var idString = match.Groups[2].Value;
                var variable = match.Groups[3].Value;

                if (match.Groups[1].Value != string.Empty)
                {
                    if (AddonAPI.IsLoaded(guid) && int.TryParse(idString, out var id))
                    {
                        if (id >= AddonAPI.ModIdCutoff)
                            return $"{AddonAPI.ModID(id, guid)}{variable}";
                        return $"{idString}{variable}";
                    }
                }
                else if (int.TryParse(idString, out var id))
                {
                    if (id >= AddonAPI.ModIdCutoff)
                        return $"{id + offset}{variable}";
                    return $"{idString}{variable}";
                }
            }

            return tokenValue;
        }
    }
}
