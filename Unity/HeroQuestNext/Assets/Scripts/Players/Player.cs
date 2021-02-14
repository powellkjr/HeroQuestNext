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
    MovingToTarget,
    MoveComplete,
    WaitingForActTarget,
    DoingAct,
    ActComplete,
    TurnComplete,
    TurnActive,
    Reset,
}

public enum eAI_StateType
{
    Sleeping,
    Roaming,
    FollowTargetTeam,
    FollowTargetEnemy,
    FleeTargetTeam,
    FleeTargetEnemy,
    Manual,
}







public class Player : MonoBehaviour, ICharacter
//public class Player :  ICharacter
{
    protected Vector2 vPosition;
    protected Vector2 vStartOfMove;
    protected Vector2Int vFromOfMove;
    protected Vector2Int vToOfMove;
    protected Vector2Int vMoveTarget;
    protected Vector2 vVelocity;
    protected Vector2Int vHomePos;

    protected eTeamType eTeam;
    protected eTeamType eEnemyTeam;
    protected ePlayerStateType ePlayerState;
    protected eEquipmentRefType ePlayerClass;


    //Texture2D tIcon;
    protected int iRefIndex;
    protected PathFinding<HeroTile> pPathFinding;
    protected List<HeroTile> lCurrentPath;
    protected List<HeroTile> lMoveRange;
    protected List<HeroTile> lScanRange;
    protected List<HeroTile> lActRange;
    protected int iActRange = 1;
    //PathFinding<TGridObject> where TGridObject : IPathable<TGridObject> pPathFinding;
    protected BlackBocksGrid<HeroTile> arrGrid;
    
    protected bool bActiveTurn { set; get; }
    //private int iMoveRange = 12;
    protected float fMoveSpeed = 4f;
    protected bool bMoveReady;
    protected bool bFirstMove;
    protected bool bActReady;
    protected EquipmentData sStats;
    protected List<EquipmentData> lEquiped;
    protected List<EquipmentData> lStored;
    protected eAI_StateType eAI_State;
    protected int iMovesRemaining;
    protected (eMoveableType, int) sMoveableKey;
    protected (eMoveableType, int) sAI_TargetKey;
    protected List<(eMoveableType, int)> sAI_TargetList;
    protected eDiceFace eAttackFace = eDiceFace.Skull;
    protected eDiceFace eDefendFace;
    protected Vector2Int vAI_TargetLastPos;
    protected string strPlayerName;

    public delegate void MyMove();
    protected MyMove AI_Move;

    delegate void MyAct();

    //public event EventHandler OnSkillUpdate;
    public Action<List<EquipmentData>> OnSkillUpdate;
    public Action<List<HeroTile>, eSpritePages, int> ActiveSkillRange;
    public eEquipmentRefType ActiveSkillRefType;
    public Action HideUserRange;
    public Action<(eMoveableType, int), HeroTile, Action> OnPlayerTryCombat;

    public List<HeroTile> GetActRange()
    {
        return lActRange;
    }    

    public List<HeroTile> GetCurrentPath()
    {
        return lCurrentPath;
    }

    public List<HeroTile> GetMoveRange()
    {
        return lMoveRange;
    }

    public List<HeroTile> GetScanRange()
    {
        return lScanRange;
    }
    protected void OnSkillMouseOver()
    {
        Debug.LogWarning("MousedOver");
    }

    public string GetPlayerName()
    {
        return strPlayerName;
    }
    protected void OnSkillActivate()
    {
        Debug.LogWarning("SkillActivated");
    }
    protected void OnSkillUpdate_Local(List<EquipmentData> inEquipmentData)
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

    protected MyMove GetMyMoveFromIndex()
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
    protected virtual void TurnActive()
    {
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
    }

    protected virtual void Reset()
    {
        bMoveReady = true;
        bActReady = true;
        bFirstMove = true;
        iMovesRemaining = GetMovesRemaining();
        ResetMoveRange();
        if (lMoveRange.Count == 0)
        {
            bMoveReady = false;
        }

        ResetActRange();
        UpdateTargetList();
        if(sAI_TargetKey.Item1 == eMoveableType.None)
        {
            GetNextAI_State();
        }

        if(sAI_TargetList.Contains(sAI_TargetKey))
        {
            List<HeroTile> lTestMoveList = lMoveRange;
            Vector2Int vTestpos = GetNearestInList(vAI_TargetLastPos, lTestMoveList);
            do
            {
                vTestpos = GetNearestInList(vAI_TargetLastPos, lTestMoveList);

            } while (!SetMoveTarget(vTestpos);
            
        }


        ePlayerState = ePlayerStateType.TurnActive;
    }

    protected void GetNextAI_State()
    {

        eAI_State = eAI_StateType
        
    }

    protected virtual int GetMovesRemaining()
    {
        return sStats.iMoveSquares;
    }
    protected virtual void MoveComplete()
    {
        ePlayerState = ePlayerStateType.TurnActive;
        bMoveReady = false;
        
        if (bActReady)
        {
            ResetActRange();
        }
        
    }
    protected virtual void WaitingForActTarget()
    {
        bActReady = false;
        ePlayerState = ePlayerStateType.TurnActive;
    }
    protected virtual void DoingAct()
    {

    }
    protected virtual void ActComplete()
    {
        ePlayerState = ePlayerStateType.TurnActive;
        bActReady = false;
    }

    protected virtual void MovingToTarget()
    {
        if (Vector2.Distance(vPosition, vToOfMove) < fMoveSpeed / 10f)
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
                ePlayerState = ePlayerStateType.MoveComplete;
                vPosition = vMoveTarget;
                lMoveRange.Clear();
                bFirstMove = false;
            }
        }
        else
        {
            vVelocity = (vToOfMove - vPosition);
            vPosition += vVelocity * fMoveSpeed * Time.deltaTime;

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

        OnPlayerTryCombat += GameCombatHandler.Instance.Player_OnPlayerTryCombat;
        UICombatLog.AddRecord(eCombatRecordType.Join, strPlayerName);

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
        ePlayerState = ePlayerStateType.MoveComplete;
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

    protected Vector2Int GetFarthestInList(Vector2Int vStartPos, List<HeroTile> inTiles)
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


    

    public void UpdateTargetList()
    {
        sAI_TargetList.Clear();
        List<HeroTile> lSenseRange = pPathFinding.FindTileInRange(GetPosXY(), (int)(sStats.iMoveSquares * 1.5f));
        foreach (HeroTile aTile in lSenseRange)
        {
            if (aTile.GetMoveable().Count > 0)
            {
                if (aTile.GetMoveable()[0].Item1 != eMoveableType.Furniture)
                {
                    sAI_TargetList.Add( aTile.GetMoveable()[0]);
                    if( aTile.GetMoveable()[0] == sAI_TargetKey)
                    {
                        vAI_TargetLastPos = aTile.GetPosition();
                    }
                }
            }
        }
    }

    
    

    
    public virtual void GameCombatUpdate()
    {
        {
            switch (ePlayerState)
            {
                case ePlayerStateType.Idle:
                    return;
                case ePlayerStateType.TurnActive:
                    TurnActive();
                    break;
                case ePlayerStateType.WaitingForActTarget:
                    WaitingForActTarget();
                    break;
                case ePlayerStateType.DoingAct:
                    DoingAct();
                    break;
                case ePlayerStateType.ActComplete:
                    ActComplete();
                    break;
                case ePlayerStateType.WaitingForMoveTarget:
                    AI_Move();
                    break;
                case ePlayerStateType.MovingToTarget:
                    MovingToTarget();
                    break;
                case ePlayerStateType.MoveComplete:
                    MoveComplete();
                    break;
                case ePlayerStateType.Reset:
                    Reset();
                    break;
            }
        }
    }

    protected void ResetMoveRange()
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

    protected void ResetActRange()
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
