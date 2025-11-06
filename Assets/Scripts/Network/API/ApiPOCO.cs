using Newtonsoft.Json;
using UnityEngine;

namespace Network.API
{
    #region Base API Response Models
    public class APIResponse<T>
    {
        public string message { get; set; }
        public bool success { get; set; }
        public T data { get; set; }
        public bool display { get; set; }
    }
    #endregion
    #region LoginResponse Data
    public class PlayerDataResponse
    {
        public string playerId { get; set; }
        public string username { get; set; }
        public int avatar { get; set; }
        [JsonProperty("authToken")] public string backendAuthToken { get; set; }
    }
    #endregion
}

