using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum eCharacterSheetDataType
{
    Name,
    Character,
    Quest,
    MaxBodyPoints,
    MaxMindPoints,
    CurrBodyPoints,
    CurrMindPoints,
    Equipment
}
public class UI_CharacterSheet : MonoBehaviour
{



    public static UI_CharacterSheet Instance { get; private set; }
    private TextMeshProUGUI tCharacterSheet_Name;
    private TextMeshProUGUI tCharacterSheet_Character;
    private TextMeshProUGUI tCharacterSheet_BodyPoints;
    private TextMeshProUGUI tCharacterSheet_MindPoints;
    private TMP_Dropdown dQuestDropDown;
    private RectTransform rCharacterSheetBackground;

    private List<string> lQuests = new List<string>();
    private List<EquipmentData> lStored = new List<EquipmentData>();
    private List<EquipmentData> lEquipped = new List<EquipmentData>();

    private int iMaxBodyPoints;
    private int iMaxMindPoints;
    private int iCurrBodyPoints;
    private int iCurrMindPoints;

    private string strName;
    private string strCharacter;

    [SerializeField] private Camera cUI_Camera;



    public static void UpdateRecord(eCharacterSheetDataType inUpdateData, string inValue)
    {
        Instance.UpdateRecord_Local(inUpdateData, inValue);

    }
        public static void InitCharacterSheet(string inName, string inCharacter, int inMaxBodyPoints, int inMaxMindPoints)
        {
            Instance.UpdateRecord_Local(eCharacterSheetDataType.Name, inName);
            Instance.UpdateRecord_Local(eCharacterSheetDataType.Character, inCharacter);
            Instance.UpdateRecord_Local(eCharacterSheetDataType.MaxBodyPoints, inMaxBodyPoints);
            Instance.UpdateRecord_Local(eCharacterSheetDataType.MaxMindPoints, inMaxMindPoints);
            Instance.UpdateRecord_Local(eCharacterSheetDataType.CurrBodyPoints, inMaxBodyPoints);
            Instance.UpdateRecord_Local(eCharacterSheetDataType.CurrMindPoints, inMaxMindPoints);
            Instance.UpdateRecord_Local(eCharacterSheetDataType.Quest, "The Trial");
        }


    private void UpdateRecord_Local(eCharacterSheetDataType inUpdateData, int inValue)
    {
        UpdateRecord_Local(inUpdateData, inValue.ToString());
    }
        private void UpdateRecord_Local(eCharacterSheetDataType inUpdateData, string inValue)
    {
      switch(inUpdateData)
        {
            case eCharacterSheetDataType.MaxBodyPoints:
                iMaxBodyPoints = int.Parse(inValue);
                tCharacterSheet_BodyPoints.SetText(GetBodyText());
                tCharacterSheet_BodyPoints.ForceMeshUpdate();
                break;
            case eCharacterSheetDataType.MaxMindPoints:
                iMaxMindPoints = int.Parse(inValue);
                tCharacterSheet_MindPoints.SetText(GetMindText());
                tCharacterSheet_MindPoints.ForceMeshUpdate();
                break;
            case eCharacterSheetDataType.CurrBodyPoints:
                iCurrBodyPoints = int.Parse(inValue);
                tCharacterSheet_BodyPoints.SetText(GetBodyText());
                tCharacterSheet_BodyPoints.ForceMeshUpdate();
                break;
            case eCharacterSheetDataType.CurrMindPoints:
                iCurrMindPoints = int.Parse(inValue);
                tCharacterSheet_MindPoints.SetText(GetMindText());
                tCharacterSheet_MindPoints.ForceMeshUpdate();
                break;

            case eCharacterSheetDataType.Character:
                strCharacter = inValue;
                tCharacterSheet_Character.SetText(GetCharacterText()); 
                tCharacterSheet_Character.ForceMeshUpdate();
                break;
            case eCharacterSheetDataType.Name:
                strName = inValue;
                tCharacterSheet_Name.SetText(GetNameText());
                tCharacterSheet_Name.ForceMeshUpdate();
                break;
            case eCharacterSheetDataType.Quest:
                lQuests.Add(GetQuestText(lQuests.Count+1,inValue));
                SetQuestDropDownText();
                
                break;
        }

    }

    private string GetBodyText()
    {
        return "<i>Body Points</i>\n<b>"+iCurrBodyPoints.ToString() + " / " + iMaxBodyPoints.ToString() + "</b>";
    }

    private string GetMindText()
    {
        return "<i>Mind Points</i>\n<b>"+ iCurrMindPoints.ToString() + " / " + iMaxMindPoints.ToString() + "</b>";
    }

    private string GetNameText()
    {
        return "<i>Name: </i><b>" +strName + "</b>";  
    }

    private string GetCharacterText()
    {
        return "<i>Character: </i><b>" + strCharacter + "</b>";

    }

    private string GetQuestText(int inQuestNumber, string inQuestName)
    {
        return "(" + inQuestNumber + "/" + (lQuests.Count+1) + ") " + inQuestName;
    }
    private void SetQuestDropDownText()
    {
        dQuestDropDown.ClearOptions();
        dQuestDropDown.AddOptions(lQuests);

    }

    private void Awake()
    {
        Instance = this;
        rCharacterSheetBackground = transform.Find("iCharacterSheetBackGround").GetComponent<RectTransform>();
        tCharacterSheet_Name = transform.Find("tCharacterSheet_Name").GetComponent<TextMeshProUGUI>();
        tCharacterSheet_Character = transform.Find("tCharacterSheet_Character").GetComponent<TextMeshProUGUI>();
        tCharacterSheet_BodyPoints = transform.Find("tCharacterSheet_BodyPoints").GetComponent<TextMeshProUGUI>();
        tCharacterSheet_MindPoints = transform.Find("tCharacterSheet_MindPoints").GetComponent<TextMeshProUGUI>();
        tCharacterSheet_Name = transform.Find("tCharacterSheet_Name").GetComponent<TextMeshProUGUI>();
        dQuestDropDown = transform.Find("dQuestDropDown").GetComponent<TMP_Dropdown>();


    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
