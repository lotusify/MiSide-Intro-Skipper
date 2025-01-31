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
            var playableDirector = GameObject
                .Find("Scene")
                ?.GetComponent<PlayableDirector>();
            
            playableDirector?.PlayableGraph.GetRootPlayable(0).SetDuration(0);
            ModCore.Log("Aihasto intro skipped");
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
            // Tìm và ẩn Canvas chính
            GameObject.Find("MenuGame/Canvas")?.SetActive(false);
            ModCore.Log("Đã ẩn thành công Canvas menu");
            
            // Phương pháp dự phòng sau 2 giây
            ModCore.Loader.DelayedAction(2f, () => {
                var backupCanvas = GameObject.Find("MenuGame/Canvas");
                if(backupCanvas != null && backupCanvas.activeSelf) 
                {
                    backupCanvas.SetActive(false);
                    ModCore.Log("Đã ẩn Canvas bằng phương pháp dự phòng");
                }
            });
        }
        catch (Exception e)
        {
            ModCore.LogError($"Lỗi ẩn menu: {e.Message}");
        }
    }

/*
    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPatch(typeof(Menu), "Start")]
        private static void Postfix(Menu __instance)
        {
            if (SceneTracker.LastSceneName == "Scene 16 - TheEnd") return;

            try
            {
                __instance.eventSkip.Invoke();
                __instance.SkipStart();
            }
            catch { }

            ModCore.Log("Menu cutscene skipped");
        }
    }
	*/
}