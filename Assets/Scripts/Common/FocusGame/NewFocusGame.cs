using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FNI.XR;

public enum FocusGameProgressState
{
    NONE,
    INIT_FOCUS,
    INIT_TIME,
    QUESTION,
    HIDE_TIME,
    CHECK_ANSWER,
    CHECK_END
}

public class NewFocusGame : MonoBehaviour
{

    /// <summary>
    /// 2019.07.08
    /// 포커스 게임 진행 State
    /// </summary>
    private FocusGameProgressState mFocusState = FocusGameProgressState.NONE;

    /// <summary>
    /// 연속되는걸 허용할 건지 아닐지
    /// </summary>
    private bool isAcceptContinuousTarget = true;

    /// <summary>
    /// 생성된 정답 표제어 목록 수
    /// </summary>
    private int generatedRightAnswer = 0;
    /// <summary>
    /// 총 타겟 개수
    /// </summary>
    //private int totalCount = 300;
    private int totalCount = 150;

    /// <summary>
    /// 타겟 만드는 개수
    /// </summary>
    public int TargetCount = 50;
    //public const int kTargetMaxIndex = 16;
    /// <summary>
    /// 각 인덱스의 최대 개수
    /// </summary>
    //public const int TargetMaxCount = 18;

    private int NbackRange = 2;

    private const int PracticeIntervalShowTime = 1000;
    /// <summary>
    /// 도형 보여주는 시간
    /// </summary>
#if UNITY_EDITOR
    private double IntervalShowTime = 500;

#else
    private double IntervalShowTime = 500;
#endif

    private double PracticeIntervalHideTime = 3000;
    /// <summary>
    /// 도형 숨기는 시간
    /// </summary>
#if UNITY_EDITOR
    private double IntervalHideTime = 2500;
#else
    private double IntervalHideTime = 2500;
#endif
    /// <summary>
    /// 첫 실행 시 0.5f 숨김 시간
    /// </summary>
    private const double kIntervalFirstHideTime = 500;

    /// <summary>
    /// 미션에 따른 레벨 인덱스
    /// </summary>
    private int LevelIndex = 0;
    /// <summary>
    /// 도형 인덱스
    /// </summary>
    private int FigureIndex = 0;

    /// <summary>
    /// 각 도형 인덱스의 생성 개수 저장 배열
    /// </summary>
    //public int[] numCharges = new int[kTargetMaxIndex];
    /// <summary>
    /// 도형 인덱스 리스트
    /// </summary>
    public List<int> indexList = new List<int>();
    /// <summary>
    /// 타겟의 총 개수
    /// </summary>
    //private int targetCount = TargetCount;

    /// <summary>
    /// 연습에서 맞춘 횟수:: 종료 조건으로 쓰임
    /// </summary>
    private int PracticeCount = 0;

    /// <summary>
    /// 랜덤 객체 1
    /// </summary>
    private System.Random random = new System.Random();
    /// <summary>
    /// 랜덤 객체 2
    /// </summary>
    private System.Random random2 = new System.Random();

    /// <summary>
    /// 연습 끝내기 설명 이미지
    /// </summary>
    public Image alterImage;
    /// <summary>
    /// 테블릿 화면에 이미지 객체
    /// </summary>
    public Image ShowImage;

    /// <summary>
    /// 조이스틱 이미지
    /// </summary>
    public Image JoyStickImage;
    /// <summary>
    /// 테블릿 테두리 이미지
    /// </summary>
    public Image Border;

    /// <summary>
    /// 조이스틱 이미지 리스트
    /// </summary>
    public List<Sprite> JoystickList = new List<Sprite>();

    /// <summary>
    /// 레벨별 도형 이미지 리스트
    /// </summary>
    public List<List<Sprite>> LevelFigureList = new List<List<Sprite>>();
    /// <summary>
    /// 도형 인덱스를 처음것부터 넣는 리스트::정답 체크를 하기 위한 리스트
    /// </summary>
    private List<int> indexStackList = new List<int>();
    /// <summary>
    /// 카운트 다운 이미지 리스트
    /// </summary>
    public List<Sprite> CountImageList = new List<Sprite>();

    /// <summary>
    /// 빈화면시 보여줄 이미지
    /// </summary>
    public Sprite BlankSprite;

    /// <summary>
    /// 게임 시작 체크
    /// </summary>
    public bool isStart = false;
    /// <summary>
    /// 처음 시작인지 체크
    /// </summary>
    public bool isFirst = true;
    /// <summary>
    /// 카운트가 끝난는지 체크
    /// </summary>
    public bool isCountDownEnd = false;

    /// <summary>
    /// Miss 체크
    /// </summary>
    public bool isMiss = false;
    /// <summary>
    /// 도형의 Show 상태
    /// </summary>
    public bool isFigureShow = false;
    /// <summary>
    /// 버튼을 눌렀는지 체크
    /// </summary>
    public bool isPressed = false;
    /// <summary>
    /// 도형 변경 유무
    /// </summary>
    //private bool isChanged = false;
    private bool isShowJoyStickImg = false;

    /// <summary>
    /// 게임 시작시간
    /// </summary>
    private string StartTime = "";
    /// <summary>
    /// 게임이 종료되면 실행하는 CallBack함수(SceneManager측의 함수)
    /// </summary>
    private UnityAction CallBackFunc;

    /// <summary>
    /// 클릭 사운드
    /// </summary>
    private AudioClip ClickSound;
    /// <summary>
    /// 오답 시 사운드
    /// </summary>
    private AudioClip WrongSound;
    /// <summary>
    /// 카운트 사운드
    /// </summary>
    private AudioClip CountSound;
    /// <summary>
    /// 정답 시 사운드
    /// </summary>
    private AudioClip RightSound;
    private AudioSource audioSource;

    #region ====================== Practice ======================
    /// <summary>
    /// 연습모드 체크
    /// </summary>
    public bool isPractice = false;
    /// <summary>
    /// 연습모드에서 버튼을 한번 눌렀는지 체크
    /// </summary>
    private bool isOneCycle = false;
    private int PracticeFigureIndex = 0;
    /// <summary>
    /// 연습하기에 관한 Root Transform
    /// </summary>
    private Transform PracticeRootTR;
    /// <summary>
    /// 연습하기에서 생성된 이미지를 붙일 부모 Transform
    /// </summary>
    private Transform CreatePracticeImagesTR;
    /// <summary>
    /// 연습하기에서 정답 시 텍스트 오브젝트의 부모 객체
    /// </summary>
    private GameObject PracticeAlertTextImage;
    /// 연습하기에서 정답 이후 텍스트 오브젝트의 부모 객체
    /// </summary>
    private GameObject PracticeAlertTextImage2;
    /// <summary>
    /// 연습하기에서 정답 시 화살표 오브젝트
    /// </summary>
    private UIAnimation PracticeAlertArrowImage;
    /// <summary>
    /// 연습하기에서 생성된 도형 이미지
    /// </summary>
    private PracticeObject PracticeFigure;
    /// <summary>
    /// Room 씬에서 사용하는 도형 이미지
    /// </summary>
    private PracticeObject PracticeFigureRoom;
    private List<PracticeObject> PracticeFigureList = new List<PracticeObject>();
    #endregion


    /// <summary>
    /// 총 진행 시간
    /// </summary>
    private float TotalTime = 0.0f;

    /// <summary>
    /// 반응 시간
    /// </summary>
    private float ReActionTime = 0;

    /// <summary>
    /// 현재 시간
    /// </summary>
    private DateTime CurrentDateTime = new DateTime();
    public double GetIntervalTime
    {
        get
        {
            return (DateTime.Now - CurrentDateTime).TotalMilliseconds;
        }
    }
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
    /// 도형 보여주는 코루틴
    /// </summary>
    private IEnumerator ShowCoroutineIE;

    /// <summary>
    /// 조이스틱 깜박이는 이미지 코루틴
    /// </summary>
    private IEnumerator JoyStickIE;

    /// <summary>
    /// 테두리 보여주는 코루틴
    /// </summary>
    private IEnumerator borderIE;

    /// <summary>
    /// 카운트 다운 코루틴
    /// </summary>
    private IEnumerator CountDown;

    public bool TriggerPressed { get => triggerPressed; set => triggerPressed = value; }
    private bool triggerPressed = false;

    private void Update()
    {
        if (isStart && !DataManager.Inst.isPause)
        {
            switch (mFocusState)
            {
                case FocusGameProgressState.INIT_FOCUS: InitFocusGameUpdate(); break;
                case FocusGameProgressState.INIT_TIME: InitTime(); break;
                case FocusGameProgressState.QUESTION: Question(); break;
                //case FocusGameProgressState.HIDE_TIME: HideTime(); break;
                case FocusGameProgressState.CHECK_ANSWER: AnswerCheck(); break;
                case FocusGameProgressState.CHECK_END: CheckEndGame(); break;
            }
        }
    }

    //    public void Update()
    //    {
    //        if (isStart && !isPractice)
    //        {
    //            CurrentTime += Time.deltaTime;
    //            // 0.5초가 지나면 변수값 변경 및 CurrentTime 초기화
    //            if (CurrentTime > kIntervalFirstHideTime && isFirst)
    //            {

    //                isFirst = false;
    //                CurrentTime = 0;
    //            }
    //            // 첫 실행이면 0.5f초 이전까지 기다림
    //            if (isFirst) return;



    //            // 현재 시간이 ShowTime보다 작으면서 도형이 OFF 상태이면 도형을 보여준다
    //            if ((CurrentTime < kIntervalShowTime) && !isFigureShow && !isChanged)
    //            {
    //                isFigureShow = true;
    //                ChangeFigure();
    //                ShowFigureTime = DateTime.Now;

    //            }
    //            // 현재 시간이 ShowTime보다 크고 HideTime보다 작으면서 도형이 ON 상태이면 도형을 숨긴다
    //            else if ( (CurrentTime > kIntervalShowTime) && (CurrentTime < kIntervalHideTime) && isFigureShow)
    //            {
    //                isFigureShow = false;
    //                ShowTargetImg(ShowImage, BlankSprite);
    //            }
    //            else
    //            {
    //                CheckEndGame();                
    //            }

    //#if UNITY_EDITOR
    //            // 버튼을 눌렀을 때 체크
    //            if (Input.GetKeyDown(KeyCode.Keypad0))
    //#else
    //            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
    //#endif
    //            {
    //                // 버튼 클릭음 재생
    //                audioSource.clip = ClickSound;
    //                audioSource.Play();

    //                // 이미 한번 눌렀으면 return
    //                if (isPressed)
    //                    return;

    //                // 버튼을 눌렀을 때 시간 Set
    //                PushButtonTime = DateTime.Now;
    //                // 시간 차이 
    //                deltaTime = PushButtonTime - ShowFigureTime;
    //                float rt = (float)deltaTime.TotalMilliseconds;

    //                Debug.Log(deltaTime.TotalMilliseconds);


    //                // 인덱스 배열이 NbackRange -1 이상부터 정답 체크
    //                if (FigureIndex > kNbackRange - 1)
    //                {                       
    //                    // 입력 시간이 기준 시간보다 작으면 Right
    //                    if (
    //                        !isPressed &&
    //                        (indexList[FigureIndex] == indexList[FigureIndex- kNbackRange])
    //                        )
    //                    {
    //                        DataManager.Inst.AddFocusValue(new FocusValue(1, 1, FocusState.RIGHT, DataManager.Inst.GetTotalTime, rt));
    //                    }

    //                    // 입력 시간이 기준 시간보다 크면 Wrong
    //                    else if (
    //                        !isPressed &&
    //                        (indexList[FigureIndex] != indexList[FigureIndex - kNbackRange]))
    //                    {
    //                        DataManager.Inst.AddFocusValue(new FocusValue(0, 1, FocusState.CE, DataManager.Inst.GetTotalTime, rt));
    //                    }
    //                }
    //                isPressed = true;
    //            }

    //            if (CurrentTime >= kIntervalShowTime + kIntervalHideTime)
    //            {
    //                if(!isPressed)
    //                {
    //                    // 인덱스 배열이 NbackRange -1 이상부터 정답 체크
    //                    if (FigureIndex > kNbackRange - 1)
    //                    {                        
    //                        // 2개 이전 도형과 같고 버튼을 누르지 않으면 OE
    //                        if (isChanged && !isMiss && !isPressed &&
    //                            (indexList[FigureIndex] == indexList[FigureIndex - kNbackRange]))
    //                        {
    //                            isMiss = true;
    //                            DataManager.Inst.AddFocusValue(new FocusValue(1, 0, FocusState.OE, DataManager.Inst.GetTotalTime, kIntervalShowTime + kIntervalHideTime));
    //                        }
    //                        // 2개 이전 도형과 다르고 버튼을 누르지 않으면 Right
    //                        else if (
    //                            isChanged && !isMiss && !isPressed &&
    //                            (indexList[FigureIndex] != indexList[FigureIndex - kNbackRange]))
    //                        {
    //                            isPressed = true;
    //                            DataManager.Inst.AddFocusValue(new FocusValue(1, 1, FocusState.RIGHT, DataManager.Inst.GetTotalTime, 0));
    //                        }
    //                    }
    //                }

    //                isChanged = false;
    //                isFirst = false;
    //                CurrentTime = 0.0f;
    //                isMiss = false;
    //                isPressed = false;


    //            }
    //        }
    //    }

    /// <summary>
    /// 포커스 게임 시작시 최초 1회 실행
    /// </summary>
    public void InitFocusGameUpdate()
    {
        // 0.5초가 지나면 변수값 변경 및 CurrentTime 초기화
        if ((GetIntervalTime) > kIntervalFirstHideTime && isFirst)
        {
            isFirst = false;
            CurrentDateTime = DateTime.Now;
        }

        // 첫 실행이면 0.5f초 이전까지 기다림
        if (isFirst) return;

        mFocusState = FocusGameProgressState.INIT_TIME;
    }

    private void SetActiveObject(GameObject target, bool toggle)
    {
        target.SetActive(toggle);
    }

    /// <summary>
    /// 연습 도형 생성 및 이동, 페이드
    /// </summary>
    private void CreatePracticeFigure(Sprite sprite)
    {
        int ListCount = PracticeFigureList.Count;
        if (ListCount > 0)
        {
            for (int i = 0; i < ListCount; i++)
            {
                PracticeFigureList[i].UIMove(
                    PracticeFigureList[i].transform,
                    PracticeFigureList[i].transform.localPosition,
                    new Vector3(PracticeFigureList[i].transform.localPosition.x - 120, PracticeFigureList[i].transform.localPosition.y, PracticeFigureList[i].transform.localPosition.z),
                    250
                    );
            }
            if (ListCount > 3)
            {
                PracticeFigureList[PracticeFigureIndex].UIFadeOut(
                    PracticeFigureList[PracticeFigureIndex].GetComponent<Image>(),
                    250);
                SetActiveObject(PracticeFigureList[PracticeFigureIndex].gameObject, false);
                if (indexList.Count - 1 > PracticeFigureIndex)
                    PracticeFigureIndex++;
            }
        }
        //PracticeObject instanceObject = SelectPracticeFigure();
        PracticeObject image = Instantiate(PracticeFigure, Vector3.zero, Quaternion.identity);
        image.transform.SetParent(CreatePracticeImagesTR);
        image.transform.localPosition = Vector3.zero;
        image.transform.localScale = Vector3.one;
        Quaternion qu = new Quaternion();
        qu.eulerAngles = Vector3.zero;
        image.transform.localRotation = qu;
        image.GetFigureImage.sprite = sprite;
        PracticeFigureList.Add(image);

        // 테두리 깜박이는 애니메이션 효과
        if (FigureIndex == 4)
        {
            ShowBorderAndBlink(FigureIndex, false, true);
            ShowBorderAndBlink(FigureIndex - NbackRange, true, true);

        }
        else if (FigureIndex > 5)
        {
            bool isRight = indexList[FigureIndex] == indexList[FigureIndex - NbackRange];
            if (isRight)
            {
                ShowBorderAndBlink(FigureIndex, false, true);
                ShowBorderAndBlink(FigureIndex - NbackRange, true, true);
            }
        }
    }

    /// <summary>
    /// 정답인 도형들의 깜박임 코루틴 시작
    /// </summary>
    /// <param name="index"></param>
    /// <param name="text"></param>
    /// <param name="active"></param>
    private void ShowBorderAndBlink(int index, bool isNBack, bool active)
    {
        PracticeFigureList[index].SetTitleImageActive(active);
        PracticeFigureList[index].SetBorderActive(active);
        PracticeFigureList[index].SetTitleSprite(isNBack);
        PracticeFigureList[index].SetMyFigureImageColor(new Color(1, 1, 1, 1.0f));
        PracticeFigureList[index].SetMyImageColor(new Color(1, 1, 1, 1.0f));

        PracticeFigureList[index].UIBlink(new List<Color>() { Color.white, new Color(0.972f, 0.062f, 0.278f, 1) }, new List<Graphic>() { PracticeFigureList[index].GetTitleImage, PracticeFigureList[index].GetBorder });
        //PracticeFigureList[index].UIBlink(PracticeFigureList[index].GetTitleImage.color);
    }
    private void HideBorderAndStopBlink(int index)
    {
        PracticeFigureList[index].UIStopBlink(PracticeFigureList[index].GetBorder);
        PracticeFigureList[index].SetTitleImageActive(false);

        //PracticeFigureList[index].GetFigureImage.gameObject.SetActive(false);
    }
    /// <summary>
    /// 도형 변경 함수
    /// </summary>
    private void ChangeFigure()
    {
        //Debug.Log("<color=yellow> ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ</color>");

        //Debug.Log("LevelFigureList Count : " + LevelFigureList.Count);
        //Debug.Log("LevelIndex : " + LevelIndex);

        //Debug.Log("indexList Count : " + indexList.Count);
        //Debug.Log("FigureIndex : " + FigureIndex);
        Sprite copyImage = LevelFigureList[LevelIndex][indexList[FigureIndex]];
        if (isPractice)
        {
            CreatePracticeFigure(copyImage);
        }
        // 각 레벨에 맞게 도형 변경
        ShowImage.sprite = LevelFigureList[LevelIndex][indexList[FigureIndex]];
    }

    /// <summary>
    /// 도형 변경, 도형 보여준 시점의 시간, 현재 타임 셋팅
    /// </summary>
    public void InitTime()
    {
        if (isPractice)
        {
            if (FigureIndex > 0)
            {
                // 인덱스 -1의 번째의 도형의 알파값 조절
                PracticeFigureList[FigureIndex - 1].SetMyFigureImageColor(new Color(1, 1, 1, 0.7f));
                PracticeFigureList[FigureIndex - 1].SetMyImageColor(new Color(1, 1, 1, 0.7f));
            }
            if (FigureIndex == 5)
            {
                CallBackFunc();
                PracticeAlertTextImage2.SetActive(true);
                if (LevelIndex == 3)
                    PracticeAlertTextImage2.transform.localPosition = new Vector3(0, 0, 0);
                for (int i = 0; i < PracticeFigureList.Count; i++)
                {
                    PracticeFigureList[i].SetBorderActive(false);
                    PracticeFigureList[i].SetTitleImageActive(false);
                    PracticeFigureList[i].SetMyFigureImageColor(new Color(1, 1, 1, 0.7f));
                    PracticeFigureList[i].SetMyImageColor(new Color(1, 1, 1, 0.7f));
                }
            }
            else if (FigureIndex > 5)
            {
                HideBorderAndStopBlink(FigureIndex - 1);
                HideBorderAndStopBlink(FigureIndex - NbackRange - 1);
                PracticeFigureList[FigureIndex - NbackRange - 1].SetMyFigureImageColor(new Color(1, 1, 1, 0.7f));
                PracticeFigureList[FigureIndex - NbackRange - 1].SetMyImageColor(new Color(1, 1, 1, 0.7f));
                //PracticeFigureList[FigureIndex - kNbackRange-1].UIStopBlink(PracticeFigureList[FigureIndex - kNbackRange - 1].GetBorder);
            }
        }
        ChangeFigure();
        CurrentDateTime = DateTime.Now;
        ShowFigureTime = DateTime.Now;
        double calcVal = (double)(((float)DataManager.Inst.GetTotalElapsedTime - (float)DataManager.Inst.GetElapsedTime) / (totalCount - (FigureIndex + 1)));
        float sum = 0.5f + 2.5f;
        Debug.Log(string.Format("TotalElapsedTime : {0} / GetElapsedTime : {1} / TotalCount : {2} / FigureIndex : {3}"
            , DataManager.Inst.GetTotalElapsedTime, DataManager.Inst.GetElapsedTime, totalCount, FigureIndex));
        Debug.Log("CalcVal : " + calcVal);
        IntervalShowTime = calcVal * (0.5f / sum);
        IntervalHideTime = calcVal * (2.5f / sum);
        Debug.Log("IntervalShowTime : " + IntervalShowTime);
        Debug.Log("IntervalHideTime : " + IntervalHideTime);
        mFocusState = FocusGameProgressState.QUESTION;
    }

    /// <summary>
    /// 버튼을 눌렀는지 체크
    /// </summary>
    private void PressCheck()
    {
        // 버튼을 눌렀을 때 체크
        if (XRManager.Instance.RightTriggerPressed)
        {
            if (triggerPressed != true)
            {
                triggerPressed = true;

                // 버튼 클릭음 재생
                audioSource.clip = ClickSound;
                audioSource.Play();

                // 이미 한번 눌렀으면 return
                if (isPressed)
                    return;

                isPressed = true;
                if (isPractice)
                    isOneCycle = true;
                // 버튼을 눌렀을 때 시간 Set
                PushButtonTime = DateTime.Now;
                // 시간 차이 
                deltaTime = PushButtonTime - ShowFigureTime;
                ReActionTime = (float)deltaTime.TotalMilliseconds;
            }
        }
    }

    /// <summary>
    /// 도형 눌렀는지 체크 및 도형 숨김
    /// </summary>
    public void Question()
    {
        PressCheck();

        if (isPractice)
        {
            if (FigureIndex > NbackRange - 1)
            {
                bool isRight = indexList[FigureIndex] == indexList[FigureIndex - NbackRange];

                if (isRight)
                {
                    //PracticeAlertTextImage.SetActive(true);
                    if (FigureIndex > 5)
                    {
                        PracticeAlertArrowImage.gameObject.SetActive(true);
                        if (LevelIndex == 0)
                        {
                            PracticeAlertArrowImage.UITransToTrans(
                            PracticeAlertArrowImage.transform,
                            new Vector3(0, -30, 0),
                            new Vector3(0, -50, 0), 250);
                        }
                        else if (LevelIndex == 3)
                        {
                            PracticeAlertArrowImage.UITransToTrans(
                            PracticeAlertArrowImage.transform,
                            new Vector3(0, -160, 0),
                            new Vector3(0, -175, 0), 250);
                        }
                        else
                        {
                            PracticeAlertArrowImage.UITransToTrans(
                            PracticeAlertArrowImage.transform,
                            new Vector3(0, -90, 0),
                            new Vector3(0, -110, 0), 250);
                        }
                    }
                    if (isPressed && isOneCycle)
                    {
                        UIManager.Inst.PlayNarration(RightSound);
                        isOneCycle = false;
                    }
                }
                else
                {
                    if (FigureIndex > 5)
                    {
                        if (LevelIndex == 0)
                        {
                            if (PracticeAlertArrowImage.gameObject.activeSelf)
                                PracticeAlertArrowImage.UIStopTransToTrans(PracticeAlertArrowImage.transform, new Vector3(0, -30, 0));
                        }
                        else
                        {
                            if (PracticeAlertArrowImage.gameObject.activeSelf)
                                PracticeAlertArrowImage.UIStopTransToTrans(PracticeAlertArrowImage.transform, new Vector3(0, -90, 0));
                        }

                        PracticeAlertArrowImage.gameObject.SetActive(false);
                        PracticeAlertArrowImage.setPlay = false;
                    }
                    if (isPressed && isOneCycle)
                    {
                        UIManager.Inst.PlayEffectSound(WrongSound);
                        isOneCycle = false;
                    }
                }
            }

            if ((GetIntervalTime <= PracticeIntervalShowTime))
            {

            }
            else if (FigureIndex == 4 && !isPressed)
            {
                return;
            }
            else if (GetIntervalTime <= (PracticeIntervalHideTime + PracticeIntervalShowTime))
            {
                if (ShowImage.sprite != BlankSprite)
                    ShowTargetImg(ShowImage, BlankSprite);
            }
            else
            {
                mFocusState = FocusGameProgressState.CHECK_ANSWER;
            }
        }
        else
        {
            if ((GetIntervalTime <= IntervalShowTime))
            {

            }
            else if (GetIntervalTime <= (IntervalHideTime + IntervalShowTime))
            {
                if (ShowImage.sprite != BlankSprite)
                    ShowTargetImg(ShowImage, BlankSprite);
            }
            else
            {
                mFocusState = FocusGameProgressState.CHECK_ANSWER;
            }
        }
    }

    /// <summary>
    /// 정답 체크
    /// </summary>
    public void AnswerCheck()
    {
        if (isPractice)
        {
            if (FigureIndex < 4)
            {

            }
            else
            {
                if (FigureIndex > NbackRange - 1)
                {
                    bool isRight = indexList[FigureIndex] == indexList[FigureIndex - NbackRange];

                    if (!isPressed)
                    {
                        // 2개 이전 도형과 같고 버튼을 누르지 않으면 OE
                        if (isRight)
                        {
                            UIManager.Inst.PlayEffectSound(WrongSound);
                        }
                        // 2개 이전 도형과 다르고 버튼을 누르지 않으면 Right
                        else
                        {
                            //UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("AD_ALL_07-2"));
                        }
                    }
                }
            }
        }
        else
        {
            if (FigureIndex > NbackRange - 1)
            {
                bool isRight = indexList[FigureIndex] == indexList[FigureIndex - NbackRange];

                if (isPressed)
                {
                    if (isRight)
                    {
                        DataManager.Inst.AddFocusValue(new FocusValue(1, 1, FocusState.RIGHT, DataManager.Inst.GetTotalTime, ReActionTime));
                    }
                    // 입력 시간이 기준 시간보다 크면 Wrong
                    else
                    {
                        DataManager.Inst.AddFocusValue(new FocusValue(0, 1, FocusState.CE, DataManager.Inst.GetTotalTime, ReActionTime));
                    }

                }
                else
                {
                    // 2개 이전 도형과 같고 버튼을 누르지 않으면 OE
                    if (isRight)
                    {
                        DataManager.Inst.AddFocusValue(new FocusValue(1, 0, FocusState.OE, DataManager.Inst.GetTotalTime, (float)(IntervalShowTime + IntervalHideTime)));
                    }
                    // 2개 이전 도형과 다르고 버튼을 누르지 않으면 Right
                    else
                    {
                        DataManager.Inst.AddFocusValue(new FocusValue(1, 1, FocusState.RIGHT, DataManager.Inst.GetTotalTime, 0));
                    }
                }
            }
        }


        isPressed = false;

        mFocusState = FocusGameProgressState.CHECK_END;
    }

    /// <summary>
    /// 종료 체크
    /// </summary>
    private void CheckEndGame()
    {
        if (isPractice)
        {
            if (FigureIndex == indexList.Count - 1)
            {
                Debug.Log("<color=magenta> 연습 게임 종료? </color>");
                //EndPracticeGame();
                FigureIndex = 0;
            }
            mFocusState = FocusGameProgressState.INIT_TIME;

            if (FigureIndex < indexList.Count)
                FigureIndex++;
            else
                FigureIndex = indexList.Count - 1;
        }
        else
        {
            Debug.Log($"<color=yellow>{FigureIndex}/ {indexList.Count - 1}</color>");
            if (FigureIndex == indexList.Count - 1)
            {
                EndGame();
                FigureIndex = 0;
            }
            else
                mFocusState = FocusGameProgressState.INIT_TIME;

            if (FigureIndex < indexList.Count)
                FigureIndex++;
            else
                FigureIndex = indexList.Count - 1;
        }

    }

    public void Start()
    {
        // 세팅 파일 읽기
        ReadInfoFile();
        // 오브젝트 할당
        FindObjects();
        // 리소스 로드
        ResourcesLoad();

        // 시퀀스 인덱스 생성
        //CreateRandomIndexis(0, kTargetMaxIndex);
        //StartCoroutine(CreateRandomIndexisRoutine(0, 16));
        audioSource = GetComponent<AudioSource>();

        // 클릭음 셋팅
        audioSource.clip = ClickSound;

        gameObject.SetActive(false);
        ShowImage.gameObject.SetActive(false);
        Border.gameObject.SetActive(false);
    }

    /// <summary>
    /// 오브젝트 셋팅
    /// </summary>
    private void FindObjects()
    {
        if (alterImage == null)
            alterImage = transform.Find("Practice/AlertTextImage2").GetComponent<Image>();
        if (ShowImage == null)
            ShowImage = transform.Find("Tablet/Image").GetComponent<Image>();
        if (JoyStickImage == null)
            JoyStickImage = transform.Find("JoyStickImg").GetComponent<Image>();
        if (Border == null)
            Border = transform.Find("Tablet/border").GetComponent<Image>();

        // 연습 Object Allocate
        PracticeRootTR = transform.Find("Practice").transform;
        CreatePracticeImagesTR = PracticeRootTR.Find("CreateImages").transform;
        PracticeAlertArrowImage = PracticeRootTR.Find("AlertArrowImage").GetComponent<UIAnimation>();
        PracticeAlertTextImage2 = PracticeRootTR.Find("AlertTextImage2").gameObject;
        //PracticeAlertText = PracticeRootTR.Find("AlertTextImage/AlertText").gameObject;
        PracticeFigure = Resources.Load<PracticeObject>("Prefabs/2019/NewFocusGame/PracticeFigure");
        PracticeFigureRoom = Resources.Load<PracticeObject>("Prefabs/2019/NewFocusGame/PracticeFigureRoom");
        PracticeRootTR.gameObject.SetActive(false);
        //PracticeAlertText.SetActive(false);
        PracticeAlertArrowImage.gameObject.SetActive(false);
        PracticeAlertTextImage2.SetActive(false);
    }

    /// <summary>
    /// NBack 설정값 읽어오기
    /// </summary>
    private void ReadInfoFile()
    {
        try
        {
            string path = string.Format("{0}/{1}",

#if UNITY_EDITOR
                                    Directory.GetParent(Application.dataPath),
#else
                                    Application.persistentDataPath,
#endif

                                    "Setting"
                                    );
            Debug.Log("Folder Path : " + path);

            // txt 파일 path
            string FullPath = string.Format("{0}/Setting.txt", path);

            // 디렉토리 체크하여 생성
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("TargetCount=100");
                sb.AppendLine("AcceptContinuousTarget=false");

                File.WriteAllText(FullPath, sb.ToString());
                SetDefaultValue();
            }
            else
            {
                if (!File.Exists(FullPath))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("TargetCount=100");
                    sb.AppendLine("AcceptContinuousTarget=true");
                    File.WriteAllText(FullPath, sb.ToString());
                    SetDefaultValue();
                }
                else
                {
                    string[] ReadText = File.ReadAllLines(FullPath);
                    for (int i = 0; i < ReadText.Length; i++)
                    {
                        string[] splitTxt = ReadText[i].Trim().Split('=');
                        if (splitTxt.Length > 1)
                        {
                            switch (splitTxt[0])
                            {
                                case "TotalCount":
                                    totalCount = int.Parse(splitTxt[1]);
                                    break;
                                case "TargetCount":
                                    TargetCount = int.Parse(splitTxt[1]);
                                    break;
                                case "AcceptContinuousTarget":
                                    if (splitTxt[1].ToLower().Equals("true"))
                                    {
                                        isAcceptContinuousTarget = true;
                                        NBackGenerator.isAcceptContinuousTarget = true;
                                    }
                                    else
                                    {
                                        isAcceptContinuousTarget = false;
                                        NBackGenerator.isAcceptContinuousTarget = false;

                                    }
                                    break;
                                    //case "NBackRange":
                                    //    NbackRange = int.Parse(splitTxt[1]);
                                    //break;
                            }

                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            SetDefaultValue();
        }

    }

    /// <summary>
    /// 인덱스 생성 매개변수 오류 시 기본값 셋팅
    /// </summary>
    private void SetDefaultValue()
    {
        totalCount = 300;
        TargetCount = 100;
        NbackRange = 2;
        isAcceptContinuousTarget = true;
        NBackGenerator.isAcceptContinuousTarget = true;
    }

    /// <summary>
    /// 리소스 로드
    /// </summary>
    private void ResourcesLoad()
    {
        // 사운드 리소스
        ClickSound = Resources.Load<AudioClip>("Sound/Button/DpadClick");
        CountSound = Resources.Load<AudioClip>("Sound/Effect/Sound_Beep_2");
        WrongSound = Resources.Load<AudioClip>("Sound/Effect/Sound_wrong");
        RightSound = Resources.Load<AudioClip>("Sound/Common/AD_ALL_07-2");
        // 에임 이미지
        BlankSprite = Resources.Load<Sprite>("UI/FocusGame/aim");

        // 레벨별 도형 이미지 로드
        Sprite[] level1 = Resources.LoadAll<Sprite>("UI/FocusGame/CPT1");
        Sprite[] level2 = Resources.LoadAll<Sprite>("UI/FocusGame/CPT2");
        Sprite[] level3 = Resources.LoadAll<Sprite>("UI/FocusGame/CPT3");
        Sprite[] level4 = Resources.LoadAll<Sprite>("UI/FocusGame/CPT4");

        LevelFigureList = new List<List<Sprite>>();

        for (int i = 0; i < 4; i++)
            LevelFigureList.Add(new List<Sprite>());

        for (int i = 0; i < level1.Length; i++)
            LevelFigureList[0].Add(level1[i]);

        for (int i = 0; i < level2.Length; i++)
            LevelFigureList[1].Add(level2[i]);

        for (int i = 0; i < level3.Length; i++)
            LevelFigureList[2].Add(level3[i]);

        for (int i = 0; i < level4.Length; i++)
            LevelFigureList[3].Add(level4[i]);

        // 카운트 다운 이미지 로드
        Sprite[] CountImages = Resources.LoadAll<Sprite>("UI/Count");
        for (int i = 0; i < CountImages.Length; i++)
        {
            CountImageList.Add(CountImages[i]);
        }

        Sprite[] joystickImages = Resources.LoadAll<Sprite>("UI/JoyStick");
        for (int i = 0; i < joystickImages.Length; i++)
            JoystickList.Add(joystickImages[i]);


    }

    /// <summary>
    /// 미션 레벨에 맞게 레벨 인덱스 설정
    /// </summary>
    private void SetLevel()
    {
        if (DataManager.Inst == null) return;
        switch (DataManager.Inst.CurrentLevelType)
        {
            case LevelType.ROOM_LEVEL0:
            case LevelType.ROOM_LEVEL1:
            case LevelType.ROOM_LEVEL2:
            case LevelType.ROOM_LEVEL3:
                LevelIndex = 0;
                break;
            case LevelType.LIBRARY_LEVEL0:
            case LevelType.LIBRARY_LEVEL1:
            case LevelType.LIBRARY_LEVEL2:
            case LevelType.LIBRARY_LEVEL3:
                LevelIndex = 1;
                break;
            case LevelType.STREET_LEVEL0:
            case LevelType.STREET_LEVEL1:
            case LevelType.STREET_LEVEL2:
            case LevelType.STREET_LEVEL3:
                LevelIndex = 2;
                break;
            case LevelType.CAFE_LEVEL0:
            case LevelType.CAFE_LEVEL1:
            case LevelType.CAFE_LEVEL2:
            case LevelType.CAFE_LEVEL3:
                LevelIndex = 3;

                break;

        }

    }

    /// <summary>
    /// 인덱스 생성 함수
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void CreateRandomIndexis()
    {
        indexList.Clear();

        List<int> list = NBackGenerator.Generate(totalCount, TargetCount, NbackRange, out generatedRightAnswer);

        while (generatedRightAnswer != TargetCount) // 만약을 위해 원하는 정답 수가 나올때 까지 반복
        {
            list = NBackGenerator.Generate(totalCount, TargetCount, NbackRange, out generatedRightAnswer);
        }
        //int forTestCnt = 4;
        //if (generatedRightAnswer != TargetCount)
        //{
        //    Debug.Log("For 루프 시작");
        //    for (int i = 0; i < forTestCnt; i++)
        //    {
        //        Debug.Log(i);
        //        list.Clear();
        //        list = NBackGenerator.Generate(totalCount, TargetCount, NbackRange, out generatedRightAnswer);
        //        if (generatedRightAnswer == TargetCount)
        //        {
        //            Debug.Log("<color=cyan>숫자 같음 : </color>" + TargetCount + " /" + generatedRightAnswer);

        //            break;
        //        }
        //    }

        //}
        //while (generatedRightAnswer != TargetCount) // 만약을 위해 원하는 정답 수가 나올때 까지 반복
        //{
        //    list = NBackGenerator.Generate(totalCount, TargetCount, NbackRange, out generatedRightAnswer);
        //}

        Debug.Log("<color=yellow>[While]생성되기를 원하는 정답 표제어 목록 수 : </color>" + TargetCount);
        Debug.Log("<color=yellow>[While]생성된 정답 표제어 목록 수 : </color>" + generatedRightAnswer);
        indexList = list;

        //InitValue(false);
    }

    /// <summary>
    /// 인덱스 생성 시 조건이 맞지 않을 경우 초기화 후 다시 재생성 함수
    /// </summary>
    /// <param name="isCoroutine"></param>
    //public void InitValue(bool isCoroutine)
    //{
    //    if (targetCount > 0)
    //    {
    //        numCharges = new int[kTargetMaxIndex];
    //        indexList.Clear();
    //        targetCount = TargetCount;
    //        if(!isCoroutine)
    //        {
    //            CreateRandomIndexis(0, kTargetMaxIndex);
    //        }
    //        else
    //        {
    //            StartCoroutine(CreateRandomIndexisRoutine(0, kTargetMaxIndex));
    //        }

    //    }
    //}

    /// <summary>
    /// 초기화 함수
    /// </summary>
    public void InitGame(int LevelIndex = 0)
    {
        // 레벨 인덱스 설정
        SetLevel();

        mFocusState = FocusGameProgressState.NONE;
        if (!isPractice)
        {
            ShowImage.gameObject.SetActive(false);
        }
        else
        {
            ShowImage.gameObject.SetActive(true);
            ShowImage.sprite = LevelFigureList[this.LevelIndex][0];
        }
        Border.gameObject.SetActive(false);

        isFirst = true;
        isStart = false;
        //numCharges = new int[kTargetMaxIndex];

        //targetCount = TargetCount;

        CreateRandomIndexis();
        PracticeAlertArrowImage.gameObject.SetActive(false);
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

        Debug.Log("<color=magenta>StartFocusGame</color>");
        ShowImage.sprite = BlankSprite;
        ShowImage.gameObject.SetActive(true);
        // 시작시간 셋팅
        StartTime = DataManager.NowToString();
        isStart = true;
        // 처음 이미지를 보여주었기 때문에 시작하자마자 DateTime 셋팅
        ShowFigureTime = DateTime.Now;

        CurrentDateTime = DateTime.Now;
        ShowImage.gameObject.SetActive(true);

        Debug.Log("Start Time : " + DataManager.NowToString());
        DataManager.Inst.StartTime = DataManager.NowToString();

        DataManager.Inst.SetStartDateTime();
        // DataManager에 AOI 셋팅
        AOI tAOI = new AOI(DataManager.Inst.CurrentLevel, DataManager.Inst.CurrentLevelType, DataManager.NowToString());
        DataManager.Inst.AddAOI(tAOI);
        // DataManager에 현재 AOI 데이터 셋팅
        DataManager.Inst.CurrentAOI = tAOI;

        mFocusState = FocusGameProgressState.INIT_FOCUS;
    }

    /// <summary>
    /// 게임 종료 후 실행되는 함수
    /// </summary>
    public void EndGame()
    {
        //DataManager.Inst.SetEndDateTime();
        //DataManager.Inst.SetElapsedDateTime();
        Debug.Log("FocusGame ElapsedTIme : " + DataManager.Inst.GetElapsedTime);
        mFocusState = FocusGameProgressState.NONE;
        // AOI 종료
        HeadTracer.Inst.EndAOI();
        isCountDownEnd = false;
        UIManager.Inst.HalfFadeIn();
        Debug.Log("<color=magenta>Focus.EndGame</color>");
        isStart = false;
        //if(isPractice)
        //    alterImage.gameObject.SetActive(true);
        ShowImage.gameObject.SetActive(false);
        if (CallBackFunc != null)
            CallBackFunc.Invoke();
    }

    /// <summary>
    /// 포커스 연습 게임 시작하는 함수(SceneBase에서 virtual FocusGameStart 함수에서 호출함)
    /// </summary>
    /// <param name="CallBackAction">게임 종료 후 실행 할 함수포인터</param>
    /// <param name="practice">연습모드 체크</param>
    public void StartPracticeFocusGame(UnityAction callback)
    {
        Debug.Log("<color=magenta> 연습 게임 시작</color>");
        if (callback != null)
            CallBackFunc = callback;
        isPractice = true;
        Debug.Log("StartPracticeFocusGame");
        //ShowImage.sprite = BlankSprite;
        ShowImage.sprite = LevelFigureList[LevelIndex][0];
        ShowImage.gameObject.SetActive(true);
        // 처음 이미지를 보여주었기 때문에 시작하자마자 DateTime 셋팅
        //ShowFigureTime = DateTime.Now;
        ShowImage.gameObject.SetActive(false);
        Debug.Log("Start Time : " + DataManager.NowToString());
        isStart = true;

        for (int i = 0; i < PracticeFigureList.Count; i++)
        {
            DestroyImmediate(PracticeFigureList[i].gameObject);
        }
        PracticeFigureList.Clear();

        // 강제로 연습 시퀀스 넣기
        AddPracticeIndexise();
        // 연습 최상이 오브젝트 SetActice 
        PracticeRootTR.gameObject.SetActive(true);
        CurrentDateTime = DateTime.Now;
        mFocusState = FocusGameProgressState.INIT_FOCUS;

    }

    /// <summary>
    /// 강제로 연습 시퀀스 넣기
    /// </summary>
    private void AddPracticeIndexise()
    {
        indexList.Insert(0, 0);
        indexList.Insert(1, 1);
        indexList.Insert(2, 2);
        indexList.Insert(3, 3);
        indexList.Insert(4, 2);
    }

    /// <summary>
    /// 연습 종료 함수
    /// </summary>
    public void EndPracticeGame()
    {
        for (int i = 0; i < PracticeFigureList.Count; i++)
        {
            DestroyImmediate(PracticeFigureList[i].gameObject);
        }
        indexList.Clear();
        PracticeFigureList.Clear();

        PracticeAlertTextImage2.SetActive(false);

        isCountDownEnd = false;
        isFirst = true;
        Debug.Log("Focus.EndPracticeGame");
        isStart = false;
        isPractice = false;
        ShowImage.gameObject.SetActive(false);
        //if (CallBackFunc != null)
        //CallBackFunc.Invoke();
    }

    /// <summary>
    /// 화살표 애니메이션 오브젝트 액티브 함수
    /// </summary>
    /// <param name="toggle"></param>
    public void SetArrowActive(bool toggle)
    {
        PracticeAlertArrowImage.gameObject.SetActive(toggle);
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

    public void SetJoyStickPosition(float x, float y, float z)
    {
        JoyStickImage.transform.localPosition = new Vector3(x, y, z);
    }
    public void SetJoyStickPosition(Vector3 vec)
    {
        JoyStickImage.transform.localPosition = vec;
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
    /// 도형 숨김
    /// </summary>
    public void HideAnswerFigure()
    {
        ShowImage.gameObject.SetActive(false);
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
    /// 정답 외 도형 보여주는 함수
    /// </summary>
    public void ShowAnswerFigure()
    {
        ShowImage.gameObject.SetActive(true);
        //ShowImage.sprite = FigureArray[TempAnswer].sprite;
    }

    /// <summary>
    /// 연습모드 세팅
    /// </summary>
    /// <param name="val"></param>
    public void SetPracticeMode(bool val)
    {
        isPractice = val;
    }

    /// <summary>
    /// 조이스틱 이미지 보여주는 함수(Coroutine)
    /// </summary>
    public void ShowJoyStick()
    {
        //Debug.Log("<color=magenta> JoyStick Image </color>");
        JoyStickIE = ShowJoyStickImgRoutine();
        StartCoroutine(JoyStickIE);
    }

    /// <summary>
    /// 테두리 보여주기
    /// </summary>
    public void ShowBorder()
    {
        borderIE = BorderFlashRoutine();
        StartCoroutine(borderIE);
    }
    /// <summary>
    /// 테두리 보여주기 코루틴 Stop
    /// </summary>
    public void StopBorder()
    {
        if (borderIE != null)
            StopCoroutine(borderIE);
        SetActive(Border, false);
    }

    /// <summary>
    /// 포커스 게임 오브젝트 액티브 함수
    /// </summary>
    /// <param name="check"></param>
    public void SetActive(bool check)
    {
        gameObject.SetActive(check);
    }

    /// <summary>
    /// 도형을 보여주는 함수인데 주 용도는 랜덤으로 도형 보여주기
    /// </summary>
    public void ShowFigure()
    {
        ShowCoroutineIE = ShowFigureRoutine();
        StartCoroutine(ShowCoroutineIE);
    }
    /// <summary>
    /// ShowFigure Stop 함수
    /// </summary>
    public void StopFigure()
    {
        if (ShowCoroutineIE != null)
            StopCoroutine(ShowCoroutineIE);
    }
    private void SetActive(Image target, bool tbool)
    {
        target.gameObject.SetActive(tbool);
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
            JoyStickImage.sprite = JoystickList[num];
            num++;
            if (num >= JoystickList.Count)
                num = 0;
            yield return tSec;
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
    private IEnumerator ShowFigureRoutine()
    {
        WaitForSeconds tSec = new WaitForSeconds(1.3f);

        while (true)
        {
            int cnt = UnityEngine.Random.Range(0, LevelFigureList[LevelIndex].Count);
            ShowImage.sprite = LevelFigureList[LevelIndex][cnt];
            ShowImage.gameObject.SetActive(true);
            yield return tSec;
            ShowImage.gameObject.SetActive(false);
            yield return tSec;
        }
    }

    private IEnumerator ShowCountDownRoutine()
    {
        WaitForSeconds tOneSec = new WaitForSeconds(1.0f);
        int count = 2;
        ShowImage.sprite = null;
        ShowImage.gameObject.SetActive(true);

        while (count >= 0)
        {
            // 카운트 효과음 재생
            audioSource.clip = CountSound;
            audioSource.Play();
            ShowImage.sprite = CountImageList[count];// CountArray[count].sprite;
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
        while (true)
        {
            if (Border.gameObject.activeInHierarchy)
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

static class NBackGenerator
{
    /// <summary>
    /// 표제어 목록
    /// </summary>
    static int[] Entry = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

    /// <summary>
    /// 연속성을 허용 체크
    /// </summary>
    public static bool isAcceptContinuousTarget = true;

    /// <summary>
    /// NBack 목록 생성
    /// </summary>
    /// <param name="totalCount">전체 표제어 목록 수</param>
    /// <param name="desiredRightAnswerCount">생성되기를 원하는 정답 표제어 목록 수</param>
    /// <param name="nBack">n-back</param>
    /// <param name="generatedRightAnswer">생성된 정답 표제어 목록 수</param>
    /// <returns></returns>
    public static List<int> Generate(int totalCount, int desiredRightAnswerCount, int nBack, out int generatedRightAnswer)
    {
        Debug.Log("<color=cyan>[Generate/NewFocusGame] </color> TargetCount : " + desiredRightAnswerCount + "/ TotalCount : " + totalCount);
        List<int> result = new List<int>();
        System.Random random = new System.Random();
        int curRightAnswerCount = 0; //루프 중에 생성된 정답 갯수
        int lastRightIndex = -1; // 마지막 정답 인덱스

        for (int n = 0; n < totalCount; n++)
        {
            if (n >= nBack && curRightAnswerCount >= desiredRightAnswerCount) // 이미 정답 갯수를 채운 경우
            {
                Debug.Log("[if]이미 정답 갯수를 채운 경우");
                int nSelected = random.Next(0, Entry.Length);
                while (Entry[nSelected] == result[n - nBack])
                {
                    nSelected = random.Next(0, Entry.Length);
                }
                result.Add(Entry[nSelected]);
            }
            else
            {
                //Debug.Log("[else]정답 갯수를 채우지 않은 경우");
                float probility = (float)(desiredRightAnswerCount - curRightAnswerCount) / (totalCount - n); // 정답이 되어야 할 확률 : (100 - 0)/(40-n)
                Console.WriteLine("probility : {0}", probility);
                int frequency = (int)(1 / probility + 0.5f); // 정답 빈도
                Console.WriteLine("frequency : {0}", frequency);
                if (frequency != -1 && n >= nBack) // n이 2 이상
                {
                    //if (lastRightIndex != n - 1 && n % frequency == 0)
                    if (n >= lastRightIndex + frequency)
                    {
                        result.Add(result[n - nBack]);
                    }
                    else
                    {
                        int nSelected = random.Next(0, Entry.Length);
                        while (Entry[nSelected] == result[n - 1]) // 연이어서 동일한 표제어 나오지 않도록
                        {
                            nSelected = random.Next(0, Entry.Length);
                        }
                        if (!isAcceptContinuousTarget)
                        {
                            while (lastRightIndex == n - 1 && Entry[nSelected] == result[n - nBack]) // 연이어서 정답 나오지 않도록
                            {
                                nSelected = random.Next(0, Entry.Length);
                            }
                        }
                        result.Add(Entry[nSelected]);
                    }

                }
                else // n이 0 또는 1일 때
                {
                    int nSelected = random.Next(0, Entry.Length); // 0 ~ 16
                    if (n > 0)
                    {
                        while (Entry[nSelected] == result[n - 1]) // 연이어서 동일한 표제어 나오지 않도록
                        {
                            nSelected = random.Next(0, Entry.Length);
                        }

                    }
                    result.Add(Entry[nSelected]);
                }


                // 루프중에 생성된 정답 갯수 계산
                if (n >= nBack && result[n] == result[n - nBack])
                {
                    curRightAnswerCount++;

                    //Debug.Log("else " + n + "/ " + curRightAnswerCount);
                    lastRightIndex = n;
                }
            }

        }

        generatedRightAnswer = curRightAnswerCount;

        if (desiredRightAnswerCount != generatedRightAnswer)
            Debug.Log("<color=red>숫자 다름 : </color>" + desiredRightAnswerCount + " /" + generatedRightAnswer);
        else
            Debug.Log("<color=cyan>숫자 같음 : </color>" + desiredRightAnswerCount + " /" + generatedRightAnswer);
        return result;
    }

}