using BepInEx;
using BinarySerializer;
using Failbetter.Core;
using Failbetter.Core.DataInterfaces;
using Failbetter.Data;
using HarmonyLib;
using Il2CppSystem.Linq;
using Newtonsoft.Json;
using Skyless.Assets.Code.Failbetter.Data;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using Event = Failbetter.Core.Event;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SkylessAPI
{
    public static class AddonAPI
    {
        private static readonly Dictionary<string, AddonInfo> Addons = new Dictionary<string, AddonInfo>();
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
        
        private static bool _loaded;
        public static bool Loaded
        {
            get => _loaded;
            internal set => _loaded = value;
        }

        private static void Load()
        {
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Area).GetMethod(nameof(BinarySerializer_Area.DeserializeCollection)),
                 postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Area_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic))); 
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Bargain).GetMethod(nameof(BinarySerializer_Bargain.DeserializeCollection)),
                 postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Bargain_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Domicile).GetMethod(nameof(BinarySerializer_Domicile.DeserializeCollection)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Domicile_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Event).GetMethod(nameof(BinarySerializer_Event.DeserializeCollection)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Event_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Exchange).GetMethod(nameof(BinarySerializer_Exchange.DeserializeCollection)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Exchange_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Persona).GetMethod(nameof(BinarySerializer_Persona.DeserializeCollection)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Persona_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Prospect).GetMethod(nameof(BinarySerializer_Prospect.DeserializeCollection)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Prospect_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Quality).GetMethod(nameof(BinarySerializer_Quality.DeserializeCollection)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Quality_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Setting).GetMethod(nameof(BinarySerializer_Setting.DeserializeCollection)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Setting_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Logging.LogInfo("Loaded AddonAPI");
            _loaded = true;
        }

        [InvokeOnStart]
        private static void RegisterAddons()
        {
            // Initialize addon list and get directories which contain an addon manifest
            List<AddonInfo> addons = new List<AddonInfo>();
            var directories = Directory.GetFiles(Paths.PluginPath, "manifest.json", SearchOption.AllDirectories)
                .Select(a => Path.GetDirectoryName(a)).Concat(Directory.GetFiles(Application.persistentDataPath, "manifest.json", SearchOption.AllDirectories)
                    .Select(a => Path.GetDirectoryName(a))).ToHashSet();

            // Try to construct addon info, only registering if manifest is valid
            foreach (string directory in directories)
            {
                var addon = new AddonInfo(directory);
                if (addon.Manifest != null && !addon.Manifest.Guid.IsNullOrWhiteSpace() && !addon.Manifest.Version.IsNullOrWhiteSpace())
                    addons.Add(addon);
            }

            // Resolve version conflicts if there are multiple versions of the same addon present, only keeping the newest
            addons = addons.GroupBy(
                addon => addon.Manifest.Guid,
                addon => addon,
                (guid, versions) => versions.OrderByDescending(v => v.Manifest.Version).First()
            ).ToList();

            // Only hook necessary methods if there are addons present
            if (addons.Count > 0)
            {
                foreach (AddonInfo addon in addons)
                    Addons[addon.Manifest.Guid] = addon;
                Load();
            }
        }

        #region Postfixes
        private static Il2CppSystem.Collections.Generic.List<Area> Area_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Area> __result) =>
            QuickMergeAddonRepos(__result, "areas.json");

        private static Il2CppSystem.Collections.Generic.List<Bargain> Bargain_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Bargain> __result) =>
            QuickMergeAddonRepos(__result, "bargains.json");

        private static Il2CppSystem.Collections.Generic.List<Domicile> Domicile_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Domicile> __result) =>
            QuickMergeAddonRepos(__result, "domiciles.json");

        private static Il2CppSystem.Collections.Generic.List<Event> Event_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Event> __result) =>
            QuickMergeAddonRepos(__result, "events.json");

        private static Il2CppSystem.Collections.Generic.List<Exchange> Exchange_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Exchange> __result) =>
            QuickMergeAddonRepos(__result, "exchanges.json");

        private static Il2CppSystem.Collections.Generic.List<Persona> Persona_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Persona> __result) =>
            QuickMergeAddonRepos(__result, "personas.json");

        private static Il2CppSystem.Collections.Generic.List<Prospect> Prospect_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Prospect> __result) =>
            QuickMergeAddonRepos(__result, "prospects.json");

        private static Il2CppSystem.Collections.Generic.List<Quality> Quality_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Quality> __result) =>
            QuickMergeAddonRepos(__result, "qualities.json");

        private static Il2CppSystem.Collections.Generic.List<Setting> Setting_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Setting> __result) =>
            QuickMergeAddonRepos(__result, "settings.json");
        #endregion

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
                    var modEvents = JsonConvert.DeserializeObject<Il2CppSystem.Collections.Generic.List<T>>(File.ReadAllText(Path.Combine(directory, slug)));
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
                var values = JsonConvert.DeserializeObject<Il2CppSystem.Collections.Generic.List<T>>(File.ReadAllText(path));
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

        internal class AddonInfo
        {
            public AddonInfo(string directory)
            {
                Directory = directory;
                Repos = new List<string>();
                
                string manifestPath = Path.Combine(directory, "manifest.json");
                try {
                    Manifest = JsonSerializer.Deserialize<AddonManifest>(File.ReadAllText(manifestPath));
                }
                catch (Exception e) {
                    Manifest = null;
                    SkylessAPI.Logging.LogError($"Could not deserialize {manifestPath}, addon will not be loaded");
                    SkylessAPI.Logging.LogDebug(e);
                }

                foreach (string file in System.IO.Directory.GetFiles(directory))
                {
                    var filename = Path.GetFileName(file);
                    if (RepoNames.Contains(filename))
                        Repos.Add(filename);
                }
            }

            public List<string> Repos;    
            public AddonManifest Manifest;
            public string Directory { get; internal set; }
        }

        internal class AddonManifest
        {
            public AddonManifest(string Guid, string Version, string[] UpdateKeys)
            {
                this.Guid = Guid;
                this.Version = Version;
                this.UpdateKeys = UpdateKeys;
            }
            
            public string Guid { get; internal set; }
            public string Version { get; internal set; }
            public string[] UpdateKeys { get; internal set; }
        }
    }
}
