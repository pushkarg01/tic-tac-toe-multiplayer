using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton, clientButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Hide();
        });

        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
