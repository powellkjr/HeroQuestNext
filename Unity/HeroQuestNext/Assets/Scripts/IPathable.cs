using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathable<TGridObject>
{
    int x { get; set; }
    int y { get; set; }
    int iGCost { get; set; }
    int iHCost { get; set; }
    int iFCost { get; set; }
    TGridObject ICameFrom { get; set; }

    eNavType eNav { get; set; }
    Vector2Int GetPosition();

    void CalculateFCost();
    bool IsBlocked(eNavType inNavType, eMoveableType inMoveable);
    List<(eMoveableType,int)> GetMoveable();
    void InitPathable(int x, int y);

 }
