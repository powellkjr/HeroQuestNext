using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Mathematics;
using UnityEngine;

public class PathNode : IPathable<PathNode>
{
    //private BlackBocksGrid<IPathable> arrGrid;
    public int x { get; set; }
    public int y { get; set; }
    public int iGCost { get; set; }
    public int iHCost { get; set; }
    public  int iFCost { get; set; }

    public eNavType eNav { get; set; }

    public PathNode ICameFrom { get; set; }

    public void GetPosition(out Vector2Int outPos) {outPos =  new Vector2Int(x, y); }
    public Vector2Int GetPosition() { return new Vector2Int(x, y); }
    public void GetPosition(out Vector2 outPos) { outPos = new Vector2(x, y); }
    public void GetPosition(out Vector3 outPos) { outPos = new Vector3(x, y); }
    public void GetPosition(out int outX, out int outY) { outX = x; outY = y; }
   
    public bool IsBlocked(eNavType inNavType)
    {
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


    public void CalculateFCost()
    {
        iFCost = iGCost + iHCost;
        //arrGrid.TriggerGridObjectChanged(new Vector2Int(x, y));
    }
     public PathNode(int inX, int inY)
    {
        //arrGrid = inGrid;
        x = inX;
        y = inY;
        eNav = eNavType.Any;
    }
    public void InitPathable(int inX, int inY)
    {
        x = inX;
        y = inY;
    }

    public override string ToString()
    {
        return ((eNavText)eNav).ToString();
            //+ "\nG" + iGCost + "H" + iHCost + "F" + iFCost;
    }
}
