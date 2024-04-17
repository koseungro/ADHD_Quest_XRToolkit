using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager> {

    public static int Static_0Sec = 0;
    public static int Static_300Sec = 300;
    public static int Static_600Sec = 600;
    public static int Static_900Sec = 990;

    public Datas data = new Datas();
    public PlayerData Player = new PlayerData();
    public int SceneLevel = 0;
    /* ================================ Network Value ================================ */
    /// <summary>
    /// 화면 동기화 유무
    /// </summary>
    public bool isViewScync = true;
    public string SceneName = "";
    public int MissionLevel = 0;
    /// <summary>
    /// 현재 진행 중인 미션의 Preogress Value
    /// </summary>
    public float MissionProgressValue
    {
        get { return ((TotalTime / 900.0f) * 100.0f) >= 100 ? 100.0f : (TotalTime / 900.0f) * 100.0f; }
    }    
    /* ================================ Network Value ================================ */
    public string CurrentLevel = "";
    public LevelType CurrentLevelType = LevelType.None;
    public string StartTime = "";
    public string NextSituation = "";
    public float TotalTime = 0.0f;
    public bool IsStart = false;
    public FocusData CurrentFocusData;
    public VasData CurrentVasData;    
    public AOI CurrentAOI;
    /// <summary>
    /// 2019.05.14 
    /// 동기화 유무 체크
    /// </summary>
    public bool isSync = false;
    /// <summary>
    /// 2019.05.15 
    /// 현재 디바이스가 서버인지 체크
    /// </summary>
    public bool isServer = false;

    /// <summary>
    /// 2019.06.24
    /// 서버 연결을 패스할지 체크
    /// </summary>
    public bool isPassConnect = false;

    /// <summary>
    /// 2019.06.26
    /// 서버 Scene 로드가 완료 되었는지 체크
    /// </summary>
    public bool isServerLoaded = false;
    /// <summary>
    /// 2019.06.26
    /// 클라이언트 Scene 로드가 완료 되었는지 체크
    /// </summary>
    public bool isSClientLoaded = false;
    /// <summary>
    /// 2019.08.28
    /// 일시정지인지 체크
    /// </summary>
    public bool isPause = false;

    // 구매 여부 확인
    private bool IsPurchase = false;

    public bool GetIsPurchase{ get { return IsPurchase; } }
    public bool SetIsPurchase { set { IsPurchase = value; } }

    // App Store Upload Version
    public SaveCSV saveCSV = new SaveCSV();

    // 그래프 만들기 위한 변수
    float Count = 0;
    float TotalCount = 0;
    float AverageValue = 0;

    /// <summary>
    /// 지시점 프리팹
    /// </summary>
    public DirectionEffect DirectionEffectPrefab;

    private double TotalElapsedTime = 120000;
    public double GetTotalElapsedTime { get { return TotalElapsedTime; } }
    public DateTime StartDateTime;
    public double ElpasedDateTime;
    public double GetElapsedTime
    {
        get { return (DateTime.Now - StartDateTime).TotalMilliseconds; }
    }

    public float GetTotalTime
    {
        get { return TotalTime; }
    }

    public void SetTotalTime(float val)
    {
        TotalTime = val;
    }
    public void SetTimeStart(bool val)
    {
        IsStart = val;
    }
    public VasData GetSetVasData
    {
        get { return CurrentVasData; }
        set { CurrentVasData = value; }
    }
    public FocusData GetSetFocusData
    {
        get { return CurrentFocusData; }
        set { CurrentFocusData = value; }
    }
    public AOI GetSetAOI
    {
        get { return CurrentAOI; }
        set { CurrentAOI = value; }
    }

    /// <summary>
    /// Client에 저장하고 있을 vas값
    /// </summary>
    public float SendVasValue = 0.0f;
    /// <summary>
    /// 결과보기::FocusGame 점수
    /// </summary>
    public float FocusScore = 0.0f;
    /// <summary>
    /// 결과보기::시선 점수
    /// </summary>
    public float AOIScore = 0.0f;
    /// <summary>
    /// 결과보기::RTV 점수
    /// </summary>
    public float RTV = 0.0f;
    /// <summary>
    /// 결과보기::CV 점수
    /// </summary>
    public float CV = 0.0f;
    /// <summary>
    /// 결과보기::meanTBR 값
    /// </summary>
    public float meanTBR = 0.0f;

    /// <summary>
    /// 그래프 보기::정답개수 리스트
    /// </summary>
    public List<float> AnswerCountList;
    /// <summary>
    /// 그래프 보기::시선집중 리스트
    /// </summary>
    public List<float> AOICountList;
    /// <summary>
    /// 그래프 보기::EEG 비율 리스트
    /// </summary>
    public List<float> EEGRatioList;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        LoadCommonPrefabs();
    }

    // Use this for initialization
    void Start () {

        //AddAOI("Test", DateTime.Now);
	}

    /// <summary>
    /// 2019.08.21
    /// 결과보기, 그래프보기 데이터 초기화
    /// </summary>
    public void InitValues()
    {
        FocusScore = 0.0f;
        AOIScore = 0.0f;
        RTV = 0.0f;
        CV = 0.0f;
        meanTBR = 0.0f;
        AnswerCountList.Clear();
        AOICountList.Clear();
        EEGRatioList.Clear();
    }

    /// <summary>
    /// 2019.06.24
    /// 공통 프리팹 로드
    /// </summary>
    private void LoadCommonPrefabs()
    {
        DirectionEffectPrefab = Resources.Load<DirectionEffect>("Prefabs/Effect/OrderEffect");
    }

    public void SetStartDateTime()
    {
        StartDateTime = DateTime.Now;
    }
    
    public void DataManagerClear()
    {
        data.DatasClear();
        //Player.PlayerDataClear();
        CurrentLevel = "";
        CurrentLevelType = LevelType.None;
        StartTime = "";
        NextSituation = "";
        CurrentFocusData.FocusClear();
        CurrentVasData.VasDataClear();
        CurrentAOI.AOIClear();
    }

    public FocusData FindFocusData(LevelType id)
    {
        FocusData find = CurrentFocusData;//.Find(x => x.Type == id && x.StartTime.Equals(StartTime));
        return find;
    }
    public FocusData FindFocusData(LevelType id, string findTime)
    {
        FocusData find = data.FocusList.Find(x => x.Type == id && x.StartTime.Equals(findTime));
        return find;
    }


    public void AddFocusData(FocusData value)
    {
        data.FocusList.Add(value);
    }
    public void AddFocusValue(FocusValue value)
    {
        CurrentFocusData.FocusValueList.Add(value);
        //FocusData find = data.FocusList.Find(x => x.Id == id && x.StartTime.Equals(StartTime));
        //if(find != null)
        //{
        //    find.FocusValueList.Add(value);
        //}
    }
    public void AddFocusValue(LevelType id, string findTime, FocusValue value)
    {
        FocusData find = data.FocusList.Find(x => x.Type == id && x.StartTime.Equals(findTime));
        if (find != null)
        {
            find.FocusValueList.Add(value);
        }
    }

    public AOI FindAOI(LevelType id)
    {
        AOI find = CurrentAOI;// data.AOIList.Find(x => x.Type == id && x.StartTime.Equals(StartTime));

        if (find != null)
        {
            return find;
        }

        return find;
    }
    public AOI FindAOI(LevelType id, string findTime)
    {
        AOI find = data.AOIList.Find(x => x.Type == id && x.StartTime.Equals(findTime));

        if (find != null)
        {
            return find;
        }

        return find;
    }

    public void AddAOI(string id, string findTime)
    {        
        AOI find = data.AOIList.Find(x => x.Type == CurrentLevelType && x.StartTime.Equals(findTime));
        if(find != null)
            data.AOIList.Add(new AOI(id, CurrentLevelType, findTime));
    }
    public void AddAOI(AOI aoi)
    {
        data.AOIList.Add(aoi);
        //AOI find = data.AOIList.Find(x => x.Id == id && x.StartTime.Equals(findTime));
        //if (find != null)
        //    data.AOIList.Add(new AOI(id, findTime));
    }
    public void AddAOI(string id)
    {
        AOI find = data.AOIList.Find(x => x.Type == CurrentLevelType && x.StartTime.Equals(StartTime));
        if (find != null)
            data.AOIList.Add(new AOI(id, CurrentLevelType, StartTime));
    }

    public void AddAOIValue(string id, AOIType type, float Lookingtime, string startTime, float totalTime)
    {
        CurrentAOI.valueList.Add(new AOIValue(type, Lookingtime, startTime, totalTime));
        //AOI find = data.AOIList.Find(x => x.Id == id && x.StartTime.Equals(StartTime));
        //if (find != null)
        //{
        //    find.valueList.Add(new AOI_Value(type, Lookingtime, startTime));
        //}
    }
    public void AddAOIValue(LevelType id, string findTime, AOIType type, float Lookingtime, string startTime, float totalTime)
    {
        AOI find = data.AOIList.Find(x => x.Type == id && x.StartTime.Equals(findTime));
        if (find != null)
        {
            find.valueList.Add(new AOIValue(type, Lookingtime, startTime, totalTime));
        }
    }

    public VasData FindVasData(LevelType id)
    {
        VasData find = CurrentVasData;// data.VasList.Find(x => x.Type == id && x.StartTime.Equals(StartTime));
        return find;
    }
    public VasData FindVasData(LevelType id, string findTime)
    {
        VasData find = data.VasList.Find(x => x.Type == id && x.StartTime.Equals(findTime));
        return find;
    }
    public void AddVasData(VasData VasData)
    {        
        data.VasList.Add(VasData);
    }
    public void AddVasData(string id, string startTime)
    {        
        VasData find = data.VasList.Find(x => x.Type == CurrentLevelType && x.StartTime.Equals(startTime));

        if (find == null)
            data.VasList.Add(new VasData(id, CurrentLevelType, startTime));
    }
    public void AddVasData(string id)
    {
        VasData find = CurrentVasData;// data.VasList.Find(x => x.Type == CurrentLevelType && x.StartTime.Equals(StartTime));

        if (find == null)
            data.VasList.Add(new VasData(id, CurrentLevelType, StartTime));
    }
    public void AddVasDataValue(float val, string time)
    {
        CurrentVasData.VasValueList.Add(new VasValue(val, time));
    }
    public static string NowToString(int index = 0)
    {
        string date = "";
        if(index == 0)
        {
            date = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
        }
        else
        {
            date = DateTime.Now.ToString("yyyy-MM-dd");
        }
        return date;
    }
    public static string NowTimeToString()
    {
        string date = "";
        
        date = DateTime.Now.ToString("HH-mm-ss");
                
        return date;
    }
    public static string NowTimeToString2()
    {
        string date = "";

        date = DateTime.Now.ToString("HH-mm");

        return date;
    }
    public float GetVasValue(int index)    
    {
        float val = 0.0f;
        val = CurrentVasData.VasValueList[index].value * 100.0f;
        return val;
    }

    public int GetAnswerCount(LevelType id)
    {
        int result = 0;
        FocusData find = FindFocusData(id);
        result = find.FocusValueList.FindAll(x => x.State == FocusState.RIGHT).Count;

        return result;
    }

    /// <summary>
    /// 구간별 정답카운트를 리스트로 반환
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public List<float> GetAnswerCountList(LevelType id)
    {
        List<float> list = new List<float>();

        calculateAnswerGraphValue(id, ref list, Static_0Sec, Static_300Sec);
        calculateAnswerGraphValue(id, ref list, Static_300Sec, Static_600Sec);
        calculateAnswerGraphValue(id, ref list, Static_600Sec, Static_900Sec);

        return list;
    }

    private void calculateAnswerGraphValue(LevelType id, ref List<float> list, float min, float max)
    {
        List<FocusValue> find = FindFocusData(id).FocusValueList;

        for (int i = 0; i < find.Count; i++)
        {            
            if (find[i].elapseTime < max && find[i].elapseTime > min)
            {
                TotalCount++;
                if (find[i].State == FocusState.RIGHT)
                {
                    Count++;                    
                }                
            }
        }        
        if(TotalCount == 0)
        {
            list.Add(0);
        }
        else
        {
            AverageValue = (Count / TotalCount) * 100;
            list.Add(AverageValue);
        }
        Debug.Log("Count : " + Count);
        Debug.Log("TotalCount : " + TotalCount);
        Debug.Log("AverageValue : " + AverageValue);
        Count = 0;
        TotalCount = 0;
        AverageValue = 0;
    }

    public List<float> GetAOICountList(LevelType id)
    {
        List<float> list = new List<float>();

        calculateAOIGraphValue(id, ref list, Static_0Sec, Static_300Sec, Static_300Sec);
        calculateAOIGraphValue(id, ref list, Static_300Sec, Static_600Sec, Static_300Sec);
        calculateAOIGraphValue(id, ref list, Static_600Sec, Static_900Sec, Static_300Sec);
        
        return list;
    }

    private void calculateAOIGraphValue(LevelType id, ref List<float> list, float min, float max, float totalTime)
    {
        List<AOIValue> find = FindAOI(id).valueList;
        TotalCount = totalTime;        
        for (int i = 0; i < find.Count; i++)
        {
            if (find[i].AOIType == AOIType.AOI1)
            {
                float LookStart = find[i].EnterTime;
                float LookEnd = find[i].endTime;

                if(LookEnd < max)
                {
                    if (LookEnd < min)
                        continue;
                    else if(LookStart > min && LookEnd < max)
                    {
                        Count += LookEnd - LookStart;
                        continue;
                    }
                    else if(LookStart < min && LookEnd < max)
                    {
                        float delta = LookEnd - min;
                        Count += delta;
                        continue;
                    }
                    else
                        Count += LookEnd - LookStart;
                }
                else if(LookEnd > max)
                {
                    if (LookStart > max)
                        break;
                    else if(LookStart < min && LookEnd > max)
                    {
                        Count += max - min;
                        break;
                    }
                    else
                    {
                        Count += max - LookStart;
                        break;
                    }
                }
            }
        }
        AverageValue = (Count / TotalCount) * 100;
        AverageValue = AverageValue > 100 ? 100 : AverageValue;
        Debug.Log("Count : " + Count);
        Debug.Log("AverageValue : " + AverageValue);
        list.Add(AverageValue);
        Count = 0;
        AverageValue = 0;
        TotalCount = 0;
    }

    public float GetAOITime(LevelType id, AOIType type)
    {
        float time = 0.0f;
        AOI find = FindAOI(id);
        if(find != null)
        {
           List<AOIValue> list = find.valueList.FindAll(x => x.AOIType == type);
           for(int i = 0; i < list.Count; i++)
            {
                time += list[i].timeSpan;
            }
            return time;
        }
        return time;
    }

    /// <summary>
    /// 현재 포커스 List의 Count값 반환
    /// </summary>
    /// <returns></returns>
    public int FocusListCount()
    {
        int cnt = 0;
        if(data.FocusList != null)
        {
            cnt = CurrentFocusData.FocusValueList.Count;
        }
        return cnt;
    }

    private void Update()
    {
        if(IsStart && !isPause)
        {
            TotalTime += Time.deltaTime;
        }
    }

    public float GetCE(FocusData data)
    {
        float result = 0.0f;

        List<FocusValue> valueList = data.FocusValueList;

        for (int i = 0; i < valueList.Count; i++)
        {
            if (valueList[i].CRESP != 1 && valueList[i].RESP == 1)
            {
                result++;
            }

        }
        return result;
    }

    public float GetOE(FocusData data)
    {
        float result = 0.0f;
        List<FocusValue> valueList = data.FocusValueList;

        for (int i = 0; i < valueList.Count; i++)
        {
            // OE
            if (valueList[i].CRESP == 1 && valueList[i].RESP != 1)
            {
                result++;
            }

        }
        return result;
    }

    public float GetRTV(FocusData data)
    {
        float RTCnt = 0.0f;
        float TotalRT = 0.0f;
        float meanRT = 0.0f;
        float RTV = 0.0f;
        List<FocusValue> valueList = data.FocusValueList;
        List<float> standardDeviationList = new List<float>();
        for (int i = 0; i < valueList.Count; i++)
        {
            if (valueList[i].CRESP == 1 && valueList[i].RESP == 1)
            {
                standardDeviationList.Add(valueList[i].reactionTime);
                RTCnt++;
                TotalRT += valueList[i].reactionTime;
            }
        }
        meanRT = TotalRT / RTCnt;
        float result = 0.0f;
        for (int k = 0; k < standardDeviationList.Count; k++)
        {

            float value = standardDeviationList[k] - meanRT;
            result += value * value;
        }
        RTV = Mathf.Sqrt(result / (RTCnt - 1.0f));

        return RTV;
    }

    public float GetCV(FocusData data)
    {
        float RTCnt = 0.0f;
        float TotalRT = 0.0f;
        float meanRT = 0.0f;
        float RTV = 0.0f;
        float CV = 0.0f;
        List<FocusValue> valueList = data.FocusValueList;
        List<float> standardDeviationList = new List<float>();

        for (int i = 0; i < valueList.Count; i++)
        {
            if (valueList[i].CRESP == 1 && valueList[i].RESP == 1)
            {
                standardDeviationList.Add(valueList[i].reactionTime);
                RTCnt++;
                TotalRT += valueList[i].reactionTime;
            }
        }
        meanRT = TotalRT / RTCnt;
        float result = 0.0f;
        for (int k = 0; k < standardDeviationList.Count; k++)
        {

            float value = standardDeviationList[k] - meanRT;
            result += value * value;
        }
        RTV = Mathf.Sqrt(result / (RTCnt - 1.0f));
        CV = RTV / meanRT;
        return CV;
    }

    public List<FocusValue> FocusValueProcessing(List<FocusValue> list, float min, float max)
    {
        List<FocusValue> returnlist = new List<FocusValue>();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].elapseTime <= max && list[i].elapseTime >= min)
            {
                returnlist.Add(list[i]);                
            }
        }
        return returnlist;
    }

    public float calculateAOI(List<AOIValue> list, float min, float max)
    {
        float result = 0.0f;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].AOIType == AOIType.AOI1)
            {
                float LookStart = list[i].EnterTime;
                float LookEnd = list[i].endTime;

                if (LookEnd < max)
                {
                    if (LookEnd < min)
                        continue;
                    else if (LookStart > min && LookEnd < max)
                    {
                        result += LookEnd - LookStart;
                        continue;
                    }
                    else if (LookStart < min && LookEnd < max)
                    {
                        float delta = LookEnd - min;
                        result += delta;
                        continue;
                    }
                    else
                        result += LookEnd - LookStart;
                }
                else if (LookEnd > max)
                {
                    if (LookStart > max)
                        break;
                    else if (LookStart < min && LookEnd > max)
                    {
                        result += max - min;
                        break;
                    }
                    else
                    {
                        result += max - LookStart;
                        break;
                    }
                }
            }
        }
        Debug.Log("calculateAOI AOI :" + result);
        return result;
    }

    public void Add_BetaCh1(float val)
    {
        data.EEGList[0].AddValue(val, GetTotalTime);
    }
    public void Add_BetaCh2(float val)
    {
        data.EEGList[1].AddValue(val, GetTotalTime);
    }
    public void Add_ThetaCh1(float val)
    {
        data.EEGList[2].AddValue(val, GetTotalTime);
    }
    public void Add_ThetaCh2(float val)
    {
        data.EEGList[3].AddValue(val, GetTotalTime);
    }
    public void CreateEEGList()
    {
        for(int i = 0; i < 4; i++)
        {
            data.EEGList.Add(new EEG());
        }
    }

    public List<float> GetEEGGraphValue()
    {
        List<float> list = new List<float>();

        list.Add(calculateBetaThetaRatioGraph(Static_0Sec, Static_300Sec));
        list.Add(calculateBetaThetaRatioGraph(Static_300Sec, Static_600Sec));
        list.Add(calculateBetaThetaRatioGraph(Static_600Sec, Static_900Sec));

        return list;
    }

    /// <summary>
    /// 베타세타 리스트의 마지막 Ratio값 리턴
    /// </summary>
    /// <returns></returns>
    public float calculateLastBetaThetaRatio()
    {
        if (data.EEGList == null) return 0;

        if (data.EEGList[0] == null) return 0;
        if (data.EEGList[1] == null) return 0;
        if (data.EEGList[2] == null) return 0;
        if (data.EEGList[3] == null) return 0;

        if (data.EEGList[0].valueList == null) return 0;
        if (data.EEGList[1].valueList == null) return 0;
        if (data.EEGList[2].valueList == null) return 0;
        if (data.EEGList[3].valueList == null) return 0;

        List<EEGValue> betaCh1 = data.EEGList[0].valueList;
        List<EEGValue> ThetaCh1 = data.EEGList[2].valueList;
        List<EEGValue> betaCh2 = data.EEGList[1].valueList;
        List<EEGValue> ThetaCh2 = data.EEGList[3].valueList;

        if (betaCh1 == null) return 0;
        if (betaCh2 == null) return 0;
        if (ThetaCh1 == null) return 0;
        if (ThetaCh2 == null) return 0;

        if (betaCh1.Count == 0) return 0;
        if (betaCh2.Count == 0) return 0;
        if (ThetaCh1.Count == 0) return 0;
        if (ThetaCh2.Count == 0) return 0;

        float CalcValue = 0;

        CalcValue = ((betaCh1[betaCh1.Count - 1].value + betaCh2[betaCh2.Count - 1].value) + (ThetaCh1[ThetaCh1.Count - 1].value + ThetaCh2[ThetaCh2.Count - 1].value)) * 0.5f;
        
        return CalcValue;
    }

    /// <summary>
    /// 그래프를 위한 EEG 퍼센트 계산
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private float calculateBetaThetaRatioGraph(float min, float max)
    {
        List<EEGValue> betaCh1 = data.EEGList[0].valueList;
        List<EEGValue> ThetaCh1 = data.EEGList[2].valueList;
        List<EEGValue> betaCh2 = data.EEGList[1].valueList;
        List<EEGValue> ThetaCh2 = data.EEGList[3].valueList;

        List<EEGValue> sbata1 = new List<EEGValue>();
        List<EEGValue> stheta1 = new List<EEGValue>();
        List<EEGValue> sbata2 = new List<EEGValue>();
        List<EEGValue> stheta2 = new List<EEGValue>();

        float TotalElapseTime = 0.0f;
        float TotalCount = 0;
        float Count = 0;
        float percent = 0;
        // 범위내 데이터 추출
        for (int i = 0; i < betaCh1.Count; i++)
        {
            if(betaCh1[i].elapseTime > min && betaCh1[i].elapseTime < max)
            {
                sbata1.Add(betaCh1[i]);
            }
        }
        for (int i = 0; i < betaCh2.Count; i++)
        {
            if (betaCh2[i].elapseTime > min && betaCh2[i].elapseTime < max)
            {
                sbata2.Add(betaCh2[i]);
            }
        }

        for (int i = 0; i < ThetaCh1.Count; i++)
        {
            if(ThetaCh1[i].elapseTime > min && ThetaCh1[i].elapseTime < max)
            {
                stheta1.Add(ThetaCh1[i]);
            }
        }
        for (int i = 0; i < ThetaCh2.Count; i++)
        {
            if (ThetaCh2[i].elapseTime > min && ThetaCh2[i].elapseTime < max)
            {
                stheta2.Add(ThetaCh2[i]);
            }
        }

        //Debug.Log("sBata Count : " + sbata1.Count);

        //for (int i = 0; i < sbata1.Count; i++)
        //{
        //    if(i != 0)
        //    {
        //        float val = (((stheta1[i].value / sbata1[i].value) + (stheta2[i].value / sbata2[i].value)) * 0.5f);
        //        //Debug.Log(string.Format("Theata/Beta Ratio[{1}] : {0}", val, i));
        //        //Debug.Log(string.Format("sBata[{0}].elapseTime : {1} / sBata[{0}-1].elapseTime : {2}", i, sbata1[i].elapseTime, sbata1[i - 1].elapseTime));

        //        if (float.IsNaN(val))
        //            val = 0;
        //        else
        //        {
        //            if (val >= 1.3f)
        //            {                        
        //                TotalElapseTime += sbata1[i].elapseTime - sbata1[i - 1].elapseTime;
        //            }
        //        }
        //    }            
        //}
        //Debug.Log("TotalEEG ElapseTime : " + TotalElapseTime);

        //float percent = 0;
        //if (TotalElapseTime == 0)        
        //    percent = 0;        
        //else        
        //    percent = (TotalElapseTime / (max - min)) * 100.0f;

        //Debug.Log("EEG Percent : " + percent);

        for (int i = 0; i < sbata1.Count; i++)
        {
            
            float val = (((stheta1[i].value / sbata1[i].value) + (stheta2[i].value / sbata2[i].value)) * 0.5f);
                
            if (float.IsNaN(val))
                val = 0;
            else
            {
                if (val >= 1.4f)
                {
                    Count++;
                }
            }
            TotalCount++;
        }

        if (Count == 0)
            return 0;

        float Ratio = 0.0f;

        Ratio = (Count / TotalCount) * 100.0f;

        if (float.IsNaN(Ratio))
            return 0;
        Debug.Log("Graph:: EEG Ratio : " + Ratio + " / Count : " + Count + " / TotalCount : " + TotalCount);
        return Ratio;
        //return percent;
    }

    /// <summary>
    /// TBR 계산
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public float calculateBetaThetaRatio(float min, float max)
    {
        if (data.EEGList.Count == 0) return 0;

        List<EEGValue> betaCh1 = data.EEGList[0].valueList;
        List<EEGValue> ThetaCh1 = data.EEGList[2].valueList;
        List<EEGValue> betaCh2 = data.EEGList[1].valueList;
        List<EEGValue> ThetaCh2 = data.EEGList[3].valueList;

        List<EEGValue> sbata1 = new List<EEGValue>();
        List<EEGValue> stheta1 = new List<EEGValue>();
        List<EEGValue> sbata2 = new List<EEGValue>();
        List<EEGValue> stheta2 = new List<EEGValue>();

        float TotalRatio = 0.0f;
        float Count = 0.0f;
        float TotalCount = 0.0f;
        // 범위내 데이터 추출
        for (int i = 0; i < betaCh1.Count; i++)
        {
            if (betaCh1[i].elapseTime > min && betaCh1[i].elapseTime < max)
            {
                sbata1.Add(betaCh1[i]);
            }
        }
        for (int i = 0; i < betaCh2.Count; i++)
        {
            if (betaCh2[i].elapseTime > min && betaCh2[i].elapseTime < max)
            {
                sbata2.Add(betaCh2[i]);
            }
        }

        for (int i = 0; i < ThetaCh1.Count; i++)
        {
            if (ThetaCh1[i].elapseTime > min && ThetaCh1[i].elapseTime < max)
            {
                stheta1.Add(ThetaCh1[i]);
            }
        }
        for (int i = 0; i < ThetaCh2.Count; i++)
        {
            if (ThetaCh2[i].elapseTime > min && ThetaCh2[i].elapseTime < max)
            {
                stheta2.Add(ThetaCh2[i]);
            }
        }

        for (int i = 0; i < sbata1.Count; i++)
        {
            if (i != 0)
            {
                float val = (((stheta1[i].value / sbata1[i].value) + (stheta2[i].value / sbata2[i].value)) * 0.5f);

                TotalRatio += val;

            }
            Count++;
        }

        //for (int i = 0; i < sbata1.Count; i++)
        //{            
        //    float val = (((stheta1[i].value / sbata1[i].value) + (stheta2[i].value / sbata2[i].value)) * 0.5f);

        //    if(val >= 1.3f)
        //    {
        //        Count++;
        //    }

        //    TotalCount++;            
        //}

        float Ratio = 0.0f;

        //if (Count == 0)
        //    return 0;

        //Ratio = (Count / TotalCount) * 100.0f;

        //if (float.IsNaN(Ratio))
        //    return 0;



        if (TotalRatio == 0)
            Ratio = 0;
        else
            Ratio = TotalRatio / Count;

        Debug.Log("EEG Ratio : " + Ratio + " / Count : " + Count + " / TotalRatio : " + TotalRatio);

        return Ratio;
    }    
}

