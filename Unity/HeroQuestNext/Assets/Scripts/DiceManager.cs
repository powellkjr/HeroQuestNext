using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eDiceTypes
{
    MoveDice,
    CombatDice,
    
}

public  enum eDiceFace
{
    Skull,
    White,
    Black,
}


public class DiceManager
{
    public static DiceManager Instance { get; private set; }

    private static List<int> lMoveDice = new List<int> { 1, 2, 3, 4, 5, 6 };
    private static List<string> lMoveDiceUnicode = new List<string> { "[\u2680]", "[\u2681]", "[\u2682]", "[\u2683]", "[\u2684]", "[\u2685]" };

    private static List<eDiceFace> lCombatDice = new List<eDiceFace> { eDiceFace.Skull, eDiceFace.Skull, eDiceFace.Skull, eDiceFace.White, eDiceFace.White, eDiceFace.Black };
    private static List<string> lCombatDiceUnicode = new List<string> { "[\u2620]", "[\u2620]", "[\u2620]", "[\u2656]", "[\u2656]" ,"[\u263b]"};
    


    public DiceManager() // where TGridObject : IPathable
    {
        Instance = this;
    }

    public List<int> RollRedDice(int inDice)
    {
        string strLog = "RedDiceRoll: ";
        List<int> lReturn = new List<int>();
        for(int i = 0; i < inDice; i++)
        {
            int j = Random.Range(0, lMoveDice.Count - 1);
            lReturn.Add(lMoveDice[j]);
            strLog += lMoveDiceUnicode[j];
        }
        Debug.Log(strLog);
        return lReturn;
    }

    public List<eDiceFace> RollCombatDice(int inDice)
    {
        string strLog = "ComabtDiceRoll: ";
        List<eDiceFace> lReturn = new List<eDiceFace>();
        for (int i = 0; i < inDice; i++)
        {
            int j = Random.Range(0, lCombatDice.Count - 1);
            lReturn.Add(lCombatDice[j]);
            strLog += lCombatDiceUnicode[j];
        }
        Debug.Log(strLog);
        return lReturn;
    }

    public int RollForMove()
    {
        List<int> lRolls = RollRedDice(2);
        int iReturn = 0;
        foreach(int aRoll in lRolls)
        {
            iReturn += aRoll;
        }
        Debug.Log("MoveRoll: " + iReturn);
        return iReturn;
    }

    public int RollForCombatFace(int inDice,eDiceFace inDiceFace)
    {
        List<eDiceFace> lRolls = RollCombatDice(inDice);
        int iReturn = 0;
        foreach (eDiceFace aRoll in lRolls)
        {
            iReturn += (aRoll == inDiceFace) ?1:0;
        }
        Debug.Log("RollForCombatFace: " + iReturn + lCombatDiceUnicode[(int)inDiceFace]);
        return iReturn;
    }

    public int RollForAttack(int inDice1, eDiceFace inDiceFace1, int inDice2, eDiceFace inDiceFace2)
    {
        int iAttack = RollForCombatFace(inDice1, inDiceFace1);
        int iDefend = RollForCombatFace(inDice2, inDiceFace2);
        Debug.Log("RollForAttack: " + iAttack + lCombatDiceUnicode[(int)inDiceFace1] + ":" + iDefend + lCombatDiceUnicode[(int)inDiceFace2]);
        return Mathf.Min(0,  iAttack - iDefend);
    }
}

