#region

using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

#endregion

    public static class MySerializer
    {
        public static string Serialize<T>(T @object)
        {
            try
            {
                string result = JsonConvert.SerializeObject(@object);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not serialize {@object.GetType()}. {e.Message}");
                return string.Empty;
            }
        }

        public static T Deserialize<T>(string text)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(text);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not parse json {text}. {e.Message}");
                return default;
            }
        }
    }

