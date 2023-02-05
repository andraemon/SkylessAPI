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
using System.Text.Json;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SkylessAPI
{
    [BepInPlugin("dev.exotico.skylessapi", "SkylessAPI", "1.0.0.0")]
    public class SkylessAPI : BasePlugin
    {
        public static string SkylessDataPath { get; private set; }

        internal static string DataPath { get; private set; }

        internal static ManualLogSource Logging { get; private set; }

        internal static Harmony Harmony { get; private set; }

        internal static ConfigEntry<bool> AlwaysMergeRepos { get; private set; }

        internal static JsonSerializerOptions JsonOptions { get; private set; }

        public SkylessAPI()
        {
            Logging = Log;
            Harmony = new Harmony("dev.exotico.skylessapi.harmony");
            AlwaysMergeRepos = Config.Bind("Debug", "AlwaysMergeRepos", false, "Whether or not to always merge mod repos instead of only when necessary.");
        }

        public override void Load()
        {
            SkylessDataPath = Application.persistentDataPath;
            DataPath = Path.Combine(SkylessDataPath, "SkylessAPI");
            JsonOptions = new JsonSerializerOptions()
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                IncludeFields = true,
                WriteIndented = true
            };

            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

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
