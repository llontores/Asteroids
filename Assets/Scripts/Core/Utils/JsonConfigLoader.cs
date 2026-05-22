using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json; 

public static class JsonConfigLoader
{
    public static T LoadFromResources<T>(string path) where T : class
    {
        var asset = Resources.Load<TextAsset>(path);
        
        if (asset == null)
        {
            throw new FileNotFoundException($"Не удалось загрузить конфиг из Resources! Путь: {path}");
        }

        T config = JsonConvert.DeserializeObject<T>(asset.text);

        if (config == null)
        {
            throw new InvalidOperationException($"JSON конфиг по пути {path} пуст или имеет неверный формат!");
        }

        return config;
    }
}