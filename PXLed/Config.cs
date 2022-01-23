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
            
            // Load config when first constructed
            Load();
        }

        public string FileName { get; }

        Dictionary<string, object>? dataDictionary;

        public T GetData<T>()
        {
            // JSON property name is equal to actual class name
            // This makes it so that there can only be one of every class per config file
            string key = typeof(T).FullName!;

            // Return the property if it's in the config already, otherwise, return a new instance of the property
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
            // JSON property name is equal to actual class name
            // This makes it so that there can only be one of every class per config file
            string key = typeof(T).FullName!;

            // Overwrite property if already in config, otherwise, add it to config.
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
            // Read file if it exists
            // If not, create a new empty config dictionary
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
            // Write data to file
            string json = JsonSerializer.Serialize(dataDictionary, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
    }
}
