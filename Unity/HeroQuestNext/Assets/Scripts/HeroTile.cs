using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public enum eNavType
{
    Any = 0,
    North = 1,
    NorthEast = 2,
    East = 3,
    SouthEast = 4,
    South = 5,
    SouthWest = 6,
    West = 7,
    NorthWest = 8,
    Spare1 = 9,
    All = 10,
    NorthSouth = 11,
    WestEast= 12,
}

public enum eNavText
{
    AA = 0,
    NN = 1,
    NE = 2,
    EE = 3,
    SE = 4,
    SS = 5,
    SW = 6,
    WW = 7,
    NW = 8,
    SP = 9,
    AL = 10,
    NS = 11,
    WE = 12,
}

public enum eMonsters
{
    None,
    Skeleton,
    Zombie,
    Mummy,
    Goblin,
    Orc,
    Fimir,
    ChaosWarrior,
    Gargoyle,
    ChaosScorcer,
}

public enum ePlayers
{
    None,
    Barbarian,
    Dwarf,
    Elf,
    Wizard,
}

public enum eVisualIcons
{
    None,
    OutlineSelector,
    CornerSelector,

}
public enum eNavTiles
{
    None,
    WallTileSingle,
    WallTileFallen,
    WallTileDouble0,
    WallTileDouble1,
    StartStair00,
    StartStair01,
    StartStair11,
    StartStair10,
    TrapDoorOpen,
    DoorNorthClosed,
    DoorNorthOpen,
    DoorSouthClosed,
    DoorSouthOpen,
    Pit,
    Rocks,
    Spear,

}
public enum eFurniture
{
    None,
    Chest,
    Shelf0,
    Shelf1,
    Shelf2,
    Fireplace0,
    Fireplace1,
    Fireplace2,
    BookShelf0,
    BookShelf1,
    BookShelf2,
    Arms0,
    Arms1,
    Arms2,
    Throne,
    Tomb00,
    Tomb01,
    Tomb11,
    Tomb21,
    Tomb20,
    Tomb10,
    Desk00,
    Desk01,
    Desk11,
    Desk21,
    Desk20,
    Desk10,
    Alter00,
    Alter01,
    Alter11,
    Alter21,
    Alter20,
    Alter10,    
    Table00,
    Table01,
    Table11,
    Table21,
    Table20,
    Table10, 
    Rack00,
    Rack01,
    Rack11,
    Rack21,
    Rack20,
    Rack10,
}

public enum eRoomIDs
{
    Hallways,

    Block00Rm0,
    Block00Rm1,
    Block00Rm2,
    Block00Rm3,
    Block00Rm4,
    Block00Rm5,

    Block01Rm0,
    Block01Rm1,
    Block01Rm2,
    Block01Rm3,
    Block01Rm4,

    Block02Rm0,
    Block02Rm1,
    Block02Rm2,
    Block02Rm3,
    Block02Rm4,

    Block03Rm0,
    Block03Rm1,
    Block03Rm2,
    Block03Rm3,
    Block03Rm4,

    Block05Rm1
}

public enum eMoveableType
{
    None,
    Furniture,
    Player,
    Enemy
}


public class HeroTile : IPathable<HeroTile>
{

    public static Vector2Int[] vNavType = 
    {
        new Vector2Int( 0,  0),
        new Vector2Int( 0,  1),
        new Vector2Int( 1,  1),
        new Vector2Int( 1,  0),
        new Vector2Int( 1, -1),
        new Vector2Int( 0, -1),
        new Vector2Int(-1,-1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1,1)
    };

    public static float GetRotationFromNavType(eNavType inNavType)
    {
        switch (inNavType)
        {
            case eNavType.North:
            case eNavType.NorthEast:
            case eNavType.East:
            case eNavType.SouthEast:
            case eNavType.South:
            case eNavType.SouthWest:
            case eNavType.West:
            case eNavType.NorthWest:
                return ((int)inNavType - 1) * 45;
            default:
                return 0;
        }
    }
    //private Vector2Int vPosition;


    public int x { get; set; }
    public int y { get; set; }
    public int iGCost { get; set; }
    public int iHCost { get; set; }
    public int iFCost { get; set; }

    public eNavType eNav { get; set; }

    public HeroTile ICameFrom { get; set; }

    public void GetPosition(out Vector2Int outPos) { outPos = new Vector2Int(x, y); }
    public Vector2Int GetPosition() { return new Vector2Int(x, y); }
    public void GetPosition(out Vector2 outPos) { outPos = new Vector2(x, y); }
    public void GetPosition(out Vector3 outPos) { outPos = new Vector3(x, y); }
    public void GetPosition(out int outX, out int outY) { outX = x; outY = y; }

    
    public bool IsBlocked(eNavType inNavType, eMoveableType inMoveable = eMoveableType.None)
    {
        //if the tile is solid
        if (eNav == eNavType.All)
        {
            return true;
        }

        //if it is currently occupied
        if(lMoveable.Count > 0)
        {
        //if the tile is not friendly
             if(lMoveable[0].Item1 != inMoveable)
            {
                return true;
            }
        }

        

        switch (inNavType)
        {
            case eNavType.North:
                return (eNav == eNavType.NorthWest) || (eNav == eNavType.North) || (eNav == eNavType.NorthEast) || (eNav == eNavType.NorthSouth);
            case eNavType.NorthWest:
                return (eNav == eNavType.North) || (eNav == eNavType.NorthWest) || (eNav == eNavType.West) || (eNav == eNavType.NorthSouth) || (eNav == eNavType.WestEast);
            case eNavType.West:
                return (eNav == eNavType.NorthWest) || (eNav == eNavType.West) || (eNav == eNavType.SouthWest) || (eNav == eNavType.WestEast);
            case eNavType.SouthWest:
                return (eNav == eNavType.West) || (eNav == eNavType.SouthWest) || (eNav == eNavType.South) || (eNav == eNavType.NorthSouth) || (eNav == eNavType.WestEast);
            case eNavType.South:
                return (eNav == eNavType.SouthWest) || (eNav == eNavType.South) || (eNav == eNavType.SouthEast) || (eNav == eNavType.NorthSouth);
            case eNavType.SouthEast:
                return (eNav == eNavType.South) || (eNav == eNavType.SouthEast) || (eNav == eNavType.East) || (eNav == eNavType.NorthSouth) || (eNav == eNavType.WestEast);
            case eNavType.East:
                return (eNav == eNavType.SouthEast) || (eNav == eNavType.East) || (eNav == eNavType.NorthEast) || (eNav == eNavType.WestEast);
            case eNavType.NorthEast:
                return (eNav == eNavType.East) || (eNav == eNavType.NorthEast) || (eNav == eNavType.North) || (eNav == eNavType.NorthSouth) || (eNav == eNavType.WestEast);
            case eNavType.All:
                return true;
            default:
            case eNavType.Spare1:
            case eNavType.Any:
                return false;
        }
    }
    public void CalculateFCost()
    {
        iFCost = iGCost + iHCost;
        //arrGrid.TriggerGridObjectChanged(new Vector2Int(x, y));
    }

    public void InitPathable(int inX, int inY) { x = inX; y = inY; }


    private int iSpriteIndex;
    private int iBaseIndex;
    private int iWallIndex;
    private eRoomIDs eRoomID;

    private eNavTiles eNavTileIndex;
    private eFurniture eFurnitureIndex;
    private float fTileRotation;
    private float fTileScale;
    public BlackBocksGrid<HeroTile> gHostGrid;
    private List<(eMoveableType,int)> lMoveable;

    public List<(eMoveableType, int)> GetMoveable()
    {
        return lMoveable;
    }

    public void SetMoveable((eMoveableType, int) inMoveable)
    {
        lMoveable.Clear();
        lMoveable.Add(inMoveable);
    }


    //public Vector2 GetPosition() { return vPosition; }
    public int GetSpriteIndex() { return iSpriteIndex; }

    public eRoomIDs GetRoomID() { return eRoomID; }
    public void SetRoomID(eRoomIDs inRoomID) { eRoomID = inRoomID; }
    public int GetBaseIndex() { return iBaseIndex; }
    public int GetWallIndex() { return iWallIndex; }

    public eNavTiles GetNavTileIndex() { return eNavTileIndex; }
    public eFurniture GetFurnitureIndex() { return eFurnitureIndex; }

   
    
    public void IncrementSpriteIndex()
    {
        iSpriteIndex++;
        iSpriteIndex %= System.Enum.GetValues(typeof(eNavType)).Length;
        eNav = (eNavType)iSpriteIndex;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }

    public void IncrementNavTileIndex()
    {
        int i = (int)eNavTileIndex;
        i++;
        i %= System.Enum.GetValues(typeof(eNavTiles)).Length;
        eNavTileIndex = (eNavTiles)i;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }

    public void IncrementFurnitureTileIndex()
    {
        int i = (int)eFurnitureIndex;
        i++;
        i %= System.Enum.GetValues(typeof(eFurniture)).Length;
        eFurnitureIndex = (eFurniture)i;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }

    public void SetSpriteIndex(int inSpriteIndex)
    {
        iSpriteIndex = inSpriteIndex;
        iSpriteIndex %= System.Enum.GetValues(typeof(eNavType)).Length;
        eNav = (eNavType)iSpriteIndex;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }

    public void SetBaseIndex(int inBaseIndex)
    {
        iBaseIndex = inBaseIndex;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }

    public void SetTileRotation(float inRotation)
    {
        fTileRotation = inRotation;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }

    public float GetTileRotation()
    {
        return fTileRotation;
    }

    public void SetWalIndex(int inWallIndex)
    {
        iWallIndex = inWallIndex;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }

    public void SetNavTileIndex(eNavTiles inNavTileIndex)
    {
        eNavTileIndex = inNavTileIndex;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }

    public void SetFurintureIndex(eFurniture inFurnitureTileIndex)
    {
        eFurnitureIndex = inFurnitureTileIndex;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }
    public void SetNav(eNavType inNavType)
    {
        iSpriteIndex = (int)inNavType;
        iSpriteIndex %= System.Enum.GetValues(typeof(eNavType)).Length;
        eNav = inNavType;
        gHostGrid.TriggerGridObjectChanged(GetPosition());
    }

    public HeroTile(
       BlackBocksGrid<HeroTile> inHostGrid,
       int inX = 0,
       int inY = 0,
       bool inWalkable = false,
       int inSpriteIndex = 0,
       int inBaseIndex = 0,
       int inWallIndex = 0,
       float inTileRotation = 0,
       float inTileScale = 1f,
       eNavType inNavType = eNavType.Any,
       eNavTiles inNavTileIndex = eNavTiles.None,
       eFurniture inFuritureTileIndex = eFurniture.None) 
    {
        //this.vPosition = new Vector2Int(inX, inY);
        this.x = inX;
        this.y = inY;
        this.gHostGrid = inHostGrid;
        
        
        this.iSpriteIndex = inSpriteIndex;
        this.eNav = inNavType;
        this.iBaseIndex = inBaseIndex;
        this.iWallIndex = inWallIndex;
        this.fTileRotation = inTileRotation;
        this.fTileScale = inTileScale;
        this.eNavTileIndex = inNavTileIndex;
        this.eFurnitureIndex = inFuritureTileIndex;
        this.lMoveable = new List<(eMoveableType, int)>();
        
    }

    public bool HasEntered((eMoveableType,int) inMoveable)
    {
        if (lMoveable.Count > 0)
        {
            if (lMoveable[0].Item1 == inMoveable.Item1)
            {
                //tile is occupied you can share
                lMoveable.Add(inMoveable);
                gHostGrid.TriggerGridObjectChanged(GetPosition());
                return true;
            }
        }
        else
        {
            //tile was empty, now is yours
            lMoveable.Add(inMoveable);
            gHostGrid.TriggerGridObjectChanged(GetPosition());
            return true;
        }
        //tile is not empty or you are not welcome;
        return false;
    }

    public bool HasLeft((eMoveableType,int) inMoveable)
    {
        if(lMoveable.Contains(inMoveable))
        {
            //if you are here you can leave
            lMoveable.Remove(inMoveable);
            gHostGrid.TriggerGridObjectChanged(GetPosition());
            return true;
        }
        //tile does not think you are here, leaving would be hard
        return false;
    }


    public override string ToString()
    {
        string strReturn = "";
        //return iSpriteIndex.ToString();
        //if (lMoveable.Count > 0)
        //{
        //    strReturn = "xy" + x + "," + y + ",";
        //    foreach ((eMoveableType, int) aMoveable in lMoveable)
        //    {
        //        strReturn += "\n" +aMoveable.Item1;
        //    }
        //}
        return strReturn;
    }

    
    public SaveObject GetSaveObject()
    {
        eMoveableType aMoveableItem1 = eMoveableType.None;
        int aMoveableItem2 = 0;
        if(lMoveable.Count !=0)
        {
            aMoveableItem1 = lMoveable[0].Item1;
            aMoveableItem2 = lMoveable[0].Item2;
        }
        return new SaveObject
        {
            vPosition = GetPosition(),
            //gHostGrid = inHostGrid;

            iSpriteIndex = iSpriteIndex,
            eRoomID = eRoomID,
            eMoveableItem1 = aMoveableItem1,
            eMoveableItem2 = aMoveableItem2,
            eNav = eNav,
            iBaseIndex = iBaseIndex,
            iWallIndex = iWallIndex,
            fTileRotation = fTileRotation,
            fTileScale = fTileScale,
            eNavTileIndex = eNavTileIndex,
            eFurnitureIndex = eFurnitureIndex,
        };
    }

    
    [System.Serializable]
    public class SaveObject
    {
        public Vector2Int vPosition;
        public eRoomIDs eRoomID;
        public int iSpriteIndex;
        public int iBaseIndex;
        public int iWallIndex;
        public eNavTiles eNavTileIndex;
        public eFurniture eFurnitureIndex;
        public eNavType eNav;
        public eMoveableType eMoveableItem1;
        public int eMoveableItem2;
        public float fTileRotation;
        public float fTileScale;
    }

    public void Load(SaveObject iSaveObject)
    {
        x = iSaveObject.vPosition.x;
        y = iSaveObject.vPosition.y;
        //gHostGrid = inHostGrid
        iSpriteIndex = iSaveObject.iSpriteIndex;
        eRoomID = iSaveObject.eRoomID;
        eNav = iSaveObject.eNav;
        iBaseIndex = iSaveObject.iBaseIndex;
        iWallIndex = iSaveObject.iWallIndex;
        fTileRotation = iSaveObject.fTileRotation;
        fTileScale = iSaveObject.fTileScale;
        eNavTileIndex = iSaveObject.eNavTileIndex;
        eFurnitureIndex = iSaveObject.eFurnitureIndex;
        lMoveable.Clear();
        if(iSaveObject.eMoveableItem1!= eMoveableType.None)
        {
            lMoveable.Add((iSaveObject.eMoveableItem1, iSaveObject.eMoveableItem2));
        }
                
}
}
