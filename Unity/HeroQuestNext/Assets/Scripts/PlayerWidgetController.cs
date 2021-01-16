using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class PlayerWidgetController : MonoBehaviour
{
    //[SerializeField] private GameObject PlayerUIMenu;
    public static PlayerWidgetController Instance { get; private set; }
    private List<GameObject> lPlayerMenuItems = new List<GameObject>();
    private List<string> lSkillString = new List<string>();
    private Color cBG_Disabled = new Color(1f, 1f, 1f);
    private Dictionary<eSkillReadyStateType, Color> dGetColorFromState = new Dictionary<eSkillReadyStateType, Color>()
    {
        {eSkillReadyStateType.Disabled, new Color(.5f, .5f, .5f) },
        {eSkillReadyStateType.MoveReady, new Color(0f, 0f, .5f) },
        {eSkillReadyStateType.MoveActive,new Color(0f, 0f, 1f) },
        {eSkillReadyStateType.ActReady, new Color(.5f, .5f, 0f) },
        {eSkillReadyStateType.ActActive, new Color(1f, 1f, 0f) },
        {eSkillReadyStateType.Passive, new Color(0f,.5f,0f) },
    };


    

    private float fTimer = 0f;

    
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
     

    }

    void Start()
    {
        //GameCombatHandler.Instance.lPlayerTeam[0].OnSkillUpdate += PlayerWidgetController_OnSkillUpdate;
        

    }

    //public void PlayerWidgetController_OnSkillUpdate(object sender, EventArgs e)
    public void PlayerWidgetController_OnSkillUpdate(List<EquipmentData> inEquipmentData)

    {
        Debug.Log("PlayerWidgetController_OnSkillUpdate" + inEquipmentData[0].tToolTipLong.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        fTimer += Time.deltaTime;
        fTimer %= 100;
    }
    public static void SetPlayerWidgetText_Static(List<EquipmentData> inEquipData)
    {
        Instance.SetPlayerWidgetText(inEquipData);
    }

    public void SetPlayerWidgetText(List<EquipmentData> inEquipData)
    {
        GameObject tCurrentMenu = (GameObject)GameObject.Find("PlayerWidget");
        GameObject tRefItem = (GameObject)GameObject.Find("PlayerMenuItem1");
        //Vector3 tRefVector = tRefItem.GetComponentInChildren<SpriteRenderer>().bounds.size;
        RectTransform tRefRect = tRefItem.GetComponentInChildren<RectTransform>();
        foreach (GameObject aOldMenu in lPlayerMenuItems)
        {
            Destroy(aOldMenu);
        }
        lPlayerMenuItems.Clear();
        int i = 0;
        foreach (EquipmentData aSkillData in inEquipData)
        {
            GameObject tMenuItem = Instantiate(tRefItem, tCurrentMenu.transform);
            Vector3 vMenuItemPos = new Vector3(tRefItem.transform.position.x + i * tRefRect.rect.width * tRefItem.transform.lossyScale.x, tRefItem.transform.position.y, tRefItem.transform.position.z);
            tMenuItem.transform.position = vMenuItemPos;
            TextMeshProUGUI tMenuItemText = tMenuItem.GetComponentInChildren<TextMeshProUGUI>();
            UnityEngine.UI.Image tMenuItemBackGround = tMenuItem.GetComponentInChildren<UnityEngine.UI.Button>().GetComponentInChildren<UnityEngine.UI.Image>();

            tMenuItemText.SetText("<size=13><b><align=center>" + aSkillData.tToolTipShort + "</align></b></size>");
            lSkillString.Add(aSkillData.tToolTipShort);
            string sSkillColor = ColorUtility.ToHtmlStringRGB(dGetColorFromState[aSkillData.eSkillReadyState]);
            string sToolTipTitleText = "<size=17><b>" + aSkillData.tToolTipShort + "</b></size>\n<size=15><i><color=#" + sSkillColor + ">" + aSkillData.eSkillReadyState + "</color></i></size>";
            string sToolTipBodyText = aSkillData.tToolTipLong;
            if (aSkillData.eKeywords.Count > 0)
            {
                sToolTipTitleText += "\n<i>";
                foreach (eKeywordType aKeyword in aSkillData.eKeywords)
                {
                    sToolTipBodyText += EquipmentManger.dKeywordText[aKeyword];
                    sToolTipTitleText += aKeyword.ToString();
                }
            }


            System.Func<string> getToolTipTitleTextFunc = () =>
            {
                return sToolTipTitleText;
            };

            System.Func<string> getToolTipBodyTextFunc = () =>
            {
                return sToolTipBodyText;
            };
            tMenuItem.GetComponent<BlackBocks_UI>().OnMouseOverOnceFunc = () => UIToolTip.ShowToolTip_Static(getToolTipTitleTextFunc, getToolTipBodyTextFunc);
            tMenuItem.GetComponent<BlackBocks_UI>().OnMouseOutOnceFunc = () => UIToolTip.HideToolTip_Static();
            tMenuItemBackGround.color = dGetColorFromState[aSkillData.eSkillReadyState];
            lPlayerMenuItems.Add(tMenuItem);
            i++;

            
        }
    }
 
}
