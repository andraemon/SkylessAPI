using BepInEx;
using Failbetter.Core;
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
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Event = Failbetter.Core.Event;

namespace SkylessAPI
{
    public static class AddonAPI
    {
        private static HashSet<SkylessAddon> Addons;
        private static bool _loaded;

        public static bool Loaded
        {
            get => _loaded;
            internal set => _loaded = value;
        }

        [InvokeOnStart]
        private static void Load()
        {
            SkylessAPI.Logging.LogDebug("Loaded AddonAPI");
            SkylessAPI.Harmony.Patch(typeof(RepositoryManager).GetMethod(nameof(RepositoryManager.InitialiseForResources)), 
                postfix: new HarmonyMethod(typeof(AddonAPI).GetMethod(nameof(InitializeAddonRepos), BindingFlags.Static | BindingFlags.NonPublic)));
        }

        private static void InitializeAddonRepos(RepositoryManager __instance)
        {

        }

        private static void DeserializeAddons()
        {
            var directories = Directory.GetFiles(Paths.PluginPath, "metadata.json", SearchOption.AllDirectories)
                .Select(a => Path.GetDirectoryName(a)).Concat(Directory.GetFiles(Application.persistentDataPath, "metadata.json", SearchOption.AllDirectories)
                    .Select(a => Path.GetDirectoryName(a))).ToList();

            foreach (string directory in directories) Addons.Add(new SkylessAddon(directory));

            Addons = Addons.GroupBy(
                addon => addon.Manifest.Guid,
                addon => addon,
                (guid, versions) => versions.OrderByDescending(v => v.Manifest.Version).First()
            ).ToHashSet();
        }

        private static void DeserializeJson<T>(string directory, string file, ref Il2CppSystem.Collections.Generic.Dictionary<int, T> entities) where T : Entity
        {
            var values = JsonConvert.DeserializeObject<Il2CppSystem.Collections.Generic.IEnumerable<T>>(Path.Combine(directory, file));
            Func<T, int> key = x => x.Id;
            Func<T, T> value = x => x;
            entities = Il2CppSystem.Linq.Enumerable.ToDictionary<T, int, T>(values, key, value);
        }

        public class SkylessAddon
        {
            // todo: separate out deserialization into a method which merges it with main repositories on the fly
            // storing separate collections for each plugin is pointless, idk why I did this, this class is useless
            // only AddonManifest is required for what I want to do
            public SkylessAddon(string directory)
            {
                foreach (string file in Directory.GetFiles(directory))
                {
                    if (file.FastContains("areas.json") && Areas == null)
                        DeserializeJson(directory, "areas.json", ref Areas);
                    else if (file.FastContains("bargains.json") && Bargains == null)
                        DeserializeJson(directory, "bargains.json", ref Bargains);
                    else if (file.FastContains("domiciles.json") && Domiciles == null)
                        DeserializeJson(directory, "domiciles.json", ref Domiciles);
                    else if (file.FastContains("events.json") && Events == null)
                        DeserializeJson(directory, "events.json", ref Events);
                    else if (file.FastContains("exchanges.json") && Exchanges == null)
                        DeserializeJson(directory, "exchanges.json", ref Exchanges);
                    else if (file.FastContains("personas.json") && Personas == null)
                        DeserializeJson(directory, "personas.json", ref Personas);
                    else if (file.FastContains("prospects.json") && Prospects == null)
                        DeserializeJson(directory, "prospects.json", ref Prospects);
                    else if (file.FastContains("qualities.json") && Qualities == null)
                        DeserializeJson(directory, "qualities.json", ref Qualities);
                    else if (file.FastContains("settings.json") && Settings == null)
                        DeserializeJson(directory, "settings.json", ref Settings);
                }

                Manifest = JsonConvert.DeserializeObject<AddonManifest>(Path.Combine(directory, "metadata.json"));
            }

            public Il2CppSystem.Collections.Generic.Dictionary<int, Area> Areas;
            public Il2CppSystem.Collections.Generic.Dictionary<int, Bargain> Bargains;
            public Il2CppSystem.Collections.Generic.Dictionary<int, Domicile> Domiciles;
            public Il2CppSystem.Collections.Generic.Dictionary<int, Event> Events;
            public Il2CppSystem.Collections.Generic.Dictionary<int, Exchange> Exchanges;
            public Il2CppSystem.Collections.Generic.Dictionary<int, Persona> Personas;
            public Il2CppSystem.Collections.Generic.Dictionary<int, Prospect> Prospects;
            public Il2CppSystem.Collections.Generic.Dictionary<int, Quality> Qualities;
            public Il2CppSystem.Collections.Generic.Dictionary<int, Setting> Settings;
            public AddonManifest Manifest;
        }

        public class AddonManifest
        {
            public AddonManifest(string guid, string version, string[] updateKeys)
            {
                Guid = guid;
                Version = version;
                UpdateKeys = updateKeys;
            }

            public readonly string Guid;
            public readonly string Version;
            internal string[] UpdateKeys;
        }
    }
}
