using Failbetter.Core;
using HarmonyLib;
using Il2CppSystem.Linq;
using Skyless.Assets.Code.Skyless.Game.Data;
using Skyless.Assets.Code.Skyless.Utilities.Serialization;
using Skyless.Game.Data;
using SkylessAPI.ModInterop;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Event = Failbetter.Core.Event;

namespace SkylessAPI
{
    public static class AddonAPI
    {
        internal static List<string> LoadOrder { get; private set; }

        internal static bool AddonsUpdated { get; private set; }

        private static HashSet<string> RepoNames { get; } = new HashSet<string>() {
            "areas.json",
            "bargains.json",
            "domiciles.json",
            "events.json",
            "exchanges.json",
            "personas.json",
            "prospects.json",
            "qualities.json",
            "settings.json"
        };

        private static Regex IDRegex { get; } = new Regex(@"""Id""\s*:\s*(\d+)", RegexOptions.Compiled);

        public const int ModIdCutoff = 700000;

        #region Public API
        /// <summary>
        /// Gets the actual ID of an added game object.
        /// </summary>
        /// <param name="guid">The GUID of the plugin.</param>
        /// <param name="baseId">The original ID of the object.</param>
        /// <returns>The actual ID of an added game object, or -1 if the plugin cannot be found or has no addon.</returns>
        public static int ModID(int baseId, string guid)
        {
            if (PluginManager.Plugins.TryGetValue(guid, out var plugin) && plugin.Addon != null)
                return baseId + plugin.Addon.IdOffset;
            return -1;
        } 

        /// <summary>
        /// Gets the ID offset of an addon.
        /// </summary>
        /// <param name="guid">The GUID of the plugin.</param>
        /// <returns>The ID offset of the addon, if it can be found.</returns>
        public static int IDOffset(string guid)
        {
            if (PluginManager.Plugins.TryGetValue(guid, out var plugin) && plugin.Addon != null)
                return plugin.Addon.IdOffset;
            return -1;
        }

        /// <summary>
        /// Checks whether or not an addon is loaded.
        /// </summary>
        /// <param name="guid">The GUID of the plugin.</param>
        /// <returns>True if the addon is loaded, false otherwise.</returns>
        public static bool IsLoaded(string guid)
        {
            if (PluginManager.Plugins.TryGetValue(guid, out var plugin) && plugin.Addon != null)
                return plugin.Addon.Loaded;
            return false;
        }
        #endregion

        internal static void Load()
        {
            FindAddonsForPlugins();

            AddonsUpdated = AreAddonsUpdated();

            if (AddonsUpdated)
            {
                CalculateOffsets();
                LoadOrder = GetLoadOrder();
            }
            else LoadOrder = new List<string>();

            if (LoadOrder.Count > 0)
            {
                SkylessAPI.Logging.LogDebug("Addon load order:");
                LoadOrder.ForEach(s => SkylessAPI.Logging.LogDebug($" - {s}"));

                ApplyPatches();
            }

            SkylessAPI.Logging.LogInfo("Loaded AddonAPI");
        }

        private static void ApplyPatches()
        {
            SkylessAPI.Harmony.Patch(typeof(BinarySerializationService).GetMethod(nameof(BinarySerializationService.DeserializeAreasFromResources)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Area_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializationService).GetMethod(nameof(BinarySerializationService.DeserializeBargainsFromResources)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Bargain_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializationService).GetMethod(nameof(BinarySerializationService.DeserializeDomicilesFromResources)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Domicile_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializationService).GetMethod(nameof(BinarySerializationService.DeserializeEventsFromResources)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Event_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializationService).GetMethod(nameof(BinarySerializationService.DeserializeExchangesFromResources)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Exchange_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializationService).GetMethod(nameof(BinarySerializationService.DeserializePersonaeFromResources)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Persona_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializationService).GetMethod(nameof(BinarySerializationService.DeserializeProspectsFromResources)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Prospect_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializationService).GetMethod(nameof(BinarySerializationService.DeserializeQualitiesFromResources)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Quality_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializationService).GetMethod(nameof(BinarySerializationService.DeserializeSettingsFromResources)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Setting_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));

            SkylessAPI.Harmony.Patch(typeof(CharacterRepository).GetMethod(nameof(CharacterRepository.LoadCharacter)),
                prefix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(CleanupLineage), BindingFlags.Static | BindingFlags.NonPublic)));
        }

        #region Helper Methods
        private static void FindAddonsForPlugins()
        {
            if (!PluginManager.PluginsUpdated)
                foreach (var plugin in PluginManager.Plugins.Values)
                {
                    plugin.Addon = PluginManager.OldPlugins[plugin.Manifest.Guid].Addon;
                    plugin.Addon.Repos = FindReposInDirectory(plugin.Directory);
                }
            else
                foreach (var plugin in PluginManager.Plugins.Values)
                    plugin.Addon = GetAddonForPlugin(plugin);
        }

        private static Addon GetAddonForPlugin(PluginManager.Plugin plugin)
        {
            var repos = FindReposInDirectory(plugin.Directory);

            if (repos == null)
                SkylessAPI.Logging.LogWarning($"{plugin.Manifest.Name} has a duplicate repository file; its addon will not be loaded.");
            else if (repos.Count == 0)
                SkylessAPI.Logging.LogDebug($"No addon detected for {plugin.Manifest.Name}.");
            else
            {
                SkylessAPI.Logging.LogDebug($"Addon registered for {plugin.Manifest.Name}.");
                return new Addon(repos);
            }

            return null;
        }

        private static Dictionary<string, string> FindReposInDirectory(string directory)
        {
            var repos = new Dictionary<string, string>();

            foreach (var jsonPath in Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(jsonPath);

                if (RepoNames.Contains(fileName))
                {
                    if (repos.ContainsKey(fileName))
                        return null;
                    repos.Add(fileName, jsonPath);
                }
            }

            return repos;
        }

        private static bool AreAddonsUpdated()
        {
            if (!PluginManager.PluginsUpdated) return false;

            foreach (var plugin in PluginManager.Plugins.Values)
                if (plugin.Addon != null)
                    if (!PluginManager.OldPlugins.TryGetValue(plugin.Manifest.Guid, out var oldPlugin)
                        || plugin.Manifest.Version != oldPlugin.Manifest.Version)
                        return true;

            return false;
        }

        private static void CalculateOffsets()
        {
            int maxId = ModIdCutoff;
            foreach (var plugin in PluginManager.Plugins.Values)
            {
                var addon = plugin.Addon;

                addon.EstablishIdRange();
                addon.IdOffset = maxId - addon.BaseModIDRange.Min;
                maxId = addon.BaseModIDRange.Max + addon.IdOffset + 1;
            }
        }

        private static List<string> GetLoadOrder()
        {
            var plugins = ResolveHardDependencies(PluginManager.Plugins.Values.Where(p => p.Addon != null));

            var pluginGuids = plugins.Select(p => p.Manifest.Guid);
            var dependencyDict = plugins.ToDictionary(p => p.Manifest.Guid, p => p.AllDependencies());

            try
            {
                return pluginGuids.TopologicalSort(s => dependencyDict[s]);
            }
            catch (ArgumentException)
            {
                SkylessAPI.Logging.LogWarning("There is a cyclic dependency in the addon dependency tree, no addons will be loaded.");
            }

            return new List<string>();
        }

        private static IEnumerable<PluginManager.Plugin> ResolveHardDependencies(IEnumerable<PluginManager.Plugin> plugins)
        {
            var pluginDict = plugins.ToDictionary(p => p.Manifest.Guid);
            var prevCount = 0;

            while (pluginDict.Count > prevCount)
            {
                var pluginsToRemove = new HashSet<string>();

                foreach (var plugin in pluginDict.Values)
                {
                    foreach (var dep in plugin.Manifest.HardDependencies)
                    {
                        if (!pluginDict.ContainsKey(dep))
                        {
                            pluginsToRemove.Add(plugin.Manifest.Guid);
                            SkylessAPI.Logging.LogWarning($"{plugin.Manifest.Name} is missing hard dependency {dep} and will not be loaded.");
                            break;
                        }
                    }
                }

                foreach (var remove in pluginsToRemove)
                    pluginDict.Remove(remove);

                prevCount = pluginDict.Count;
            }

            return pluginDict.Select(p => p.Value);
        }
        #endregion

        #region Postfixes
        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Area> Area_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Area> __result) =>
            RepositoryMerger.MergeOrLoadRepos(__result.ToList().ToManagedList(), ModInterop.Mergers.AreaMerger.Instance, "areas").ToIl2CppList().ToIList();

        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Bargain> Bargain_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Bargain> __result) =>
            RepositoryMerger.MergeOrLoadRepos(__result.ToList().ToManagedList(), ModInterop.Mergers.BargainMerger.Instance, "bargains").ToIl2CppList().ToIList();

        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Domicile> Domicile_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Domicile> __result) =>
            RepositoryMerger.MergeOrLoadRepos(__result.ToList().ToManagedList(), ModInterop.Mergers.DomicileMerger.Instance, "domiciles").ToIl2CppList().ToIList();

        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Event> Event_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Event> __result) =>
            RepositoryMerger.MergeOrLoadRepos(__result.ToList().ToManagedList(), ModInterop.Mergers.EventMerger.Instance, "events").ToIl2CppList().ToIList();

        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Exchange> Exchange_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Exchange> __result) =>
            RepositoryMerger.MergeOrLoadRepos(__result.ToList().ToManagedList(), ModInterop.Mergers.ExchangeMerger.Instance, "exchanges").ToIl2CppList().ToIList();

        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Persona> Persona_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Persona> __result) =>
            RepositoryMerger.MergeOrLoadRepos(__result.ToList().ToManagedList(), ModInterop.Mergers.PersonaMerger.Instance, "personas").ToIl2CppList().ToIList();

        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Prospect> Prospect_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Prospect> __result) =>
            RepositoryMerger.MergeOrLoadRepos(__result.ToList().ToManagedList(), ModInterop.Mergers.ProspectMerger.Instance, "prospects").ToIl2CppList().ToIList();

        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Quality> Quality_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Quality> __result) =>
            RepositoryMerger.MergeOrLoadRepos(__result.ToList().ToManagedList(), ModInterop.Mergers.QualityMerger.Instance, "qualities").ToIl2CppList().ToIList();

        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Setting> Setting_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Setting> __result) =>
            RepositoryMerger.MergeOrLoadRepos(__result.ToList().ToManagedList(), ModInterop.Mergers.SettingMerger.Instance, "settings").ToIl2CppList().ToIList();
        #endregion

        // Move these to a class in ModInterop
        private static void CleanupLineage(CharacterRepository __instance, string lineageFolder)
        {
            return;
            string basePath = Path.Combine("characterrepository", lineageFolder);
            foreach (string save in new string[] { "autosave.json", "autosave_backup.json" })
            {
                string path = Path.Combine(basePath, save);
                SkylessCharacter character = null;
                if (__instance._fileHelper.FileOrDirectoryExists(path))
                    character = __instance._jsonSerializer.DeserializeFromStream<SkylessCharacter>(path);
                CleanupCharacter(character);
                __instance._jsonSerializer.SerializeToStream(path, character);
            }
        }

        private static void CleanupCharacter(SkylessCharacter character)
        {

        }

        internal class Addon
        {
            [JsonConstructor]
            public Addon(Dictionary<string, string> Repos, int IdOffset, (int, int) BaseModIdRange)
            {
                this.Repos = Repos;
                this.IdOffset = IdOffset;
                this.BaseModIDRange = BaseModIdRange;
            }

            public Addon(Dictionary<string, string> repos)
            {
                Repos = repos;
                IdOffset = 0;
                BaseModIDRange = (-1, -1);
            }

            public void EstablishIdRange()
            {
                int min = int.MaxValue, max = 0;

                foreach (var repoPath in Repos.Values)
                {
                    using (StreamReader sr = new StreamReader(repoPath))
                    {
                        while (sr.Peek() >= 0)
                        {
                            foreach (Match m in IDRegex.YieldMatches(sr.ReadLine()))
                            {
                                if (m.Success && int.TryParse(m.Groups[1].Value, out int id) && id >= ModIdCutoff)
                                {
                                    if (id < min) min = id;
                                    if (id > max) max = id;
                                }
                            }
                        }
                    }
                }

                if (max == 0) BaseModIDRange = (0, 0);
                else BaseModIDRange = (min, max);
            }

            public Dictionary<string, string> Repos { get; internal set; }
            public int IdOffset { get; internal set; }
            public (int Min, int Max) BaseModIDRange { get; internal set; }
            [JsonIgnore]
            public bool Loaded { get; internal set; }
        }
    }
}
