using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class FocusGame : MonoBehaviour
{
    [SerializeField]
    public struct HideCount
    {
        public float One;
        public float Two;
        public float Four;

        public HideCount(float x, float y, float i)
        {
            One = x;
            Two = y;
            Four = i;
        }
    }
    [SerializeField]
    public List<HideCount> HideCountList = new List<HideCount>();

    public Image ShowImage;
    public Sprite BlankImage;
    public Image JoyStickImage;
    public Image Border;
    public GameObject FigureList;
    public GameObject Count;
    public List<GameObject> LevelImageList = new List<GameObject>(); 
    public Image[] FigureArray;
    public Image[] JoySticArray;
    public Image[] CountArray;

    public List<int> Sequences;
    public List<int> Sequences2;
    public List<int> PracticeSequenceList;
    public List<float> HideTimeSequenceList;
    /// <summary>
    /// 게임 시작 체크
    /// </summary>
    public bool isStart = false;
    public bool isFirst = true;
    public bool isCountDownEnd = false;
    /// <summary>
    /// 연습모드 체크
    /// </summary>
    public bool isPractice = false;

    /// <summary>
    /// Miss 체크
    /// </summary>
    public bool isMiss = false;
    /// <summary>
    /// 도형 총 개수
    /// </summary>
    private int figureCount = 0;
    public int CurrentCount = 0;
    public int CurrentIndex = 0;
    private int PracticeCount = 0;
    private int PracticeIndex = 0;
    /// <summary>
    /// 현재 도형
    /// </summary>
    public FigureState figureState = FigureState.NONE;

    /// <summary>
    /// 현재 시간
    /// </summary>
    private float CurrentTime = 0.0f;
    /// <summary>
    /// 총 진행 시간
    /// </summary>
    private float TotalTime = 0.0f;
    /// <summary>
    /// 도형 간격 시간
    /// </summary>
    private float IntervalTime = 0.5f;

    /// <summary>
    /// 도형을 보고 누른 시간
    /// </summary>
    private DateTime PushButtonTime;
    /// <summary>
    /// 도형이 나타난 시간
    /// </summary>
    private DateTime ShowFigureTime;
    /// <summary>
    /// 누른 시간과 나타난 시간의 차
    /// </summary>
    private TimeSpan deltaTime;
    /// <summary>
    /// 버튼 누른 시간차의 범위
    /// </summary>
    private float CheckIntervalTime = 400;

    private float ShowTime = 0.25f;
    private float HideTime = 0.5f;

    private const int k_OneSec = 110;
    private const int k_TwoSec = 150;
    private const int k_FourSec = 100;

    /// <summary>
    /// 버튼을 눌렀는지 체크
    /// </summary>
    public bool isPressed = false;
    private bool DuplicatePressed = false;
    private bool isShowJoyStickImg = false;
    private bool isFocusGameStart = false;
    /// <summary>
    /// 게임 시작시간
    /// </summary>
    private string StartTime = "";
    /// <summary>
    /// 게임이 종료되면 실행하는 CallBack함수(SceneManager측의 함수)
    /// </summary>
    private UnityAction CallBackFunc;

    /// <summary>
    /// 도형 보여주는 IEnumerator
    /// </summary>
    private IEnumerator ShowCoroutine;
    private IEnumerator JoyStick;
    private IEnumerator CountDown;
    private IEnumerator borderIE;
    /// <summary>
    /// 정답 도형 번호
    /// </summary>
    public int AnswerFigureNum = 0;
    public int TempAnswer = 0;

    private AudioClip ClickSound;
    private AudioClip CountSound;
    private AudioSource audioSource;

    /// <summary>
    /// 도형 변경 유무
    /// </summary>
    private bool isChanged = false;

    public float GetToTalTime
    {
        get { return TotalTime; }
    }

	// Use this for initialization
	void Start ()
    {
        CreateRandomHideList();
        ClickSound = Resources.Load<AudioClip>("Sound/Button/DpadClick");
        CountSound = Resources.Load<AudioClip>("Sound/Effect/Sound_Beep_2");
        audioSource = GetComponent<AudioSource>();
        // 클릭음 셋팅
        audioSource.clip = ClickSound;
        
        // 시퀀스 로드
        LoadSequence();

        JoySticArray = JoyStickImage.transform.GetComponentsInChildren<Image>();
        CountArray = Count.transform.GetComponentsInChildren<Image>();

        

        if (FigureArray.Length == 0)
            //FigureArray = FigureList.GetComponentsInChildren<Image>();

        FigureList.SetActive(false);

        for (int i = 0; i < CountArray.Length; i++)
            CountArray[i].gameObject.SetActive(false);

        for (int i = 0; i < JoySticArray.Length; i++)
            JoySticArray[i].gameObject.SetActive(false);

        for (int i = 0; i < LevelImageList.Count; i++)
            LevelImageList[i].SetActive(false);

        ShowImage.gameObject.SetActive(false);
        Border.gameObject.SetActive(false);

        gameObject.SetActive(false);
#if UNITY_EDITOR
        //SetActive(true);
        //InitGame();
        //StartFocusGame();
#else
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (isFocusGameStart)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                isFocusGameStart = false;
                EndGame();
            }
        }
        // 실전 모드
        if (isStart && !isPractice)
        {
            CurrentTime += Time.deltaTime;
            
            // 현재 도형의 보여준 시간이 ShowTime보다 크면 도형을 숨긴다
            if (CurrentTime <= HideTime)
            {                
                ShowTargetImg(ShowImage, BlankImage);
            }
            // 현재 도형의 보여준 시간이 ShowTime보다 작으면 도형을 보여준다
            else if (CurrentTime > ShowTime)
            {                
                if(!isFirst)
                {
                    if (!isChanged && !isMiss && !isPressed && CurrentIndex == AnswerFigureNum)
                    {
                        isPressed = true;
                        DataManager.Inst.AddFocusValue(new FocusValue(1, 1, FocusState.RIGHT, DataManager.Inst.GetTotalTime, 0));
                    }
                    else if (!isChanged && !isMiss && !isPressed && CurrentIndex != AnswerFigureNum)
                    {
                        isMiss = true;
                        DataManager.Inst.AddFocusValue(new FocusValue(1, 0, FocusState.OE, DataManager.Inst.GetTotalTime, CheckIntervalTime));
                    }
                }

                // 게임 종료 체크
                CheckEndGame();


                if (!isChanged)
                {
                    ChangeFigure();
                    isPressed = false;
                }

                ShowTargetImg(ShowImage, FigureArray[CurrentIndex].sprite);
                AnswerFigureNum = TempAnswer;

            }
#if UNITY_EDITOR
            // 버튼을 눌렀을 때 체크
            if (Input.GetKeyDown(KeyCode.Keypad0))

#else
            //if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
#endif
            {
                // 버튼 클릭음 재생
                audioSource.clip = ClickSound;
                audioSource.Play();

                // 이미 한번 눌렀으면 return
                if (isPressed)
                    return;
                                
                // 버튼을 눌렀을 때 시간 Set
                PushButtonTime = DateTime.Now;
                // 시간 차이 
                deltaTime = PushButtonTime - ShowFigureTime;
                float rt = (float)deltaTime.TotalMilliseconds;

                Debug.Log(deltaTime.TotalMilliseconds);
                Debug.Log(CheckIntervalTime);
                
                // 입력 시간이 기준 시간보다 작으면 Right
                if (!isFirst && deltaTime.TotalMilliseconds < CheckIntervalTime && CurrentIndex != AnswerFigureNum && !isPressed)
                {
                    //Debug.Log("Right");
                    isPressed = true;
                    DataManager.Inst.AddFocusValue(new FocusValue(1, 1, FocusState.RIGHT, DataManager.Inst.GetTotalTime, rt));
                }

                // 입력 시간이 기준 시간보다 크면 Wrong
                else if(!isFirst && (deltaTime.TotalMilliseconds > CheckIntervalTime || CurrentIndex == AnswerFigureNum))
                {
                    //Debug.Log("Wrong");
                    isPressed = true;
                    DataManager.Inst.AddFocusValue(new FocusValue(0, 1, FocusState.CE, DataManager.Inst.GetTotalTime, rt));
                }
            }

            if (CurrentTime >= ShowTime + HideTime)
            {   
                isChanged = false;
                isFirst = false;
                CurrentTime = 0.0f;                                
                ShowTargetImg(ShowImage, BlankImage);
                HideTime = ReturnRandomHideTime();
                CheckIntervalTime = HideTime * 1000;                
                isMiss = false;
                //isPressed = false;                
            }            
        }
        // 연습모드
        else if(isStart && isPractice)
        {
            CurrentTime += Time.deltaTime;

            // 현재 도형의 보여준 시간이 ShowTime보다 크면 도형을 숨긴다
            if (CurrentTime <= HideTime)
            {
                ShowTargetImg(ShowImage, BlankImage);
            }
            // 현재 도형의 보여준 시간이 ShowTime보다 작으면 도형을 보여준다
            else if (CurrentTime > ShowTime)
            {
                if (!isFirst)
                {                    
                    if (!isChanged && !isMiss && !isPressed && CurrentIndex == AnswerFigureNum)
                    {
                        isMiss = true;                        
                        UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_07-2"));
                        //DataManager.Inst.AddFocusValue(new FocusValue(1, 1, FocusState.RIGHT, DataManager.Inst.GetTotalTime, 0));
                    }
                    else if (!isChanged && !isMiss && !isPressed && CurrentIndex != AnswerFigureNum)
                    {                        
                        isPressed = true;
                        UIManager.Inst.PlayEffectSound(UIManager.Inst.FindEffectSound("Sound_Beep_2"));
                        
                        //DataManager.Inst.AddFocusValue(new FocusValue(0, 0, FocusState.MISS, DataManager.Inst.GetTotalTime, CheckIntervalTime));
                    }
                }

                // 게임 종료 체크
                CheckEndGame();

                if (!isChanged)
                {
                    ChangeFigure();
                    isPressed = false;
                }

                ShowTargetImg(ShowImage, FigureArray[CurrentIndex].sprite);
                AnswerFigureNum = TempAnswer;

            }
#if UNITY_EDITOR
            // 버튼을 눌렀을 때 체크
            if (Input.GetKeyDown(KeyCode.Keypad0))

#else
            //if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
#endif
            {
                // 버튼 클릭음 재생
                audioSource.clip = ClickSound;
                audioSource.Play();

                // 이미 한번 눌렀으면 return
                if (isPressed)
                    return;

                // 버튼을 눌렀을 때 시간 Set
                PushButtonTime = DateTime.Now;
                // 시간 차이 
                deltaTime = PushButtonTime - ShowFigureTime;
                float rt = (float)deltaTime.TotalMilliseconds * 0.001f;

                Debug.Log(deltaTime.TotalMilliseconds);
                Debug.Log(CheckIntervalTime);

                // 입력 시간이 기준 시간보다 작으면 Right
                if (!isFirst && deltaTime.TotalMilliseconds < CheckIntervalTime && CurrentIndex != AnswerFigureNum)
                {
                    Debug.Log("Right");
                    isPressed = true;
                    UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_07-2"));
                    PracticeCount++;
                    //DataManager.Inst.AddFocusValue(new FocusValue(1, 1, FocusState.RIGHT, DataManager.Inst.GetTotalTime, rt));
                }

                // 입력 시간이 기준 시간보다 크면 Wrong
                else if (!isFirst && (deltaTime.TotalMilliseconds > CheckIntervalTime || CurrentIndex == AnswerFigureNum))
                {
                    Debug.Log("Wrong");
                    isPressed = true;
                    UIManager.Inst.PlayEffectSound(UIManager.Inst.FindEffectSound("Sound_Beep_2"));
                    //DataManager.Inst.AddFocusValue(new FocusValue(0, 1, FocusState.WRONG, DataManager.Inst.GetTotalTime, rt));
                }
            }

            if (CurrentTime >= ShowTime + HideTime)
            {
                isChanged = false;
                isFirst = false;
                CurrentTime = 0.0f;
                ShowTargetImg(ShowImage, BlankImage);
                HideTime = ReturnRandomHideTime();
                CheckIntervalTime = HideTime * 1000;
                isMiss = false;
                //isPressed = false;
                //Debug.Log("isPressed : " + isPressed);
            }
        }
    }
    /// <summary>
    /// 초기화 함수
    /// </summary>
    public void InitGame(int LevelIndex = 0)
    {
        // Image 리스트 불러오기
        LoadImage(LevelIndex);

        isStart = false;
        // 랜덤 정답 index 생성
        RandomAnswerNum();
        // 랜덤 시퀀스 생성
        CreateRandomSequence();
        // 하이드 리스트 생성
        HideTimeSequenceList = new List<float>();
        // 하이드 리스트 초기화 및 생성
        InitHideSequenceList();

        figureCount = FigureArray.Length;
        figureState = FigureState.NONE;
        CurrentIndex = 0;
        CurrentCount = 0;
        CurrentTime = 0.0f;
        TotalTime = 0.0f;
        IntervalTime = 0.8f;
        StartTime = "";
        if (isPractice)
        {
            ShowTime = 0.5f;
        }
        else
        {
            ShowTime = 0.25f;
        }
        HideTime = ReturnRandomHideTime(0);
        isFirst = true;
        CheckIntervalTime = HideTime * 1000.0f;
        AllActive(FigureArray, false);
    }
    private void LoadImage(int index)
    {
        Image[] temp = LevelImageList[index].GetComponentsInChildren<Image>(true);

        FigureArray = temp;
    }
    public void SetActive(bool check)
    {
        gameObject.SetActive(check);
    }
    /// <summary>
    /// 포커스 게임 시작하는 함수(SceneBase에서 virtual FocusGameStart 함수에서 호출함)
    /// </summary>
    /// <param name="CallBackAction">게임 종료 후 실행 할 함수포인터</param>
    /// <param name="practice">연습모드 체크</param>
    public void StartFocusGame(UnityAction CallBackAction = null, bool practice = false)
    {   
        if (DataManager.Inst != null /*&& !isPractice*/)
        {
            FocusData data = new FocusData(DataManager.Inst.CurrentLevel, DataManager.Inst.CurrentLevelType);
            DataManager.Inst.AddFocusData(data);
            DataManager.Inst.CurrentFocusData = data;//DataManager.Inst.FindFocusData(DataManager.Inst.CurrentLevel);
        }
        // Focus 게임 종료 후 실행 할 함수
        CallBackFunc = CallBackAction;

        Debug.Log("StartFocusGame");
        ShowImage.sprite = BlankImage;
        ShowImage.gameObject.SetActive(true);
        figureState = FigureState.DIAMOND;
        // 시작시간 셋팅
        StartTime = DataManager.NowToString();
        isStart = true;
        // 처음 이미지를 보여주었기 때문에 시작하자마자 DateTime 셋팅
        ShowFigureTime = DateTime.Now;
        ShowImage.gameObject.SetActive(true);

        Debug.Log("Start Time : " + DataManager.NowToString());
        DataManager.Inst.StartTime = DataManager.NowToString();
        // DataManager에 AOI 셋팅
        AOI tAOI = new AOI(DataManager.Inst.CurrentLevel, DataManager.Inst.CurrentLevelType, DataManager.NowToString());
        DataManager.Inst.AddAOI(tAOI);
        // DataManager에 현재 AOI 데이터 셋팅
        DataManager.Inst.CurrentAOI = tAOI;
    }

    /// <summary>
    /// 게임 종료 후 실행되는 함수
    /// </summary>
    public void EndGame()
    {
        // AOI 종료
        HeadTracer.Inst.EndAOI();
        isCountDownEnd = false;
        UIManager.Inst.HalfFadeIn();
        CurrentCount = 0;
        CurrentIndex = 0;
        TempAnswer = 0;
        Debug.Log("Focus.EndGame");
        isStart = false;        
        ShowImage.gameObject.SetActive(false);
        if(CallBackFunc != null)
            CallBackFunc.Invoke();
    }
    /// <summary>
    /// 포커스 연습 게임 시작하는 함수(SceneBase에서 virtual FocusGameStart 함수에서 호출함)
    /// </summary>
    /// <param name="CallBackAction">게임 종료 후 실행 할 함수포인터</param>
    /// <param name="practice">연습모드 체크</param>
    public void StartPracticeFocusGame(UnityAction callback)
    {
        CallBackFunc = callback;
        isPractice = true;
        Debug.Log("StartPracticeFocusGame");
        ShowImage.sprite = BlankImage;
        ShowImage.gameObject.SetActive(true);
        figureState = FigureState.DIAMOND;        
        // 처음 이미지를 보여주었기 때문에 시작하자마자 DateTime 셋팅
        ShowFigureTime = DateTime.Now;
        ShowImage.gameObject.SetActive(true);
        Debug.Log("Start Time : " + DataManager.NowToString());
        isStart = true;
    }

    /// <summary>
    /// 연습 종료 함수
    /// </summary>
    public void EndPracticeGame()
    {        
        isCountDownEnd = false;
        PracticeSequenceList.Clear();
        HideTimeSequenceList.Clear();
    
        CurrentIndex = 0;
        CurrentCount = 0;
        CurrentTime = 0.0f;
        TotalTime = 0.0f;
        IntervalTime = 0.8f;
        StartTime = "";
        ShowTime = 0;
        HideTime = 0;
        isFirst = true;
        Debug.Log("Focus.EndPracticeGame");
        isStart = false;
        isPractice = false;
        ShowImage.gameObject.SetActive(false);
        if (CallBackFunc != null)
            CallBackFunc.Invoke();
    }

    /// <summary>
    /// 도형 변경 함수
    /// </summary>
    private void ChangeFigure()
    {
        isChanged = true;
        
        if (isPractice)
        {            
            CurrentCount = PracticeIndex;
            CurrentIndex = PracticeSequenceList[CurrentCount];
            PracticeIndex++;
            if(PracticeIndex >= PracticeSequenceList.Count)
            {
                PracticeIndex = 0;
            }
        }
        else
        {
            CurrentCount = UnityEngine.Random.Range(0, Sequences2.Count);
            if (Sequences2.Count > 0)
            {
                CurrentIndex = Sequences2[CurrentCount];
                Sequences2.RemoveAt(CurrentCount);
            }
        }
        // 도형이 나타난 시간 Set
        ShowFigureTime = DateTime.Now;

        // 타입 변경
        ChangeFigureType(CurrentIndex);

    }
    private void CheckEndGame()
    {
        if(isPractice)
        {
            if(PracticeCount >= 10)
            {
                EndPracticeGame();
            }
        }
        else
        {
            if (HideTimeSequenceList.Count < 1)
            {
                EndGame();
            }
        }
    }

    private void ChangeFigureType(int num)
    {
        switch(num)
        {
            case 0:
                {
                    figureState = FigureState.TRIANGLE;
                }
                break;
            case 1:
                {
                    figureState = FigureState.RECTANGULAR;
                }
                break;
            case 2:
                {
                    figureState = FigureState.CIRCLE;
                }
                break;
            case 3:
                {
                    figureState = FigureState.DIAMOND;
                }
                break;
            case 4:
                {
                    figureState = FigureState.PENTAGON;
                }
                break;
        }
    }

    private void SetActive(Image target, bool tbool)
    {
        target.gameObject.SetActive(tbool);
    }

    private void AllActive(Image[] target, bool setbool)
    {
        for(int i = 0; i < target.Length; i++)
        {
            target[i].gameObject.SetActive(setbool);
        }
    }

    public void SetPracticeMode(bool val)
    {
        isPractice = val;
    }
    /// <summary>
    /// 도형을 보여주는 함수인데 주 용도는 랜덤으로 도형 보여주기
    /// </summary>
    public void ShowFigure()
    {        
        ShowCoroutine = ShowFigureRoutine();
        StartCoroutine(ShowCoroutine);
    }
    /// <summary>
    /// ShowFigure Stop 함수
    /// </summary>
    public void StopFigure()
    {
        if(ShowCoroutine != null)
            StopCoroutine(ShowCoroutine);        
    }
    /// <summary>
    /// 정답 외 도형 보여주는 함수
    /// </summary>
    public void ShowAnswerFigure()
    {
        ShowImage.gameObject.SetActive(true);        
        ShowImage.sprite = FigureArray[TempAnswer].sprite;
    }
    /// <summary>
    /// 도형 숨김
    /// </summary>
    public void HideAnswerFigure()
    {
        ShowImage.gameObject.SetActive(false);
    }
    /// <summary>
    /// time동안 기다린 후 EndTime동안 도형을 보여줌
    /// </summary>
    /// <param name="time">Wait Time</param>
    /// <param name="EndTime">도형 보여주는 시간</param>
    public void WaitAndShowAnswer(float time, float EndTime)
    {
        StartCoroutine(WaitAndShowAnswerRoutine(time, EndTime));
    }
    /// <summary>
    /// 조이스틱 이미지 보여주는 함수(Coroutine)
    /// </summary>
    public void ShowJoyStick()
    {
        JoyStick = ShowJoyStickImgRoutine();
        StartCoroutine(JoyStick);
    }
    /// <summary>
    /// ShowJoyStick Stop함수(StopCoroutine)
    /// </summary>
    public void HideJoyStick()
    {
        isShowJoyStickImg = true;
        //StopCoroutine(JoyStick);
        JoyStickImage.gameObject.SetActive(false);        
    }
    /// <summary>
    /// 과제수행에서 이미지 Actice 함수
    /// </summary>
    /// <param name="target"></param>
    /// <param name="val"></param>
    public void ShowTargetImg(Image target, bool val)
    {
        target.gameObject.SetActive(val);
    }
    public void ShowTargetImg(Image target, Sprite sprite)
    {
        target.sprite = sprite;
    }

    /// <summary>
    /// 랜덤 정답 버튼 번호 구하기
    /// </summary>
    private void RandomAnswerNum()
    {
        int Length = FigureArray.Length;        
        TempAnswer = UnityEngine.Random.Range(0, Length);
        Debug.Log("TempAnswer : " + TempAnswer);
    }

    /// <summary>
    /// 시퀀스 텍스트를 읽어와 List<int>에 집어넣기
    /// </summary>
    private void LoadSequence()
    {
        TextAsset asset = Resources.Load<TextAsset>("Questions/Sequence");        
        string str = asset.text;
        char[] array = str.ToCharArray();
        List<char> temp = new List<char>();

        for (int i = 0; i < array.Length; i++)
        {
            if(array[i].Equals('\n') || array[i].Equals('\r'))
            {
                //Debug.Log("index [" + i + "] :" + str[i]);                
            }
            else
            {
                temp.Add(array[i]);
            }
        }                
        Sequences = new List<int>();
        for (int i = 0; i < temp.Count; i++)
        {            
            Sequences.Add(int.Parse(temp[i].ToString()));
        }
            
    }
    /// <summary>
    /// 3,2,1 카운트 함수(Coroutine)
    /// </summary>
    public void ShowCountDown()
    {
        CountDown = ShowCountDownRoutine();
        StartCoroutine(CountDown);
    }
    /// <summary>
    /// 카운트 다운이 끝났는지 확인하는 함수인데 주용도는 함수포인터로 넘겨서 계속 체크함
    /// 함수로 만든 이유는 값타입을 넘기면 체크가 불가능하기 때문에
    /// </summary>
    /// <returns></returns>
    public bool CountDownEnd()
    {
        return isCountDownEnd;
    }

    /// <summary>
    /// ShowCountDown Stop(StopCoroutine)
    /// </summary>
    /// <returns></returns>
    public void StopCountDown()
    {        
        StopCoroutine(CountDown);
    }
    
    /// <summary>
    /// 도형 클릭하는 시간 간격 설정 함수
    /// </summary>
    /// <param name="value"></param>
    public void SetIntervalTime(int value)
    {
        CheckIntervalTime = value;
    }
    /// <summary>
    /// 시퀀스의 Count 반환
    /// </summary>
    /// <returns></returns>
    public float SequenceCount()
    {        
        return 900.0f;
    }

    public void SetJoyStickPosition(float x, float y, float z)
    {
        JoyStickImage.transform.localPosition = new Vector3(x, y, z);
    }
    public void SetJoyStickPosition(Vector3 vec)
    {
        JoyStickImage.transform.localPosition = vec;
    }
    public void ShowBorder()
    {
        borderIE = BorderFlashRoutine();
        StartCoroutine(borderIE);
    }
    public void StopBorder()
    {
        if (borderIE != null)
            StopCoroutine(borderIE);
        SetActive(Border, false);
    }
    public void CreateRandomSequence()
    {
        if (isPractice)
        {
            PracticeSequenceList = new List<int>();
            for(int i = 0; i < 4; i++)
            {
                PracticeSequenceList.Add(i);
            }
        }
        else
        {
            int a = 1;
            int b = 1;
            int c = 1;
            int d = 1;

            int aR = 0;
            int bR = 0;
            int cR = 0;
            int dR = 0;

            if (TempAnswer == 0)
            {
                aR = 1;
                bR = UnityEngine.Random.Range(1, 11 - aR - c - d);
                cR = UnityEngine.Random.Range(1, 11 - aR - bR - d);
                dR = 10 - aR - bR - cR;
                int total = aR + bR + cR + dR;
                Debug.Log(aR + " " + bR + " " + cR + " " + dR + " = " + total);
            }
            else if (TempAnswer == 1)
            {
                aR = UnityEngine.Random.Range(1, 8);
                bR = 1;
                cR = UnityEngine.Random.Range(1, 11 - aR - bR - d);
                dR = 10 - aR - bR - cR;
                int total = aR + bR + cR + dR;
                Debug.Log(aR + " " + bR + " " + cR + " " + dR + " = " + total);
            }
            else if (TempAnswer == 2)
            {
                aR = UnityEngine.Random.Range(1, 8);
                bR = UnityEngine.Random.Range(1, 11 - aR - c - d);
                cR = 1;
                dR = 10 - aR - bR - cR;
                int total = aR + bR + cR + dR;
                Debug.Log(aR + " " + bR + " " + cR + " " + dR + " = " + total);
            }
            else if (TempAnswer == 3)
            {
                dR = 1;
                aR = UnityEngine.Random.Range(1, 11 - dR - b - c);
                bR = UnityEngine.Random.Range(1, 11 - aR - c - d);
                cR = UnityEngine.Random.Range(1, 11 - aR - bR - d);
                int total = aR + bR + cR + dR;
                Debug.Log(aR + " " + bR + " " + cR + " " + dR + " = " + total);
            }


            Sequences2 = new List<int>();

            for (int i = 1; i <= aR * 36; i++)
            {
                Sequences2.Add(0);
            }
            for (int i = 1; i <= bR * 36; i++)
            {
                Sequences2.Add(1);
            }
            for (int i = 1; i <= cR * 36; i++)
            {
                Sequences2.Add(2);
            }
            for (int i = 1; i <= dR * 36; i++)
            {
                Sequences2.Add(3);
            }
        }
    }
    private void CreateRandomHideList()
    {
        int x;
        int y;
        int z;
        int count = 0;
        z = 448 / 3;
        for (int i = 0; i <= (int)z; i++)
        {
            y = (-3) * i + 448;
            x = 2 * i - 88;
            float temp = (x + 2 * y + 4 * i);
            if (x >= 0)
            {
                count++;
                
                HideCountList.Add(new HideCount(x, y, i));
                //Debug.Log(x + " " + y + " " + i + " total = " + temp + " count:" + count);

            }
        }
    }
    private void InitHideSequenceList()
    {
        HideTimeSequenceList.Clear();
        if (isPractice)
        {
            HideTimeSequenceList.Add(2.0f);
        }
        else
        {            
            int index = UnityEngine.Random.Range(0, HideCountList.Count);

            HideCount temp = HideCountList[index];

            Debug.Log("temp.One : " + temp.One);
            Debug.Log("temp.Two : " + temp.Two);
            Debug.Log("temp.Four : " + temp.Four);

            HideTimeSequenceList.Add(1);

            for (int i = 0; i < temp.One; i++)
            {
                HideTimeSequenceList.Add(1.0f);
            }
            for (int i = 0; i < temp.Two; i++)
            {
                HideTimeSequenceList.Add(2.0f);
            }
            for (int i = 0; i < temp.Four; i++)
            {
                HideTimeSequenceList.Add(4.0f);
            }
            float total = 0.0f;
            for (int i = 0; i < HideTimeSequenceList.Count; i++)
            {
                total += HideTimeSequenceList[i];
            }
            Debug.Log("HideTimeSequenceList Count : " + HideTimeSequenceList.Count + " / Time : " + total);
        }
    }

    private float ReturnRandomHideTime(int first = -1)
    {
        float hideTime = 0;
        if (isPractice)
        {
            hideTime = HideTimeSequenceList[0];
            return hideTime;
        }
        else
        {
            if (first != -1)
            {
                hideTime = HideTimeSequenceList[0];
                HideTimeSequenceList.RemoveAt(first);
                return hideTime;
            }
            else
            {
                int index = UnityEngine.Random.Range(0, HideTimeSequenceList.Count);

                if (HideTimeSequenceList.Count > 0)
                {
                    hideTime = HideTimeSequenceList[index];
                }
                if (HideTimeSequenceList.Count > 0)
                    HideTimeSequenceList.RemoveAt(index);
            }
        }
        return hideTime;
    }

    private IEnumerator ShowFigureRoutine()
    {        
        WaitForSeconds tSec = new WaitForSeconds(1.3f);
        
        while (true)
        {
            int cnt = UnityEngine.Random.Range(0, FigureArray.Length);
            ShowImage.sprite = FigureArray[cnt].sprite;
            ShowImage.gameObject.SetActive(true);
            yield return tSec;
            ShowImage.gameObject.SetActive(false);
            yield return tSec;
        }        
    }
    private IEnumerator WaitAndShowAnswerRoutine(float time, float EndTime)
    {
        yield return new WaitForSeconds(time);
        StopFigure();
        ShowAnswerFigure();
        yield return new WaitForSeconds(EndTime);
        HideAnswerFigure();
    }
    private IEnumerator ShowJoyStickImgRoutine()
    {
        //for (int i = 0; i < JoySticArray.Length; i++)
        //    JoySticArray[i].gameObject.SetActive(true);

        isShowJoyStickImg = false;
        JoyStickImage.gameObject.SetActive(true);
        WaitForSeconds tSec = new WaitForSeconds(0.5f);
        int num = 0;

        while (!isShowJoyStickImg)
        {
            JoyStickImage.sprite = JoySticArray[num].sprite;
            num++;
            if (num >= JoySticArray.Length)
                num = 0;
            yield return tSec;
        }
    }
    private IEnumerator ShowCountDownRoutine()
    {
        WaitForSeconds tOneSec = new WaitForSeconds(1.0f);
        int count = 2;
        ShowImage.sprite = null;        
        ShowImage.gameObject.SetActive(true);
        
        while(count >= 0)
        {
            // 카운트 효과음 재생
            audioSource.clip = CountSound;
            audioSource.Play();
            ShowImage.sprite = CountArray[count].sprite;
            count--;
            yield return tOneSec;
        }

        isCountDownEnd = true;
        ShowImage.sprite = null;
        ShowImage.gameObject.SetActive(false);        
    }

    private IEnumerator BorderFlashRoutine()
    {
        WaitForSeconds tSec = new WaitForSeconds(0.5f);
        while(true)
        {
            if(Border.gameObject.activeInHierarchy)
            {
                SetActive(Border, false);
            }
            else
            {
                SetActive(Border, true);
            }
            yield return tSec;
        }
    }
}