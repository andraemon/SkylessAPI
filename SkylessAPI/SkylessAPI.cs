using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using SkylessAPI.Utilities;
using Skyless.Assets.Code.Skyless.Utilities.Serialization;
using UnhollowerRuntimeLib;
using UnityEngine;
using HarmonyLib;

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
            Logging.LogDebug("SkylessAPI loaded successfully");
            StartupHelper.CallInvokeOnStart();
            Logging.LogDebug(Paths.PluginPath);
        }
    }
}
