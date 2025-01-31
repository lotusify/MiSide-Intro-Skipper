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
        // Xử lý scene Aihasto
        if (sceneName == "SceneAihasto")
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
        // Thêm xử lý cho SceneMenu
        else if (sceneName == "SceneMenu")
        {
            try
            {
                // Ẩn các thành phần UI
                GameObject.Find("NameGame")?.SetActive(false);
                GameObject.Find("FrameMenu")?.SetActive(false);
                ModCore.Log("Đã ẩn menu components");
            }
            catch (Exception e)
            {
                ModCore.LogError($"Lỗi ẩn menu: {e.Message}");
            }
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
                // Bỏ qua exception như trước
            }

            ModCore.Log("The opening menu cutscene should be skipped");
        }
    }
}