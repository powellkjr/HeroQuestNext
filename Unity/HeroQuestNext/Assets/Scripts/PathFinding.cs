using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class PathFinding<TGridObject> where TGridObject : IPathable<TGridObject>
{
    private const int MOVE_EDGE_COST = 10;
    private const int MOVE_CORNER_COST = 14;
    private BlackBocksGrid<TGridObject> arrGrid;
    private List<TGridObject> lOpenList;
    private List<TGridObject> lClosedList;
    public static PathFinding<TGridObject> Instance { get; private set; }
   public PathFinding(BlackBocksGrid<TGridObject> inGrid) // where TGridObject : IPathable
    {
        Instance = this;
        arrGrid = inGrid;
    }

    public BlackBocksGrid<TGridObject> GetGrid()
    {
        return arrGrid;
    }

    public List<TGridObject> FindPath(Vector2Int inStartPos,Vector2Int inEndPos, eMoveableType inMoveable)
    {
        return FindPath(inStartPos.x, inStartPos.y, inEndPos.x, inEndPos.y, inMoveable);
    }
    public List<TGridObject> FindPath(int inStartX, int inStartY, int inEndX, int inEndY, eMoveableType inMoveable)
    {
        if(!arrGrid.IsValid(inStartX,inStartY) || !arrGrid.IsValid(inEndX, inEndY))
        {
            return null;
        }

        TGridObject pStartNode = arrGrid.GetGridObject(inStartX, inStartY);
        TGridObject pEndNode = arrGrid.GetGridObject(inEndX, inEndY);

        lOpenList = new List<TGridObject> { pStartNode };
        lClosedList = new List<TGridObject>();

        for (int x = 0; x < arrGrid.GetWidth(); x++)
        {
            for (int y = 0; y < arrGrid.GetHeight(); y++)
            {
                TGridObject pPathNode = arrGrid.GetGridObject(x, y);
                pPathNode.iGCost = int.MaxValue;
                pPathNode.CalculateFCost();
                pPathNode.ICameFrom = default;
            }
        }

        pStartNode.iGCost = 0;
        pStartNode.iHCost = CalculateDistanceCost(pStartNode, pEndNode);
        pStartNode.CalculateFCost();

        while(lOpenList.Count > 0)
        {
            TGridObject pCurrentNode = GetLowestFCostNode(lOpenList);
            //Debug.Log("Current" + pCurrentNode.x + ":" + pCurrentNode.y);
            if(pCurrentNode.GetPosition() == pEndNode.GetPosition())
            {
                return CalculatePath(pEndNode);
            }
            else
            {
                lOpenList.Remove(pCurrentNode);
                lClosedList.Add(pCurrentNode);
            }

            //foreach (PathNode aPathNode in arrGrid.GetNeighborsList(pCurrentNode.x, pCurrentNode.y))
            foreach (TGridObject aPathNode in GetNavNeighborsList(pCurrentNode.x, pCurrentNode.y, inMoveable))
            {
                if (lClosedList.Contains(aPathNode))
                {
                    continue;
                }
                int iTentativeGCost = pCurrentNode.iGCost + CalculateDistanceCost(pCurrentNode, aPathNode);
                if(iTentativeGCost < aPathNode.iGCost)
                {
                    aPathNode.ICameFrom = pCurrentNode;
                    aPathNode.iGCost = iTentativeGCost;
                    aPathNode.iHCost = CalculateDistanceCost(pCurrentNode, aPathNode);
                    aPathNode.CalculateFCost();

                    if (!lOpenList.Contains(aPathNode))
                    {
                        lOpenList.Add(aPathNode);
                    }
                }
            }
        }
        return null;
    }

    public List<TGridObject> FindPathWithRange(Vector2Int inStartPos,  int inRange, eMoveableType inMoveable = eMoveableType.None)
    {
        return FindPathWithRange(inStartPos.x, inStartPos.y, inRange, inMoveable);
    }
    public List<TGridObject> FindPathWithRange(int inStartX, int inStartY, int inRange,eMoveableType inMoveable=eMoveableType.None)
    {
        if(!arrGrid.IsValid(inStartX, inStartY))
        {
            return null;
        }
        int iRange = inRange + 1;
        List<TGridObject> lReturn = new List<TGridObject>();
        for (int x = inStartX - iRange; x < inStartX + iRange; ++x)
        {
            for (int y = inStartY - iRange; y < inStartY + iRange; ++y)
            {
                if (arrGrid.IsValid(x, y))
                {
                    if (arrGrid.GetGridObject(x, y).GetMoveable().Count == 0)
                    {
                        List<TGridObject> Test = FindPath(inStartX, inStartY, x, y, inMoveable);
                        if (Test != null)
                        {
                            if (Test.Count <= iRange)
                            {
                                lReturn.Add(Test[Test.Count - 1]);
                            }
                        }
                    }
                }
            }
        }
        return lReturn;
    }

    public List<TGridObject> FindTileInRange(Vector2Int inStartPos, int inRange)
    {
        return FindTileInRange(inStartPos.x, inStartPos.y, inRange);
    }
    public List<TGridObject> FindTileInRange(int inStartX, int inStartY, int inRange)
    {
        if (!arrGrid.IsValid(inStartX, inStartY))
        {
            return null;
        }
        int iRange = inRange + 1;
        List<TGridObject> lReturn = new List<TGridObject>();
        for (int x = inStartX - iRange; x < inStartX + iRange; ++x)
        {
            for (int y = inStartY - iRange; y < inStartY + iRange; ++y)
            {
                if (arrGrid.IsValid(x, y))
                {
                   if(Vector2.Distance(new Vector2Int(inStartX,inStartY),new Vector2Int(x,y)) <iRange)
                    {
                        lReturn.Add(arrGrid.GetGridObject(x, y));
                    }
                }
            }
        }
        return lReturn;
    }

    private List<TGridObject> CalculatePath(TGridObject inEndNode)
    {
        List<TGridObject> lReturn = new List<TGridObject>();
        lReturn.Add(inEndNode);
        TGridObject pCurrentNode = inEndNode;
        while(pCurrentNode.ICameFrom != null)
        {
            lReturn.Add(pCurrentNode.ICameFrom);
            pCurrentNode = pCurrentNode.ICameFrom;

        }
        lReturn.Reverse();
        return lReturn;
    }

    private int CalculateDistanceCost(TGridObject inA, TGridObject inB)
    {
        int iXDistance = Mathf.Abs(inA.x - inB.x);
        int iYDistance = Mathf.Abs(inA.y - inB.y);
        int iRemaining = Mathf.Abs(iXDistance - iYDistance);

        return MOVE_CORNER_COST * Mathf.Min(iXDistance, iYDistance) + MOVE_EDGE_COST * iRemaining;
    }

    private TGridObject GetLowestFCostNode(List<TGridObject> inPathNodeList)
    {
        TGridObject pLowestFCostNode = inPathNodeList[0];
        for (int i = 0; i< inPathNodeList.Count; i++)
        {
            if(inPathNodeList[i].iFCost < pLowestFCostNode.iFCost)
            {
                pLowestFCostNode = inPathNodeList[i];
            }
        }
        return pLowestFCostNode;
    }

    public List<TGridObject> GetNavNeighborsList(int inX, int inY, eMoveableType inMoveable = eMoveableType.None)
    {
        List<TGridObject> lReturn = new List<TGridObject>();
        Vector2Int vHome = new Vector2Int(inX, inY);
        TGridObject pHome = arrGrid.GetGridObject(vHome);
        
        for (int i = (int)eNavType.North; i <= (int)eNavType.NorthWest; i+=2)
        {
            if (arrGrid.IsValid(vHome + HeroTile.vNavType[i]))
            {
                if (IsClear(pHome,(eNavType)i,inMoveable))
                {
                    lReturn.Add(arrGrid.GetGridObject(vHome + HeroTile.vNavType[i]));
                }
            }
        }



        return lReturn;


    }

    private bool IsClear(TGridObject pHome, eNavType eNavDir, eMoveableType inMoveable = eMoveableType.None)
    {
        TGridObject pReturn = arrGrid.GetGridObject(pHome.x + (HeroTile.vNavType[(int)eNavDir]).x, pHome.y + (HeroTile.vNavType[(int)eNavDir].y));
        switch(eNavDir)
        {
            case eNavType.North:
                if(!pReturn.IsBlocked(eNavType.South, inMoveable) && !pHome.IsBlocked(eNavType.North, inMoveable))
                {
                    return true;
                }
                break;
            case eNavType.NorthEast:
                if (!pReturn.IsBlocked(eNavType.SouthWest, inMoveable) && !pHome.IsBlocked(eNavType.NorthEast, inMoveable))
                {
                    return true;
                }
                break;            
            case eNavType.East:
                if (!pReturn.IsBlocked(eNavType.West, inMoveable) && !pHome.IsBlocked(eNavType.East, inMoveable))
                {
                    return true;
                }
                break;
            case eNavType.SouthEast:
                if (!pReturn.IsBlocked(eNavType.NorthWest, inMoveable) && !pHome.IsBlocked(eNavType.SouthEast, inMoveable))
                {
                    return true;
                }
                break;
            case eNavType.South:
                if (!pReturn.IsBlocked(eNavType.North, inMoveable) && !pHome.IsBlocked(eNavType.South, inMoveable))
                {
                    return true;
                }
                break;
            case eNavType.SouthWest:
                if (!pReturn.IsBlocked(eNavType.NorthEast, inMoveable) && !pHome.IsBlocked(eNavType.SouthWest, inMoveable))
                {
                    return true;
                }
                break;
            case eNavType.West:
                if (!pReturn.IsBlocked(eNavType.East, inMoveable) && !pHome.IsBlocked(eNavType.West, inMoveable))
                {
                    return true;
                }
                break;
            case eNavType.NorthWest:
                if (!pReturn.IsBlocked(eNavType.SouthEast, inMoveable) && !pHome.IsBlocked(eNavType.NorthWest, inMoveable))
                {
                    return true;
                }
                break;
        }


        return false;
    }

}


