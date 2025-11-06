using System;
using System.Threading.Tasks;
using UnityEngine;
using SocketIOClient;

namespace W3.Tambola.Network
{
    public class SocketService : IDisposable // IInitializable,
    {
        private SocketIOUnity _socket;

        public bool IsConnected => _socket != null && _socket.Connected;

        public event Action OnConnected;
        public event Action OnDisconnected;

        public async void Initialize()
        {
            var uri = new Uri("http://localhost:3000"); // change to backend IP if needed

            _socket = new SocketIOUnity(uri, new SocketIOOptions
            {
                Path = "/socket.io/",
                Reconnection = true,
                ReconnectionAttempts = 5,
                ReconnectionDelay = 2000
            });

            _socket.OnConnected += (sender, e) =>
            {
                Debug.Log("[SocketService] Connected");
                OnConnected?.Invoke();
            };

            _socket.OnDisconnected += (sender, e) =>
            {
                Debug.Log("[SocketService] Disconnected");
                OnDisconnected?.Invoke();
            };

            _socket.On("pong_from_server", response =>
            {
                Debug.Log("[SocketService] Pong: " + response.GetValue().ToString());
            });

            await ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            if (_socket == null)
                return;

            try
            {
                Debug.Log("[SocketService] Connecting...");
                await _socket.ConnectAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SocketService] Connection failed: {e.Message}");
            }
        }

        public void Emit(string eventName, object data = null)
        {
            if (_socket == null || !_socket.Connected)
            {
                Debug.LogWarning("[SocketService] Tried to emit but socket not connected.");
                return;
            }

            if (data == null)
                _socket.Emit(eventName);
            else
                _socket.Emit(eventName, data);
        }

        public void On(string eventName, Action<SocketIOResponse> callback)
        {
            _socket?.On(eventName, callback);
        }

        public void Dispose()
        {
            Debug.Log("[SocketService] Disposing...");
            _socket?.Disconnect();
        }
    }
}
