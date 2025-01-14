using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private int x, y;
    private void OnMouseDown()
    {
       GameManager.Instance.ClickedGridPosRpc(x, y,GameManager.Instance.GetPlayerType());
    }
}
