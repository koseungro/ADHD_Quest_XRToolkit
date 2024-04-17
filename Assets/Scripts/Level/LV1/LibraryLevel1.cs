using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryLevel1 : LevelBase {

    

    public override void InitFunc(SceneBase sceneBase = null, LevelType level = LevelType.None, VideoController player = null, int LevelIndex = 0)
    {
        StartTraining = transform.Find("StartTraining").GetComponent<CanvasGroup>();

        StartTrainingButton = StartTraining.transform.Find("StartTrainingButton").GetComponent<ButtonFunc>();

        UIManager.Inst.LoadButtonData("StartTraining", string.Format("SceneLibraryManager/{0}/", gameObject.name));
        DataManager.Inst.SceneName = "도서관에서 - 칸막이 열람실 자리";
        UIManager.Inst.SettingButtonData("StartTraining", StartTrainingButton);

        base.InitFunc(sceneBase, level, player, LevelIndex);

    }

    public override void ShowJoyStickImg()
    {
        UIManager.Inst.HideNarrationText();
        UIManager.Inst.HalfFadeOut();
        UIManager.Inst.SkipNarration();
        SceneManager.focusGame.HideJoyStick();
        SceneManager.focusGame.InitGame(DataManager.Inst.SceneLevel);
        UIManager.Inst.HideAndInteractableCanvasGroup(StartTraining);

        SceneManager.ShowCountDown();
        UIManager.Inst.CompleteAndCallback(SceneManager.focusGame.CountDownEnd,
            // Level 1은 반복 방해음이 쭉 이어가지 않기 때문에 스탑
            EnvironmentSound.Inst.AllLoopStop,
            SceneManager.FocusGameStart);

        //// 나레이션 나오는 도중에         
        //UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_RLCS_L123_Q"), "", false,
        //    delegate { UIManager.Inst.HideNarrationText(); },
        //    delegate { SceneManager.focusGame.StopBorder(); },
        //    delegate { SceneManager.ShowCountDown(); },            
        //    delegate {
        //        UIManager.Inst.CompleteAndCallback(SceneManager.focusGame.CountDownEnd,
        //    // Level 1은 반복 방해음이 쭉 이어가지 않기 때문에 스탑
        //    EnvironmentSound.Inst.AllLoopStop,
        //    SceneManager.FocusGameStart);
        //    }
        //    );
        //// 정답 도형 보여줌
        //UIManager.Inst.WaitSeconds(1.5f, delegate { SceneManager.Step3(4); },
        //                                 delegate { SceneManager.focusGame.ShowBorder(); }
        //);
    }
}
