using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SceneRoom : SceneBase
{

    public GameObject Player;
    public GameObject ParentFixedUI;
    public GameObject Room;

    public List<GameObject> LevelGameObjects = new List<GameObject>();
    public List<GameObject> LevelGameObjects2 = new List<GameObject>();
    public Animator Door;

    //public int DiscouragementLevel = 0;
    //public int LevelIndex = 0;

    private UIManager uiManager;

    private Character MomCharacter;

    private Sprite NarrationText1;
    private Sprite NarrationText2;

    protected override void Awake()
    {
        base.Awake();
        Door = GameObject.Find("VR_Flagship_Room/door").GetComponent<Animator>();

        MomCharacter = GameObject.Find("mom").GetComponent<Character>();

        MomCharacter.InitFunc();
        MomCharacter.LoadLipSyncData("Room/");
        MomCharacter.SetPosition(0, 0, -40);


    }
    protected override void EnterFunc()
    {
        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("ad_03R_1"),
            "You've chosen to train in your room!\n"+
"<color=#35AC88>The door to your room is closed, so it should be relatively quiet.\n"+
"but it could be noisy outside and someone could walk in.\n"+
"Ignore the noise and try to focus and do 2 minutes of N-back training.</color>\n" +
"Not sure what the N - back training is?\n"+
  "Click the exercise button below to hear a detailed explanation of the drill.",
            true,
            delegate { ShowStartButton(); });
        UIManager.Inst.FullFadeOut();
    }
    // Use this for initialization
    protected override void Start()
    {
        if (SceneLoader.Inst != null)
        {
            if (SceneLoader.Inst.IsLoadingTextActive())
                SceneLoader.Inst.SetLoadingTextActive(false);
        }
        base.Start();
        //CheckPurchase();
        uiManager = UIManager.Inst;
        Player = GameObject.Find("Player");
        Room = GameObject.Find("Room");

        UIManager.Inst.LoadNarrationClip("Room");

        IsMeshVisible(delegate { EnterFunc(); });
    }

    /// <summary>
    /// 연습 버튼
    /// </summary>
    public override void PracticeFunc()
    {

        Debug.Log("<color=yellow> PracticeFunc </color>");
        UIManager.Inst.SkipNarration();
        UIManager.Inst.HideAndInteractableCanvasGroup(Intro);
        UIManager.Inst.ShowAndInteractableCanvasGroup(Practice);
        UIManager.Inst.HalfFadeOut();

        SetActive(PracticeEndButton.gameObject, false);

        MoveToDesk();
        focusGame.SetActive(true);
        focusGame.SetPracticeMode(true);
        focusGame.InitGame();
        focusGame.SetJoyStickPosition(new Vector3(0, 185, 0));

        SetPosition(PracticeEndButton.transform, new Vector3(0, 50, 0));
        // 나레이션 나오는 도중에        
        UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("ad_03R_2"));

        UIManager.Inst.ShowNarrationText("You're wondering about the N-back traininng.\n You'll see different alphabets appear on the screen one by one.\n"+
"There are no rules about the order in which they appear.", 0, 280.0f, 0);

        UIManager.Inst.WaitSeconds(10.5f, delegate { UIManager.Inst.ShowNarrationText("However, look carefully at the alphabet as shown now.\nYou must press the trigger button the second time it matches the alphabet that appeared before."); });

        UIManager.Inst.WaitSeconds(11.5f,
            delegate
            {
                focusGame.StartPracticeFocusGame(delegate
                {
                    PracticeEndFunc();
                    Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

                    //if (ConnectedClient != null && ConnectedClient.isServer)
                    //{
                    //    SendMessageFromServer("PracticeEndFunc");
                    //}
                });
            });

    }
    /// <summary>
    /// 연습 훈련 종료시
    /// </summary>
    public override void PracticeEnd()
    {
        StopWaitForSecAndFunc();
        UIManager.Inst.HalfFadeIn();
        MoveToInit();
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


    public override void Level1Step()
    {
        base.Level1Step();
        // Level 셋팅
        SetLevelIndex(0);

        // Level 별 Active해야하는 Object 활성화
        SetLevelGameObject(LevelGameObjects, 0, true);
        SetLevelGameObject(LevelGameObjects2, 0, true);
        // 책상 앞으로 이동
        MoveToDesk();

        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        //UIManager.Inst.HalfFadeOut();

        level1.InitFunc(this, LevelType.ROOM_LEVEL1, null, 0);
        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ROM_05R_L1"), "", false, delegate { focusGame.HideJoyStick(); });

        focusGame.SetActive(true);

        level1.ShowStartButton();

        // 나레이션 보여주기
        //UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n조이스틱 버튼을 누르세요", 0, 70.5f);
        UIManager.Inst.ShowNarrationText("The rest of the family is out and I'm home alone.\nIt's quiet, and with no one around, I'm kind of tempted to just lie in bed and rest..\n"+
            "but This is the perfect environment to focus, so let's sit down at my desk.", 0, 80.5f);
        // 조이스틱 버튼 보여주기 깜박이기
        focusGame.ShowJoyStick();
        focusGame.SetJoyStickPosition(0, 170, 0);


    }
    public override void Level2Step()
    {
        //App Store Upload Version
        //if (!DataManager.Inst.GetIsPurchase)
        //{
        //    base.Level2Step();
        //    return;
        //}
        base.Level2Step();
        SetLevelIndex(1);

        // Level 별 Active해야하는 Object 활성화
        SetLevelGameObject(LevelGameObjects, 1, true);
        SetLevelGameObject(LevelGameObjects2, 1, true);

        MoveToDesk();
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);

        level2.InitFunc(this, LevelType.ROOM_LEVEL2, null, 0);
        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ROM_05R_L2"),
            "",
            false,
            delegate
            {
                focusGame.HideJoyStick();
                EnvironmentSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L2_D1"), 360.0f, 1.0f);
            });
        focusGame.SetActive(true);

        level2.ShowStartButton();

        // 나레이션 보여주기
        //UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n테블릿 화면 위에 조이스틱 버튼을 누르세요", 0, 70.5f);
        UIManager.Inst.ShowNarrationText("Mom and Dad are watching TV and my sister hasn't come home yet.\n"+
"I think she'll be here soon, and it's going to be quite noisy when she comes.\n"+
"Let's just focus on training for now, shall we?", 0, 80.5f);
        // 조이스틱 버튼 보여주기 깜박이기
        focusGame.ShowJoyStick();
        focusGame.SetJoyStickPosition(0, 170, 0);

        MomCharacter.SetVoice("AD_ROM_05R_L2_D7");
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
        SetLevelIndex(2);

        // Level 별 Active해야하는 Object 활성화
        SetLevelGameObject(LevelGameObjects, 2, true);
        SetLevelGameObject(LevelGameObjects2, 2, true);

        MoveToDesk();
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        //UIManager.Inst.HalfFadeOut();

        level3.InitFunc(this, LevelType.ROOM_LEVEL3, null, 0);
        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_ROM_05R_L3"),
            "",
            false,
            delegate
            {
                focusGame.HideJoyStick();
                EnvironmentSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L3_D0"), 1200.0f, 1.0f);
            });
        focusGame.SetActive(true);
        level3.ShowStartButton();

        // 나레이션 보여주기
        //UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n테블릿 화면 위에 조이스틱 버튼을 누르세요", 0, 70.5f);
        UIManager.Inst.ShowNarrationText("Dad and sister are watching TV and my mom is cooking.\n"+
            "The upstairs neighbour sometimes plays music loudly, but it's quiet now.\n"+
            "It's still a bit noisy, but let's start training. Let's sit at the desk.", 0, 80.5f);
        // 조이스틱 버튼 보여주기 깜박이기
        focusGame.ShowJoyStick();
        focusGame.SetJoyStickPosition(0, 170, 0);

        MomCharacter.SetVoice("AD_ROM_05R_L3_D8");
    }

    private void MoveToDesk()
    {
        MoveChracter(-2.82f, 11.3f, -9.5f);
    }
    private void MoveToInit()
    {
        MoveChracter(-4.22f, 14.87f, -3.7f);
    }
    private void MoveChracter(Vector3 tVec)
    {
        Room.transform.localPosition = tVec;
    }
    private void MoveChracter(float x, float y, float z)
    {
        Player.transform.localPosition = new Vector3(x, y, z);
    }


    public override void FocusGameStart()
    {
        DiscouragementLevel = 0;
        base.FocusGameStart();
        float[] elapseTimes = null;
        UnityAction[] callbacks = null;
        if (LevelIndex == 0)
        {
            elapseTimes = new float[]
            {
                360.0f, 240.0f
            };
            callbacks = new UnityAction[]
            {
            delegate{ Phase0_0(); },
            delegate { Phase0_1(); }
            };
        }
        else if (LevelIndex == 1)
        {
            elapseTimes = new float[]
            {
                20.0f, 70.0f, 90.0f, 10.0f,
                110.0f, 180.0f, 180.0f, 120.0f

            };
            callbacks = new UnityAction[]
            {
                delegate{ Phase1_0(); },
                delegate{ Phase1_1(); },
                delegate{ Phase1_2(); },
                delegate{ Phase1_3(); },
                delegate{ Phase1_4(); },
                delegate{ Phase1_5(); },
                delegate{ Phase1_6(); },
                delegate{ Phase1_7(); }
            };
        }
        else if (LevelIndex == 2)
        {
            elapseTimes = new float[]
            {
                40.00f, 80.00f, 180.00f, 120.00f,
                30.00f, 90.00f, 30.00f, 60.00f,
                30.00f, 120.00f
            };

            //elapseTimes = new float[]
            //{
            //    0.00f, 20.00f, 20.00f, 20.00f,
            //    20.00f, 20.00f, 20.00f, 20.00f,
            //    20.00f, 20.00f
            //};


            callbacks = new UnityAction[]
            {
                delegate { Phase2_0(); },
                delegate { Phase2_1(); },
                delegate { Phase2_2(); },
                delegate { Phase2_3(); },
                delegate { Phase2_4(); },
                delegate { Phase2_5(); },
                delegate { Phase2_6(); },
                delegate { Phase2_7(); },
                delegate { Phase2_8(); },
                delegate { Phase2_9(); }
            };
        }
        StartDiscouragement(elapseTimes, callbacks);
    }

    protected override void EndSetting()
    {
        MomCharacter.StopAnimation();
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

    private void Phase0_0()
    {
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L1_D1"));
    }
    private void Phase0_1()
    {
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L1_D2"));
    }

    private void Phase1_0()
    {
        // 엄마 아빠 웃는 소리 20, 40초 때                            
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L2_D2"), 0, 1, 0);
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L2_D2"), 20, 1, 0);
    }
    private void Phase1_1()
    {
        // 엄마 아빠 서로 대화하는 소리  1분 30초
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L2_D3"));
    }
    private void Phase1_2()
    {
        // 엄마 대사 후 아빠 대사 3분
        DiscouragementSound.Inst.WaitAndFunc(0, UIManager.Inst.FindNarration("AD_ROM_05R_L2_D4-1"), UIManager.Inst.FindNarration("AD_ROM_05R_L2_D4-2"));
    }
    private void Phase1_3()
    {
        // 엄마 대사 3분 10초
        DiscouragementSound.Inst.WaitAndFunc(0, UIManager.Inst.FindNarration("AD_ROM_05R_L2_D5"));
    }
    private void Phase1_4()
    {
        // 초인종 소리 후 여동생 대사
        DiscouragementSound.Inst.WaitAndFunc(0, UIManager.Inst.FindNarration("AD_ROM_05R_L2_D6-1"), UIManager.Inst.FindNarration("AD_ROM_05R_L2_D6-2"));
    }
    private void Phase1_5()
    {
        // 엄마 방 문 열고 들어오는 애니메이션 후 대사 8분
        MomCharacter.SetPosition(0, 0, 0.0f);
        MomCharacter.ActiveRenderer(true);
        Door.SetTrigger("OpenDoor");
        MomCharacter.SetTrigger("LV2_RoomComeIn");
        MomCharacter.PlayLipSyncAndSound();
    }
    private void Phase1_6()
    {
        // 여동생 대사 후 엄마 대사 11분
        DiscouragementSound.Inst.WaitAndFunc(0, UIManager.Inst.FindNarration("AD_ROM_05R_L2_D8-1"), UIManager.Inst.FindNarration("AD_ROM_05R_L2_D8-2"));
    }
    private void Phase1_7()
    {
        // 아빠 웃는 소리 13분 (엄마아빠 웃는 소리로 일단 놔둠)
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L2_D2"));
    }

    private void Phase2_0()
    {
        // 여동생이 날카롭게 웃는 소리 40초, 60초
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L3_D1"), 0);
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L3_D1"), 20.0f);
    }
    private void Phase2_1()
    {
        // 1분간 믹서기 가는 소리 2분 
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L3_D3"));
    }
    private void Phase2_2()
    {
        // 마늘 빻는 소리 1분간
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L3_D4"));
    }
    private void Phase2_3()
    {
        // 위층에서 들려오는 락음악 7분 때
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L3_D5"), 0, 180.0f, 0, 0.7f);
    }
    private void Phase2_4()
    {
        // 엄마 대사 , 여동생 대사, 아빠 대사 7분 30초
        DiscouragementSound.Inst.WaitForSecAndPlay(0, 1, UIManager.Inst.FindNarration("AD_ROM_05R_L3_D6-1"),
                                                         UIManager.Inst.FindNarration("AD_ROM_05R_L3_D6-2"),
                                                         UIManager.Inst.FindNarration("AD_ROM_05R_L3_D6-3"));
    }
    private void Phase2_5()
    {
        // 아빠 대사 9분
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L3_D7"));
    }
    private void Phase2_6()
    {
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("doorknock"));
        // 엄마 애니메이션 9분 30초
        MomCharacter.SetPosition(0, 0, 0.0f);
        WaitForSecAndFunc(3.0f,
            delegate
            {
                Door.SetTrigger("OpenDoor");
                MomCharacter.SetTrigger("LV3_RoomComeIn");
                MomCharacter.PlayLipSyncAndSound();
            });
    }
    private void Phase2_7()
    {
        // 더 커지는 음악 소리 10분 30초
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L3_D9"), 0, 180.0f, 0, 1.0f);
    }
    private void Phase2_8()
    {
        // 아빠, 여동생, 엄마 11분
        DiscouragementSound.Inst.WaitForSecAndPlay(0, 1, UIManager.Inst.FindNarration("AD_ROM_05R_L3_D10-1"),
                                                         UIManager.Inst.FindNarration("AD_ROM_05R_L3_D10-2"),
                                                         UIManager.Inst.FindNarration("AD_ROM_05R_L3_D10-3"));
    }
    private void Phase2_9()
    {
        // 현관문 닫히는 소리
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_ROM_05R_L3_D11"));
    }

    protected override void Update()
    {
        base.Update();

    }

}
