using HarmonyLib;
using Skyless.Assets.Code.Skyless.Game.Config;
using Skyless.Assets.Code.Skyless.Game.PlayerInput;
using Skyless.Assets.Code.Skyless.UI.Interfaces;
using Skyless.Assets.Scenes;
using SkylessAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkylessAPI
{
    /// <summary>
    /// Provides access to dependencies injected with Zenject.
    /// </summary>
    public static class ZenjectAPI
    {
        public static MasterInstaller Installer { get; private set; }

        [InvokeOnStart]
        private static void Load()
        {
            SkylessAPI.Harmony.Patch(typeof(MasterInstaller).GetMethod(nameof(MasterInstaller.InstallBindings)),
                postfix: new HarmonyMethod(typeof(ZenjectAPI).GetMethod(nameof(GetMasterInstaller), BindingFlags.Static | BindingFlags.NonPublic)));
        }

        private static void GetMasterInstaller(MasterInstaller __instance)
        {
            Installer = __instance;
        }

        /// <summary>
        /// Attempts to resolve an instance matching the given type.
        /// </summary>
        /// <typeparam name="T">The type to match against.</typeparam>
        /// <returns>The resolved instance, or null if none is found.</returns>
        public static T ResolveInstance<T>() where T : class
            => Installer.Container.TryResolve<T>();

        /// <summary>
        /// Attempts to resolve an instance matching the given type, passing it by reference.
        /// </summary>
        /// <typeparam name="T">The type to match against.</typeparam>
        /// <param name="instance">The resolved instance, or null if no match is found.</param>
        /// <returns>True if a match is found; otherwise, false.</returns>
        public static bool TryResolveInstance<T>(out T instance) where T : class
        {
            instance = Installer.Container.TryResolve<T>();
            return instance != null;
        }
    }
}
