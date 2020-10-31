using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingPathFinding : MonoBehaviour
{
    //private PathFinding pPathFinding;
    private List<HeroTile> lLastPath;
    private eNavType ePenType;
    private int iWidth = 26;
    private int iHeight = 19;
    private BlackBocksGrid<HeroTile> arrGrid;
    private Vector2Int vStartPos;
    private List<HeroTile> lLastRange;
    // Start is called before the first frame update
    void Start()
    {
        arrGrid = new BlackBocksGrid<HeroTile>(iWidth, iHeight, 4, new Vector3(-51, -37), (BlackBocksGrid<HeroTile> g, int x, int y) => new HeroTile(g, x, y));
        new PathFinding<HeroTile>(arrGrid);
        vStartPos = new Vector2Int(0, 0);
        lLastRange = PathFinding<HeroTile>.Instance.FindPathWithRange(vStartPos.x, vStartPos.y, 12);

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vMouseWorldPosition = BlackBocks.GetMouseWorldPosition();
        if (Input.GetMouseButtonDown(0))
        {

            //Vector3 vMouseWorldPosition = BlackBocks.GetMouseWorldPosition();
            Vector2Int vEndPos = PathFinding<HeroTile>.Instance.GetGrid().GetGridPostion(vMouseWorldPosition);
            //List<PathNode> lPath = pPathFinding.FindPath(0, 0, vEndPos.x, vEndPos.y);
            lLastPath = new List<HeroTile>();
            //lLastPath = pPathFinding.FindPath(0, 0, vEndPos.x, vEndPos.y);
            if (arrGrid.IsValid(vEndPos))
            {
                lLastPath = PathFinding<HeroTile>.Instance.FindPath(vStartPos.x, vStartPos.y, vEndPos.x, vEndPos.y);
                arrGrid.TriggerGridObjectChanged(vEndPos);
            }
           
        }

        if (Input.GetMouseButtonDown(1))
        {
            //Vector3 vMouseWorldPosition = BlackBocks.GetMouseWorldPosition();
            arrGrid.GetGridObject(vMouseWorldPosition).eNav = ePenType;

        }


        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            ePenType = eNavType.North;
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            ePenType = eNavType.NorthEast;
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            ePenType = eNavType.East;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            ePenType = eNavType.SouthEast;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            ePenType = eNavType.South;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ePenType = eNavType.SouthWest;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            ePenType = eNavType.West;
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            ePenType = eNavType.NorthWest;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            ePenType = eNavType.Any;
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            ePenType = eNavType.All;
        }

        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
        {
            //Vector3 vMouseWorldPosition = BlackBocks.GetMouseWorldPosition();
            Vector2Int vNewPos = PathFinding<HeroTile>.Instance.GetGrid().GetGridPostion(vMouseWorldPosition);
            if (arrGrid.IsValid(vNewPos))
            {
                vStartPos = vNewPos;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }

        for (int x = 0; x < arrGrid.GetWidth(); x++)
        {
            for (int y = 0; y < arrGrid.GetHeight();y++)
            {
                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.North))
                {
                    Debug.DrawLine(
                        arrGrid.GetWorldPosition(x + .1f, y + .9f),
                        arrGrid.GetWorldPosition(x + .9f, y + .9f),
                        Color.red);
                }

                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.East))
                {
                    Debug.DrawLine(
                        arrGrid.GetWorldPosition(x + .9f, y + .1f),
                        arrGrid.GetWorldPosition(x + .9f, y + .9f),
                        Color.red);
                }

                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.South))
                {
                    Debug.DrawLine(
                        arrGrid.GetWorldPosition(x + .1f, y + .1f),
                        arrGrid.GetWorldPosition(x + .9f, y + .1f),
                        Color.red);
                }

                if (arrGrid.GetGridObject(x, y).IsBlocked(eNavType.West))
                {
                    Debug.DrawLine(
                        arrGrid.GetWorldPosition(x + .1f, y + .1f),
                        arrGrid.GetWorldPosition(x + .1f, y + .9f),
                        Color.red);
                }
            }
        }

        if (lLastRange != null)
        {
            for (int i = 0; i < lLastRange.Count - 1; i++)
            {
                Debug.DrawLine(arrGrid.GetWorldPosition(lLastRange[i].x + .1f, lLastRange[i].y + .1f),
                        arrGrid.GetWorldPosition(lLastRange[i].x + .9f, lLastRange[i].y + .9f),
                        Color.blue);
                
            }
        }

        if (lLastPath != null)
        {
            for (int i = 0; i < lLastPath.Count - 1; i++)
            {
                Debug.DrawLine(arrGrid.GetWorldPosition(lLastPath[i+0].x +.5f, lLastPath[i+0].y + .5f),
                        arrGrid.GetWorldPosition(lLastPath[i+1].x +.5f, lLastPath[i+1].y + .5f),
                        Color.green);
                
            }
        }

        Debug.Log("Position: " + vMouseWorldPosition.x + "," + vMouseWorldPosition.y + " CurrentPen:" + ePenType);
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

            Debug.Log("Loaded!");
        }
    }
}
