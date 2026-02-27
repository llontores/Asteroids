using UnityEngine;

public static class JsonConfigLoader
{
    public static T LoadFromResources<T>(string path) where T : class
    {
        var asset = Resources.Load<TextAsset>(path);
        if (asset == null)
            return null;

        return JsonUtility.FromJson<T>(asset.text);
    }
}