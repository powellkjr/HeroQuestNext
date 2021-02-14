using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eEquipmentRefType
{
    Idle,
    Weapon_Broadsword,
    Weapon_ShortSword,
    Weapon_LongSword,
    Weapon_Dagger,
    Weapon_Crossbow,
    Weapon_BattleAxe,
    Weapon_Staff,
    Spell_BallOfFlame,
    Body_Barbaian,
    Body_Dwarf,
    Body_Elf,
    Body_Wizard,
    Body_Skeleton,
    Body_Zombie,
    Body_Mummy,
    Body_Goblin,
    Body_Orc,
    Body_Fimir,
    Body_ChaosWarrior,
    Body_Gargoyle,
    Body_ChaosScorcer,
    Core_TreasureMap,
    Core_PassageMap,
    Core_TrapMap,
    Core_LockPick,
    Legs_PlayerBoots,
    Kit_TrapTools,

}

public enum eEquipmentRangeType
{
    Self,
    Tile_Adjacent,
    Melee_Adjacent,
    Melee_Diagnoal,
    Melee_AdjacentReach,
    Range_Ranged,
    Range_Thrown,
    Scan,
}
public enum eEquipmentSlotType
{
    Core,
    Body,
    Head,
    Legs,
    Kit,
    Gloves,
    Weapon,
    Spell,
    Total
}
public enum eSkillReadyStateType
{
    Disabled,
    MoveReady,
    ActReady,
    MoveActive,
    ActActive,
    Passive,

}

public enum eSkillTokenType
{
    Passive,
    Movement,
    Action,
    Shop
}
public class EquipmentData
{
    public eEquipmentRefType eEquipmentRef;
    public eSkillTokenType eSkillToken;
    public eEquipmentSlotType eEquipmentSlot = eEquipmentSlotType.Weapon;
    public eEquipmentRangeType eEquipmentRange = eEquipmentRangeType.Melee_Adjacent;
    public int iMoveSquares = 0;
    public int iAttackDice;
    public int iDefenceDice;
    public eDiceFace eUseFace = eDiceFace.Skull;
    public int iHitPoints = 0;
    public int iMindPoints = 0;
    public List<eKeywordType> eKeywords = new List<eKeywordType>();
    public string tToolTipShort;
    public string tToolTipLong;
    public int iCost = 0;
    public eSkillReadyStateType eSkillReadyState = eSkillReadyStateType.Disabled;
    public Action OnMouseOver;
    public Action OnMouseOut;
    public Action OnActivate;
    

    public static EquipmentData operator +(EquipmentData sLeftSide, EquipmentData sRightSide)
    {
        return new EquipmentData
        {
            iMoveSquares = sLeftSide.iMoveSquares + sRightSide.iMoveSquares,
            iAttackDice = sLeftSide.iAttackDice + sRightSide.iAttackDice,
            iDefenceDice = sLeftSide.iDefenceDice + sRightSide.iDefenceDice,
            iHitPoints = sLeftSide.iHitPoints + sRightSide.iHitPoints,
            iMindPoints = sLeftSide.iMindPoints + sRightSide.iMindPoints,
        };
    }
}

public enum eEquipmentListType
{
    PlayerList,MonsterList,ShopList,

}
public enum eKeywordType
{
    Bulky,
    Cumbersome,
    Ranged,
    Thrown,
    Reach,
    Two_Handed,
    Unlimited,
    Trap,
    Discarded
}

public class EquipmentManger : MonoBehaviour
{
    [SerializeField] private Texture2D tSearchForTreasure;
    [SerializeField] private Texture2D tSearch_ForTreasure;
    [SerializeField] private Texture2D tSearch_ForSecretDoor;
    [SerializeField] private Texture2D tSearch_ForTraps;
    [SerializeField] private Texture2D tDisarm_Trap;
    [SerializeField] private Texture2D tAttack_Broadsword;
    [SerializeField] private Texture2D tAttack_ShortSword;
    [SerializeField] private Texture2D tAttack_Dagger;
    [SerializeField] private Texture2D tSpell_FireBall;
    [SerializeField] private Texture2D tAttack_Enemy_R1;
    public static EquipmentManger Instance { get; private set; }
    private static List<EquipmentData> lPlayerEqipment = new List<EquipmentData> {
        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Core_TreasureMap,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Core,
            eEquipmentRange = eEquipmentRangeType.Scan,
            tToolTipShort ="SEARCH\nFOR TREASURE",
            tToolTipLong = "<color=yellow>[Search]</color> for treauser in the room you're in",

        },
        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Core_PassageMap,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Core,
            eEquipmentRange = eEquipmentRangeType.Scan,

            tToolTipShort ="SEARCH\nFOR SECRET DOORS",
            tToolTipLong = "<color=yellow>[Search]</color> for secret doors in the room or corridor you're in.",
        },
        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Core_TrapMap,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Core,
            eEquipmentRange = eEquipmentRangeType.Scan,
            tToolTipShort ="SEARCH\nFOR TRAPS",
            tToolTipLong = "<color=yellow>[Search]</color> for traps in the room or corridor you're in",
        },
        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Kit_TrapTools,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Kit,
            eEquipmentRange = eEquipmentRangeType.Self,
            tToolTipShort ="DISARM\n A TRAP",
            tToolTipLong = "<color=yellow>[Disarm]</color> a trap on the square you're on."
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Legs_PlayerBoots,
            eSkillToken = eSkillTokenType.Movement,
            eEquipmentSlot = eEquipmentSlotType.Core,
            eEquipmentRange = eEquipmentRangeType.Scan,

            tToolTipShort ="MOVE\nTO TILE",
            tToolTipLong = "As the character explore Morcar's dungeons, they enter new rooms and passages."
        },
        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Core_LockPick,
            eSkillToken = eSkillTokenType.Movement,
            eEquipmentSlot = eEquipmentSlotType.Core,
            eEquipmentRange = eEquipmentRangeType.Tile_Adjacent,
            tToolTipShort ="MOVE\nOPEN DOOR",
            tToolTipLong = "You can open a door by moving onto the square in front of it. " +
                "\nYou do not have to open the door if you do not want to. Opening a door does not count as a move. " +
                "\nHaving opened a door you can keep moving, if you have any space left to move.",
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Barbaian,
            eSkillToken = eSkillTokenType.Passive,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iDefenceDice = 2,
            iHitPoints= 8,
            iMindPoints=2,
            tToolTipShort ="CLASS\nBARBARIAN",
            tToolTipLong = "You are the Barbarian, " +
            "\nthe greatest warrior of " +
            "\nall. But beware of magic " +
            "\nfor your sword is no " +
            "\ndefence against it."
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Dwarf,
            eSkillToken = eSkillTokenType.Passive,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iDefenceDice = 2,
            iHitPoints= 7,
            iMindPoints=3,
            tToolTipShort ="CLASS\nDWARF",
            tToolTipLong = "You are the Dwarf." +
            "\nYour are a good warrior " +
            "\nand have the unique ability" +
            "\nto disarm traps without a tool kit"
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Elf,
            eSkillToken = eSkillTokenType.Passive,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iDefenceDice = 2,
            iHitPoints= 6,
            iMindPoints=4,
            tToolTipShort ="CLASS\nELF",
            tToolTipLong = "You are the Elf," +
            "\n a master of both magic " +
            "\nand the sword. You must use both well" +
            "\nif you are to triumph."

        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Wizard,
            eSkillToken = eSkillTokenType.Passive,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iDefenceDice = 2,
            iHitPoints= 4,
            iMindPoints=6,
            tToolTipShort ="CLASS\nWIZARD",
            tToolTipLong = "You are the Wizard." +
            "\nYou have many spells to cast in combat, " +
            "\nbut without armor or large weapons" +
            "\nyou must use them wisely to avoid phyiscal combat."
        },
    };
    private static List<EquipmentData> lMonsterEqipment = new List<EquipmentData> {
        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Skeleton,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iMoveSquares = 6,
            iDefenceDice = 2,
            iHitPoints= 1,
            iMindPoints=0,
            tToolTipShort ="CLASS\nSKELETON",
        },

                new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Zombie,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iMoveSquares = 5,
            iAttackDice = 2,
            iDefenceDice = 3,
            iHitPoints= 1,
            iMindPoints=0,
            tToolTipShort ="CLASS\nZOMBIE",
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Mummy,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iMoveSquares = 4,
            iDefenceDice = 4,
            iHitPoints= 2,
            iMindPoints=0,
            tToolTipShort ="CLASS\nZOMBIE",
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Goblin,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iMoveSquares = 10,
            iDefenceDice = 1,
            iHitPoints= 1,
            iMindPoints=1,
            tToolTipShort ="CLASS\nZOMBIE",
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Orc,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iMoveSquares = 8,
            iDefenceDice = 2,
            iHitPoints= 1,
            iMindPoints=2,
            tToolTipShort ="CLASS\nORC",
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Fimir,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iMoveSquares = 6,
            iDefenceDice = 3,
            iHitPoints= 2,
            iMindPoints=3,
            tToolTipShort ="CLASS\nORC",
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_ChaosWarrior,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iMoveSquares = 7,
            iDefenceDice = 2,
            iMindPoints=2,
            iHitPoints= 8,
            tToolTipShort ="CLASS\nCHAOS WARRIOR",
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_Gargoyle,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iMoveSquares = 6,
            iDefenceDice = 5,
            iHitPoints= 3,
            iMindPoints=4,
            tToolTipShort ="CLASS\nGARGOYLE",
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Body_ChaosScorcer,
            eEquipmentSlot = eEquipmentSlotType.Body,
            iMoveSquares = 0,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            tToolTipShort ="CLASS\nGARGOYLE",
        },
    };
    private static List<EquipmentData> lStoreEquipment = new List<EquipmentData>
    {
        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Weapon_Dagger,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Weapon,
            eEquipmentRange = eEquipmentRangeType.Range_Thrown,
            iAttackDice = 1,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            iMoveSquares = 0,
            eUseFace= eDiceFace.Skull,
            tToolTipShort ="WEAPON\nDAGGER",
            tToolTipLong = "This wide blade gives you the <color=yellow>[Attack]</color> strength of 1 combat dice.",
            eKeywords = new List<eKeywordType>(){eKeywordType.Thrown},
            iCost = 25,
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Weapon_Broadsword,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Weapon,
            eEquipmentRange = eEquipmentRangeType.Melee_Adjacent,
            iAttackDice = 3,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            iMoveSquares = 0,
            eUseFace= eDiceFace.Skull,
            tToolTipShort ="WEAPON\nBROADSWORD",
            tToolTipLong = "This wide blade gives you the <color=yellow>[Attack]</color> strength of 3 combat dice.",
            eKeywords = new List<eKeywordType>(){eKeywordType.Cumbersome},
            iCost = 250,
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Weapon_ShortSword,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Weapon,
            eEquipmentRange = eEquipmentRangeType.Melee_Adjacent,
            iAttackDice = 2,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            iMoveSquares = 0,
            eUseFace= eDiceFace.Skull,
            tToolTipShort ="WEAPON\nSHORTSWORD",
            tToolTipLong = "This short blade gives you the <color=yellow>[Attack]</color> strengh of 2 combat dice.",
            eKeywords = new List<eKeywordType>(){eKeywordType.Cumbersome},
            iCost = 150,
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Weapon_LongSword,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Weapon,
            eEquipmentRange = eEquipmentRangeType.Melee_AdjacentReach,
            iAttackDice = 2,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            iMoveSquares = 0,
            eUseFace= eDiceFace.Skull,
            tToolTipShort ="WEAPON\nLONGSWORD",
            tToolTipLong = "This short blade gives you the <color=yellow>[Attack]</color> strengh of 2 combat dice.",
            eKeywords = new List<eKeywordType>(){eKeywordType.Reach,eKeywordType.Cumbersome},
            iCost = 350,
        },

                new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Weapon_Crossbow,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Weapon,
            eEquipmentRange = eEquipmentRangeType.Range_Ranged,
            iAttackDice = 3,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            iMoveSquares = 0,
            eUseFace= eDiceFace.Skull,
            tToolTipShort ="WEAPON\nCROSSBOW",
            tToolTipLong = "This long-range weapon gives you the <color=yellow>[Attack]</color> strength of 3 combat dice.",
            eKeywords = new List<eKeywordType>(){eKeywordType.Ranged,eKeywordType.Unlimited,eKeywordType.Cumbersome},
            iCost = 350,
        },

        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Weapon_BattleAxe,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Weapon,
            eEquipmentRange = eEquipmentRangeType.Melee_Adjacent,
            iAttackDice = 4,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            iMoveSquares = 0,
            eUseFace= eDiceFace.Skull,
            tToolTipShort ="WEAPON\nBATTLEAXE",
            tToolTipLong = "This heavy, double-edged axe givs you the <color=yellow>[Attack]</color> strength of 4 combat dice. ",
            eKeywords = new List<eKeywordType>(){ eKeywordType.Two_Handed,eKeywordType.Cumbersome},
            iCost = 450,
        },
        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Weapon_Staff,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Weapon,
            eEquipmentRange = eEquipmentRangeType.Melee_AdjacentReach,
            iAttackDice = 1,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            iMoveSquares = 0,
            eUseFace= eDiceFace.Skull,
            tToolTipShort ="WEAPON\nSTAFF",
            tToolTipLong = "This long, strudy wooden staff gives you the <color=yellow>[Attack]</color> strength of 1 combat die. ",
            eKeywords = new List<eKeywordType>(){eKeywordType.Reach,eKeywordType.Two_Handed},
            iCost = 350,
        },

                new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Kit_TrapTools,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Weapon,
            eEquipmentRange = eEquipmentRangeType.Melee_AdjacentReach,
            iAttackDice = 1,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            iMoveSquares = 0,
            eUseFace= eDiceFace.Skull,
            tToolTipShort ="KIT\nTOOL KIT",
            tToolTipLong = "This tool kit enables you to remove any trap that you find. Roll 1 combat die:",
            eKeywords = new List<eKeywordType>(){eKeywordType.Trap},
            iCost = 250,
        },
        new EquipmentData
        {
            eEquipmentRef = eEquipmentRefType.Spell_BallOfFlame,
            eSkillToken = eSkillTokenType.Action,
            eEquipmentSlot = eEquipmentSlotType.Spell,
            eEquipmentRange = eEquipmentRangeType.Range_Ranged,
            iAttackDice = 2,
            iDefenceDice = 0,
            iHitPoints= 0,
            iMindPoints=0,
            iMoveSquares = 0,
            eUseFace= eDiceFace.Skull,
            tToolTipShort ="SPELL\nBALL OF FLAME",
            tToolTipLong = "This spell may be cast at any one monster or player." +
            "\nIt will inflict two points of Body damage." +
            "\nThe victim may roll two dice. For each sheild" +
            "\nhe rolls he may reduce the damage by one.",
            eKeywords = new List<eKeywordType>(){eKeywordType.Discarded},
            iCost = 250,
        },

    };
    public static Dictionary<eKeywordType, string> dKeywordText = new Dictionary<eKeywordType, string>()
{
    {eKeywordType.Bulky, "<color=red>\n-May not be used with a Two-Handed weapon.</color>"},
    {eKeywordType.Cumbersome,"<color=red>\n-May not be used by the Wizard.</color>" },
    {eKeywordType.Ranged,"<color= green>\n+You may fire at any moster that you can 'see'. </color>" +
            "<color=red>\n+You may not fire at any moster adjacent to you. </color>" },
    {eKeywordType.Thrown,"<color=green>\n+You may throw at any moster that you can 'see'.  </color>" +
            "<color=red>\n-Weapon is lost once it is thrown.</color>" },
    {eKeywordType.Unlimited,"<color=green>\n+You have an unlimited supply of arrows.</color>"},
    {eKeywordType.Two_Handed,"<color=red>\n-Requires two hands to use, may not be used with a shield</color>" },
    {eKeywordType.Trap, "<color=green>\n+On a shield the trap is disarmed and remove the trap.</color>" +
            "<color=red>\n-On a skull lose one Body point and remove the trap.</color>" },
    {eKeywordType.Discarded,"<color=red>\n-On use the spell is discarded." }
};
    //public EquipmentManger()
    //    {
    //    Instance = this;
    //    }
    private void Awake()
    {
        Instance = this;
    }
    public static EquipmentData GetEquipmentData_Static(eEquipmentRefType inEquipmentRefType)
    {
        return Instance.GetEquipmentData(inEquipmentRefType);
    }
    public EquipmentData GetEquipmentData(eEquipmentRefType inEquipmentRefType)
    {
        foreach (EquipmentData aEquipmentData in lStoreEquipment)
        {
            if (aEquipmentData.eEquipmentRef == inEquipmentRefType)
            {
                return aEquipmentData;
            }
        }

        foreach (EquipmentData aEquipmentData in lMonsterEqipment)
        {
            if (aEquipmentData.eEquipmentRef == inEquipmentRefType)
            {
                return aEquipmentData;
            }
        }

        foreach (EquipmentData aEquipmentData in lPlayerEqipment)
        {
            if (aEquipmentData.eEquipmentRef == inEquipmentRefType)
            {
                return aEquipmentData;
            }
        }

        return new EquipmentData();
    }

    public static EquipmentData GetStartingEquipmentFromBody_Static(eEquipmentRefType inEquipRefType)
    {
        return Instance.GetStartingEquipmentFromBody(inEquipRefType);
    }
    public EquipmentData GetStartingEquipmentFromBody(eEquipmentRefType inEquipRefType)
    {
        switch(inEquipRefType)
        {
            case eEquipmentRefType.Body_Barbaian:
                return GetEquipmentData(eEquipmentRefType.Weapon_Broadsword);
            case eEquipmentRefType.Body_Dwarf:
                return GetEquipmentData(eEquipmentRefType.Weapon_ShortSword);
            case eEquipmentRefType.Body_Elf:
                return GetEquipmentData(eEquipmentRefType.Weapon_ShortSword);
            case eEquipmentRefType.Body_Wizard:
                return GetEquipmentData(eEquipmentRefType.Weapon_Dagger);
            case eEquipmentRefType.Body_Gargoyle:
                return GetEquipmentData(eEquipmentRefType.Weapon_BattleAxe);
            case eEquipmentRefType.Body_ChaosWarrior:
            case eEquipmentRefType.Body_Fimir:
            case eEquipmentRefType.Body_Mummy:
            case eEquipmentRefType.Body_Orc:
                return GetEquipmentData(eEquipmentRefType.Weapon_Broadsword);
            case eEquipmentRefType.Body_Goblin:
            case eEquipmentRefType.Body_Skeleton:
            case eEquipmentRefType.Body_Zombie:
                return GetEquipmentData(eEquipmentRefType.Weapon_ShortSword);
        }
        return new EquipmentData();
    }

    public static void EquipItem_Static(EquipmentData inEquipmentData, out EquipmentData inStats, List<EquipmentData> inEquipped, List<EquipmentData> inStored)
    {
        Instance.EquipItem(inEquipmentData, out inStats, inEquipped, inStored);
    }
    public void EquipItem(EquipmentData inEquipmentData, out EquipmentData inStats, List<EquipmentData> inEquipped, List<EquipmentData> inStored)
    {
        for (int i = 0; i < inEquipped.Count; i++)
        {
            EquipmentData aEquipment = inEquipped[i];
            if (aEquipment.eEquipmentSlot == inEquipmentData.eEquipmentSlot && inEquipmentData.eEquipmentSlot != eEquipmentSlotType.Core)
            {
                inStored.Add(aEquipment);
                inEquipped.RemoveAt(i);
            }
        }
        inEquipped.Add(inEquipmentData);
        //stats needs an equal operator
        UpdateStats(inEquipped, out inStats);
    }
    public void UpdateStats(List<EquipmentData> inEquipped, out EquipmentData inStats)
    {
        EquipmentData sReturn = new EquipmentData();
       
        sReturn.iMoveSquares = 0;
        sReturn.iAttackDice = 0;
        sReturn.iDefenceDice = 0;
        sReturn.iHitPoints = 0;
        sReturn.iMindPoints = 0;
        
        foreach (EquipmentData aEquipment in inEquipped)
        {
            sReturn += aEquipment;
        }
        inStats = sReturn;
    }
}
