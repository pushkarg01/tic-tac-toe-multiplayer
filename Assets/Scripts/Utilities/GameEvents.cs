#region

using System;
using UnityEngine;
using UnityEngine.UI;

#endregion
    public class GameEvents : MonoBehaviour
    {

        public event Action OnSocketConnected = delegate { };
        //public Action<SocketEvents, string> OnSocketEventReceived = delegate { };
        public void SocketConnectionOpen()
        {
            OnSocketConnected.Invoke();
        }
        //public void SocketEventReceived(SocketEvents ev, string data)
        //{
        //    OnSocketEventReceived.Invoke(ev, data);
        //}

        public event Action OnGameStarted;

        public void GameStarted()
        {
            OnGameStarted?.Invoke();
        }

        //Events for Audio
        public Action OnButtonPressed = delegate { };
        public void OntriggerButtonPressed()
        {
            OnButtonPressed?.Invoke();
        }

        public Action OnInGameButtonPressed = delegate { };
        public void OntriggerInGameButtonPressed()
        {
            OnInGameButtonPressed?.Invoke();
        }


        public void ErrorReceived(int errorCode, string responseData)
        {
            Debug.LogError($"Error Received: {errorCode} | Data: {responseData}");
            OnErrorReceived?.Invoke(errorCode, responseData);
        }


        public event Action<int, object> OnErrorReceived;

    }
