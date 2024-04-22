using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Unity.XR.CoreUtils;

public class SceneBase : MonoBehaviour
{
    public Transform ovrCamera1;
    /// <summary>
    /// 오른쪽 컨트롤러(주용도 : 숨기기)
    /// </summary>
    public GameObject RightController;
    /// <summary>
    /// 과제수행 변수
    /// </summary>
    //public FocusGame focusGame;
    public NewFocusGame focusGame;
    /// <summary>
    /// 360영상 변수(Play, Stop 등)
    /// </summary>
    public VideoController videoPlayer;

    /// <summary>
    /// EndSeesion에서 Vas값 조절을 위한 Slider 변수
    /// </summary>
    public SliderFunc slider;

    /// <summary>
    /// 과제 수행 시작 하였는가 체크
    /// </summary>
    public bool isGameStart = false;
    /// <summary>
    /// 버튼에 접촉해 있는가
    /// </summary>
    public bool isButtonEnter = false;

    /// <summary>
    /// 과제수행 전체 개수
    /// </summary>
    protected float FocusTotalCount = 0.0f;
    /// <summary>
    /// AOI 총 시간
    /// </summary>
    protected float AOITotalTime = 0.0f;

    /// <summary>
    /// 과제수행 걸린 시간
    /// </summary>
    public float FocusPlayTime = 0.0f;

    /// <summary>
    /// 방해요소 단계
    /// </summary>
    protected int DiscouragementLevel = 0;
    /// <summary>
    /// Level Index
    /// </summary>
    protected int LevelIndex = 0;


    /// <summary>
    /// 훈련 연습에서 2번째 나레이션 판단 변수
    /// </summary>
    protected bool isPracticeNextNarration = false;
    /// <summary>
    /// 방해요소를 중단하는가에 대한 판단 변수
    /// </summary>
    protected bool IsDiscouragementStop = false;
    /// <summary>
    /// 결과보기에서 정답률 텍스트
    /// </summary>
    protected Text AnswerPercentText;
    /// <summary>
    /// 결과보기에서 시선집중도 텍스트
    /// </summary>
    protected Text AOIPercentText;
    /// <summary>
    /// 결과보기에서 Exellent, Good, Let's Retry 텍스트
    /// </summary>
    protected Text ResultText;

    /// <summary>
    /// 일반 버튼 클릭음
    /// </summary>
    protected AudioClip clip;
    /// <summary>
    /// 상황별 Level1
    /// </summary>
    protected LevelBase level1;
    /// <summary>
    /// 상황별 Level2
    /// </summary>
    protected LevelBase level2;
    /// <summary>
    /// 상황별 Level3
    /// </summary>
    protected LevelBase level3;
    /// <summary>
    /// 상황별 연습 시작 버튼
    /// </summary>
    protected ButtonFunc PracticeButton;
    /// <summary>
    /// 상황별 연습 종료 버튼
    /// </summary>
    protected ButtonFunc PracticeEndButton;
    /// <summary>
    /// 결과보기에서 정답률과 시선집중도 계산 IE 
    /// </summary>
    protected IEnumerator ScoreIE;
    /// <summary>
    /// 컨트롤러 PrimaryIndexTrigger 클릭음
    /// </summary>
    protected AudioClip DefaultClick;
    /// <summary>
    /// 버튼 객체 클릭음
    /// </summary>
    protected AudioClip ButtonClick;
    /// <summary>
    /// 연습하기->버튼 클릭음
    /// </summary>
    protected AudioClip PracticeClick;
    /// <summary>
    /// Scene객체의 AudioSource
    /// </summary>
    protected AudioSource audioSource;

    protected Image JoyStickImg;
    protected Image[] JoyStickImgs;

    protected bool isShowJoyStickImg = false;

    protected IEnumerator JoyStickIE;

    protected CanvasGroup Intro;
    protected CanvasGroup Practice;
    protected CanvasGroup SelectLevel;
    protected CanvasGroup EndSession;
    protected CanvasGroup FeedBack;
    protected CanvasGroup Report;
    protected CanvasGroup Purchase;
    ///protected CanvasGroup Back;

    protected List<CanvasGroup> CanvasList = new List<CanvasGroup>();

    protected ButtonFunc StartButton;

    protected ButtonFunc Level1;
    protected ButtonFunc Level2;
    protected ButtonFunc Level3;

    protected ButtonFunc CommitButton;

    protected ButtonFunc ReportButton;
    protected ButtonFunc RetryButton;
    protected ButtonFunc NextLevelButton;
    protected ButtonFunc EndButton;

    protected ButtonFunc ReportEndButton;

    protected ButtonFunc PurchaseCancleButton;
    protected ButtonFunc PurchaseButton;
    protected ButtonFunc BackButton;

    protected List<ButtonFunc> ButtonList = new List<ButtonFunc>();

    public bool isTest = false;

    protected IEnumerator WaitForSecAndFuncIE;
    protected IEnumerator SetRotationIE;
    protected IEnumerator EnterFuncIE;
    /// <summary>
    /// 방안에서, 도서관에서 사용할 IE
    /// </summary>
    protected IEnumerator DiscouragementSequenceIE;
    [SerializeField]
    protected MeshRenderer[] meshRender;

    /// <summary>
    /// 연결된 클라이언트 객체
    /// </summary>
    //protected AsyncClient ConnectedClient;

    /// <summary>
    /// 방향 지시를 하기 위한 캔버스와 2D 게임 오브젝트 클래스
    /// </summary>
    protected RotateCompass rotateCompass;

    //public void OVR_Mounted()
    //{
    //    //OVRManager.HMDMounted += OVRHMDMountedFunc;
    //}
    //public void OVR_UnMounted()
    //{
    //    //OVRManager.HMDUnmounted += OVRHMDUnMountedFunc;
    //}

    public void OVRHMDMountedFunc()
    {
        ClientOVRMounted(true);
        if (videoPlayer != null)
        {
            if (focusGame != null)
            {
                if (focusGame.isStart)
                    videoPlayer.VideoPlay2();
            }

        }
        DataManager.Inst.isPause = false;
        Time.timeScale = 1.0f;
        UIManager.Inst.SetNarrationPitch(1);

    }
    public void OVRHMDUnMountedFunc()
    {
        ClientOVRMounted(false);
        if (videoPlayer != null)
        {
            if (focusGame != null)
            {
                if (focusGame.isStart)
                    videoPlayer.VideoPause();
            }

        }
        DataManager.Inst.isPause = true;
        Time.timeScale = 0.0f;
        UIManager.Inst.SetNarrationPitch(0);
    }

    private void ClientOVRMounted(bool toggle)
    {
        Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

        //if (ConnectedClient != null)
        //{
        //    if (ConnectedClient.isServer)
        //    {
        //        ConnectedClient.RpcOVRMounted(toggle);
        //    }
        //}
    }

    protected virtual void Awake()
    {
        //UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 2.0f;
    }

    protected virtual void IsMeshVisible(params UnityAction[] callback)
    {
        EnterFuncIE = IsMeshVisibleRoutine(callback);
        StartCoroutine(EnterFuncIE);
    }

    protected virtual void IsAssetLoad(params UnityAction[] callback)
    {

    }

    protected virtual void EnterFunc()
    {

    }

    /// <summary>
    /// 결제 여부를 확인 App Store Upload Version
    /// </summary>    
    //protected void CheckPurchase()
    //{
    //    if(DataManager.Inst.GetIsPurchase)
    //    {

    //    }
    //    else
    //    {            
    //        SchoolButton.IsCanTouch = false;
    //        SchoolButton.GetSetLock = true;
    //        //SchoolButton.ChangeHoverImage(SchoolButton.DefaultSprite, SchoolButton.LockSprite);
    //        TouristButton.IsCanTouch = false;
    //        TouristButton.GetSetLock = true;
    //        //TouristButton.ChangeHoverImage(TouristButton.DefaultSprite, TouristButton.LockSprite);            
    //    }
    //}

    protected virtual void Start()
    {
        //OVR_Mounted();
        //OVR_UnMounted();
        StartCoroutine(CurvedSettingRoutine());
        StartCoroutine(SetFocusCurvedMeshColliderRoutine(false));

        UIManager.Inst.ExitPoint(delegate { ExitPoint(); });
        RightController = GameObject.Find("RightHandAnchor");
#if UNITY_EDITOR
        //if (RightController != null)
        //    RightController.SetActive(false);
        //if (GameObject.Find("CurvedUILaserBeam") != null)
        //{
        //    var LaserBeam = GameObject.Find("CurvedUILaserBeam");
        //    LaserBeam.SetActive(false);
        //}
#else        
#endif
        if (isTest)
        {
            SettingCanvasAndButtonData();
            AddListener();
        }

        if (DataManager.Inst.CurrentLevel.Length != 0 && DataManager.Inst.CurrentLevelType != LevelType.None)
        {
            SettingCanvasAndButtonData();
            AddListener();
        }

        DefaultClick = Resources.Load<AudioClip>("Sound/Button/DpadClick");
        ButtonClick = Resources.Load<AudioClip>("Sound/Button/ButtonClick");
        PracticeClick = Resources.Load<AudioClip>("Sound/Button/DpadClick");
        audioSource = GetComponent<AudioSource>();
        //UIManager.Inst.FullFadeOut();
        StartCoroutine(FindAsyncClient());
        SetRotateCompass();
    }

    private void TestMode()
    {
        SettingCanvasAndButtonData();
        AddListener();
    }

    protected virtual void SettingCanvasAndButtonData()
    {

        Transform Parent = GameObject.Find("Canvas_FixedUI").transform;
        //try
        //{
        level1 = Parent.transform.Find("Level1").GetComponent<LevelBase>();
        level2 = Parent.transform.Find("Level2").GetComponent<LevelBase>();
        level3 = Parent.transform.Find("Level3").GetComponent<LevelBase>();

        Intro = Parent.transform.Find("Intro").GetComponent<CanvasGroup>();
        Practice = Parent.transform.Find("Practice").GetComponent<CanvasGroup>();
        SelectLevel = Parent.transform.Find("SelectLevel").GetComponent<CanvasGroup>();
        EndSession = Parent.transform.Find("EndSesstion").GetComponent<CanvasGroup>();
        FeedBack = Parent.transform.Find("FeedBack").GetComponent<CanvasGroup>();
        Report = Parent.transform.Find("Report").GetComponent<CanvasGroup>();
        Purchase = Parent.transform.Find("PurchasePopUp").GetComponent<CanvasGroup>();
        //Back = Parent.transform.Find("Back").GetComponent<CanvasGroup>();

        AnswerPercentText = FeedBack.transform.Find("AnswerPercent").GetComponent<Text>();
        AOIPercentText = FeedBack.transform.Find("AOIPercent").GetComponent<Text>();

        StartButton = Intro.transform.Find("StartButton").GetComponent<ButtonFunc>();
        PracticeButton = Intro.transform.Find("PracticeButton").GetComponent<ButtonFunc>();
        PracticeEndButton = Practice.transform.Find("PracticeEndButton").GetComponent<ButtonFunc>();

        Level1 = SelectLevel.transform.Find("Level1").GetComponent<ButtonFunc>();
        Level2 = SelectLevel.transform.Find("Level2").GetComponent<ButtonFunc>();
        Level3 = SelectLevel.transform.Find("Level3").GetComponent<ButtonFunc>();

        CommitButton = EndSession.transform.Find("CommitButton").GetComponent<ButtonFunc>();
        ReportButton = FeedBack.transform.Find("ReportButton").GetComponent<ButtonFunc>();
        RetryButton = FeedBack.transform.Find("RetryButton").GetComponent<ButtonFunc>();
        NextLevelButton = FeedBack.transform.Find("NextLevelButton").GetComponent<ButtonFunc>();
        EndButton = FeedBack.transform.Find("EndButton").GetComponent<ButtonFunc>();
        BackButton = FeedBack.transform.Find("BackButton").GetComponent<ButtonFunc>();

        ReportEndButton = Report.transform.Find("ReportEndButton").GetComponent<ButtonFunc>();

        PurchaseCancleButton = Purchase.transform.Find("PurchaseCancleButton").GetComponent<ButtonFunc>();
        PurchaseButton = Purchase.transform.Find("PurchaseButton").GetComponent<ButtonFunc>();

        ResultText = FeedBack.transform.Find("Result").GetComponent<Text>();

        UIManager.Inst.LoadButtonData("Intro", string.Format("{0}/{1}", gameObject.name, "Intro/"));
        UIManager.Inst.LoadButtonData("Practice", string.Format("{0}/{1}", gameObject.name, "Practice/"));
        UIManager.Inst.LoadButtonData("SelectLevel", string.Format("{0}/{1}", gameObject.name, "SelectLevel/"));
        UIManager.Inst.LoadButtonData("EndSession", string.Format("{0}/{1}", gameObject.name, "EndSession/"));
        UIManager.Inst.LoadButtonData("FeedBack", string.Format("{0}/{1}", gameObject.name, "FeedBack/"));
        UIManager.Inst.LoadButtonData("Back", string.Format("{0}/{1}", gameObject.name, "Back/"));
        UIManager.Inst.LoadButtonData("Report", string.Format("{0}/{1}", gameObject.name, "Report/"));
        UIManager.Inst.LoadButtonData("Purchase", string.Format("{0}/{1}", gameObject.name, "Purchase/"));
        UIManager.Inst.SettingButtonData("Intro", StartButton, PracticeButton);
        UIManager.Inst.SettingButtonData("Practice", PracticeEndButton);
        UIManager.Inst.SettingButtonData("SelectLevel", Level1, Level2, Level3);
        UIManager.Inst.SettingButtonData("EndSession", CommitButton);
        UIManager.Inst.SettingButtonData("FeedBack", ReportButton, RetryButton, NextLevelButton, EndButton, BackButton);
        UIManager.Inst.SettingButtonData("Report", ReportEndButton);
        UIManager.Inst.SettingButtonData("Purchase", PurchaseCancleButton, PurchaseButton);

        //UIManager.Inst.SettingButtonData("Back", BackButton);

        AddCanvas(CanvasList, Intro, Practice, SelectLevel, EndSession, FeedBack, Report, Purchase);

        //AddButtons(IntroButtonList, StartButton, PracticeButton);
        //AddButtons(PracticeButtonList, PracticeEndButton);
        //AddButtons(SelectLevelButtonList, ParkButton, SchoolButton, TouristButton);
        //AddButtons(EndSessionButtonList, CommitButton);
        //AddButtons(FeedBackButtonList, ReportButton, RetryButton, NextLevelButton, EndButton);

        AddButtons(ButtonList, StartButton, PracticeButton, PracticeEndButton, Level1,
                    Level2, Level3, CommitButton, ReportButton, RetryButton,
                    NextLevelButton, EndButton, ReportButton, RetryButton, NextLevelButton,
                    EndButton, ReportEndButton, PurchaseButton, PurchaseCancleButton, BackButton);
        //}
        //catch (System.Exception e)
        //{

        //    Debug.Log(name + ": " + e.Message);
        //}
    }

    private void AddButtons(List<ButtonFunc> list, params ButtonFunc[] btns)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            list.Add(btns[i]);
        }
    }
    private void AddCanvas(List<CanvasGroup> list, params CanvasGroup[] canvas)
    {
        for (int i = 0; i < canvas.Length; i++)
        {
            list.Add(canvas[i]);
        }
    }

    protected virtual void AddListener()
    {
        try
        {
            StartButton.AddPressedListener(Step2);
            PracticeButton.AddPressedListener(PracticeFunc);
            PracticeEndButton.AddPressedListener(PracticeEnd);
            Level1.AddPressedListener(Level1Step);
            Level2.AddPressedListener(Level2Step);
            Level3.AddPressedListener(Level3Step);

            //CommitButton.AddPressedListener(delegate { AddVasData(); }, delegate { ShowFeedBack(); });
            CommitButton.AddPressedListener(AddVasData);

            BackButton.AddPressedListener(GoToBackTitle);

            ReportButton.AddPressedListener(ShowGraphMenu);
            RetryButton.AddPressedListener(LevelReTry);
            NextLevelButton.AddPressedListener(NextLevel);
            EndButton.AddPressedListener(ApplicationQuit);

            ReportEndButton.AddPressedListener(GoToFeedBack);

            AddEnterListener(ButtonList.ToArray(), delegate { EnterPoint(); });
            AddExitListener(ButtonList.ToArray(), delegate { ExitPoint(); });
            AddClickSoundListener(ButtonList.ToArray(), delegate { ButtonClickSound(); });
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }


        Debug.Log("<color=yellow> AddListener End </color>");
    }

    private void AddEnterListener(ButtonFunc[] btns, params UnityAction[] func)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            for (int j = 0; j < func.Length; j++)
            {
                btns[i].AddEnterActionListener(func[j]);
            }

        }
    }
    private void AddPressedListener(ButtonFunc[] btns, params PressedEvent[] func)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            for (int j = 0; j < func.Length; j++)
            {
                btns[i].AddPressedListener(func[j]);
            }

        }
    }
    private void AddExitListener(ButtonFunc[] btns, params UnityAction[] func)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            for (int j = 0; j < func.Length; j++)
            {
                btns[i].AddExitActionListener(func[j]);
            }

        }
    }
    private void AddClickSoundListener(ButtonFunc[] btns, params UnityAction[] func)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            for (int j = 0; j < func.Length; j++)
            {
                btns[i].AddClickSoundActionListener(func[j]);
            }

        }
    }
    /// <summary>
    /// Level 선택 화면 전환
    /// </summary>
    public virtual void Step2()
    {
        UIManager.Inst.SkipNarration();
        UIManager.Inst.HideAndInteractableCanvasGroup(Intro);
        UIManager.Inst.ShowAndInteractableCanvasGroup(SelectLevel);
    }


    public virtual void Level1Step()
    {

        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
    }
    public virtual void Level2Step()
    {
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        // App Store Upload Version

        //if (DataManager.Inst.GetIsPurchase)
        //{

        //}
        //else
        //{
        //    for(int i = 0; i < ButtonList.Count; i++)
        //    {
        //        if(ButtonList[i].isActiveAndEnabled)
        //            ButtonList[i].SetTextEnabled(true);
        //    }
        //    UIManager.Inst.SetFullFade2(true, 1);
        //    UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        //    UIManager.Inst.ShowAndInteractableCanvasGroup(Purchase);
        //    PurchaseCancleButton.AddPressedListener(delegate { BackToSelectLevel(); });
        //    PurchaseButton.AddPressedListener(
        //        delegate 
        //        {
        //            InAppSys.Inst.BuyHDVideo(
        //                delegate
        //                {                            
        //                    BackToSelectLevel();
        //                    SchoolButton.IsCanTouch = true;
        //                    SchoolButton.GetSetLock = false;
        //                    TouristButton.IsCanTouch = true;
        //                    TouristButton.GetSetLock = false;                            
        //                });
        //        }
        //    );

        //}
    }
    public virtual void Level3Step()
    {
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        // App Store Upload Version

        //if (DataManager.Inst.GetIsPurchase)
        //{

        //}
        //else
        //{
        //    for (int i = 0; i < ButtonList.Count; i++)
        //    {
        //        if (ButtonList[i].isActiveAndEnabled)
        //            ButtonList[i].SetTextEnabled(true);
        //    }
        //    UIManager.Inst.SetFullFade2(true, 1);
        //    UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        //    UIManager.Inst.ShowAndInteractableCanvasGroup(Purchase);
        //    PurchaseCancleButton.AddPressedListener(delegate { BackToSelectLevel(); });
        //    PurchaseButton.AddPressedListener(
        //        delegate
        //        {
        //            InAppSys.Inst.BuyHDVideo(
        //                delegate
        //                {
        //                    BackToSelectLevel();
        //                    SchoolButton.IsCanTouch = true;
        //                    SchoolButton.GetSetLock = false;
        //                    TouristButton.IsCanTouch = true;
        //                    TouristButton.GetSetLock = false;                            
        //                });
        //        }
        //    );            
        //}
    }
    private void BackToSelectLevel()
    {
        Debug.Log("BackToSelectLevel");
        UIManager.Inst.SetFullFade2(false, 0);
        UIManager.Inst.HideAndInteractableCanvasGroup(Purchase);
        UIManager.Inst.ShowAndInteractableCanvasGroup(SelectLevel);
    }
    public virtual void ShowJoyStick()
    {
        JoyStickIE = ShowJoyStickImgRoutine();
        StartCoroutine(JoyStickIE);
    }
    public virtual void HideJoyStick()
    {
        Debug.Log("HideJoyStick");
        StopCoroutine(JoyStickIE);
        JoyStickImg.gameObject.SetActive(false);
        focusGame.ShowFigure();
    }
    public void SetLevelIndex(int index)
    {
        LevelIndex = index;
    }
    /// <summary>
    /// 포커스 게임 시작. 매개변수로 FocusGameEnd()를 넣어준다.
    /// </summary>
    public virtual void FocusGameStart()
    {
        //UIManager.Inst.SetResolutionScale(1);
        UIManager.Inst.ShowAndHideCanvasFixedUI(false);
        focusGame.StopFigure();

        Debug.Log("SceneStreet FocusGameStart");
        // AOI 시작
        HeadTracer.Inst.StartAOI();
        FocusTotalCount = 0;
        AOITotalTime = 0;
        DataManager.Inst.SetTotalTime(0);
        // 게임 시작
        isGameStart = true;
        DataManager.Inst.SetTimeStart(true);
        focusGame.StartFocusGame(delegate { FocusGameEnd(); });

        if (MainBioSignal.Inst != null)
        {
            if (MainBioSignal.Inst.GetConnectMng.IsConnected())
            {
                MainBioSignal.Inst.ConnectBluetooth();
                MainBioSignal.Inst.GetConnectMng.Start();
                MainBioSignal.Inst.GetRec.RecordStart(); ;
            }
        }
    }
    /// <summary>
    /// 포커스 게임이 끝나면 실행됨. FocusGameStart에서 매개변수로 넣어준 이 함수가 실행 됨
    /// </summary>
    public virtual void FocusGameEnd()
    {
        if (MainBioSignal.Inst != null)
        {
            if (MainBioSignal.Inst.GetConnectMng.IsConnected())
            {
                MainBioSignal.Inst.GetRec.RecordStop();
                MainBioSignal.Inst.GetConnectMng.Stop();
            }
        }

        //UIManager.Inst.SetResolutionScale(2);
        DataManager.Inst.SetTimeStart(false);
        isGameStart = false;

        // 과제수행 중에 List에 넣은 개수
        FocusTotalCount = DataManager.Inst.FocusListCount();
        // 포커스 게임의 시퀀스 리스트의 개수 == AOI의 TotalTime
        AOITotalTime = DataManager.Inst.GetTotalTime;

        Debug.Log("End Time : " + DataManager.NowToString());
        Debug.Log("FocusGame End");
        UIManager.Inst.ShowAndHideCanvasFixedUI(true);
        focusGame.SetActive(false);
        if (videoPlayer != null)
            videoPlayer.VideoPause();

        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_06"), "", false);
        EndSessionStep();
    }

    protected virtual void EndSetting()
    {

    }

    /// <summary>
    /// 방안에서, 도서관에서 사용되는 함수
    /// 시간이 지남에 따라 방해요소 함수 활성화
    /// 방해요서 함수는 Phase1,2,3...으로 delegate로 넣어야 함
    /// 반드시 경과시간 Count와 callBack Count가 같아야 함
    /// </summary>
    /// <param name="elapseTimes">경과시간 배열</param>
    /// <param name="callBacks">경과시간에 따라 실행 할 함수 배열</param>
    protected virtual void StartDiscouragement(float[] elapseTimes, UnityAction[] callBacks)
    {
        Debug.Log("elapseTimes Length : " + elapseTimes.Length);
        Debug.Log("callBacks Length : " + callBacks.Length);
        if (elapseTimes.Length != callBacks.Length)
        {
            Debug.Log("elaseTime.Length != callBacks.Length");
            return;
        }

        IsDiscouragementStop = false;
        DiscouragementSequenceIE = DiscouragementSequenceRoutine(elapseTimes, callBacks);
        StartCoroutine(DiscouragementSequenceIE);
    }
    protected void StopDiscouragement()
    {
        IsDiscouragementStop = true;
        if (DiscouragementSequenceIE != null)
            StopCoroutine(DiscouragementSequenceIE);
    }

    private IEnumerator DiscouragementSequenceRoutine(float[] elapseTimes, UnityAction[] callBacks)
    {
        int cnt = elapseTimes.Length;
        int index = 0;
        //WaitForSeconds[] wfs = new WaitForSeconds[cnt];

        while (!IsDiscouragementStop)
        {
            Debug.Log("다음 방해요소는 " + elapseTimes[index] + "초 후");
            yield return new WaitForSeconds(elapseTimes[index]);
            callBacks[index].Invoke();

            if (index < cnt - 1)
                index++;
            else
            {
                IsDiscouragementStop = true;
            }
        }
    }
    protected virtual void EndSessionStep()
    {
        DataManager.Inst.SceneName = "결과화면";
        UIManager.Inst.ShowAndInteractableCanvasGroup(EndSession);

        CalcScore();
        CommitButton.dummyInteractable = false;
        CommitButton.Interactable = false;
        if (DataManager.Inst.isServer)
        {
            slider = EndSession.transform.Find("Slider").GetComponent<SliderFunc>();
            slider.slider.onValueChanged.AddListener(SendClientVasValue);
        }
    }

    private void SendClientVasValue(float val)
    {
        CommitButton.dummyInteractable = true;
        CommitButton.Interactable = true;

        Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

        //if (ConnectedClient != null)
        //{
        //    ConnectedClient.RpcSetVasValue(val);
        //    // 2019.08.21 수정

        //    if (ConnectedClient != null)
        //    {
        //        ConnectedClient.RpcSetResultData(
        //            DataManager.Inst.FocusScore,
        //            DataManager.Inst.AOIScore,
        //            DataManager.Inst.RTV,
        //            DataManager.Inst.CV,
        //            DataManager.Inst.meanTBR
        //        );
        //        ConnectedClient.RpcSetGraphData(
        //            DataManager.Inst.AnswerCountList.ToArray(),
        //            DataManager.Inst.AOICountList.ToArray(),
        //            DataManager.Inst.EEGRatioList.ToArray()
        //            );
        //    }
        //}
    }

    public void AddVasData()
    {
        Debug.Log("AddVasData");
        slider = EndSession.transform.Find("Slider").GetComponent<SliderFunc>();
        VasData data = new VasData(DataManager.Inst.CurrentLevel, DataManager.Inst.CurrentLevelType, DataManager.NowToString());
        DataManager.Inst.AddVasData(data);
        DataManager.Inst.CurrentVasData = data;
        DataManager.Inst.AddVasDataValue(slider.SliderValue, DataManager.NowToString());

        // 2019.05.13 수정
        ShowFeedBack();
    }

    protected void CalcScore()
    {
        DataManager.Inst.InitValues();

        DataManager.Inst.FocusScore = CalculateFocusScore(DataManager.Inst.CurrentLevelType);
        DataManager.Inst.AOIScore = CalculateAOIScore(DataManager.Inst.CurrentLevelType);
        DataManager.Inst.RTV = DataManager.Inst.GetRTV(DataManager.Inst.CurrentFocusData);
        DataManager.Inst.CV = DataManager.Inst.GetCV(DataManager.Inst.CurrentFocusData);
        DataManager.Inst.meanTBR = DataManager.Inst.calculateBetaThetaRatio(0, 900);

        DataManager.Inst.AnswerCountList = DataManager.Inst.GetAnswerCountList(DataManager.Inst.CurrentLevelType);
        DataManager.Inst.AOICountList = DataManager.Inst.GetAOICountList(DataManager.Inst.CurrentLevelType);
        DataManager.Inst.EEGRatioList = new List<float>();
    }

    protected virtual void ShowFeedBack()
    {
        EndSession.transform.GetComponentInChildren<Slider>().value = 0;
        UIManager.Inst.HideAndInteractableCanvasGroup(EndSession);
        UIManager.Inst.ShowAndInteractableCanvasGroup(FeedBack);

        ShowScore(DataManager.Inst.FocusScore, DataManager.Inst.AOIScore);

        if (MainBioSignal.Inst != null)
        {
            if (MainBioSignal.Inst.GetConnectMng.IsConnected())
            {
                // 점수에 따라 음성 재생
                if (ChooseNarration(true, DataManager.Inst.FocusScore, DataManager.Inst.meanTBR) == 1)
                {
                    ResultText.text = "Exellent!";
                    UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_07-1"), "", false,
                    delegate { ShowFeedBack2(); }
                    );
                }
                else if (ChooseNarration(true, DataManager.Inst.FocusScore, DataManager.Inst.meanTBR) == 2)
                {
                    ResultText.text = "Good!";
                    UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_07-2"), "", false,
                    delegate { ShowFeedBack2(); }
                    );
                }
                else if (ChooseNarration(true, DataManager.Inst.FocusScore, DataManager.Inst.meanTBR) == 3)
                {
                    ResultText.text = "Let's Retry!";
                    UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_07-3"), "", false,
                    delegate { ShowFeedBack2(); }
                    );
                }
            }
            else
            {
                // 점수에 따라 음성 재생
                if (ChooseNarration(false, DataManager.Inst.FocusScore) == 1)
                {
                    ResultText.text = "Exellent!";
                    UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_07-1"), "", false,
                    delegate { ShowFeedBack2(); }
                    );
                }
                else if (ChooseNarration(false, DataManager.Inst.FocusScore) == 2)
                {
                    ResultText.text = "Good!";
                    UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_07-2"), "", false,
                    delegate { ShowFeedBack2(); }
                    );
                }
                else if (ChooseNarration(false, DataManager.Inst.FocusScore) == 3)
                {
                    ResultText.text = "Let's Retry!";
                    UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_07-3"), "", false,
                    delegate { ShowFeedBack2(); }
                    );
                }
            }
        }
        else
        {
            // 점수에 따라 음성 재생
            if (ChooseNarration(false, DataManager.Inst.FocusScore) == 1)
            {
                ResultText.text = "Exellent!";
                UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_07-1"), "", false,
                delegate { ShowFeedBack2(); }
                );
            }
            else if (ChooseNarration(false, DataManager.Inst.FocusScore) == 2)
            {
                ResultText.text = "Good!";
                UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_07-2"), "", false,
                delegate { ShowFeedBack2(); }
                );
            }
            else if (ChooseNarration(false, DataManager.Inst.FocusScore) == 3)
            {
                ResultText.text = "Let's Retry!";
                UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ALL_07-3"), "", false,
                delegate { ShowFeedBack2(); }
                );
            }
        }

        /* App Store Upload Version 업로드 버전으로 인한 주석 */
        switch (DataManager.Inst.CurrentLevelType)
        {
            case LevelType.ROOM_LEVEL0:
            case LevelType.LIBRARY_LEVEL0:
            case LevelType.STREET_LEVEL0:
            case LevelType.CAFE_LEVEL0:
                break;
            case LevelType.ROOM_LEVEL1:
            case LevelType.LIBRARY_LEVEL1:
            case LevelType.STREET_LEVEL1:
            case LevelType.CAFE_LEVEL1:
                DataManager.Inst.saveCSV.CreateVariableData(DataManager.Inst.CurrentLevelType, 0);
                break;

            case LevelType.ROOM_LEVEL2:
            case LevelType.LIBRARY_LEVEL2:
            case LevelType.STREET_LEVEL2:
            case LevelType.CAFE_LEVEL2:
                DataManager.Inst.saveCSV.CreateVariableData(DataManager.Inst.CurrentLevelType, 1);
                break;

            case LevelType.ROOM_LEVEL3:
            case LevelType.LIBRARY_LEVEL3:
            case LevelType.STREET_LEVEL3:
            case LevelType.CAFE_LEVEL3:
                DataManager.Inst.saveCSV.CreateVariableData(DataManager.Inst.CurrentLevelType, 2);
                break;
        }
    }

    protected virtual void ShowFeedBack2()
    {
        // 음성 재생 후 버튼 활성화
        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration(""), "", false,
            delegate { }
            );
    }

    /// <summary>
    /// 다시 도전
    /// </summary>
    public virtual void LevelReTry()
    {
        DataManager.Inst.InitValues();
        DataManager.Inst.data.DatasClear();
        UIManager.Inst.HideAndInteractableCanvasGroup(FeedBack);
        switch (DataManager.Inst.CurrentLevelType)
        {
            case LevelType.ROOM_LEVEL1:
            case LevelType.LIBRARY_LEVEL1:
            case LevelType.CAFE_LEVEL1:
            case LevelType.STREET_LEVEL1:
                Level1Step();
                break;
            case LevelType.ROOM_LEVEL2:
            case LevelType.LIBRARY_LEVEL2:
            case LevelType.CAFE_LEVEL2:
            case LevelType.STREET_LEVEL2:
                Level2Step();
                break;
            case LevelType.ROOM_LEVEL3:
            case LevelType.LIBRARY_LEVEL3:
            case LevelType.CAFE_LEVEL3:
            case LevelType.STREET_LEVEL3:
                Level3Step();
                break;
        }
    }

    /// <summary>
    /// 다음 레벨로 이동
    /// </summary>
    public virtual void NextLevel()
    {
        DataManager.Inst.InitValues();
        DataManager.Inst.data.DatasClear();
        UIManager.Inst.SkipNarration();
        UIManager.Inst.HideAndInteractableCanvasGroup(FeedBack);
        UIManager.Inst.ShowAndInteractableCanvasGroup(SelectLevel);
    }

    /// <summary>
    /// 프로그램 종료
    /// </summary>
    public virtual void ApplicationQuit()
    {
        Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

        Application.Quit();

        //if (ConnectedClient != null)
        //{
        //    if (ConnectedClient.isServer)
        //        Application.Quit();
        //}
        //else
        //    Application.Quit();
    }

    /// <summary>
    /// 루프영상 재생 및 시작시간과 끝나는 시간 기입
    /// </summary>
    /// <param name="start">시작 시간</param>
    /// <param name="end">끝나는 시간</param>
    public virtual void VideoLoopPlay(long start, long end)
    {

    }
    /// <summary>
    /// 영상 재생
    /// </summary>
    public virtual void VideoPlay()
    {

    }
    /// <summary>
    /// 영상 정지
    /// </summary>
    public virtual void StopLoopPlayer()
    {
        Debug.Log("SceneBase.StopLoopPlayer");
        videoPlayer.StopVideoLoopPlay();
        //youtubePlayer.StopVideoLoopPlay();
    }

    /// <summary>
    /// 피드백에서 점수 올려주는 함수
    /// </summary>
    /// <param name="Answer">정답율 값</param>
    /// <param name="AOI">시선집중도 값</param>
    protected virtual void ShowScore(float Answer, float AOI)
    {
        ScoreIE = ShowScoreRoutine(Answer, AOI);
        StartCoroutine(ScoreIE);
    }

    protected virtual float CalculateFocusScore(LevelType id)
    {
        float focusPercent = 0.0f;
        int AnswerCount = DataManager.Inst.GetAnswerCount(id);
        focusPercent = (float)Math.Truncate((AnswerCount / FocusTotalCount) * 100);
        Debug.Log("focusPercent : " + focusPercent);
        AnswerCount = AnswerCount >= 100 ? 100 : AnswerCount;
        return focusPercent;
    }

    protected virtual float CalculateAOIScore(LevelType id)
    {
        float time = 0.0f;
        float aoiPercent = 0.0f;
        time = DataManager.Inst.GetAOITime(id, AOIType.AOI1);

        aoiPercent = (float)Math.Truncate((time / AOITotalTime) * 100);
        aoiPercent = aoiPercent >= 100 ? 100 : aoiPercent;
        return aoiPercent;
    }

    /// <summary>
    /// 점수 평가
    /// </summary>
    /// <param name="IsBio"></param>
    /// <param name="focus"></param>
    /// <param name="aoi"></param>
    /// <param name="RTV"></param>
    /// <param name="CV"></param>
    /// <param name="eeg"></param>
    /// <returns></returns>
    protected int ChooseNarration(bool IsBio, float focus, float eeg = 0)
    {
        int temp = 0;
        float A = 0;
        // 훈련에 집중한 정도( Vas 값 )
        if (DataManager.Inst.isPassConnect || DataManager.Inst.isServer)
            A = DataManager.Inst.GetVasValue(0);
        else
            A = DataManager.Inst.SendVasValue;

        Debug.Log("My Vas : " + A);
        // 훈련 정답률
        float B = focus;
        // EEG 값
        float C = eeg;

        if (IsBio)
        {
            // Exellent
            if (A > 80 && B > 95 && C > 80)
            {
                temp = 1;
            }
            // Retry
            else if (A < 60 || B < 90)
            {
                temp = 3;
            }
            else
            {
                temp = 2;
            }
        }
        else
        {
            //Exellent
            if (A > 80 && B > 95)
            {
                temp = 1;
            }
            // Let's Retry!

            else if (A < 60 || B < 90)
            {
                temp = 3;
            }
            // Good!
            else
            {
                temp = 2;
            }
        }

        Debug.Log("Rating : " + temp);

        return temp;
    }
    protected virtual void ShowStartButton()
    {
        UIManager.Inst.ShowAndInteractableCanvasGroup(Intro);
    }
    public void ButtonFuncTest()
    {

        Debug.Log("<color=yellow> ButtonFunctTest </color>");
        UIManager.Inst.StopWaitForNarrationAndFunc();
        UIManager.Inst.SkipNarration();
        UIManager.Inst.HideAndInteractableCanvasGroup(Intro);
        UIManager.Inst.ShowAndInteractableCanvasGroup(Practice);

        SetActive(PracticeEndButton.gameObject, false);

        focusGame.SetActive(true);
        focusGame.SetPracticeMode(true);
        focusGame.InitGame();

    }

    /// <summary>
    /// 연습 버튼
    /// </summary>
    public virtual void PracticeFunc()
    {

        UIManager.Inst.StopWaitForNarrationAndFunc();
        UIManager.Inst.SkipNarration();
        UIManager.Inst.HideAndInteractableCanvasGroup(Intro);
        UIManager.Inst.ShowAndInteractableCanvasGroup(Practice);

        SetActive(PracticeEndButton.gameObject, false);

        focusGame.SetActive(true);
        focusGame.SetPracticeMode(true);
        focusGame.InitGame();
        focusGame.SetJoyStickPosition(new Vector3(0, 100, 0));
        SetPosition(PracticeEndButton.transform, new Vector3(0, -40, 0));

        // 나레이션 나오는 도중에        
        UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_RLCS_L123_T_1"));
        //delegate { PracticNextNarration(); });

        UIManager.Inst.ShowNarrationText("여러가지 모양의 도형이 하나씩 화면에 등장할거에요.\n도형이 나타는 순서는 그 어떤 규칙도 없어요.", 0, 10.0f, 0);

        // 7초 뒤 랜덤 도형이 나와서 움직임.
        //UIManager.Inst.WaitSeconds(7.3f, delegate { focusGame.ShowFigure(); });

        UIManager.Inst.WaitSeconds(11.7f, delegate { UIManager.Inst.ShowNarrationText("단, 이 도형과 다른 도형이 나타나면 조이스틱 버튼을 누르셔야 해요."); });
        // 정답 도형 보여줌
        UIManager.Inst.WaitSeconds(11.7f, delegate { focusGame.StopFigure(); },
                                          delegate
                                          {
                                              focusGame.ShowJoyStick();
                                              focusGame.HideAnswerFigure();
                                              //focusGame.ShowAnswerFigure();
                                          });
        //UIManager.Inst.WaitSeconds(12.8f, delegate { Step3(1); });
        UIManager.Inst.WaitSeconds(15.0f, delegate { focusGame.ShowBorder(); });
        WaitForSecAndFunc(20.0f, delegate { UIManager.Inst.HideNarrationText(); },
            delegate { focusGame.StopBorder(); },
            delegate { SetActive(PracticeEndButton.gameObject, true); },
            //delegate { focusGame.HideJoyStick(); },
            delegate { ShowCountDown(); },
            delegate
            {
                UIManager.Inst.CompleteAndCallback(focusGame.CountDownEnd,
                 delegate
                 {
                     focusGame.StartPracticeFocusGame(delegate { UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_RLCS_L123_T_2"), "", false); });
                 }
         );
            }
            );
    }
    /// <summary>
    /// 연습 훈련 종료시
    /// </summary>
    public virtual void PracticeEnd()
    {
        StopWaitForSecAndFunc();
        isPracticeNextNarration = false;

        UIManager.Inst.StopWaitForNarrationAndFunc();
        UIManager.Inst.StopWaitSeconds();
        UIManager.Inst.SetNarrationPosition(0, 50, 0);
        UIManager.Inst.HideNarrationText();
        UIManager.Inst.StopCompleteAndCallback();

        focusGame.HideAnswerFigure();
        focusGame.HideJoyStick();
        focusGame.SetActive(false);
        focusGame.SetPracticeMode(false);
        focusGame.StopFigure();
        focusGame.StopBorder();
        focusGame.EndPracticeGame();

        UIManager.Inst.SkipNarration();
        UIManager.Inst.HideAndInteractableCanvasGroup(Practice);
        UIManager.Inst.ShowAndInteractableCanvasGroup(SelectLevel);
    }

    public virtual void Step3(int index = 0)
    {
        focusGame.StopFigure();
        focusGame.ShowAnswerFigure();

        // 연습 일 때
        if (index == 1)
        {
            focusGame.ShowJoyStick();
            UIManager.Inst.ShowNarrationText("조이스틱 버튼을 눌러주세요.", 0, 0, 0);
        }
        // Room 연습 일 때
        else if (index == 2)
        {
            focusGame.ShowJoyStick();
            UIManager.Inst.ShowNarrationText("이 도형과 다른 도형이 나올 때 조이스틱 버튼을 눌러주세요.", 0, 255, 0);
        }
        // Room 실전 일 때
        else if (index == 3)
        {
            focusGame.ShowJoyStick();
            UIManager.Inst.ShowNarrationText("이 도형과 다른 도형이 나올 때 조이스틱 버튼을 눌러주세요.", 0, 58, 0);
        }
        else if (index == 4)
        {
            UIManager.Inst.ShowNarrationText("이 도형과 다른 도형이 나올 때 조이스틱 버튼을 눌러주세요.", 0, -300.0f, 0);
        }
        // 그 외 360 영상 및 Room 제외
        else
        {
            UIManager.Inst.ShowNarrationText("처음 도형을 잘 기억하세요\n자, 그럼 시작합니다.", 0, 0, 0);
        }
    }

    public void PracticNextNarration()
    {
        isPracticeNextNarration = true;
    }

    public void ShowCountDown()
    {
        Debug.Log("ShowCountDown");
        focusGame.HideAnswerFigure();
        focusGame.HideJoyStick();
        focusGame.ShowCountDown();
    }

    public void ButtonClickSound()
    {
        if (audioSource != null && ButtonClick != null)
        {
            audioSource.clip = ButtonClick;
            audioSource.Play();
        }
    }
    public void EnterPoint()
    {
        isButtonEnter = true;
    }
    public void ExitPoint()
    {
        isButtonEnter = false;
    }
    public void SetActive(GameObject go, bool check)
    {
        go.SetActive(check);
    }
    public void SetPosition(Transform tr, float x, float y, float z)
    {
        tr.localPosition = new Vector3(x, y, z);
    }
    public void SetPosition(Transform tr, Vector3 vec)
    {
        tr.localPosition = vec;
    }
    public void SetRotation(Transform tr, float x = 0, float y = 0, float z = 0)
    {
        Quaternion qu = new Quaternion();

        qu.eulerAngles = new Vector3(x, y, z);
        tr.localRotation = qu;
    }
    /// <summary>
    ///  SceneMain의 LoginButton전용 함수
    /// </summary>
    public virtual void ActiveLoginButton(bool IsEmpty)
    {

    }
    public void SetRotation2(Transform tr, float x = 0, float y = 0, float z = 0)
    {
        SetRotationIE = SetRotationRoutine(tr, x, y, z);
        StartCoroutine(SetRotationIE);
    }

    protected void SetLevelGameObject(List<GameObject> list, int index, bool val)
    {
        for (int i = 0; i < list.Count; i++)
            list[i].SetActive(false);

        list[index].SetActive(val);
    }
    protected void SetActiveLevelGameObject(List<GameObject> list, bool val)
    {
        for (int i = 0; i < list.Count; i++)
            list[i].SetActive(false);
    }

    public virtual void ShowGraphMenu()
    {
        UIManager.Inst.HideAndInteractableCanvasGroup(FeedBack);
        UIManager.Inst.ShowAndInteractableCanvasGroup(Report);
        //List<float> AnswerCountList = DataManager.Inst.GetAnswerCountList(DataManager.Inst.CurrentLevelType);
        //List<float> AOICountList = DataManager.Inst.GetAOICountList(DataManager.Inst.CurrentLevelType);
        //List<float> EEGRatioList = new List<float>();
        if (MainBioSignal.Inst != null)
        {
            if (MainBioSignal.Inst.GetConnectMng.IsConnected())
                DataManager.Inst.EEGRatioList = DataManager.Inst.GetEEGGraphValue();
        }
        //WindowGraph.Inst.ShowGraph(AnswerCountList, new Color(0.93f, 0.24f, 0.23f), 600);
        //WindowGraph.Inst.ShowAOIGraph(AOICountList, new Color(0.23f, 0.85f, 0.21f), 600);
        WindowGraph.Inst.CreateBarGraph(0, DataManager.Inst.AnswerCountList);
        WindowGraph.Inst.CreateBarGraph(1, DataManager.Inst.AOICountList);

        //if (ConnectedClient != null)
        //{
        //    if (DataManager.Inst.EEGRatioList.Count > 0)
        //    {
        //        WindowGraph.Inst.GetBioLegend.SetActive(true);
        //        WindowGraph.Inst.CreateBarGraph(2, DataManager.Inst.EEGRatioList);
        //    }
        //    else
        //    {
        //        WindowGraph.Inst.GetBioLegend.SetActive(false);
        //    }

        //}
        if (MainBioSignal.Inst != null)
        {
            if (MainBioSignal.Inst.GetConnectMng.IsConnected())
                WindowGraph.Inst.CreateBarGraph(2, DataManager.Inst.EEGRatioList);
        }
        float RTV = DataManager.Inst.GetRTV(DataManager.Inst.CurrentFocusData);
        float CV = DataManager.Inst.GetCV(DataManager.Inst.CurrentFocusData);
        RatingScore(DataManager.Inst.AnswerCountList, DataManager.Inst.AOICountList, DataManager.Inst.EEGRatioList, RTV, CV);

        //DataManager.Inst.CurrentLevel = "";
        //DataManager.Inst.CurrentLevelType = LevelType.None;
        //DataManager.Inst.NextSituation = "SelectTraining";
        //UIManager.Inst.StopNarration();
        //UIManager.Inst.StopWaitForNarrationAndFunc();
        //UIManager.Inst.StopWaitSeconds();
        //UIManager.Inst.HideNarrationText();
        //UIManager.Inst.FullFadeIn(true);
        //UIManager.Inst.DontTouch(FeedBack);
        //SceneLoader.Inst.LoadMain();
    }
    private void RatingScore(List<float> AnswerList, List<float> AOIList, List<float> EEGList, float RTV, float CV)
    {
        int First = 0;
        int Second = 0;
        int Third = 0;
        if (MainBioSignal.Inst == null)
        {
            First = ChooseNarration(false, AnswerList[0]);
            Second = ChooseNarration(false, AnswerList[1]);
            Third = ChooseNarration(false, AnswerList[2]);
        }
        else if (MainBioSignal.Inst.GetConnectMng.IsConnected())
        {
            First = ChooseNarration(true, AnswerList[0], EEGList[0]);
            Second = ChooseNarration(true, AnswerList[1], EEGList[1]);
            Third = ChooseNarration(true, AnswerList[2], EEGList[2]);
        }
        else
        {
            First = ChooseNarration(false, AnswerList[0]);
            Second = ChooseNarration(false, AnswerList[1]);
            Third = ChooseNarration(false, AnswerList[2]);
        }


        for (int i = 0; i < AnswerList.Count; i++)
        {
            Debug.Log("Answer[" + i + "] : " + AnswerList[i]);
        }
        for (int i = 0; i < AOIList.Count; i++)
        {
            Debug.Log("AOIList[" + i + "] : " + AOIList[i]);
        }

        Debug.Log("First : " + First);
        Debug.Log("Second : " + Second);
        Debug.Log("Third : " + Third);

        switch (First)
        {
            // Excellent
            case 1:
                switch (Second)
                {
                    case 1:
                        switch (Third)
                        {
                            case 1:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-EEE")); });
                                break;
                            case 2:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-EEG")); });
                                break;
                            case 3:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-EEL")); });
                                break;
                        }
                        break;
                    case 2:
                        switch (Third)
                        {
                            case 1:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-EGE")); });
                                break;
                            case 2:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-EGG")); });
                                break;
                            case 3:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-EGL")); });
                                break;
                        }
                        break;
                    case 3:
                        switch (Third)
                        {
                            case 1:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-ELE")); });
                                break;
                            case 2:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-ELG")); });
                                break;
                            case 3:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-ELL")); });
                                break;
                        }
                        break;
                }
                break;
            // Good
            case 2:
                switch (Second)
                {
                    case 1:
                        switch (Third)
                        {
                            case 1:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-GEE")); });
                                break;
                            case 2:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-GEG")); });
                                break;
                            case 3:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-GEL")); });
                                break;
                        }
                        break;
                    case 2:
                        switch (Third)
                        {
                            case 1:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-GGE")); });
                                break;
                            case 2:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-GGG")); });
                                break;
                            case 3:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-GGL")); });
                                break;
                        }
                        break;
                    case 3:
                        switch (Third)
                        {
                            case 1:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-GLE")); });
                                break;
                            case 2:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-GLG")); });
                                break;
                            case 3:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-GLL")); });
                                break;
                        }
                        break;
                }
                break;
            // Retry
            case 3:
                switch (Second)
                {
                    case 1:
                        switch (Third)
                        {
                            case 1:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-LEE")); });
                                break;
                            case 2:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-LEG")); });
                                break;
                            case 3:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-LEL")); });
                                break;
                        }
                        break;
                    case 2:
                        switch (Third)
                        {
                            case 1:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-LGE")); });
                                break;
                            case 2:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-LGG")); });
                                break;
                            case 3:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-LGL")); });
                                break;
                        }
                        break;
                    case 3:
                        switch (Third)
                        {
                            case 1:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-LLE")); });
                                break;
                            case 2:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-LLG")); });
                                break;
                            case 3:
                                UIManager.Inst.WaitSeconds(2.0f, delegate { UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_08_S-LLL")); });
                                break;
                        }
                        break;
                }
                break;
        }
    }
    public virtual void GoToFeedBack()
    {
        UIManager.Inst.StopWaitSeconds();
        UIManager.Inst.StopNarration();
        WindowGraph.Inst.ClearGraph();
        UIManager.Inst.HideAndInteractableCanvasGroup(Report);
        UIManager.Inst.ShowAndInteractableCanvasGroup(FeedBack);
    }

    public virtual void GoToBackTitle()
    {
        DataManager.Inst.InitValues();
        InitBase();
        DataManager.Inst.CurrentLevel = "";
        DataManager.Inst.CurrentLevelType = LevelType.None;
        DataManager.Inst.NextSituation = "SelectTraining";
        UIManager.Inst.StopNarration();
        UIManager.Inst.StopWaitForNarrationAndFunc();
        UIManager.Inst.StopWaitSeconds();
        UIManager.Inst.HideNarrationText();
        UIManager.Inst.FullFadeIn(true);
        UIManager.Inst.DontTouch(CanvasList.ToArray());
        SceneLoader.Inst.LoadMain();
    }
    /// <summary>
    /// 일정시간 후에 함수 실행
    /// </summary>
    /// <param name="term">지연시간</param>
    /// <param name="receiver">실행 할 함수 delegate</param>
    protected void WaitForSecAndFunc(float term, params UnityAction[] receiver)
    {
        if (WaitForSecAndFuncIE != null)
            WaitForSecAndFuncIE = null;
        WaitForSecAndFuncIE = WaitForSecAndFuncRoutine(term, receiver);
        StartCoroutine(WaitForSecAndFuncIE);
    }
    protected void StopWaitForSecAndFunc()
    {
        if (WaitForSecAndFuncIE != null)
            StopCoroutine(WaitForSecAndFuncIE);
    }

    protected virtual void InitBase()
    {
        DataManager.Inst.SetTotalTime(0);
        DataManager.Inst.SetTimeStart(false);
        DataManager.Inst.DataManagerClear();
        UIManager.Inst.StopNarration();
        if (videoPlayer != null)
        {
            videoPlayer.Volume = 0.0f;
            videoPlayer.StopVideo();
            videoPlayer.StopVideoLoopPlay();
        }

        RightController = null;
        isGameStart = false;
        isButtonEnter = false;

        FocusTotalCount = 0;
        AOITotalTime = 0;

        isPracticeNextNarration = false;
        AnswerPercentText = null;
        AOIPercentText = null;
        ResultText = null;

        level1 = null;
        level2 = null;
        level3 = null;
        PracticeButton = null;
        PracticeEndButton = null;
        ScoreIE = null;
        DefaultClick = null;
        ButtonClick = null;
        PracticeClick = null;
        audioSource = null;

        isShowJoyStickImg = false;

        Intro = null;
        Practice = null;
        SelectLevel = null;
        EndSession = null;
        FeedBack = null;
        //Back = null;

        StartButton = null;

        Level1 = null;
        Level2 = null;
        Level3 = null;

        CommitButton = null;

        ReportButton = null;
        RetryButton = null;
        NextLevelButton = null;
        EndButton = null;

        //BackButton = null;

        //IntroButtonList = null;
        //PracticeButtonList = null;
        //SelectLevelButtonList = null;
        //EndSessionButtonList = null;
        //FeedBackButtonList = null;
        ButtonList = null;

        isTest = false;
    }

    /// <summary>
    /// 뒤로가기 버튼 눌렀을 때 이전 UI 화면 표시 및 변수 초기화
    /// </summary>
    public void ReservationBackButtonFunc()
    {
        switch (DataManager.Inst.CurrentLevelType)
        {
            case LevelType.None:
#if UNITY_ANDROID && !UNITY_EDITOR
                //OVRManager.PlatformUIConfirmQuit();
#endif
                break;
            default:
                GoToBackTitle();
                break;
        }
    }

    /// <summary>
    /// 지시 하는 오브젝트 객체 셋팅 및 부모 객체 설정
    /// </summary>
    protected void SetRotateCompass()
    {
        Debug.Log("SetRotateCompass");
        rotateCompass = RotateCompass.Inst;
        Transform parent = FindObjectOfType<XROrigin>().transform;
        Transform position = parent.Find("Camera Offset/Main Camera/CenterEyeAnchor").transform;
        if (rotateCompass != null)
        {
            rotateCompass.transform.SetParent(position);
            rotateCompass.transform.localPosition = Vector3.zero;
            rotateCompass.transform.localRotation = Quaternion.identity;
            rotateCompass.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 서버가 클라쪽에 함수 Message Send
    /// </summary>
    protected void SendMessageFromServer(string FuncionName)
    {
        Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

        //if (ConnectedClient != null)
        //{
        //    System.Text.StringBuilder ParentPath = new System.Text.StringBuilder();
        //    Component[] parents = gameObject.transform.GetComponentsInParent(typeof(Transform), true);
        //    for (int i = parents.Length - 1; i >= 0; i--)
        //    {
        //        if (i == 0)
        //            ParentPath.Append(string.Format("{0}", parents[i].name));
        //        else
        //            ParentPath.Append(string.Format("{0}/", parents[i].name));
        //    }

        //    Debug.Log("Total Path : " + ParentPath.ToString());
        //    ConnectedClient.RpcButtonPressed(
        //        GetType().Name,
        //        ParentPath.ToString(),
        //        FuncionName);
        //}
    }

    /// <summary>
    /// 연습부분에서 버튼을 누르면 호출되는 함수
    /// </summary>
    public virtual void PracticeEndFunc()
    {
        Debug.Log("<color=magenta> 연습 게임 종료 </color>");

        SetActive(PracticeEndButton.gameObject, true);
        UIManager.Inst.HidenarrationBG();
        focusGame.isPressed = true;

        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_RLCS_L123_T_2"), "", false);
    }

    private IEnumerator ShowJoyStickImgRoutine()
    {
        JoyStickImg.gameObject.SetActive(true);
        WaitForSeconds tSec = new WaitForSeconds(0.5f);
        int num = 0;

        while (!isShowJoyStickImg)
        {
            JoyStickImg.sprite = JoyStickImgs[num].sprite;
            num++;
            if (num >= JoyStickImgs.Length)
                num = 0;
            yield return tSec;
        }
    }

    /// <summary>
    /// 피드백에서 점수 올라가는 코루틴
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    protected IEnumerator ShowScoreRoutine(float a, float b)
    {
        WaitForEndOfFrame eof = new WaitForEndOfFrame();
        float elapsedTime = 0;
        float Answer = 0;
        float AOI = 0;

        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 1.0f;
            Answer = Mathf.Lerp(0, a, t);
            AOI = Mathf.Lerp(0, b, t);

            AnswerPercentText.text = Math.Truncate(Answer).ToString();
            AOIPercentText.text = Math.Truncate(AOI).ToString();
            yield return eof;
        }
        AnswerPercentText.text = Math.Truncate(a).ToString();
        AOIPercentText.text = Math.Truncate(b).ToString();
    }

    private IEnumerator WaitForSecAndFuncRoutine(float term, params UnityAction[] receiver)
    {
        WaitForSeconds tSec = new WaitForSeconds(term);

        yield return tSec;

        if (receiver != null)
        {
            for (int i = 0; i < receiver.Length; i++)
            {
                receiver[i].Invoke();
            }
        }
    }

    private IEnumerator SetRotationRoutine(Transform tr, float x, float y, float z)
    {

        Quaternion qu = new Quaternion();
        Vector3 a = new Vector3(
            tr.localEulerAngles.x >= 180 ? tr.localEulerAngles.x - 360 : tr.localEulerAngles.x,
            tr.localEulerAngles.y >= 180 ? tr.localEulerAngles.y - 360 : tr.localEulerAngles.y,
            tr.localEulerAngles.z >= 180 ? tr.localEulerAngles.z - 360 : tr.localEulerAngles.z);
        Vector3 b = new Vector3(
            x == 0 ? tr.localEulerAngles.x >= 180 ? tr.localEulerAngles.x - 360 : tr.localEulerAngles.x : x,
            y == 0 ? tr.localEulerAngles.y >= 180 ? tr.localEulerAngles.y - 360 : tr.localEulerAngles.y : y,
            z == 0 ? tr.localEulerAngles.z >= 180 ? tr.localEulerAngles.z - 360 : tr.localEulerAngles.z : z);

        float elapsedTime = 0;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.5f;

            float tx = Mathf.Lerp(a.x, b.x, t);
            float ty = Mathf.Lerp(a.y, b.y, t);
            float tz = Mathf.Lerp(a.z, b.z, t);

            qu.eulerAngles = new Vector3(tx, ty, tz);

            tr.localRotation = qu;
            yield return null;
        }
    }

    public IEnumerator IsMeshVisibleRoutine(params UnityAction[] callbackFunc)
    {
        bool isCheck = false;
        WaitForSeconds tSec = new WaitForSeconds(0.5f);
        while (!isCheck)
        {
            for (int i = 0; i < meshRender.Length; i++)
            {
                if (!meshRender[i].isVisible && meshRender[i] != null)
                {
                    isCheck = true;
                }
                else
                {
                    isCheck = false;
                }
            }
            yield return tSec;
        }

        yield return SendCompleteSceneLoaded();
        yield return CheckSceneLoadedRoutine();

        for (int i = 0; i < callbackFunc.Length; i++)
        {
            if (callbackFunc[i] != null)
            {
                callbackFunc[i].Invoke();
            }
        }
    }

    /// <summary>
    /// AsyncClient 찾기    
    /// </summary>
    /// <returns></returns>
    IEnumerator FindAsyncClient()
    {
        Debug.Log("<color=yellow>[FindAsyncClient] 2024.04.16 주석 처리</color>");
        yield return null;
        //while (true)
        //{
        //    AsyncClient client = FindObjectOfType<AsyncClient>();
        //    if (DataManager.Inst.isPassConnect)
        //    {
        //        Debug.Log("FindAsyncClient::Connect Pass");
        //        yield break;
        //    }
        //    if (client == null)
        //        yield return null;
        //    else
        //    {
        //        Debug.Log("Find AsyncClient");
        //        client.SetMainScene();
        //        break;
        //    }
        //}
    }

    /// <summary>
    /// Server, Client에 따라 CurvedUI 셋팅
    /// </summary>
    /// <returns></returns>
    IEnumerator CurvedSettingRoutine()
    {

        Canvas CanvasFixedUI = null;
        if (GameObject.Find("Canvas_FixedUI") != null)
            CanvasFixedUI = GameObject.Find("Canvas_FixedUI").GetComponent<Canvas>();

        yield return null;
        //AsyncClient client = FindObjectOfType<AsyncClient>();

        //while (true)
        //{
        //    client = FindObjectOfType<AsyncClient>();
        //    if (DataManager.Inst.isPassConnect)
        //    {
        //        Debug.Log("CurvedSettingRoutine::Connect Pass");
        //        if (GameObject.Find("ClientCamera") != null)
        //            GameObject.Find("ClientCamera").SetActive(false);
        //        yield break;
        //    }
        //    if (client == null)
        //        yield return null;
        //    else
        //        break;
        //}
        //ConnectedClient = client;
        //if (client.isClient)
        //{
            //GameObject eventSystem = GameObject.Find("EventSystem");
            //if (eventSystem != null)
            //    Destroy(eventSystem.gameObject);

            //OVRManager ovrMng = FindObjectOfType<OVRManager>();
            //if (ovrMng != null)
            //    ovrMng.enabled = false;
            //OVRCameraRig ovrCamera = FindObjectOfType<OVRCameraRig>();
            //if (ovrCamera != null)
            //    ovrCamera.enabled = false;

            Debug.Log("<color=red>[CurvedSettingRoutine] 주석 처리(2024.04.03)</color>");
#if UNITY_EDITOR
            //CurvedUI.CurvedUISettings settings = CanvasFixedUI.GetComponent<CurvedUI.CurvedUISettings>();
            //settings.ControlMethod = CurvedUIInputModule.CUIControlMethod.MOUSE;
#else
#endif
            //RightController = GameObject.Find("RightHandAnchor");
            //if (RightController != null)
            //{
            //    RightController.SetActive(false);
            //}
            //if (GameObject.Find("CurvedUILaserBeam") != null)
            //{
            //    var LaserBeam = GameObject.Find("CurvedUILaserBeam");
            //    LaserBeam.SetActive(false);
            //}
            //if (GameObject.Find("CurvedUIHandSwitcher") != null)
            //{
            //    var switcher = GameObject.Find("CurvedUIHandSwitcher");
            //    switcher.SetActive(false);
            //}
            //CurvedUIInputModule curvedModule = FindObjectOfType<CurvedUIInputModule>();
            //Debug.Log(curvedModule.gameObject.name);
            //if (curvedModule != null)
            //    Destroy(curvedModule);

        //}
    }

    /// <summary>
    /// Scene Load가 완료됬다는걸 Send
    /// </summary>
    protected IEnumerator SendCompleteSceneLoaded()
    {
        Debug.Log("<color=yellow>[SendCompleteSceneLoaded] 2024.04.16 주석 처리</color>");

        // 연결을 안하고 시작하면 종료
        if (DataManager.Inst.isPassConnect) yield break;

        // AsyncClient 객체를 찾기 위한 시간
        float FindConnectedClientTimeOut = 5.0f;
        // 경과 시간
        float ElapsedTime = 0;
        // AsyncClient 찾기
        //if (ConnectedClient == null)
        //{
        //    AsyncClient client;
        //    while (true)
        //    {
        //        client = FindObjectOfType<AsyncClient>();
        //        ElapsedTime += Time.deltaTime;
        //        if (client == null)
        //            yield return null;
        //        if (ElapsedTime > FindConnectedClientTimeOut) break;
        //        else
        //            break;
        //    }

        //    if (client == null)
        //        yield break;
        //    else
        //        ConnectedClient = client;
        //    ElapsedTime = 0;
        //}

        //if (ConnectedClient.isServer)
        //{
        //    ConnectedClient.RpcSendCompleteSceneLoad();
        //}
        //else
        //{
        //    ConnectedClient.CmdSendCompleteSceneLoad();
        //}
    }

    /// <summary>
    /// 서버와 클라가 Scene Load가 완료되었는지 체크
    /// </summary>
    /// <returns></returns>
    protected IEnumerator CheckSceneLoadedRoutine()
    {
        // 연결을 안하고 시작하면 종료
        if (DataManager.Inst.isPassConnect) yield break;

        // AsyncClient 객체를 찾기 위한 시간
        float FindConnectedClientTimeOut = 5.0f;
        // Scene Load 체크 타임 아웃 시간
        float CheckTimeOut = 30.0f;
        // 경과 시간
        float ElapsedTime = 0;
        // AsyncClient 찾기
        //if (ConnectedClient == null)
        //{
        //    AsyncClient client;
        //    while (true)
        //    {
        //        client = FindObjectOfType<AsyncClient>();
        //        ElapsedTime += Time.deltaTime;
        //        if (client == null)
        //            yield return null;
        //        if (ElapsedTime > FindConnectedClientTimeOut) break;
        //        else
        //            break;
        //    }

        //    if (client == null)
        //        yield break;
        //    else
        //        ConnectedClient = client;
        //    ElapsedTime = 0;
        //}

        //if (ConnectedClient.isServer)
        //{
        //    DataManager.Inst.isServerLoaded = true;
        //    while (true)
        //    {
        //        if (DataManager.Inst.isSClientLoaded)
        //        {
        //            break;
        //        }
        //        if (ElapsedTime > CheckTimeOut) yield break;
        //        ElapsedTime += Time.deltaTime;
        //        yield return null;
        //    }
        //}
        //else if (ConnectedClient.isClient)
        //{
        //    DataManager.Inst.isSClientLoaded = true;
        //    while (true)
        //    {
        //        if (DataManager.Inst.isServerLoaded)
        //        {
        //            break;
        //        }
        //        if (ElapsedTime > CheckTimeOut) yield break;
        //        ElapsedTime += Time.deltaTime;
        //        yield return null;
        //    }
        //}
        DataManager.Inst.isServerLoaded = false;
        DataManager.Inst.isSClientLoaded = false;
    }

    public void SendFocusGameEnd()
    {
        focusGame.EndGame();
    }

    /// <summary>
    /// 포커스 게임의 커브드 MeshCollider 액티브
    /// </summary>
    /// <param name="toggle"></param>
    /// <returns></returns>
    private IEnumerator SetFocusCurvedMeshColliderRoutine(bool toggle)
    {
        Debug.Log("SetFocusCurvedMeshColliderRoutine");
        while (true)
        {
            if (GameObject.Find("Canvas_Focus") == null)
                break;
            GameObject focusCanvas = GameObject.Find("Canvas_Focus").gameObject;
            MeshCollider find = focusCanvas.GetComponent<MeshCollider>();
            if (find != null)
            {
                Debug.Log("Find!!");
                find.enabled = false;
                break;
            }
            yield return null;
        }
    }

    public void PaseTest()
    {
        if (Time.timeScale == 1)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    /// <summary>
    /// 키보드 카메라 회전 속도
    /// </summary>
    public float speed = 0.5f;

    protected virtual void Update()
    {
#if UNITY_EDITOR

#else
        if (!UnityEngine.XR.XRSettings.isDeviceActive) return;
#endif
        //if (OVRInput.GetDown(OVRInput.Button.One)) // Quest2 콘트롤러 A버튼으로 수정 [2024.04.03]
        //{
        //    Debug.Log("========================== Pressed BackButton ==========================");

        //if (SceneManager.GetActiveScene().name == "SceneMain")
        //    Debug.Log("<color=red>SceneMain이 활성화 되어있어 뒤로가기 기능을 실행하지 않습니다.</color>");
        //else
        //{
        //    ReservationBackButtonFunc();
        //    SendMessageFromServer("GoToBackTitle");
        //}
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PaseTest();
            SendMessageFromServer("PaseTest");
        }

        //if (DataManager.Inst.CurrentLevel.Length != 0) // Quest2 콘트롤러 조이스틱으로 수정 [2024.04.03]
        //{
        //    float x = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x;
        //    float y = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;

        //    if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick)) //
        //    {
        //        // 과제  수행 중인가?
        //        if (focusGame.isStart)
        //        {
        //            SendFocusGameEnd();
        //            SendMessageFromServer("SendFocusGameEnd");
        //        }
        //    }

        //    // 뒤로가기 기능과 겹쳐서 주석 처리함
        //    //if (((x > -0.1f && x < 0.1f) && y > 0.8f && OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick)) || Input.GetKeyDown(KeyCode.Keypad6))
        //    //{
        //    //    if (!SceneLoader.Inst.isStart) // 씬 로딩 중이 아닐 때
        //    //    {
        //    //        switch (DataManager.Inst.CurrentLevelType)
        //    //        {
        //    //            case LevelType.CAFE_LEVEL0:
        //    //            case LevelType.CAFE_LEVEL1:
        //    //            case LevelType.CAFE_LEVEL2:
        //    //            case LevelType.CAFE_LEVEL3:
        //    //                GoToBackTitle();
        //    //                SendMessageFromServer("GoToBackTitle");
        //    //                break;
        //    //            case LevelType.STREET_LEVEL0:
        //    //            case LevelType.STREET_LEVEL1:
        //    //            case LevelType.STREET_LEVEL2:
        //    //            case LevelType.STREET_LEVEL3:
        //    //                GoToBackTitle();
        //    //                SendMessageFromServer("GoToBackTitle");
        //    //                break;
        //    //            case LevelType.ROOM_LEVEL0:
        //    //            case LevelType.ROOM_LEVEL1:
        //    //            case LevelType.ROOM_LEVEL2:
        //    //            case LevelType.ROOM_LEVEL3:
        //    //                GoToBackTitle();
        //    //                SendMessageFromServer("GoToBackTitle");
        //    //                break;
        //    //            case LevelType.LIBRARY_LEVEL0:
        //    //            case LevelType.LIBRARY_LEVEL1:
        //    //            case LevelType.LIBRARY_LEVEL2:
        //    //            case LevelType.LIBRARY_LEVEL3:
        //    //                GoToBackTitle();
        //    //                SendMessageFromServer("GoToBackTitle");
        //    //                break;
        //    //        }
        //    //    }
        //    //}
        //    //else if (((y > -0.3f && y < 0.3f) && x > 0.7f && OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick)) || Input.GetKeyDown(KeyCode.Keypad8))
        //    //{
        //    //    // 과제  수행 중인가?
        //    //    if (focusGame.isStart)
        //    //    {
        //    //        SendFocusGameEnd();
        //    //        SendMessageFromServer("SendFocusGameEnd");
        //    //    }
        //    //}
        //}

//#if UNITY_EDITOR
//        // 버튼을 눌렀을 때 체크
//        if (Input.GetMouseButtonDown(0)/*GetKeyDown(KeyCode.Keypad9)*/)
//        {
//            if (!isGameStart && !isButtonEnter)
//            {
//                if (DefaultClick != null && audioSource != null)
//                {
//                    audioSource.clip = DefaultClick;
//                    audioSource.Play();
//                }
//            }

//            if (isPracticeNextNarration)
//            {
//                focusGame.HideJoyStick();
//                isPracticeNextNarration = false;
//                if (PracticeClick != null && audioSource != null)
//                {
//                    audioSource.clip = PracticeClick;
//                    audioSource.Play();
//                }
//                UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_RLCS_L123_T_2"), "", false);
//            }
//        }

//#else
//        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
//        {
//            if(!isGameStart && !isButtonEnter)
//            {
//                if (DefaultClick != null && audioSource != null)
//                {
//                    audioSource.clip = DefaultClick;
//                    audioSource.Play();
//                }
//            }
//            if(isPracticeNextNarration)
//            {
//                focusGame.HideJoyStick();
//                isPracticeNextNarration = false;
//                if(PracticeClick != null && audioSource != null)
//                {
//                    audioSource.clip = PracticeClick;
//                    audioSource.Play();
//                }
//                UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_RLCS_L123_T_2"), "", false);
//            }
//        }
//#endif

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Quaternion qu = new Quaternion();
            qu.eulerAngles = new Vector3(10, 0, 0);
            ovrCamera1.localRotation = qu;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            ovrCamera1.localRotation = new Quaternion(0, 0, 0, 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            Quaternion t = new Quaternion(ovrCamera1.localRotation.x + -speed * Time.deltaTime,
                                           ovrCamera1.localRotation.y,
                                           ovrCamera1.localRotation.z,
                                           ovrCamera1.localRotation.w);
            ovrCamera1.localRotation = t;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Quaternion t = new Quaternion(ovrCamera1.localRotation.x + speed * Time.deltaTime,
                                           ovrCamera1.localRotation.y,
                                           ovrCamera1.localRotation.z,
                                           ovrCamera1.localRotation.w);
            ovrCamera1.localRotation = t;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Quaternion t = new Quaternion(ovrCamera1.localRotation.x,
                                           ovrCamera1.localRotation.y + speed * Time.deltaTime,
                                           ovrCamera1.localRotation.z,
                                           ovrCamera1.localRotation.w);
            ovrCamera1.localRotation = t;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Quaternion t = new Quaternion(ovrCamera1.localRotation.x,
                                           ovrCamera1.localRotation.y + -speed * Time.deltaTime,
                                           ovrCamera1.localRotation.z,
                                           ovrCamera1.localRotation.w);
            ovrCamera1.localRotation = t;
        }
#else
#endif
    }
}
