using BepInEx;
using Mono.Cecil;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI.ModInterop
{
    internal static class PluginManager
    {
        private static HashSet<AssemblyDefinition> _pluginAssemblies;

        internal static HashSet<AssemblyDefinition> PluginAssemblies {
            get
            {
                if (_pluginAssemblies == null)
                {
                    SkylessAPI.Logging.LogDebug("Loading plugin assemblies...");
                    var assemblies = new List<AssemblyDefinition>();
                    foreach (string plugin in Directory.GetFiles(Paths.PluginPath, "*.dll", SearchOption.AllDirectories))
                    {
                        if (Path.GetFileName(plugin) == "SkylessAPI.dll") continue;

                        try {
                            assemblies.Add(AssemblyDefinition.ReadAssembly(plugin, BepInEx.Bootstrap.TypeLoader.ReaderParameters));
                        }
                        catch (Exception) {
                            SkylessAPI.Logging.LogDebug($"Cecil ReadAssembly couldn't read {plugin}");
                        }
                    }

                    _pluginAssemblies = ResolveDuplicatePlugins(assemblies);

                    foreach (AssemblyDefinition asas in _pluginAssemblies) SkylessAPI.Logging.LogDebug(asas.Name);
                }

                return _pluginAssemblies;
            }
        }

        private static HashSet<AssemblyDefinition> ResolveDuplicatePlugins(List<AssemblyDefinition> assemblies)
        {
            var plugins = new List<CustomAttribute>();

            foreach (AssemblyDefinition assembly in assemblies)
                plugins.AddRange(assembly.MainModule.Types.SelectMany(t => t.CustomAttributes)
                    .Where(c => c.AttributeType.FullName == typeof(BepInPlugin).FullName));

            return plugins.GroupBy(
                plugin => (string)plugin.ConstructorArguments[0].Value,
                plugin => plugin,
                (guid, versions) 
                    => versions.OrderByDescending(p => (string)p.ConstructorArguments[2].Value).First().AttributeType.Module.Assembly
            ).ToHashSet();
        }
    }
}
