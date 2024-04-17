using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBase : MonoBehaviour {
    
    /// <summary>
    /// 레벨 Enum
    /// </summary>
    protected LevelType Level;
    /// <summary>
    /// 레벨별 VideoPath
    /// </summary>
    protected string VideoPath;
    /// <summary>
    /// 해당 레벨 완료 하였는가
    /// </summary>
    protected bool isClear = false;
    /// <summary>
    /// Level 시작 유무
    /// </summary>
    protected bool isStarted = false;
    /// <summary>
    /// Level 훈련시작부터 총 시간 체크
    /// </summary>
    protected float TotalTime = 0.0f;

    protected SceneBase SceneManager;

    protected VideoController videoPlayer;


    protected CanvasGroup StartTraining;

    protected ButtonFunc StartTrainingButton;

    public virtual void InitFunc(SceneBase sceneBase = null, LevelType level = LevelType.None, VideoController player = null, int LevelIndex = 0)
    {
        Debug.Log("class Name : " + this.GetType().Name);
        DataManager.Inst.CurrentLevel = this.GetType().Name;

        DataManager.Inst.CurrentLevelType = level;
        SceneManager = sceneBase;
        Level = level;
        videoPlayer = player;
        DataManager.Inst.SceneLevel = LevelIndex;

        switch(level)
        {
            case LevelType.ROOM_LEVEL1:
            case LevelType.LIBRARY_LEVEL1:
            case LevelType.CAFE_LEVEL1:
            case LevelType.STREET_LEVEL1:
                DataManager.Inst.MissionLevel = 1;
                break;
            case LevelType.ROOM_LEVEL2:
            case LevelType.LIBRARY_LEVEL2:
            case LevelType.CAFE_LEVEL2:
            case LevelType.STREET_LEVEL2:
                DataManager.Inst.MissionLevel = 2;
                break;
            case LevelType.ROOM_LEVEL3:
            case LevelType.LIBRARY_LEVEL3:
            case LevelType.CAFE_LEVEL3:
            case LevelType.STREET_LEVEL3:
                DataManager.Inst.MissionLevel = 3;
                break;
        }
        SceneManager.focusGame.SetArrowActive(false);
        StartTrainingButton.AddPressedListener(ShowJoyStickImg);
        StartTrainingButton.AddClickSoundActionListener(SceneManager.ButtonClickSound);
        StartTrainingButton.AddEnterActionListener(SceneManager.EnterPoint);
        StartTrainingButton.AddExitActionListener(SceneManager.ExitPoint);
    }
    
    public virtual void ShowJoyStickImg()
    {
        UIManager.Inst.HideNarrationText();
        UIManager.Inst.SkipNarration();
        SceneManager.focusGame.HideJoyStick();
        SceneManager.focusGame.InitGame(DataManager.Inst.SceneLevel);
        UIManager.Inst.HideAndInteractableCanvasGroup(StartTraining);

        SceneManager.ShowCountDown();
        UIManager.Inst.CompleteAndCallback(SceneManager.focusGame.CountDownEnd,
         SceneManager.StopLoopPlayer,
         SceneManager.VideoPlay,
         SceneManager.FocusGameStart);

        // 나레이션 나오는 도중에         
        //UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_RLCS_L123_Q"), "", false,
        //    delegate { UIManager.Inst.HideNarrationText(); },
        //    delegate { SceneManager.focusGame.StopBorder(); },
        //    delegate { SceneManager.ShowCountDown(); },
        //    delegate {
        //        UIManager.Inst.CompleteAndCallback(SceneManager.focusGame.CountDownEnd,
        // SceneManager.StopLoopPlayer,
        // SceneManager.VideoPlay,
        // SceneManager.FocusGameStart);
        //    }
        //    );
        //// 정답 도형 보여줌
        //UIManager.Inst.WaitSeconds(1.5f, delegate { SceneManager.Step3(); },
        //                                 delegate { SceneManager.focusGame.ShowBorder(); }
        //);
    }

    public virtual void ShowStartButton()
    {
        Debug.Log("ShowSchoolStartButton");
        //UIManager.Inst.SkipNarration();
        UIManager.Inst.ShowAndInteractableCanvasGroup(StartTraining);
        //UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_CAF_05C_L2"), "", false,
        //    delegate { }
        //);
    }
}
