using Godot;
using System;

namespace Game.Utils
{
    public static class CollectionUtils
    {
        public static T Get<T>(this Godot.Collections.Dictionary dict, Variant key)
        {
            if (dict.ContainsKey(key))
            {
                return (T)dict[key].Obj;
            }
            else
            {
                return default(T);
            }
        }
        public static int GetInt(this Godot.Collections.Dictionary dict, Variant key)
        {
            if (dict.ContainsKey(key))
            {
                return Convert.ToInt32(dict[key]);
            }
            else
            {
                return 0;
            }
        }
        public static float GetFloat(this Godot.Collections.Dictionary dict, Variant key)
        {
            if (dict.ContainsKey(key))
            {
                return Convert.ToSingle(dict[key]);
            }
            else
            {
                return 0.0f;
            }
        }
        public static int GetInt(this Godot.Collections.Dictionary<string, Variant> dict, string key)
        {
            if (dict.ContainsKey(key))
            {
                return Convert.ToInt32(dict[key]);
            }
            else
            {
                return 0;
            }
        }
        public static float GetFloat(this Godot.Collections.Dictionary<string, Variant> dict, string key)
        {
            if (dict.ContainsKey(key))
            {
                return Convert.ToSingle(dict[key]);
            }
            else
            {
                return 0.0f;
            }
        }
        public static T Get<T>(this Godot.Collections.Dictionary<string, Variant> dict, string key)
        {
            return ((Godot.Collections.Dictionary)dict).Get<T>(key);
        }

        public static T GetRandomElement<T>(this Godot.Collections.Array array)
        {
            if (array.Count == 0) return default(T);
            return (T)(array[(int)Mathf.Floor(GD.Randf() * array.Count)].Obj);
        }
        public static Godot.Collections.Dictionary GetRandomElement(this Godot.Collections.Array<Godot.Collections.Dictionary> array)
        {
            return ((Godot.Collections.Array)array).GetRandomElement<Godot.Collections.Dictionary>();
        }
    }
}