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
    public EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private int iWidth;
    private int iHeight;
    private float fCellSize;
    private Vector3 vOrigin;
    private TGridObject[,] arrGridArray;
    private bool bDebugEnabled = true;
    private readonly Vector3 GRID_VERSION = new Vector3(0, 0, 0);

    public BlackBocksGrid(
        int inWidth,
        int inHeight,
        float inCellSize,
        Vector3 inOrigin,
        Func<BlackBocksGrid<TGridObject>, int, int, TGridObject> createDefaultObject)
    {
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
                arrGridArray[x, y] = createDefaultObject(this, x, y);
            }
        }

        if (bDebugEnabled)
        {
            TextMesh[,] arrDebugTextArray = new TextMesh[iWidth, iHeight];

            for (int x = 0; x < arrGridArray.GetLength(0); x++)
            {
                for (int y = 0; y < arrGridArray.GetLength(1); y++)
                {
                    arrDebugTextArray[x, y] = BlackBocks.CreateWorldText(arrGridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(fCellSize, fCellSize) * .5f, 20);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 10);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 10);
                }
            }

            Debug.DrawLine(GetWorldPosition(0, iWidth), GetWorldPosition(iHeight, iWidth), Color.white, 10);
            Debug.DrawLine(GetWorldPosition(iHeight, 0), GetWorldPosition(iHeight, iWidth), Color.white, 10);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
            {
                arrDebugTextArray[eventArgs.x, eventArgs.y].text = arrGridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
        SaveSystem.Initialize();
    }


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

    public Vector3 GetGridVersion()
    {
        return GRID_VERSION;
    }

    public Vector3 GetWorldPosition(
        int inX,
        int inY)
    {
        return new Vector3(inX, inY) * fCellSize + vOrigin;
    }

    public Vector3 GetWorldPosition(
    float inX,
    float inY)
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
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = inX, y = inY });
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
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = inX, y = inY });
    }

    public TGridObject GetGridObject(
        int inX,
        int inY)
    {
        if (IsValid(inX, inY))
        {
            return arrGridArray[inX, inY];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public bool IsValid(Vector2Int inXY)
    {
        return IsValid(inXY.x, inXY.y);
    }
    public bool IsValid(int inX, int inY)
    {
        return (inX >= 0 && inY >= 0 && inX < iWidth && inY < iHeight);
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
        return GetGridObject(vGridPosition);
    }

    public List<TGridObject> GetEdgeList(int inX, int inY)
    {
        List<TGridObject> lReturn = new List<TGridObject>();
        if (IsValid(inX + 1, inY + 0))
        {
            lReturn.Add(GetGridObject(inX + 1, inY + 0));
        }
        if (IsValid(inX + 0, inY + 1))
        {
            lReturn.Add(GetGridObject(inX + 0, inY + 1));
        }
        if (IsValid(inX - 1, inY + 0))
        {
            lReturn.Add(GetGridObject(inX - 1, inY + 0));
        }
        if (IsValid(inX + 0, inY - 1))
        {
            lReturn.Add(GetGridObject(inX + 0, inY - 1));
        }

        return lReturn;
    }

    public List<TGridObject> GetCornerList(int inX, int inY)
    {
        List<TGridObject> lReturn = new List<TGridObject>();
        if (IsValid(inX + 1, inY + 1))
        {
            lReturn.Add(GetGridObject(inX + 1, inY + 1));
        }
        if (IsValid(inX + 1, inY - 1))
        {
            lReturn.Add(GetGridObject(inX + 1, inY - 1));
        }
        if (IsValid(inX - 1, inY + 1))
        {
            lReturn.Add(GetGridObject(inX - 1, inY + 1));
        }
        if (IsValid(inX - 1, inY - 1))
        {
            lReturn.Add(GetGridObject(inX - 1, inY - 1));
        }

        return lReturn;
    }

    public List<TGridObject> GetNeighborsList(int inX, int inY)
    {
        List<TGridObject> lReturn = new List<TGridObject>();
        foreach (TGridObject aGridObject in GetEdgeList(inX, inY))
        {
            lReturn.Add(aGridObject);
        }
        foreach (TGridObject aGridObject in GetCornerList(inX, inY))
        {
            lReturn.Add(aGridObject);
        }
        return lReturn;
    }


}
    
