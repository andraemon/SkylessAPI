using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SkylessAPI.Utilities
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class InvokeOnStart : Attribute { }

    internal static class StartupHelper
    {
        internal static void CallInvokeOnStart() =>
            Assembly.GetExecutingAssembly().GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
                .Where(m => m.GetCustomAttribute(typeof(InvokeOnStart)) != null).ToList().ForEach(m => m.Invoke(null, null));
    }
}
