using UnityEngine;
using SocketIOClient;

public class SocketManager : MonoBehaviour
{
    private SocketIOUnity socket;

    void Start()
    {
        // Use ws:// or wss:// depending on your backend
        var uri = new System.Uri("http://localhost:3000");

        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Path = "/socket.io/",
            Reconnection = true,
            ReconnectionAttempts = 5,
            ReconnectionDelay = 2000
        });

        // Bind events
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connected to Socket.IO server!");
        };

        socket.On("pong_from_server", response =>
        {
            Debug.Log($"Pong from server: {response}");
        });

        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Disconnected from server");
        };

        // Connect
        socket.Connect();
    }

    void Update()
    {
        // Example: send ping when pressing space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            socket.Emit("ping_from_client", "Ping from Unity!");
        }
    }

    void OnDestroy()
    {
        socket.Disconnect();
    }
}
