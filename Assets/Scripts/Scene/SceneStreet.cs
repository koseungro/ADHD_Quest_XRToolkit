using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SceneStreet : SceneBase {

    protected override void Awake()
    {
        base.Awake();
        videoPlayer = GetComponent<VideoController>();
        //youtubePlayer = GameObject.Find("VR3D_BG/Shpere/VR3D_Player_Pre").GetComponent<VideoPlayer_KKC>();
    }

    protected override void EnterFunc()
    {
        if (SceneLoader.Inst != null)
        {
            if (SceneLoader.Inst.IsLoadingTextActive())
                SceneLoader.Inst.SetLoadingTextActive(false);
        }
        UIManager.Inst.FullFadeOut();

        videoPlayer.SetFrame(10);
        videoPlayer.Volume = 0;
        videoPlayer.VideoPause();

        UIManager.Inst.LoadNarrationClip("Street");
        UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("ad_03S_1"),
            "You chose to train on the streets!\n"+
"<color=#35AC88>It's not easy to concentrate when people are coming and going.\n" +
"If you can maintain concentration even in this environment, there will be no problem anytime, anywhere.\n"+
"Now then, shall we calm down and do N - back training for 2 minutes?\n</color>" +
  "Not sure what N - back training is?\n"+
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
        UIManager.Inst.PlayNarration(UIManager.Inst.FindNarration("ad_03S_2"));
        //delegate { PracticNextNarration(); });

        UIManager.Inst.ShowNarrationText("You are wondering what N-back training is!\n"+
"The positions and colors of the colored points inside the square divided into 16 parts will appear on the screen.\n"+
"There are no rules as to the order in which they appear.", 0, 138f, 0);

        // 7초 뒤 랜덤 도형이 나와서 움직임.
        //UIManager.Inst.WaitSeconds(7.3f, delegate { focusGame.ShowFigure(); });
        
        UIManager.Inst.WaitSeconds(14.7f, delegate { UIManager.Inst.ShowNarrationText("However, take a close look at where the points appear as shown now.\nWhen you reach the same location as before the second time, you need to press the trigger button."); });
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
        // 정답 도형 보여줌
        //UIManager.Inst.WaitSeconds(13.5f, delegate { focusGame.StopFigure(); },
        //                                  delegate {
        //                                      focusGame.ShowJoyStick();
        //                                      focusGame.ShowAnswerFigure();
        //                                  });
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
        //                       UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("ad_03S_3"), "", false);
        //                   });
        //            }
        //        );
        //    }
        //);
    }

    protected override void IsAssetLoad(params UnityAction[] callback)
    {
        videoPlayer.LoadVideoInfo();
        videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[0].GetVideoPath);
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

        for (int i = 0; i < callback.Length; i++)
        {
            callback[i].Invoke();
        }
    }
    // Use this for initialization
    protected override void Start () {        
        base.Start();
        //CheckPurchase();
        IsAssetLoad(EnterFunc);

    }
	
    public override void Level1Step()
    {
        
        base.Level1Step();        
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);        
        videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.PARK].GetVideoPath);
        //SetURLID(LevelType.STREET_LEVEL1, YoutubeResolution.HD720);
        level1.InitFunc(this, LevelType.STREET_LEVEL1, videoPlayer, 2);
        //level1.InitFunc(youtubePlayer, this, LevelType.STREET_LEVEL1,  2);
        videoPlayer.Volume = 0.0f;
        //youtubePlayer.SetVolume = 0.0f;
        Func<bool> func = videoPlayer.isPreparedFunc;
        //Func<bool> func = youtubePlayer.isPreparedFunc;
        UIManager.Inst.FullFadeIn(
            delegate { UIManager.Inst.CompleteAndCallback(func, delegate { UIManager.Inst.FullFadeOut(); },
                                                                delegate { UIManager.Inst.HalfFadeOut(); },
                                                                delegate { videoPlayer.VideoLoopPlay(95.15f, 100.15f); },
                                                                //delegate { youtubePlayer.VideoLoopPlay(95.15f, 100.15f); },
                                                                delegate { focusGame.SetActive(true); },
                                                                delegate { focusGame.ShowJoyStick(); },
                                                                delegate { focusGame.SetJoyStickPosition(0, 190, 0); },
                                                                delegate { UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_STR_05S_L1"), "", false, delegate { focusGame.HideJoyStick(); }); },
                                                                //delegate { UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n컨트롤러 버튼을 누르세요", 0, 90.0f); },
                                                                delegate { UIManager.Inst.ShowNarrationText("I came to a quiet park. There are very few people going there.\n"+
"This is the optimal environment for quiet training!\n"+
"Now, shall we sit down on an empty bench and start training?", 0, 142.0f); },
                                                                delegate { level1.ShowStartButton(); });  
                     }
            );
        
    }

    public override void Level2Step()
    {
        //App Store Upload Version
        //if(!DataManager.Inst.GetIsPurchase)
        //{
        //    base.Level2Step();
        //    return;
        //}
        base.Level2Step();
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);        
        videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.SCHOOL].GetVideoPath);
        //SetURLID(LevelType.STREET_LEVEL2, YoutubeResolution.HD720);
        level2.InitFunc(this, LevelType.STREET_LEVEL2, videoPlayer, 2);
        //level2.InitFunc(youtubePlayer, this, LevelType.STREET_LEVEL2,  2);
        videoPlayer.Volume = 0.0f;
        Func<bool> func = videoPlayer.isPreparedFunc;
        //Func<bool> func = youtubePlayer.isPreparedFunc;
        UIManager.Inst.FullFadeIn(
            delegate {
                UIManager.Inst.CompleteAndCallback(func, delegate { UIManager.Inst.FullFadeOut(); },
                                                         delegate { UIManager.Inst.HalfFadeOut(); },
                                                         delegate { videoPlayer.VideoLoopPlay(784.27f, 789.27f); },
                                                         //delegate { youtubePlayer.VideoLoopPlay(784.27f, 789.27f); },
                                                         delegate { focusGame.SetActive(true); },
                                                         delegate { focusGame.ShowJoyStick(); },
                                                         delegate { focusGame.SetJoyStickPosition(0, 190, 0); },
                                                         delegate { UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_STR_05S_L2"), "", false, delegate { focusGame.HideJoyStick(); }); },
                                                         //delegate { UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n컨트롤러 버튼을 누르세요", 0, 90.0f); },
                                                         delegate { UIManager.Inst.ShowNarrationText("There are a lot of students going there.\n"+
"It may be a little noisy because it's school time, but it would be nice to practice while breathing the fresh morning air.\n"+
"So, shall we stop for a moment and start by sitting on an empty bench across from the school?", 0, 142.0f); },
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
        UIManager.Inst.HideAndInteractableCanvasGroup(SelectLevel);        
        videoPlayer.SetVideoURL(videoPlayer.videoInfoArray[(int)VideoType.TOURIST].GetVideoPath);
        //SetURLID(LevelType.STREET_LEVEL3, YoutubeResolution.HD720);
        level3.InitFunc(this, LevelType.STREET_LEVEL3, videoPlayer, 2);
        //level3.InitFunc(youtubePlayer, this, LevelType.STREET_LEVEL3,  2);
        videoPlayer.Volume = 0.0f;
        Func<bool> func = videoPlayer.isPreparedFunc;
        //Func<bool> func = youtubePlayer.isPreparedFunc;
        UIManager.Inst.FullFadeIn(
            delegate
            {
                UIManager.Inst.CompleteAndCallback(func, delegate { UIManager.Inst.FullFadeOut(); },
                                                         delegate { UIManager.Inst.HalfFadeOut(); },
                                                         delegate { videoPlayer.VideoLoopPlay(38.16f, 43.16f); },
                                                         //delegate { youtubePlayer.VideoLoopPlay(38.16f, 43.16f); },
                                                         delegate { focusGame.SetActive(true); },
                                                         delegate { focusGame.ShowJoyStick(); },
                                                         delegate { focusGame.SetJoyStickPosition(0, 190, 0); },
                                                         delegate { UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_STR_05S_L3"), "", false, delegate { focusGame.HideJoyStick(); }); },
                                                         //delegate { UIManager.Inst.ShowNarrationText("처음보여지는 도형을 잘 기억하시고, 다른 도형이 나타날 때 \n컨트롤러 버튼을 누르세요", 0, 90.0f); },
                                                         delegate { UIManager.Inst.ShowNarrationText("I went out to see the tourist attractions with my friend.\n" +
                                                             "Ah! My friend said he had something to exchange at the store in front of me and asked me to wait for a moment.\n"+
"It might be a bit noisy, but it might be a good idea to do some training while waiting for your friend\n"+
"Now, shall we concentrate for a moment while our friend comes?", 0, 142.0f); },
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
        //youtubePlayer.PlayYoutubeVideo();
    }

	// Update is called once per frame
	protected override void Update () {

        base.Update();        
    }
}
