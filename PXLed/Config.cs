using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            string key = typeof(T).AssemblyQualifiedName!;

            // Return the property if it's in the config already, otherwise, return a new instance of the property
            if (dataDictionary!.ContainsKey(key))
            {
                return (T)dataDictionary[key];
            } else
            {
                return Activator.CreateInstance<T>();
            }
        }

        public void SetData<T>(T data)
        {
            // JSON property name is equal to actual class name
            // This makes it so that there can only be one of every class per config file
            string key = typeof(T).AssemblyQualifiedName!;

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
            dataDictionary = new Dictionary<string, object>();

            // Create new empty data dictionary
            // Populate it with data from config file if it exists
            if (File.Exists(FileName))
            {
                string json = File.ReadAllText(FileName);
                Dictionary<string, object>? jsonContent = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                foreach (KeyValuePair<string, object> kvp in jsonContent!)
                {
                    Type? type = Type.GetType(kvp.Key);
                    if (type != null)
                    {
                        JObject jsonObject = (JObject)kvp.Value;
                        object? savedValue = jsonObject.ToObject(type);
                        if (savedValue != null)
                        {
                            dataDictionary![kvp.Key] = savedValue;
                        }
                    }
                }
            }
        }

        public void Save()
        {
            // Write data to file
            string json = JsonConvert.SerializeObject(dataDictionary, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }
    }
}
