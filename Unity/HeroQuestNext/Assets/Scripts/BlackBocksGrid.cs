using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlackBocksGrid<TGridObject> 
{
    private int iWidth;
    private int iHeight;
    private TGridObject[,] arrGridArray;
    //private TextMesh[,] arrDebugTextArray;
    private float fCellSize;
    private Vector3 vOrigin;
    [SerializeField] public bool bDebugEnabled = true;

    public const int HEAT_MAP_MAX_VAL = 100;
    public const int HEAT_MAP_MIN_VAL = 0;

    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }
    public EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;

    public int GetWidth()
    {
        return iWidth;
    }

    public int GetHeight()
    {
        return iHeight;
    }

    public float GetCellSize()
    {
        return fCellSize;
    }


    public BlackBocksGrid(
        int inWidth,
        int inHeight,
        float inCellSize,
        Vector3 inOrigin,
        Func<BlackBocksGrid<TGridObject>,int, int,TGridObject> createDefaultObject)
    {
        int i = 0;
        this.iWidth = inWidth;
        this.iHeight = inHeight;
        this.fCellSize = inCellSize;
        this.vOrigin = inOrigin;

        
        arrGridArray = new TGridObject[iWidth, iHeight];
       // arrDebugTextArray = new TextMesh[iWidth, iHeight];


        for (int x = 0; x < arrGridArray.GetLength(0); x++)
        {
            for (int y = 0; y < arrGridArray.GetLength(1); y++)
            {
                arrGridArray[x, y] = createDefaultObject(this,x,y);
            }
        }

        if (bDebugEnabled)
        {
            Debug.Log(iWidth + " , " + iHeight);

            for (int x = 0; x < arrGridArray.GetLength(0); x++)
            {
                for (int y = 0; y < arrGridArray.GetLength(1); y++)
                {
                    Debug.Log("grid " + i + ", x=" + x + ", y=" + y);
                    i++;
                    //arrDebugTextArray[x, y] = BlackBocks.CreateWorldText(arrGridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(fCellSize, fCellSize) * .5f, 20);
                    //arrDebugTextArray[x, y] = new TextMesh();
                    //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100);
                    //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100);
                }
            }

            //Debug.DrawLine(GetWorldPosition(0, iWidth), GetWorldPosition(iHeight, iWidth), Color.white, 100);
            //Debug.DrawLine(GetWorldPosition(iHeight, 0), GetWorldPosition(iHeight, iWidth), Color.white, 100);
        }
        //SetValue(2, 1, 56);
    }

    public Vector3 GetWorldPosition(
        int inX,
        int inY)
    {
        return new Vector3(inX, inY) * fCellSize + vOrigin;
    }

    public Vector2Int GetGridPostion(Vector3 inWorldPostion)
    {
        return new Vector2Int(Mathf.FloorToInt((inWorldPostion - vOrigin).x / fCellSize), Mathf.FloorToInt((inWorldPostion - vOrigin).y / fCellSize));
    }

    public void SetValue(
        Vector2Int inXY,
        TGridObject inValue)
    {
        SetValue(inXY.x, inXY.y, inValue);
    }

    public void SetValue(
    Vector3 inXYZ,
    TGridObject inValue)
    {
        SetValue((int)inXYZ.x, (int)inXYZ.y, inValue);
    }
    public void SetValue(
        int inX,
        int inY,
        TGridObject inValue)
    {
        if (inX >= 0 && inY >= 0 && inX < iWidth && inY < iHeight)
        {
            arrGridArray[inX, inY] = inValue;
            //arrGridArray[inX, inY] = Mathf.Clamp((float)inValue, HEAT_MAP_MIN_VAL, HEAT_MAP_MAX_VAL);
            //arrDebugTextArray[inX, inY].text = arrGridArray[inX, inY]?.ToString();
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = inX, y = inY });
        }
    }

    public void TriggerGridObjectChanged(Vector2Int inPos)
    {
        TriggerGridObjectChanged(inPos.x, inPos.y);
    }
    public void TriggerGridObjectChanged(
        int inX,
        int inY)
    {
        //arrDebugTextArray [inX, inY].text = arrGridArray [inX, inY]?.ToString();
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = inX, y = inY});
    }

    public TGridObject GetGridObject(
        int inX, 
        int inY)
    {
        if (inX >= 0 && inY >= 0 && inX < iWidth && inY < iHeight)
        {
            return arrGridArray[inX, inY];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetGridObject(
        Vector2Int inGridPosistion)
    {
        return GetGridObject(inGridPosistion.x, inGridPosistion.y);
    }

    public TGridObject GetGridObject(
        Vector3 inWorldPosition)
    {
        Vector2Int vGridPosition = GetGridPostion(inWorldPosition);
        if (vGridPosition.x >= 0 && vGridPosition.y >= 0 && vGridPosition.x < iWidth && vGridPosition.y < iHeight)
        {
            return GetGridObject(vGridPosition);
        }
        else
        {
            return default(TGridObject);
        }
    }

    

}
