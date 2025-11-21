#region

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

#endregion

namespace Network.API
{
    public class APIHandler : MonoBehaviour, IInitializable
    {
        private bool _initializing;
        private JsonSerializationOption _options;
        private Dictionary<string, object> _postDict = new Dictionary<string, object>();
        private ApiClient _client, _clientAuthenticated;

        [Inject] private GameEvents _gameEvents;

        public ApiClient Client => IsInitialized ? _client : null;
        public ApiClient ClientAuthenticated => IsInitialized ? _clientAuthenticated : null;

        #region Initialization and Token Setup
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            _ = Init();
        }

        //void Awake()
        //{
        //    string token = PlayerPrefs.GetString(UserPrefs.backendJWTToken, null);
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        OnLoggedIn(token);
        //    }
        //}

        private async Task Init()
        {
            Debug.Log("Init API Client -- Start");
            if (IsInitialized) return;

            while (_initializing)
            {
                Debug.Log($"Init API Client -- Initializing : {_initializing}");
                await Task.Delay(200);
                if (IsInitialized) return;
            }

            _initializing = true;

            _options = new JsonSerializationOption(null);
            _client = new ApiClient(_gameEvents, _options);

            IsInitialized = true;
            _initializing = false;

            Debug.Log("Init API Client -- Done");
        }

        public void OnLoggedIn(string token)
        {
            // Update serialization option with the token
            if (_options == null)
                _options = new JsonSerializationOption(token);
            else
                _options.Token = token;

            // Authenticated client now uses the same token automatically
            _clientAuthenticated = new ApiClient(_gameEvents, _options);

            Debug.Log("Authenticated client initialized with token.");
        }

        public void OnLoggedOut()
        {
            if (_options != null)
                _options.Token = null;

            _clientAuthenticated = null;
        }

        public string GetCurrentToken() => _options?.Token;

        //public void HandleTokenExpired()
        //{
        //    OnLoggedOut();
        //    PlayerPrefs.DeleteKey(UserPrefs.backendJWTToken);
        //    PlayerPrefs.SetInt(UserPrefs.IsProfileSet, 0);
        //    PlayerPrefs.Save();

        // //   UnityEngine.SceneManagement.SceneManager.LoadScene("Splash");
        //}
        #endregion


        public async void RegisterUser(string userName, Action<bool, RegisterUserResponse> onResponse)
        {
            if (!IsInitialized)
                await Init();

            _postDict.Clear();
            _postDict.Add("userName", userName);

            string postData = MySerializer.Serialize(_postDict);

            Debug.Log($"Register User Post Data: {postData}");

            string url = UniversalConstants.BaseUrl + UniversalConstants.RegisterUrl;
            // string url = ""; 

            var res = await Client.Post<APIResponse<RegisterUserResponse>>(url, postData);
            if (res != null && res.isSuccess)
            {
                string playerID = res.data.userId;  
                UniversalConstants.AuthID = playerID;
                PlayerPrefs.SetString(UserPrefs.AuthToken, playerID);
                PlayerPrefs.Save(); 
                onResponse?.Invoke(true, res.data);
            }
            else
            {
                Debug.LogError("User registration failed: " + (res?.message ?? "No response"));
                onResponse?.Invoke(false, null);
            }
        }

        public async void CheckUserExistence(string playerID, Action<bool, RegisterUserResponse> onResponse)
        {
            if (!IsInitialized)
                await Init();

            string url = UniversalConstants.BaseUrl + string.Format(UniversalConstants.CheckUserExistenceUrl, playerID);
            Debug.Log($"Checking user existence: {url}");
            var res = await Client.Get<APIResponse<RegisterUserResponse>>(url);
            if (res != null && res.isSuccess)
            {
                onResponse?.Invoke(true, res.data);
            }
            else
            {
                Debug.LogError("User existence check failed: " + (res?.message ?? "No response"));
                onResponse?.Invoke(false, null);
            }
        }

        public async void CreateRoom(string playerID,string character,Action<bool, CreateRoomResponse> onResponse)
        {
            if (!IsInitialized)
                await Init();

            _postDict.Clear();
            _postDict.Add("playerID", playerID);
            _postDict.Add("character", character);
            string postData = MySerializer.Serialize(_postDict);
            Debug.Log($"Create Room Post Data: {postData}");
            string url = UniversalConstants.BaseUrl + UniversalConstants.CreateRoomUrl;
            // string url = ""; 
            var res = await ClientAuthenticated.Post<APIResponse<CreateRoomResponse>>(url, postData);
            if (res != null && res.isSuccess)
            {
                Debug.Log("Room creation successful.");
                onResponse?.Invoke(true, res.data);
            }
            else
            {
                Debug.LogError("Room creation failed: " + (res?.message ?? "No response"));
                onResponse?.Invoke(false, null);
            }
        }


        //#region Create and Join Room

        //public async void CreateRoom(string roomName, string dealerType, int numbersDrawn, int ticketPrice, List<string> claims, Dictionary<string, int> claimsShare, Dictionary<string, float> claimDistribution, int maxTicketsPerPlayer, Action<bool, CreateRoomDataResponse> onResponse)
        //{
        //    if (!IsInitialized)
        //        await Init();

        //    if (ClientAuthenticated == null)
        //    {
        //        Debug.LogWarning("Authenticated client not initialized. Please login first.");
        //        onResponse?.Invoke(false, null);
        //        return;
        //    }

        //    _postDict.Clear();
        //    _postDict.Add("roomName", roomName);
        //    _postDict.Add("dealerType", dealerType);
        //    _postDict.Add("numbersDrawn", numbersDrawn);
        //    _postDict.Add("ticketPrice", ticketPrice);
        //    _postDict.Add("claims", claims);
        //    _postDict.Add("claimsShare", claimsShare);
        //    _postDict.Add("claimDistribution", claimDistribution);
        //    _postDict.Add("maxTicketsPerPlayer", maxTicketsPerPlayer);
        //    string postData = MySerializer.Serialize(_postDict);
        //    Debug.Log($"CreateRoom Post Data: {postData}");

        //    string url = UniversalConstants.BaseUrl + UniversalConstants.CreateRoomUrl;

        //    var res = await ClientAuthenticated.Post<APIResponse<CreateRoomDataResponse>>(url, postData);

        //    if (res != null && res.success)
        //    {
        //        Debug.Log("Room created successfully.");
        //        onResponse?.Invoke(true, res.data);
        //    }
        //    else
        //    {
        //        if (res?.message != null && res.message.ToLower().Contains("token"))
        //        {
        //            HandleTokenExpired();
        //            return;
        //        }
        //        Debug.LogWarning("User not found or error: " + (res?.message ?? "Unknown"));
        //        onResponse?.Invoke(false, null);
        //    }
        //}

        //public async void JoinRoom(string roomCode, Action<bool, CreateRoomDataResponse> onResponse)
        //{
        //    if (!IsInitialized)
        //        await Init();

        //    if (ClientAuthenticated == null)
        //    {
        //        Debug.LogWarning("Authenticated client not initialized. Please login first.");
        //        onResponse?.Invoke(false, null);
        //        return;
        //    }

        //    string url = UniversalConstants.BaseUrl + string.Format(UniversalConstants.JoinRoomUrl, roomCode);
        //    Debug.Log($"Joining room at: {url}");

        //    string postData = "{}";

        //    var res = await ClientAuthenticated.Post<APIResponse<CreateRoomDataResponse>>(url, postData);

        //    if (res != null && res.success)
        //    {
        //        onResponse?.Invoke(true, res.data);
        //    }
        //    else
        //    {
        //        if (res?.message != null && res.message.ToLower().Contains("token"))
        //        {
        //            HandleTokenExpired();
        //            return;
        //        }

        //        Debug.LogError($"Join room failed: {res?.message ?? "Unknown error"}");
        //        onResponse?.Invoke(false, null);
        //    }
        //}

        //#endregion


        //public async void StartGame(string roomCode, Action<bool, GameStartResponse> onResponse)
        //{
        //    if (!IsInitialized)
        //        await Init();
        //    if (ClientAuthenticated == null)
        //    {
        //        Debug.LogWarning("Authenticated client not initialized. Please login first.");
        //        onResponse?.Invoke(false, null);
        //        return;
        //    }

        //    string url = UniversalConstants.BaseUrl + string.Format(UniversalConstants.StartGameUrl, roomCode);
        //    Debug.Log($"Checking user existence: {url}");


        //    var res = await ClientAuthenticated.Get<APIResponse<GameStartResponse>>(url);
        //    if (res != null && res.success)
        //    {
        //        Debug.Log("Login successful. Backend token stored." + UniversalConstants.backendJWTToken);
        //        onResponse?.Invoke(true, res.data);
        //    }
        //    else
        //    {
        //        Debug.LogError("Login failed: " + (res?.message ?? "No response"));
        //        onResponse?.Invoke(false, null);
        //    }
        //}

    }
}
