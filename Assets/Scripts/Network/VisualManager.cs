using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VisualManager : NetworkBehaviour
{
    [SerializeField] private Transform crossPrefab, circlePrefab,lineCompletePrefab;
    private const float gridSize = 3.1f;

    private List<GameObject> visualGameObjectList;

    private void Awake()
    {
        visualGameObjectList = new List<GameObject>();
    }

    void Start()
    {
        GameManager.Instance.OnClickedOnGridPos += GameManager_OnClickedOnGridPos;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
    }

    private void GameManager_OnRematch(object sender, System.EventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        foreach (GameObject visualGO in visualGameObjectList)
        {
            Destroy(visualGO);
        }
        visualGameObjectList.Clear();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        float eularZ = 0f;
        switch (e.line.orientation)
        {
            default:
            case GameManager.Orientation.Horizontal:    eularZ =0f; break;
            case GameManager.Orientation.Vertical:      eularZ =90f; break;
            case GameManager.Orientation.DiagonalA:     eularZ = 45f; break;
            case GameManager.Orientation.DiagonalB:     eularZ = -45f; break;
        }
        Transform lineComplete = Instantiate(lineCompletePrefab,
            GetWorldGridPos(e.line.centerGridPos.x,e.line.centerGridPos.y),Quaternion.Euler(0,0,eularZ));

        lineComplete.GetComponent<NetworkObject>().Spawn(true);

        visualGameObjectList.Add(lineComplete.gameObject);
    }

    private void GameManager_OnClickedOnGridPos(object sender, GameManager.OnClickedOnGridPosEventArgs e)
    {
        SpawnObjRpc(e.x, e.y,e.playerType);
    }

    // Remote Procedural Calls
    [Rpc(SendTo.Server)]
    private void SpawnObjRpc(int x,int y,GameManager.PlayerType playerType)
    {
        Transform prefab;
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                prefab = crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = circlePrefab;
                break;
        }

        Transform spawnedCross = Instantiate(prefab,GetWorldGridPos(x, y), Quaternion.identity);
        spawnedCross.GetComponent<NetworkObject>().Spawn(true);

        visualGameObjectList.Add(spawnedCross.gameObject);
    }

    private Vector2 GetWorldGridPos(int x,int y)
    {
        return new Vector2(-gridSize +x*gridSize,-gridSize +y*gridSize);
    }
}
