using UnityEngine;
using UnityEngine.Rendering;

public class UniversalConstants : MonoBehaviour
{
    public static UniversalConstants Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Player Info
    public static string AuthID;
    public static bool IsLogedIn;
    public static string UserName;
    public static bool IsLoggedIn;

    //Game Info

    public static string PlayerSymbol;
    public static string RoomCode;

    // Game Urls

    public static string BaseUrl = "https://physiocratic-kenzie-swimmingly.ngrok-free.dev";

    public static string RegisterUrl = "/auth/register";

    public static string CheckUserExistenceUrl = "/users/me/{0}";

    public static string CreateRoomUrl = "/room/create";

    public static string JoinRoomUrl = "/room/join";
}
