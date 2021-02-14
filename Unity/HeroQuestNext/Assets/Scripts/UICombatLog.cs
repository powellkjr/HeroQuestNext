using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum eCombatRecordType
{
    Basic,
    Join,
    Removed,
    Combat,
    Roll,
    Action,
    Movement,
    Enviornment,
    Story,
    
}

public class UICombatLog : MonoBehaviour

{

    public static UICombatLog Instance { get; private set; }
    private TextMeshProUGUI tCombatLogTMP;

    private RectTransform rCombatLogBackground;


    private System.Func<string> getToolTipTitleTextFunc;
    private System.Func<string> getToolTipBodyTextFunc;
    private List<string> lRecords = new List<string>();
    private List<string> lArchive= new List<string>();
    [SerializeField] private Camera cUI_Camera;



    public static void AddRecord(string inString)
    {
        Instance.AddRecord_Local(eCombatRecordType.Basic, inString);
    }

    public static void AddRecord(eCombatRecordType inCombatRecordType, string inString)
    {
        Instance.AddRecord_Local(inCombatRecordType, inString);
    }
    private void AddRecord_Local(eCombatRecordType inRecordType, string inString)
    {
        string strRecordText = FormatText(inRecordType, inString);
        lRecords.Add(strRecordText);
        lArchive.Add(strRecordText);
        string strShowLog ="";
        string strTestLog = "";
        foreach(string aRecord in lRecords)
        {
            strTestLog += "\n"+ aRecord;
            //tCombatLogTMP.SetText(lRecords.ToString());
            //tCombatLogTMP.ForceMeshUpdate();
        }
        tCombatLogTMP.SetText("\n\n\n" + strTestLog);
        tCombatLogTMP.ForceMeshUpdate();
        float fHeight = tCombatLogTMP.GetRenderedValues(false).y;
        if(fHeight > rCombatLogBackground.rect.height)
        {
            lRecords.RemoveAt(0);
        }    
        

    }

    private string FormatText(eCombatRecordType inRecordType, string inString)
    {
        string strReturn = "";
        switch(inRecordType)
        {
            case eCombatRecordType.Join:
                strReturn = "<color=yellow>" + inString + " has entered the dungeon.</color>";
                break;
            case eCombatRecordType.Removed:
                strReturn = "<color=yellow>" + inString + "</color>";
                break;
            case eCombatRecordType.Roll:
                strReturn = "<color=#f0f0f0>" + inString + "</color>";
                break;
            default:
            strReturn = inString;
                break;
        }
        return strReturn;
    }
    private void Awake()
    {
        Instance = this;
        rCombatLogBackground = transform.Find("iCombatLogBackGround").GetComponent<RectTransform>();
        tCombatLogTMP = transform.Find("tCombatLogTMP").GetComponent<TextMeshProUGUI>();
        
    }
    private void OnDiceManager_MovementRoll(List<int> inRolls)
    {
        string strRecord = "Rolled for Movement (" + inRolls[0].ToString() + ")(" + inRolls[1].ToString() + ")";
        AddRecord_Local(eCombatRecordType.Roll, strRecord);
    }

    private void OnDiceManager_CombatRoll(int inAttackDice, int inDefendDice, int inTotal)
    {
        string strRecord = "Rolled for Attack (" + inTotal + ") Rolled (" + inAttackDice + ") attack with (" + inDefendDice + ") defence";
        AddRecord_Local(eCombatRecordType.Roll, strRecord);
    }

    private void GameCombatHandler_EnemyKilled(string inKilled, string inBy)
    {
        string strRecord = inKilled + "  has been killed by " + inBy;
        AddRecord_Local(eCombatRecordType.Removed, strRecord);
    }
    // Start is called before the first frame update
    void Start()
    {
        DiceManager.Instance.OnMovmentRoll += OnDiceManager_MovementRoll;
        DiceManager.Instance.OnRollForAttack += OnDiceManager_CombatRoll;
        GameCombatHandler.Instance.OnEnemyKilled += GameCombatHandler_EnemyKilled;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
