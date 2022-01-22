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
        private static readonly Dictionary<string, (AddonManifest Manifest, string Directory)> Addons 
            = new Dictionary<string, (AddonManifest Manifest, string Directory)>();
        
        private static bool _loaded;
        public static bool Loaded
        {
            get => _loaded;
            internal set => _loaded = value;
        }

        private static void Load()
        {
            //SkylessAPI.Harmony.Patch(typeof(RepositoryManager).GetMethod(nameof(RepositoryManager.InitialiseForResources)), 
            //    postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(InitializeAddonRepos), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Harmony.Patch(typeof(BinarySerializer_Event).GetMethod(nameof(BinarySerializer_Event.DeserializeCollection)),
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(Event_DeserializeCollectionPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
            SkylessAPI.Logging.LogInfo("Loaded AddonAPI");
            _loaded = true;
        }

        [InvokeOnStart]
        private static void CheckForAddons()
        {
            // Initialize addon list and get directories which contain an addon manifest
            List<(AddonManifest Manifest, string)> addons = new List<(AddonManifest, string)>();
            var directories = Directory.GetFiles(Paths.PluginPath, "manifest.json", SearchOption.AllDirectories)
                .Select(a => Path.GetDirectoryName(a)).Concat(Directory.GetFiles(Application.persistentDataPath, "manifest.json", SearchOption.AllDirectories)
                    .Select(a => Path.GetDirectoryName(a))).ToHashSet();

            // Try to deserialize addon manifests
            foreach (string directory in directories)
            {
                string path = Path.Combine(directory, "manifest.json");
                try {
                    addons.Add((JsonSerializer.Deserialize<AddonManifest>(File.ReadAllText(Path.Combine(directory, "manifest.json"))), directory));
                }
                catch (Exception e) {
                    SkylessAPI.Logging.LogError($"Could not deserialize {path}, addon will not be loaded");
                    SkylessAPI.Logging.LogDebug(e);
                }
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
                foreach ((AddonManifest Manifest, string) addon in addons)
                    Addons[addon.Manifest.Guid] = addon;
                Load();
            }
        }

        private static Il2CppSystem.Collections.Generic.List<Event> Event_DeserializeCollectionPostfix(Il2CppSystem.Collections.Generic.List<Event> __result)
        {
            var moddedList = new Il2CppSystem.Collections.Generic.List<Event>();
            var tempDict = new Dictionary<int, Event>();

            for (int i = 0; i < __result.Count; i++)
            {
                tempDict[__result[i].Id] = __result[i];
            }

            foreach (KeyValuePair<string, (AddonManifest Manifest, string Directory)> addonInfo in Addons)
            {
                string directory = addonInfo.Value.Directory;
                foreach (string file in Directory.GetFiles(addonInfo.Value.Directory))
                {
                    string filename = Path.GetFileName(file);
                    if (filename == "events.json")
                    {
                        var modEvents = JsonConvert.DeserializeObject<Il2CppSystem.Collections.Generic.List<Event>>(File.ReadAllText(Path.Combine(directory, "events.json")));
                        for (int i = 0; i < modEvents.Count; i++)
                            tempDict[modEvents[i].Id] = modEvents[i];
                    }
                }
            }

            foreach (KeyValuePair<int, Event> item in tempDict)
                moddedList.Add(item.Value);

            return moddedList;
        }

        //private static void InitializeAddonRepos(RepositoryManager __instance)
        //{
        //    foreach (KeyValuePair<string, (AddonManifest Manifest, string Directory)> addonInfo in Addons)
        //    {
        //        string directory = addonInfo.Value.Directory;
        //        foreach (string file in Directory.GetFiles(addonInfo.Value.Directory))
        //        {
        //            string filename = Path.GetFileName(file);
        //            if (filename == "areas.json")
        //                __instance._areaRepository.Cast<AreaRepository>().Entities.Merge(DeserializeJson<Area>(Path.Combine(directory, "areas.json")));
        //            else if (filename == "bargains.json")
        //                __instance._bargainRepository.Cast<BargainRepository>().Entities.Merge(DeserializeJson<Bargain>(Path.Combine(directory, "bargains.json")));
        //            else if (filename == "domiciles.json")
        //                __instance._domicileRepository.Cast<DomicileRepository>().Entities.Merge(DeserializeJson<Domicile>(Path.Combine(directory, "domiciles.json")));
        //            else if (filename == "events.json")
        //                __instance._eventRepository.Cast<EventRepository>().Entities.Merge(DeserializeJson<Event>(Path.Combine(directory, "events.json")));
        //            else if (filename == "exchanges.json")
        //                __instance._exchangeRepository.Cast<ExchangeRepository>().Entities.Merge(DeserializeJson<Exchange>(Path.Combine(directory, "exchanges.json")));
        //            else if (filename == "personas.json")
        //                __instance._personaRepository.Cast<PersonaRepository>().Entities.Merge(DeserializeJson<Persona>(Path.Combine(directory, "personas.json")));
        //            else if (filename == "prospects.json")
        //                __instance._prospectRepository.Cast<ProspectRepository>().Entities.Merge(DeserializeJson<Prospect>(Path.Combine(directory, "prospects.json")));
        //            else if (filename == "qualities.json")
        //                __instance._qualityRepository.Cast<QualityRepository>().Entities.Merge(DeserializeJson<Quality>(Path.Combine(directory, "qualities.json")));
        //            else if (filename == "settings.json")
        //                __instance._settingRepository.Cast<SettingRepository>().Entities.Merge(DeserializeJson<Setting>(Path.Combine(directory, "settings.json")));
        //        }
        //    }
        //}

        private static Il2CppSystem.Collections.Generic.Dictionary<int, T> DeserializeJson<T>(string path) where T : Entity
        {
            try {
                var values = JsonConvert.DeserializeObject<Il2CppSystem.Collections.Generic.IEnumerable<T>>(File.ReadAllText(path));
                Func<T, int> key = x => x.Id;
                Func<T, T> value = x => x;
                return Il2CppSystem.Linq.Enumerable.ToDictionary<T, int, T>(values, key, value);
            }
            catch (Exception e) {
                SkylessAPI.Logging.LogError($"Could not deserialize {path}");
                SkylessAPI.Logging.LogDebug(e);
                return null;
            }
        }

        public class AddonManifest
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
