using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneReference : SceneBase
{
    public GameObject player;

    // 레벨별 공간 리스트
    public List<GameObject> LevelArea;
    // 레벨별 캐릭터 시작 위치 리스트
    public List<Vector3> PlayerPos = new List<Vector3>();

    // 레벨별 캐릭터 리스트
    public List<Character> Characters = new List<Character>();
    
    // Level2
    // 유저2 손에 있는 콜라캔
    public GameObject User2_Coke;
    // 책상에 붙어있는 콜라캔
    public GameObject User2_Coke2;

    // 유저1이 들고 있는 책
    [SerializeField]
    private GameObject User1_Book;
    // 유저3이 쥐고 있는 펜
    [SerializeField]
    private GameObject User3_Pen;

    protected override void Awake()
    {
        PlayerPos.Add(new Vector3(-73.03f, 12.059f, 29.599f));
        PlayerPos.Add(new Vector3(-51.22f, 12.05f, 8.97f));
        PlayerPos.Add(new Vector3(-32.78f, -4.63f, -29.91f));
    }
    protected override void EnterFunc()
    {
        Level2Step();
    }
    // Use this for initialization
    protected override void Start()
    {
        if (SceneLoader.Inst != null)
        {
            if (SceneLoader.Inst.IsLoadingTextActive())
                SceneLoader.Inst.SetLoadingTextActive(false);
        }
        player = GameObject.Find("Player");
        
        SetPosition(player.transform, PlayerPos[1]);
        SetRotation(player.transform, 0, 180);
        //SetLevelGameObject(LevelArea, 0, true);
        
        base.Start();
        //CheckPurchase();
        // 유저2 콜라 액티브 flase
        SetActive(User2_Coke, false);
        // 책상에 붙어있는 콜라 액티브 false
        SetActive(User2_Coke2, false);
        // Level Index Set
        SetLevelIndex(1);
        // 유저1 책 액티브 false
        SetActive(User1_Book, false);
        // 유저3 책 액티브 true
        SetActive(User3_Pen, true);


        // 캐릭터 리스트 셋팅
        Character[] temp = GameObject.Find("reference_room/Characters").GetComponentsInChildren<Character>(true);
        Characters.Clear();
        for (int i = 0; i < temp.Length; i++)
        {
            Characters.Add(temp[i]);
            Characters[i].InitFunc();
            Characters[i].LoadLipSyncData("Lib/");
        }
        Characters[1].SetActive(false);

        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);

        IsMeshVisible(delegate { EnterFunc(); });

        UIManager.Inst.LoadNarrationClip("Lib");

        // 레벨 선택 시 반복 환경음 시작
        DiscouragementSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D2"), 180.0f, 1, 1);
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D0"), 1);
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D5_1"), 1);
    }

    public override void Level1Step()
    {
        base.Level1Step();
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        //UIManager.Inst.FullFadeIn(delegate { SceneLoader.Inst.LoadScene(6); });
        UIManager.Inst.FullFadeIn(delegate { SceneLoader.Inst.LoadScene("SceneLibrary"); });
    }

    public override void Level2Step()
    {
        // App Store Upload Version
        //if (!DataManager.Inst.GetIsPurchase)
        //{
        //    base.Level2Step();
        //    return;
        //}
        base.Level2Step();
        // 페이드 인
        UIManager.Inst.FullFadeOut(            
            delegate { SetPosition(player.transform, PlayerPos[1]); },
            delegate { SetRotation(player.transform, 0, 180); },
            delegate { level2.InitFunc(this, LevelType.LIBRARY_LEVEL2, null, 1); },
            delegate { level2.ShowStartButton(); },
            delegate { focusGame.SetActive(true); },
            delegate { focusGame.ShowJoyStick(); },
            delegate { focusGame.SetJoyStickPosition(0, 120, 0); },
            delegate { UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_LIB_05L_L2"), "", false, delegate { focusGame.HideJoyStick(); }); },
            // 나레이션 보여주기
            delegate {
                //UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n컨트롤러 버튼을 누르세요", 0, -108.0f);
                UIManager.Inst.ShowNarrationText("There are quite a few people in the data room.\n Let’s study at the open table in the middle of the data room.\n"+
"Although my attention may be distracted by people's movements, I decide to focus and train.\n"+
"So, shall we go to an empty seat and start? ", 0, -108.0f);
            }
        );
    }

    public override void Level3Step()
    {
        // App Store Upload Version
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

    public override void FocusGameStart()
    {        
        base.FocusGameStart();
        float[] elaseTimes = new float[]
        {
            30.00f, 90.0f, 60.0f, 120.0f,
            30.0f, 60.0f, 90.0f, 2.75f,
            3.75f, 3.8f, 3.0f, 3.5f,
            4.5f, 120.0f, 30.0f, 60.0f,
            60.0f
        };

        //float[] elaseTimes = new float[]
        //{
        //    0.00f, 10.0f, 10.0f, 10.0f,
        //    10.0f, 10.0f, 10.0f, 2.75f,
        //    3.75f, 3.8f, 3.0f, 3.5f,
        //    4.5f, 10.0f, 10.0f, 10.0f,
        //    10.0f
        //};

        UnityAction[] callbacks = new UnityAction[]
        {
            delegate{ Phase1(); },  delegate{ Phase2(); },  delegate{ Phase3(); },
            delegate{ Phase4(); },  delegate{ Phase5(); },  delegate{ Phase6(); },
            delegate{ Phase7(); },  delegate{ Phase8(); },  delegate{ Phase9(); },
            delegate{ Phase10(); }, delegate{ Phase11(); }, delegate{ Phase12(); },
            delegate{ Phase13(); }, delegate{ Phase14(); }, delegate{ Phase15(); },
            delegate{ Phase16(); }, delegate{ Phase17(); }
        };

        StartDiscouragement(elaseTimes, callbacks);
    }

    protected override void EndSetting()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Characters[i].StopAnimation();
        }
    }


    public override void FocusGameEnd()
    {
        EndSetting();
        StopDiscouragement();
        FocusPlayTime = 0.0f;
        DiscouragementLevel = 0;
        EnvironmentSound.Inst.AllLoopStop();
        EnvironmentSound.Inst.AllPlayStop();
        DiscouragementSound.Inst.AllStop();
        base.FocusGameEnd();
    }

    private void Phase1()
    {
        // 펜쓰는 소리, 2분 간격
        DiscouragementSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D1"), 120.0f, 1, 1);
    }
    private void Phase2()
    {
        //책장 넘기는 소리 3분 간격
        DiscouragementSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D2"), 180.0f, 1, 1);

    }
    private void Phase3()
    {
        Characters[1].SetActive(true);
        // 키보드(노트북) 소리
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D3"), 1);
    }
    private void Phase4()
    {
        Characters[0].WaitForSecAndAniPlay("walk", 14.0f);
        Characters[1].SetAnimation("idle", 0, 21.02f);
        DiscouragementSound.Inst.WaitAndFunc(1, UIManager.Inst.FindNarration("AD_LIB_05L_L2_D4-1"),
                                             UIManager.Inst.FindNarration("AD_LIB_05L_L2_D4-2"));
        Characters[1].WaitForSecAndAniPlay("follow", 14.0f);
    }
    private void Phase5()
    {
        // 키보드(노트북) 소리
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D3"), 1);
    }
    private void Phase6()
    {
        // 의자 끄는 소리 , 걷는 소리
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D3"), 1);
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D5_1"), 1);
    }
    private void Phase7()
    {
        // 이용객 2 대사
        Characters[2].SetTrigger("idle");
        Characters[2].SetVoice("AD_LIB_05L_L2_D6-1");
        Characters[2].PlayLipSyncAndSound();
    }
    private void Phase8()
    {
        // 이용객 3 대사
        Characters[3].SetTrigger("idle");
        Characters[3].SetVoice("AD_LIB_05L_L2_D6-2");
        Characters[3].PlayLipSyncAndSound();
    }
    private void Phase9()
    {
        // 이용객 2 대사
        Characters[2].SetVoice("AD_LIB_05L_L2_D6-3");
        Characters[2].PlayLipSyncAndSound();
    }
    private void Phase10()
    {
        // 이용객 3 대사
        Characters[3].SetVoice("AD_LIB_05L_L2_D6-4");
        Characters[3].PlayLipSyncAndSound();
    }
    private void Phase11()
    {
        // 유저3 책 액티브 false
        SetActive(User3_Pen, false);
        // 이용객 2, 3 밖으로 이동
        Characters[2].SetTrigger("standup_walk");
        Characters[3].SetTrigger("standup_walk");
        // 의자 끄는 소리, 걷는 소리
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D3"), 1);
    }
    private void Phase12()
    {
        // 의자 끄는 소리, 걷는 소리
        //DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D3"), 1);
        //DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D5_1"), 1);
    }
    private void Phase13()
    {
        // 사람들 걸어다니는 소리 30초 간격 반복
        //DiscouragementSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D7"), 30.0f);

    }
    private void Phase14()
    {
        // 유저3 책 액티브 true
        SetActive(User3_Pen, true);

        // 이용객 2, 3 다시 돌아와 앉음
        // 의자 끄는 소리 및 캔 음료 내려 놓는 소리
        SetActive(User2_Coke, true);
        Characters[2].SetTrigger("walk_sitdown");
        Characters[3].SetTrigger("walk_sitdown");
        WaitForSecAndFunc(9.4f, delegate { SetActive(User2_Coke, false); });
        WaitForSecAndFunc(9.4f, delegate { SetActive(User2_Coke2, true); });
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D8"), 9.4f);
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D3"), 9.5f);
    }
    private void Phase15()
    {
        // 이용객 3 머리를 만짐
        Characters[2].SetTrigger("study_idle");
        Characters[3].SetTrigger("hair_motion");
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L1_D3"), 1);
    }
    private void Phase16()
    {
        // 이용객2,3의 소곤거리는 소리
        // 10초 간격으로 노트북 키보드 소리 반복
        Characters[2].SetTrigger("idle");
        Characters[3].SetTrigger("idle");
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D10"), 1);
        DiscouragementSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D3"), 10.0f);
    }
    private void Phase17()
    {
        // 유저1 책 액티브 false
        SetActive(User1_Book, true);
        // 이용객 1 카운터 앞으로 이동 후 퇴장
        Characters[1].SetTrigger("runaway");
        Characters[1].WaitForSecAndAniPlay("ending_idle", 17.0f);
        Characters[1].WaitForSecAndAniPlay("ending_step", 20.0f);
        // 대화음 시작          
        DiscouragementSound.Inst.WaitForSecAndPlay(16.0f, 1, UIManager.Inst.FindNarration("AD_LIB_05L_L2_D11-1-학생"),
            UIManager.Inst.FindNarration("AD_LIB_05L_L2_D11-2-사서")
            );
    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();

        //frame.text = string.Format("{0} / {1}", FocusPlayTime.ToString(), Time.deltaTime);

        //if (isGameStart)
        //{
        //    FocusPlayTime += Time.deltaTime;
        //    StartDiscouragement();
        //}
    }

}
