using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCombatHandler : MonoBehaviour
{
    private BlackBocksGrid<HeroTile> arrGrid;
    private PathFinding<HeroTile> pPathFinding;
    private DiceManager cDiceManager;
    public static GameCombatHandler Instance { get; private set; }

    public List<Player> lPlayerTeam;
    public List<Player> lEnemyTeam;

    public (eSpritePages, eVisualIcons, Vector2Int) sCursor;

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

        new Vector3Int(3, 12,(int)eMonsters.Goblin),
        new Vector3Int(2, 16,(int)eMonsters.Skeleton),
        new Vector3Int(3, 16,(int)eMonsters.Skeleton),
        new Vector3Int(6, 17,(int)eMonsters.Zombie) ,
        new Vector3Int(6, 16,(int)eMonsters.Mummy),
        new Vector3Int(6, 15,(int)eMonsters.Zombie),
        new Vector3Int(9, 15,(int)eMonsters.Skeleton),
        new Vector3Int(9, 14,(int)eMonsters.Mummy),
        new Vector3Int(10, 14,(int)eMonsters.Skeleton),

        new Vector3Int(6, 11,(int)eMonsters.Goblin),
        new Vector3Int(7, 11,(int)eMonsters.Goblin),


        new Vector3Int(12, 10,(int)eMonsters.Gargoyle),
        new Vector3Int(14, 10,(int)eMonsters.Fimir),
        new Vector3Int(11, 7,(int)eMonsters.ChaosWarrior),
        new Vector3Int(14, 7,(int)eMonsters.ChaosWarrior),

        new Vector3Int(17, 4,(int)eMonsters.Fimir),
        new Vector3Int(16, 3,(int)eMonsters.ChaosWarrior),
        new Vector3Int(15, 2,(int)eMonsters.ChaosWarrior),
    };

    private static int iPlayerCount = lPlayerStartPos.Count;
    private static int iEnmeyCount = lEnemyStartPos.Count;
    [SerializeField]
    private int iUsePlayers;
    [SerializeField]
    private int iUseEnemies;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        new DiceManager();
        cDiceManager = DiceManager.Instance;
    }
    private void Start()
    {
        pPathFinding = PathFinding<HeroTile>.Instance;
        arrGrid = HeroMap.Instance.GetGrid();
        HeroMapVisual.Instance.SetGameCombatHandler(this);
        HeroMapVisual.Instance.Load();
        lPlayerTeam = new List<Player>();
        for (int i = 0; i < Mathf.Min(iUsePlayers, iPlayerCount); i++)
        {
            int iThisPlayer = Random.Range(0, lPlayerStartPos.Count);
            lPlayerTeam.Add(new Player((eMoveableType.Player, i), lPlayerStartPos[iThisPlayer]));
            lPlayerStartPos.RemoveAt(iThisPlayer);
        }

        lEnemyTeam = new List<Player>();
        if (iUseEnemies == 1)
        {
            lEnemyTeam.Add(new Player((eMoveableType.Enemy, 0), new Vector3Int(3, 1, (int)eMonsters.Gargoyle)));

        }
        else
        {


            for (int i = 0; i < Mathf.Min(iUseEnemies, iEnmeyCount); i++)
            {
                int iThisPlayer = Random.Range(0, lEnemyStartPos.Count);
                lEnemyTeam.Add(new Player((eMoveableType.Enemy, i), lEnemyStartPos[iThisPlayer]));
                lEnemyStartPos.RemoveAt(iThisPlayer);
            }
        }
        pCurrentPlayer = lEnemyTeam[lEnemyTeam.Count - 1];

        GetNextPlayerTurn();
        //HeroMapVisual.Instance.SetGameCombatHandler(Instance);


    }

    // Update is called once per frame
    void Update()
    {
        sCursor = (eSpritePages.MoveTiles, eVisualIcons.CornerSelector, pCurrentPlayer.GetPosXY());
        if (pCurrentPlayer.GetMoveableKey().Item1 == eMoveableType.Player && pCurrentPlayer.GetPlayerState() == ePlayerStateType.WaitingForMoveTarget)
        {

            Vector3 vMousePositon = BlackBocks.GetMouseWorldPosition();
            Vector2Int vEndPos = pPathFinding.GetGrid().GetGridPostion(vMousePositon);
            if (arrGrid.IsValid(vEndPos))
            {
                sCursor = (eSpritePages.ActTiles, eVisualIcons.CornerSelector, vEndPos);
                arrGrid.TriggerGridObjectChanged(vEndPos);
                if (pCurrentPlayer.lMoveRange != null && pCurrentPlayer.lMoveRange.Count > 0 && pCurrentPlayer.lMoveRange.Contains(arrGrid.GetGridObject(vEndPos)))
                {

                    sCursor = (eSpritePages.MoveTiles, eVisualIcons.CornerSelector,vEndPos);
                    //arrGrid.TriggerGridObjectChanged(vEndPos);
                    //hHeatMapVisual.bDebugEnabled = bDebugEnabled;
                    if (Input.GetMouseButtonDown(0))
                    {
                        pCurrentPlayer.SetMoveTarget(vEndPos);

                    }
                }
            }
        }


        foreach (Player aPlayer in lPlayerTeam)
        {
            aPlayer.GameCombatUpdate();
        }

        foreach (Player aPlayer in lEnemyTeam)
        {
            aPlayer.GameCombatUpdate();
        }

        
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
        pCurrentPlayer.SetPlayerState(ePlayerStateType.Reset);
    }
}
