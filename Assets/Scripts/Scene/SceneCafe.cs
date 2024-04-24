using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SceneCafe : SceneBase
{

    private Vector3[] InitPosition;

    protected override void Awake()
    {
        base.Awake();
        videoPlayer = GetComponent<VideoController>();
    }

    private IEnumerator enterRoutine;

    private void StartEnterRoutine()
    {
        if (enterRoutine != null)
            StopCoroutine(enterRoutine);

        enterRoutine = EnterFuncRoutine();
        StartCoroutine(enterRoutine);
    }

    private IEnumerator EnterFuncRoutine()
    {
        //Debug.Log($"<color=cyan> [EntrFunc] 시작</color>");

        //videoPlayer.LoadVideoInfo();
        //videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.CAFEALONE1].GetVideoPath);        
        videoPlayer.SetFrame(10);
        videoPlayer.Volume = 0;
        videoPlayer.VideoPause();

        yield return new WaitForSeconds(1f);

        if (SceneLoader.Inst != null)
        {
            if (SceneLoader.Inst.IsLoadingTextActive())
                SceneLoader.Inst.SetLoadingTextActive(false);
        }
        UIManager.Inst.FullFadeOut();

        UIManager.Inst.LoadNarrationClip("Cafe");
        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("ad_03C_1"),
            "You chose to train at a cafe!\n" +
"<color=#35AC88>Some cafes are quiet and great places to work, but others can be so crowded and noisy that it's hard to hear what the other person is saying.\n" +
"How about doing N-back training for 2 minutes in a cafe with a variety of atmospheres?</color>\n" +
"Not sure what N - back training is?\n" +
"Click the practice button below to hear a detailed explanation of the training.",

            true,
            delegate { ShowStartButton(); });
    }

    protected override void EnterFunc()
    {
        //Debug.Log($"<color=cyan> [EntrFunc] 시작</color>");

        //videoPlayer.LoadVideoInfo();
        //videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.CAFEALONE1].GetVideoPath);        
        videoPlayer.SetFrame(10);
        videoPlayer.Volume = 0;
        videoPlayer.VideoPause();

        if (SceneLoader.Inst != null)
        {
            if (SceneLoader.Inst.IsLoadingTextActive())
                SceneLoader.Inst.SetLoadingTextActive(false);
        }
        UIManager.Inst.FullFadeOut(); // 씬 로드시 영상 로드되기 전에 스카이박스 보이는 현상으로 주석 처리 [2024.04.22 수정]

        UIManager.Inst.LoadNarrationClip("Cafe");
        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("ad_03C_1"),
            "You chose to train at a cafe!\n" +
"<color=#35AC88>Some cafes are quiet and great places to work, but others can be so crowded and noisy that it's hard to hear what the other person is saying.\n" +
"How about doing N-back training for 2 minutes in a cafe with a variety of atmospheres?</color>\n" +
"Not sure what N - back training is?\n" +
"Click the practice button below to hear a detailed explanation of the training.",

            true,
            delegate { ShowStartButton(); });
    }

    /// <summary>
    /// 연습 버튼
    /// </summary>
    public override void PracticeFunc()
    {
        UIManager.Inst.SkipNarration();
        UIManager.Inst.HideAndInteractableCanvasGroup(Intro);
        UIManager.Inst.ShowAndInteractableCanvasGroup(Practice);
        //UIManager.Inst.HalfFadeOut();

        SetActive(PracticeEndButton.gameObject, false);

        focusGame.SetActive(true);
        focusGame.SetPracticeMode(true);
        focusGame.InitGame();
        focusGame.SetJoyStickPosition(new Vector3(0, 36, 0));
        // 나레이션 나오는 도중에        
        UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("ad_03C_2"));
        //delegate { PracticNextNarration(); });

        UIManager.Inst.ShowNarrationText("You were wondering what N-back training is!\n" +
"The position and color of the colored points inside the square divided into four parts change.\n" +
"They will appear on the screen one by one.\n" +
"There are no rules as to the order in which shapes appear.", 0, 0, 0);

        // 7초 뒤 랜덤 도형이 나와서 움직임.
        //UIManager.Inst.WaitSeconds(7.3f, delegate { focusGame.ShowFigure(); });

        UIManager.Inst.WaitSeconds(16.5f, delegate
        {
            UIManager.Inst.ShowNarrationText("However, take a close look at the location and color of this point.\n" +
"You have to press the button on the trigger button when the same shape as before appears the second time.");
        });

        UIManager.Inst.WaitSeconds(8.0f,
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

    protected override void IsAssetLoad(params UnityAction[] callback)
    {
        videoPlayer.LoadVideoInfo();
        videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.CAFEALONE1].GetVideoPath);
        StartCoroutine(LoadCompleteRoutine(callback));
    }
    private IEnumerator LoadCompleteRoutine(params UnityAction[] callback)
    {
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        yield return SendCompleteSceneLoaded();
        yield return CheckSceneLoadedRoutine();

        //Debug.Log($"<color=cyan> 비디오 로드 완료</color>");

        for (int i = 0; i < callback.Length; i++)
        {
            callback[i].Invoke();
        }
    }
    protected override void Start()
    {
        base.Start();
        //CheckPurchase();
        InitPosition = new Vector3[3];

        //InitPosition[0] = new Vector3(0, 17.73f, 0);
        //InitPosition[1] = new Vector3(0, 12.63f, 0);
        //InitPosition[2] = new Vector3(0, 14.6f, 0);

        InitPosition[0] = new Vector3(0, 6.65f, 0);
        InitPosition[1] = new Vector3(0, 22.9f, 0);
        InitPosition[2] = new Vector3(0, 0f, 0);

        IsAssetLoad(StartEnterRoutine);
        //EnterFunc();
        //UIManager.Inst.FullFadeOut();
        //videoPlayer.LoadVideoInfo();
        //videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.CAFEALONE1].GetVideoPath);

        //videoPlayer.SetFrame(10);
        //videoPlayer.Volume = 0;
        //videoPlayer.VideoPause();

        //UIManager.Inst.LoadNarrationClip("Cafe");
        //UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_CAF_03C_1"),
        //    "한적해서 작업하기 좋은 곳도 있지만, 사람이 북적여서 상대방의 이야기가\n 잘 들리지 않을 정도로 시끄러운 곳도 있기 마련이에요.\n" +
        //    "다양한 분위기의 카페에서 15분간 지속수행훈련을 해 볼까요?",
        //    true,
        //    delegate { ShowStartButton(); });

        //Debug.Log(System.DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.ff"));
    }


    public override void Level1Step()
    {
        base.Level1Step();

        GameObject Shpere = GameObject.Find("VR3D_BG");
        Quaternion qu = new Quaternion();
        qu.eulerAngles = InitPosition[0];
        Shpere.transform.localRotation = qu;

        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.CAFEALONE1].GetVideoPath);
        level1.InitFunc(this, LevelType.CAFE_LEVEL1, videoPlayer, 3);
        videoPlayer.Volume = 0.0f;
        Func<bool> func = videoPlayer.isPreparedFunc;
        UIManager.Inst.FullFadeIn(
            delegate
            {
                UIManager.Inst.CompleteAndCallback(func, delegate { videoPlayer.VideoLoopPlay(66.22f, 71.22f); },
                                                         delegate { UIManager.Inst.FullFadeOut(); },
                                                         delegate { UIManager.Inst.HalfFadeOut(); },
                                                         delegate { focusGame.SetActive(true); },
                                                         delegate { focusGame.ShowJoyStick(); },
                                                         delegate { focusGame.SetJoyStickPosition(0, 140, 0); },
                                                         delegate { UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_CAF_05C_L1"), "", false, delegate { focusGame.HideJoyStick(); }); },
                                                         //delegate { UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n테블릿 화면 위에 조이스틱 버튼을 누르세요", 0, -80.0f); },
                                                         delegate
                                                         {
                                                             UIManager.Inst.ShowNarrationText("It's a weekday morning, so the café is empty.\n" +
"It's quiet because there aren't many customers, so it's nice to study.\n" +
"Now, shall we go to the staff, order a drink, and go sit in a corner?", 0, -10.0f);
                                                         },
                                                         delegate { level1.ShowStartButton(); });
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
        base.Level2Step();
        GameObject Shpere = GameObject.Find("VR3D_BG");
        Quaternion qu = new Quaternion();
        qu.eulerAngles = InitPosition[1];
        Shpere.transform.localRotation = qu;

        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.CAFEALONE2].GetVideoPath);
        level2.InitFunc(this, LevelType.CAFE_LEVEL2, videoPlayer, 3);
        videoPlayer.Volume = 0.0f;
        Func<bool> func = videoPlayer.isPreparedFunc;
        UIManager.Inst.FullFadeIn(
            delegate
            {
                UIManager.Inst.CompleteAndCallback(func, delegate { videoPlayer.VideoLoopPlay(690.08f, 695.08f); },
                                                         delegate { UIManager.Inst.FullFadeOut(); },
                                                         delegate { UIManager.Inst.HalfFadeOut(); },
                                                         delegate { focusGame.SetActive(true); },
                                                         delegate { focusGame.ShowJoyStick(); },
                                                         delegate { focusGame.SetJoyStickPosition(0, 140, 0); },
                                                         delegate { UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_CAF_05C_L2"), "", false, delegate { focusGame.HideJoyStick(); }); },
                                                         //delegate { UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n테블릿 화면 위에 조이스틱 버튼을 누르세요", 0, -80f); },
                                                         delegate
                                                         {
                                                             UIManager.Inst.ShowNarrationText("It's a weekday afternoon and there are quite a few people.\n" +
"There are no seats inside, so I will have to sit near the window next to the door.\n" +
"Although my attention may be distracted by the movement of people, I decide to focus and train.\n" +
"Then, shall we place our order and sit by the window next to the door?", 0, -10f);
                                                         },
                                                         delegate { level2.ShowStartButton(); });
            }
            );
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
        GameObject Shpere = GameObject.Find("VR3D_BG");
        Quaternion qu = new Quaternion();
        qu.eulerAngles = InitPosition[2];
        Shpere.transform.localRotation = qu;
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);
        //videoPlayer.SetFrame(0);
        videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.CAFETOGATHER].GetVideoPath);
        level3.InitFunc(this, LevelType.CAFE_LEVEL3, videoPlayer, 3);
        videoPlayer.Volume = 0.0f;
        Func<bool> func = videoPlayer.isPreparedFunc;
        UIManager.Inst.FullFadeIn(
            delegate
            {
                UIManager.Inst.CompleteAndCallback(func, delegate { videoPlayer.VideoLoopPlay(62.14f, 67.14f); },                                               
                                                         delegate { UIManager.Inst.FullFadeOut(); },
                                                         delegate { UIManager.Inst.HalfFadeOut(); },
                                                         delegate { focusGame.SetActive(true); },
                                                         delegate { focusGame.ShowJoyStick(); },
                                                         delegate { focusGame.SetJoyStickPosition(0, 140, 0); },
                                                         delegate { UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_CAF_05C_L3"), "", false, delegate { focusGame.HideJoyStick(); }); },
                                                         //delegate { UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n테블릿 화면 위에 조이스틱 버튼을 누르세요", 0, -80f); },
                                                         delegate
                                                         {
                                                             UIManager.Inst.ShowNarrationText("I came to a cafe to study with my friend on a weekend afternoon.\n" +
"There are a lot of people. Fortunately, the seat in front of the checkout counter was empty, so I could sit there.\n" +
"This is a location where noise is a concern, but let’s focus on training!\n" +
"Now, shall we place our order and sit in the empty seat right in front of us? ", 0, -10f);
                                                         },
                                                         delegate { level3.ShowStartButton(); });
            }
            );
    }

    public override void VideoLoopPlay(long start, long end)
    {
        videoPlayer.VideoLoopPlay(start, end);
    }
    public override void VideoPlay()
    {
        videoPlayer.VideoPlay();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
