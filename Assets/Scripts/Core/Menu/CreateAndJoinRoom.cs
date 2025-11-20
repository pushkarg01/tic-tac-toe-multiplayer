using Network.API;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

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
        _crossToggle.isOn = true;
        _circleToggle.isOn = false;

        _circleToggle.onValueChanged.AddListener(OnToggleValueChange);
        _crossToggle.onValueChanged.AddListener(OnToggleValueChange);


        if (_userName!=null)
            _userName.text = UniversalConstants.UserName;
        _createRoom.onClick.AddListener(OnClickedCreateRoom);
        _joinRoom.onClick.AddListener(OnClickedJoinRoom);
        _submitBtn.onClick.AddListener(OnClickedSubmitBtn);
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
    }

    private void OnClickedSubmitBtn()
    {
    //    _apiHandler.CreateRoom(UniversalConstants.AuthID,_userName.ToString(),(Status, data) =>
    //    {
    //        if (Status)
    //        {
    //            Debug.Log("Room Created Successfully: " + data);
    //            // Proceed to the room or next steps
    //            UniversalConstants.RoomID = data.roomId.ToString();
    //            SceneManager.LoadScene("Game");
    //        }
    //        else
    //        {
    //            _messageBox.UpdateMessage(data.ToString());
    //        }
    //    });
    }

    private void OnDestroy()
    {
        _createRoom.onClick.RemoveListener(OnClickedCreateRoom);
        _joinRoom.onClick.RemoveListener(OnClickedJoinRoom);
    }

}
