using BepInEx;
using BinarySerializer;
using Failbetter.Core;
using Failbetter.Core.DataInterfaces;
using Failbetter.Data;
using HarmonyLib;
using Il2CppSystem.Linq;
using Skyless.Assets.Code.Failbetter.Data;
using Skyless.Assets.Code.Skyless.Game.Config;
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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using Event = Failbetter.Core.Event;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SkylessAPI
{
    public static class AddonAPI
    {
        internal static Dictionary<string, AddonInfo> Addons = new Dictionary<string, AddonInfo>();
        internal static Dictionary<string, AddonInfo> OldAddons = new Dictionary<string, AddonInfo>();
        internal static List<string> LoadOrder = new List<string>() { "dev.exotico.skylesstestaddon", "dev.exotico.skylesstestaddon2" };
        private static readonly string[] RepoNames = new string[] { 
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
        
        public const int ModIdCutoff = 700000;
        internal static bool ModListUpdated;
        internal static JsonSerializerOptions JsonOptions = new JsonSerializerOptions() { 
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            IncludeFields = true,
            WriteIndented = true
        };

        private static bool _loaded;
        public static bool Loaded
        {
            get => _loaded;
            private set => _loaded = value;
        }

        private static void Load()
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

            SkylessAPI.Logging.LogInfo("Loaded AddonAPI");
            Loaded = true;
        }

        [InvokeOnStart]
        private static void StartupAlways()
        {
            if (!Directory.Exists(SkylessAPI.SkylessDataPath))
                Directory.CreateDirectory(SkylessAPI.SkylessDataPath);

            string addonInfoPath = Path.Combine(SkylessAPI.SkylessDataPath, "addon_info.json");
            var addonDict = CurrentAddonDict();

            if (File.Exists(addonInfoPath))
            {
                try
                {
                    Addons = JsonSerializer.Deserialize<Dictionary<string, AddonInfo>>(File.ReadAllText(addonInfoPath), JsonOptions);
                }
                catch (Exception e)
                {
                    SkylessAPI.Logging.LogWarning("Failed to load old addon info, save cleanup will not be available if the mod list has changed.");
                    SkylessAPI.Logging.LogDebug(e);
                }
            }

            ModListUpdated = IsModListUpdated(addonDict, Addons) || SkylessAPI.AlwaysMergeRepos.Value;

            if (addonDict.Count > 0)
            {
                if (ModListUpdated)
                {
                    SkylessAPI.Logging.LogDebug("Mod list updated");
                    CalculateOffsets(addonDict);
                    OldAddons = Addons;
                    Addons = addonDict;
                }
                else
                {
                    foreach (var addon in addonDict)
                    {
                        Addons[addon.Key].Directory = addon.Value.Directory;
                    }
                }
                File.WriteAllText(addonInfoPath, JsonSerializer.Serialize(Addons, JsonOptions));

                Load();
            }
        }

        #region Helper Methods
        // this also updates directories on the old dict
        private static bool IsModListUpdated(Dictionary<string, AddonInfo> current, Dictionary<string, AddonInfo> old)
        {
            if (current.Count != old.Count) return true;

            int matches = 0;
            foreach (string guid in old.Keys)
            {
                if (current.TryGetValue(guid, out var currentInfo))
                {
                    old[guid].Directory = currentInfo.Directory;

                    if (old[guid].Manifest.Version == currentInfo.Manifest.Version)
                        matches++;
                }
            }

            if (matches == old.Count)
                return false;
            return true;
        }

        private static Dictionary<string, AddonInfo> CurrentAddonDict()
        {
            List<AddonInfo> addons = new List<AddonInfo>();
            Dictionary<string, AddonInfo> addonDict = new Dictionary<string, AddonInfo>();

            // Get directories which contain an addon manifest
            var directories = Directory.GetFiles(Paths.PluginPath, "manifest.json", SearchOption.AllDirectories)
                .Select(a => Path.GetDirectoryName(a)).Concat(Directory.GetFiles(Application.persistentDataPath, "manifest.json", SearchOption.AllDirectories)
                    .Select(a => Path.GetDirectoryName(a))).ToHashSet();

            // Try to construct addon info, only registering if manifest is valid
            foreach (string directory in directories)
            {
                var addon = new AddonInfo(directory);
                if (addon.ValidateManifest())
                    addons.Add(addon);
            }

            // Resolve version conflicts if there are multiple versions of the same addon present, only keeping the newest
            addons = addons.GroupBy(
                addon => addon.Manifest.Guid,
                addon => addon,
                (guid, versions) => versions.OrderByDescending(v => v.Manifest.Version).First()
            ).ToList();
            
            foreach (AddonInfo addon in addons)
                addonDict.Add(addon.Manifest.Guid, addon);

            return addonDict;
        }

        private static void CalculateOffsets(Dictionary<string, AddonInfo> addonDict)
        {
            int maxId = ModIdCutoff;
            foreach (AddonInfo info in addonDict.Values)
            {
                info.EstablishIdRange();
                info.IdOffset = maxId - info.BaseModIdRange.Min;
                maxId = info.BaseModIdRange.Max + info.IdOffset + 1;
            }
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
            __result; //QuickMergeAddonRepos(__result, "qualities.json");

        [HarmonyPriority(Priority.First)]
        private static Il2CppSystem.Collections.Generic.IList<Setting> Setting_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.IList<Setting> __result) =>
            __result; //QuickMergeAddonRepos(__result, "settings.json");
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

        private static Il2CppSystem.Collections.Generic.List<T> QuickMergeAddonRepos<T>(Il2CppSystem.Collections.Generic.List<T> repo, string slug) where T : Entity
        {
            var moddedList = new Il2CppSystem.Collections.Generic.List<T>();
            var tempDict = new Dictionary<int, T>();

            for (int i = 0; i < repo.Count; i++)
            {
                tempDict[repo[i].Id] = repo[i];
            }

            foreach (AddonInfo addonInfo in Addons.Values)
            {
                string directory = addonInfo.Directory;
                if (addonInfo.Repos.Contains(slug))
                {
                    var modEvents = JsonSerializer.Deserialize<Il2CppSystem.Collections.Generic.List<T>>(File.ReadAllText(Path.Combine(directory, slug)));
                    for (int i = 0; i < modEvents.Count; i++)
                        tempDict[modEvents[i].Id] = modEvents[i];
                }
            }

            foreach (KeyValuePair<int, T> item in tempDict)
                moddedList.Add(item.Value);

            return moddedList;
        }

        private static List<T> DeserializeRepo<T>(string path)
        {
            try {
                var values = JsonSerializer.Deserialize<Il2CppSystem.Collections.Generic.List<T>>(File.ReadAllText(path));
                var list = new List<T>();
                for (int i = 0; i < values.Count; i++)
                    list.Add(values[i]);
                return list;
            }
            catch (Exception e) {
                SkylessAPI.Logging.LogError($"Could not deserialize {path}");
                SkylessAPI.Logging.LogDebug(e);
                return null;
            }
        }

        /// <summary>
        /// Gets the actual ID of an added game object.
        /// </summary>
        /// <param name="guid">The GUID of the addon.</param>
        /// <param name="baseId">The original ID of the object.</param>
        /// <returns>The actual ID of an added game object.</returns>
        public static int ModId(string guid, int baseId) =>
            baseId + Addons[guid].IdOffset;

        /// <summary>
        /// Gets the name of a mod by its GUID.
        /// </summary>
        /// <param name="guid">The GUID of the addon.</param>
        /// <returns>The name of the addon, or null if no addon with the given GUID can be found.</returns>
        public static string GetName(string guid)
            => Addons[guid].Manifest.Name;

        /// <summary>
        /// Checks whether or not an addon is loaded.
        /// </summary>
        /// <param name="guid">The GUID of the addon.</param>
        /// <returns>True if the addon is loaded; false if it's not or if no addon with the given GUID can be found.</returns>
        public static bool IsLoaded(string guid)
        {
            if (Addons.TryGetValue(guid, out var info))
                return info.Loaded;
            return false;
        }

        internal static int GetOffset(string guid)
            => Addons[guid].IdOffset;

        internal class AddonInfo
        {
            [JsonConstructor]
            public AddonInfo(List<string> Repos, AddonManifest Manifest, string Directory, int IdOffset, (int, int) BaseModIdRange)
            {
                this.Repos = Repos;
                this.Manifest = Manifest;
                this.Directory = Directory;
                this.IdOffset = IdOffset;
                this.BaseModIdRange = BaseModIdRange;
            }

            public AddonInfo(string directory)
            {
                Repos = new List<string>();
                Directory = directory;
                IdOffset = 0;
                BaseModIdRange = (-1, -1);

                FindRepos();
                ReadManifest();
            }

            public void ReadManifest()
            {
                string manifestPath = Path.Combine(Directory, "manifest.json");
                try
                {
                    Manifest = JsonSerializer.Deserialize<AddonManifest>(File.ReadAllText(manifestPath), JsonOptions);
                }
                catch (Exception e)
                {
                    Manifest = null;
                    SkylessAPI.Logging.LogWarning($"Could not deserialize {manifestPath}, addon will not be loaded.");
                    SkylessAPI.Logging.LogDebug(e);
                }
            }

            public bool ValidateManifest() =>
                Manifest != null && Manifest.Validate();

            public void FindRepos()
            {
                foreach (string file in System.IO.Directory.GetFiles(Directory))
                {
                    var filename = Path.GetFileName(file);
                    if (RepoNames.Contains(filename))
                        Repos.Add(filename);
                }
            }

            public void EstablishIdRange()
            {
                Regex r = new Regex("\"Id\"\\s*:\\s*(?<Id>\\d+)");
                int min = int.MaxValue, max = 0;
                foreach (string slug in Repos)
                {
                    using (StreamReader sr = new StreamReader(Path.Combine(Directory, slug)))
                    {
                        string repo = sr.ReadToEnd();
                        foreach (Match m in r.YieldMatches(repo))
                        {
                            if (m.Success)
                            {
                                int id = int.Parse(m.Groups["Id"].Value);
                                if (id >= ModIdCutoff)
                                {
                                    if (id < min) min = id;
                                    if (id > max) max = id;
                                }
                            }
                        }
                    }
                }
                if (max == 0) BaseModIdRange = (0, 0);
                else BaseModIdRange = (min, max);
            }

            public List<string> Repos { get; internal set; }
            public AddonManifest Manifest { get; internal set; }
            public string Directory { get; internal set; }
            public int IdOffset { get; internal set; }
            public (int Min, int Max) BaseModIdRange { get; internal set; }
            [JsonIgnore]
            public bool Loaded { get; internal set; }
        }

        internal class AddonManifest
        {
            public AddonManifest(string Guid, string Name, string Version, string[] Dependencies, string[] UpdateKeys)
            {
                this.Guid = Guid;
                this.Name = Name;
                this.Version = Version;
                this.Dependencies = Dependencies;
                this.UpdateKeys = UpdateKeys;
            }

            public bool Validate()
            {
                if (Guid.IsNullOrWhiteSpace() || Version == null) return false;
                if (Dependencies == null) Dependencies = new string[0];
                if (UpdateKeys == null) UpdateKeys = new string[0];
                return true;
            }
            
            public string Guid { get; internal set; }
            public string Name { get; internal set; }
            public string Version { get; internal set; }
            public string[] Dependencies { get; internal set; }
            public string[] UpdateKeys { get; internal set; }
        }
    }
}
