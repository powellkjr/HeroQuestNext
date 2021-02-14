using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Enemy : Player
{
    public Player_Enemy(
    (eMoveableType, int) inMoveableKey,
    Vector3Int inStartPos
    ) : base (inMoveableKey, inStartPos)
    {
        eTeam = eTeamType.EnemyTeam;
        eEnemyTeam = eTeamType.PlayerTeam;
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
        strPlayerName = NameManger.GetNameByGroupType((eNameGroupType)iRefIndex);
        EquipmentManger.EquipItem_Static(EquipmentManger.GetEquipmentData_Static(ePlayerClass), out sStats, lEquiped, lStored);
        EquipmentManger.EquipItem_Static(EquipmentManger.GetStartingEquipmentFromBody_Static(ePlayerClass), out sStats, lEquiped, lStored);
        AI_Move = GetMyMoveFromIndex();
        eAI_State = eAI_StateType.Sleeping;
    }

    private void TryToWakeUp()
    {
        List<HeroTile> lSenseRange = pPathFinding.FindTileInRange(GetPosXY(), (int)(sStats.iMoveSquares * 1.5f));
        foreach (HeroTile aTile in lSenseRange)
        {
            if (aTile.GetMoveable().Count > 0)
            {
                if (aTile.GetMoveable()[0].Item1 == eMoveableType.Player)
                {
                    eAI_State = eAI_StateType.Roaming;
                    return;
                }
            }
        }

        bMoveReady = false;
        bActReady = false;
    }

    protected override void Reset()
    {
        base.Reset();
     
        switch (eAI_State)
        {
            case eAI_StateType.FollowTarget:
                AI_Move = MoveFollowTarget;
                break;
        }
        
    }
    protected override void TurnActive()
    {
        if (eAI_State == eAI_StateType.Sleeping)
        {
            TryToWakeUp();
        }

        base.TurnActive();
    }
}
