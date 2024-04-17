using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SceneMain : SceneBase
{
    //private bool isShowJoyStickImg = true;
    //public Text InputText;
    //public Text InputText2;
    [SerializeField]
    public GameObject UdpListener;

    public Text NameButton;
    public Text YearButton;
    public Text MonthButton;
    public Text DayButton;
    public List<ButtonFunc> GenderButtonList = new List<ButtonFunc>();

    private GameObject MoveSlider;

    private CanvasGroup StartTitle;
    private CanvasGroup LoginForm;
    private CanvasGroup SelectTraining;
    /// <summary>
    /// 연결 대기하는 CanvasGroup
    /// </summary>
    private CanvasGroup Standby;
    private Image JoyStick;

    private Image[] JoyStickArray;

    //private ButtonFunc StartButton;
    private ButtonFunc LoginButton;

    private ButtonFunc RoomButton;
    private ButtonFunc LibraryButton;
    private ButtonFunc CafeButton;
    private ButtonFunc StreetButton;

    private IEnumerator JoyStickRoutine;

    private IEnumerator MoveSliderIE;
    public AnimationCurve TimeCurve;

    protected override void Start()
    {
        if (SceneLoader.Inst != null)
            if (SceneLoader.Inst.IsLoadingTextActive())
                SceneLoader.Inst.SetLoadingTextActive(false);

        //InputText = GameObject.Find("InputText").GetComponent<Text>();
        //InputText2 = GameObject.Find("InputText2").GetComponent<Text>();

        // Unity에서 부모 Object 찾기
        Transform FixedUIParent = GameObject.Find("Canvas_FixedUI").transform;
        LoginForm = FixedUIParent.Find("Intro_Input").GetComponent<CanvasGroup>();
        StartTitle = FixedUIParent.Find("Intro_Title").GetComponent<CanvasGroup>();
        SelectTraining = FixedUIParent.Find("SelectTraining").GetComponent<CanvasGroup>();
        Standby = FixedUIParent.Find("Standby").GetComponent<CanvasGroup>();
        MoveSlider = FixedUIParent.Find("MoveSlider").gameObject;
        //UdpListener = FindObjectOfType
        MoveSlider.SetActive(false);
        // 조이스틱 깜박이기 셋팅
        //SetJoyStcik();
        // 버튼 셋팅
        SettingButtons(FixedUIParent);

        UIManager.Inst.HideAndInteractableCanvasGroup(LoginForm, SelectTraining);

        if (DataManager.Inst.NextSituation.Equals("SelectTraining"))
        {
            UIManager.Inst.ShowAndInteractableCanvasGroup(SelectTraining);
        }
        else
        {
            UIManager.Inst.ShowAndInteractableCanvasGroup(StartTitle);
        }
        base.Start();
    }
    private void SetJoyStcik()
    {
        JoyStick = StartTitle.transform.Find("Controller").GetComponent<Image>();
        JoyStickArray = JoyStick.transform.GetComponentsInChildren<Image>();
        for (int i = 0; i < JoyStickArray.Length; i++)
            JoyStickArray[i].gameObject.SetActive(false);
        isShowJoyStickImg = true;
        ShowJoyStick();
    }

    public void Login()
    {
        StartCoroutine(LoginRoutine());
    }

    IEnumerator LoginRoutine()
    {
        Debug.Log("SceneMain Login Start");
        UIManager.Inst.FadeOut(LoginForm, 1,
            delegate
            {
                SetAlpha(LoginForm, 0);
                SetActive(LoginForm.gameObject, false);
                //SetAlpha(SelectTraining, 1);
                //SetActive(SelectTraining.gameObject, true);
            });
        UIManager.Inst.HideAndInteractableCanvasGroup(StartTitle);
        UIManager.Inst.HideAndInteractableCanvasGroup(LoginForm);


        // 2019.05.13 추가
        //SaveInfomation();

        // 유저 정보 텍스트 파일 읽기
        DataManager.Inst.Player.ReadInfoFile();

        //StandbySceneLoad();
        UIManager.Inst.ShowAndInteractableCanvasGroup(Standby);
        UdpListener.SetActive(true);
        while (!DataManager.Inst.isSync)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
                break;
            if (DataManager.Inst.isPassConnect)
                break;

            yield return null;
        }

        //UnLoadScene("StandbyScene");
        UIManager.Inst.HideAndInteractableCanvasGroup(Standby);
        //GoToBackTitle();

        UIManager.Inst.ShowAndInteractableCanvasGroup(SelectTraining);
    }

    /// <summary>
    /// 테블릿 연결 패스
    /// </summary>
    public void PassConnect()
    {
        if (UserMng.Inst != null)
            UserMng.Inst.PassConnect();
    }

    /* AppSort Upload version으로 인해 삭제 */
    private void SaveInfomation()
    {
        if (isTest)
            return;
        string Name = NameButton.text;
        string Year = YearButton.text;
        string Month = MonthButton.text;
        string Day = DayButton.text;
        int Gender = 0;
        for (int i = 0; i < GenderButtonList.Count; i++)
        {
            if (GenderButtonList[i].IsSelected)
                Gender = i;
        }
        DataManager.Inst.Player.Name = Name;
        DataManager.Inst.Player.Age = DateTime.Now.Year - int.Parse(Year) + 1;
        DataManager.Inst.Player.Year = int.Parse(Year);
        DataManager.Inst.Player.Month = int.Parse(Month);
        DataManager.Inst.Player.Day = int.Parse(Day);
        DataManager.Inst.Player.Gender = Gender;
    }

    private void StandbySceneLoad()
    {
        SceneLoader.Inst.AddScene("StandbyScene");
    }
    private void UnLoadScene(string SceneName)
    {
        SceneLoader.Inst.UnLoadScene(SceneName);
    }


    private void SettingButtons(Transform FixedUIParent)
    {
        StartButton = FixedUIParent.Find("Intro_Title/StartButton").GetComponent<ButtonFunc>();

        LoginButton = FixedUIParent.Find("Intro_Input/LoginButton").GetComponent<ButtonFunc>();

        //NameButton = FixedUIParent.Find("Intro_Input/Intro/Info/Name/NameButton").GetComponent<ButtonFunc>();
        //YearButton = FixedUIParent.Find("Intro_Input/Intro/Info/Age/YearButton").GetComponent<ButtonFunc>();
        //MonthButton = FixedUIParent.Find("Intro_Input/Intro/Info/Age/MonthButton").GetComponent<ButtonFunc>();
        //DayButton = FixedUIParent.Find("Intro_Input/Intro/Info/Age/DayButton").GetComponent<ButtonFunc>();

        RoomButton = FixedUIParent.Find("SelectTraining/InRoomButton").GetComponent<ButtonFunc>();
        LibraryButton = FixedUIParent.Find("SelectTraining/InLibraryButton").GetComponent<ButtonFunc>();
        CafeButton = FixedUIParent.Find("SelectTraining/InCafeButton").GetComponent<ButtonFunc>();
        StreetButton = FixedUIParent.Find("SelectTraining/InStreetButton").GetComponent<ButtonFunc>();

        UIManager.Inst.LoadButtonData(string.Format("{0}", "Intro_Title"), string.Format("{0}", gameObject.name + "/Intro_Title/"));
        UIManager.Inst.LoadButtonData(string.Format("{0}", "Intro_Input"), string.Format("{0}", gameObject.name + "/Intro_Input/"));
        UIManager.Inst.LoadButtonData(string.Format("{0}", "SelectTraining"), string.Format("{0}", gameObject.name + "/SelectTraining/"));
        UIManager.Inst.SettingButtonData("Intro_Title", StartButton);
        UIManager.Inst.SettingButtonData("Intro_Input", LoginButton/*, NameButton, YearButton, MonthButton, DayButton*/);
        UIManager.Inst.SettingButtonData("SelectTraining", RoomButton, LibraryButton, CafeButton, StreetButton);

        //RoomButton.AddPressedListener(delegate { ChangeScene(RoomButton.SceneNumber); });
        //LibraryButton.AddPressedListener(delegate { ChangeScene(LibraryButton.SceneNumber); });
        //StreetButton.AddPressedListener(delegate { ChangeScene(StreetButton.SceneNumber); });
        //CafeButton.AddPressedListener(delegate { ChangeScene(CafeButton.SceneNumber); });

        //RoomButton.AddPressedListener(delegate { ChangeScene("TestRoom"); });
        //LibraryButton.AddPressedListener(delegate { ChangeScene("SceneLibrary"); });
        //StreetButton.AddPressedListener(delegate { ChangeScene("SceneStreet360Degree"); });
        //CafeButton.AddPressedListener(delegate { ChangeSlcene("SceneStreet360Degree"); });

        RoomButton.AddChangeScenePressedListener(ChangeScene);
        LibraryButton.AddChangeScenePressedListener(ChangeScene);
        StreetButton.AddChangeScenePressedListener(ChangeScene);
        CafeButton.AddChangeScenePressedListener(ChangeScene);

        //RoomButton.isInteractable = false;
        //LibraryButton.isInteractable = false;

        //StartButton.AddPressedListener(delegate { StartFunc(); /*Login();*/ });
        StartButton.AddPressedListener(StartFunc);
        /* AppSort Upload version으로 인해 삭제 */
        //LoginButton.AddPressedListener(delegate { Login(); }, delegate { SaveInfomation(); });
        LoginButton.AddPressedListener(Login);


        LoginButton.AddClickSoundActionListener(delegate { ButtonClickSound(); }); ;
        RoomButton.AddClickSoundActionListener(delegate { ButtonClickSound(); }); ;
        LibraryButton.AddClickSoundActionListener(delegate { ButtonClickSound(); }); ;
        CafeButton.AddClickSoundActionListener(delegate { ButtonClickSound(); }); ;
        StreetButton.AddClickSoundActionListener(delegate { ButtonClickSound(); }); ;
        StartButton.AddClickSoundActionListener(delegate { ButtonClickSound(); });

        StartButton.AddEnterActionListener(delegate { EnterPoint(); });
        LoginButton.AddEnterActionListener(delegate { EnterPoint(); });

        RoomButton.AddEnterActionListener(delegate { EnterPoint(); });
        LibraryButton.AddEnterActionListener(delegate { EnterPoint(); });
        CafeButton.AddEnterActionListener(delegate { EnterPoint(); });
        StreetButton.AddEnterActionListener(delegate { EnterPoint(); });

        StartButton.AddExitActionListener(delegate { ExitPoint(); });
        LoginButton.AddExitActionListener(delegate { ExitPoint(); });
        RoomButton.AddExitActionListener(delegate { ExitPoint(); });
        LibraryButton.AddExitActionListener(delegate { ExitPoint(); });
        CafeButton.AddExitActionListener(delegate { ExitPoint(); });
        StreetButton.AddExitActionListener(delegate { ExitPoint(); });

    }

    public void StartFunc()
    {
        //HideJoyStick();
        //UIManager.Inst.FadeOut(StartTitle.GetComponent<CanvasGroup>(), 1, delegate { SetAlpha(LoginForm, 1); }, delegate { SetActive(LoginForm.gameObject, true); });
        UIManager.Inst.HideAndInteractableCanvasGroup(StartTitle);
        //UIManager.Inst.ShowAndInteractableCanvasGroup(LoginForm);
        Login();
    }

    //public void ChangeScene(int SceneNumber)
    //{
    //    //Debug.Log("ChangeScene");
    //    UIManager.Inst.DontTouch(SelectTraining);
    //    UIManager.Inst.FullFadeIn(delegate { SceneLoader.Inst.LoadScene(SceneNumber); });        
    //}

    public void ChangeScene(string SceneNumber)
    {
        //Debug.Log("ChangeScene");
        UIManager.Inst.DontTouch(SelectTraining);
        try
        {
            Debug.Log(SceneNumber + "!!");
            UIManager.Inst.FullFadeIn(delegate { SceneLoader.Inst.LoadScene(SceneNumber); });

        }
        catch (Exception e)
        {
            if (SceneLoader.Inst != null)
                Debug.Log("SceneLoader Not null");
            else
                Debug.Log("SceneLoader null...");

        }
    }

    public void SetActive(GameObject target, bool val)
    {
        target.SetActive(val);
    }

    public void SetAlpha(CanvasGroup target, float val)
    {
        target.alpha = val;
    }
    public override void ShowJoyStick()
    {
        JoyStickRoutine = ShowJoyStickIE();
        StartCoroutine(JoyStickRoutine);
    }
    public override void HideJoyStick()
    {
        isShowJoyStickImg = false;
        //StopCoroutine(JoyStickRoutine);
    }

    /// <summary>
    /// SceneBase에서 개인정보 value값 확인을 위해 전용으로 만든 함수 AppSort Upload version으로 인해 삭제
    /// </summary>
    public override void ActiveLoginButton(bool IsEmpty)
    {
        if (isTest)
            return;

        if (LoginButton == null) return;

        if (IsEmpty)
        {
            LoginButton.isInteractable = false;
            LoginButton.ChangeAlpha(LoginButton.mSprite, 0.7f);
            LoginButton.ChangeAlpha(LoginButton.mText, 0.7f);
        }
        else
        {
            LoginButton.isInteractable = true;
            LoginButton.ChangeAlpha(LoginButton.mSprite, 1.0f);
            LoginButton.ChangeAlpha(LoginButton.mText, 1.0f);
        }

    }

    private IEnumerator ShowJoyStickIE()
    {
        JoyStick.gameObject.SetActive(true);
        WaitForSeconds tSec = new WaitForSeconds(0.5f);
        int num = 0;

        while (isShowJoyStickImg)
        {
            JoyStick.sprite = JoyStickArray[num].sprite;
            num++;
            if (num >= JoyStickArray.Length)
                num = 0;
            yield return tSec;
        }
    }

    public void MoveSliderFunc(Transform target, float duration = 0.5f)
    {
        MoveSlider.SetActive(true);
        MoveSliderIE = MoveSliderRoutine(target, duration);
        StartCoroutine(MoveSliderIE);
    }

    IEnumerator MoveSliderRoutine(Transform target, float duration = 0.5f)
    {
        float elapsedTime = 0;
        Vector3 a = MoveSlider.transform.localPosition;
        Vector3 b = target.localPosition;
        WaitForEndOfFrame eof = new WaitForEndOfFrame();
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = TimeCurve.Evaluate(elapsedTime / duration);
            MoveSlider.transform.localPosition = Vector3.Lerp(a, b, t);
            yield return eof;
        }
        target.localPosition = b;
    }

    protected override void Update()
    {
        base.Update();
    }

}
