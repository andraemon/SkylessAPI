using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Linq;

namespace SkylessAPI.Utilities
{
    public static class Il2CppClassExtensions
    {
        #region IList Extensions
        /// <summary>
        /// Converts an <see cref="IList{T}"/> to a <see cref="System.Collections.Generic.IList{T}"/>.
        /// </summary>
        public static System.Collections.Generic.IList<T> ToManagedList<T>(this IList<T> list)
        {
            var managedList = new System.Collections.Generic.List<T>();
            var castList = list.TryCast<List<T>>();
            for (int i = 0; i < castList.Count; i++)
                managedList.Add(castList[i]);
            return managedList;
        }
        #endregion
    }
}
