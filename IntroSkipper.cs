using HarmonyLib;
using Mod.Utils;
using UnityEngine;
using UnityEngine.Playables;
#if ML
using Il2Cpp;
#elif BIE
using BepInEx.IL2CPP;
#endif

namespace Mod;

public static class IntroSkipper
{
    private static HarmonyLib.Harmony _harmony = null!;

    public static void Init()
    {
        ModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;

        _harmony = new("com.miside.introskipper");
        _harmony.PatchAll(typeof(Patch));

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

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPatch(typeof(Menu), "Start")]
        private static void Postfix(Menu __instance)
        {
            if (SceneTracker.LastSceneName == "Scene 16 - TheEnd")
            {
                return;
            }

            try
            {
                __instance.eventSkip.Invoke();
                __instance.SkipStart();
            }
            catch (Exception)
            {
                /*
                    __instance.SkipStart() throws an exception
                    but it works anyway and we ignore this exception
                */
            }

            ModCore.Log("The opening menu cutscene should be skipped");
        }
    }
}
