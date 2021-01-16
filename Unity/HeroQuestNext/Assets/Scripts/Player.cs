using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public enum eTeamType
{
    PlayerTeam,
    EnemyTeam
}

public enum ePlayerStateType
{
    Idle,
    WaitingForMoveTarget,
    WaitingForActTarget,
    MovingToTarget,
    AtTarget,
    ActComplete,
    TurnComplete,
    TurnActive,
    Reset,
}

public enum eAI_StateType
{
    Sleeping,
    Roaming,
    FollowTarget,
    FleeTarget,
    Manual,
}







public class Player : MonoBehaviour, ICharacter
//public class Player :  ICharacter
{
    private Vector2 vPosition;
    private Vector2 vStartOfMove;
    private Vector2Int vFromOfMove;
    private Vector2Int vToOfMove;
    private Vector2Int vMoveTarget;
    private Vector2 vVelocity;
    private Vector2Int vHomePos;

    public eTeamType eTeam;
    private ePlayerStateType ePlayerState;
    private eEquipmentRefType ePlayerClass;


    //Texture2D tIcon;
    private int iRefIndex;
    private PathFinding<HeroTile> pPathFinding;
    public List<HeroTile> lCurrentPath;
    public List<HeroTile> lMoveRange;
    public List<HeroTile> lScanRange;
    public List<HeroTile> lActRange;
    public int iActRange = 1;
    //PathFinding<TGridObject> where TGridObject : IPathable<TGridObject> pPathFinding;
    private BlackBocksGrid<HeroTile> arrGrid;
    private DiceManager cDiceManager;
    private bool bActiveTurn { set; get; }
    //private int iMoveRange = 12;
    private float fMoveSpeed = 4f;
    private bool bMoveReady;
    private bool bFirstMove;
    private bool bActReady;
    private EquipmentData sStats;
    private List<EquipmentData> lEquiped;
    private List<EquipmentData> lStored;
    private eAI_StateType eAI_State;
    private int iMovesRemaining;
    (eMoveableType, int) sMoveableKey;
    (eMoveableType, int) sAI_TargetKey;
    private eDiceFace eAttackFace = eDiceFace.Skull;
    private eDiceFace eDefendFace;
    Vector2Int vAI_TargetLastPos;

    public delegate void MyMove();
    MyMove AI_Move;

    delegate void MyAct();

    //public event EventHandler OnSkillUpdate;
    public Action<List<EquipmentData>> OnSkillUpdate;

    private void OnSkillUpdate_Local(List<EquipmentData> inEquipmentData)
    {
        Debug.Log("OnSkillUpdate_Local" + inEquipmentData[0].tToolTipLong.ToString());
    }
    
    public bool CanMove()
    {
        return bMoveReady;
    }
    public bool CanAct()
    {
        return bActReady;
    }
    public void SetPlayerState(ePlayerStateType inPlayerState)
    {
        ePlayerState = inPlayerState;
    }
    public ePlayerStateType GetPlayerState()
    {
        return ePlayerState;
    }
    public void SetRefIndex( int inRefIndex)
    {
        iRefIndex = inRefIndex;
    }
    public int GetRefIndex()
    {
        return iRefIndex;
    }

    public eDiceFace GetDefendFace ()
    {
        return eDefendFace;
    }
    public void TestButton()
    {
        Debug.Log("Clicked a button!");
    }
    public EquipmentData GetStats()
    {
        return sStats;
    }
    public void SetMoveRange(int inMoveRange)
    {
        sStats.iMoveSquares = inMoveRange;
    }
    public bool SetMoveTarget(Vector2Int inMoveTarget)
    {
        List<HeroTile> lPath;
        if(arrGrid.IsValid(inMoveTarget))
        {
            lPath = pPathFinding.FindPath(GetPosXY(),inMoveTarget,sMoveableKey.Item1);
            if (lPath != null)
            {
                lCurrentPath = new List<HeroTile>();
                lCurrentPath = lPath;
                iMovesRemaining -= (lPath.Count - 1);
                ePlayerState = ePlayerStateType.MovingToTarget;
                vStartOfMove = GetPosXY();
                vFromOfMove = GetPosXY();
                vToOfMove = lCurrentPath[0].GetPosition();
                vMoveTarget = inMoveTarget;
                return true;
            }
        }

        return false;

    }
    public Vector2Int GetPosXY()
    {
        return new Vector2Int(Mathf.RoundToInt(vPosition.x), Mathf.RoundToInt(vPosition.y));

    }
    public Vector2 GetPos()
    {
        return vPosition;
    }
    public Vector2 GetMoveTarget()
    {
        return vMoveTarget;
    }
    public void SetPos(Vector2 inPos)
    {
        vPosition = inPos;
    }
    public void SetVelocity(Vector2 inVelocity)
    {
        vVelocity = inVelocity;
    }

    private MyMove GetMyMoveFromIndex()
    {

        switch (iRefIndex)
        {
            case (int)eMonsters.ChaosScorcer:
            case (int)eMonsters.ChaosWarrior:
            case (int)eMonsters.Gargoyle:
            case (int)eMonsters.Fimir:
                return MoveRandomInRoom;
            default:
                return MoveRandomInRange;
        }

    }


  

    public Player(
        (eMoveableType, int) inMoveableKey,
        Vector3Int inStartPos
        )
    {
        sMoveableKey = inMoveableKey;
        vPosition = new Vector2Int(inStartPos.x,inStartPos.y);
        iRefIndex = inStartPos.z;
        bActiveTurn = false;
       

        lMoveRange = new List<HeroTile>();
        lActRange = new List<HeroTile>();
        //iMoveRange = inMoveRange;
        ePlayerState = ePlayerStateType.Idle;
        pPathFinding = PathFinding<HeroTile>.Instance;
        arrGrid = HeroMap.Instance.GetGrid();
        vHomePos = new Vector2Int(inStartPos.x, inStartPos.y);
        vMoveTarget = new Vector2Int(inStartPos.x, inStartPos.y);
        
        arrGrid.GetGridObject(inStartPos.x, inStartPos.y).HasEntered(sMoveableKey);

        lEquiped = new List<EquipmentData>();
        lStored = new List<EquipmentData>();
        sStats = new EquipmentData();
        

        switch (inMoveableKey.Item1)
        {
            case eMoveableType.Player:
                eTeam = eTeamType.PlayerTeam;
                eDefendFace = eDiceFace.White;
                    
                
                switch(iRefIndex)
                {
                    case (int)ePlayers.Barbarian:
                        ePlayerClass = eEquipmentRefType.Body_Barbaian;
                        break;
                    case (int)ePlayers.Dwarf:
                        ePlayerClass = eEquipmentRefType.Body_Dwarf;
                        break;
                    case (int)ePlayers.Elf:
                        ePlayerClass = eEquipmentRefType.Body_Elf;
                        break;
                    case (int)ePlayers.Wizard:
                        ePlayerClass = eEquipmentRefType.Body_Wizard;
                        break;
                }
                EquipmentManger.EquipItem_Static(EquipmentManger.GetEquipmentData_Static(ePlayerClass), out sStats, lEquiped, lStored);
                EquipmentManger.EquipItem_Static(EquipmentManger.GetStartingEquipmentFromBody_Static(ePlayerClass), out sStats, lEquiped, lStored);
                EquipBasicSkills();

                if (sMoveableKey.Item2 == 0)
                {
                    AI_Move = WaitForPlayerMove;
                    eAI_State = eAI_StateType.Manual;
                    PlayerWidgetController.SetPlayerWidgetText_Static(lEquiped);
                    UpdateScanRange();
                    OnSkillUpdate += OnSkillUpdate_Local;
                    UpdateSkillStates();
                }
                else
                {
                    AI_Move = MoveFarthestInRange;
                    eAI_State = eAI_StateType.FollowTarget;
                    sAI_TargetKey = (eMoveableType.Player, 0);
                }

                break;
            case eMoveableType.Enemy:
                eTeam = eTeamType.EnemyTeam;
                eDefendFace = eDiceFace.Black;

                switch (iRefIndex)
                {
                    case (int)eMonsters.ChaosScorcer:
                        ePlayerClass = eEquipmentRefType.Body_ChaosScorcer;
                        break;
                    case (int)eMonsters.ChaosWarrior:
                        ePlayerClass = eEquipmentRefType.Body_ChaosWarrior;
                        break;
                    case (int)eMonsters.Fimir:
                        ePlayerClass = eEquipmentRefType.Body_Fimir;
                        break;
                    case (int)eMonsters.Gargoyle:
                        ePlayerClass = eEquipmentRefType.Body_Gargoyle;
                        break;
                    case (int)eMonsters.Goblin:
                        ePlayerClass = eEquipmentRefType.Body_Goblin;
                        break;
                    case (int)eMonsters.Mummy:
                        ePlayerClass = eEquipmentRefType.Body_Mummy;
                        break;
                    case (int)eMonsters.Orc:
                        ePlayerClass = eEquipmentRefType.Body_Orc;
                        break;
                    case (int)eMonsters.Skeleton:
                        ePlayerClass = eEquipmentRefType.Body_Skeleton;
                        break;
                    case (int)eMonsters.Zombie:
                        ePlayerClass = eEquipmentRefType.Body_Zombie;
                        break;
                }
                EquipmentManger.EquipItem_Static(EquipmentManger.GetEquipmentData_Static(ePlayerClass), out sStats, lEquiped, lStored);
                EquipmentManger.EquipItem_Static(EquipmentManger.GetStartingEquipmentFromBody_Static(ePlayerClass), out sStats, lEquiped, lStored);
                AI_Move = GetMyMoveFromIndex();
                eAI_State = eAI_StateType.Sleeping;
                break;
        };
        
    }

    public void EquipBasicSkills()
    {
        lEquiped.Add(EquipmentManger.GetEquipmentData_Static(eEquipmentRefType.Core_TreasureMap));
        lEquiped.Add(EquipmentManger.GetEquipmentData_Static(eEquipmentRefType.Core_PassageMap));
        lEquiped.Add(EquipmentManger.GetEquipmentData_Static(eEquipmentRefType.Core_TrapMap));
        lEquiped.Add(EquipmentManger.GetEquipmentData_Static(eEquipmentRefType.Legs_PlayerBoots));
        lEquiped.Add(EquipmentManger.GetEquipmentData_Static(eEquipmentRefType.Core_LockPick));
        if (ePlayerClass == eEquipmentRefType.Body_Dwarf)
            lEquiped.Add(EquipmentManger.GetEquipmentData_Static(eEquipmentRefType.Kit_TrapTools));
        if (ePlayerClass == eEquipmentRefType.Body_Wizard || ePlayerClass == eEquipmentRefType.Body_Elf)
            lEquiped.Add(EquipmentManger.GetEquipmentData_Static(eEquipmentRefType.Spell_BallOfFlame));
    }

    public void TakeDamage(int inDamage)
    {
        sStats.iHitPoints -= inDamage;
    }
    public void UpdateScanRange()
    {
       
        List<HeroTile> lScanable = new List<HeroTile>();
        List<HeroTile> lRoomID = new List<HeroTile>();
        List<(Vector2Int, eNavType)> lStartPosAndDir = new List<(Vector2Int, eNavType)>()
        {
            (GetPosXY() + Vector2Int.zero, eNavType.East),
            (GetPosXY() + Vector2Int.zero, eNavType.West),
            (GetPosXY() + Vector2Int.zero, eNavType.North),
            (GetPosXY() + Vector2Int.zero, eNavType.South),
        };
        
        lRoomID.AddRange(pPathFinding.FindAllWithRoomID(arrGrid.GetGridObject(GetPosXY()).GetRoomID()));


        if (!arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.North, eMoveableType.None))
        {
            lRoomID.AddRange(pPathFinding.FindAllWithRoomID(arrGrid.GetGridObject(GetPosXY() + BlackBocks.vNorth).GetRoomID()));
            if(
                !arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.West) &&
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vWest).IsBlocked(eNavType.North) &&
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vNorth).IsBlocked(eNavType.West)
                
                )
            {
                lStartPosAndDir.Add((GetPosXY() + BlackBocks.vWest, eNavType.North));
            }

            if (
                !arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.East) && 
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vEast).IsBlocked(eNavType.North)&&
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vNorth).IsBlocked(eNavType.East))
            {
                lStartPosAndDir.Add((GetPosXY() + BlackBocks.vEast, eNavType.North));
            }
        }

        if (!arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.East, eMoveableType.None))
        {
            lRoomID.AddRange(pPathFinding.FindAllWithRoomID(arrGrid.GetGridObject(GetPosXY() + BlackBocks.vEast).GetRoomID()));
            if (
                !arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.North) && 
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vNorth).IsBlocked(eNavType.East) &&
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vEast).IsBlocked(eNavType.North))
            {
                lStartPosAndDir.Add((GetPosXY() + BlackBocks.vNorth, eNavType.East));
            }

            if (
                !arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.South) && 
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vSouth).IsBlocked(eNavType.East) &&
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vEast).IsBlocked(eNavType.South))
            {
                lStartPosAndDir.Add((GetPosXY() + BlackBocks.vSouth, eNavType.East));
            }
        }

        if (!arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.South, eMoveableType.None))
        {
            lRoomID.AddRange(pPathFinding.FindAllWithRoomID(arrGrid.GetGridObject(GetPosXY() + BlackBocks.vSouth).GetRoomID()));
            if (
                !arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.West) && 
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vWest).IsBlocked(eNavType.South) &&
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vSouth).IsBlocked(eNavType.West))
            {
                lStartPosAndDir.Add((GetPosXY() + BlackBocks.vWest, eNavType.South));
            }

            if (!arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.East) && 
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vEast).IsBlocked(eNavType.South) &&
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vSouth).IsBlocked(eNavType.East)
                )
            {
                lStartPosAndDir.Add((GetPosXY() + BlackBocks.vEast, eNavType.South));
            }
        }

        if (!arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.West, eMoveableType.None))
        {
            lRoomID.AddRange(pPathFinding.FindAllWithRoomID(arrGrid.GetGridObject(GetPosXY() + BlackBocks.vWest).GetRoomID()));
            if (
                !arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.North) && 
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vNorth).IsBlocked(eNavType.West) &&
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vWest).IsBlocked(eNavType.North))

            {
                lStartPosAndDir.Add((GetPosXY() + BlackBocks.vNorth, eNavType.West));
            }

            if (
                !arrGrid.GetGridObject(GetPosXY()).IsBlocked(eNavType.South) && 
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vSouth).IsBlocked(eNavType.West) && 
                !arrGrid.GetGridObject(GetPosXY() + BlackBocks.vWest).IsBlocked(eNavType.South))
            {
                lStartPosAndDir.Add((GetPosXY() + BlackBocks.vSouth, eNavType.West));
            }
        }

 


        foreach ((Vector2Int,eNavType) aPos in lStartPosAndDir)
        {
            List<HeroTile> lTestLine = pPathFinding.FindTileAlongDirection(aPos.Item1, aPos.Item2);
            foreach (HeroTile aTile in lTestLine)
            {
                if (!lScanable.Contains(aTile))
                {
                    lScanable.Add(aTile);
                }
            }
        }

        foreach (HeroTile aTile in lRoomID)
        {
            if (!lScanable.Contains(aTile) && aTile.GetRoomID() != eRoomIDs.Hallways)
            {
                lScanable.Add(aTile);
            }
        }


        lScanRange = lScanable;
        foreach(HeroTile aTile in lScanable)
        { 
            aTile.bVisible = true;
        }
    }

    public void WaitForPlayerMove()
    {
    }

    public void SkipPlayerMove()
    {
        iMovesRemaining = 0;
        ePlayerState = ePlayerStateType.AtTarget;
        ResetActRange();
    }

    public void SkipPlayerAct()
    {
        ePlayerState = ePlayerStateType.ActComplete;
        if(!bFirstMove && bMoveReady)
        {
            bMoveReady = false;
        }
        ResetMoveRange();
    }


    public void MoveRandomInRange()
    {
        if (lMoveRange.Count > 0)
        {
            vMoveTarget = lMoveRange[(int)Random.Range(0, (int)(lMoveRange.Count - 1))].GetPosition();
            if (!SetMoveTarget(vMoveTarget))
            {
                Debug.LogError("Failed to get a next move");

            }
        }
    }

    public void MoveFarthestInRange()
    {
        if (lMoveRange.Count > 0)
        {
            vMoveTarget = GetFarthestInList(GetPosXY(), lMoveRange);
        }
        else 
        { 
                    if (!SetMoveTarget(vMoveTarget))
            {
                Debug.LogError("Failed to get a next move");

            }
        }

    }

    public void MoveFollowTarget()
    {
        List<HeroTile> lSenseRange = pPathFinding.FindTileInRange(GetPosXY(), 15);
        foreach (HeroTile aTile in lSenseRange)
        {
            if (aTile.GetMoveable().Count > 0)
            {
                if (aTile.GetMoveable()[0] == sAI_TargetKey)
                {
                    vAI_TargetLastPos = aTile.GetPosition();
                    break;
                }
            }
        }

        Vector2Int vBackUpPos = GetNearestInList(vAI_TargetLastPos, lMoveRange);

        if (!SetMoveTarget(vBackUpPos))
        {
            Debug.LogError("Failed to get a next move");
        }
    }

    public void MoveFleeTarget()
    {
        List<HeroTile> lSenseRange = pPathFinding.FindTileInRange(GetPosXY(), 15);
        foreach (HeroTile aTile in lSenseRange)
        {
            if (aTile.GetMoveable().Count > 0)
            {
                if (aTile.GetMoveable()[0] == sAI_TargetKey)
                {
                    vAI_TargetLastPos = aTile.GetPosition();
                    break;
                }
            }
        }

        Vector2Int vBackUpPos = GetFarthestInList(vAI_TargetLastPos, lMoveRange);

        if (!SetMoveTarget(vBackUpPos))
        {
            Debug.LogError("Failed to get a next move");
        }
    }

    public Vector2Int GetRandomInList(List<HeroTile> inTiles)
    {
        return inTiles[(int)Random.Range(0, (int)(inTiles.Count - 1))].GetPosition();
    }

    public Vector2Int GetNearestInList(Vector2Int inTarget, List<HeroTile> inTiles)
    {
        int iNearest = int.MaxValue;
        Vector2Int vReturn = inTiles[0].GetPosition();

        foreach (HeroTile aTile in lMoveRange)
        {
            if (arrGrid.GetGridObject(aTile.GetPosition()).GetMoveable().Count == 0)
            {
                //float fDistance = Vector2Int.Distance(vAI_TargetLastPos, aTile.GetPosition());
                int iDistance = pPathFinding.FindPath(vAI_TargetLastPos, aTile.GetPosition(), sMoveableKey.Item1).Count;
                if (iDistance < iNearest)
                {
                    iNearest = iDistance;
                    vReturn = aTile.GetPosition();
                }
            }
        }
        return vReturn;
    }

    private Vector2Int GetFarthestInList(Vector2Int vStartPos, List<HeroTile> inTiles)
    {
        int iFarthest = 0;
        Vector2Int vReturn = inTiles[0].GetPosition();
        foreach (HeroTile aTile in lMoveRange)
        {
            if (arrGrid.GetGridObject(aTile.GetPosition()).GetMoveable().Count == 0)
            {
                //float fDistance = Vector2Int.Distance(vAI_TargetLastPos, aTile.GetPosition());
                int iDistance = pPathFinding.FindPath(vAI_TargetLastPos, aTile.GetPosition(), sMoveableKey.Item1).Count;
                if (iDistance > iFarthest)
                {
                    iFarthest = iDistance;
                    vReturn = aTile.GetPosition();
                }
            }
        }
        return vReturn;
    }


    public void MoveRandomNearHome()
    {
        Vector2Int vTestPos;
        Vector2Int vBackUpPos = Vector2Int.zero;


        vBackUpPos = GetRandomInList(lMoveRange);


        for (int iGiveUp = 0; iGiveUp < 20; iGiveUp++)
        {
            vTestPos = new Vector2Int(vHomePos.x + Random.Range(-sStats.iMoveSquares/2, sStats.iMoveSquares/2), vHomePos.y + Random.Range(-sStats.iMoveSquares/2, sStats.iMoveSquares/2));
            if (arrGrid.IsValid(vTestPos))
            {
                if (arrGrid.GetGridObject(vTestPos).GetMoveable().Count == 0)
                {
                    if (pPathFinding.FindPath(GetPosXY(), vTestPos, sMoveableKey.Item1) != null)
                    {
                        vBackUpPos = vTestPos;
                        break;
                    }
                }
            }
        }

        if (!SetMoveTarget(vBackUpPos))
        {
            Debug.LogError("Failed to get a next move");
        }
        
    }

    public void MoveRandomInRoom()
    {
        eRoomIDs eMyRoomID = arrGrid.GetGridObject(GetPosXY()).GetRoomID();
        Vector2Int vBackUpPos = GetRandomInList(lMoveRange);

        for (int iGiveUp = 0; iGiveUp < 20; iGiveUp++)
        {
            Vector2Int vTestPos = GetRandomInList(lMoveRange);
            if(arrGrid.GetGridObject(vTestPos).GetRoomID() == eMyRoomID)
            {
                vBackUpPos = vTestPos;
                break;
            }
        }

        if (!SetMoveTarget(vBackUpPos))
        {
            Debug.LogError("Failed to get a next move");
        }

    }

    public (eMoveableType, int) GetMoveableKey()
    {
        return sMoveableKey;
    }


    private void TryToWakeUp()
    {
        List<HeroTile> lSenseRange = pPathFinding.FindTileInRange(GetPosXY(), (int)(sStats.iMoveSquares * 1.5f));
        foreach (HeroTile aTile in lSenseRange)
        {
            if(aTile.GetMoveable().Count > 0)
            {
                if(aTile.GetMoveable()[0].Item1 != sMoveableKey.Item1)
                {
                    eAI_State = eAI_StateType.Roaming;
                    return;
                }
            }
        }

        bMoveReady = false;
        bActReady = false;
    }

    public void UpdateSkillStates()
    {
        foreach(EquipmentData aSkill in lEquiped)
        {
            switch (aSkill.eEquipmentRef)
            {
                case eEquipmentRefType.Legs_PlayerBoots:
                    aSkill.eSkillReadyState = bMoveReady? eSkillReadyStateType.MoveReady:eSkillReadyStateType.Disabled;
                    break;
                case eEquipmentRefType.Core_LockPick:
                    bool bAnyDoor = HeroMap.GetGridObject(GetPosXY().x,GetPosXY().y).GetNavTileIndex() == eNavTiles.DoorNorthClosed || HeroMap.GetGridObject(GetPosXY().x, GetPosXY().y).GetNavTileIndex() == eNavTiles.DoorSouthClosed;
                    aSkill.eSkillReadyState = (bMoveReady && bAnyDoor) ? eSkillReadyStateType.MoveReady : eSkillReadyStateType.Disabled;
                    break;

                case eEquipmentRefType.Weapon_BattleAxe:
                case eEquipmentRefType.Weapon_Broadsword:
                case eEquipmentRefType.Weapon_Crossbow:
                case eEquipmentRefType.Weapon_Dagger:
                case eEquipmentRefType.Weapon_LongSword:
                case eEquipmentRefType.Weapon_ShortSword:
                case eEquipmentRefType.Weapon_Staff:
                    aSkill.eSkillReadyState = !(!bActReady || lActRange.Count == 0 )? eSkillReadyStateType.ActReady : eSkillReadyStateType.Disabled;
                    break;

                case eEquipmentRefType.Body_Barbaian:
                case eEquipmentRefType.Body_Dwarf:
                case eEquipmentRefType.Body_Elf:
                case eEquipmentRefType.Body_Wizard:
                    aSkill.eSkillReadyState = eSkillReadyStateType.Passive;
                    break;

                case eEquipmentRefType.Core_PassageMap:
                case eEquipmentRefType.Core_TrapMap:
                case eEquipmentRefType.Core_TreasureMap:
                    aSkill.eSkillReadyState = bActReady? eSkillReadyStateType.ActReady : eSkillReadyStateType.Disabled;
                    break;
                case eEquipmentRefType.Kit_TrapTools:
                    bool bAnyTrap =
                        HeroMap.GetGridObject(GetPosXY().x, GetPosXY().y).GetNavTileIndex() == eNavTiles.Pit ||
                        HeroMap.GetGridObject(GetPosXY().x, GetPosXY().y).GetNavTileIndex() == eNavTiles.Rocks ||
                        HeroMap.GetGridObject(GetPosXY().x, GetPosXY().y).GetNavTileIndex() == eNavTiles.Spear;
                       
                    aSkill.eSkillReadyState = (bMoveReady && bAnyTrap) ? eSkillReadyStateType.MoveReady : eSkillReadyStateType.Disabled;
                    break;
            }
        }
        OnSkillUpdate?.Invoke(lEquiped);
    }
    public void GameCombatUpdate()
    {
        switch(ePlayerState)
        {
            case ePlayerStateType.Idle:
                return;
            case ePlayerStateType.TurnActive:
                if(eAI_State == eAI_StateType.Sleeping)
                {
                    TryToWakeUp();
                }
                switch( eTeam)
                {
                    case eTeamType.EnemyTeam:
                        if (bMoveReady)
                        {
                            ePlayerState = ePlayerStateType.WaitingForMoveTarget;
                            return;
                        }
                        if (bActReady)
                        {
                            ePlayerState = ePlayerStateType.WaitingForActTarget;
                            return;
                        }
                        ePlayerState = ePlayerStateType.Idle;
                        break;
                    case eTeamType.PlayerTeam:
                        if (!bMoveReady && (!bActReady || lActRange.Count == 0))
                        {

                            UpdateSkillStates();
                            ePlayerState = ePlayerStateType.Idle;
                        }
                        if (bMoveReady && AI_Move != WaitForPlayerMove)
                        {
                            UpdateSkillStates();
                            ePlayerState = ePlayerStateType.WaitingForMoveTarget;
                            return;
                        }
                        break;
                }

                break;
            case ePlayerStateType.WaitingForActTarget:
                
               // cDiceManager.RollForAttack(sStats.iAttackDice, eAttackFace, sStats.iDefenceDice, eDefenseFace);
                bActReady = false;
                ePlayerState = ePlayerStateType.TurnActive;
                break;
            case ePlayerStateType.MovingToTarget:
                if(Vector2.Distance(vPosition, vToOfMove) < fMoveSpeed/10f)
                {
                    arrGrid.GetGridObject(vToOfMove).HasEntered(sMoveableKey);
                    arrGrid.GetGridObject(vFromOfMove).HasLeft(sMoveableKey);
                    lActRange.Clear();
                    if (lCurrentPath.Count > 1)
                    {
                        //vMoveTarget = lCurrentPath[0].GetPosition();
                        lMoveRange = lCurrentPath;
                        vPosition = lCurrentPath[0].GetPosition();
                        vFromOfMove = GetPosXY();
                        lCurrentPath.RemoveAt(0);
                        vToOfMove = lCurrentPath[0].GetPosition();
                    }
                    else
                    { 
                        ePlayerState = ePlayerStateType.AtTarget;
                        vPosition = vMoveTarget;
                        lMoveRange.Clear();
                        bFirstMove = false;
                    }
                    if (eTeam == eTeamType.PlayerTeam)
                    {
                        UpdateScanRange();
                        UpdateSkillStates();
                    }
                }
                else
                {
                    vVelocity = (vToOfMove - vPosition); 
                    vPosition += vVelocity * fMoveSpeed * Time.deltaTime;

                }
                break;
            case ePlayerStateType.AtTarget:
                ePlayerState = ePlayerStateType.TurnActive;
                bMoveReady = false;
                if (iMovesRemaining > 0)
                {
                    //if (eTeam == eTeamType.PlayerTeam)
                    if(AI_Move == WaitForPlayerMove)
                    {
                        ResetMoveRange();
                        bMoveReady = true;
                    }
                }
                if(bActReady)
                {
                    ResetActRange();
                }
                UpdateSkillStates();

                break;
            case ePlayerStateType.ActComplete:
                ePlayerState = ePlayerStateType.TurnActive;
                bActReady = false;
                UpdateSkillStates();
                break;
            case ePlayerStateType.WaitingForMoveTarget:
                AI_Move();
                break;
            case ePlayerStateType.Reset:
                bMoveReady = true;
                bActReady = true;

                if (eTeam == eTeamType.PlayerTeam)
                {
                    bFirstMove = true;
                    iMovesRemaining = DiceManager.RollForMove();
                    ResetMoveRange();
                    ResetActRange();

                    UpdateScanRange();
                    if (lMoveRange.Count == 0)
                    {
                        bMoveReady = false;
                    }
                    else
                    {
                        switch(eAI_State)
                        {
                            case eAI_StateType.FollowTarget:
                                AI_Move = MoveFollowTarget;
                                break;
                        }
                    }
                    
                }

                if (eTeam == eTeamType.EnemyTeam)
                {
                    lMoveRange = pPathFinding.FindPathWithRange(GetPosXY(), sStats.iMoveSquares, sMoveableKey.Item1);
                    if (lMoveRange.Count == 0)
                    {
                        bMoveReady = false;
                    }
                    else
                    {
                        switch (eAI_State)
                        {
                            case eAI_StateType.FollowTarget:
                                AI_Move = MoveFollowTarget;
                                break;
                        }
                    }
                }

                ePlayerState = ePlayerStateType.TurnActive;
                UpdateSkillStates();
                break;


        }
    }

    private void ResetMoveRange()
    {
        lMoveRange.Clear();
        List<HeroTile> tlMoveRange = pPathFinding.FindPathWithRange(GetPosXY(), iMovesRemaining, sMoveableKey.Item1);
        foreach (HeroTile aTile in tlMoveRange)
        {
            if (aTile.bVisible)
            {
                lMoveRange.Add(aTile);
            }
        }
    }

    private void ResetActRange()
    {
        lActRange.Clear();
        List<HeroTile> tActRange = pPathFinding.FindPathWithRange(GetPosXY(), iActRange);
        foreach (HeroTile aTile in tActRange)
        {
            if (aTile.bVisible
                && aTile.GetMoveable() != null
                && aTile.GetMoveable().Count == 1
                && aTile.GetMoveable()[0].Item1 != eMoveableType.Furniture
                && aTile.GetMoveable()[0].Item1 != sMoveableKey.Item1)
            {
                lActRange.Add(aTile);
            }
        }

    }
    public float GetSpeedFromDistance()
    {
        return fMoveSpeed;
    }

    public override string ToString()
    {
        //return iSpriteIndex.ToString();
        return sMoveableKey.ToString();
    }
}
