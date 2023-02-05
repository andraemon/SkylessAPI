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
        /// Performs a topological sort on a graph, throwing an error if there is a cyclic induced subgraph.
        /// </summary>
        /// <typeparam name="T">The type of the elements contained in the graph.</typeparam>
        /// <param name="vertexSource">A collection of vertices.</param>
        /// <param name="getSuccessors">A function which takes a vertex to a collection of its direct successors.</param>
        /// <returns>A list containing a topological sorting of the graph.</returns>
        /// <exception cref="ArgumentException">The graph contains a cyclic induced subgraph.</exception>
        public static List<T> TopologicalSort<T>(this IEnumerable<T> vertexSource, Func<T, IEnumerable<T>> getSuccessors)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in vertexSource)
            {
                Visit(item);
            }

            return sorted;

            void Visit(T item)
            {
                var alreadyVisited = visited.TryGetValue(item, out bool inProcess);

                if (alreadyVisited && inProcess)
                {
                    throw new ArgumentException("Source contains a cyclic induced subgraph.");
                }
                else
                {
                    visited[item] = true;

                    var successors = getSuccessors(item);
                    if (successors != null)
                    {
                        foreach (var successor in successors)
                        {
                            Visit(successor);
                        }
                    }

                    visited[item] = false;
                    sorted.Add(item);
                }
            }
        }

        /// <summary>
        /// Converts an <see cref="IList{T}"/> to a <see cref="Il2CppSystem.Collections.Generic.List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="list">The list to convert.</param>
        /// <returns>The converted list.</returns>
        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this IList<T> list)
        {
            var newList = new Il2CppSystem.Collections.Generic.List<T>();
            for (int i = 0; i < list.Count; i++)
                newList.Add(list[i]);
            return newList;
        }

        /// <summary>
        /// Converts an <see cref="Il2CppSystem.Collections.Generic.IList{T}"/> to a <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="list">The list to convert.</param>
        /// <returns>The converted list.</returns>
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
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="list">The list to convert.</param>
        /// <returns>The converted list.</returns>
        public static Il2CppSystem.Collections.Generic.IList<T> ToIList<T>(this Il2CppSystem.Collections.Generic.List<T> list)
            => list.Cast<Il2CppSystem.Collections.Generic.IList<T>>();

        /// <summary>
        /// Converts an <see cref="Il2CppSystem.Collections.Generic.IList{T}"/> to a <see cref="Il2CppSystem.Collections.Generic.List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the list.</typeparam>
        /// <param name="iList">The list to convert.</param>
        /// <returns>The converted list.</returns>
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
        /// <summary>
        /// Gets the actual ID of a <see cref="JsonElement"/> representing an entity.
        /// </summary>
        /// <param name="element">The value from which to get the ID.</param>
        /// <param name="offset">The offset of the mod from which the value originates.</param>
        /// <param name="checkTargetMod">Whether or not to check for a target mod.</param>
        /// <returns>The actual ID of the value.</returns>
        internal static int Id(this JsonElement element, int offset, bool checkTargetMod = true)
        {
            var id = element.GetProperty("Id").GetInt32();

            if (id < AddonAPI.ModIdCutoff)
            {
                return id;
            }
            if (checkTargetMod && element.TryGetProperty("TargetMod", out JsonElement targetMod))
            {
                return id + AddonAPI.IDOffset(targetMod.GetString());
            }
            return id + offset;
        }


        /// <summary>
        /// Gets the value of a property by its name, or an optionally specified default value if the property is not present.
        /// </summary>
        /// <param name="element">The value from which to get the property value.</param>
        /// <param name="property">The name of the property to get.</param>
        /// <param name="default">The default value to return if the property is not found.</param>
        /// <returns>The value of the property or the default value.</returns>
        public static object GetPropertyValueOrDefault(this JsonElement element, string property, object @default = null)
        {
            if (element.TryGetProperty(property, out JsonElement el))
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

        /// <summary>
        /// Gets a property by its name, or the default <see cref="JsonElement"/> if the property is not present.
        /// </summary>
        /// <param name="element">The value from which to get the property.</param>
        /// <param name="key">The name of the property.</param>
        /// <returns>The property, or the default <see cref="JsonElement"/>.</returns>
        public static JsonElement GetPropertyOrDefault(this JsonElement element, string key)
        {
            if (element.TryGetProperty(key, out JsonElement property))
            {
                return property;
            }

            return default;
        }

        /// <summary>
        /// Gets a list from a <see cref="JsonElement"/> whose value kind is <see cref="JsonValueKind.Array"/>.
        /// </summary>
        /// <param name="element">The value from which to get the list.</param>
        /// <returns>The list.</returns>
        public static List<JsonElement> GetList(this JsonElement element)
        {
            List<JsonElement> list = new List<JsonElement>();

            foreach (var item in element.EnumerateArray())
            {
                list.Add(item);
            }

            return list;
        }
        #endregion

        #region Regex Extensions
        /// <summary>
        /// Yields all matches in a given string.
        /// </summary>
        /// <param name="regex">The <see cref="Regex"/> instance to use.</param>
        /// <param name="text">The text to search.</param>
        /// <returns>An enumeration of all successful matches.</returns>
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
