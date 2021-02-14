using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Agent : Player
{
    public Player_Agent(
       (eMoveableType, int) inMoveableKey,
       Vector3Int inStartPos
       ) : base(inMoveableKey, inStartPos)
    {
        eTeam = eTeamType.PlayerTeam;
        eEnemyTeam = eTeamType.EnemyTeam;
        eDefendFace = eDiceFace.White;


        switch (iRefIndex)
        {
            case (int)ePlayers.Barbarian:
                ePlayerClass = eEquipmentRefType.Body_Barbaian;
                strPlayerName = NameManger.GetNameByGroupType(eNameGroupType.Barbarian);
                break;
            case (int)ePlayers.Dwarf:
                ePlayerClass = eEquipmentRefType.Body_Dwarf;
                strPlayerName = NameManger.GetNameByGroupType(eNameGroupType.Dwarf);
                break;
            case (int)ePlayers.Elf:
                ePlayerClass = eEquipmentRefType.Body_Elf;
                strPlayerName = NameManger.GetNameByGroupType(eNameGroupType.Elf);
                break;
            case (int)ePlayers.Wizard:
                ePlayerClass = eEquipmentRefType.Body_Wizard;
                strPlayerName = NameManger.GetNameByGroupType(eNameGroupType.Wizard);
                break;
        }
        EquipmentManger.EquipItem_Static(EquipmentManger.GetEquipmentData_Static(ePlayerClass), out sStats, lEquiped, lStored);
        EquipmentManger.EquipItem_Static(EquipmentManger.GetStartingEquipmentFromBody_Static(ePlayerClass), out sStats, lEquiped, lStored);
        EquipBasicSkills();

       
        AI_Move = MoveFarthestInRange;
        eAI_State = eAI_StateType.FollowTarget;
        sAI_TargetKey = (eMoveableType.Player, 0);
        
        
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

    


    public void UpdateSkillStates()
    {
        ActiveSkillRefType = eEquipmentRefType.Idle;
        ActiveSkillRange = null;
        HeroMapVisual.HideUserRange();
        foreach (EquipmentData aSkill in lEquiped)
        {
            //aSkill.OnActivate = OnSkillActivate;
            //aSkill.OnMouseOver = OnSkillMouseOver;
            aSkill.OnActivate = () => {
                Debug.LogWarning("Activating the skill: " + aSkill.tToolTipShort);
                ActiveSkillRefType = aSkill.eEquipmentRef;
            };
            //aSkill.OnActivate += () => {ShowUserRange(HeroMap.GetNavNeighborsList(GetPosXY().x, GetPosXY().y), eSpritePages.MoveTiles, (int)eVisualIcons.OutlineSelector); };
            aSkill.OnMouseOver = () => { Debug.LogWarning("Show Scan with" + aSkill.eEquipmentRange.ToString()); };
            aSkill.OnMouseOut = () => {
                Debug.LogWarning("Disable Scan with" + aSkill.eEquipmentRange.ToString());
                if (ActiveSkillRefType == eEquipmentRefType.Idle)
                {
                    HeroMapVisual.HideUserRange();
                }
            };

            switch (aSkill.eEquipmentRef)
            {
                case eEquipmentRefType.Legs_PlayerBoots:
                    aSkill.eSkillReadyState = bMoveReady ? eSkillReadyStateType.MoveReady : eSkillReadyStateType.Disabled;

                    aSkill.OnActivate += () =>
                    {
                        HeroMapVisual.ShowUserRange(lMoveRange, eSpritePages.MoveTiles, (int)eVisualIcons.CornerSelector);
                    };
                    aSkill.OnMouseOver += () =>
                    {
                        if (ActiveSkillRefType == eEquipmentRefType.Idle)
                        {
                            HeroMapVisual.ShowUserRange(lMoveRange, eSpritePages.MoveTiles, (int)eVisualIcons.OutlineSelector);
                        }
                    };
                    

                    break;
                case eEquipmentRefType.Core_LockPick:
                    bool bAnyDoor = HeroMap.GetGridObject(GetPosXY().x, GetPosXY().y).GetNavTileIndex() == eNavTiles.DoorNorthClosed || HeroMap.GetGridObject(GetPosXY().x, GetPosXY().y).GetNavTileIndex() == eNavTiles.DoorSouthClosed;
                    aSkill.eSkillReadyState = (bMoveReady && bAnyDoor) ? eSkillReadyStateType.MoveReady : eSkillReadyStateType.Disabled;
                    aSkill.OnActivate += () => {
                        HeroMapVisual.ShowUserRange(HeroMap.GetGridObject(GetPosXY()), eSpritePages.MoveTiles, (int)eVisualIcons.CornerSelector);
                    };
                    aSkill.OnMouseOver += () =>
                    {
                        if (ActiveSkillRefType == eEquipmentRefType.Idle)
                        {
                            HeroMapVisual.ShowUserRange(HeroMap.GetGridObject(GetPosXY()), eSpritePages.MoveTiles, (int)eVisualIcons.OutlineSelector);
                        }
                    };
                    break;

                case eEquipmentRefType.Weapon_BattleAxe:
                case eEquipmentRefType.Weapon_Broadsword:
                case eEquipmentRefType.Weapon_Crossbow:
                case eEquipmentRefType.Weapon_Dagger:
                case eEquipmentRefType.Weapon_LongSword:
                case eEquipmentRefType.Weapon_ShortSword:
                case eEquipmentRefType.Weapon_Staff:
                    aSkill.eSkillReadyState = !(!bActReady || lActRange.Count == 0) ? eSkillReadyStateType.ActReady : eSkillReadyStateType.Disabled;
                    aSkill.OnActivate += () =>
                    {
                        HeroMapVisual.ShowUserRange(lActRange, eSpritePages.ActTiles, (int)eVisualIcons.CornerSelector);
                    };
                    aSkill.OnMouseOver += () =>
                    {
                        if (ActiveSkillRefType == eEquipmentRefType.Idle)
                        {
                            HeroMapVisual.ShowUserRange(lActRange, eSpritePages.ActTiles, (int)eVisualIcons.OutlineSelector);
                        }
                    };

                    break;

                case eEquipmentRefType.Body_Barbaian:
                case eEquipmentRefType.Body_Dwarf:
                case eEquipmentRefType.Body_Elf:
                case eEquipmentRefType.Body_Wizard:
                    aSkill.eSkillReadyState = eSkillReadyStateType.Passive;
                    aSkill.OnActivate += () => { HeroMapVisual.ShowUserRange(HeroMap.GetGridObject(GetPosXY()), eSpritePages.PassiveTiles, (int)eVisualIcons.CornerSelector); };
                    aSkill.OnMouseOver += () =>
                    {
                        if (ActiveSkillRefType == eEquipmentRefType.Idle)
                        {
                            HeroMapVisual.ShowUserRange(HeroMap.GetGridObject(GetPosXY()), eSpritePages.PassiveTiles, (int)eVisualIcons.OutlineSelector);
                        }
                    };

                    break;

                case eEquipmentRefType.Core_PassageMap:
                case eEquipmentRefType.Core_TrapMap:
                case eEquipmentRefType.Core_TreasureMap:
                    aSkill.eSkillReadyState = bActReady ? eSkillReadyStateType.ActReady : eSkillReadyStateType.Disabled;
                    aSkill.OnActivate += () => { HeroMapVisual.ShowUserRange(lScanRange, eSpritePages.ScanTiles, (int)eVisualIcons.CornerSelector); };
                    aSkill.OnMouseOver += () =>
                    {
                        if (ActiveSkillRefType == eEquipmentRefType.Idle)
                        {
                            HeroMapVisual.ShowUserRange(lScanRange, eSpritePages.ScanTiles, (int)eVisualIcons.OutlineSelector);
                        }
                    };
                    break;
                case eEquipmentRefType.Kit_TrapTools:
                    bool bAnyTrap =
                        HeroMap.GetGridObject(GetPosXY().x, GetPosXY().y).GetNavTileIndex() == eNavTiles.Pit ||
                        HeroMap.GetGridObject(GetPosXY().x, GetPosXY().y).GetNavTileIndex() == eNavTiles.Rocks ||
                        HeroMap.GetGridObject(GetPosXY().x, GetPosXY().y).GetNavTileIndex() == eNavTiles.Spear;

                    aSkill.eSkillReadyState = (bMoveReady && bAnyTrap) ? eSkillReadyStateType.MoveReady : eSkillReadyStateType.Disabled;
                    aSkill.OnActivate += () => { HeroMapVisual.ShowUserRange(HeroMap.GetGridObject(GetPosXY()), eSpritePages.ActTiles, (int)eVisualIcons.CornerSelector); };
                    aSkill.OnMouseOver += () =>
                    {
                        if (ActiveSkillRefType == eEquipmentRefType.Idle)
                        {
                            HeroMapVisual.ShowUserRange(HeroMap.GetGridObject(GetPosXY()), eSpritePages.ActTiles, (int)eVisualIcons.OutlineSelector);
                        }
                    };
                    break;
            }
        }
        OnSkillUpdate?.Invoke(lEquiped);
    }

    


    protected override void MoveComplete()
    {
        base.MoveComplete();
        UpdateScanRange();
        UpdateSkillStates();
    }

    protected override void Reset()
    {
        base.Reset();
        iMovesRemaining = DiceManager.RollForMove();
        ResetActRange();
        ResetMoveRange();
        if (lMoveRange.Count == 0)
        {
            bMoveReady = false;
        }
    }

    protected override int GetMovesRemaining()
    {
        return DiceManager.RollForMove();
    }
}
