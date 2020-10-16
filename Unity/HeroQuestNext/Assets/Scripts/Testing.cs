using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    //private Grid<HeroTile> arrGrid;
    //[SerializeField] private HeroMapVisual hHeroMapVisual;

    private BlackBocksGrid<HeatMapGridObject> arrGrid;
    [SerializeField] private HeatMapVisual hHeatMapVisual;
    [SerializeField] private bool bDebugEnabled = true;
    //[SerializeField] private int iWidth;
    //[SerializeField] private int iHeight;
    //[SerializeField] private float iScale;
    //[SerializeField] private int iWidthOffset;
    //[SerializeField] private int iHeightOffset;
    // Start is called before the first frame update
    private void Start()
    {
        // arrGrid = new Grid<HeroTile>(iWidth, iHeight, iScale,new Vector3(iWidthOffset, iHeightOffset),() => new HeroTile());
        //hHeroMapVisual.SetGrid(arrGrid);
        //arrGrid = new Grid<HeatMapGridObject>(iWidth, iHeight, iScale, new Vector3(iWidthOffset, iHeightOffset), () => new HeatMapGridObject());
        arrGrid = new BlackBocksGrid<HeatMapGridObject>(26, 19, 4, new Vector3(-51, -37), (BlackBocksGrid<HeatMapGridObject> g,int x,int y) => new HeatMapGridObject(g,x,y));
        hHeatMapVisual.bDebugEnabled = bDebugEnabled;
        hHeatMapVisual.SetGrid(arrGrid);
        arrGrid.GetGridObject(1, 1).AddValue(10);

    }

    // Update is called once per frame
    void Update()
    {
        arrGrid.bDebugEnabled = bDebugEnabled;
        hHeatMapVisual.bDebugEnabled = bDebugEnabled;
        //hHeatMapVisual.bDebugEnabled = bDebugEnabled;
        if(Input.GetMouseButtonDown(0))
        {
            //int iCurrent = arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).GetSpriteIndex();
            //arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).IncrementSpriteIndex();

            //int iCurrent = arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).GetSpriteIndex();
            Vector3 vPosition = BlackBocks.GetMouseWorldPosition();
            HeatMapGridObject hHeatMapGridObject = arrGrid.GetGridObject(vPosition);
            if(hHeatMapGridObject != null)
            {
                hHeatMapGridObject.AddValue(10);

            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log("Value at: " + arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).GetSpriteIndex());
            Debug.Log("Value at: " + arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).GetValueNormalized());
        }

        //hHeatMapVisual.UpdateHeatMapVisuals();
    }

   
}

public class HeatMapGridObject
{
    private const int MIN_HEATMAP_VALUE = 0;
    private const int MAX_HEATMAP_VALUE = 100;
    public int iValue;
    public Vector2Int vPos;
    public BlackBocksGrid<HeatMapGridObject> gHostGrid;


    public HeatMapGridObject(
        BlackBocksGrid<HeatMapGridObject> inHostGrid,
        int inX,
        int inY)
    {
        this.gHostGrid = inHostGrid;
        this.vPos = new Vector2Int(inX, inY);
    }
    public void AddValue(int inValue)
    {
        iValue += inValue;
        Mathf.Clamp(iValue, MIN_HEATMAP_VALUE, MAX_HEATMAP_VALUE);
        gHostGrid.TriggerGridObjectChanged(vPos);
    }

    public float GetValueNormalized()
    {
        return (float)iValue / MAX_HEATMAP_VALUE;
    }

    public override string ToString()
    {
        return iValue.ToString();
    }
}