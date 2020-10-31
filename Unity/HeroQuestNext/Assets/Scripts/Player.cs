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
    WaitingForTarget,
    MovingToTarget,
    AtTarget,
}


public class Player : MonoBehaviour, ICharacter
//public class Player :  ICharacter
{
    Vector2 vPosition;
    Vector2 vMoveTarget;
    Vector2 vVelocity;
    eMoveableType eTeam;
    ePlayerStateType ePlayerState;
    //Texture2D tIcon;
    private int iRefIndex;
    PathFinding<HeroTile> pPathFinding;
    public List<HeroTile> lCurrentPath;
    public List<HeroTile> lMoveRange;
    //PathFinding<TGridObject> where TGridObject : IPathable<TGridObject> pPathFinding;
    private BlackBocksGrid<HeroTile> arrGrid;
    private bool bActiveTurn { set; get; }
    private int iMoveRange = 12;
    private float fMoveSpeed = 2f;

    (eMoveableType, int) sMoveableKey;

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
        iMoveRange = inMoveRange;
    }

    public bool SetMoveTarget(Vector2 inMoveTarget)
    {
        List<HeroTile> lPath;
        if(PathFinding<HeroTile>.Instance.GetGrid().IsValid(new Vector2Int((int)inMoveTarget.x, (int)inMoveTarget.y)))
        {
            lPath = PathFinding<HeroTile>.Instance.FindPath(new Vector2Int((int)vPosition.x, (int)vPosition.y), new Vector2Int((int)inMoveTarget.x, (int)inMoveTarget.y));
            if (lPath != null)
            {
                lCurrentPath = new List<HeroTile>();
                lCurrentPath = lPath;
                return true;
            }
        }
        return false;

    }
    public Vector2Int GetPosXY()
    {
        return new Vector2Int((int)vPosition.x, (int)vPosition.y);
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

    public Player(
        (eMoveableType, int) inMoveableKey,
        Vector3Int inStartPos,
        int inMoveRange = 12
        )
    {
        sMoveableKey = inMoveableKey;
        vPosition = new Vector2Int(inStartPos.x,inStartPos.y);
        iRefIndex = inStartPos.z;
        bActiveTurn = false;
        iMoveRange = inMoveRange;
        ePlayerState = ePlayerStateType.Idle;
        //tIcon = inIcon;
        
        
    }

    public void SetGrid(BlackBocksGrid<HeroTile> inGrid)
    {
        this.arrGrid = inGrid;
    }

    public void MoveRandomInRange()
    {
        List<HeroTile> lMoves = PathFinding<HeroTile>.Instance.FindPathWithRange(new Vector2Int((int)vPosition.x, (int)vPosition.y), iMoveRange);
        lMoveRange = new List<HeroTile>();
        lMoveRange = lMoves;
        vMoveTarget = lMoves[(int)Random.Range(1, (int)(lMoves.Count - 1))].GetPosition();
        if(SetMoveTarget(vMoveTarget))
        {
            ePlayerState = ePlayerStateType.MovingToTarget;
           
        }
    }

    public (eMoveableType, int) GetMoveableKey()
    {
        return sMoveableKey;
    }

    public void GameCombatUpdate()
    {
        if(pPathFinding == null)
        {
            pPathFinding = PathFinding<HeroTile>.Instance;
        }
        switch(ePlayerState)
        {
            case ePlayerStateType.Idle:
                break;
            case ePlayerStateType.MovingToTarget:
                if(Vector2.Distance(vPosition, lCurrentPath[0].GetPosition()) < fMoveSpeed/10f)
                {
                    if (lCurrentPath.Count > 1)
                    {
                        //vMoveTarget = lCurrentPath[0].GetPosition();
                        lMoveRange = lCurrentPath;
                        vPosition = lCurrentPath[0].GetPosition();
                        lCurrentPath.RemoveAt(0);
                    }
                    else
                    { 
                        ePlayerState = ePlayerStateType.AtTarget;
                        vPosition = vMoveTarget;
                    }
                }
                else
                {
                    vVelocity = (lCurrentPath[0].GetPosition() - vPosition); 
                    vPosition = vPosition + vVelocity * fMoveSpeed * Time.deltaTime;

                }
                break;
            case ePlayerStateType.AtTarget:
                pPathFinding.GetGrid().GetGridObject(vMoveTarget).HasEntered(sMoveableKey);
                ePlayerState = ePlayerStateType.Idle;
                break;
            case ePlayerStateType.WaitingForTarget:
                MoveRandomInRange();
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
