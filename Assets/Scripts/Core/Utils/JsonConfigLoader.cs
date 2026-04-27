using UnityEngine;
using Newtonsoft.Json; 

public static class JsonConfigLoader
{
    public static T LoadFromResources<T>(string path) where T : class
    {
        var asset = Resources.Load<TextAsset>(path);
        
        if (asset == null)
        {
            Debug.LogError($"Config not found at path: {path}");
            return null;
        }

        T config = JsonConvert.DeserializeObject<T>(asset.text);
        
        return config;
    }
}