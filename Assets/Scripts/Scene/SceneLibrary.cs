using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SceneLibrary : SceneBase {

    public GameObject player;

    // 레벨별 공간 리스트
    public List<GameObject> LevelArea;
    // 레벨별 캐릭터 시작 위치 리스트
    public List<Vector3> PlayerPos = new List<Vector3>();

    // 레벨별 캐릭터 리스트
    public List<Character> Characters = new List<Character>();
    // 레벨1 창문 오브젝트 리스트
    public List<GameObject> WindowList = new List<GameObject>();

    public Text frame;

    //private IEnumerator DiscouragementSequenceIE;

    protected override void Awake()
    {
        PlayerPos.Add(new Vector3(-73.03f, 12.059f, 29.599f));
        PlayerPos.Add(new Vector3(-51.22f, 12.05f, 8.97f));
        PlayerPos.Add(new Vector3(-32.78f, -4.63f, -29.91f));
    }
    protected override void EnterFunc()
    {        
        if (DataManager.Inst.CurrentLevelType == LevelType.LIBRARY_LEVEL2 ||
           DataManager.Inst.CurrentLevelType == LevelType.LIBRARY_LEVEL3)
        {
            Level1Step();
        }
        else
        {
            DataManager.Inst.CurrentLevelType = LevelType.LIBRARY_LEVEL0;
            UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("ad_03L_1"),
                "You've chosen to train in the library!\n" +
"<color=#35AC88>Here, you can read books, study, and do assignments. It is like a second home for students.\n" +
"Shall we take advantage of this atmosphere and do N - back training for 2 minutes?</color>\n" +
 "Not sure what N - back training is?\n"+
 "Click the practice button below to hear a detailed explanation of the training.", 
            true,
            delegate { ShowStartButton(); });
            UIManager.Inst.FullFadeOut();
        }
    }
    // Use this for initialization
    protected override void Start () {

        if (SceneLoader.Inst != null)
        {
            if (SceneLoader.Inst.IsLoadingTextActive())
                SceneLoader.Inst.SetLoadingTextActive(false);
        }
        player = GameObject.Find("Player");
        Transform[] tWindows = GameObject.Find("reading_room/Background/reading_room/Window_wall_001").GetComponentsInChildren<Transform>();
        for (int i = 0; i < tWindows.Length; i++)
        {
            if (tWindows[i].name.Contains("Window_00"))
            {
                WindowList.Add(tWindows[i].gameObject);
                //SetRotation(WindowList[i].transform, 0, -90, -90);
            }
        }
        SetPosition(player.transform, PlayerPos[0]);
        SetRotation(player.transform, 0, 180);
        
        UIManager.Inst.LoadNarrationClip("Lib");

        base.Start();
        //CheckPurchase();
        IsMeshVisible(delegate { EnterFunc(); });
        
        Debug.Log(System.DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.ff"));
    }

    public override void Level1Step()
    {
        base.Level1Step();
        for (int i = 0; i < WindowList.Count; i++)
            SetRotation(WindowList[i].transform, 0, -90.0f, -90.0f);

        Character[] temp = GameObject.Find("reading_room/Characters").GetComponentsInChildren<Character>(true);
        Characters.Clear();
        for (int i = 0; i < temp.Length; i++)
        {
            Characters.Add(temp[i]);
            Characters[i].InitFunc();
            if (i >= 1)
                Characters[i].SetPosition(0, 0, 0);
        }

        // Level Index Set
        SetLevelIndex(0);

        SetLevelGameObject(LevelArea, 0, true);

        // 약하게 펜 쓰는 효과음 2분 간격으로 반복 -> 1분간격으로 줄임 2018.11.13 수정일
        EnvironmentSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D0"), 60.0f);
        // 120초 간격으로 타자 소리
        //DiscouragementSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D5"), 120.0f, 1);

        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        // 페이드 인
        UIManager.Inst.FullFadeOut(            
            delegate { SetPosition(player.transform, PlayerPos[0]); },
            delegate { SetRotation(player.transform, 0, 180); },
            delegate { level1.InitFunc(this, LevelType.LIBRARY_LEVEL1, null, 1); },
            delegate { level1.ShowStartButton(); },
            delegate { focusGame.SetActive(true); },
            delegate { focusGame.ShowJoyStick(); },
            delegate { focusGame.SetJoyStickPosition(0, 120, 0); },
            delegate { UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_LIB_05L_L1"), "", false, delegate { focusGame.HideJoyStick(); }); },
            // 나레이션 보여주기
            delegate {
                //UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n테블릿 화면 위에 조이스틱 버튼을 누르세요", 0, -108.0f);
                UIManager.Inst.ShowNarrationText("People are sparsely sitting in the reading room.\n"+
"It's a good environment to concentrate because we all study together!\n"+
"Now, shall we go to the remaining empty seat and start? ", 0, -108.0f);
            }
        );
    }
    public override void Level2Step()
    {
        //App Store Upload Version
        //if (!DataManager.Inst.GetIsPurchase)
        //{
        //    base.Level2Step();
        //    return;
        //}
        //else
        //{
        //    UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        //    UIManager.Inst.FullFadeIn(delegate { SceneLoader.Inst.LoadScene(6); });
        //}
        base.Level2Step();
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        //UIManager.Inst.FullFadeIn(delegate { SceneLoader.Inst.LoadScene(7); });
        UIManager.Inst.FullFadeIn(delegate { SceneLoader.Inst.LoadScene("SceneReference"); });

    }
    public override void Level3Step()
    {
        //App Store Upload Version
        //if (!DataManager.Inst.GetIsPurchase)
        //{
        //    base.Level3Step();
        //    return;
        //}
        base.Level3Step();
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        //UIManager.Inst.FullFadeIn(delegate { SceneLoader.Inst.LoadScene(8); });
        UIManager.Inst.FullFadeIn(delegate { SceneLoader.Inst.LoadScene("SceneLounge"); });
    }

    /// <summary>
    /// 연습 버튼
    /// </summary>
    public override void PracticeFunc()
    {
        UIManager.Inst.SkipNarration();
        UIManager.Inst.HideAndInteractableCanvasGroup(Intro);
        UIManager.Inst.ShowAndInteractableCanvasGroup(Practice);
        UIManager.Inst.HalfFadeOut();

        SetActive(PracticeEndButton.gameObject, false);

        focusGame.SetActive(true);
        focusGame.SetPracticeMode(true);
        focusGame.InitGame();
        focusGame.SetJoyStickPosition(new Vector3(0, 36, 0));
        // 나레이션 나오는 도중에        
        UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("ad_03L_2"));
        //delegate { PracticNextNarration(); });

        UIManager.Inst.ShowNarrationText("You were wondering what N-back training is!\n"+
"Various shapes will appear on the screen one by one.\n"+
"There are no rules as to the order in which shapes appear.", 0, 100, 0);

        // 7초 뒤 랜덤 도형이 나와서 움직임.
        //UIManager.Inst.WaitSeconds(7.3f, delegate { focusGame.ShowFigure(); });

        UIManager.Inst.WaitSeconds(10.5f, delegate { UIManager.Inst.ShowNarrationText("However, look closely at the shape as shown now.\nYou must press the trigger button the second time it is the same as the shape that appeared before."); });
        // 정답 도형 보여줌
        //UIManager.Inst.WaitSeconds(11.7f, delegate { focusGame.StopFigure(); },
        //                                  delegate {
        //                                      focusGame.ShowJoyStick();
        //                                      focusGame.ShowAnswerFigure();
        //                                  });
        UIManager.Inst.WaitSeconds(10.0f, 
            delegate {
                focusGame.StartPracticeFocusGame(delegate {
                    PracticeEndFunc();
                    Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

                    //if (ConnectedClient != null && ConnectedClient.isServer)
                    //{
                    //    SendMessageFromServer("PracticeEndFunc");
                    //}
                });
            });
        //UIManager.Inst.WaitSeconds(12.8f, delegate { Step3(1); });
        //UIManager.Inst.WaitSeconds(15.0f, delegate { focusGame.ShowBorder(); });
        //WaitForSecAndFunc(20.0f, 
        //    delegate
        //    {
        //        UIManager.Inst.HideNarrationText();
        //        focusGame.StopBorder();
        //        SetActive(PracticeEndButton.gameObject, true);
        //        ShowCountDown();
        //    },            
        //    delegate {
        //        UIManager.Inst.CompleteAndCallback(focusGame.CountDownEnd,
        //            delegate {
        //                focusGame.StartPracticeFocusGame(
        //                   delegate {
        //                       UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("ad_03L_3"), "", false);
        //                   });
        //            }
        //        );
        //    }
        //);
    }
    /// <summary>
    /// 연습 훈련 종료시
    /// </summary>
    public override void PracticeEnd()
    {
        StopWaitForSecAndFunc();
        UIManager.Inst.HalfFadeIn();
        isPracticeNextNarration = false;

        UIManager.Inst.StopWaitForNarrationAndFunc();
        UIManager.Inst.StopWaitSeconds();
        UIManager.Inst.StopCompleteAndCallback();
        UIManager.Inst.SetNarrationPosition(0, 50, 0);
        UIManager.Inst.HideNarrationText();

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

    public override void FocusGameStart()
    {   
        base.FocusGameStart();

        float[] elapseTimes = new float[] { 180.0f, 180.0f, 240.0f, 180.0f, 60.0f };
        //float[] elapseTimes = new float[] { 1, 10, 20, 30, 40 };
        UnityAction[] callbacks = new UnityAction[]
                                      {
                                          delegate{ Phase1(); },
                                          delegate{ Phase2(); },
                                          delegate{ Phase3(); },
                                          delegate{ Phase4(); },
                                          delegate{ Phase5(); }
                                      };

        StartDiscouragement(elapseTimes, callbacks);
    }
    public override void FocusGameEnd()
    {
        EndSetting();
        StopDiscouragement();
        EnvironmentSound.Inst.AllLoopStop();
        EnvironmentSound.Inst.AllPlayStop();
        DiscouragementSound.Inst.AllStop();
        base.FocusGameEnd();
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
    }
    protected override void EndSetting()
    {
        for(int i = 0; i < Characters.Count; i++)
        {
            Characters[i].StopAnimation();
        }        
    }

    private void Phase1()
    {
        // 도서관 사서 셋팅 및 걸어와 창문 열기, 창문 소리 재생 180초
        Characters[0].SetPosition(0, 0, 0);
        Characters[0].SetTrigger("librarian_ComeIn");
        WaitForSecAndFunc(15.3f, delegate { SetRotation2(WindowList[0].transform, 0, -65.0f, -90); });
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D1"), 15.3f, 1, 1);
        WaitForSecAndFunc(19.3f, delegate { SetRotation2(WindowList[1].transform, 0, -65.0f, -90.0f); });
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D1"), 19.3f, 1, 1);
        WaitForSecAndFunc(22.3f, delegate { SetRotation2(WindowList[2].transform, 0, -65.0f, -90.0f); });
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D1"), 22.3f, 1, 1);
        WaitForSecAndFunc(24.3f, delegate { SetRotation2(WindowList[3].transform, 0, -65.0f, -90.0f); });
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D1"), 24.3f, 1, 1);

    }

    private void Phase2()
    {
        // 책장 넘기는 약한 소리 360 -> 3분마다 나던걸 1분 30초마다 나도록 2018.11.13 수정        
        DiscouragementSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D2"), 90.0f, 1);
    }
    private void Phase3()
    {
        // 의자끄는 약한 소리 (나무의자 바퀴안달린것) 600
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D3"), 1);
    }
    private void Phase4()
    {
        // 책상에 가방내려놓는 소리 780
        DiscouragementSound.Inst.WaitAndFunc(1, UIManager.Inst.FindNarration("AD_LIB_05L_L1_D4"));
    }
    private void Phase5()
    {
        // 키보드(노트북)타이핑 작은 소리 840
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D5"),1);
        // 볼펜누르는 소리  840
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D6"),1);
    }
}
