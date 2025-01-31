using HarmonyLib;
using Mod.Utils;
using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
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
            ModCore.Loader.StartCoroutine(HideMenuWithRetry());
        }
    }

    private static IEnumerator HideMenuWithRetry()
    {
        int attempts = 0;
        bool success = false;
        
        while (attempts < 5 && !success)
        {
            yield return new WaitForSeconds(0.1f * attempts);
            
            try
            {
                GameObject menuGame = GameObject.Find("MenuGame");
                if (menuGame == null)
                {
                    ModCore.Log("Chưa tìm thấy MenuGame, thử lại...");
                    attempts++;
                    continue;
                }

                Transform canvasTransform = menuGame.transform.Find("Canvas");
                if (canvasTransform == null)
                {
                    ModCore.Log("Chưa tìm thấy Canvas, thử lại...");
                    attempts++;
                    continue;
                }

                Transform nameGame = canvasTransform.Find("NameGame");
                Transform frameMenu = canvasTransform.Find("FrameMenu");
                
                if (nameGame != null && frameMenu != null)
                {
                    nameGame.gameObject.SetActive(false);
                    frameMenu.gameObject.SetActive(false);
                    ModCore.Log("Đã ẩn UI thành công!");
                    success = true;
                }
            }
            catch (Exception e)
            {
                ModCore.LogError(e.ToString());
            }
        }

        if (!success)
        {
            ModCore.LogError("Không thể tìm thấy UI elements sau 5 lần thử");
        }
    }

    // Phần còn lại giữ nguyên như code gốc
    private static void HandleAihastoScene() { /* ... */ }

    [HarmonyPatch]
    private static class Patch
    {
        [HarmonyPatch(typeof(Menu), "Start")]
        private static void Postfix(Menu __instance)
        {
            // Thêm logic ẩn UI tại đây như dự phòng
            ModCore.Loader.StartCoroutine(BackupHideUI());
            /* ... */
        }
    }

    private static IEnumerator BackupHideUI()
    {
        yield return new WaitForEndOfFrame();
        
        Transform[] allTransforms = UnityEngine.Object.FindObjectsOfType<Transform>(true);
        foreach (Transform t in allTransforms)
        {
            if (t.name == "NameGame" || t.name == "FrameMenu")
            {
                t.gameObject.SetActive(false);
                ModCore.Log($"Đã ẩn bằng phương pháp dự phòng: {t.name}");
            }
        }
    }
}