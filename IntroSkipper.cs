using HarmonyLib;
using Mod.Utils;
using UnityEngine;

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
        if (sceneName == "SceneMenu")
        {
            HandleMenuScene();
        }
    }

    private static void HandleMenuScene()
    {
        try
        {
            // Disable the entire Canvas object
            DisableGameObject("MenuGame/Canvas");
            ModCore.Log("Canvas disabled successfully");
        }
        catch (System.Exception e)
        {
            ModCore.LogError($"Error while handling menu scene: {e.Message}");
        }
    }

    private static void DisableGameObject(string path)
    {
        GameObject obj = GameObject.Find(path);
        if (obj != null)
        {
            obj.SetActive(false); // Disable the entire GameObject
        }
        else
        {
            ModCore.LogError($"GameObject not found: {path}");
        }
    }
}
