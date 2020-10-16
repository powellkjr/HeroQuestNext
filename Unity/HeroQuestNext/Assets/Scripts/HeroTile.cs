using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public enum eNavType
{
    Any = 0,
    North = 1,
    NorthEast = 2,
    East = 3,
    SouthEast = 4,
    South = 5,
    SouthWest = 6,
    West = 7,
    NorthWest = 8,
    Spare1 = 9,
    All = 10,
    NorthSouth = 11,
    WestEast= 12,
}
public class HeroTile
{
   
    
    private Vector2Int vPosition;
    private bool bWalkable;
    private int iSpriteIndex;
    private int iBaseIndex;
    private int iWallIndex;
    private eNavType eNav;
    public BlackBocksGrid<HeroTile> gHostGrid;

    public Vector2 GetPosition() { return vPosition; }
    public int GetSpriteIndex() { return iSpriteIndex; }

    public int GetBaseIndex() { return iBaseIndex; }
    public int GetWallIndex() { return iWallIndex; }

    public bool IsWalkable() { return bWalkable; }
    public bool IsBlocked( eNavType inNavType) 
    { 
        if (!bWalkable) { return true; }

        switch (inNavType)
        {
            case eNavType.North:
                return (eNav == eNavType.NorthWest) || (eNav == eNavType.North) || (eNav == eNavType.NorthEast) || (eNav == eNavType.NorthSouth);
            case eNavType.NorthWest:
                return (eNav == eNavType.North) || (eNav == eNavType.NorthWest) || (eNav == eNavType.West) || (eNav == eNavType.NorthSouth) || (eNav == eNavType.WestEast);
            case eNavType.West:
                return (eNav == eNavType.NorthWest) || (eNav == eNavType.West) || (eNav == eNavType.SouthWest) || (eNav == eNavType.WestEast);
            case eNavType.SouthWest:
                return (eNav == eNavType.West) || (eNav == eNavType.SouthWest) || (eNav == eNavType.South) || (eNav == eNavType.NorthSouth) || (eNav == eNavType.WestEast);
            case eNavType.South:
                return (eNav == eNavType.SouthWest) || (eNav == eNavType.South) || (eNav == eNavType.SouthEast) || (eNav == eNavType.NorthSouth);
            case eNavType.SouthEast:
                return (eNav == eNavType.South) || (eNav == eNavType.SouthEast) || (eNav == eNavType.East) || (eNav == eNavType.NorthSouth) || (eNav == eNavType.WestEast);
            case eNavType.East:
                return (eNav == eNavType.SouthEast) || (eNav == eNavType.East) || (eNav == eNavType.NorthEast) || (eNav == eNavType.WestEast);
            case eNavType.NorthEast:
                return (eNav == eNavType.East) || (eNav == eNavType.NorthEast) || (eNav == eNavType.North) || (eNav == eNavType.NorthSouth) || (eNav == eNavType.WestEast);
            case eNavType.All:
                return true;
            default:
            case eNavType.Spare1:
            case eNavType.Any:
                return false;
        }
    }

    public void IncrementSpriteIndex()
    {
        iSpriteIndex++;
        iSpriteIndex %= System.Enum.GetValues(typeof(eNavType)).Length;
        eNav = (eNavType)iSpriteIndex;
        gHostGrid.TriggerGridObjectChanged(vPosition);
    }

    public void SetSpriteIndex(int inSpriteIndex)
    {
        iSpriteIndex = inSpriteIndex;
        iSpriteIndex %= System.Enum.GetValues(typeof(eNavType)).Length;
        eNav = (eNavType)iSpriteIndex;
        gHostGrid.TriggerGridObjectChanged(vPosition);
    }

    public void SetBaseIndex(int inBaseIndex)
    {
        iBaseIndex = inBaseIndex;
        gHostGrid.TriggerGridObjectChanged(vPosition);
    }

    public void SetWalIndex(int inWallIndex)
    {
        iWallIndex = inWallIndex;
        gHostGrid.TriggerGridObjectChanged(vPosition);
    }
    public void SetNav(eNavType inNavType)
    {
        iSpriteIndex = (int)inNavType;
        iSpriteIndex %= System.Enum.GetValues(typeof(eNavType)).Length;
        eNav = inNavType;
        bWalkable = true;
        gHostGrid.TriggerGridObjectChanged(vPosition);
    }

    public HeroTile(
       BlackBocksGrid<HeroTile> inHostGrid,
       int inX = 0,
       int inY = 0,
       bool inWalkable = false,
       int inSpriteIndex = 0,
       int inBaseIndex = 0,
       int inWallIndex = 0,
       eNavType inNavType = eNavType.Any)
    {
        this.vPosition = new Vector2Int(inX, inY);
        this.gHostGrid = inHostGrid;
        this.bWalkable = inWalkable;
        this.iSpriteIndex = inSpriteIndex;
        this.eNav = inNavType;
        this.iBaseIndex = inBaseIndex;
        this.iWallIndex = inWallIndex;

    }

    public override string ToString()
    {
        return iSpriteIndex.ToString();
    }

}
