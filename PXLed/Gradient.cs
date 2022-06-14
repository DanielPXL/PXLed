using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace PXLed
{
    // Inspired by https://github.com/SebLague/Gradient-Editor/blob/master/Gradient%20Editor%2003/Assets/CustomGradient.cs
    public class Gradient
    {
        public Gradient()
        {
            keys = new List<ColorKey>();
            AddKey(Color24.FromRGB(0, 0, 0), 0f);
            AddKey(Color24.FromRGB(255, 255, 255), 1f);
        }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        List<ColorKey> keys;

        public Color24 Evaluate(float time)
        {
            time = Math.Clamp(time, 0f, 1f);

            ColorKey leftKey = keys[0];
            ColorKey rightKey = keys[keys.Count - 1];

            // Find left and right key
            for (int i = 0; i < keys.Count; i++)
            {
                if (time > keys[i].time)
                {
                    leftKey = keys[i];
                }
                if (time < keys[i].time)
                {
                    rightKey = keys[i];
                    break;
                }
            }

            // Protect from division by zero
            if (leftKey.time == rightKey.time)
                return leftKey.color;
            
            float blendTime = (time - leftKey.time) / (rightKey.time - leftKey.time);
            return Color24.LerpHSV(leftKey.color, rightKey.color, blendTime);
        }

        public int AddKey(Color24 color, float time)
        {
            return AddKey(new ColorKey(color, time));
        }

        public int AddKey(ColorKey key)
        {
            if (key.time < 0f || key.time > 1f)
                throw new Exception($"{nameof(key.time)} has to be between 0 and 1");

            for (int i = 0; i < keys.Count; i++)
            {
                if (key.time < keys[i].time)
                {
                    keys.Insert(i, key);
                    return i;
                }
            }

            keys.Add(key);
            return keys.Count - 1;
        }

        public void RemoveKey(int index)
        {
            if (keys.Count >= 2)
            {
                keys.RemoveAt(index);
            }
        }

        public int UpdateKeyTime(int index, float time)
        {
            Color24 col = keys[index].color;
            RemoveKey(index);
            return AddKey(col, time);
        }

        public void UpdateKeyColor(int index, Color24 col)
        {
            keys[index] = new ColorKey(col, keys[index].time);
        }

        public List<ColorKey> GetKeys()
        {
            return keys;
        }
    }

    [Serializable]
    public struct ColorKey
    {
        public ColorKey(Color24 color, float time)
        {
            this.color = color;
            this.time = time;
        }

        public readonly Color24 color;
        public readonly float time;
    }
}
