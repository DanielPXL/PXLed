using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace PXLed
{
    public class Config
    {
        public Config(string fileName)
        {
            FileName = fileName;
            Load();
        }

        public string FileName { get; }

        Dictionary<string, object>? dataDictionary;

        public T GetData<T>()
        {
            string key = typeof(T).FullName!;

            if (dataDictionary!.ContainsKey(key))
            {
                JsonElement jsonElement = (JsonElement)dataDictionary[key];
                return jsonElement.Deserialize<T>()!;
            } else
            {
                return Activator.CreateInstance<T>();
            }
        }

        public void SetData<T>(T data)
        {
            string key = typeof(T).FullName!;

            if (dataDictionary!.ContainsKey(key))
            {
                dataDictionary[key] = data!;
            } else
            {
                dataDictionary.Add(key, data!);
            }
        }

        public void Load()
        {
            if (File.Exists(FileName))
            {
                string json = File.ReadAllText(FileName);
                dataDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            } else
            {
                dataDictionary = new Dictionary<string, object>();
            }
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(dataDictionary, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
    }
}
