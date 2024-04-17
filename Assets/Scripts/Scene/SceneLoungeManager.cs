using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SceneLoungeManager : SceneBase {

    public GameObject player;

    // 레벨별 캐릭터 시작 위치 리스트
    public List<Vector3> PlayerPos = new List<Vector3>();

    // 레벨별 캐릭터 리스트
    public List<Character> Characters = new List<Character>();
    
    // Level3
    // 이용객 2의 핸드폰 오브젝트
    public GameObject User2_CellPhone;
    // 컵라면 오브젝트
    public GameObject User2_CupRamen;
    // 책상위 컵라면 오브젝트
    public GameObject User2_CupRamen2;
    // 컵라면 덮개 오브젝트
    public GameObject User2_CupRamenCover;
    // 젓가락 오브젝트
    public GameObject User2_Chopsticks;
    // 유저3(어린아이)의 과자봉지 오브젝트
    public GameObject User3_Snack;

    public Text frame;

    //private IEnumerator WaitForSecAndFuncIE;

    protected override void Awake()
    {
        PlayerPos.Add(new Vector3(-73.03f, 12.059f, 29.599f));
        PlayerPos.Add(new Vector3(-51.22f, 12.05f, 8.97f));
        PlayerPos.Add(new Vector3(-32.78f, -4.63f, -29.91f));
    }

    protected override void EnterFunc()
    {
        Level3Step();
    }

    // Use this for initialization
    protected override void Start()
    {
        if (SceneLoader.Inst != null)
        {
            if (SceneLoader.Inst.IsLoadingTextActive())
                SceneLoader.Inst.SetLoadingTextActive(false);
        }
        Character[] temp = GameObject.Find("Lounge/Characters").GetComponentsInChildren<Character>(true);
        Characters.Clear();
        for (int i = 0; i < temp.Length; i++)
        {
            Characters.Add(temp[i]);
            Characters[i].InitFunc();
            Characters[i].LoadLipSyncData("Lib/");
        }
        Characters[0].SetActive(false);        
        Characters[1].SetActive(false);
        Characters[2].SetActive(false);
        Characters[3].SetActive(false);        
        Characters[7].SetActive(false);

        Characters[0].SetActive(true);
        

        IsMeshVisible(delegate { EnterFunc(); });

        player = GameObject.Find("Player");
        
        SetPosition(player.transform, PlayerPos[2]);
        
        base.Start();
        //CheckPurchase();
        UIManager.Inst.LoadNarrationClip("Lib");

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
        //App Store Upload Version
        //if (!DataManager.Inst.GetIsPurchase)
        //{
        //    base.Level2Step();
        //    return;
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
        SetActive(User2_Chopsticks, false);
        SetActive(User3_Snack, false);
        SetActive(User2_CupRamen, true);
        SetActive(User2_CupRamenCover, true);
        // Level Index Set
        SetLevelIndex(2);

        Character[] temp = GameObject.Find("Lounge/Characters").GetComponentsInChildren<Character>(true);
        Characters.Clear();
        for (int i = 0; i < temp.Length; i++)
        {
            Characters.Add(temp[i]);
            Characters[i].InitFunc();
            Characters[i].LoadLipSyncData("Lib/");            
        }
        Characters[0].SetActive(false);
        Characters[0].SetActive(true);

        Characters[1].SetActive(false);
        Characters[2].SetActive(false);

        Characters[3].SetActive(true);
        Characters[3].SetActive(false);
        Characters[6].SetPosition(new Vector3(-0.671f, 0.0f, 2.854f));
        Characters[7].SetActive(false);

        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);

        // 페이드 인
        UIManager.Inst.FullFadeOut(            
            delegate { SetPosition(player.transform, PlayerPos[2]); },
            delegate { SetRotation(player.transform, 0, 0); },
            delegate { level3.InitFunc(this, LevelType.LIBRARY_LEVEL3, null, 1); },
            delegate { level3.ShowStartButton(); },
            delegate { focusGame.SetActive(true); },
            delegate { focusGame.ShowJoyStick(); },
            delegate { focusGame.SetJoyStickPosition(0, 120, 0); },
            delegate { UIManager.Inst.WaitForNarrationAndFunc(
                UIManager.Inst.FindNarration("AD_LIB_05L_L3"), 
                "", 
                false, 
                delegate 
                {
                    focusGame.HideJoyStick();
                    EnvironmentSound.Inst.EnvironmentSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D1"), 0.6f, true);
                    EnvironmentSound.Inst.EnvironmentSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D13-1"), 0.4f, false);
                });
            },
            // 나레이션 보여주기
            delegate {
                //UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n테블릿 화면 위에 조이스틱 버튼을 누르세요", 0, -108.0f);
                UIManager.Inst.ShowNarrationText("Because it was the weekend, the place was full. I think I'll have to study in the empty space in the break room.\n"+
"I'm worried about the surrounding noise, but let's focus on training!\n"+
"Now, shall we sit down at an empty seat and get started? ", 0, -108.0f);
            }
        );

    }

    public override void FocusGameStart()
    {        
        base.FocusGameStart();

        float[] elapseTimes = new float[]
        {
            30, 60, 30, 90, 60,
            30, 90, 60, 90, 120,
            30, 30, 30, 30
        };

        //float[] elapseTimes = new float[]
        //{
        //    0, 10, 10, 10, 1,
        //    10, 10, 10, 10, 180,
        //    10, 10, 10, 10
        //};
        UnityAction[] callbacks = new UnityAction[]
        {
            delegate{ Phase1(); },
            delegate{ Phase2(); },
            delegate{ Phase3(); },
            delegate{ Phase4(); },
            delegate{ Phase5(); },
            delegate{ Phase6(); },
            delegate{ Phase7(); },
            delegate{ Phase8(); },
            delegate{ Phase9(); },
            delegate{ Phase10(); },
            delegate{ Phase11(); },
            delegate{ Phase12(); },
            delegate{ Phase13(); },
            delegate{ Phase14(); }
        };
        StartDiscouragement(elapseTimes, callbacks);
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
        EnvironmentSound.Inst.AllLoopStop();
        EnvironmentSound.Inst.AllPlayStop();
        DiscouragementSound.Inst.AllStop();
        base.FocusGameEnd();
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
    private void Phase0()
    {
        EnvironmentSound.Inst.ElapseDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D1"), 600.0f);
        Characters[6].WaitForSecAndAniPlay("walkingloop", 0.0f);
    }
    private void Phase1()
    {
        // 책장 넘기는 소리 2분 간격 반복 30초 때
        EnvironmentSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D2"), 120.0f, 0.8f);
        Characters[6].WaitForSecAndAniPlay("walkingloop", 0.0f);

    }
    private void Phase2()
    {
        // 의자 끄는 소리 5분 간격 반복 1분 30초 때
        EnvironmentSound.Inst.LoopDiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D3"), 300.0f, 0.8f);
        WaitForSecAndFunc(5.0f, delegate { Characters[1].SetActive(true); });
    }
    private void Phase3()
    {
        // 2분 때
        // 부스럭 거리는 소리
        // 이용객 1 전화통화 소리 
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D7-6"),1);
        Characters[1].SetTrigger("idle_R");
        Characters[1].SetVoice("AD_LIB_05L_L3_D0");
        Characters[1].PlayLipSyncAndSound();
        Characters[6].WaitForSecAndAniPlay("walkingloop", 0.0f);
    }
    private void Phase4()
    {
        // 3분 30초 때
        // 이용객 1 대사
        Characters[1].SetTrigger("idle_R");
        Characters[1].SetVoice("AD_LIB_05L_L3_D5");
        Characters[1].PlayLipSyncAndSound();
        Characters[1].WaitForSecAndAniPlay("idle2_R", 7.9f);
    }
    private void Phase5()
    {
        // 4분 30초 때
        // 사람들 발소리 (Level2 껄 가져다 씀)
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L2_D7"),1);
        Characters[6].WaitForSecAndAniPlay("walkingloop", 0.0f);
    }
    private void Phase6()
    {
        // 5분 때
        // 이용객 2 액티브 활성화 및 
        Characters[2].SetActive(true);
        User2_CellPhone.SetActive(false);
        Characters[2].SetTrigger("ramen_pickup");
        // 이용객 2 컵라면 산다는 대사 후 매점 직원 대사
        DiscouragementSound.Inst.WaitForSecAndPlay(3.0f, 1, UIManager.Inst.FindNarration("AD_LIB_05L_L2_D6-1-1"), UIManager.Inst.FindNarration("AD_LIB_05L_L2_D6-2-아줌마"));

        Characters[2].WaitForSecAndAniPlay("back_ramenwater", 7.8f);
        // 물 받는 소리
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D6-3"), 12.0f, 2.7f, 1, 1);
    }
    private void Phase7()
    {
        // 6분 30초
        // 이용객 3 액티브 활성화 및 달려오는 애니메이션 실행
        Characters[3].SetActive(true);
        Characters[3].WaitForSecAndAniPlay("stand_idle", 4.8f);
        // 달려가면서 테이블을 치고가는 소리 재생
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D7-2"), 0.5f, 1, 1, 1);
        // 몇 초 후에 이용객 3 대사 및 이어서 이용객 6 대사, 매점직원 대사
        DiscouragementSound.Inst.WaitForSecAndPlay(4.8f, 1,
            UIManager.Inst.FindNarration("AD_LIB_05L_L3_D7-3"),
            UIManager.Inst.FindNarration("AD_LIB_05L_L3_D7-4"),
            UIManager.Inst.FindNarration("AD_LIB_05L_L3_D7-5"));

        // 대화 종료 후 자리에 앉는 애니메이션 실행
        Characters[3].WaitForSecAndAniPlay("comebackseat", 10.8f);
        Characters[3].WaitForSecAndAction(new float[] { 10.8f, 6.5f }, delegate { SetActive(User3_Snack, true); },
                                                                       delegate { SetActive(User3_Snack, false); });
        Characters[6].WaitForSecAndAniPlay("mom_walking", 10.8f);
    }
    private void Phase8()
    {
        // 7분 30초
        // 부스럭거리는 소리
        DiscouragementSound.Inst.DiscouragementSoundPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D4"),1);
    }
    private void Phase9()
    {
        // 9분
        // 이용객2 컵라면 들고 자리에 앉아 먹는 소리 20초 지속
        SetActive(User2_Chopsticks, true);
        SetActive(User2_CupRamenCover, false);
        //Characters[2].WaitForSecAndAniPlay("walk_seat", 0);
        Characters[2].WaitForSecAndAniPlay("ramen_eating", 0.0f);
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 20.0f, 1, 1);

        Characters[2].WaitForSecAndAction(
            new float[]
            {
                20.0f, 10.0f, 8.0f,
                10.0f, 8.0f,
                10.0f, 8.0f,
                10.0f, 8.0f,
                10.0f, 8.0f,
                10.0f, 8.0f,
                10.0f, 8.0f,
                10.0f, 8.0f,
                10.0f, 8.0f                
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
            delegate
            {
                Characters[2].SetTrigger("ramen_eating");
                DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 8.0f, 1, 1);
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
            delegate
            {
                Characters[2].SetTrigger("ramen_eating");
                DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 8.0f, 1, 1);
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
            delegate
            {
                Characters[2].SetTrigger("ramen_eating");
                DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 8.0f, 1, 1);
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
            delegate
            {
                Characters[2].SetTrigger("ramen_eating");
                DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 8.0f, 1, 1);
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
            delegate
            {
                Characters[2].SetTrigger("ramen_eating");
                DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 8.0f, 1, 1);
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
            delegate
            {
                Characters[2].SetTrigger("ramen_eating");
                DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 8.0f, 1, 1);
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
            delegate
            {
                Characters[2].SetTrigger("ramen_eating");
                DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 8.0f, 1, 1);
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
            delegate
            {
                Characters[2].SetTrigger("ramen_eating");
                DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 8.0f, 1, 1);
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
            delegate
            {
                Characters[2].SetTrigger("ramen_eating");
                DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 8.0f, 1, 1);
            },
            delegate { Characters[2].SetTrigger("ramen_eating_idle2"); }
        );

        //Characters[2].WaitForSecAndAction(
        //    new float[]
        //    {
        //        20.0f,
        //        20.1f,
        //        12.0f,
        //        30.0f,
        //        12.0f,
        //        40.0f,
        //        12.0f
        //    },
        //    delegate{ Characters[2].SetTrigger("ramen_eating_idle2");  },
        //    delegate 
        //    {
        //        Characters[2].SetTrigger("ramen_eating");
        //        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 12.0f, 1, 1);
        //    },
        //    delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
        //    delegate
        //    {
        //        Characters[2].SetTrigger("ramen_eating");
        //        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 12.0f, 1, 1);
        //    },
        //    delegate { Characters[2].SetTrigger("ramen_eating_idle2"); },
        //    delegate
        //    {
        //        Characters[2].SetTrigger("ramen_eating");
        //        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D9-2"), 0.0f, 12.0f, 1, 1);
        //    },
        //    delegate { Characters[2].SetTrigger("ramen_eating_idle2"); }            
        //);

    }

    private void Phase10()
    {
        // 11분
        DiscouragementSound.Inst.WaitAndFunc(1,
            UIManager.Inst.FindNarration("AD_LIB_05L_L3_D11-1"),
            UIManager.Inst.FindNarration("AD_LIB_05L_L3_D11-2"),
            UIManager.Inst.FindNarration("AD_LIB_05L_L3_D11-3"),
            UIManager.Inst.FindNarration("AD_LIB_05L_L3_D11-4")
            );
    }
    private void Phase11()
    {
        // 11분 30초
        Characters[7].SetActive(true);
    }
    private void Phase12()
    {
        // 12분
        // 핸드폰 진동 소리 후 이용객2 대사
        Characters[2].WaitForSecAndAniPlay("phone", 5.95f);
        Characters[2].WaitForSecAndAniPlay("phone", 5.95f);
        Characters[2].WaitForSecAndAction(new float[] { 5.95f }, delegate { User2_CellPhone.SetActive(true); });

        Characters[2].WaitForSecAndAction(new float[] { 5.95f }, delegate { User2_CupRamen.SetActive(false); });

        Characters[2].WaitForSecAndAction(new float[] { 5.95f }, delegate { User2_CupRamen2.SetActive(true); });

        //Characters[2].WaitForSecAndAction(new float[] { 5.95f, 10.0f, 5.5f }, delegate { User2_Chopsticks.SetActive(false); },
        //                                                                        delegate { User2_Chopsticks.SetActive(true); },
        //                                                                        delegate { User2_Chopsticks.SetActive(false); });
        Characters[2].SetVoice("AD_LIB_05L_L3_D13-2");
        Characters[2].WaitForSecAndAction(new float[] { 5.95f }, delegate { Characters[2].PlayLipSyncAndSound(); });

        DiscouragementSound.Inst.WaitAndFunc(1,
            UIManager.Inst.FindNarration("AD_LIB_05L_L3_D13-1")
            );

    }
    private void Phase13()
    {
        // 12분 30초
        Characters[2].WaitForSecAndAction(new float[] { 0.2f }, delegate { User2_CellPhone.SetActive(false); });
        Characters[2].WaitForSecAndAction(new float[] { 0.4f, 5.5f }, delegate { User2_CupRamen.SetActive(true); },
                                                                      delegate { User2_CupRamen.SetActive(false); });
        Characters[2].WaitForSecAndAction(new float[] { 0.40f }, delegate { User2_CupRamen2.SetActive(false); });
        Characters[2].WaitForSecAndAction(new float[] { 5.95f }, delegate { User2_Chopsticks.SetActive(false); });
        Characters[2].SetTrigger("seat_walk");
        DiscouragementSound.Inst.WaitForSecAndPlay(UIManager.Inst.FindNarration("AD_LIB_05L_L3_D3"), 0.2f);
    }
    private void Phase14()
    {
        // 13분
        Characters[0].SetTrigger("walk");
    }
}
