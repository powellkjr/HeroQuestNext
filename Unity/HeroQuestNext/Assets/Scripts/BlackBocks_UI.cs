using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlackBocks_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Action OnMouseLeftClickFunc = null;
    public Action OnMouseRightClickFunc = null;
    public Action OnMouseMiddleClickFunc = null;


    public Action OnMouseDownOnceFunc = null;
    public Action OnMouseUpFunc = null;

    public Action OnMouseOverOnceToolTipFunc = null;
    public Action OnMouseOutOnceToolTipFunc = null;

    public Action OnMouseOverOnceFunc = null;
    public Action OnMouseOutOnceFunc = null;

    public Action OnMouseOverFunc = null;
    public Action OnMouseOverPerSecFunc = null;
    public Action OnMouseUpdate = null;
    
    public Action<PointerEventData> OnPointerClickFunc;

    public enum eHoverBehaviourType
    {
        Custom,
        Change_Color,
        Change_Image,
        Change_SetActive,
    }

    public eHoverBehaviourType eHoverBehaviour = eHoverBehaviourType.Custom;
    private Action hoverBehaviourFunc_Enter, hoverBehaviourFunc_Exit;

    public Color cHoverBehaviour_Color_Enter, cHoverBehaviour_Color_Exit;
    public Image iHoverBehaviour_Image;
    public Sprite sHoverBehaviour_Sprite_Enter, sHoverBehaviourSprite_Exit;

    public bool bHoverHaviour_Move = false;
    public Vector2 vHoverBehaviour_Move_Amount = Vector2.zero;
    private Vector2 vPos_Enter, vPos_Exit;
    public bool bTriggerMouseOutFuncOnClick = false;
    private bool bMouseOver;
    public float fMouseOverPerSecFuncTimer;

    private Action internalOnPointerEnterFunc, internalOnPointerExitFunc, internalOnPointerClickFunc;
#if SOUND_MANAGER
    public Sound_Manager.Sound mouseOverSound, mouseClickSound;
#endif
#if CURSOR_MANAGER
    public CursorManger.CursorType cursorMouseOver, cursorMouseOut;
#endif


    public virtual void OnPointerEnter(PointerEventData inEventData)
    {
        if(internalOnPointerEnterFunc != null)
        {
            internalOnPointerEnterFunc();
        }

        if(bHoverHaviour_Move)
        {
            transform.localPosition = vPos_Enter;
        }

        if(hoverBehaviourFunc_Enter != null)
        {
            hoverBehaviourFunc_Enter();
        }

        if(OnMouseOverOnceFunc != null)
        {
            OnMouseOverOnceFunc();
        }

        if(OnMouseOverOnceToolTipFunc !=null)
        {
            OnMouseOverOnceToolTipFunc();
        }

        bMouseOver = true;
        fMouseOverPerSecFuncTimer = 0f;
    }

    public virtual void OnPointerExit(PointerEventData eventinEventDataData)
    {
        if (internalOnPointerExitFunc != null)
        {
            internalOnPointerExitFunc();
        }

        if (bHoverHaviour_Move)
        {
            transform.localPosition = vPos_Exit;
        }

        if (hoverBehaviourFunc_Exit != null)
        {
            hoverBehaviourFunc_Exit();
        }

        if (OnMouseOutOnceFunc != null)
        {
            OnMouseOutOnceFunc();
        }

        if (OnMouseOutOnceToolTipFunc != null)
        {
            OnMouseOutOnceToolTipFunc();
        }

        bMouseOver = false;
    }

    public virtual void OnPointerClick(PointerEventData inEventData)
    {
        if (internalOnPointerClickFunc != null)
        {
            internalOnPointerClickFunc();
        }
        if (OnPointerClickFunc != null)
        {
            OnPointerClick(inEventData);
        }
        switch(inEventData.button)
        {
            case PointerEventData.InputButton.Left:
                if (bTriggerMouseOutFuncOnClick)
                {
                    OnPointerExit(inEventData);
                }
                if (OnMouseLeftClickFunc != null)
                {
                    OnMouseLeftClickFunc();
                }
                break;
            case PointerEventData.InputButton.Right:
                if(OnMouseRightClickFunc != null)
                {
                    OnMouseRightClickFunc();
                }
                break;
            case PointerEventData.InputButton.Middle:
                if (OnMouseMiddleClickFunc != null)
                {
                    OnMouseMiddleClickFunc();
                }
                break;
        }
    }
    public void Manual_OnPointerExit()
    {
        OnPointerExit(null);
    }
    public bool IsMouseOver()
    {
        return bMouseOver;
    }

    public void OnPointerDown(PointerEventData inEventData)
    {
        if (OnMouseDownOnceFunc != null)
        {
            OnMouseDownOnceFunc();
        }
    }

    public void OnPointerUp(PointerEventData inEventData)
    {
        if (OnMouseUpFunc!=null)
        {
            OnMouseUpFunc();
        }
    }
    void Awake()
    {
        vPos_Exit = transform.localPosition;
        vPos_Enter = (Vector2)transform.localPosition + vHoverBehaviour_Move_Amount;
        SetHoverBehaviourType(eHoverBehaviour);
#if SOUND_MANAGER
    internalOnPointerEnterFunc += ()=> {
            if(mouseOverSound != Sound_Manger.Sound.None)
            {
                SoundManager.PlaySound(mouseOverSound);
            }
        }
#endif
#if CURSOR_MANAGER
    internalOnPointerClickFunc += () => {
        if (mouseClickSound != Sound_Manger.Sound.None)
        {
            SoundManager.PlaySound(mouseClickSound);
        }
#endif
    }
        // Start is called before the first frame update
    public void SetHoverBehaviourType(eHoverBehaviourType inHoverBehaviourType)
    {
        this.eHoverBehaviour = inHoverBehaviourType;
        switch(inHoverBehaviourType)
        {
            case eHoverBehaviourType.Change_Color:
                hoverBehaviourFunc_Enter = delegate () { iHoverBehaviour_Image.color = cHoverBehaviour_Color_Enter; };
                hoverBehaviourFunc_Exit = delegate () { iHoverBehaviour_Image.color = cHoverBehaviour_Color_Exit; };
                break;
            case eHoverBehaviourType.Change_Image:
                hoverBehaviourFunc_Enter = delegate () { iHoverBehaviour_Image.sprite = sHoverBehaviour_Sprite_Enter; };
                hoverBehaviourFunc_Exit = delegate () { iHoverBehaviour_Image.sprite = sHoverBehaviourSprite_Exit; };
                break;
            case eHoverBehaviourType.Change_SetActive:
                hoverBehaviourFunc_Enter = delegate () { iHoverBehaviour_Image.gameObject.SetActive(true); };
                hoverBehaviourFunc_Exit = delegate () { iHoverBehaviour_Image.gameObject.SetActive(false); };
                break;
        }
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(bMouseOver)
        {
            if(OnMouseOverFunc!=null)
            {
                OnMouseOverFunc();
            }
            fMouseOverPerSecFuncTimer -= Time.unscaledDeltaTime;
            if(fMouseOverPerSecFuncTimer<=0)
            {
                fMouseOverPerSecFuncTimer += 1f;
                if (OnMouseOverPerSecFunc != null)
                {
                    OnMouseOverPerSecFunc();
                }
            }
                
        }
        if (OnMouseUpdate != null)
        {
            OnMouseUpdate();
        }
    }
}
