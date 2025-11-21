using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Zenject;
using Network.API;
using UnityEngine.SceneManagement;

public class CreateAndJoinRoom : MonoBehaviour
{
    [SerializeField] private Button _createRoom;
    [SerializeField] private Button _joinRoom;
    [SerializeField] private Button _submitBtn;

    [SerializeField] private GameObject _choosePanel;
    [SerializeField] private TextMeshProUGUI _userName;

    [SerializeField] private Toggle _circleToggle, _crossToggle;

    [Inject] private IMessageBox _messageBox;
    [Inject] private APIHandler _apiHandler;

    void Start()
    {
        _circleToggle.onValueChanged.AddListener(OnToggleValueChange);
        _crossToggle.onValueChanged.AddListener(OnToggleValueChange);

        if (_createRoom != null) _createRoom.onClick.AddListener(OnClickedCreateRoom);
        if (_joinRoom != null) _joinRoom.onClick.AddListener(OnClickedJoinRoom);
        if (_submitBtn != null) _submitBtn.onClick.AddListener(OnClickedSubmitBtn);
    }

    private void OnEnable()
    {
        _crossToggle.isOn = true;
        _circleToggle.isOn = false;


        SetUserName();
    }

    private void OnToggleValueChange(bool value)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            var currentToggle = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();

            if (currentToggle == _circleToggle && value)
            {
                _crossToggle.isOn = false;
            }
            else if (currentToggle == _circleToggle && !value)
            {
                _circleToggle.isOn = true;
            }
            if (currentToggle == _crossToggle && value)
                   _circleToggle.isOn = false;
            else if (currentToggle == _crossToggle && !value)
                _crossToggle.isOn = true;

        }
    }

    private void OnClickedCreateRoom()
    {
        Debug.Log("Create Room Button Clicked");
        OpenCoosePanel();
    }

    private void OnClickedJoinRoom()
    {
        Debug.Log("Join Room Button Clicked");
        OpenCoosePanel();
    }

    private void OpenCoosePanel()
    {
        _createRoom.gameObject.SetActive(false);
        _joinRoom.gameObject.SetActive(false);
        _choosePanel.SetActive(true);

        // Ensure UI shows latest username (in case registration completed after this scene started)
        SetUserName();
    }

    private void SetUserName()
    {
        string currentUser = !string.IsNullOrEmpty(UniversalConstants.UserName)
            ? UniversalConstants.UserName
            : PlayerPrefs.GetString(UserPrefs.UserName, string.Empty);

        if (_userName != null)
            _userName.text = currentUser;
    }

    private void OnClickedSubmitBtn()
    {
        string selectedSymbol = _circleToggle.isOn ? Symbol.Circle.ToString() : Symbol.Cross.ToString();
        // If you will call APIHandler.CreateRoom, ensure AuthID and authenticated client are set.
        // Example (uncomment when backend auth/client is ready):
        // _apiHandler.CreateRoom(UniversalConstants.AuthID, selectedSymbol, (Status, data) =>
        // {
        //     if (Status)
        //     {
        //         Debug.Log("Room Created Successfully: " + data);
        //         UniversalConstants.RoomID = data.roomId.ToString();
        //         SceneManager.LoadScene("Game");
        //     }
        //     else
        //     {
        //         _messageBox.UpdateMessage(data?.ToString() ?? "Room creation failed");
        //     }
        // });
    }

    private void OnDestroy()
    {
        if (_createRoom != null) _createRoom.onClick.RemoveListener(OnClickedCreateRoom);
        if (_joinRoom != null) _joinRoom.onClick.RemoveListener(OnClickedJoinRoom);
        if (_submitBtn != null) _submitBtn.onClick.RemoveListener(OnClickedSubmitBtn);

        _circleToggle.onValueChanged.RemoveListener(OnToggleValueChange);
        _crossToggle.onValueChanged.RemoveListener(OnToggleValueChange);
    }

}
public enum Symbol
{
    Circle,
    Cross
}
