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
        if (sceneName == "SceneAihasto")
        {
            HandleAihastoScene();
        }
        else if (sceneName == "SceneMenu")
        {
            HandleMenuScene();
        }
    }

    private static void HandleAihastoScene()
    {
        try
        {
            PlayableDirector? playableDirector = GameObject
                .Find("Scene")
                ?.GetComponent<PlayableDirector>();
            
            if (playableDirector != null)
            {
                playableDirector.time = playableDirector.duration;
                ModCore.Log("Aihasto intro skipped");
            }
        }
        catch (Exception e)
        {
            ModCore.LogError(e.Message);
        }
    }

    private static void HandleMenuScene()
    {
        try
        {
            // Tìm và tắt Renderer cho các object
            DisableRenderer("MenuGame/Canvas/NameGame");
            DisableRenderer("MenuGame/Canvas/FrameMenu");
            ModCore.Log("Đã tắt Renderer thành công");
        }
        catch (Exception e)
        {
            ModCore.LogError(e.Message);
        }
    }

    private static void DisableRenderer(string path)
    {
        GameObject obj = GameObject.Find(path);
        if (obj != null)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
            else
            {
                ModCore.LogError($"Không tìm thấy Renderer trên {path}");
            }
        }
        else
        {
            ModCore.LogError($"Không tìm thấy object: {path}");
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
                // Bỏ qua exception
            }

            ModCore.Log("The opening menu cutscene should be skipped");
        }
    }
}