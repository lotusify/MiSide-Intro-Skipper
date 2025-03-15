using HarmonyLib;
using Mod.Utils;
using UnityEngine;
using UnityEngine.Playables;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace Mod
{
    public static class IntroSkipper
    {
        private static HarmonyLib.Harmony _harmony = null!;

        public static void Init()
        {
            ModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;

            _harmony = new("com.miside.introskipper");
            // Remove the PatchAll call since the Patch class is removed
            // _harmony.PatchAll(typeof(Patch));

            ModCore.Log("Initialized");
        }

        private static void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName != "SceneAihasto")
            {
                return;
            }

            try
            {
                PlayableDirector? playableDirector = GameObject
                    .Find("Scene")
                    ?.GetComponent<PlayableDirector>();
                if (playableDirector == null)
                {
                    return;
                }

                playableDirector.time = playableDirector.duration;

                ModCore.Log("Aihasto intro skipped");
            }
            catch (Exception e)
            {
                ModCore.LogError(e.Message);
            }
        }

        // Remove the Patch class and its contents
    }
}