using PXLed.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PXLed
{
    public static class PluginFinder
    {
        /// <summary>
        /// Finds types in directory that implement <see cref="ILEDEffect"/> and have a <see cref="LEDEffectAttribute"/> and initializes them.
        /// </summary>
        /// <param name="directory">Directory to search. If null, use the current working directory.</param>
        /// <returns>Returns an initialized array of all found effects.</returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public static LEDEffectData[] FindEffects(string? directory = null)
        {
            if (directory == null)
                directory = Environment.CurrentDirectory;

            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException($"Directory {directory} does not exist");

            List<LEDEffectData> effects = new();

            // Iterate through each assembly in directory
            string[] assemblyFileNames = Directory.GetFiles(directory, "*.dll");
            foreach (string assemblyName in assemblyFileNames)
            {
                Assembly ass = Assembly.LoadFile(assemblyName);
                if (ass != null)
                {
                    // Iterate through each type in assembly
                    Type[] types = ass.GetTypes();
                    foreach (Type type in types)
                    {
                        // Check for type for attribute and interface
                        LEDEffectAttribute? effectAttribute = type.GetCustomAttribute<LEDEffectAttribute>();
                        if (effectAttribute != null)
                        {
                            if (!typeof(ILEDEffect).IsAssignableFrom(type))
                                throw new NotImplementedException($"{type.FullName} has LEDEffect attribute but does not inherit ILEDEffect");

                            // Initialize type and add it to list
                            ILEDEffect? effect = (ILEDEffect?)Activator.CreateInstance(type);                            
                            if (effect != null)
                            {
                                LEDEffectData effectData = new(effectAttribute.DisplayName, effectAttribute.MaxFPS, effect);
                                effects.Add(effectData);
                            }
                        }
                    }
                }
            }

            return effects.ToArray();
        }
    }
}
