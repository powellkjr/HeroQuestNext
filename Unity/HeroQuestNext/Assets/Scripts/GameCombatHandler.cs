using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCombatHandler : MonoBehaviour
{
    private BlackBocksGrid<HeroTile> arrGrid;
    public static GameCombatHandler Instance { get; private set; }

    public List<Player> lPlayerTeam;
    public List<Player> lEnemyTeam;

    Player pCurrentPlayer;

    public struct PlayerInit
    {
        Vector2Int vStartPos;
        int iRefIndex;
    }
    public static List<Vector3Int> lPlayerStartPos = new List<Vector3Int> {
        new Vector3Int(1, 4,(int)ePlayers.Barbarian),
        new Vector3Int(2, 4,(int)ePlayers.Dwarf),
        new Vector3Int(1, 3,(int)ePlayers.Elf),
        new Vector3Int(2, 3,(int)ePlayers.Wizard)
    };
    public static List<Vector3Int> lEnemyStartPos = new List<Vector3Int> {
        new Vector3Int(0, 5,(int)eMonsters.Goblin),
        new Vector3Int(2, 6,(int)eMonsters.Orc),
        new Vector3Int(2, 7,(int)eMonsters.Goblin),
        new Vector3Int(10, 4,(int)eMonsters.Goblin) ,
        new Vector3Int(10, 2,(int)eMonsters.Fimir),
        new Vector3Int(8, 3,(int)eMonsters.Orc),
        new Vector3Int(7, 4,(int)eMonsters.Orc),

        new Vector3Int(12, 0,(int)eMonsters.Orc),
        
        //new Vector3Int(3, 12,(int)eMonsters.Goblin),
        //new Vector3Int(2, 16,(int)eMonsters.Skeleton),
        //new Vector3Int(3, 16,(int)eMonsters.Skeleton),
        //new Vector3Int(6, 17,(int)eMonsters.Zombie) ,
        //new Vector3Int(6, 16,(int)eMonsters.Mummy),
        //new Vector3Int(6, 15,(int)eMonsters.Zombie),
        //new Vector3Int(9, 15,(int)eMonsters.Skeleton),
        //new Vector3Int(9, 14,(int)eMonsters.Mummy),
        //new Vector3Int(10, 14,(int)eMonsters.Skeleton),

        //new Vector3Int(6, 11,(int)eMonsters.Goblin),
        //new Vector3Int(7, 11,(int)eMonsters.Goblin),


        //new Vector3Int(12, 10,(int)eMonsters.Gargoyle),
        //new Vector3Int(14, 10,(int)eMonsters.Fimir),
        //new Vector3Int(11, 7,(int)eMonsters.ChaosWarrior),
        //new Vector3Int(14, 7,(int)eMonsters.ChaosWarrior),

        //new Vector3Int(17, 4,(int)eMonsters.Fimir),
        //new Vector3Int(16, 3,(int)eMonsters.ChaosWarrior),
        //new Vector3Int(15, 2,(int)eMonsters.ChaosWarrior),
    };

    int iPlayerCount = lPlayerStartPos.Count;
    int iEnmeyCount = lEnemyStartPos.Count;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        lPlayerTeam = new List<Player>();
        for(int i = 0; i < iPlayerCount; i++)
        {
            lPlayerTeam.Add(new Player((eMoveableType.Player,i), lPlayerStartPos[i]));
        }

        lEnemyTeam = new List<Player>();
        for (int i = 0; i < iEnmeyCount; i++)
        {
            lEnemyTeam.Add(new Player((eMoveableType.Enemy,i), lEnemyStartPos[i]));
        }
        pCurrentPlayer = lEnemyTeam[lEnemyTeam.Count-1];

        GetNextPlayerTurn();
        //HeroMapVisual.Instance.SetGameCombatHandler(Instance);

        
    }

    public void SetGrid(BlackBocksGrid<HeroTile> inGrid)
    {
        this.arrGrid = inGrid;
    }
    // Update is called once per frame
    void Update()
    {
        
        foreach (Player aPlayer in lPlayerTeam)
        {
            aPlayer.GameCombatUpdate();
        }

        foreach (Player aPlayer in lEnemyTeam)
        {
            aPlayer.GameCombatUpdate();
        }
        if(arrGrid == null)
        {
            arrGrid = PathFinding<HeroTile>.Instance.GetGrid();
        }
        arrGrid.TriggerGridObjectChanged(0, 0);
        GetNextPlayerTurn();
    }

    private void GetNextPlayerTurn()
    {
        if (pCurrentPlayer.GetPlayerState() != ePlayerStateType.Idle)
        {
            return;
        }


        eMoveableType eCurrentTeam;
        int iCurrentIndex;

       
        eCurrentTeam = pCurrentPlayer.GetMoveableKey().Item1;
        iCurrentIndex = pCurrentPlayer.GetMoveableKey().Item2;
        if (eCurrentTeam == eMoveableType.Player)
        {
            if (iCurrentIndex < lPlayerTeam.Count-1)
            {
                pCurrentPlayer = lPlayerTeam[iCurrentIndex+1];
            }
            else
            {
                pCurrentPlayer = lEnemyTeam[0];
            }
        }
        else
        {
            if (iCurrentIndex < lEnemyTeam.Count-1)
            {
                pCurrentPlayer = lEnemyTeam[iCurrentIndex+1];
            }
            else
            {
                pCurrentPlayer = lPlayerTeam[0];
            }
        }
        pCurrentPlayer.SetPlayerState(ePlayerStateType.WaitingForTarget);
    }
}
