using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;

public class HeroMapVisual : MonoBehaviour
{
    private BlackBocksGrid<HeroTile> arrGrid;
    private Mesh mMesh;
    private bool bUpdateMesh;

    [SerializeField] public bool bDebugEnabled;

    [SerializeField] private Texture2D tHeroMapBigEmpty;
    [SerializeField] private Texture2D tHeroBaseTiles;
    [SerializeField] private Texture2D tHeroWallTiles;
    [SerializeField] private Texture2D tFurnitureTiles;
    [SerializeField] private Texture2D tFurnitureTilesMask;
    [SerializeField] private Texture2D tNavTiles;
    [SerializeField] private Texture2D tNavTilesMask;

    private Texture2D tHeroBasePixels;

    [SerializeField] private Texture2D tHeroMapMainSpriteSheet;
    //[SerializeField] private Texture2D tHeroMapMainSpriteSheetCheck;
    [SerializeField] private Material mHeroMapVisualMaterial;


    [System.Serializable]
    struct sSrcSpriteIndexAndSrcRect
    {
        public int iSrcSpriteIndex;
        public Rect vSrcUVPixels;
    }



    public class dSpriteIndexToUV : Dictionary<int, Rect> { }


    //[SerializeField] private sSrcSpriteIndexAndSrcRect[] arrBaseTileSrcSpritePixelRect;
    //[SerializeField] private sSrcSpriteIndexAndSrcRect[] arrWallTileSrcSpritePixelRect;
    //[SerializeField] private sSrcSpriteIndexAndSrcRect[] arrNavTileSrcSpritePixelRect;
    //[SerializeField] private sSrcSpriteIndexAndSrcRect[] arrFurnitureTileSrcSpritePixelRect;
    
    private sSrcSpriteIndexAndSrcRect[] arrBaseTileSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumRooms];
    private sSrcSpriteIndexAndSrcRect[] arrWallTileSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumRooms];
    private sSrcSpriteIndexAndSrcRect[] arrNavTileSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumnavTiles];
    private sSrcSpriteIndexAndSrcRect[] arrFurnitureTileSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumFurnitureTiles];



    private static int iNumRooms = System.Enum.GetValues(typeof(eRoomIDs)).Length;
    private static int iNumPages = System.Enum.GetValues(typeof(eSpritePages)).Length;
    private static int iNumnavTiles = System.Enum.GetValues(typeof(eNavTiles)).Length;
    private static int iNumFurnitureTiles = System.Enum.GetValues(typeof(eFurniture)).Length;

    private int iNumLayers = 6;
    //int[] iPageStartsAt= new int[iNumPages];
    
    private const int iWall = 15;
    private const int iBaseTile = 128;
    private static Vector2Int[] iCellXY;
    private Vector2Int vBaseTileReference = new Vector2Int(iBaseTile, iBaseTile);
    private Vector2Int vWallTileReference = new Vector2Int(iWall, iBaseTile);

    private Dictionary<(eSpritePages, int), Rect> dSpritePageAndIndexToMainUVRect = new Dictionary<(eSpritePages, int), Rect>();
    

    private enum eSpritePages
    {
        BaseTiles,
        WallTiles,
        NavTiles,
        FurnitureTiles,
    }

    private void Awake()
    {
        mMesh = new Mesh { };
        GetComponent<MeshFilter>().mesh = mMesh;
        //tHeroBasePixels = GetComponent<Texture2D>();


        //tHeroMapMainSpriteSheet = new Texture2D(iNumFurnitureTiles * vBaseTileReference.x, iBaseTile * iNumPages, TextureFormat.RGBA32, true);
        //Color[] cClear = tHeroMapMainSpriteSheet.GetPixels(0, 0, tHeroBasePixels.width, tHeroBasePixels.height);
        //for(int i =0; i < cClear.Length; i++)
        //{
        //    cClear[i] = Color.clear;
        //}
        //tHeroMapMainSpriteSheet.SetPixels(0, 0, tHeroMapMainSpriteSheet.width, tHeroMapMainSpriteSheet.height, cClear);

        tHeroMapMainSpriteSheet = new Texture2D(1, 1, TextureFormat.RGBA32, true);
        tHeroMapMainSpriteSheet.SetPixel(0, 0, Color.clear);
        tHeroMapMainSpriteSheet.Resize(iNumFurnitureTiles * vBaseTileReference.x, iBaseTile * iNumPages);



        tHeroMapMainSpriteSheet.Apply();
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;



        Color[] arrBaseTileColors = new Color[iNumRooms];
        Color[] arrWallTileColors = new Color[iNumRooms];
        Color[] arrNavTileColors = new Color[1];
        Color[] arrFurnitureTileColors = new Color[1];

        for (int i = 0; i < iNumRooms; i++)
        {
            arrBaseTileColors[i] = BlackBocks.GetRandomColor(new Color(.5f, .5f, 0, 1), new Color(.9f, .9f, 0, 1));
            arrWallTileColors[i] = BlackBocks.GetRandomColor(new Color(.1f, .1f, 0, 1), new Color(.4f, .4f, 0, 1));
            arrBaseTileSrcSpritePixelRect[i].iSrcSpriteIndex = i;
            arrBaseTileSrcSpritePixelRect[i].vSrcUVPixels = new Rect(0, 0, iBaseTile, iBaseTile);

            arrWallTileSrcSpritePixelRect[i].iSrcSpriteIndex = i;
            arrWallTileSrcSpritePixelRect[i].vSrcUVPixels = new Rect(0, 0, iBaseTile, iBaseTile);
        }
        arrNavTileColors[0] = Color.white;
        arrFurnitureTileColors[0] = Color.white;



        string sTilePath = AssetDatabase.GetAssetPath(tFurnitureTiles);
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(sTilePath);


        foreach (Object aObject in sprites)
        {
            if (aObject is Sprite)
            {
                Sprite aSprite = (Sprite)aObject;
                Debug.Log(aSprite.name.ToString());
                string[] aPath = aSprite.name.Split("."[0]);
                if (aPath.Length == 3)
                {
                    int i = int.Parse(aPath[2]);
                    switch (aPath[1])
                    {
                        case "NavTiles":
                            arrNavTileSrcSpritePixelRect[i].vSrcUVPixels = aSprite.rect;
                            arrNavTileSrcSpritePixelRect[i].iSrcSpriteIndex = i;
                            break;
                        case "FurnitureTiles":
                            arrFurnitureTileSrcSpritePixelRect[i].vSrcUVPixels = aSprite.rect;
                            arrFurnitureTileSrcSpritePixelRect[i].iSrcSpriteIndex = i;
                            break;
                    }
                }

            }
        }




        AddNewTilesToSpritePageAndIndexToMainUVRect(tHeroBaseTiles, tHeroBaseTiles, arrBaseTileSrcSpritePixelRect, vBaseTileReference, eSpritePages.BaseTiles, arrBaseTileColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        AddNewTilesToSpritePageAndIndexToMainUVRect(tHeroWallTiles, tHeroBaseTiles, arrWallTileSrcSpritePixelRect, vWallTileReference, eSpritePages.WallTiles, arrWallTileColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        AddNewTilesToSpritePageAndIndexToMainUVRect(tNavTiles, tNavTilesMask, arrNavTileSrcSpritePixelRect, vBaseTileReference, eSpritePages.NavTiles, arrNavTileColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        AddNewTilesToSpritePageAndIndexToMainUVRect(tFurnitureTiles, tFurnitureTilesMask, arrFurnitureTileSrcSpritePixelRect, vBaseTileReference, eSpritePages.FurnitureTiles, arrFurnitureTileColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        //tHeroMapMainSpriteSheet = new Texture2D(iNumFurnitureTiles * vBaseTileReference.x, iBaseTile * iNumPages, TextureFormat.RGBA32, true);
        Color[] cClear = tHeroMapMainSpriteSheet.GetPixels(0, 0, tHeroMapMainSpriteSheet.width, tHeroMapMainSpriteSheet.height);
        for (int i = 0; i < cClear.Length; i++)
        {
            if (cClear[i].a < 1)
            {
                cClear[i] = Color.clear;
                //cClear[i] = new Color(1,1,1,0);
            }
        }
        tHeroMapMainSpriteSheet.SetPixels(0, 0, tHeroMapMainSpriteSheet.width, tHeroMapMainSpriteSheet.height, cClear);
        tHeroMapMainSpriteSheet.Apply();




    }

    private void AddNewTilesToSpritePageAndIndexToMainUVRect(
        Texture2D inSpirteSheet,
        Texture2D inSpriteMask,
        sSrcSpriteIndexAndSrcRect[] inSrcSpriteIndexAndSrcRectArray,
        Vector2Int inRefSize,
        eSpritePages inSpritePage,
        Color[] inTint)
    {
        foreach (sSrcSpriteIndexAndSrcRect aSrcSpriteIndexAndSrcRect in inSrcSpriteIndexAndSrcRectArray)
        {
            //copy
            Color[] arrSrcPixels = inSpirteSheet.GetPixels(
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.x, 
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.y, 
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.width, 
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.height);
            Color[] arrMaskPixels = inSpriteMask.GetPixels(
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.x,
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.y,
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.width,
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.height);
            //color
            //BlackBocks.TintColorArray(arrSrcPixels, inTint[aSrcSpriteIndexAndSrcRect.iSrcSpriteIndex % inTint.Length]);
            BlackBocks.TintColorArrayInsideMask(arrSrcPixels, inTint[aSrcSpriteIndexAndSrcRect.iSrcSpriteIndex % inTint.Length], arrMaskPixels);
            //locate
            RectInt rMainUVRect = new RectInt(aSrcSpriteIndexAndSrcRect.iSrcSpriteIndex * inRefSize.x, (int)inSpritePage * inRefSize.y, inRefSize.x, inRefSize.y);
            //paste
            //tHeroMapMainSpriteSheet.SetPixels(rMainUVRect.x, rMainUVRect.y, rMainUVRect.width, rMainUVRect.height, arrSrcPixels);
            Color[] arrBasePixels = tHeroMapMainSpriteSheet.GetPixels(rMainUVRect.x, rMainUVRect.y, rMainUVRect.width, rMainUVRect.height);
            BlackBocks.MergeColorArray(arrBasePixels, arrSrcPixels);
            tHeroMapMainSpriteSheet.SetPixels(rMainUVRect.x, rMainUVRect.y, rMainUVRect.width, rMainUVRect.height, arrBasePixels);
            tHeroMapMainSpriteSheet.Apply();

            //add to UV dictionary
            dSpritePageAndIndexToMainUVRect[(inSpritePage, aSrcSpriteIndexAndSrcRect.iSrcSpriteIndex)] = new Rect((float)rMainUVRect.x / (float)tHeroMapMainSpriteSheet.width, (float)rMainUVRect.y / tHeroMapMainSpriteSheet.height, (float)rMainUVRect.width / tHeroMapMainSpriteSheet.width, (float)rMainUVRect.height / tHeroMapMainSpriteSheet.height);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //arrGrid = new Grid<int>(20, 20, 10f, Vector3.zero);

    }

    public void SetGrid(BlackBocksGrid<HeroTile> inGrid)
    {
        this.arrGrid = inGrid;

        inGrid.OnGridObjectChanged += Grid_OnGridObjectChanged;
    }

    private void Grid_OnGridObjectChanged(object sender, BlackBocksGrid<HeroTile>.OnGridObjectChangedEventArgs e)
    {
        bUpdateMesh = true;
    }

    private void LateUpdate()
    {
        if (bUpdateMesh)
        {
            UpdateHeroMapVisuals();
            bUpdateMesh = false;
        }
    }
    public void UpdateHeroMapVisuals()
    {
        BlackBocks.CreateEmptyMeshArrays(arrGrid.GetWidth() * arrGrid.GetHeight()* iNumLayers, out Vector3[] vVertices, out Vector2[] vUVs, out int[] iTriangles);
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
                int i = x * arrGrid.GetHeight() * iNumLayers + y* iNumLayers;
                //Debug.Log(x + "," + y);
                //add basetile
                Rect rUV = dSpritePageAndIndexToMainUVRect[(eSpritePages.BaseTiles, arrGrid.GetGridObject(x, y).GetBaseIndex() % arrBaseTileSrcSpritePixelRect.Length)];
                BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 0, arrGrid.GetWorldPosition(x, y) + vQuadSize * .5f, 0, vQuadSize, dSpritePageAndIndexToMainUVRect[(eSpritePages.BaseTiles, arrGrid.GetGridObject(x, y).GetBaseIndex() % arrBaseTileSrcSpritePixelRect.Length)]);
                if (arrGrid.GetGridObject(x, y).GetNavTileIndex() != eNavTiles.None)
                {
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 5, arrGrid.GetWorldPosition(x, y) + vQuadSize * .5f, arrGrid.GetGridObject(x,y).GetTileRotation(), vQuadSize*1f, dSpritePageAndIndexToMainUVRect[(eSpritePages.NavTiles, (int)arrGrid.GetGridObject(x, y).GetNavTileIndex() % arrNavTileSrcSpritePixelRect.Length)]);
                }

                if (arrGrid.GetGridObject(x, y).GetFurnitureIndex() != eFurniture.None)
                {
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 5, arrGrid.GetWorldPosition(x, y) + vQuadSize * .5f, arrGrid.GetGridObject(x, y).GetTileRotation(), vQuadSize*1f, dSpritePageAndIndexToMainUVRect[(eSpritePages.FurnitureTiles, (int)arrGrid.GetGridObject(x, y).GetFurnitureIndex() % arrFurnitureTileSrcSpritePixelRect.Length)]);
                }
                //add North if north
                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.North))
                {
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 1, arrGrid.GetWorldPosition(x, y) + vNorthWall, 90, vWallSize, dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                }

                //add East if East
                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.East))
                {
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 2, arrGrid.GetWorldPosition(x, y) + vEastWall, 0, vWallSize, dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                }

                //add South if South
                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.South))
                {
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 3, arrGrid.GetWorldPosition(x, y) + vSouthWall, 90, vWallSize, dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                }

                //add West if West
                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.West))
                {
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + 4, arrGrid.GetWorldPosition(x, y) + vWestWall, 0, vWallSize, dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
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



    public void Save()
    {
        Debug.Log("Saving");

        List<HeroTile.SaveObject> lTGridArrayList = new List<HeroTile.SaveObject>();

        for (int x = 0; x < arrGrid.GetWidth(); x++)
        {
            for (int y = 0; y < arrGrid.GetHeight(); y++)
            {
                HeroTile aHeroTile = arrGrid.GetGridObject(x, y);
                lTGridArrayList.Add(aHeroTile.GetSaveObject());
            }
        }

        SaveGridToList oSaveObject = new SaveGridToList { TGridObjectArray = lTGridArrayList.ToArray() };

        string strJSONSave = JsonUtility.ToJson(oSaveObject);
        SaveSystem.Save(strJSONSave, "GridData/");
        Debug.Log("Saved!");
    }
    
    public class SaveGridToList
    {
        public HeroTile.SaveObject[] TGridObjectArray;
    }

    public void Load()
    {
        SaveGridToList oSaveList = SaveSystem.LoadMostRecentObject<SaveGridToList>("GridData/");
        if (oSaveList != null)
        {
            Debug.Log("Loading!");
            foreach (HeroTile.SaveObject aSaveObject in oSaveList.TGridObjectArray)
            { 
                HeroTile aHeroTile = arrGrid.GetGridObject((int)aSaveObject.vPosition.x, (int)aSaveObject.vPosition.y);
                aHeroTile.Load(aSaveObject);       
            }

            UpdateHeroMapVisuals();
            Debug.Log("Loaded!");
        }
    }
}

