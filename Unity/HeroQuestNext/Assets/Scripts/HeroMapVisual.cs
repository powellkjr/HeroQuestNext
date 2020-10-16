using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Serialization;
using UnityEngine;

public class HeroMapVisual : MonoBehaviour
{
    private BlackBocksGrid<HeroTile> arrGrid;
    private Mesh mMesh;
    private bool bUpdateMesh;

    [SerializeField] public bool bDebugEnabled;

    [SerializeField] private Texture2D tHeroMapBigEmpty;
    [SerializeField] private Texture2D tHeroBaseTile;
    [SerializeField] private Texture2D tHeroWallTile;

    [SerializeField] private Texture2D tHeroMapMainSpriteSheet;
    [SerializeField] private Material mHeroMapVisualMaterial;


    private int iPageWidth = 6;
    private int iPageHeight = 5;
    private int iPageOffset = 200;
    private const int iWall = 15;
    private const int iBaseTile = 128;
    private static Vector2Int[] iCellXY;
    private Vector2Int vBaseTileReference = new Vector2Int(iBaseTile, iBaseTile);
    private Vector2Int vWallTileReference = new Vector2Int(iWall, iBaseTile);
    


    private void Awake()
    {
        CacheCellXY();
        mMesh = new Mesh { };
        GetComponent<MeshFilter>().mesh = mMesh;
        
        tHeroMapMainSpriteSheet = new Texture2D(tHeroMapBigEmpty.width, tHeroMapBigEmpty.height, TextureFormat.RGBA32, true);

        Color[] cRoomColors = new Color[28];
        Color[] cWallColors = new Color[28];

        for (int i = 0; i < cRoomColors.Length;i++)
        {
            cRoomColors[i] = BlackBocks.GetRandomColor(new Color(128, 128, 128), new Color(212, 212, 212));
            cWallColors[i] = BlackBocks.GetRandomColor(new Color(64, 64, 64), new Color(128, 128, 128));
        }


        Color[] arrSpriteSheetBasePixels = tHeroMapBigEmpty.GetPixels(0, 0, tHeroMapBigEmpty.width, tHeroMapBigEmpty.height);
        tHeroMapMainSpriteSheet.SetPixels(0, 0, tHeroMapBigEmpty.width, tHeroMapBigEmpty.height, arrSpriteSheetBasePixels);
        tHeroMapMainSpriteSheet.Apply();

        //Color[] arrBasePixels = tHeroBaseTile.GetPixels(0, 0, tHeroBaseTile.width, tHeroBaseTile.height);
        Color[] arrBasePixels;
        for (int i =0; i < cRoomColors.Length; i++)
        {
            arrBasePixels = tHeroBaseTile.GetPixels(0, 0, vBaseTileReference.x, vBaseTileReference.y);
            BlackBocks.TintColorArray(arrBasePixels, cRoomColors[i]);
            tHeroMapMainSpriteSheet.SetPixels(vBaseTileReference.x * i, iPageOffset * 0, vBaseTileReference.x, vBaseTileReference.y, arrBasePixels);
            tHeroMapMainSpriteSheet.Apply();
        }
        //Color[] arrBasePixels = tHeroBaseTile.GetPixels(0, 0, iBaseTile, iBaseTile);
        //tHeroMapMainSpriteSheet.SetPixels(0, iPageOffset * 0, tHeroBaseTile.width, tHeroBaseTile.height, arrBasePixels);
        //tHeroMapMainSpriteSheet.Apply();

        Color[] arrWallPixels;
        for (int i = 0; i < cRoomColors.Length; i++)
        {
            arrWallPixels = tHeroBaseTile.GetPixels(0, 0, vWallTileReference.x, vWallTileReference.y);
            BlackBocks.TintColorArray(arrWallPixels, cWallColors[i]);
            tHeroMapMainSpriteSheet.SetPixels(vWallTileReference.x * i, iPageOffset * 1, vWallTileReference.x, vWallTileReference.y, arrWallPixels);
            tHeroMapMainSpriteSheet.Apply();
        }
        //Color[] arrWallPixels = tHeroWallTile.GetPixels(0, 0, tHeroWallTile.width, tHeroWallTile.height);
        //tHeroMapMainSpriteSheet.SetPixels(0, iPageOffset * 1, tHeroWallTile.width, tHeroWallTile.height, arrWallPixels);
        //tHeroMapMainSpriteSheet.Apply();






        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;
        //UpdateHeroMapVisuals();




    }

    private void CacheCellXY()
    {
        if (iCellXY != null) return;

        iCellXY = new Vector2Int[40];
        iCellXY[0] = new Vector2Int(1, 1);
        iCellXY[1] = new Vector2Int(1, 2);
        iCellXY[2] = new Vector2Int(2, 2);
        iCellXY[3] = new Vector2Int(2, 1);
        iCellXY[4] = new Vector2Int(2, 0);
        iCellXY[5] = new Vector2Int(1, 0);
        iCellXY[6] = new Vector2Int(0, 0);
        iCellXY[7] = new Vector2Int(0, 1);
        iCellXY[8] = new Vector2Int(0, 2);

        iCellXY[10] = new Vector2Int(3, 2);
        iCellXY[11] = new Vector2Int(3, 0);
        iCellXY[12] = new Vector2Int(3, 1);

        iCellXY[20] = new Vector2Int(0, 3);
        iCellXY[21] = new Vector2Int(1, 3);
        iCellXY[22] = new Vector2Int(2, 3);
        iCellXY[23] = new Vector2Int(3, 3);

        iCellXY[24] = new Vector2Int(4, 0);
        iCellXY[25] = new Vector2Int(4, 1);
        iCellXY[26] = new Vector2Int(4, 2);
        iCellXY[27] = new Vector2Int(4, 3);

        iCellXY[30] = new Vector2Int(0, 4);
        iCellXY[31] = new Vector2Int(1, 4);
        iCellXY[32] = new Vector2Int(2, 4);
        iCellXY[33] = new Vector2Int(3, 4);

        iCellXY[34] = new Vector2Int(5, 0);
        iCellXY[35] = new Vector2Int(5, 1);
        iCellXY[36] = new Vector2Int(5, 2);
        iCellXY[37] = new Vector2Int(5, 3);
    }
    // Start is called before the first frame update
    void Start()
    {
        //arrGrid = new Grid<int>(20, 20, 10f, Vector3.zero);

    }

    public void SetGrid(BlackBocksGrid<HeroTile> inGrid)
    {
        this.arrGrid = inGrid;

        inGrid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, BlackBocksGrid<HeroTile>.OnGridValueChangedEventArgs e)
    {
        //UpdateHeroMapVisuals();
        bUpdateMesh = true;
    }

    private void LateUpdate()
    {
        if (bUpdateMesh)
        {
            UpdateHeroMapVisuals();
            bUpdateMesh = false;
        }
        arrGrid.bDebugEnabled = bDebugEnabled;

    }
    public void UpdateHeroMapVisuals()
    {
        BlackBocks.CreateEmptyMeshArrays(arrGrid.GetWidth() * arrGrid.GetHeight()*5, out Vector3[] vVertices, out Vector2[] vUVs, out int[] iTriangles);
        Vector3 vQuadSize = arrGrid.GetCellSize() * new Vector3(1, 1);
        Vector3 vWallSize = arrGrid.GetCellSize() * new Vector3((float)iWall/(float)iBaseTile, 1);
        Vector3 vNorthWall = arrGrid.GetCellSize() * new Vector3( 0.5f,  1.0f - (float)iWall / (float)iBaseTile/2); 
        Vector3 vEastWall  = arrGrid.GetCellSize() * new Vector3( 1.0f - (float)iWall / (float)iBaseTile / 2,  0.5f);
        Vector3 vSouthWall = arrGrid.GetCellSize() * new Vector3( 0.5f,  0.0f + (float)iWall / (float)iBaseTile / 2);
        Vector3 vWestWall  = arrGrid.GetCellSize() * new Vector3( 0.0f + (float)iWall / (float)iBaseTile / 2,  0.5f);
        for (int x = 0; x < arrGrid.GetWidth(); x++)
        {
            for (int y = 0; y < arrGrid.GetHeight(); y++)
            {
                int i = x * arrGrid.GetHeight() *5 + y*5;
                Debug.Log(i + ":" + x + "," + y);
                Vector2 vUV00;
                Vector2 vUV11;

                //GetUVFromSpriteIndex(arrGrid.GetGridObject(x, y).GetSpriteIndex(), out vUV00, out vUV11);
                //GetUVPositionFromIndexAndPage(arrGrid.GetGridObject(x, y).GetSpriteIndex(),0, out vUV00, out vUV11);

                GetUVFromIndexAndPage(arrGrid.GetGridObject(x, y).GetBaseIndex(), 0, vBaseTileReference, out vUV00, out vUV11);
                BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 0, arrGrid.GetWorldPosition(x, y) + vQuadSize * .5f, 0, vQuadSize, vUV00, vUV11);
                if(arrGrid.GetGridObject(x, y).IsBlocked(eNavType.North))
                {
                    GetUVFromIndexAndPage(arrGrid.GetGridObject(x, y).GetWallIndex(), 1, vWallTileReference, out vUV00, out vUV11);
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 1, arrGrid.GetWorldPosition(x, y) + vNorthWall, 90, vWallSize, vUV00, vUV11);
                }

                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.East))
                {
                    GetUVFromIndexAndPage(arrGrid.GetGridObject(x, y).GetWallIndex(), 1, vWallTileReference, out vUV00, out vUV11);
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 2, arrGrid.GetWorldPosition(x, y) + vEastWall, 0, vWallSize, vUV00, vUV11);
                }

                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.South))
                {
                    GetUVFromIndexAndPage(arrGrid.GetGridObject(x, y).GetWallIndex(), 1, vWallTileReference, out vUV00, out vUV11);
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 3, arrGrid.GetWorldPosition(x, y) + vSouthWall, 90, vWallSize, vUV00, vUV11);
                }

                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.West))
                {
                    GetUVFromIndexAndPage(arrGrid.GetGridObject(x, y).GetWallIndex(), 1, vWallTileReference, out vUV00, out vUV11);
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 4, arrGrid.GetWorldPosition(x, y) + vWestWall, 0, vWallSize, vUV00, vUV11);
                }
            }
        }

        mMesh.vertices = vVertices;
        mMesh.uv = vUVs;
        mMesh.triangles = iTriangles;
    }
    // Update is called once per frame
    void Update()
    {
        //UpdateHeroMapVisuals();
    }



    public void CreateRoomFrom00to11(Vector2Int v00, Vector2Int v11, int inBaseIndex, int inWallIndex)
    {

        for (int x = v00.x; x <= v11.x; x++)
        {
            for (int y = v00.y; y <= v11.y; y++)
            {
                arrGrid.GetGridObject(x, y).SetNav(eNavType.Any);
                arrGrid.GetGridObject(x, y).SetBaseIndex(inBaseIndex);
                arrGrid.GetGridObject(x, y).SetWalIndex(inWallIndex);
            }
        }

        for (int x = v00.x; x <= v11.x; x++)
        {
            arrGrid.GetGridObject(x, v11.y).SetNav(eNavType.North); 
            arrGrid.GetGridObject(x, v00.y).SetNav(eNavType.South); 
        }
        for (int y = v00.y; y <= v11.y; y++)
        {
            arrGrid.GetGridObject(v11.x, y).SetNav(eNavType.East); 
            arrGrid.GetGridObject(v00.x, y).SetNav(eNavType.West); 
        }
        arrGrid.GetGridObject(v00.x, v00.y).SetNav(eNavType.SouthWest);
        arrGrid.GetGridObject(v00.x, v11.y).SetNav(eNavType.NorthWest);
        arrGrid.GetGridObject(v11.x, v11.y).SetNav(eNavType.NorthEast);
        arrGrid.GetGridObject(v11.x, v00.y).SetNav(eNavType.SouthEast);



    }


    public void GetUVFromIndexAndPage(
        int inBaseIndex,
        int inPage,
        Vector2Int inRefShape,
        out Vector2 vUV00,
        out Vector2 vUV11)
    {
        vUV00 = new Vector2(inRefShape.x * (inBaseIndex + 0), iPageOffset * inPage);
        vUV11 = new Vector2(inRefShape.x * (inBaseIndex + 1), iPageOffset * inPage + inRefShape.y);

        vUV00 /= tHeroMapMainSpriteSheet.width;
        vUV11 /= tHeroMapMainSpriteSheet.width;
    }


}

