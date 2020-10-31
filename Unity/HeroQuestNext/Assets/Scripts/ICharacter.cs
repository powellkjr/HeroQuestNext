using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{

    (eMoveableType, int) GetMoveableKey();
    Vector2Int GetPosXY();
    Vector2 GetPos();
    void SetPos(Vector2 inPos);
    void SetVelocity(Vector2 inVelocity);

}
