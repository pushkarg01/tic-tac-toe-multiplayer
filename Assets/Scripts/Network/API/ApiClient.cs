#region

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

#endregion

namespace Network.API
{
    public class ApiClient
    {
        public delegate void OnCompletionAction();
        private const int Timeout = 30;

        //public event OnCompletionAction OnCompletion;

        private readonly ISerializationOption _serializationOption;
        private readonly GameEvents _gameEvents;

        public ApiClient(GameEvents gameEvents)
        {
            _gameEvents = gameEvents;
            _serializationOption = new JsonSerializationOption();
        }
        public ApiClient(GameEvents gameEvents, ISerializationOption serializationOption)
        {
            _gameEvents = gameEvents;
            _serializationOption = serializationOption;
        }

        public async Task<TResultType> Get<TResultType>(string url, int timeOut = 30)
        {
            try
            {
                Debug.Log($"CALLING: {url}"); //, ErrorTypes.API_CALL);

                //Debug.Log($"token: {serializationOption.UserIdentifier.token} ");
                using var webRequest = UnityWebRequest.Get(url);
                //webRequest.SetRequestHeader("Content-Type", _serializationOption.ContentType);
                //webRequest.SetRequestHeader("vNo", _serializationOption.Version);
                if (!string.IsNullOrEmpty(_serializationOption.Token))
                {
                    webRequest.SetRequestHeader("Authorization", $"Bearer {_serializationOption.Token}");
                    Debug.Log($"Bearer token: {_serializationOption.Token} ");
                }

                //webRequest.SetRequestHeader("Device-Type", _serializationOption.DeviceType);
                //  webRequest.SetRequestHeader("UUID", string.IsNullOrEmpty(_serializationOption.UUi) ? SystemInfo.deviceUniqueIdentifier : serializationOption.UserIdentifier.UUID);

                if (timeOut > -1)
                    webRequest.timeout = timeOut;

                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed: {url} {webRequest.error}{webRequest.downloadHandler.text} "); //, ErrorTypes.API_CALL);
                    _gameEvents.ErrorReceived((int)webRequest.responseCode, webRequest.downloadHandler.text);

                    //OnCompletion?.Invoke();
                    return default;
                }
                Debug.Log($"Success: {url}  {webRequest.downloadHandler.text}"); //, ErrorTypes.API_CALL);
                var result = _serializationOption.Deserialize<TResultType>(webRequest.downloadHandler.text);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(Get)} Failed: {e.Message} | {e.StackTrace}"); //, ErrorTypes.API_CALL);
                return default;
            }
        }

        public async Task<TResultType> Patch<TResultType>(string url, string postData, int timeOut = -100)
        {
            try
            {
                Debug.Log($"CALLING: {url} {MySerializer.Serialize(postData)}"); //, ErrorTypes.API_CALL);

                //Debug.Log(MySerializer.Serialize(postData));
                using var webRequest = new UnityWebRequest(url, "PATCH"); //Post(url, postData);
                byte[] jsonToSend = new UTF8Encoding().GetBytes(postData);

                //webRequest.method = "POST";
                webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", _serializationOption.ContentType);

                // webRequest.SetRequestHeader("vNo", serializationOption.UserIdentifier.vNo);
                // webRequest.SetRequestHeader("token", serializationOption.UserIdentifier.token);
                // webRequest.SetRequestHeader("Device-Type", serializationOption.UserIdentifier.DeviceType);
                // webRequest.SetRequestHeader("UUID", string.IsNullOrEmpty(serializationOption.UserIdentifier.UUID) ? SystemInfo.deviceUniqueIdentifier : serializationOption.UserIdentifier.UUID);

                if (timeOut == -100)
                    webRequest.timeout = Timeout;
                else
                {
                    if (timeOut <= 0)
                    {
                        return default;
                    }
                    webRequest.timeout = timeOut > Timeout ? Timeout : timeOut;
                }

                //Debug.Log(url + "  :  " + webRequest.timeout);
                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed: {webRequest.error} , {webRequest.result} "); //, ErrorTypes.API_CALL);
                    _gameEvents.ErrorReceived((int)webRequest.responseCode, webRequest.downloadHandler.text);

                    //OnCompletion?.Invoke();
                    return default;
                }
                Debug.Log($"{url}: {webRequest.downloadHandler.text}"); //, ErrorTypes.API_CALL);
                var result = _serializationOption.Deserialize<TResultType>(webRequest.downloadHandler.text);

                //Debug.Log(MySerializer.Serialize(result));
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(Patch)} Failed: {url} | {e.Message} | {e.StackTrace} "); //, ErrorTypes.API_CALL);
                return default;
            }
        }

        public async Task<TResultType> Post<TResultType>(string url, string postData)
        {
            try
            {
                Debug.Log($"CALLING: {url} | {postData}"); //, ErrorTypes.API_CALL);

                //Debug.Log(MySerializer.Serialize(postData));
                using var webRequest = new UnityWebRequest(url, "POST"); //Post(url, postData);
                byte[] jsonToSend = new UTF8Encoding().GetBytes(postData);

                //webRequest.method = "POST";
                webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", _serializationOption.ContentType);
                // webRequest.SetRequestHeader("Cookie", ApiEndPoints.cookie);
                //webRequest.SetRequestHeader("vNo", "4");
                //webRequest.SetRequestHeader("token", PlayerPrefs.GetString(UserPrefs.AuthToken));
                if (!string.IsNullOrEmpty(_serializationOption.Token))
                {
                    webRequest.SetRequestHeader("Authorization", $"Bearer {_serializationOption.Token}");
                    Debug.Log($"token: {_serializationOption.Token} ");
                }
                //webRequest.SetRequestHeader("Device-Type", "IOS");
                webRequest.timeout = Timeout;


                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed: {webRequest.error} , {webRequest.result} "); //, ErrorTypes.API_CALL);
                    _gameEvents.ErrorReceived((int)webRequest.responseCode, webRequest.downloadHandler.text);
                    return default;
                }
                Debug.Log($"Success: {url} | {webRequest.downloadHandler.text}"); //, ErrorTypes.API_CALL);
                var result = _serializationOption.Deserialize<TResultType>(webRequest.downloadHandler.text);
                Debug.Log(MySerializer.Serialize(result));
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(Post)} Failed: {url} | {e.Message} | {e.StackTrace} "); //, ErrorTypes.API_CALL);
                return default;
            }
        }

        public async Task<TResultType> Post<TResultType>(string url, object postData)
        {
            try
            {
                Debug.Log($"CALLING: {url} | {MySerializer.Serialize(postData)}"); //, ErrorTypes.API_CALL);

                //Debug.Log(MySerializer.Serialize(postData));
                using var webRequest = new UnityWebRequest(url, "POST"); //Post(url, postData);
                byte[] jsonToSend = Encoding.UTF8.GetBytes(MySerializer.Serialize(postData));
                webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                // webRequest.SetRequestHeader("Content-Type", serializationOption.ContentType);
                // webRequest.SetRequestHeader("vNo", serializationOption.UserIdentifier.vNo);if (!string.IsNullOrEmpty(_serializationOption.Token))
                if (!string.IsNullOrEmpty(_serializationOption.Token))
                {
                    webRequest.SetRequestHeader("Authorization", $"Bearer {_serializationOption.Token}");
                    Debug.Log($"token: {_serializationOption.Token} ");
                }
                // webRequest.SetRequestHeader("Device-Type", serializationOption.UserIdentifier.DeviceType);
                // webRequest.SetRequestHeader("UUID", string.IsNullOrEmpty(serializationOption.UserIdentifier.UUID) ? SystemInfo.deviceUniqueIdentifier : serializationOption.UserIdentifier.UUID);
                webRequest.timeout = Timeout;

                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed: {webRequest.error} , {webRequest.result} "); //, ErrorTypes.API_CALL);
                    _gameEvents.ErrorReceived((int)webRequest.responseCode, webRequest.downloadHandler.text);
                    return default;
                }
                Debug.Log($"Response: {url} | {webRequest.downloadHandler.text}"); //, ErrorTypes.API_CALL);
                var result = _serializationOption.Deserialize<TResultType>(webRequest.downloadHandler.text);
                Debug.Log(MySerializer.Serialize(result));
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(Post)} Failed: {url} | {e.Message} | {e.StackTrace} "); //, ErrorTypes.API_CALL);
                return default;
            }
        }


        public async Task<TResultType> PostForm<TResultType>(string url, WWWForm postData)
        {
            try
            {
                Debug.Log($"CALLING: {url} | {MySerializer.Serialize(postData)}"); //, ErrorTypes.API_CALL);

                //Debug.Log(MySerializer.Serialize(postData));
                using var webRequest = new UnityWebRequest(url, "POST"); //Post(url, postData);
                byte[] jsonToSend = postData.data; //new System.Text.UTF8Encoding().GetBytes(postData);

                //webRequest.method = "POST";

                webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "multipart/form-data");

                // webRequest.SetRequestHeader("vNo", serializationOption.UserIdentifier.vNo);
                // webRequest.SetRequestHeader("token", serializationOption.UserIdentifier.token);
                // webRequest.SetRequestHeader("Device-Type", serializationOption.UserIdentifier.DeviceType);
                // webRequest.SetRequestHeader("UUID", string.IsNullOrEmpty(serializationOption.UserIdentifier.UUID) ? SystemInfo.deviceUniqueIdentifier : serializationOption.UserIdentifier.UUID);
                webRequest.timeout = Timeout;

                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed: {webRequest.error} , {webRequest.result} "); //, ErrorTypes.API_CALL);
                    _gameEvents.ErrorReceived((int)webRequest.responseCode, webRequest.downloadHandler.text);
                    return default;
                }
                Debug.Log($"Success: {url} | {webRequest.downloadHandler.text}"); //, ErrorTypes.API_CALL);
                var result = _serializationOption.Deserialize<TResultType>(webRequest.downloadHandler.text);
                Debug.Log(MySerializer.Serialize(result));
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(Post)} Failed: {url} | {e.Message} | {e.StackTrace} "); //, ErrorTypes.API_CALL);
                return default;
            }
        }

        public async Task<TResultType> Put<TResultType>(string url, string postData, int timeOut = -100)
        {
            try
            {
                Debug.Log($"CALLING: {url} {MySerializer.Serialize(postData)}"); //, ErrorTypes.API_CALL);

                //Debug.Log(MySerializer.Serialize(postData));
                byte[] jsonToSend = new UTF8Encoding().GetBytes(postData);
                using var webRequest = UnityWebRequest.Put(url, jsonToSend);
                {
                    // new UnityWebRequest(url, "PATCH"); //Post(url, postData);
                    //webRequest.method = "POST";
                    webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    webRequest.SetRequestHeader("Content-Type", _serializationOption.ContentType);

                    // webRequest.SetRequestHeader("vNo", serializationOption.UserIdentifier.vNo);
                    // webRequest.SetRequestHeader("token", serializationOption.UserIdentifier.token);
                    // webRequest.SetRequestHeader("Device-Type", serializationOption.UserIdentifier.DeviceType);
                    // webRequest.SetRequestHeader("UUID", string.IsNullOrEmpty(serializationOption.UserIdentifier.UUID) ? SystemInfo.deviceUniqueIdentifier : serializationOption.UserIdentifier.UUID);

                    if (timeOut == -100)
                        webRequest.timeout = Timeout;
                    else
                    {
                        if (timeOut <= 0)
                        {
                            // webRequest.Dispose();
                            return default;
                        }
                        webRequest.timeout = timeOut > Timeout ? Timeout : timeOut;
                    }

                    //Debug.Log(url + "  :  " + webRequest.timeout);
                    var operation = webRequest.SendWebRequest();

                    while (!operation.isDone)
                        await Task.Yield();

                    if (webRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"Failed: {webRequest.error} , {webRequest.result} "); //, ErrorTypes.API_CALL);
                        _gameEvents.ErrorReceived((int)webRequest.responseCode, webRequest.downloadHandler.text);

                        // webRequest.Dispose();
                        return default;
                    }
                    Debug.Log($"Success: {url} | {webRequest.downloadHandler.text}"); //, ErrorTypes.API_CALL);
                    var result = _serializationOption.Deserialize<TResultType>(webRequest.downloadHandler.text);
                    Debug.Log(MySerializer.Serialize(result));

                    // webRequest.Dispose();
                    return result;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(Put)} Failed: {url} | {e.Message} | {e.StackTrace} "); //, ErrorTypes.API_CALL);
                return default;
            }
        }

        public IEnumerator CoGetTexture(string url, int id)
        {
            using var webRequest = UnityWebRequestTexture.GetTexture(url);

            // webRequest.SetRequestHeader("vNo", serializationOption.UserIdentifier.vNo);
            // webRequest.SetRequestHeader("token", serializationOption.UserIdentifier.token);
            // webRequest.SetRequestHeader("Device-Type", serializationOption.UserIdentifier.DeviceType);
            // webRequest.SetRequestHeader("UUID", string.IsNullOrEmpty(serializationOption.UserIdentifier.UUID) ? SystemInfo.deviceUniqueIdentifier : serializationOption.UserIdentifier.UUID);
            //webRequest.SetRequestHeader("Content-Type", serializationOption.ContentType);
            yield return webRequest.SendWebRequest();

            //while (!operation.isDone)
            //    yield return null;
            try
            {
                if (webRequest.result != UnityWebRequest.Result.Success)
                    Debug.LogError($"Failed: {webRequest.error}"); //, ErrorTypes.API_CALL);

                //var result = serializationOption.Deserialize<TResultType>(webRequest.downloadHandler.text);
                //Debug.Log($"Success: {webRequest.downloadHandler.text}");
                // questionsDataSO.textureDict.Add(id, ((DownloadHandlerTexture)webRequest.downloadHandler).texture); //DownloadHandlerTexture.GetContent(webRequest);
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(Get)} Failed: {e.Message} | Stacktrace: {e.StackTrace}"); //, ErrorTypes.API_CALL);
            }
        }

        // public async Task<Texture2D> GetTexture(string url)
        // {
        //     Debug.Log("getTexture : " + url);
        //     if (string.IsNullOrEmpty(url) || url.Contains("null", System.StringComparison.OrdinalIgnoreCase))
        //     {
        //         return default;
        //     }
        //
        //     using var webRequest = UnityWebRequestTexture.GetTexture(url);
        //     // webRequest.SetRequestHeader("vNo", serializationOption.UserIdentifier.vNo);
        //     // webRequest.SetRequestHeader("token", serializationOption.UserIdentifier.token);
        //     // webRequest.SetRequestHeader("Device-Type", serializationOption.UserIdentifier.DeviceType);
        //     // webRequest.SetRequestHeader("UUID", string.IsNullOrEmpty(serializationOption.UserIdentifier.UUID) ? SystemInfo.deviceUniqueIdentifier : serializationOption.UserIdentifier.UUID);
        //
        //     try
        //     {
        //         var operation = webRequest.SendWebRequest();
        //
        //         while (!operation.isDone)
        //             await Task.Yield();
        //
        //         Texture2D result;
        //         if (url.Contains(".webp"))
        //         {
        //             result = Texture2DExt.CreateTexture2DFromWebP(webRequest.downloadHandler.data, true, false, out Error error);
        //             if (error != Error.Success)
        //                 Debug.Log($"Unable to fetch texture: {url} | {error} | {webRequest.downloadHandler.error}"); //); //, ErrorTypes.API_CALL);
        //         }
        //         else
        //         {
        //             if (webRequest.result != UnityWebRequest.Result.Success) // && !url.Contains("webp"))
        //                 Debug.Log($"Unable to fetch texture: {url} | {webRequest.error} | {webRequest.downloadHandler.error}"); //); //, ErrorTypes.API_CALL);
        //
        //             result = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
        //         }
        //
        //         return result ?? default;
        //     }
        //     catch (System.Exception e)
        //     {
        //         Debug.Log($"Failed to Get Texture : {e.Message}, {e.StackTrace},{url}"); //); //, ErrorTypes.API_CALL);
        //         return default;
        //     }
        // }


        public async Task SaveTexture(string key, string url, string filename)
        {
            using var webRequest = UnityWebRequestTexture.GetTexture(url);

            // webRequest.SetRequestHeader("vNo", serializationOption.UserIdentifier.vNo);
            // webRequest.SetRequestHeader("token", serializationOption.UserIdentifier.token);
            // webRequest.SetRequestHeader("Device-Type", serializationOption.UserIdentifier.DeviceType);
            // webRequest.SetRequestHeader("UUID", string.IsNullOrEmpty(serializationOption.UserIdentifier.UUID) ? SystemInfo.deviceUniqueIdentifier : serializationOption.UserIdentifier.UUID);
            try
            {
                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (webRequest.result != UnityWebRequest.Result.Success)
                    Debug.Log($"Unable to fetch texture: {url}. {webRequest.error} {webRequest.downloadHandler.error}"); //, ErrorTypes.API_CALL);

                string path = $"{Application.persistentDataPath}/{key}/{filename}";
                byte[] dataBytes = webRequest.downloadHandler.data; //result.EncodeToPNG();
                await File.WriteAllBytesAsync(path, dataBytes);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to Get Texture : {e.Message}, {e.StackTrace}"); //, ErrorTypes.API_CALL);

                //return default;
            }
        }


        public async Task GetFile(int key, string url)
        {
            using var webRequest = UnityWebRequest.Get(url);
            string path = $"{Application.persistentDataPath}/{key}/";
            Debug.Log(path);
            webRequest.downloadHandler = new DownloadHandlerFile(path);

            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {webRequest.error}"); //, ErrorTypes.API_CALL);
        }
    }
}