using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI result;
    [SerializeField] private Color winColor, loseColor,tieColor;
    [SerializeField] private Button rematchButton;

    private void Awake()
    {
        rematchButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RematchRpc();
        });    
    }

    private void Start()
    {
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        GameManager.Instance.OnTie += GameManager_OnTie;
        Hide();
    }

    private void GameManager_OnTie(object sender, System.EventArgs e)
    {
        result.text = "TIE!";
        result.color = tieColor;
        Show();
    }

    private void GameManager_OnRematch(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if(e.winPlayerType == GameManager.Instance.GetPlayerType())
        {
            result.text = "YOU WIN!";
            result.color = winColor;
        }
        else
        {
            result.text = "YOU LOSE!";
            result.color = loseColor;
        }
       Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
