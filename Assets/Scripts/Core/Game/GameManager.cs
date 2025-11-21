using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }

    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB,
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance");
        }
        Instance = this;
   }


}
