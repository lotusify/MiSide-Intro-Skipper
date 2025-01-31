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
		/*
        if (sceneName == "SceneAihasto")
        {
            HandleAihastoScene();
        }
        else if (sceneName == "SceneMenu")
        {
            HandleMenuScene();
        }
		*/
		if (sceneName == "SceneMenu")
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
            // Tìm root object MenuGame trước
            GameObject menuGame = GameObject.Find("MenuGame");
            if (menuGame == null)
            {
                ModCore.LogError("Không tìm thấy MenuGame");
                return;
            }

            // Tìm theo transform con để tránh conflict
            Transform canvasTransform = menuGame.transform.Find("Canvas");
            if (canvasTransform == null)
            {
                ModCore.LogError("Không tìm thấy Canvas");
                return;
            }

            // Ẩn các thành phần cụ thể
            canvasTransform.Find("NameGame")?.gameObject.SetActive(false);
            canvasTransform.Find("FrameMenu")?.gameObject.SetActive(false);
            
            ModCore.Log("Đã ẩn thành phần menu");
        }
        catch (Exception e)
        {
            ModCore.LogError($"Lỗi ẩn menu: {e.Message}");
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