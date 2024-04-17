using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Datas{

    public List<FocusData> FocusList = new List<FocusData>();
    public List<AOI> AOIList = new List<AOI>();
    public List<VasData> VasList = new List<VasData>();
    public List<EEG> EEGList = new List<EEG>();
    
    public void DatasClear()
    {
        /* ======================== DatasClear ======================== */
        FocusList.Clear();
        AOIList.Clear();
        VasList.Clear();
        for(int i = 0; i < EEGList.Count; i++)
            EEGList[i].Clear();
    }
}

[Serializable]
public class EEG
{
    public string LevelID = "";
    public LevelType LevelType = LevelType.None;
    public List<EEGValue> valueList = new List<EEGValue>();

    public EEG()
    {
        valueList = new List<EEGValue>();
    }

    public EEG(string _LevelID, LevelType _LevelType)
    {
        LevelID = _LevelID;
        LevelType = _LevelType;
        valueList = new List<EEGValue>();
    }
    public void AddValue(float val, float elapseTime)
    {
        valueList.Add(new EEGValue(val, elapseTime));
    }
    public void Clear()
    {
        LevelID = "";
        LevelType = LevelType.None;
        valueList.Clear();
    }
}

[Serializable]
public struct EEGValue
{
    public float elapseTime;
    public float value;
    public EEGValue(float val, float _elapseTime)
    {
        elapseTime = _elapseTime;
        value = val;
    }
}

[Serializable]
public class FocusData
{
    /// <summary>
    /// 게임 시작 시간
    /// </summary>
    public string Id = "";
    public LevelType Type = LevelType.None;
    public string StartTime = "";    
    public List<FocusValue> FocusValueList = new List<FocusValue>();

    public void FocusClear()
    {
        Id = "";
        Type = LevelType.None;
        StartTime = "";
        FocusValueList.Clear();
    }

    public FocusData(string id, LevelType type)
    {
        Id = id;
        Type = type;
        StartTime = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
    }

    public void ValueAdd(float cr, float resp, FocusState state, float eTime, float rt)
    {
        FocusValueList.Add(new FocusValue(cr, resp, state, eTime, rt));
    }
}

[Serializable]
public struct FocusValue
{
    public float ACC;
    public float CRESP;
    public float RESP;
    public float reactionTime;
    public float elapseTime;

    public FocusState State;

    /// <summary>
    /// 포커스 데이터 입력
    /// </summary>
    /// <param name="cr">CRESP 정해진 정답 1 : target이 아닌 자극이 나와서 반응해야 맞는 자극</param>
    /// <param name="resp">RESP 반응여부</param>
    /// <param name="state"></param>
    /// <param name="eTime">경과시간</param>
    /// <param name="rt">reaction Time 반응시간</param>
    public FocusValue(float cr, float resp, FocusState state, float eTime, float rt)
    {        
        State = state;
        elapseTime = eTime;
        reactionTime = rt;
        CRESP = cr;
        RESP = resp;
        if (CRESP == 1 && RESP == 1)
            ACC = 1;
        else
            ACC = 0;
    }

}

[Serializable]
public class AOI
{
    public string Id = "";
    public LevelType Type = LevelType.None;
    public string StartTime;
    public List<AOIValue> valueList;

    public void AOIClear()
    {
        Id = "";
        Type = LevelType.None;
        StartTime = "";
        valueList.Clear();
    }
    public AOI(string id, LevelType type, string time)
    {
        Id = id;
        Type = type;
        StartTime = time;
        valueList = new List<AOIValue>();
    }
}

[Serializable]
public struct AOIValue
{
    public AOIType AOIType;
    public float timeSpan;
    public float endTime;
    public float EnterTime;
    public string StartTime;
    
    public AOIValue(AOIType AOI_Type, float LookingTime, string startTime, float totalTime)
    {
        AOIType = AOI_Type;
        timeSpan = LookingTime;
        StartTime = startTime;
        endTime = totalTime;
        EnterTime = (endTime - timeSpan) < 0 ? 0 : (endTime - timeSpan);
    }
}

[Serializable] 
public class VasData
{
    public string Id = "";
    public LevelType Type = LevelType.None;
    public string StartTime = "";
    public List<VasValue> VasValueList = new List<VasValue>();

    public void VasDataClear()
    {
        Id = "";
        Type = LevelType.None;
        StartTime = "";
        VasValueList.Clear();
    }
    public VasData(string id, LevelType type, string startTime)
    {
        Id = id;
        Type = type;
        StartTime = startTime;
        VasValueList = new List<VasValue>();
    }
}

[Serializable] 
public struct VasValue
{
    public float value;
    public string time;

    public VasValue(float Value, string Time)
    {
        value = Value;
        time = Time;
    }
}

public struct AOIGraphValue
{
    public float enterTime;
    public float endTime;
    public float percent;

    public AOIGraphValue(float sTime, float eTime, float per)
    {
        enterTime = sTime;
        endTime = eTime;
        percent = per;
    }
}

public struct AnswerGraphValue
{
    public float enterTime;    
    public float percent;

    public AnswerGraphValue(float sTime, float per)
    {
        enterTime = sTime;        
        percent = per;
    }
}

