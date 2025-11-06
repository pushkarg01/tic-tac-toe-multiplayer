using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RegisterAndMenu : MonoBehaviour
{
    [SerializeField] private GameObject _registerMenu;
    [SerializeField] private GameObject _mainMenu;

    [SerializeField] private Button _submitBtn;
    [SerializeField] private TMP_InputField _usernameInput;

    string _userName = "";
    private bool _isUserNameValid = false;

    [Inject] private IMessageBox _messageBox;

    private void Start()
    {
        _registerMenu.SetActive(true);
        _mainMenu.SetActive(false);

        _usernameInput.onValueChanged.AddListener(OnvalueChangeInUserName);
        _submitBtn.onClick.AddListener(OnClickedSubmitBtn);
    }

    private void OnvalueChangeInUserName(string value)
    {
        _userName = value;
    }

    private void OnClickedSubmitBtn()
    {
        ValidateUserName();
        if (!_isUserNameValid) return;

        _registerMenu.SetActive(false);
        _mainMenu.SetActive(true);
    }

    private bool ValidateUserName()
    {
        if (_userName == null || _userName == "" || _userName.Length < 3 || _userName.Length >= 10)
        {
            _messageBox.UpdateMessage("Username must be between 3 and 10 characters.");
            _isUserNameValid = false;
            return _isUserNameValid;
        }
        else
        {
            _isUserNameValid = true;
            return _isUserNameValid;

        }
    }
}
