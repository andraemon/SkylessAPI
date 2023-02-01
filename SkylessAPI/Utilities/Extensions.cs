using Failbetter.Core.QAssoc.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkylessAPI.Utilities
{
    public static class Extensions
    {
        #region IEnumerable Extensions
        /// <summary>
        /// Converts an <see cref="IList{T}"/> to a <see cref="Il2CppSystem.Collections.Generic.List{T}"/>.
        /// </summary>
        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this IList<T> list)
            => ToIl2CppList(list, s => s);

        /// <summary>
        /// Converts an <see cref="IList{T}"/> to a <see cref="Il2CppSystem.Collections.Generic.List{T}"/>, passing each item through a converter.
        /// </summary>
        public static Il2CppSystem.Collections.Generic.List<TOut> ToIl2CppList<TIn, TOut>(this IList<TIn> list, Func<TIn, TOut> converter)
        {
            var newList = new Il2CppSystem.Collections.Generic.List<TOut>();
            for (int i = 0; i < list.Count; i++)
                newList.Add(converter(list[i]));
            return newList;
        }

        /// <summary>
        /// Converts an <see cref="Il2CppSystem.Collections.Generic.IList{T}"/> to a <see cref="List{T}"/>.
        /// </summary>
        public static List<T> ToManagedList<T>(this Il2CppSystem.Collections.Generic.List<T> list)
        {
            var newList = new List<T>();
            for (int i = 0; i < list.Count; i++)
                newList.Add(list[i]);
            return newList;
        }

        /// <summary>
        /// Converts a <see cref="Il2CppSystem.Collections.Generic.List{T}"/> to an <see cref="Il2CppSystem.Collections.Generic.IList{T}"/>.
        /// </summary>
        public static Il2CppSystem.Collections.Generic.IList<T> ToIList<T>(this Il2CppSystem.Collections.Generic.List<T> list)
            => list.Cast<Il2CppSystem.Collections.Generic.IList<T>>();

        /// <summary>
        /// Converts an <see cref="Il2CppSystem.Collections.Generic.IList{T}"/> to a <see cref="Il2CppSystem.Collections.Generic.List{T}"/>.
        /// </summary>
        public static Il2CppSystem.Collections.Generic.List<T> ToList<T> (this Il2CppSystem.Collections.Generic.IList<T> iList)
        {
            var list = new Il2CppSystem.Collections.Generic.List<T>();
            for (int i = 0; i < iList.Cast<Il2CppSystem.Collections.Generic.ICollection<T>>().Count; i++)
            {
                list.Add(iList[i]);
            }
            return list;
        }
        #endregion

        #region JsonElement Extensions
        public static int Id(this JsonElement element, int offset, bool checkTargetMod = true)
        {
            var id = element.GetPropertyInt("Id");

            if (id < AddonAPI.ModIdCutoff)
            {
                return id;
            }
            if (checkTargetMod && element.TryGetProperty("TargetMod", out JsonElement targetMod))
            {
                return id + AddonAPI.GetOffset(targetMod.GetString());
            }
            return id + offset;
        }

        /// <returns>The value of the property, or an optionally specified default value if the property is not present.</returns>
        public static object GetPropertyValueOrDefault(this JsonElement element, string key, object @default = null)
        {
            if (element.TryGetProperty(key, out JsonElement el))
            {
                JsonValueKind valueKind = el.ValueKind;

                if (valueKind == JsonValueKind.Array) return el.GetList();
                if (valueKind == JsonValueKind.String) return el.GetString();
                if (valueKind == JsonValueKind.Number) return el.GetInt32();
                if (valueKind == JsonValueKind.True) return true;
                if (valueKind == JsonValueKind.False) return false;
                if (valueKind == JsonValueKind.Null || valueKind == JsonValueKind.Undefined) return null;
                return el;
            }

            return @default;
        }

        /// <returns>The property, or the default <see cref="JsonElement"/> if it's not present.</returns>
        public static JsonElement GetPropertyOrDefault(this JsonElement element, string key)
        {
            if (element.TryGetProperty(key, out JsonElement property))
            {
                return property;
            }

            return default;
        }

        public static List<JsonElement> GetList(this JsonElement element)
        {
            List<JsonElement> list = new List<JsonElement>();

            foreach (var item in element.EnumerateArray())
            {
                list.Add(item);
            }

            return list;
        }

        public static int GetPropertyInt(this JsonElement element, string key)
            => element.GetProperty(key).GetInt32();
        #endregion

        #region Regex Extensions
        /// <summary>
        /// Yields all matches in a given string.
        /// </summary>
        /// <param name="regex"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<Match> YieldMatches(this Regex regex, string text)
        {
            Match match = regex.Match(text);
            yield return match;
            while (match.Success)
            {
                match = match.NextMatch();
                yield return match;
            }
        }
        #endregion
    }
}
