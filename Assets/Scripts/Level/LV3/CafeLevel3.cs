﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CafeLevel3 : LevelBase
{
    public override void InitFunc(SceneBase sceneBase = null, LevelType level = LevelType.None, VideoController player = null, int LevelIndex = 0)
    {
        StartTraining = transform.Find("StartTraining").GetComponent<CanvasGroup>();
        StartTrainingButton = StartTraining.transform.Find("StartTrainingButton").GetComponent<ButtonFunc>();
        UIManager.Inst.LoadButtonData("StartTraining", string.Format("SceneCafeManager/{0}/", gameObject.name));
        UIManager.Inst.SettingButtonData("StartTraining", StartTrainingButton);
        DataManager.Inst.SceneName = "카페에서 - 친구와 함께";
        base.InitFunc(sceneBase, level, player, LevelIndex);
    }
}
