using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static GameManager;

public class GameManager :NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPosEventArgs> OnClickedOnGridPos;
    public class OnClickedOnGridPosEventArgs : EventArgs
    {
        public int x, y;
        public PlayerType playerType;
    }

    public event EventHandler OnGameStarted;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
        public PlayerType winPlayerType;
    }
    public event EventHandler OnCurrentTypeChange;
    public event EventHandler OnRematch;
    public event EventHandler OnTie;
    public event EventHandler OnScoreChange;
    public event EventHandler OnPlacedObject;

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

    public struct Line 
    {
        public List<Vector2Int> gridList;
        public Vector2Int centerGridPos;
        public Orientation orientation;
    }

    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayerType = new NetworkVariable<PlayerType>();  // Network Variable

    private PlayerType[,] playerTypeArray; 
    private List<Line> lineList;

    private NetworkVariable<int> playerCrossScore = new NetworkVariable<int>();
    private NetworkVariable<int> playerCircleScore = new NetworkVariable<int>();

    private void Awake()
    {
        if(Instance != null) {
            Debug.Log("More than one instance");
        }
        Instance = this;

        playerTypeArray = new PlayerType[3, 3];

        lineList = new List<Line>
        {
            // Horizontal
            new Line
            {
                gridList =  new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,0),new Vector2Int(2,0) },centerGridPos = new Vector2Int(1,0),
                orientation=Orientation.Horizontal
            },
            new Line
            {
                gridList =  new List<Vector2Int>{new Vector2Int(0,1),new Vector2Int(1,1),new Vector2Int(2,1) },centerGridPos = new Vector2Int(1,1),
                orientation=Orientation.Horizontal
            },
            new Line
            {
                gridList =  new List<Vector2Int>{new Vector2Int(0,2),new Vector2Int(1,2),new Vector2Int(2,2) },centerGridPos = new Vector2Int(1,2),
                orientation=Orientation.Horizontal
            },

            // Vertical
            new Line
            {
                gridList =  new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(0,1),new Vector2Int(0,2) },centerGridPos = new Vector2Int(0,1),
                orientation=Orientation.Vertical
            },
            new Line
            {
                gridList =  new List<Vector2Int>{new Vector2Int(1,0),new Vector2Int(1,1),new Vector2Int(1,2) },centerGridPos = new Vector2Int(1,1),
                orientation=Orientation.Vertical
            },
            new Line
            {
                gridList =  new List<Vector2Int>{new Vector2Int(2,0),new Vector2Int(2,1),new Vector2Int(2,2) },centerGridPos = new Vector2Int(2,1),
                orientation=Orientation.Vertical
            },

            //Diagonal
            new Line
            {
                gridList =  new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,1),new Vector2Int(2,2) },centerGridPos = new Vector2Int(1,1),
                orientation=Orientation.DiagonalA
            },
            new Line
            {
                gridList =  new List<Vector2Int>{new Vector2Int(0,2),new Vector2Int(1,1),new Vector2Int(2,0) },centerGridPos = new Vector2Int(1,1),
                orientation=Orientation.DiagonalB
            },

        };
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(NetworkManager.Singleton.LocalClientId);
        if(NetworkManager.Singleton.LocalClientId == 0 )
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        currentPlayerType.OnValueChanged += (PlayerType oldPlayer, PlayerType newPlayer) => {
            OnCurrentTypeChange?.Invoke(this, EventArgs.Empty);
        };

        playerCrossScore.OnValueChanged += (int prevScore, int newScore) =>
        {
            OnScoreChange?.Invoke(this, EventArgs.Empty);
        };

        playerCircleScore.OnValueChanged += (int prevScore, int newScore) =>
        {
            OnScoreChange?.Invoke(this, EventArgs.Empty);
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
       if(NetworkManager.Singleton.ConnectedClientsList.Count == 2)
       {
            currentPlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartRpc();
       } 
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }


    [Rpc(SendTo.Server)]
    public void ClickedGridPosRpc(int x, int y,PlayerType playerType)
    {
        // Debug.Log(x + "," + y);
        if (playerType != currentPlayerType.Value) return;

        if (playerTypeArray[x, y] != PlayerType.None) return;

        playerTypeArray[x, y] = playerType;
        TriggerOnPlacedObjectRpc();

        OnClickedOnGridPos?.Invoke(this, new OnClickedOnGridPosEventArgs
        {
            x = x,
            y = y,
            playerType = playerType
        });

        switch (currentPlayerType.Value)
        {
            default:
            case PlayerType.Cross:
                currentPlayerType.Value = PlayerType.Circle; 
                break;
            case PlayerType.Circle:
                currentPlayerType.Value = PlayerType.Cross; 
                break;
        }
        TestWinner();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnPlacedObjectRpc()
    {
        OnPlacedObject?.Invoke(this, EventArgs.Empty);
    }

    private bool TestWinnerLine(Line line)
    {
        return TestWinnerLine(playerTypeArray[line.gridList[0].x, line.gridList[0].y],
            playerTypeArray[line.gridList[1].x, line.gridList[1].y],
            playerTypeArray[line.gridList[2].x, line.gridList[2].y]);
    }

    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        return aPlayerType != PlayerType.None && aPlayerType == bPlayerType && bPlayerType == cPlayerType;
    }

    private void TestWinner()
    {
        for(int i=0;i<lineList.Count;i++)
        {
            Line line = lineList[i];
            if (TestWinnerLine(line))
            {
                Debug.Log("Winner");
                currentPlayerType.Value = PlayerType.None;
                PlayerType winPlayerType = playerTypeArray[line.centerGridPos.x, line.centerGridPos.y];
                switch (winPlayerType)
                {
                    case PlayerType.Cross:
                        playerCrossScore.Value++;
                        break;
                    case PlayerType.Circle:
                        playerCircleScore.Value++;
                        break;
                }
                TriggerOnGameWinRpc(i,winPlayerType );
                break;
            }
        }

        bool hasTie = true;
        for(int x = 0; x < playerTypeArray.GetLength(0); x++)
        {
            for(int y = 0; y < playerTypeArray.GetLength(1); y++)
            {
                if (playerTypeArray[x, y] == PlayerType.None)
                {
                    hasTie = false;
                    break;
                }
            }
        }
        if(hasTie)
        {
            TriggerOnTieRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnTieRpc()
    {
        OnTie?.Invoke(this, EventArgs.Empty);
    }


    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex,PlayerType winPlayerType)
    {
        Line line = lineList[lineIndex];
        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            line = line,
            winPlayerType = winPlayerType
        });
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for(int x=0;x<playerTypeArray.GetLength(0);x++)
        {
            for(int y=0;y<playerTypeArray.GetLength(1);y++)
            {
                playerTypeArray[x,y]=PlayerType.None;
            }
        }
        currentPlayerType.Value = PlayerType.Cross;

        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }

    #region Return PlayerType Functions

    public PlayerType GetPlayerType()
    {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayerType()
    {
        return currentPlayerType.Value;
    }

    public void GetScores(out int playerCrossScore, out int playerCircleScore)
    {
        playerCrossScore = this.playerCrossScore.Value;
        playerCircleScore = this.playerCircleScore.Value;
    }
    #endregion

}
