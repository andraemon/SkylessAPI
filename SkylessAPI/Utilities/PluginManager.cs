using BepInEx;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkylessAPI.Utilities
{
    public static class PluginManager
    {
        internal static bool PluginsUpdated { get; private set; }
        internal static Dictionary<string, Plugin> Plugins { get; private set; }
        internal static Dictionary<string, Plugin> OldPlugins { get; private set; }

        /// <summary>
        /// Gets the name of a plugin by its GUID.
        /// </summary>
        /// <param name="guid">The GUID of the plugin.</param>
        /// <returns>The name of the plugin, or the GUID if the plugin can't be found.</returns>
        public static string PluginName(string guid)
        {
            if (Plugins.TryGetValue(guid, out var plugin))
                return plugin.Manifest.Name;
            return guid;
        }

        [InvokeOnStart]
        private static void Load()
        {
            var pluginListPath = Path.Combine(SkylessAPI.DataPath, "plugins.json");
            Plugins = GetCurrentPluginList();
            OldPlugins = GetOldPluginList();

            if (OldPlugins == null)
            {
                if (File.Exists(pluginListPath))
                    SkylessAPI.Logging.LogWarning("Could not read old plugin list, save cleanup will not be available if it has changed.");
                OldPlugins = new Dictionary<string, Plugin>();
            }

            PluginsUpdated = ArePluginsUpdated() || SkylessAPI.AlwaysMergeRepos.Value;

            AddonAPI.Load();

            File.WriteAllText(pluginListPath, JsonSerializer.Serialize(Plugins, SkylessAPI.JsonOptions));
        }

        #region Helper Methods
        private static bool ArePluginsUpdated()
        {
            if (Plugins.Count != OldPlugins.Count) return true;

            foreach (var plugin in Plugins)
                if (!OldPlugins.TryGetValue(plugin.Key, out var oldPlugin) || plugin.Value.Manifest.Version != oldPlugin.Manifest.Version)
                    return true;

            return false;
        }

        private static Dictionary<string, Plugin> GetCurrentPluginList()
        {
            var plugins = new List<Plugin>();
            foreach (var directory in GetPluginDirectories())
            {
                try
                {
                    var manifest = JsonSerializer.Deserialize<PluginManifest>(File.ReadAllText(Path.Combine(directory, "manifest.json")), SkylessAPI.JsonOptions);

                    plugins.Add(new Plugin(manifest, directory));
                }
                catch (Exception e)
                {
                    SkylessAPI.Logging.LogError($"Could not read manifest at {directory}.");
                    SkylessAPI.Logging.LogDebug(e);
                }
            }

            return ResolvePluginCollisions(plugins).ToDictionary(p => p.Manifest.Guid);
        }

        private static Dictionary<string, Plugin> GetOldPluginList()
        {
            var pluginListPath = Path.Combine(SkylessAPI.DataPath, "plugins.json");

            if (File.Exists(pluginListPath))
            {
                try
                {
                    var oldPlugins = JsonSerializer.Deserialize<Dictionary<string, Plugin>>(File.ReadAllText(pluginListPath), SkylessAPI.JsonOptions);

                    return oldPlugins;
                }
                catch (Exception e)
                {
                    SkylessAPI.Logging.LogDebug(e);
                }
            }

            return null;
        }

        private static HashSet<string> GetPluginDirectories()
            => Directory.GetFiles(Paths.PluginPath, "manifest.json", SearchOption.AllDirectories)
                .Select(a => Path.GetDirectoryName(a)).Concat(Directory.GetFiles(SkylessAPI.SkylessDataPath, "manifest.json", SearchOption.AllDirectories)
                    .Select(a => Path.GetDirectoryName(a))).ToHashSet();

        private static IEnumerable<Plugin> ResolvePluginCollisions(IEnumerable<Plugin> plugins)
            => plugins.GroupBy(
                plugin => plugin.Manifest.Guid,
                plugin => plugin,
                (guid, versions) => versions.OrderByDescending(v => v.Manifest.Version).First());
        #endregion

        internal class Plugin
        {
            public Plugin(PluginManifest manifest, string directory)
            {
                Manifest = manifest;
                Directory = directory;
            }
            
            [JsonConstructor]
            public Plugin(PluginManifest Manifest, AddonAPI.Addon Addon)
            {
                this.Manifest = Manifest;
                this.Addon = Addon;
            }

            public HashSet<string> AllDependencies()
                => Manifest.Dependencies.Union(Manifest.HardDependencies).ToHashSet();

            [JsonIgnore]
            public string Directory { get; }
            public PluginManifest Manifest { get; }
            public AddonAPI.Addon Addon { get; set; }
        }

        internal class PluginManifest
        {
            public PluginManifest(string Guid, string Name, string Version, HashSet<string> Dependencies, HashSet<string> HardDependencies, HashSet<string> UpdateKeys)
            {
                this.Guid = Guid;
                this.Name = Name;
                this.Version = Version;
                this.Dependencies = Dependencies ?? new HashSet<string>();
                this.HardDependencies = HardDependencies ?? new HashSet<string>();
                this.UpdateKeys = UpdateKeys ?? new HashSet<string>();
            }

            public bool Validate()
            {
                if (Guid.IsNullOrWhiteSpace() || Version.IsNullOrWhiteSpace()) return false;
                return true;
            }

            public string Guid { get; }
            public string Name { get; }
            public string Version { get; }
            public HashSet<string> Dependencies { get; }
            public HashSet<string> HardDependencies { get; }
            public HashSet<string> UpdateKeys { get; }
        }
    }
}
