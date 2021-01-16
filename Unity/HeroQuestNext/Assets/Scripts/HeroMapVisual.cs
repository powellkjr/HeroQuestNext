using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;


public enum eSpritePages
{
    None,
    BaseTiles,
    FOGTiles,
    WallTiles,
    NavTiles,
    FurnitureTiles,
    PlayerTiles,
    EnemyTiles,
    MoveTiles,
    ActTiles,
    ScanTiles,
}
public class HeroMapVisual : MonoBehaviour
{
    private BlackBocksGrid<HeroTile> arrGrid;
    private GameCombatHandler cGameCombatHandler;
    private Mesh mMesh;
    private bool bUpdateMesh;
    public static HeroMapVisual Instance { get; private set; }

    [SerializeField] public bool bDebugEnabled;

    [SerializeField] private Texture2D tHeroMapBigEmpty;
    [SerializeField] private Texture2D tHeroBaseTiles;
    [SerializeField] private Texture2D tHeroWallTiles;
    [SerializeField] private Texture2D tFurnitureTiles;
    [SerializeField] private Texture2D tFurnitureTilesMask;
    [SerializeField] private Texture2D tNavTiles;
    [SerializeField] private Texture2D tNavTilesMask;
    [SerializeField] private Texture2D tPlayerIcons;
    [SerializeField] private Texture2D tPlayerIconsMask;

    [SerializeField] private Texture2D tVisualIcons;
    [SerializeField] private Texture2D tVisualIconsMask;

    private Texture2D tHeroBasePixels;

    [SerializeField] private Texture2D tHeroMapMainSpriteSheet;
    [SerializeField] private Texture2D tHeroMapMainSpriteSheetCheck;
    [SerializeField] private Material mHeroMapVisualMaterial;






    public class dSpriteIndexToUV : Dictionary<int, Rect> { }


    //[SerializeField] private sSrcSpriteIndexAndSrcRect[] arrBaseTileSrcSpritePixelRect;
    //[SerializeField] private sSrcSpriteIndexAndSrcRect[] arrWallTileSrcSpritePixelRect;
    //[SerializeField] private sSrcSpriteIndexAndSrcRect[] arrNavTileSrcSpritePixelRect;
    //[SerializeField] private sSrcSpriteIndexAndSrcRect[] arrFurnitureTileSrcSpritePixelRect;
    
    private sSrcSpriteIndexAndSrcRect[] arrBaseTileSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumRooms];
    private sSrcSpriteIndexAndSrcRect[] arrWallTileSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumRooms];
    private sSrcSpriteIndexAndSrcRect[] arrFOGTileSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumRooms];
    private sSrcSpriteIndexAndSrcRect[] arrNavTileSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumnavTiles];
    private sSrcSpriteIndexAndSrcRect[] arrFurnitureTileSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumFurnitureTiles];
    private sSrcSpriteIndexAndSrcRect[] arrPlayerIconsSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumPlayerIcons];
    private sSrcSpriteIndexAndSrcRect[] arrEnemyIconsSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumEnemyIcons];
    private sSrcSpriteIndexAndSrcRect[] arrMoveIconsSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumVisualIcons];
    private sSrcSpriteIndexAndSrcRect[] arrActIconsSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumVisualIcons];
    private sSrcSpriteIndexAndSrcRect[] arrScanIconsSrcSpritePixelRect = new sSrcSpriteIndexAndSrcRect[iNumVisualIcons];
    



    private static int iNumRooms = System.Enum.GetValues(typeof(eRoomIDs)).Length;
    private static int iNumPages = System.Enum.GetValues(typeof(eSpritePages)).Length;
    private static int iNumnavTiles = System.Enum.GetValues(typeof(eNavTiles)).Length;
    private static int iNumFurnitureTiles = System.Enum.GetValues(typeof(eFurniture)).Length;
    private static int iNumPlayerIcons = System.Enum.GetValues(typeof(ePlayers)).Length;
    private static int iNumEnemyIcons = System.Enum.GetValues(typeof(eMonsters)).Length;
    private static int iNumVisualIcons = System.Enum.GetValues(typeof(eVisualIcons)).Length;

    private enum eGameLayerType
    {
        BaseTile,
        North,
        East,
        South,
        West,
        Nav_Furniture,
        FogOfWar,
        MoveRange,
        ScanRange,
        Cursor

    }

    private enum eMoveableLayerType
    {
        RefIndex,
        MoveTarget,
        East,
        South,
        West,
        Nav_Furniture
    }
    private int iNumLayers = 10;
    //int[] iPageStartsAt= new int[iNumPages];
    
    private const int iWall = 15;
    private const int iBaseTile = 128;
    private static Vector2Int[] iCellXY;
    private Vector2Int vBaseTileReference = new Vector2Int(iBaseTile, iBaseTile);
    private Vector2Int vWallTileReference = new Vector2Int(iWall, iBaseTile);

    private Dictionary<(eSpritePages, int), Rect> dSpritePageAndIndexToMainUVRect = new Dictionary<(eSpritePages, int), Rect>();

    Vector3 vQuadSize;
    Vector3 vWallSize;
    Vector3 vNorthWall;
    Vector3 vEastWall;
    Vector3 vSouthWall;
    Vector3 vWestWall;
    Vector3[] vVertices;
    Vector2[] vUVs;
    int[] iTriangles;



    private void Awake()
    {
        Instance = this;
        mMesh = new Mesh { };
        GetComponent<MeshFilter>().mesh = mMesh;


        tHeroMapMainSpriteSheet = new Texture2D(iNumFurnitureTiles * vBaseTileReference.x, iBaseTile * iNumPages, TextureFormat.RGBA32, true);
        tHeroMapMainSpriteSheetCheck = new Texture2D(tHeroMapMainSpriteSheet.width, tHeroMapMainSpriteSheet.height, TextureFormat.RGBA32, true);
        Color[] cClear = tHeroMapMainSpriteSheet.GetPixels(0, 0, tHeroMapMainSpriteSheet.width, tHeroMapMainSpriteSheet.height);
        for (int i = 0; i < cClear.Length; i++)
        {
            cClear[i] = Color.clear;
        }
        tHeroMapMainSpriteSheet.SetPixels(0, 0, tHeroMapMainSpriteSheet.width, tHeroMapMainSpriteSheet.height, cClear);
        tHeroMapMainSpriteSheet.Apply();
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;



        Color[] arrBaseTileColors = new Color[iNumRooms];
        Color[] arrFOGColors = new Color[iNumRooms];
        Color[] arrWallTileColors = new Color[iNumRooms];
        
        Color[] arrNavTileColors = new Color[] { new Color(1, 1, 1, 0) };
        Color[] arrFurnitureTileColors = new Color[] { new Color(1, 1, 1, 0) };
        Color[] arrPlayerColors = new Color[] { new Color(1, 1, 1, 0) };
        Color[] arrEnemyColors = new Color[] { new Color(1, 1, 1, 0) };
        Color[] arrMoveColors = new Color[] { new Color(1, 1, 1, 0), new Color(0, 0, 1, 0), new Color(0, 0, 1, 0) };
        Color[] arrActColors = new Color[] { new Color(1, 1, 1, 0), new Color(1, 0, 0, 0), new Color(1, 0, 0, 0) };
        Color[] arrScanColors = new Color[] { new Color(1, 1, 1, 0), new Color(1, 1, 0, 0), new Color(1, 1, 0, 0) };

        for (int i = 0; i < iNumRooms; i++)
        {
            arrBaseTileColors[i] = BlackBocks.GetRandomColor(new Color(.5f, .5f, 0, 0), new Color(.9f, .9f, 0, 0));
            arrWallTileColors[i] = BlackBocks.GetRandomColor(new Color(.1f, .1f, 0, 0), new Color(.4f, .4f, 0, 0));
            arrFOGColors[i] = new Color(i * 1f / iNumRooms, i * 1f / iNumRooms, i * 1f / iNumRooms, 0);
            arrBaseTileSrcSpritePixelRect[i].iSrcSpriteIndex = i;
            arrBaseTileSrcSpritePixelRect[i].vSrcUVPixels = new Rect(0, 0, iBaseTile, iBaseTile);

            arrWallTileSrcSpritePixelRect[i].iSrcSpriteIndex = i;
            arrWallTileSrcSpritePixelRect[i].vSrcUVPixels = new Rect(0, 0, iBaseTile, iBaseTile);

            arrFOGTileSrcSpritePixelRect[i].iSrcSpriteIndex = i;
            arrFOGTileSrcSpritePixelRect[i].vSrcUVPixels = new Rect(0, 0, iBaseTile, iBaseTile);
        }



        string sTilePath = AssetDatabase.GetAssetPath(tFurnitureTiles);
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(sTilePath);

        foreach (Object aObject in sprites)
        {
            if (aObject is Sprite)
            {
                Sprite aSprite = (Sprite)aObject;
                //Debug.Log(aSprite.name.ToString());
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

        sTilePath = AssetDatabase.GetAssetPath(tPlayerIcons);
        sprites = AssetDatabase.LoadAllAssetsAtPath(sTilePath);

        foreach (Object aObject in sprites)
        {
            if (aObject is Sprite)
            {
                Sprite aSprite = (Sprite)aObject;
                //Debug.Log(aSprite.name.ToString());
                string[] aPath = aSprite.name.Split("."[0]);
                if (aPath.Length == 3)
                {
                    int i = int.Parse(aPath[2]);
                    switch (aPath[1])
                    {
                        case "PlayerTiles":
                            arrPlayerIconsSrcSpritePixelRect[i].vSrcUVPixels = aSprite.rect;
                            arrPlayerIconsSrcSpritePixelRect[i].iSrcSpriteIndex = i;
                            break;
                        case "EnemyTiles":
                            arrEnemyIconsSrcSpritePixelRect[i].vSrcUVPixels = aSprite.rect;
                            arrEnemyIconsSrcSpritePixelRect[i].iSrcSpriteIndex = i;
                            break;
                        case "VisualTiles":
                            arrMoveIconsSrcSpritePixelRect[i].vSrcUVPixels = aSprite.rect;
                            arrMoveIconsSrcSpritePixelRect[i].iSrcSpriteIndex = i;

                            arrActIconsSrcSpritePixelRect[i].vSrcUVPixels = aSprite.rect;
                            arrActIconsSrcSpritePixelRect[i].iSrcSpriteIndex = i;

                            arrScanIconsSrcSpritePixelRect[i].vSrcUVPixels = aSprite.rect;
                            arrScanIconsSrcSpritePixelRect[i].iSrcSpriteIndex = i;
                            break;
                    }
                }

            }
        }


        AddNewTilesToSpritePageAndIndexToMainUVRect(tHeroBaseTiles, tHeroBaseTiles, arrBaseTileSrcSpritePixelRect, vBaseTileReference, eSpritePages.BaseTiles, arrBaseTileColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        AddNewTilesToSpritePageAndIndexToMainUVRect(tHeroWallTiles, tHeroWallTiles, arrWallTileSrcSpritePixelRect, vBaseTileReference, eSpritePages.WallTiles, arrWallTileColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        AddNewTilesToSpritePageAndIndexToMainUVRect(tPlayerIcons, tPlayerIconsMask, arrPlayerIconsSrcSpritePixelRect, vBaseTileReference, eSpritePages.PlayerTiles, arrPlayerColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        AddNewTilesToSpritePageAndIndexToMainUVRect(tPlayerIcons, tPlayerIconsMask, arrMoveIconsSrcSpritePixelRect, vBaseTileReference, eSpritePages.MoveTiles, arrMoveColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        AddNewTilesToSpritePageAndIndexToMainUVRect(tPlayerIcons, tPlayerIconsMask, arrActIconsSrcSpritePixelRect, vBaseTileReference, eSpritePages.ActTiles, arrActColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        AddNewTilesToSpritePageAndIndexToMainUVRect(tPlayerIcons, tPlayerIconsMask, arrScanIconsSrcSpritePixelRect, vBaseTileReference, eSpritePages.ScanTiles, arrScanColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;


        AddNewTilesToSpritePageAndIndexToMainUVRect(tHeroBaseTiles, tHeroBaseTiles, arrFOGTileSrcSpritePixelRect, vBaseTileReference, eSpritePages.FOGTiles, arrFOGColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        //AddNewTilesToSpritePageAndIndexToMainUVRect(tHeroWallTiles, tHeroWallTiles, arrWallTileSrcSpritePixelRect, vWallTileReference, eSpritePages.WallTiles, arrWallTileColors);

        AddNewTilesToSpritePageAndIndexToMainUVRect(tNavTiles, tNavTilesMask, arrNavTileSrcSpritePixelRect, vBaseTileReference, eSpritePages.NavTiles, arrNavTileColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        AddNewTilesToSpritePageAndIndexToMainUVRect(tFurnitureTiles, tFurnitureTilesMask, arrFurnitureTileSrcSpritePixelRect, vBaseTileReference, eSpritePages.FurnitureTiles, arrFurnitureTileColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;


        AddNewTilesToSpritePageAndIndexToMainUVRect(tPlayerIcons, tPlayerIconsMask, arrEnemyIconsSrcSpritePixelRect, vBaseTileReference, eSpritePages.EnemyTiles, arrEnemyColors);
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        tHeroMapMainSpriteSheet.Apply();
        mHeroMapVisualMaterial.mainTexture = tHeroMapMainSpriteSheet;

        Color[] cCheck = tHeroMapMainSpriteSheet.GetPixels(0, 0, tHeroMapMainSpriteSheet.width, tHeroMapMainSpriteSheet.height);

        tHeroMapMainSpriteSheetCheck.SetPixels(0, 0, tHeroMapMainSpriteSheet.width, tHeroMapMainSpriteSheet.height, cCheck);
        tHeroMapMainSpriteSheetCheck.Apply();
    }

    // Start is called before the first frame update
    void Start()
    {
       // arrGrid = HeroMap.Instance.GetGrid();
        vQuadSize =  arrGrid.GetCellSize() * new Vector3(1f, 1f);
        vWallSize =  arrGrid.GetCellSize() * new Vector3((float)iWall / (float)iBaseTile, 1f);
        vNorthWall = arrGrid.GetCellSize() * new Vector3(0.5f, 1.0f - (float)iWall / (float)iBaseTile / 2.0f);
        vEastWall =  arrGrid.GetCellSize() * new Vector3(1.0f - (float)iWall / (float)iBaseTile / 2.0f, 0.5f);
        vSouthWall = arrGrid.GetCellSize() * new Vector3(0.5f, 0.0f + (float)iWall / (float)iBaseTile / 2.0f);
        vWestWall =  arrGrid.GetCellSize() * new Vector3(0.0f + (float)iWall / (float)iBaseTile / 2.0f, 0.5f);
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
            //This is empty no need
            //Color[] arrBasePixels = tHeroMapMainSpriteSheet.GetPixels(rMainUVRect.x, rMainUVRect.y, rMainUVRect.width, rMainUVRect.height);
            //BlackBocks.MergeColorArray(arrBasePixels, arrSrcPixels);
            //tHeroMapMainSpriteSheet.SetPixels(rMainUVRect.x, rMainUVRect.y, rMainUVRect.width, rMainUVRect.height, arrBasePixels);
            //Do try and center
            //Debug.Log(inSpirteSheet.name + "," +  inSpritePage + aSrcSpriteIndexAndSrcRect.iSrcSpriteIndex);
            tHeroMapMainSpriteSheet.SetPixels(
                (int)(rMainUVRect.x + (rMainUVRect.width - aSrcSpriteIndexAndSrcRect.vSrcUVPixels.width) / 2f), 
                (int)(rMainUVRect.y + (rMainUVRect.height - aSrcSpriteIndexAndSrcRect.vSrcUVPixels.height) / 2f), 
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.width,
                (int)aSrcSpriteIndexAndSrcRect.vSrcUVPixels.height,
                arrSrcPixels);
            tHeroMapMainSpriteSheet.Apply();

            //add to UV dictionary
            dSpritePageAndIndexToMainUVRect[(inSpritePage, aSrcSpriteIndexAndSrcRect.iSrcSpriteIndex)] = new Rect((float)rMainUVRect.x / (float)tHeroMapMainSpriteSheet.width, (float)rMainUVRect.y / tHeroMapMainSpriteSheet.height, (float)rMainUVRect.width / tHeroMapMainSpriteSheet.width, (float)rMainUVRect.height / tHeroMapMainSpriteSheet.height);
        }
    }


    public void SetGrid(BlackBocksGrid<HeroTile> inGrid)
    {
        this.arrGrid = inGrid;

        inGrid.OnGridObjectChanged += Grid_OnGridObjectChanged;
    }

    public void SetGameCombatHandler(GameCombatHandler inGameCombatHandler)
    {
        this.cGameCombatHandler = inGameCombatHandler;
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



        if (cGameCombatHandler != null)
        {

            BlackBocks.CreateEmptyMeshArrays(
                (arrGrid.GetWidth() * arrGrid.GetHeight()) * iNumLayers
                + (cGameCombatHandler.GetUsePlayer() + cGameCombatHandler.GetUseEnemies()) * iNumLayers
                + iNumLayers, out Vector3[] vVerticesT, out Vector2[] vUVsT, out int[] iTrianglesT);
            vVertices = vVerticesT;
            vUVs = vUVsT;
            iTriangles = iTrianglesT;
        }
        else
        {
            BlackBocks.CreateEmptyMeshArrays(
                (arrGrid.GetWidth() * arrGrid.GetHeight()) * iNumLayers
                + 1, out Vector3[] vVerticesT, out Vector2[] vUVsT, out int[] iTrianglesT);
            vVertices = vVerticesT;
            vUVs = vUVsT;
            iTriangles = iTrianglesT;
        }

        DrawBaseTiles();
        if (cGameCombatHandler != null)
        {
            foreach (Player aPlayer in cGameCombatHandler.lPlayerTeam)
            {
                int i = arrGrid.GetHeight() * arrGrid.GetWidth() * iNumLayers + 
                    aPlayer.GetMoveableKey().Item2 * iNumLayers;

                BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eMoveableLayerType.RefIndex,
                    arrGrid.GetWorldPosition(aPlayer.GetPos().x + .5f, aPlayer.GetPos().y + .5f)
                    , 0, vQuadSize * .9f,
                    dSpritePageAndIndexToMainUVRect[(eSpritePages.PlayerTiles, aPlayer.GetRefIndex())]);

                if (aPlayer.GetPlayerState() != ePlayerStateType.Idle)
                {
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eMoveableLayerType.MoveTarget,
                        arrGrid.GetWorldPosition(aPlayer.GetMoveTarget().x + .5f, aPlayer.GetMoveTarget().y + .5f)
                        , 0, vQuadSize * .9f,
                        dSpritePageAndIndexToMainUVRect[(eSpritePages.MoveTiles, (int)eVisualIcons.CornerSelector)]);


                    DrawMoveRange(aPlayer);
                    DrawActRange(aPlayer);
                    //DrawScanRange(aPlayer);
                    DrawCurrentPath(aPlayer);
                    

                }
            }


            foreach (Player aPlayer in cGameCombatHandler.lEnemyTeam)
            {
                if(arrGrid.GetGridObject(aPlayer.GetPosXY()).IsVisible())
                { 
                    int i = arrGrid.GetHeight() * arrGrid.GetWidth() * iNumLayers + 
                        cGameCombatHandler.GetUsePlayer() * iNumLayers + 
                        aPlayer.GetMoveableKey().Item2 * iNumLayers;
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eMoveableLayerType.RefIndex,
                        arrGrid.GetWorldPosition(aPlayer.GetPos().x + .5f, aPlayer.GetPos().y + .5f)
                        , 0, vQuadSize * .9f,
                        dSpritePageAndIndexToMainUVRect[(eSpritePages.EnemyTiles, aPlayer.GetRefIndex())]);

                    //if (aPlayer.GetPlayerState() != ePlayerStateType.Idle)
                    if (aPlayer.CanMove())
                    {
                        BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eMoveableLayerType.MoveTarget,
                            arrGrid.GetWorldPosition(aPlayer.GetMoveTarget().x + .5f, aPlayer.GetMoveTarget().y + .5f), 0, vQuadSize * .9f,
                            dSpritePageAndIndexToMainUVRect[(eSpritePages.MoveTiles, (int)eVisualIcons.CornerSelector)]);

                        DrawCurrentPath(aPlayer);
                    }
                }
            }
            DrawCursor();


        }

        mMesh.vertices = vVertices;
        mMesh.uv = vUVs;
        mMesh.triangles = iTriangles;
    }

    private void DrawBaseTiles()
    {
        for (int x = 0; x < arrGrid.GetWidth(); x++)
        {
            for (int y = 0; y < arrGrid.GetHeight(); y++)
            {
                int i = x * arrGrid.GetHeight() * iNumLayers + y * iNumLayers;
                //Debug.Log(x + "," + y);
                //add basetile
                
                if (!arrGrid.GetGridObject(x, y).IsVisible())
                {
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.FogOfWar,
                    arrGrid.GetWorldPosition(x, y) + vQuadSize * .5f, 0, vQuadSize * 1f,
                    dSpritePageAndIndexToMainUVRect[(eSpritePages.FOGTiles, 0)]);
                }
                else
                {
                    BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.BaseTile,
                        arrGrid.GetWorldPosition(x, y) + vQuadSize * .5f, 0, vQuadSize,
                        dSpritePageAndIndexToMainUVRect[(eSpritePages.BaseTiles, arrGrid.GetGridObject(x, y).GetBaseIndex() % arrBaseTileSrcSpritePixelRect.Length)]);
                    if (arrGrid.GetGridObject(x, y).GetFurnitureIndex() != eFurniture.None)
                    {
                        BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.Nav_Furniture,
                            arrGrid.GetWorldPosition(x, y) + vQuadSize * .5f, arrGrid.GetGridObject(x, y).GetTileRotation(), vQuadSize * 1f,
                            dSpritePageAndIndexToMainUVRect[(eSpritePages.FurnitureTiles, (int)arrGrid.GetGridObject(x, y).GetFurnitureIndex() % arrFurnitureTileSrcSpritePixelRect.Length)]);

                        //add North if north
                        if (arrGrid.GetGridObject(new Vector2Int(x,y) + BlackBocks.vNorth).GetFurnitureIndex() == eFurniture.None && 
                            arrGrid.GetGridObject(new Vector2Int(x, y) + BlackBocks.vNorth).IsBlocked(eNavType.South))
                        {
                            BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.North, 
                                arrGrid.GetWorldPosition(x, y) + vNorthWall, 90, vWallSize, 
                                dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                        }

                        //add East if East
                        if (arrGrid.GetGridObject(new Vector2Int(x, y) + BlackBocks.vEast).GetFurnitureIndex() == eFurniture.None &&
                            arrGrid.GetGridObject(new Vector2Int(x, y) + BlackBocks.vEast).IsBlocked(eNavType.West))
                        {
                            BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.East, 
                                arrGrid.GetWorldPosition(x, y) + vEastWall, 0, vWallSize, 
                                dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                        }

                        //add South if South
                        if (arrGrid.GetGridObject(new Vector2Int(x, y) + BlackBocks.vSouth).GetFurnitureIndex() == eFurniture.None &&
                            arrGrid.GetGridObject(new Vector2Int(x, y) + BlackBocks.vSouth).IsBlocked(eNavType.North))
        {
                            BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.South, 
                                arrGrid.GetWorldPosition(x, y) + vSouthWall, 90, vWallSize, 
                                dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                        }

                        //add West if West
                        if (arrGrid.GetGridObject(new Vector2Int(x, y) + BlackBocks.vWest).GetFurnitureIndex() == eFurniture.None &&
                            arrGrid.GetGridObject(new Vector2Int(x, y) + BlackBocks.vWest).IsBlocked(eNavType.East))
                        {
                            BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.West, 
                                arrGrid.GetWorldPosition(x, y) + vWestWall, 0, vWallSize, 
                                dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                        }
                        continue;
                    }

                    if (arrGrid.GetGridObject(x, y).GetNavTileIndex() != eNavTiles.None)
                    {
                        BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.Nav_Furniture,
                            arrGrid.GetWorldPosition(x, y) + vQuadSize * .5f, arrGrid.GetGridObject(x, y).GetTileRotation(), vQuadSize * 1f,
                            dSpritePageAndIndexToMainUVRect[(eSpritePages.NavTiles, (int)arrGrid.GetGridObject(x, y).GetNavTileIndex() % arrNavTileSrcSpritePixelRect.Length)]);
                    }

                    //add North if north
                    if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.North))// && arrGrid.GetGridObject(x, y).GetMoveable().Count == 0)
                    {
                        BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.North, 
                            arrGrid.GetWorldPosition(x, y) + vNorthWall, 90, vWallSize, 
                            dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                    }

                    //add East if East
                    if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.East))// && arrGrid.GetGridObject(x, y).GetMoveable().Count == 0)
                    {
                        BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.East, 
                            arrGrid.GetWorldPosition(x, y) + vEastWall, 0, vWallSize, 
                            dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                    }

                    //add South if South
                    if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.South))// && arrGrid.GetGridObject(x, y).GetMoveable().Count == 0)
                    {
                        BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.South, 
                            arrGrid.GetWorldPosition(x, y) + vSouthWall, 90, vWallSize, 
                            dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                    }

                    //add West if West
                    if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.West))// && arrGrid.GetGridObject(x, y).GetMoveable().Count == 0)
                    {
                        BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, i + (int)eGameLayerType.West, 
                            arrGrid.GetWorldPosition(x, y) + vWestWall, 0, vWallSize, 
                            dSpritePageAndIndexToMainUVRect[(eSpritePages.WallTiles, arrGrid.GetGridObject(x, y).GetWallIndex() % arrWallTileSrcSpritePixelRect.Length)]);
                    }
                }
            }
        }
    }
    private void DrawCursor()
    {
        if (GameCombatHandler.Instance.sCursor.Item1 != eSpritePages.None)
        {
            int j = GameCombatHandler.Instance.sCursor.Item3.x * arrGrid.GetHeight() * iNumLayers + GameCombatHandler.Instance.sCursor.Item3.y * iNumLayers;
            BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, j + (int)eGameLayerType.Cursor,
                arrGrid.GetWorldPosition(GameCombatHandler.Instance.sCursor.Item3.x + .5f, GameCombatHandler.Instance.sCursor.Item3.y + .5f), 0, vQuadSize * .9f,
                dSpritePageAndIndexToMainUVRect[(GameCombatHandler.Instance.sCursor.Item1, (int)GameCombatHandler.Instance.sCursor.Item2)]);
        }
    }

    private void DrawScanRange(Player inPlayer)
    {
        if (inPlayer.lScanRange != null)
        {
            foreach (HeroTile aTile in inPlayer.lScanRange)
            {
                int j = aTile.GetPosition().x * arrGrid.GetHeight() * iNumLayers + aTile.GetPosition().y * iNumLayers;
                BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, j + (int)eGameLayerType.ScanRange,
                  arrGrid.GetWorldPosition(aTile.GetPosition().x + .5f, aTile.GetPosition().y + .5f)
                  , 0, vQuadSize * .9f,
                  dSpritePageAndIndexToMainUVRect[(eSpritePages.ScanTiles, (int)eVisualIcons.OutlineSelector)]);
            }
        }
    }
    private void DrawCurrentPath(Player inPlayer)
    {
        if (inPlayer.lCurrentPath != null)
        {
            for (int iPathIndex = 0; iPathIndex < inPlayer.lCurrentPath.Count - 1; iPathIndex++)
            {
                Debug.DrawLine(arrGrid.GetWorldPosition(inPlayer.lCurrentPath[iPathIndex + 0].x + .5f, inPlayer.lCurrentPath[iPathIndex + 0].y + .5f),
                        arrGrid.GetWorldPosition(inPlayer.lCurrentPath[iPathIndex + 1].x + .5f, inPlayer.lCurrentPath[iPathIndex + 1].y + .5f),
                        Color.blue, 0, false);

            }
            if (inPlayer.lCurrentPath.Count > 0)
            {
                Debug.DrawLine(arrGrid.GetWorldPosition(inPlayer.GetPos().x + .5f, inPlayer.GetPos().y + .5f),
                            arrGrid.GetWorldPosition(inPlayer.lCurrentPath[0].x + .5f, inPlayer.lCurrentPath[0].y + .5f),
                            Color.blue, 0, false);
            }
        }
    }

    private void DrawMoveRange(Player inPlayer)
    {
        if (inPlayer.lMoveRange != null)
        {
            foreach (HeroTile aTile in inPlayer.lMoveRange)
            {
                int j = aTile.GetPosition().x * arrGrid.GetHeight() * iNumLayers + aTile.GetPosition().y * iNumLayers;
                BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, j + (int)eGameLayerType.MoveRange,
                    arrGrid.GetWorldPosition(aTile.GetPosition().x + .5f, aTile.GetPosition().y + .5f)
                    , 0, vQuadSize * .9f,
                    dSpritePageAndIndexToMainUVRect[(eSpritePages.MoveTiles, (int)eVisualIcons.OutlineSelector)]);

            }
        }
    }

    private void DrawActRange(Player inPlayer)
    {
        if (inPlayer.lActRange != null)
        {
            foreach (HeroTile aTile in inPlayer.lActRange)
            {
                int j = aTile.GetPosition().x * arrGrid.GetHeight() * iNumLayers + aTile.GetPosition().y * iNumLayers;
                BlackBocks.AddToMeshArrays(vVertices, vUVs, iTriangles, j + (int)eGameLayerType.MoveRange,
                    arrGrid.GetWorldPosition(aTile.GetPosition().x + .5f, aTile.GetPosition().y + .5f)
                    , 0, vQuadSize * .9f,
                    dSpritePageAndIndexToMainUVRect[(eSpritePages.ActTiles, (int)eVisualIcons.OutlineSelector)]);

            }
        }
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

    public void SetRoomFrom00to11(Vector2Int v00, Vector2Int v11, int inBaseIndex)
    {

        for (int x = v00.x; x <= v11.x; x++)
        {
            for (int y = v00.y; y <= v11.y; y++)
            {
                arrGrid.GetGridObject(x, y).SetRoomID((eRoomIDs)inBaseIndex);
            }
        }
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

