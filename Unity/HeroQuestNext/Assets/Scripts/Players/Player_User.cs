using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_User : Player_Agent
{ 
    public Player_User(
    (eMoveableType, int) inMoveableKey,
    Vector3Int inStartPos
    ) : base(inMoveableKey, inStartPos)
    {
        
            AI_Move = WaitForPlayerMove;
            eAI_State = eAI_StateType.Manual;
            PlayerWidgetController.SetPlayerWidgetText_Static(lEquiped);
            UI_CharacterSheet.InitCharacterSheet(strPlayerName, ((ePlayers)iRefIndex).ToString(), sStats.iHitPoints, sStats.iMindPoints);
            UpdateScanRange();
            OnSkillUpdate += OnSkillUpdate_Local;
            UpdateSkillStates();
            GameCombatHandler.Instance.OnPlayerUsedSkill += GameCombatHandler_OnPlayerUsedSkill;
            OnSkillUpdate += PlayerWidgetController.SetPlayerWidgetText_Static;

    }





    protected void GameCombatHandler_OnPlayerUsedSkill((eMoveableType, int) inCaller, HeroTile inCursorPosition)
    {
        if(inCaller == sMoveableKey)
        {
            //the user, who is me, tried to do a thing
            Debug.LogWarning("Player Activated current skill: " + ActiveSkillRefType);
            switch(ActiveSkillRefType)
            {
                case eEquipmentRefType.Legs_PlayerBoots:
                    if (inCursorPosition.GetPosition() == GetPosXY())
                    {
                        //you said you want to move to where you are
                        SkipPlayerMove();
                        return;
                    }
                     
                    if (!lMoveRange.Contains(inCursorPosition))
                    {
                        //you tried to move off the map or somewhere dumb. ignore
                        ActiveSkillRefType = eEquipmentRefType.Idle;
                        return;
                    }
                     
                    //you are trying to move that a space that is valid
                    SetMoveTarget(inCursorPosition.GetPosition());
                    ePlayerState = ePlayerStateType.MovingToTarget;
                        
                    

                    break;
                case eEquipmentRefType.Weapon_BattleAxe:
                case eEquipmentRefType.Weapon_Broadsword:
                case eEquipmentRefType.Weapon_Crossbow:
                case eEquipmentRefType.Weapon_Dagger:
                case eEquipmentRefType.Weapon_LongSword:
                case eEquipmentRefType.Weapon_ShortSword:
                case eEquipmentRefType.Weapon_Staff:
                    if (inCursorPosition.GetPosition() == GetPosXY())
                    {
                        //you said you want to attack yourself. rude
                        SkipPlayerAct();
                        return;
                    }
                    if (!lActRange.Contains(inCursorPosition))
                    {
                        //you clicked somewhere dumb, ignore
                        ActiveSkillRefType = eEquipmentRefType.Idle;
                        return;
                    }
                    
                    ePlayerState = ePlayerStateType.DoingAct;
                    OnPlayerTryCombat?.Invoke(sMoveableKey, inCursorPosition,() => ePlayerState = ePlayerStateType.ActComplete);


            
                    break;
            }
        }
    }


    public override void GameCombatUpdate()
    {
        switch (ePlayerState)
        {
            case ePlayerStateType.Idle:
                return;
            case ePlayerStateType.TurnActive:
                
                if (!bMoveReady && (!bActReady || lActRange.Count == 0))
                {
                    bMoveReady = false;
                    bActReady = false;
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
            case ePlayerStateType.WaitingForActTarget:

                // cDiceManager.RollForAttack(sStats.iAttackDice, eAttackFace, sStats.iDefenceDice, eDefenseFace);
                bActReady = false;
                ePlayerState = ePlayerStateType.TurnActive;
                break;
            case ePlayerStateType.MovingToTarget:
                MovingToTarget();
                break;
            case ePlayerStateType.MoveComplete:
                MoveComplete();
                
                if (iMovesRemaining > 0)
                {
                    //if (eTeam == eTeamType.PlayerTeam)
                    if (AI_Move == WaitForPlayerMove)
                    {
                        ResetMoveRange();
                        bMoveReady = true;
                    }
                }
                break;
            case ePlayerStateType.ActComplete:
                ActComplete();
                UpdateSkillStates();
                break;
            case ePlayerStateType.WaitingForMoveTarget:
                AI_Move();
                break;
            case ePlayerStateType.Reset:
                bMoveReady = true;
                bActReady = true;

                   

                    UpdateScanRange();
                    if (lMoveRange.Count == 0)
                    {
                        bMoveReady = false;
                    }
                ePlayerState = ePlayerStateType.TurnActive;
                UpdateSkillStates();
                break;


        }
    }


    protected override void MoveComplete()
    {
        base.MoveComplete();
        ResetMoveRange();
        UpdateScanRange();
        UpdateSkillStates();
    }

    protected override int GetMovesRemaining()
    {
        return Mathf.Max(DiceManager.RollForMove(),8);
    }

}
