using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.Utilities
{
    public static class ClassExtensions
    {
        #region IEnumerable Extensions
        /// <summary>
        /// Extension to allow left outer joins on sequences.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="result">A function to create a result element from two matching elements. Must handle null cases on the second parameter.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that has elements of type <typeparamref name="TResult"/> 
        /// that are obtained by performing a left outer join on two sequences.</returns>
        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> result)
        {
            return from o in outer
                   join i in inner on outerKeySelector(o) equals innerKeySelector(i) into igroup
                   from item in igroup.DefaultIfEmpty()
                   select result(o, item);
        }

        /// <summary>
        /// Extension to allow right outer joins on sequences, excluding joined elements.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="result">A function to create a result element from two matching elements. Must handle null cases on the first parameter.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that has elements of type <typeparamref name="TResult"/> 
        /// that are obtained by performing an exclusive right outer join on two sequences.</returns>
        public static IEnumerable<TResult> ExclusiveRightOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> result)
        {
            return from i in inner
                   join o in outer on innerKeySelector(i) equals outerKeySelector(o) into ogroup
                   from otem in ogroup.DefaultIfEmpty()
                   where otem == null
                   select result(otem, i);
        }

        /// <summary>
        /// Extension to allow full outer joins on sequences.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="result">A function to create a result element from two matching elements. Must handle null cases.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that has elements of type <typeparamref name="TResult"/> 
        /// that are obtained by performing a full outer join on two sequences.</returns>
        public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> result)
        {
            return LeftOuterJoin(outer, inner, outerKeySelector, innerKeySelector, result)
                .Concat(ExclusiveRightOuterJoin(outer, inner, outerKeySelector, innerKeySelector, result));
        }
        #endregion

        #region List Extensions
        /// <summary>
        /// Searches for elements which match the conditions defined by the specified predicate, 
        /// and returns an array of the zero-based indices of all occurrences within the entire <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the list.</typeparam>
        /// <param name="self">The list to search.</param>
        /// <param name="match">The <see cref="Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>An <see cref="Array"/> of zero-based indices of all elements matching the conditions defined by <paramref name="match"/>.</returns>
        public static int[] FindIndices<T>(this List<T> self, Predicate<T> match)
        {
            int index = -1;
            List<int> result = new List<int>();

            while (true)
            {
                index = self.FindIndex(index + 1, match);
                if (index == -1) return result.ToArray();
                else result.Add(index);
            }
        }
        #endregion

        #region Dictionary Extensions
        /// <summary>
        /// Moves all key-value pairs from one dictionary into another, overwriting duplicate keys in the original.
        /// </summary>
        public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictTo, Dictionary<TKey, TValue> dictFrom) =>
            dictFrom.ToList().ForEach(delegate (KeyValuePair<TKey, TValue> item) { dictTo[item.Key] = item.Value; });
        #endregion

        #region String Extensions
        /// <summary>
        /// Faster implementation of the String.Contains() method, because why not.
        /// </summary>
        /// <param name="self">The string to search.</param>
        /// <param name="value">The string to seek.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter occurs within this string; otherwise, <c>false</c>.</returns>
        public static bool FastContains(this string self, string value)
        {
            return ((self.Length - self.Replace(value, string.Empty).Length) / value.Length) > 0;
        }

        /// <summary>
        /// Tests if a string contains all values in an array. Uses a faster implementation of the String.Contains() method.
        /// </summary>
        /// <param name="self">The string to search.</param>
        /// <param name="values">The strings to seek.</param>
        /// <returns><c>true</c> if all strings in the <paramref name="values"/> parameter occur within this string; otherwise, <c>false</c>.</returns>
        public static bool FastContainsMany(this string self, string[] values)
        {
            int c = 0;
            for (int i = 0; i < values.Length; i++)
            {
                c += self.FastContains(values[i]) ? 1 : 0;
            }
            if (c == values.Length) return true;
            return false;
        }

        /// <summary>
        /// Tests if a string contains any value in an array. Uses a faster implementation of the String.Contains() method.
        /// </summary>
        /// <param name="self">The string to search.</param>
        /// <param name="values">The strings to seek.</param>
        /// <returns><c>true</c> if any string in the <paramref name="values"/> parameter occurs within this string; otherwise, <c>false</c>.</returns>
        public static bool FastContainsAny(this string self, string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (self.FastContains(values[i])) return true;
            }
            return false;
        }
        #endregion
    }
}
