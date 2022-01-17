using Mono.Cecil;
using SkylessAPI.ModInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.Utilities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class SkylessSubmodule : Attribute { }

    /// <summary>
    /// Attribute to have at the top of your BasePlugin class if you want to load a specific SkylessAPI submodule.
    /// Parameter(s) are the names of the submodules,
    /// e.g. [SkylessSubmoduleDependency(nameof(AddonAPI))]
    /// </summary>
    public class SkylessSubmoduleDependency : Attribute
    {
        public SkylessSubmoduleDependency(params string[] submodules)
        {
            Submodules = submodules;
        }

        public string[] Submodules { get; }
    }

    public static class SubmoduleManager
    {
        private static HashSet<string> Dependencies;
        private static HashSet<string> LoadedSubmodules;

        /// <summary>
        /// Return true if the specified submodule is loaded.
        /// </summary>
        /// <param name="submodule">Name of the submodule.</param>
        public static bool IsLoaded(string submodule) => LoadedSubmodules.Contains(submodule);

        [InvokeOnStart]
        private static void LoadSubmodules()
        {
            LoadedSubmodules = new HashSet<string>();
            ScanPluginDependencies();
            if (Dependencies.Count == 0) return;
            var modulesToLoad = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute(typeof(SkylessSubmodule)) != null && Dependencies.Contains(t.Name)).ToHashSet();
            foreach (Type module in modulesToLoad)
            {
                if (!(bool)module.GetField("_loaded", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null))
                {
                    module.GetMethod("Load", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
                    SkylessAPI.Logging.LogInfo($"Loaded SkylessAPI submodule {module.Name}");
                    LoadedSubmodules.Add(module.Name);
                }
            }
        }

        private static void ScanPluginDependencies()
        {
            Dependencies = new HashSet<string>();

            foreach (AssemblyDefinition assembly in PluginManager.PluginAssemblies)
                Dependencies.UnionWith(assembly.MainModule.Types.SelectMany(delegate (TypeDefinition type)
                    {
                        var modules = new List<string>();

                        foreach (CustomAttribute att in type.CustomAttributes)
                            if (att.AttributeType.FullName == typeof(SkylessSubmoduleDependency).FullName)
                                modules.AddRange(att.ConstructorArguments.SelectMany(d => (CustomAttributeArgument[])d.Value).Select(c => (string)c.Value));

                        return modules;
                    })
                );
        }
    }
}
