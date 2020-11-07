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
    void Start()
    {
        hHeroMapVisual.Load();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
