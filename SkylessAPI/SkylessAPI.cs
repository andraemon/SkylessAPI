using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using SkylessAPI.Utilities;
using System.IO;
using static SkylessAPI.AddonAPI;

namespace SkylessAPI
{
    [BepInPlugin("dev.exotico.skylessapi", "SkylessAPI", "1.0.0.0")]
    public class SkylessAPI : BasePlugin
    {
        internal static ManualLogSource Logging { get; set; }

        internal static Harmony Harmony { get; set; }

        public SkylessAPI()
        {
            Logging = Log;
            Harmony = new Harmony("dev.exotico.skylessapi.harmony");
        }

        public override void Load()
        {
            StartupHelper.CallInvokeOnStart();
            Logging.LogInfo("SkylessAPI loaded successfully");
        }
    }
}
