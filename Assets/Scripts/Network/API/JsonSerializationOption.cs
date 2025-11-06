#region

using System;
using Newtonsoft.Json;
using UnityEngine;

#endregion

namespace Network.API
{
    public class JsonSerializationOption : ISerializationOption
    {

        // public UserIdentifier UserIdentifier { get { return _userIdentifier; } }
        // private UserIdentifier _userIdentifier;

        //Move somewhere logical like in a user stats
        //public static UserIdentifier _UserIdentifier;

        public string ContentType
        {
            get => "application/json";
        }
        public string Token { get; set; }

        public string Version { get; }

        public string DeviceType { get; }

        public JsonSerializationOption(string token)
        {
            Token = token;
            Version = Application.version;
            DeviceType = Application.platform.ToString();

        }

        public JsonSerializationOption()
        {
            Token = "";
            Version = Application.version;
            DeviceType = Application.platform.ToString();
        }
        //
        // public JsonSerializationOption(UserIdentifier userIdentifier)
        // {
        //     _userIdentifier = userIdentifier;
        // }

        public T Deserialize<T>(string text)
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
}