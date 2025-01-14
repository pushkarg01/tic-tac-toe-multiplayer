using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGO, circleArrowGO;
    [SerializeField] private GameObject crossTextGO, circleTextGO;
    [SerializeField] private TextMeshProUGUI crossScore, circleScore;

    private void Awake()
    {
        crossArrowGO.SetActive(false);
        circleArrowGO.SetActive(false);
        circleTextGO.SetActive(false);
        crossTextGO.SetActive(false);

        crossScore.text = "";
        circleScore.text = "";
    }

    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnCurrentTypeChange += GameManager_OnCurrentTypeChange;
        GameManager.Instance.OnScoreChange += GameManager_OnScoreChange;
    }

    private void GameManager_OnScoreChange(object sender, System.EventArgs e)
    {
        GameManager.Instance.GetScores(out int playerCrossScore, out int playerCircleScore);

        crossScore.text = playerCrossScore.ToString();
        circleScore.text = playerCircleScore.ToString();
    }

    private void GameManager_OnCurrentTypeChange(object sender, System.EventArgs e)
    {
        UpdateCurrentArrow();
    }

    private void GameManager_OnGameStarted(object sender, System.EventArgs e)
    {
        if(GameManager.Instance.GetPlayerType()== GameManager.PlayerType.Cross)
        {
            crossTextGO.SetActive(true);
        }
        else
        {
            circleTextGO.SetActive(true);
        }
        crossScore.text = "0";
        circleScore.text = "0";

        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if(GameManager.Instance.GetCurrentPlayerType()== GameManager.PlayerType.Cross)
        {
            crossArrowGO.SetActive(true);
            circleArrowGO.SetActive(false);
        }
        else
        {
            crossArrowGO.SetActive(false);
            circleArrowGO.SetActive(true);
        }
    }
}
