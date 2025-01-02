namespace Mod.Utils;

public static class SceneTracker
{
    private static string _lastSceneName = string.Empty;
    public static string LastSceneName
    {
        get => _lastSceneName;
        private set => _lastSceneName = value;
    }
    private static string _currentSceneName = string.Empty;

    public static void Init()
    {
        ModCore.Loader.SceneWasLoaded += OnSceneWasLoaded;
    }

    private static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        _lastSceneName = _currentSceneName;
        _currentSceneName = sceneName;
    }
}
