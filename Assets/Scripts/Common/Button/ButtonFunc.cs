using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum BUTTON_TYPE
{
    NONE,
    TYPE_1,
    TYPE_2,
    TYPE_2_1,
    TYPE_3,
    TYPE_4
}

public class ButtonFunc : ButtonBase
{
    //private AsyncClient asyncClient;

    public BUTTON_TYPE buttonType = BUTTON_TYPE.NONE;

    public Image mSprite;
    public Image mScaleImage;
    public Text mText;
    public Color mColor;
    public Color mPointColor;

    public Color NewDefaultColor = new Color();
    public Color NewEnterColor = new Color();
    public Color NewPressedColor = new Color();

    public Color NewTextDefaultColor = new Color();
    public Color NewTextEnterColor = new Color();
    public Color NewTextPressedColor;

    public string NewButtonText;
    public bool isScaleChange = false;
    /// <summary>
    /// 버튼 기본 Sprite
    /// </summary>
    public Sprite DefaultSprite = null;
    /// <summary>
    /// Hover 시 Sprite
    /// </summary>
    public Sprite HoverSprite = null;
    /// <summary>
    /// Pressed 시 Sprite
    /// </summary>
    public Sprite PressedSprite = null;
    public Sprite DisabledSprite = null;

    public Sprite LockSprite = null;
    public Sprite PressedLockSprite = null;

    /// <summary>
    /// 상황 선택에서 Scale 조절에 필요한 변수
    /// </summary>
    public AnimationCurve TimeCurve;

    /// <summary>
    /// Scene 전환하는 버튼이면 SceneNumber가 저장됨 -1은 초기값
    /// </summary>
    public int SceneNumber = -1;
    /// <summary>
    /// 2019.05.13 
    /// Scene 전환하는 버튼이면 SceneName이 저장됨
    /// </summary>
    public string SceneName = "";
    /// <summary>
    /// 키보드에서 특수키 체크(ex:Shift, TransferKey)
    /// </summary>
    public bool IsFunctionKey = false;

    public bool IsToggle = false;
    public bool IsSelected = false;
    public bool IsCanTouch = true;
    public bool IsLock = false;

    /// <summary>
    /// 2019.05.02 추가
    /// </summary>
    public List<ButtonFunc> ToggleList;
    /// <summary>
    /// 2019.05.02 추가
    /// </summary>
    public int ToggleIndex = -1;

    public Image GetSetImage
    {
        get
        {
            return mSprite;
        }
        set
        {
            mSprite = value;
        }
    }

    public string GetSetText
    {
        get
        {
            return mText.text;
        }
        set
        {
            if(mText != null)
                mText.text = value;
        }
    }
    public bool GetSetToggle
    {
        get { return IsToggle; }
        set { IsToggle = value;}
    }
    public bool GetSetSelected
    {
        get { return IsSelected; }
        set { IsSelected = value; }
    }
    public bool GetSetLock
    {
        get { return IsLock; }
        set { IsLock = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        FindChildrens();

        //asyncClient = FindObjectOfType<AsyncClient>();
    }
    
    private void FindChildrens()
    {
        mColor = new Color(0.21f, 0.21f, 0.21f, 1);
        mPointColor = new Color(0.24f, 0.9294f, 0.6784f, 1);
        mSprite = GetComponent<Image>();
        if (transform.Find("ButtonText") != null)
            mText = transform.Find("ButtonText").GetComponent<Text>();
        if (transform.Find("Image") != null)
            mScaleImage = transform.Find("Image").GetComponent<Image>();
        if (mText != null)
            mText.color = Color.white;
        Interactable = true;
        dummyInteractable = true;
        IsCanTouch = true;

        LockSprite = Resources.Load<Sprite>("UI/Button/Common/BTN_Lock");
        PressedLockSprite = Resources.Load<Sprite>("UI/Button/Common/BTN_Lock");
    }

    public void Allocate()
    {
        FindChildrens();
        switchAllocate();
    }

    private void switchAllocate()
    {
        if(mText != null)
            mText.text = NewButtonText;
        switch (buttonType)
        {
            case BUTTON_TYPE.TYPE_1:
                Debug.Log(NewButtonText);
                if(mSprite != null)
                {
                    mSprite.color = NewDefaultColor;
                }
                if(mText != null)
                    mText.color = Color.white;
                break;

            case BUTTON_TYPE.TYPE_2:
                if(mSprite != null)
                    mSprite.color = NewDefaultColor;
                if(mText != null)
                    mText.color = Color.white;
                break;

            case BUTTON_TYPE.TYPE_2_1:
                if (mSprite != null)
                    mSprite.color = NewDefaultColor;
                if (mText != null)
                    mText.color = Color.white;
                break;
            case BUTTON_TYPE.TYPE_4:
                if (mSprite != null)
                    mSprite.color = NewDefaultColor;
                if (mText != null)
                    mText.color = NewTextDefaultColor;
                break;
        }
    }

    private void Start()
    {
        
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!Interactable)
            return;

        base.OnPointerEnter(eventData);

        SwitchEnterEvent();

        if (isScaleChange)
        {
            ChangeScale(mScaleImage.transform, 1.2f);
            ChangeScale(mText.transform, 1.2f);
            if (EnterAction != null)
                EnterAction.Invoke();
        }
        else
        {
            
        }
        if (EnterAction != null)
        {
            EnterAction.Invoke();
        }
        if (HoverSprite != null)
        {
            if (IsSelected)
                return;

            if(GetSetLock)
            {
                if(LockSprite != null)
                    mSprite.sprite = LockSprite;
                mText.enabled = false;
            }
            else
            {
                mSprite.sprite = HoverSprite;
                if(mText != null)
                    mText.enabled = true;
            }
                
        }

    }

    private void SwitchEnterEvent()
    {
        switch (buttonType)
        {
            case BUTTON_TYPE.TYPE_1:
                ChangeColor(mSprite, NewEnterColor);
                break;
            case BUTTON_TYPE.TYPE_2:
                ChangeColor(mSprite, NewEnterColor);
                break;
            case BUTTON_TYPE.TYPE_2_1:
                ChangeColor(mSprite, NewEnterColor);
                break;
            case BUTTON_TYPE.TYPE_4:
                ChangeColor(mSprite, NewEnterColor);
                ChangeColor(mText, NewTextDefaultColor);
                ChangeScale(this.transform, 1.2f);
                this.transform.SetAsLastSibling();
                break;
        }
    }

    private void SwitchDownEvent()
    {
        switch (buttonType)
        {
            case BUTTON_TYPE.TYPE_1:
                ChangeColor(mSprite, NewPressedColor);
                ChangeColor(mText, NewTextPressedColor);
                break;
            case BUTTON_TYPE.TYPE_2:
                ChangeColor(mSprite, NewPressedColor);
                ChangeColor(mText, NewTextPressedColor);
                break;
            case BUTTON_TYPE.TYPE_2_1:
                ChangeColor(mSprite, NewPressedColor);
                ChangeColor(mText, NewTextPressedColor);
                break;
            case BUTTON_TYPE.TYPE_4:
                ChangeColor(mSprite, NewPressedColor);
                ChangeColor(mText, NewTextPressedColor);
                //ChangeScale(this.transform, 1.0f);
                break;
        }
    }

    private void SwitchUpEvent()
    {
        switch (buttonType)
        {
            case BUTTON_TYPE.TYPE_1:
                ChangeColor(mSprite, NewDefaultColor);
                ChangeColor(mText, Color.white);
                break;
            case BUTTON_TYPE.TYPE_2:
                ChangeColor(mSprite, NewDefaultColor);
                ChangeColor(mText, Color.white);
                break;
            case BUTTON_TYPE.TYPE_2_1:
                ChangeColor(mSprite, NewDefaultColor);
                break;
            case BUTTON_TYPE.TYPE_4:
                ChangeColor(mSprite, NewDefaultColor);
                ChangeColor(mText, NewTextDefaultColor);
                ChangeScale(this.transform, 1.0f);
                break;
        }
    }

    private void SwitchExitEvent()
    {
        switch (buttonType)
        {
            case BUTTON_TYPE.TYPE_1:
                ChangeColor(mSprite, NewDefaultColor);
                ChangeColor(mText, Color.white);
                break;
            case BUTTON_TYPE.TYPE_2:
                ChangeColor(mSprite, NewDefaultColor);
                ChangeColor(mText, Color.white);
                break;
            case BUTTON_TYPE.TYPE_2_1:
                ChangeColor(mSprite, NewDefaultColor);
                break;
            case BUTTON_TYPE.TYPE_4:
                ChangeColor(mSprite, NewDefaultColor);
                ChangeColor(mText, NewTextDefaultColor);
                ChangeScale(this.transform, 1.0f);                
                break;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!Interactable)
            return;

        if (isScaleChange)
        {
            ChangeScale(mScaleImage.transform, 1.0f);
            ChangeScale(mText.transform, 1.0f);
        }
        //else
        //{
        //    if(IsToggle)
        //    {
        //        if (PressedSprite != null)
        //            mSprite.sprite = PressedSprite;                
        //    }
        //    else
        //    {
        //        if(GetSetLock)
        //        {
        //            mSprite.sprite = PressedLockSprite;                    
        //        }
        //        else if (PressedSprite != null)
        //        {
        //            mSprite.sprite = PressedSprite;
        //        }
        //        if (mText != null)
        //        {
        //            mText.color = mColor;
        //        }
        //    }            
        //}

        SwitchDownEvent();

        if (ClickSoundAction != null)
        {
            ClickSoundAction.Invoke();
        }
        base.OnPointerDown(eventData);
        
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!Interactable)
            return;

        SwitchUpEvent();

        if (isScaleChange)
        {
            if(isOn)
            {
                ChangeAlpha(mText, 1.0f);
            }
            
            ChangeScale(mScaleImage.transform, 1.0f);
        }
        else
        {            
            if(IsToggle && isOn)
            {                
                IsSelected = true;

                //if (HoverSprite != null)
                //    mSprite.sprite = DefaultSprite;
                //if (mText != null)
                //    mText.color = Color.white;
            }
            else
            {
                //if (HoverSprite != null && isOn)
                //    mSprite.sprite = DefaultSprite;
                //if (mText != null && isOn)
                //    mText.color = Color.white;
            }
        }

        //Debug.Log(EventPressed.Method.);

        if (Interactable)
        {
            if(IsToggle && isOn)
            {
                //EventPressed();
                AsyncClientCall();
                EventTogglePressed(ToggleIndex);
                //pressedAction.Invoke();
            }
            else
            {
                if (EventPressed != null && isOn == true)
                {
                    AsyncClientCall();
                    EventPressed();
                    //pressedAction.Invoke();
                }
                else if(EventChangeScenePressed != null && isOn)
                {
                    Debug.Log("DataManager.Inst.SceneName :" + SceneName);
                    AsyncClientCall();
                    EventChangeScenePressed(SceneName);
                }
            }
            
        }
        else
        {
            base.OnPointerUp(eventData);
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!Interactable)
            return;
        
        base.OnPointerExit(eventData);

        if (isScaleChange)
        {
            ChangeScale(mScaleImage.transform, 1.0f);
            ChangeScale(mText.transform, 1.0f);
        }
        //else
        //{            
        //    if (IsSelected)
        //        return;

        //    if (DefaultSprite != null)
        //        mSprite.sprite = DefaultSprite;
        //    if (mText != null && !IsToggle)
        //    {
        //        mText.color = Color.white;            
        //        mText.enabled = true;
        //    }            
        //}

        SwitchExitEvent();

        if (ExitAction != null)
        {
            ExitAction.Invoke();
        }                
    }

    private void AsyncClientCall()
    {
        Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");
        //asyncClient = FindObjectOfType<AsyncClient>();

        //if (asyncClient != null)
        //{
        //    Debug.Log("asyncCleint Invoke");
        //    asyncClient.ClearEvent();
        //    if(IsToggle && isOn)
        //    {
        //        //asyncClient.CmdSetParameters(new object[] { ToggleList, ToggleIndex });
                

        //        asyncClient.RpcToggleButtonPressed(
        //            EventTogglePressed.Target.GetType().Name,
        //            gameObject.name,
        //            EventTogglePressed.Method.Name, ToggleIndex);
        //    }
        //    else
        //    {
        //        System.Text.StringBuilder ParentPath = new System.Text.StringBuilder();
        //        Component[] parents = gameObject.transform.GetComponentsInParent(typeof(Transform), true);
        //        for (int i = parents.Length - 1; i >= 0; i--)
        //        {
        //            if (i == 0)
        //                ParentPath.Append(string.Format("{0}", parents[i].name));
        //            else
        //                ParentPath.Append(string.Format("{0}/", parents[i].name));
        //        }
        //        Debug.Log("Total Path : " + ParentPath.ToString());

        //        if (EventPressed != null)
        //        {
        //            asyncClient.RpcButtonPressed(
        //            EventPressed.Target.GetType().Name,
        //            ParentPath.ToString(),
        //            EventPressed.Method.Name);
        //        }
        //        else if(EventChangeScenePressed != null)
        //        {
        //            asyncClient.RpcButtonPressed2(
        //            EventChangeScenePressed.Target.GetType().Name,
        //            ParentPath.ToString(),
        //            EventChangeScenePressed.Method.Name,
        //            SceneName);
        //        }
                
        //    }
        //}
    }


    //private void OnToggleClick(List<ButtonFunc> btn, int index)
    //{
    //    btn[index].mSprite.sprite = btn[index].PressedSprite;
    //    btn[index].ChangeColor(btn[index].mText, btn[index].mColor);
    //    btn[index].ChangeAlpha(btn[index].mSprite, 1.0f);
    //    for (int i = 0; i < btn.Count; i++)
    //    {
    //        if (i == index)
    //        {
    //            //Gender = i;
    //        }
    //        else
    //        {
    //            btn[i].IsSelected = false;
    //            btn[i].mSprite.sprite = btn[i].DefaultSprite;
    //            btn[i].ChangeColor(btn[i].mText, Color.white);
    //            btn[i].ChangeAlpha(btn[i].mText, 0.5f);
    //            btn[i].ChangeAlpha(btn[i].mSprite, 0.5f);
    //        }
    //    }
    //}

    private void ChangeSprite( )
    {
        
    }
    public void SetTextEnabled(bool val)
    {
        if(mText != null)
            mText.enabled = val;
    }
    public void ChangeColor(Text target, Color color)
    {
        if (target != null)
            target.color = color;
    }
    public void ChangeColor(Image target, Color color)
    {
        if (target != null)
            target.color = color;
    }
    public void ChangeColor(Text target, float r, float g, float b, float a)
    {
        if (target != null)
            target.color = new Color(r,g,b,a);
    }    
    public void ChangeAlpha(Image target, float value)
    {
        if(target != null)
            target.color = new Color(target.color.r, target.color.g, target.color.b, value);
    }
    public void ChangeAlpha(Text target, float value)
    {
        if(target != null)
            target.color = new Color(target.color.r, target.color.g, target.color.b, value);
    }
    public void ChangeHoverImage(Sprite Target, Sprite Change)
    {
        HoverSprite = Change;
    }
    //Coroutine m_curScaleRoutine = null;
    public void ChangeScale(Transform target, float scale)
    {
        target.localScale = new Vector3(scale, scale, scale);
        //if (m_curScaleRoutine != null)
        //    StopCoroutine(m_curScaleRoutine);
        //float scale = 0.8f;
        //if (toBigger)
        //    scale = 1.2f;
        //m_curScaleRoutine = StartCoroutine(ChangeScaleRoutine(this.transform, scale, 0.5f));
    }
    
    IEnumerator ChangeScaleRoutine(Transform target, float scale, float duration = 0.3f)
    {
        WaitForEndOfFrame endF = new WaitForEndOfFrame();
        float elapsedTime = 0;
        Vector3 a = target.localScale;
        Vector3 b = new Vector3(scale, scale, scale);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = TimeCurve.Evaluate(elapsedTime / duration);
            target.localScale = Vector3.Lerp(a, b, t);
            yield return endF;
        }
        target.localScale = b;
    }
}
