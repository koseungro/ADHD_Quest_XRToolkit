using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLevel3 : LevelBase {

	// Use this for initialization
	void Start () {
		
	}

    public override void InitFunc(SceneBase sceneBase = null, LevelType level = LevelType.None, VideoController player = null, int LevelIndex = 0)
    {
        StartTraining = transform.Find("StartTraining").GetComponent<CanvasGroup>();

        StartTrainingButton = StartTraining.transform.Find("StartTrainingButton").GetComponent<ButtonFunc>();

        UIManager.Inst.LoadButtonData("StartTraining", string.Format("SceneRoomManager/{0}/", gameObject.name));
        DataManager.Inst.SceneName = "방 안에서 - 심한 층간소음";
        UIManager.Inst.SettingButtonData("StartTraining", StartTrainingButton);

        base.InitFunc(sceneBase, level, player);
    }

    public override void ShowJoyStickImg()
    {
        UIManager.Inst.HideNarrationText();
        UIManager.Inst.HalfFadeOut();
        UIManager.Inst.SkipNarration();
        SceneManager.focusGame.HideJoyStick();
        SceneManager.focusGame.InitGame(0);
        UIManager.Inst.HideAndInteractableCanvasGroup(StartTraining);

        SceneManager.ShowCountDown();
        UIManager.Inst.CompleteAndCallback(SceneManager.focusGame.CountDownEnd,
         SceneManager.FocusGameStart);

        // 나레이션 나오는 도중에         
        //UIManager.Inst.WaitForNarrationAndFunc(UIManager.Inst.FindNarration("AD_RLCS_L123_Q"), "", false,
        //    delegate { UIManager.Inst.HideNarrationText(); },
        //    delegate { SceneManager.focusGame.StopBorder(); },
        //    delegate { SceneManager.ShowCountDown(); },            
        //    delegate {
        //        UIManager.Inst.CompleteAndCallback(SceneManager.focusGame.CountDownEnd,
        // SceneManager.FocusGameStart);
        //    }
        //    );
        //// 정답 도형 보여줌
        //UIManager.Inst.WaitSeconds(1.5f, delegate { SceneManager.Step3(3); },
        //                                 delegate { SceneManager.focusGame.ShowBorder(); }
        //);
    }

}
