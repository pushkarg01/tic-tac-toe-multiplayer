using Newtonsoft.Json;
using UnityEngine;

namespace Network.API
{
    #region Base API Response Models
    public class APIResponse<T>
    {
        public string message { get; set; }
        public bool isSuccess { get; set; }
        public T data { get; set; }
    }
    #endregion

    public class RegisterUserResponse
    {
        [JsonProperty("playerId")]public string userId { get; set; }

        [JsonProperty("userName")]public string userName { get; set; }
    }

    public class CreateRoomResponse
    {
        [JsonProperty("roomId")] public string roomId { get; set; }
    }
}

