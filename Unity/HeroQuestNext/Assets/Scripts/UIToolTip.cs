using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIToolTip : MonoBehaviour
{
    public static UIToolTip Instance { get; private set; }
    private TextMeshProUGUI tToolTipTitleTMP;
    private TextMeshProUGUI tToolTipBodyTMP;
    private RectTransform rToolTipBackground;

    private System.Func<string> getToolTipTitleTextFunc;
    private System.Func<string> getToolTipBodyTextFunc;

    [SerializeField] private Camera cUI_Camera;
    private float fTextPaddingSize = 4;

    private void Awake()
    {
        Instance = this;
        rToolTipBackground = transform.Find("backGround").GetComponent<RectTransform>();
        tToolTipTitleTMP = transform.Find("TextTitleTMP").GetComponent<TextMeshProUGUI>();
        tToolTipBodyTMP = transform.Find("TextBodyTMP").GetComponent<TextMeshProUGUI>();
        HideToolTip();
        //ShowToolTip("Random ToolTip Text");
    }

    private void ShowToolTip(string inToolTipTitleString, string inToolTipBodyString ="")
    {
        ShowToolTip(() => inToolTipTitleString, () => inToolTipBodyString);
    }

    private void ShowToolTip(System.Func<string> getToolTipTitleTextFunc, System.Func<string> getToolTipBodyTextFunc)
    {
        this.getToolTipTitleTextFunc = getToolTipTitleTextFunc;
        this.getToolTipBodyTextFunc = getToolTipBodyTextFunc;
        gameObject.SetActive(true);
        SetText(getToolTipTitleTextFunc(), getToolTipBodyTextFunc());
    }

    private void SetText(string inToolTipTitleString, string inToolTipBodyString="")
    {
        tToolTipTitleTMP.SetText(inToolTipTitleString);
        tToolTipBodyTMP.SetText(inToolTipBodyString);
        tToolTipTitleTMP.ForceMeshUpdate();
        tToolTipBodyTMP.ForceMeshUpdate();
        float fWidth = Mathf.Max(tToolTipBodyTMP.GetRenderedValues(false).x, tToolTipTitleTMP.GetRenderedValues(false).x);
        float fHeight = tToolTipBodyTMP.GetRenderedValues(false).y + tToolTipTitleTMP.GetRenderedValues(false).y;
        Vector2 vBackGroundSize = new Vector2(fWidth,fHeight) + new Vector2(fTextPaddingSize * 2, fTextPaddingSize * 2);
        Vector3 vCurrentTitlePos = tToolTipBodyTMP.transform.localPosition + new Vector3(0,tToolTipBodyTMP.GetRenderedValues(false).y,0);
        tToolTipTitleTMP.transform.localPosition = vCurrentTitlePos;

        rToolTipBackground.sizeDelta = vBackGroundSize;
        //Update();
    }

    private void HideToolTip()
    {
        gameObject.SetActive(false);
    }

    public static void ShowToolTip_Static(string inToolTipString)
    {
        Instance.ShowToolTip(inToolTipString);
    }

    public static void ShowToolTip_Static(System.Func<string> getToolTipTitleTextFunc, System.Func<string> getToolTipBodyTextFunc)
    {
        Instance.ShowToolTip(getToolTipTitleTextFunc,getToolTipBodyTextFunc);
    }

    public static void HideToolTip_Static()
    {
        Instance.HideToolTip();
    }
    private void Update()
    {
        SetText(getToolTipTitleTextFunc(),getToolTipBodyTextFunc());
        Vector2 vLocalPoint;
        //RectTransform rParent = transform.parent.parent.GetComponent<RectTransform>();
        RectTransform rParent = transform.parent.GetComponent<RectTransform>();
        Vector3 vMousePosition = Input.mousePosition;

        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rParent, vMousePosition, cUI_Camera, out vLocalPoint);

        //tToolTipBodyTMP.GetComponent<RectTransform>(). = transform.GetComponent<RectTransform>().rect.yMin;
        
        if(vLocalPoint.x + rToolTipBackground.rect.width > rParent.rect.xMax)
        {
            vLocalPoint.x = rParent.rect.xMax - rToolTipBackground.rect.width;
        }

        if (vLocalPoint.y - rToolTipBackground.rect.height > rParent.rect.yMin)
        {
            vLocalPoint.y = rParent.rect.yMin + rToolTipBackground.rect.height;
        }

        transform.localPosition = vLocalPoint;
    }
}
