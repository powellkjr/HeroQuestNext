using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCombatHandler : MonoBehaviour
{
    //private BlackBocksGrid<HeroTile> arrGrid;
    private PathFinding<HeroTile> pPathFinding;
    
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
    public int GetUsePlayer()
    { 
        return iUsePlayers;
    }

    public int GetUseEnemies()
    {
        return iUseEnemies;
    }
    private void Awake()
    {
        Instance = this;
       
    }
    private void Start()
    {
        pPathFinding = PathFinding<HeroTile>.Instance;
        //arrGrid = HeroMap.Instance.GetGrid();
        HeroMapVisual.Instance.SetGameCombatHandler(this);
        HeroMapVisual.Instance.Load();
        for(int x = 0; x<HeroMap.GetWidth();x++)
        {
            for(int y=0; y< HeroMap.GetHeight();y++)
            {
                HeroMap.GetGridObject(x, y).bTrapScanned = false;
                HeroMap.GetGridObject(x, y).bVisible = false;
                HeroMap.GetGridObject(x, y).bTreasureScanned = false;
            }
        }
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
        //lPlayerTeam[0].OnSkillUpdate += PlayerWidgetController.Instance.PlayerWidgetController_OnSkillUpdate;
        lPlayerTeam[0].OnSkillUpdate += PlayerWidgetController.SetPlayerWidgetText_Static;

    }

    // Update is called once per frame
    void Update()
    {
        sCursor = (eSpritePages.MoveTiles, eVisualIcons.CornerSelector, pCurrentPlayer.GetPosXY());
        if (pCurrentPlayer.GetMoveableKey().Item1 == eMoveableType.Player && pCurrentPlayer.GetPlayerState() == ePlayerStateType.TurnActive)
        {

            Vector3 vMousePositon = BlackBocks.GetMouseWorldPosition();
            Vector2Int vEndPos = pPathFinding.GetGrid().GetGridPostion(vMousePositon);
            if (HeroMap.IsValid(vEndPos))
            {
                sCursor = (eSpritePages.ScanTiles, eVisualIcons.CornerSelector, vEndPos);
                HeroMap.TriggerGridObjectChanged(vEndPos);

                if (pCurrentPlayer.CanAct() && pCurrentPlayer.lActRange != null && pCurrentPlayer.lActRange.Count > 0 && (pCurrentPlayer.lActRange.Contains(HeroMap.GetGridObject(vEndPos)) || pCurrentPlayer.GetPosXY() == vEndPos))
                {
                    sCursor = (eSpritePages.ActTiles, eVisualIcons.CornerSelector, vEndPos);
                    //arrGrid.TriggerGridObjectChanged(vEndPos);
                    //hHeatMapVisual.bDebugEnabled = bDebugEnabled;
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (vEndPos == pCurrentPlayer.GetPosXY())
                        {
                            pCurrentPlayer.SkipPlayerAct();
                        }
                        List<(eMoveableType, int)> tMoveable = HeroMap.GetGridObject(vEndPos).GetMoveable();
                        if (tMoveable != null && tMoveable.Count == 1)
                        {
                            if (tMoveable[0].Item1 == eMoveableType.Enemy)
                            {
                                Player tEnemy = GetEnemyPlayerFromRef(tMoveable[0].Item2);
                                StartCombat(pCurrentPlayer, tEnemy, out bool bTargetkilled);
                                if (bTargetkilled)
                                {
                                    
                                    int iRemove = GetEnemyIndexFromRef(tMoveable[0].Item2);
                                    lEnemyTeam.RemoveAt(iRemove);
                                            HeroMap.GetGridObject(vEndPos).HasLeft(tEnemy.GetMoveableKey());
                                }
                                pCurrentPlayer.SkipPlayerAct();
                            }
                        }


                    }
                }
                if (pCurrentPlayer.CanMove() && pCurrentPlayer.lMoveRange != null && pCurrentPlayer.lMoveRange.Count > 0 && (pCurrentPlayer.lMoveRange.Contains(HeroMap.GetGridObject(vEndPos)) || pCurrentPlayer.GetPosXY() == vEndPos))
                {

                    sCursor = (eSpritePages.MoveTiles, eVisualIcons.CornerSelector, vEndPos);
                    //arrGrid.TriggerGridObjectChanged(vEndPos);
                    //hHeatMapVisual.bDebugEnabled = bDebugEnabled;
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (vEndPos == pCurrentPlayer.GetPosXY())
                        {
                            pCurrentPlayer.SkipPlayerMove();
                        }
                        else
                        {
                            pCurrentPlayer.SetMoveTarget(vEndPos);

                        }
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

        HeroMap.TriggerGridObjectChanged(pCurrentPlayer.GetPosXY());
        GetNextPlayerTurn();


    }

    public Player GetEnemyPlayerFromRef(int inRef)
    {
        for(int i = 0; i < lEnemyTeam.Count; i++)
        {
            if(lEnemyTeam[i].GetMoveableKey().Item2 == inRef)
            {
                return lEnemyTeam[i];
            }
        }
        return null;
    }

    public int GetEnemyIndexFromRef(int inRef)
    {
        for (int i = 0; i < lEnemyTeam.Count; i++)
        {
            if (lEnemyTeam[i].GetMoveableKey().Item2 == inRef)
            { 
                return i;
            }
        }
        return -1;
    }
    private void StartCombat(Player inAttackPlayer, Player inDefendPlayer, out bool bTargetKilled)
    {
        eDiceFace eDefendFace = inDefendPlayer.GetDefendFace();
        int iAttackDice = inAttackPlayer.GetStats().iAttackDice;
        int iDefendDice = inAttackPlayer.GetStats().iDefenceDice;
        int iDamage = DiceManager.RollForAttack(iAttackDice, eDiceFace.Skull, iDefendDice, eDefendFace);
        inDefendPlayer.TakeDamage(iDamage);
        bTargetKilled = inDefendPlayer.GetStats().iHitPoints <= 0;
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
