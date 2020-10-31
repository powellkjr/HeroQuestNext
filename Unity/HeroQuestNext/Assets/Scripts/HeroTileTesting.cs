using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTileTesting : MonoBehaviour
{
    //private Grid<HeroTile> arrGrid;
    //[SerializeField] private HeroMapVisual hHeroMapVisual;

    private BlackBocksGrid<HeroTile> arrGrid;
    [SerializeField] private HeroMapVisual hHeroMapVisual;
    [SerializeField] private bool bDebugEnabled = true;
    //[SerializeField] private int iWidth;
    //[SerializeField] private int iHeight;
    private List<HeroTile> lLastPath;
    private List<HeroTile> lLastRange;
    private Vector2Int vStartPos;
    private int iWidth = 26;
    private int iHeight = 19;
    private enum ePenModeType
    {
        NavIndex,NavTiles,FurnitureTiles
    }
    private int iCurrentPen = 0;
    private ePenModeType ePenMode;
    private float fPenRotation = 0;
    //[SerializeField] private float iScale;
    //[SerializeField] private int iWidthOffset;
    //[SerializeField] private int iHeightOffset;
    // Start is called before the first frame update

    private void Start()
    {
        // arrGrid = new Grid<HeroTile>(iWidth, iHeight, iScale,new Vector3(iWidthOffset, iHeightOffset),() => new HeroTile());
        //hHeroMapVisual.SetGrid(arrGrid);
        //arrGrid = new Grid<HeatMapGridObject>(iWidth, iHeight, iScale, new Vector3(iWidthOffset, iHeightOffset), () => new HeatMapGridObject());
        arrGrid = new BlackBocksGrid<HeroTile>(iWidth, iHeight, 4, new Vector3(-51, -37), (BlackBocksGrid<HeroTile> g, int x, int y) => new HeroTile(g, x, y));
        new PathFinding<HeroTile>(arrGrid);
        hHeroMapVisual.bDebugEnabled = bDebugEnabled;
        hHeroMapVisual.SetGrid(arrGrid);

        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(0, 0), new Vector2Int(iWidth-1, iHeight-1), 0, 0);

        for (int x = 0; x < iWidth; x++)
        {
            arrGrid.GetGridObject(x, 0).SetNav(eNavType.NorthSouth);
            arrGrid.GetGridObject(x, 6).SetNav(eNavType.NorthSouth);
            arrGrid.GetGridObject(x, 9).SetNav(eNavType.NorthSouth);
            arrGrid.GetGridObject(x, 12).SetNav(eNavType.NorthSouth);
            arrGrid.GetGridObject(x, 18).SetNav(eNavType.NorthSouth);
        }

        for (int y = 0; y < iHeight; y++)
        {
            arrGrid.GetGridObject(0, y).SetNav(eNavType.WestEast);
            arrGrid.GetGridObject(9, y).SetNav(eNavType.WestEast);
            arrGrid.GetGridObject(12, y).SetNav(eNavType.West);
            arrGrid.GetGridObject(13, y).SetNav(eNavType.East);
            arrGrid.GetGridObject(16, y).SetNav(eNavType.WestEast);
            arrGrid.GetGridObject(25, y).SetNav(eNavType.WestEast);
        }

        arrGrid.GetGridObject(0, 18).SetNav(eNavType.NorthWest);
        arrGrid.GetGridObject(25, 18).SetNav(eNavType.NorthEast);
        arrGrid.GetGridObject(25, 0).SetNav(eNavType.SouthEast);
        arrGrid.GetGridObject(0, 0).SetNav(eNavType.SouthWest);

        arrGrid.GetGridObject(9, 12).SetNav(eNavType.NorthWest);
        arrGrid.GetGridObject(16, 12).SetNav(eNavType.NorthEast);
        arrGrid.GetGridObject(16, 6).SetNav(eNavType.SouthEast);
        arrGrid.GetGridObject(9, 6).SetNav(eNavType.SouthWest);

        arrGrid.GetGridObject(9, 18).SetNav(eNavType.NorthSouth);
        arrGrid.GetGridObject(16, 18).SetNav(eNavType.NorthSouth);
        arrGrid.GetGridObject(16, 0).SetNav(eNavType.NorthSouth);
        arrGrid.GetGridObject(9, 0).SetNav(eNavType.NorthSouth);

        arrGrid.GetGridObject(12, 12).SetNav(eNavType.South);
        arrGrid.GetGridObject(13, 12).SetNav(eNavType.South);
        arrGrid.GetGridObject(12, 6).SetNav(eNavType.North);
        arrGrid.GetGridObject(13, 6).SetNav(eNavType.North);

        arrGrid.GetGridObject(12, 0).SetNav(eNavType.South);
        arrGrid.GetGridObject(13, 0).SetNav(eNavType.South);
        arrGrid.GetGridObject(12, 18).SetNav(eNavType.North);
        arrGrid.GetGridObject(13, 18).SetNav(eNavType.North);

        arrGrid.GetGridObject(9, 9).SetNav(eNavType.East);
        arrGrid.GetGridObject(16, 9).SetNav(eNavType.West);
        arrGrid.GetGridObject(25, 9).SetNav(eNavType.East);
        arrGrid.GetGridObject(0, 9).SetNav(eNavType.West);



        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(1, 15), new Vector2Int(4, 17), 1, 1);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(5, 15), new Vector2Int(8, 17), 2, 2);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(9, 13), new Vector2Int(11, 17), 3, 3);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(1, 10), new Vector2Int(4, 14), 4, 4);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(5, 10), new Vector2Int(8, 14), 5, 5);

        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(14, 13), new Vector2Int(16, 17), 6, 6);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(17, 14), new Vector2Int(20, 17), 7, 7);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(21, 14), new Vector2Int(24, 17), 8, 8);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(17, 10), new Vector2Int(20, 13), 9, 9);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(21, 10), new Vector2Int(24, 13), 10, 10);


        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(10, 7), new Vector2Int(15, 11), 11, 11);

        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(1, 1), new Vector2Int(4, 4), 12, 12);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(1, 5), new Vector2Int(4, 8), 13, 13);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(5, 6), new Vector2Int(6, 8), 14, 14);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(7, 6), new Vector2Int(8, 8), 15, 15);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(5, 1), new Vector2Int(8, 5), 16, 16);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(9, 1), new Vector2Int(11, 5), 17, 17);

        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(17, 5), new Vector2Int(20, 8), 18, 18);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(21, 5), new Vector2Int(24, 8), 19, 19);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(14, 1), new Vector2Int(17, 5), 20, 20);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(18, 1), new Vector2Int(20, 4), 21, 21);
        hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(21, 1), new Vector2Int(24, 4), 22, 23);

        //hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(0, 18), new Vector2Int(25, 18), iRoomBase, iRoomWall);
        //hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(21, 5), new Vector2Int(24, 8), iRoomBase, iRoomWall);
        //hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(14, 1), new Vector2Int(17, 5), iRoomBase, iRoomWall);
        //hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(18, 1), new Vector2Int(20, 4), iRoomBase, iRoomWall);
        //hHeroMapVisual.CreateRoomFrom00to11(new Vector2Int(21, 1), new Vector2Int(24, 4), iRoomBase, iRoomWall);


        arrGrid.GetGridObject(17, 6).SetNav(eNavType.SouthWest);
        arrGrid.GetGridObject(18, 5).SetNav(eNavType.SouthWest);




        // hHeroMapVisual.UpdateHeroMapVisuals();

    }

    // Update is called once per frame
    void Update()
    {

        lLastRange = PathFinding<HeroTile>.Instance.FindPathWithRange(vStartPos.x, vStartPos.y, 12);
        Vector3 vPosition = BlackBocks.GetMouseWorldPosition();
        //hHeatMapVisual.bDebugEnabled = bDebugEnabled;
        if (Input.GetMouseButtonDown(0))
        {
            //int iCurrent = arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).GetSpriteIndex();
            //arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).IncrementSpriteIndex();

            //int iCurrent = arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).GetSpriteIndex();
            HeroTile hHeroMapGridObject = arrGrid.GetGridObject(vPosition);
            if (hHeroMapGridObject != null)
            {
                switch (ePenMode)
                {
                    case ePenModeType.NavIndex:
                        //hHeroMapGridObject.IncrementSpriteIndex();
                        hHeroMapGridObject.SetBaseIndex(iCurrentPen%= System.Enum.GetValues(typeof(eNavType)).Length);
                        //iCurrentPen = hHeroMapGridObject.GetSpriteIndex();
                        break;
                    case ePenModeType.NavTiles:
                        //hHeroMapGridObject.IncrementNavTileIndex();
                        iCurrentPen %= System.Enum.GetValues(typeof(eNavTiles)).Length;
                        hHeroMapGridObject.SetNavTileIndex((eNavTiles)iCurrentPen);
                        hHeroMapGridObject.SetTileRotation(fPenRotation);
                        break;
                    case ePenModeType.FurnitureTiles:
                        iCurrentPen %= System.Enum.GetValues(typeof(eFurniture)).Length;
                        hHeroMapGridObject.SetFurintureIndex((eFurniture)iCurrentPen);
                        hHeroMapGridObject.SetTileRotation(fPenRotation);
                        break;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            //Debug.Log("Value at: " + arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).GetSpriteIndex());
            Debug.Log("Value at: " + arrGrid.GetGridObject(vPosition).GetPosition() + " :" + arrGrid.GetGridObject(BlackBocks.GetMouseWorldPosition()).GetSpriteIndex());
            ++iCurrentPen;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ePenMode = ePenModeType.FurnitureTiles;
            iCurrentPen = 0;
            fPenRotation = 0;
}

        if (Input.GetKeyDown(KeyCode.N))
        {
            ePenMode = ePenModeType.NavTiles;
            iCurrentPen = 0;
            fPenRotation = 0;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ePenMode = ePenModeType.NavIndex;
            iCurrentPen = 0;
            fPenRotation = 0;
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            fPenRotation = HeroTile.GetRotationFromNavType(eNavType.SouthWest);
            Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            fPenRotation = HeroTile.GetRotationFromNavType(eNavType.South);
            Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            fPenRotation = HeroTile.GetRotationFromNavType(eNavType.SouthEast);Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
            Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            fPenRotation = HeroTile.GetRotationFromNavType(eNavType.West);
            Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            fPenRotation = HeroTile.GetRotationFromNavType(eNavType.Any);
            Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            fPenRotation = HeroTile.GetRotationFromNavType(eNavType.East);
            Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            fPenRotation = HeroTile.GetRotationFromNavType(eNavType.NorthWest);
            Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            fPenRotation = HeroTile.GetRotationFromNavType(eNavType.North);
            Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            fPenRotation = HeroTile.GetRotationFromNavType(eNavType.NorthEast);
            Debug.Log("Pen:" + iCurrentPen + " Rotation: " + fPenRotation);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            arrGrid.GetGridObject(vPosition).eNav = eNavType.All;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            hHeroMapVisual.Save();
        }
        
        if(Input.GetKeyDown(KeyCode.L))
        {
            hHeroMapVisual.Load();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            //Vector3 vMouseWorldPosition = BlackBocks.GetMouseWorldPosition();
            Vector2Int vEndPos = PathFinding<HeroTile>.Instance.GetGrid().GetGridPostion(vPosition);
            //List<PathNode> lPath = pPathFinding.FindPath(0, 0, vEndPos.x, vEndPos.y);
            lLastPath = new List<HeroTile>();
            //lLastPath = pPathFinding.FindPath(0, 0, vEndPos.x, vEndPos.y);
            if (arrGrid.IsValid(vEndPos))
            {
                lLastPath = PathFinding<HeroTile>.Instance.FindPath(vStartPos.x, vStartPos.y, vEndPos.x, vEndPos.y);
                arrGrid.TriggerGridObjectChanged(vEndPos);
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
        {
            //Vector3 vMouseWorldPosition = BlackBocks.GetMouseWorldPosition();
            Vector2Int vNewPos = PathFinding<HeroTile>.Instance.GetGrid().GetGridPostion(vPosition);
            if (arrGrid.IsValid(vNewPos))
            {
                vStartPos = vNewPos;
            }
        }


        if (lLastPath != null)
        {
            for (int i = 0; i < lLastPath.Count - 1; i++)
            {
                Debug.DrawLine(arrGrid.GetWorldPosition(lLastPath[i + 0].x + .5f, lLastPath[i + 0].y + .5f),
                        arrGrid.GetWorldPosition(lLastPath[i + 1].x + .5f, lLastPath[i + 1].y + .5f),
                        Color.green);

            }
        }

        //if (lLastRange != null)
        //{
        //    for (int i = 0; i < lLastRange.Count; i++)
        //    {
        //        Debug.DrawLine(arrGrid.GetWorldPosition(lLastRange[i].x + .1f, lLastRange[i].y + .1f),
        //                arrGrid.GetWorldPosition(lLastRange[i].x + .9f, lLastRange[i].y + .9f),
        //                Color.blue);

        //    }
        //}


    }


}

