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
    private static GameObject _coroutineRunner; // Thêm GameObject để chạy coroutine

    public static void Init()
    {
        ModCore.Loader.SceneWasInitialized += OnSceneWasInitialized;
        _harmony = new("com.miside.introskipper");
        _harmony.PatchAll(typeof(Patch));
        
        // Tạo GameObject để chạy coroutine
        _coroutineRunner = new GameObject("CoroutineRunner");
        UnityEngine.Object.DontDestroyOnLoad(_coroutineRunner);
        
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
            // Sử dụng component để chạy coroutine
            _coroutineRunner.AddComponent<CoroutineRunner>().StartHideCoroutine();
        }
    }

    // Thêm class helper để chạy coroutine
    private class CoroutineRunner : MonoBehaviour
    {
        public void StartHideCoroutine()
        {
            StartCoroutine(HideMenuWithRetry());
        }

        private IEnumerator HideMenuWithRetry()
        {
            int attempts = 0;
            bool success = false;
            
            while (attempts < 5 && !success)
            {
                yield return new WaitForSeconds(0.1f * attempts);
                
                try
                {
                    GameObject menuGame = GameObject.Find("MenuGame");
                    if (menuGame == null) continue;

                    Transform canvasTransform = menuGame.transform.Find("Canvas");
                    if (canvasTransform == null) continue;

                    Transform nameGame = canvasTransform.Find("NameGame");
                    Transform frameMenu = canvasTransform.Find("FrameMenu");
                    
                    if (nameGame != null && frameMenu != null)
                    {
                        nameGame.gameObject.SetActive(false);
                        frameMenu.gameObject.SetActive(false);
                        ModCore.Log("UI elements hidden successfully!");
                        success = true;
                    }
                }
                catch (Exception e)
                {
                    ModCore.LogError(e.ToString());
                }
                finally
                {
                    attempts++;
                }
            }

            if (!success)
            {
                ModCore.LogError("Failed to hide UI elements after 5 attempts");
            }
            
            Destroy(this); // Hủy component sau khi hoàn thành
        }
    }

    // Phần còn lại giữ nguyên
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
            catch (Exception) { /* Ignored */ }

            ModCore.Log("Menu cutscene skipped");
        }
    }
}