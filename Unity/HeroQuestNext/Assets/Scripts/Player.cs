using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
public enum eEquipmentType
{
    Core,
    Head,
    Gloves,     
    Weapon,
    Total
}
public struct sEquipmentData
{
    public int iRefIndex;
    public eEquipmentType eSlot;
    public int iMoveSquares;
    public int iAttackDice;
    public int iDefenceDice;
    public eDiceFace eAttackFace;
    public eDiceFace eDefenceFace;
    public int iHitPoints;
    public int iMindPoints;

    public static sEquipmentData operator +(sEquipmentData sLeftSide, sEquipmentData sRightSide)
    {
        return new sEquipmentData
        {
            iMoveSquares = sLeftSide.iMoveSquares + sRightSide.iMoveSquares,
            iAttackDice = sLeftSide.iAttackDice + sRightSide.iAttackDice,
            iDefenceDice = sLeftSide.iMoveSquares + sRightSide.iDefenceDice,
            iHitPoints = sLeftSide.iHitPoints + sRightSide.iHitPoints,
            iMindPoints = sLeftSide.iMoveSquares + sRightSide.iMindPoints,
        };
    }
}






public class Player : MonoBehaviour, ICharacter
//public class Player :  ICharacter
{
    private static List<sEquipmentData> lPlayerEqipment = new List<sEquipmentData> {
        new sEquipmentData
        {
            iRefIndex = (int)ePlayers.None,
            eSlot = eEquipmentType.Core,
            iAttackDice = 1,
            iDefenceDice = 1,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.White,
            iHitPoints= 1,
            iMindPoints=1,
            iMoveSquares = 0
        },

        new sEquipmentData
        {
            iRefIndex = (int)ePlayers.Barbarian,
            eSlot = eEquipmentType.Core,
            iAttackDice = 3,
            iDefenceDice = 2,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.White,
            iHitPoints= 8,
            iMindPoints=2,
            iMoveSquares = 0
            
        },

        new sEquipmentData
        {
            iRefIndex = (int)ePlayers.Dwarf,
            eSlot = eEquipmentType.Core,
            iAttackDice = 2,
            iDefenceDice = 2,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.White,
            iHitPoints= 7,
            iMindPoints = 3,
            iMoveSquares = 0
        },

        new sEquipmentData
        {
            iRefIndex = (int)ePlayers.Elf,
            eSlot = eEquipmentType.Core,
            iAttackDice = 2,
            iDefenceDice = 2,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.White,
            iHitPoints= 6,
            iMindPoints=4,
            iMoveSquares = 0
        },

        new sEquipmentData
        {
            iRefIndex = (int)ePlayers.Wizard,
            eSlot = eEquipmentType.Core,
            iAttackDice = 1,
            iDefenceDice = 2,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.White,
            iHitPoints= 4,
            iMindPoints= 6,
            iMoveSquares = 12
        },
    };
    private static List<sEquipmentData> lMonsterEqipment = new List<sEquipmentData> {
        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.None,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 6,
            iAttackDice = 1,
            iDefenceDice = 1,
            iMindPoints=1,
            iHitPoints= 1,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },

        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.Skeleton,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 6,
            iAttackDice = 2,
            iDefenceDice = 2,
            iHitPoints= 1,
            iMindPoints=0,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },

        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.Zombie,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 5,
            iAttackDice = 2,
            iDefenceDice = 3,
            iHitPoints= 1,
            iMindPoints=0,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },

        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.Mummy,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 4,
            iAttackDice = 3,
            iDefenceDice = 4,
            iHitPoints= 2,
            iMindPoints=0,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },

        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.Goblin,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 10,
            iAttackDice = 2,
            iDefenceDice = 1,
            iHitPoints= 1,
            iMindPoints=1,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },

        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.Orc,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 8,
            iAttackDice = 3,
            iDefenceDice = 2,
            iHitPoints= 1,
            iMindPoints=2,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },

        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.Fimir,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 6,
            iAttackDice = 3,
            iDefenceDice = 3,
            iHitPoints= 2,
            iMindPoints=3,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },
        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.ChaosWarrior,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 7,
            iAttackDice = 4,
            iDefenceDice = 2,
            iMindPoints=2,
            iHitPoints= 8,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },

        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.Gargoyle,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 6,
            iAttackDice = 4,
            iDefenceDice = 5,
            iHitPoints= 3,
            iMindPoints=4,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },

        new sEquipmentData
        {
            iRefIndex = (int)eMonsters.ChaosScorcer,
            eSlot = eEquipmentType.Core,
            iMoveSquares = 0,
            iAttackDice = 0,
            iDefenceDice = 0,
            iMindPoints=0,
            iHitPoints= 0,
            eAttackFace= eDiceFace.Skull,
            eDefenceFace= eDiceFace.Black,
        },
    };

    private Vector2 vPosition;
    private Vector2 vStartOfMove;
    private Vector2Int vFromOfMove;
    private Vector2Int vToOfMove;
    private Vector2Int vMoveTarget;
    private Vector2 vVelocity;
    private Vector2Int vHomePos;

    eDiceFace eAttackFace;
    eDiceFace eDefenseFace;
    eTeamType eTeam;
    ePlayerStateType ePlayerState;


    //Texture2D tIcon;
    private int iRefIndex;
    private PathFinding<HeroTile> pPathFinding;
    public List<HeroTile> lCurrentPath;
    public List<HeroTile> lMoveRange;
    //PathFinding<TGridObject> where TGridObject : IPathable<TGridObject> pPathFinding;
    private BlackBocksGrid<HeroTile> arrGrid;
    private DiceManager cDiceManager;
    private bool bActiveTurn { set; get; }
    //private int iMoveRange = 12;
    private float fMoveSpeed = 4f;
    private bool bMoveReady;
    private bool bActReady;
    private sEquipmentData sStats;
    private List<sEquipmentData> lEquiped;
    private List<sEquipmentData> lStored;
    private eAI_StateType eAI_State;
    (eMoveableType, int) sMoveableKey;
    (eMoveableType, int) sAI_TargetKey;
    Vector2Int vAI_TargetLastPos;

    delegate void MyMove();
    MyMove AI_Move;

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


    public void EquipItem(sEquipmentData inEquipmentData, eEquipmentType inEquipmentType)
    {
        for (int i =0; i < lEquiped.Count; i++)
        {
            sEquipmentData aEquipment = lEquiped[i];
            if(aEquipment.eSlot == inEquipmentType)
            {
                lStored.Add(aEquipment);
                lEquiped.RemoveAt(i);
            }
        }
        lEquiped.Add(inEquipmentData);
        UpdateStats();
    }
    public void UpdateStats()
    {
        sStats.iMoveSquares = 0;
        sStats.iAttackDice = 0;
        sStats.iDefenceDice = 0;
        sStats.iHitPoints = 0;
        sStats.iMindPoints = 0;

        foreach (sEquipmentData aEquipment in lEquiped)
        {
            sStats += aEquipment;
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
        //iMoveRange = inMoveRange;
        ePlayerState = ePlayerStateType.Idle;
        pPathFinding = PathFinding<HeroTile>.Instance;
        arrGrid = HeroMap.Instance.GetGrid();
        cDiceManager = DiceManager.Instance;
        vHomePos = new Vector2Int(inStartPos.x, inStartPos.y);
        arrGrid.GetGridObject(inStartPos.x, inStartPos.y).HasEntered(sMoveableKey);

        lEquiped = new List<sEquipmentData>();
        lStored = new List<sEquipmentData>();

        switch (inMoveableKey.Item1)
        {
            case eMoveableType.Player:
                eTeam = eTeamType.PlayerTeam;
                if (sMoveableKey.Item2 == 0)
                {
                    AI_Move = WaitForPlayerMove;
                    eAI_State = eAI_StateType.Manual;
                }
                else
                {
                    AI_Move = MoveFarthestInRange;
                    eAI_State = eAI_StateType.FollowTarget;
                    sAI_TargetKey = (eMoveableType.Player, 0);
                }
                EquipItem(lPlayerEqipment[iRefIndex], eEquipmentType.Core);
                break;
            case eMoveableType.Enemy:
                eTeam = eTeamType.EnemyTeam;
                EquipItem(lMonsterEqipment[iRefIndex], eEquipmentType.Core);
                AI_Move = GetMyMoveFromIndex();
                eAI_State = eAI_StateType.Sleeping;
                break;
        };
        
    }

    public void WaitForPlayerMove()
    {
    }

    public void MoveRandomInRange()
    {
        if (lMoveRange.Count > 0)
        {
            vMoveTarget = lMoveRange[(int)Random.Range(1, (int)(lMoveRange.Count - 1))].GetPosition();
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
                if(bMoveReady)
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
            case ePlayerStateType.WaitingForActTarget:
                
                cDiceManager.RollForAttack(sStats.iAttackDice, eAttackFace, sStats.iDefenceDice, eDefenseFace);
                bActReady = false;
                ePlayerState = ePlayerStateType.TurnActive;
                break;
            case ePlayerStateType.MovingToTarget:
                if(Vector2.Distance(vPosition, vToOfMove) < fMoveSpeed/10f)
                {
                    arrGrid.GetGridObject(vToOfMove).HasEntered(sMoveableKey);
                    arrGrid.GetGridObject(vFromOfMove).HasLeft(sMoveableKey);                        
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
                    }
                }
                else
                {
                    vVelocity = (vToOfMove - vPosition); 
                    vPosition = vPosition + vVelocity * fMoveSpeed * Time.deltaTime;

                }
                break;
            case ePlayerStateType.AtTarget:
                ePlayerState = ePlayerStateType.TurnActive;
                bMoveReady = false;
                break;
            case ePlayerStateType.WaitingForMoveTarget:
                AI_Move();
                break;
            case ePlayerStateType.Reset:
                bMoveReady = true;
                bActReady = true;

                if (eTeam == eTeamType.PlayerTeam)
                {
                    lMoveRange = pPathFinding.FindPathWithRange(GetPosXY(), cDiceManager.RollForMove(), sMoveableKey.Item1);
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
                break;


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
