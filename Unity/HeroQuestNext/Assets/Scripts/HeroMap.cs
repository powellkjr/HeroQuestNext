using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMap : MonoBehaviour
{
   
    public static HeroMap Instance { get; private set; }
    private BlackBocksGrid<HeroTile> arrGrid;
    [SerializeField] private HeroMapVisual hHeroMapVisual;

    private int iWidth = 26;
    private int iHeight = 19;
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
        arrGrid = new BlackBocksGrid<HeroTile>(iWidth, iHeight, 4, new Vector3(-51, -37), (BlackBocksGrid<HeroTile> g, int x, int y) => new HeroTile(g, x, y));
        new PathFinding<HeroTile>(arrGrid);
        hHeroMapVisual.SetGrid(arrGrid);

    }

    public BlackBocksGrid<HeroTile> GetGrid()
    {
        return arrGrid;
    }

    public static int GetWidth()
    {
        return Instance.iWidth;
    }

    public static int GetHeight()
    {
        return Instance.iHeight;
    }

    public static HeroTile GetGridObject(int inX, int inY)
    {
        return Instance.arrGrid.GetGridObject(inX, inY);
    }

    public static HeroTile GetGridObject(Vector2Int inXY)
        {
            return Instance.arrGrid.GetGridObject(inXY.x, inXY.y);
        }
        public static List<HeroTile> GetNavNeighborsList(int inX, int inY)
    {
        return Instance.GetNavNeighborsList_Internal(inX, inY);
    }


    public static void TriggerGridObjectChanged(Vector2Int inXY)
    {
        Instance.arrGrid.TriggerGridObjectChanged(inXY.x, inXY.y);
    }
    public static void TriggerGridObjectChanged(int inX, int inY)
    {
        Instance.arrGrid.TriggerGridObjectChanged(inX, inY);
    }

    public static bool IsValid(int inX, int inY)
    {
        return Instance.arrGrid.IsValid(inX, inY);
    }

    public static bool IsValid(Vector2Int inXY)
    {
        return Instance.arrGrid.IsValid(inXY.x, inXY.y);
    }
    public List<HeroTile> GetNavNeighborsList_Internal(int inX, int inY)
    {
        List<HeroTile> lReturn = new List<HeroTile>();
        Vector2Int vHome = new Vector2Int(inX, inY);
        HeroTile pHome = arrGrid.GetGridObject(vHome);

        for (int i = (int)eNavType.North; i <= (int)eNavType.NorthWest; i += 2)
        {
            if (arrGrid.IsValid(vHome + HeroTile.vNavType[i]))
            {
                lReturn.Add(arrGrid.GetGridObject(vHome + HeroTile.vNavType[i]));
            }
        }

        return lReturn;


    }
    void Start()
    {
        hHeroMapVisual.Load();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
