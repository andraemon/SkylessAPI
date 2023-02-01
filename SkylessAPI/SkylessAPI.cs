using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Rewired;
using Skyless.Assets.Code.Skyless.Game.Config;
using SkylessAPI.Utilities;
using System;
using System.IO;
using UnityEngine;

namespace SkylessAPI
{
    [BepInPlugin("dev.exotico.skylessapi", "SkylessAPI", "1.0.0.0")]
    public class SkylessAPI : BasePlugin
    {
        public static string SkylessDataPath { get; private set; }

        internal static ManualLogSource Logging { get; set; }

        internal static Harmony Harmony { get; set; }

        internal static ConfigEntry<bool> AlwaysMergeRepos { get; set; }

        public SkylessAPI()
        {
            Logging = Log;
            Harmony = new Harmony("dev.exotico.skylessapi.harmony");
            AlwaysMergeRepos = Config.Bind("Debug", "AlwaysMergeRepos", false, "Whether or not to always merge mod repos instead of only when necessary.");
        }

        public override void Load()
        {
            SkylessDataPath = Path.Combine(Application.persistentDataPath, "SkylessAPI");
            StartupHelper.CallInvokeOnStart();
            AddComponent<TestBehaviour>();
            Logging.LogInfo("SkylessAPI loaded successfully");
        }
    }

    public class TestBehaviour : MonoBehaviour
    {
        public TestBehaviour(IntPtr handle) : base(handle) { }

        private void Update()
        {
            if (ReInput.controllers.Keyboard.GetButtonDown(Keyboard.GetElementIdentifierIdByKeyCode(KeyboardKeyCode.F7)))
            {
                ZenjectAPI.ResolveInstance<IConfiguration>().EditorMode = true;
            }
            else if (ReInput.controllers.Keyboard.GetButtonDown(Keyboard.GetElementIdentifierIdByKeyCode(KeyboardKeyCode.U)))
            {
                var list = FindObjectsOfType<MonoBehaviour>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].isActiveAndEnabled)
                        SkylessAPI.Logging.LogDebug(list[i].name);
                }
            }
            else if (ReInput.controllers.Keyboard.GetButtonDown(Keyboard.GetElementIdentifierIdByKeyCode(KeyboardKeyCode.L)))
            {
                var decks = EditorController.GetDecks().GetEnumerator();
                while (decks.MoveNext())
                {
                    SkylessAPI.Logging.LogDebug($"{decks.Current.Key}, {decks.Current.Value}");
                }
            }
        }
    }
}
