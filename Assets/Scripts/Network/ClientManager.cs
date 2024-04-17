using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientManager : Singleton<ClientManager> {

    /// <summary>
    /// Program Quit
    /// </summary>
    private bool isQuit = false;

    private bool isConnected = false;

    /// <summary>
    /// Network Manager
    /// </summary>
    //public NetworkMng Mng;
    //private DrawOrder m_Director;
    private DirectionEffect m_oriDirectionEffectPrefab;
    /// <summary>
    /// ClientCanvas BG
    /// </summary>
    private GameObject m_ClientCanvasBGGO;

    /// <summary>
    /// Root ClientCanvas  
    /// </summary>
    private Canvas m_Canvas;
    private Canvas m_Canvas_FixedUI;
        
    /// <summary>
    /// Top Root GameObject
    /// </summary>
    private GameObject m_TopGO;
    /// <summary>
    /// 시선연동, 지시버튼, 강제종료 버튼 리스트 오브젝트
    /// </summary>
    private GameObject m_TopGOSyncButtons;
    /// <summary>
    /// Left Root GameObject
    /// </summary>
    private GameObject m_LeftGO;
    /// <summary>
    /// Center Root GameObject 
    /// </summary>
    private GameObject m_CenterGO;

    #region BioSignalUI Member
    /// <summary>
    /// 생체신호 부모 오브젝트
    /// </summary>
    private Transform m_BioSignalUI;
    /// <summary>
    /// 생체신호의 EEG최상위 오브젝트
    /// </summary>
    private Transform m_EEGParent;
    /// <summary>
    /// EEG 값 텍스트
    /// </summary>
    private Text m_EEGValueTtext;

    #endregion

    #region TopUI Member
    /* =================================== TopUI Member =================================== */
    /// <summary>
    /// DayInfo Root Transform
    /// </summary>
    private Transform m_TopDayInfoRootTR;
    /// <summary>
    /// UserInfo Root Transform
    /// </summary>
    private Transform m_TopUserInfoRootTR;
    /// <summary>
    /// 이름을 제외한 나머지 정보 Transform;
    /// </summary>
    private Transform m_TopOtherInfosTR;
    /// <summary>
    /// 상황 및 난이도의 상위 오브젝트
    /// </summary>
    private RectTransform m_TopSituationLevel;
    /// <summary>
    /// UserName Text
    /// </summary>
    private Text m_TopUserNameText;
    /// <summary>
    /// Current Situation Text
    /// </summary>
    private Text m_TopSituationText;
    /// <summary>
    /// Current Level Text
    /// </summary>
    private Text m_TopLevelText;
    /// <summary>
    /// Current Progress Percent Text
    /// </summary>
    private Text m_TopProgressText;
    /// <summary>
    /// Current Preogress Gauge bar
    /// </summary>
    //private Scrollbar m_TopProgressbar;
    private Slider m_TopProgressbar;
    /// <summary>
    /// Current Progress percent Value
    /// </summary>
    private float m_TopProgressValue;
    /// <summary>
    /// Current Time Text
    /// </summary>
    private Text m_TopTime;
    /// <summary>
    /// Today Date
    /// </summary>
    private Text m_TopDate;
    /// <summary>
    /// Today
    /// </summary>
    private Text m_TopDay;
    /// <summary>
    /// 시선연동 버튼 텍스트
    /// </summary>
    private Text m_SyncButtonText;

    /// <summary>
    /// 시선연동 
    /// </summary>
    private ButtonToggle m_ViewSyncOnButton;
    /// <summary>
    /// 시선연동 해제
    /// </summary>
    private ButtonToggle m_ViewSyncOffButton;

    /* =================================== TopUI Member =================================== */
    #endregion

    #region LeftUI Member
    /* =================================== LeftUI Member =================================== */
    /// <summary>
    /// UserList Transform
    /// </summary>
    private Transform m_UserListRootTR;
    /// <summary>
    /// 대기중인 유저 목록 추가할 Transform
    /// </summary>
    private Transform m_UserAddTR;
    /// <summary>
    /// AddedUserList Transform
    /// </summary>
    private Transform m_AddedUserListRootTR;
    /// <summary>
    /// 추가된 유저의 Transform
    /// </summary>
    private Transform m_AddedUserTR;
    /* =================================== LeftUI Member =================================== */
    #endregion

    #region CenterUI Member
    /* =================================== CenterUI Member =================================== */
    private Transform m_DecorationTR;
    /* =================================== CenterUI Member =================================== */
    #endregion
    #region =================================== Cross Hair ===================================
    /// <summary>
    /// 크로스 헤어 부모 오브젝트
    /// </summary>
    private Transform m_CrossHairTR;
    /// <summary>
    /// 크로스 헤어 버튼 오브젝트
    /// </summary>
    private Button m_CrossHairButton;
    /// <summary>
    /// 크로스 헤어 페이드 오브젝트
    /// </summary>
    private GameObject m_CrossHairBG;
    /// <summary>
    /// 크로스헤어크로스헤어 버튼 Pressed Image 오브젝트
    /// </summary>
    private GameObject m_CrossHairTouchBG;

    /// <summary>
    /// 시선연동 해제 시 특정 문구 표시
    /// </summary>
    private GameObject m_ViewSyncAlertText;
    /// <summary>
    /// 시선연동 해제 시 문구 액티브 코루틴
    /// </summary>
    private IEnumerator ShowViewSyncAlertTextIE;
    #endregion 

    /// <summary>
    /// 연결된 유저
    /// </summary>
    private ButtonNetwork m_ConnectedUser;
    /// <summary>
    /// 확인된 유저 리스트
    /// </summary>
    private List<ButtonNetwork> m_UserList = new List<ButtonNetwork>();
    /// <summary>
    /// 유저 버튼 프리팹
    /// </summary>
    private ButtonNetwork m_oriButtonNetworkPrefab;
        
    protected override void Awake()
    {
        base.Awake();
        //Mng = FindObjectOfType<NetworkMng>();
    }

    // Use this for initialization
    void Start () {

        m_oriButtonNetworkPrefab = Resources.Load<ButtonNetwork>("Prefabs/network/UserButton");
        m_oriDirectionEffectPrefab = Resources.Load<DirectionEffect>("Prefabs/Effect/DirectionEffectPrefab");
        //m_Director = GetComponent<DrawOrder>();
        m_Canvas = GameObject.Find("ClientCanvas").GetComponent<Canvas>();
        m_ClientCanvasBGGO = m_Canvas.transform.Find("ClientCanvasBG").gameObject;
        m_TopGO = m_Canvas.transform.Find("TopUI").gameObject;        
        m_LeftGO = m_Canvas.transform.Find("LeftUI").gameObject;
        m_CenterGO = m_Canvas.transform.Find("CenterUI").gameObject;
        FindTopUI();
        FindLeftUI();
        FindCenterUI();
        FindBioSignalUI();
        SetActive(m_LeftGO, false);        
        SetActive(m_UserListRootTR.gameObject, false);

        StartCoroutine(TimerRoutine());
    }
    
    /// <summary>
    /// Find BioSignalUI
    /// </summary>
    private void FindBioSignalUI()
    {
        m_BioSignalUI = m_Canvas.transform.Find("BioSignalUI").transform;
        m_EEGParent = m_BioSignalUI.Find("EEG");
        m_EEGValueTtext = m_EEGParent.Find("Value").GetComponent<Text>();

        //SetActive(m_BioSignalUI.gameObject, false);
        SetActive(m_EEGParent.gameObject, false);

    }

    /// <summary>
    /// Find TopUI Objects
    /// </summary>
    private void FindTopUI()
    {
        m_TopGOSyncButtons = m_TopGO.transform.Find("Buttons/TopGOSyncButtons").gameObject;
        m_TopGOSyncButtons.SetActive(false);

        m_ViewSyncOnButton = m_TopGOSyncButtons.transform.Find("SyncViewOnBtn").GetComponent<ButtonToggle>();
        m_ViewSyncOffButton = m_TopGOSyncButtons.transform.Find("SyncViewOffBtn").GetComponent<ButtonToggle>();

        m_TopUserInfoRootTR = m_TopGO.transform.Find("UserInfo").transform;
        m_TopDayInfoRootTR = m_TopGO.transform.Find("DayInfo").transform;

        m_TopUserNameText = m_TopUserInfoRootTR.transform.Find("NameText").GetComponent<Text>();
        m_TopOtherInfosTR = m_TopUserInfoRootTR.transform.Find("OtherInfos").transform;

        m_TopSituationLevel = m_TopOtherInfosTR.transform.Find("SituationLevel").GetComponent<RectTransform>();
        m_TopSituationText = m_TopOtherInfosTR.transform.Find("SituationLevel/SituationText").GetComponent<Text>();
        m_TopLevelText = m_TopOtherInfosTR.transform.Find("SituationLevel/LevelImage/Text").GetComponent<Text>();
        m_TopProgressText = m_TopOtherInfosTR.transform.Find("ProgressText").GetComponent<Text>();
        m_TopProgressbar = m_TopOtherInfosTR.transform.Find("Slider").GetComponent<Slider>();

        m_TopTime = m_TopDayInfoRootTR.Find("Time").GetComponent<Text>();
        m_TopDate = m_TopDayInfoRootTR.Find("Date").GetComponent<Text>();
        m_TopDay = m_TopDayInfoRootTR.Find("Day").GetComponent<Text>();
    }

    /// <summary>
    /// Find LeftUI Objects
    /// </summary>
    private void FindLeftUI()
    {
        m_AddedUserListRootTR = m_LeftGO.transform.Find("FirstUI/AddedUserList").transform;
        m_UserListRootTR = m_LeftGO.transform.Find("FirstUI/UserList").transform;
        m_UserAddTR = m_UserListRootTR.Find("Viewport/Content").transform;
        m_AddedUserTR = m_AddedUserListRootTR.Find("Viewport/Content").transform;
    }

    /// <summary>
    /// Find CenterUI Objects
    /// </summary>
    private void FindCenterUI()
    {
        InitCrossHair();
        m_DecorationTR = m_CenterGO.transform.Find("DecorationTR").transform;
        //m_CenterUITR = m_CenterGO.transform.Find("")
    }

    /// <summary>
    /// CenterUI Active Toggle
    /// </summary>
    /// <param name="toggle"></param>
    public void SetCenterUIActive(bool toggle)
    {
        if(m_CenterGO != null)
        {
            //m_CenterGO.SetActive(toggle);
            m_DecorationTR.gameObject.SetActive(toggle);
            if(toggle)
            {
                //m_CrossHairBG.SetActive(false);
                //m_CrossHairButton.gameObject.SetActive(false);
                m_CrossHairTR.gameObject.SetActive(false);

            }
        }
    }

    /// <summary>
    /// Disconnected 시 호출
    /// </summary>
    public void InitViewSyncBtnColor()
    {
        m_ViewSyncOnButton.ChangeGroupToggleListColor();
        SetViewSync(true);
    }

    /// <summary>
    /// 지시버튼 오브젝트 초기화
    /// </summary>
    private void InitCrossHair()
    {
        m_CrossHairTR = m_Canvas.transform.Find("CenterUI/CrossHairTR").transform;
        m_CrossHairButton = m_CrossHairTR.Find("CrossHairButton").GetComponent<Button>();
        m_CrossHairBG = m_CrossHairTR.Find("CrossHairBG").gameObject;
        m_CrossHairTouchBG = m_CrossHairTR.Find("CrossHairTouchBG").gameObject;
        m_CrossHairTR.gameObject.SetActive(false);
        m_CrossHairButton.onClick.AddListener(ShootRay);

        m_ViewSyncAlertText = m_CrossHairTR.Find("ViewSyncAlertText").gameObject;
        m_ViewSyncAlertText.SetActive(false);
    }

    /// <summary>
    /// 크로스 헤어 액티브 함수
    /// </summary>
    public void SetActiveCrossHair(bool toggle)
    {
        Debug.Log("SetActive CrossHair");
        //if(m_CrossHairTR.gameObject.activeSelf)
        //{
            m_CrossHairTR.gameObject.SetActive(toggle);
        //}
        //else
        //    m_CrossHairTR.gameObject.SetActive(true);
    }

    /// <summary>
    /// 시선 연동 해제 시 나오는 알림 문구 함수
    /// </summary>
    /// <param name="toggle"></param>
    public void SetActiveViewSyncAlertText(bool toggle)
    {
        if (ShowViewSyncAlertTextIE != null)
            StopCoroutine(ShowViewSyncAlertTextIE);

        if(toggle)
        {
            ShowViewSyncAlertTextIE = ShowViewSyncAlertTextRoutine();
            StartCoroutine(ShowViewSyncAlertTextIE);
        }
        else
        {
            m_ViewSyncAlertText.SetActive(false);
        }
    }
    
    /// <summary>
    /// 대기 리스트에 유저 추가하기
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="NameStr"></param>
    /// <param name="sex"></param>
    /// <param name="PercentStr"></param>
    /// <param name="ExplainStr"></param>
    /// <param name="LevelStr"></param>
    public void AddStandbyUser(string ip, string NameStr, string sex, string PercentStr, string ExplainStr, string LevelStr, bool isStart)
    {
        bool isSame = false;
        for(int i = 0; i < m_UserList.Count; i++)
        {
            //Debug.Log("ip : " + ip);
            //Debug.Log("m_UserList :" + m_UserList[i].ipAddress);
            if(m_UserList[i].ipAddress.Equals(ip))
            {
                isSame = true;
                break;
            }
        }
        if (isSame) return;

        ButtonNetwork button = Instantiate(m_oriButtonNetworkPrefab, Vector3.zero, Quaternion.identity);
        button.SetText(ip, NameStr, sex, PercentStr, ExplainStr, LevelStr);
        button.AddMoveButtonUpEvent(PushAddedUserList);
        SetParentToTarget(m_UserAddTR, button.transform);
        m_UserList.Add(button);
    }

    /// <summary>
    /// Add User에 넣기
    /// </summary>
    public void PushAddedUserList(ButtonNetwork parent)
    {
        SetParentToTarget(m_AddedUserTR, parent.transform);
        parent.ChangeMoveButtonSprite(true);
        parent.SetAllAlpha(true);
        parent.AddUpEvent(ConnectToUser, PopAddedUserList);
    }
    /// <summary>
    /// Add User에서 빼기
    /// </summary>
    /// <param name="parent"></param>
    public void PopAddedUserList(ButtonNetwork parent)
    {
        // 현재 연결중인 상태라면 옮기지 못하게 막기
        //if (parent.Connector != null) return;

        SetParentToTarget(m_UserAddTR, parent.transform);
        parent.ChangeMoveButtonSprite(false);
        parent.SetAllAlpha(true);
        parent.AddUpEvent(null, PushAddedUserList);
    }

    /// <summary>
    /// 해당 유저 연결하기
    /// </summary>
    public void ConnectToUser(string ipAddress)
    {
        Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

        //if (Mng.networkAddress.Equals(ipAddress)) return;
        //Mng.SetIP(ipAddress);
        //Mng.StartGearVRServer(false);
    }

    /// <summary>
    /// 시선 연동/해제 하기
    /// </summary>
    public void SetViewSync(bool toggle)
    {
        DataManager.Inst.isViewScync = toggle;

        SetActiveCrossHair(!toggle);
        SetActiveViewSyncAlertText(!toggle);


    }
    /// <summary>
    /// 강제 종료하기
    /// </summary>
    public void SendAppQuit()
    {
        Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

        //AsyncClient asyncClient = FindObjectOfType<AsyncClient>();
        //if(asyncClient != null)
        //    asyncClient.CmdAppQuit();
    }
    /// <summary>
    /// 유저 연결시 TopUI 변경
    /// </summary>
    /// <param name="isConnected"></param>
    public void ChangeTopUI(bool _isConnected)
    {
        if(_isConnected)
        {
            SetActive(m_TopDayInfoRootTR.gameObject, false);
            SetActive(m_TopUserInfoRootTR.gameObject, true);
            SetActive(m_TopGOSyncButtons, true);
        }
        else
        {
            SetActive(m_TopDayInfoRootTR.gameObject, true);
            SetActive(m_TopUserInfoRootTR.gameObject, false);
            SetActive(m_TopGOSyncButtons, false);
        }
        
    }

    /// <summary>
    /// Target오브젝트로 부모 지정하기
    /// </summary>
    /// <param name="target"></param>
    /// <param name="child"></param>
    private void SetParentToTarget(Transform target, Transform child)
    {
        child.SetParent(target);
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
    }
    private void SetParentToTarget(Transform target, Transform child, Vector3 ToPos, bool isWorldPos)
    {
        child.SetParent(target);
        if (isWorldPos)
            child.position = ToPos;
        else
            child.localPosition = ToPos;

        child.localScale = Vector3.one;
    }

    /// <summary>
    /// Added User List 버튼 클릭 함수
    /// </summary>
    public void ShowAddedUserList()
    {
        if(m_LeftGO.activeInHierarchy)
            SetActive(m_LeftGO, false);
        else
            SetActive(m_LeftGO, true);

        if (m_UserListRootTR.gameObject.activeSelf)
            SetActive(m_UserListRootTR.gameObject, false);
    }

    /// <summary>
    /// Add User 버튼 클릭 함수
    /// </summary>
    public void ShowAndHideStandbyUserList()
    {
        if(m_UserListRootTR.gameObject.activeInHierarchy)
            SetActive(m_UserListRootTR.gameObject, false);
        else
            SetActive(m_UserListRootTR.gameObject, true);
    }

    public List<ButtonNetwork> GetUserList()
    {
        return m_UserList;
    }

    /// <summary>
    /// ip가 같은 ButtonNetwork객체 반환
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <returns></returns>
    public ButtonNetwork FindButton(string ipAddress)
    {
        return m_UserList.Find(x => x.ipAddress.Equals(ipAddress));
    }
    /// <summary>
    /// 클라이언트 연결되면 버튼에 NetworkConnection Setting
    /// </summary>
    /// <param name="btn"></param>
    public void SetConnectedUser(ButtonNetwork btn)
    {
        if(btn == null)
        {
            m_ConnectedUser.SetAllAlpha(true);
            m_ConnectedUser.SetButtonBG(false);
            ButtonNetwork find = FindButton(m_ConnectedUser.ipAddress);
            m_UserList.Remove(find);
            Destroy(find.gameObject);
            //m_ConnectedUser.Connector = null;
            m_ConnectedUser = null;
            return;
        }
        m_ConnectedUser = btn;
        m_ConnectedUser.SetAllAlpha(false);
        m_ConnectedUser.SetButtonBG(true);


    }
    /// <summary>
    /// 클라이언트 연결되면 유저정보 Show
    /// </summary>
    public void StartShowUserInfo()
    {
        StartCoroutine(UserInfoRoutine());
    }

    public void ShootRay()
    {
        Debug.Log("ShootRay");
        UIManager uiManager = FindObjectOfType<UIManager>();
        if(uiManager != null)
        {
            uiManager.ShootRay();
        }
        //RaycastHit target = m_Director.ShootRay();
        //if(target.transform != null)
        //{
        //    GameObject canvas = GameObject.Find("Canvas_FixedUI");

        //    if(canvas != null)
        //    {
        //        DirectionEffect effect = Instantiate(m_oriDirectionEffectPrefab, Vector3.zero, Quaternion.identity);
        //        SetParentToTarget(canvas.transform, effect.transform, target.point, true);
        //    }
        //}
    }

    /// <summary>
    /// EEg Object의 Text 초기화 및 액티브 설정
    /// </summary>
    public void InitEEgData(bool active)
    {
        m_EEGValueTtext.text = "";
        m_EEGParent.gameObject.SetActive(active);
    }

    /// <summary>
    /// ClientCanvasBG SetActive
    /// </summary>
    /// <param name="toggle"></param>
    public void ShowAndHideBG(bool toggle)
    {
        SetActive(m_ClientCanvasBGGO.gameObject, toggle);
    }

    private void SetCanvasGroup(CanvasGroup target, bool check, float showValue)
    {
        target.interactable = check;
        target.alpha = showValue;
    }

    private void SetActive(GameObject target, bool toggle)
    {
        target.SetActive(toggle);
    }

    /// <summary>
    /// Top UI Text Change Routine
    /// </summary>
    /// <returns></returns>
    IEnumerator TimerRoutine()
    {
        WaitForSeconds WFS = new WaitForSeconds(10.0f);
        while(!isQuit)
        {
            m_TopTime.text = System.DateTime.Now.ToString("HH:mm");
            m_TopDate.text = System.DateTime.Now.ToString("yyyy-MM-dd");
            m_TopDay.text = System.DateTime.Now.ToString("dddd");
            yield return WFS;
        }
    }

    /// <summary>
    /// if Connected,Top UI Show UserInfomation Routine
    /// </summary>
    /// <returns></returns>
    IEnumerator UserInfoRoutine()
    {
        if (m_ConnectedUser == null) yield break;
        WaitForEndOfFrame EOF = new WaitForEndOfFrame();
        WaitForSeconds WaitSec = new WaitForSeconds(0.5f);
        int SituationTextLength = 0;
        m_TopOtherInfosTR.gameObject.SetActive(false);
        Image eegImage = m_EEGParent.GetComponent<Image>();

        while (m_ConnectedUser != null)
        {
            m_TopUserNameText.text = m_ConnectedUser.UserNameText.text;
            
            if(m_ConnectedUser.isMissionStart)
            {
                m_TopOtherInfosTR.gameObject.SetActive(true);
                m_TopSituationText.text = m_ConnectedUser.ExplainText.text;
                SituationTextLength = m_TopSituationText.text.Length;
                m_TopSituationText.rectTransform.sizeDelta = new Vector2(
                    SituationTextLength * 28, m_TopSituationText.rectTransform.sizeDelta.y);
                m_TopLevelText.text = m_ConnectedUser.LevelText.text;
                m_TopProgressText.text = m_ConnectedUser.PercentText.text;
                m_TopProgressbar.value = m_ConnectedUser.PercentVal;

                m_EEGParent.gameObject.SetActive(true);
                
                float ratio = float.Parse(m_ConnectedUser.EegValueStr);
                ratio = ratio / 0.7f;

                Color lerpColor = Color.Lerp(Color.yellow, Color.red, ratio);

                eegImage.color = lerpColor;
                m_EEGValueTtext.text = m_ConnectedUser.EegValueStr;

                Vector2 newSize;
                newSize = new Vector2((SituationTextLength * 28) + m_TopLevelText.rectTransform.sizeDelta.x + 10, m_TopLevelText.rectTransform.sizeDelta.y);
                m_TopSituationLevel.sizeDelta = newSize;
                
            }
            else
            {
                m_TopOtherInfosTR.gameObject.SetActive(false);
                m_TopLevelText.text = "훈련 선택 중";
                m_TopProgressText.text = "0%";
                m_TopProgressbar.value = 0;
                m_TopOtherInfosTR.gameObject.SetActive(false);
                eegImage.color = Color.yellow;
                m_EEGParent.gameObject.SetActive(false);
            }
            yield return WaitSec;
            yield return EOF;
        }
        m_TopLevelText.text = "초급";
        m_TopProgressText.text = "0%";
        m_TopProgressbar.value = 0;
        m_TopOtherInfosTR.gameObject.SetActive(false);
        eegImage.color = Color.yellow;
        m_EEGParent.gameObject.SetActive(false);
    }

    private IEnumerator ShowViewSyncAlertTextRoutine()
    {
        WaitForSeconds ThreeSec = new WaitForSeconds(3.0f);
        m_ViewSyncAlertText.SetActive(true);
        yield return ThreeSec;
        m_ViewSyncAlertText.SetActive(false);
    }

    public override void OnApplicationQuit()
    {
        isQuit = true;
        base.OnApplicationQuit();
    }
}
