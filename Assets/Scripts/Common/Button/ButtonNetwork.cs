using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class ButtonNetwork : Button {

    /// <summary>
    /// IPAddress
    /// </summary>
    public string ipAddress = "";

    public Button MoveButton;

    public Image LeftSprite;
    public Image MoveButtonImage;
    public Image LevelImage;

    public Sprite PushSprite;
    public Sprite PopSprite;

    public Text PercentText;
    public Text UserNameText;
    public Text ExplainText;
    public Text LevelText;

    [SerializeField]
    private Color m_Alpha80 = new Color(1, 1, 1, 0.8f);
    private Color m_Alpha0 = new Color(1, 1, 1, 0.0f);

    private Color m_ConntedColor = new Color(0.10f, 0.84f, 0.55f, 1.0f);

    public float PercentVal = 0.0f;
    public bool isMissionStart = false;
    public string EegValueStr = "";
    /// <summary>
    /// 클라이언트와 서버가 연겨되면 서버로부터 받은 커넥션 등록 멤버
    /// </summary>
    //private NetworkConnection m_UserConnector;
    ///// <summary>
    ///// NetworkConnection Property
    ///// </summary>
    //public NetworkConnection Connector
    //{
    //    get { return m_UserConnector; }
    //    set { m_UserConnector = value; }
    //}
    /// <summary>
    /// 객체 삭제될 시간
    /// </summary>
    private float m_TimeOut = 5.0f;
    /// <summary>
    /// 객체 유지된 시간
    /// </summary>
    private float m_ElapsedTime = 0.0f;
    public float LifeTime
    {
        get { return m_ElapsedTime; }
        set { m_ElapsedTime = value; }
    }
    private bool isEnter = false;

    private bool isToggleOn = false;

    private UnityAction<string> m_UpEvent;

    //private UnityAction<ButtonNetwork> m_MoveButtonUpEvent;

    protected override void Start()
    {
        StartCoroutine(TimeOutRoutine());
        SetAllAlpha(true);
    }

    /// <summary>
    /// 클라이언트 버튼 프리팹의 UI 컬러값 변경 함수
    /// </summary>
    /// <param name="setColor"></param>
    public void SetAllAlpha(bool isOn)
    {
        Color setColor = Color.white;
        if (isOn)        
            setColor = m_Alpha80;        
        else        
            setColor = Color.white;        

        LevelImage.color = setColor;
        LeftSprite.color = setColor;
        UserNameText.color = setColor;
        //PercentText.color = setColor;
        ExplainText.color = setColor;        
        LevelText.color = setColor;
    }

    /// <summary>
    /// 연결된 객체면 바탕색 변경
    /// </summary>
    /// <param name="isConnected"></param>
    public void SetButtonBG(bool isConnected)
    {
        if (isConnected)
            image.color = m_ConntedColor;
        else
            image.color = m_Alpha0;
    }

    /// <summary>
    /// 버튼 Text 변경 함수
    /// </summary>
    /// <param name="NameStr">이름, 성별</param>
    /// <param name="PercentStr">현재 진행도</param>
    /// <param name="ExplainStr">상황 설명</param>
    /// <param name="LevelStr">현재 난이도</param>
    public void SetText(string ip, string NameStr, string sex, string PercentStr, string ExplainStr, string LevelStr)
    {
        ipAddress = ip;
        UserNameText.text = string.Format("{0} ({1})", NameStr, sex);
        PercentText.text = PercentStr;
        ExplainText.text = ExplainStr;
        LevelText.text = LevelStr;
    }
    /// <summary>
    /// Add Pointer Up Event
    /// </summary>
    /// <param name="action"></param>
    public void AddUpEvent(UnityAction<string> action, UnityAction<ButtonNetwork> CallbackAction)
    {
        m_UpEvent = null;
        if(action != null)
            m_UpEvent = action;
        MoveButton.onClick.RemoveAllListeners();
        MoveButton.onClick.AddListener(delegate { CallbackAction(this); });
    }

    /// <summary>
    /// Add Pointer Up Event
    /// </summary>
    /// <param name="action"></param>
    public void AddMoveButtonUpEvent(UnityAction<ButtonNetwork> action)
    {
        MoveButton.onClick.RemoveAllListeners();
        MoveButton.onClick.AddListener(delegate { action(this); });
    }

    public void ChangeMoveButtonSprite(bool isOn)
    {
        if (isOn)
            MoveButton.image.sprite = PopSprite;
        else
            MoveButton.image.sprite = PushSprite;
    }

    /// <summary>
    /// Pointer Enter
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;
        base.OnPointerEnter(eventData);
    }
    /// <summary>
    /// Pointer Down
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }
    /// <summary>
    /// Pointer Up
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerUp(PointerEventData eventData)
    {
        if(isEnter)
        {
            base.OnPointerUp(eventData);
            if(m_UpEvent != null)
            {
                if(ipAddress != null)
                    m_UpEvent(ipAddress);

                if(isToggleOn)
                {

                }
            }
        }
        
    }
    /// <summary>
    /// Pointer Exit
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;
        base.OnPointerExit(eventData);
    }
    /// <summary>
    /// 시간이 지나면 객체 리스트에서 삭제
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeOutRoutine()
    {
        m_ElapsedTime = 0.0f;
        WaitForEndOfFrame EOF = new WaitForEndOfFrame();
        while(true)
        {
            if (m_ElapsedTime > m_TimeOut)
            {
                List<ButtonNetwork> userList = ClientManager.Inst.GetUserList();
                userList.Remove(this);
                Destroy(gameObject);
                break;
            }
            else
            {
                m_ElapsedTime += Time.deltaTime;
                yield return EOF;
            }
        }
        
    }
}
