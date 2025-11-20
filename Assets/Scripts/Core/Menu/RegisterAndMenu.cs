using Network.API;
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
    private bool _isLogin = false;

    [Inject] private IMessageBox _messageBox;
    [Inject] private APIHandler _apiHandler;

    private void Start()
    {
        if (_isLogin)
        {
            OnAlreadyLogin();
            return;
        }
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

        _apiHandler.RegisterUser(_userName, (Status, data) =>
        {
           if (Status)
            {
                _registerMenu.SetActive(false);
                _mainMenu.SetActive(true);
                UniversalConstants.UserName = _userName;
                PlayerPrefs.SetString(UserPrefs.UserName, _userName);
                Debug.Log("User Registered Successfully: "+ UniversalConstants.UserName);
                _isLogin = true;
                PlayerPrefs.SetInt(UserPrefs.IsLoggedIn, _isLogin ? 1 : 0);
            }
            else
            {
                _messageBox.UpdateMessage(data);
            }
        });
    }

    private void OnAlreadyLogin()
    {
        _apiHandler.CheckUserExistence(UniversalConstants.AuthID,(Status, data) =>
        {
            if (Status)
            {
                UniversalConstants.UserName = data;
                Debug.Log("User Already Logged In: " + UniversalConstants.UserName);

                _registerMenu.SetActive(false);
                _mainMenu.SetActive(true);
            }
            else
            {
                _isLogin = false;
                PlayerPrefs.SetInt(UserPrefs.IsLoggedIn, _isLogin ? 1 : 0);
            }
        });
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
