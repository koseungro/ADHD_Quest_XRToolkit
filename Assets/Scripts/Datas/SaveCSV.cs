using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SaveCSV{
 
    private const string kNEXT_LINE = "\r\n";
    private const string kCPT1 = "CPT1_BP";
    private const string kAOI1 = "CPT1_AOI_BP";
    private const string kCPT2 = "CPT2_BP";
    private const string kAOI2 = "CPT2_AOI_BP";
    private const string kCPT3 = "CPT3_BP";
    private const string kAOI3 = "CPT3_AOI_BP";
    private const string kCPT4 = "CPT4_BP";
    private const string kAOI4 = "CPT4_AOI_BP";

    private const string Comma = ",";
    private const string kDOUBLE_NEXT_LINE = kNEXT_LINE + kNEXT_LINE;

    //private string CPT_FirstColumns = "이름, 설명, AOI 상세, 참고사항";
    
    private string CPT_FirstColumns = "ACC, CRESP, RESP, RT";
    private string CPT_LastColumns = "정답율, CE, OE, meanRT, RTV, CV";
    private string AOIColumns = "AOI타입, 진입시간, 경과시간, 이탈시간";

    private Datas Datas
    {
        get { return DataManager.Inst.data; }
    }

    public void CreateAOI(int level)
    {
        List<AOI> aoiList = Datas.AOIList;

        StringBuilder dataList = new StringBuilder();

        if(aoiList.Count != 0)
        {
            int cnt1 = aoiList.Count;
            for(int i = 0; i < cnt1; i++)
            {
                dataList.AppendFormat("상황,{0}",DataManager.Inst.CurrentLevel + kNEXT_LINE);
                dataList.AppendFormat("StartTime,{0}", aoiList[i].StartTime + kNEXT_LINE);
                dataList.Append(AOIColumns + kNEXT_LINE);

                List<AOIValue> temp = aoiList[i].valueList;
                int cnt2 = temp.Count;
                for(int j = 0; j < cnt2; j++)
                {
                    dataList.AppendFormat("{0},", temp[j].AOIType.ToString());
                    dataList.AppendFormat("{0},", temp[j].EnterTime.ToString());
                    dataList.AppendFormat("{0},", temp[j].timeSpan.ToString());
                    dataList.AppendFormat("{0}{1}", temp[j].endTime.ToString(), kNEXT_LINE);
                }
                dataList.Append(kNEXT_LINE);
            }

            switch (level)
            {
                case 1:
                    SaveToCSV(kAOI1, dataList.ToString());
                    break;
                case 2:
                    SaveToCSV(kAOI2, dataList.ToString());
                    break;
                case 3:
                    SaveToCSV(kAOI3, dataList.ToString());
                    break;
                case 4:
                    SaveToCSV(kAOI4, dataList.ToString());
                    break;
            }
        }
    }

    public void CreateVariableData(LevelType type, int Level)
    {
        Variable var = new Variable();

        List<FocusData> list = Datas.FocusList;
        List<AOI> aoilist = Datas.AOIList;
        List<VasData> vasList = DataManager.Inst.data.VasList;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < list.Count; i++)
        {            
            GetFocusDataProcessing(ref var, DataManager.Inst.FocusValueProcessing(list[i].FocusValueList, DataManager.Static_0Sec, DataManager.Static_900Sec), 0);
            GetFocusDataProcessing(ref var, DataManager.Inst.FocusValueProcessing(list[i].FocusValueList, DataManager.Static_0Sec, DataManager.Static_300Sec), 1);
            GetFocusDataProcessing(ref var, DataManager.Inst.FocusValueProcessing(list[i].FocusValueList, DataManager.Static_300Sec, DataManager.Static_600Sec), 2);
            GetFocusDataProcessing(ref var, DataManager.Inst.FocusValueProcessing(list[i].FocusValueList, DataManager.Static_600Sec, DataManager.Static_900Sec), 3);

            GetAOIProcessing(ref var, aoilist[i].valueList);

            GetEEGProcessing(ref var);

            var.attention_SR = vasList[i].VasValueList[0].value * 100.0f;

            Debug.Log(var.ToCSV(type, Level));

            sb.Append(var.ToCSV(type, Level));
            sb.Append(kDOUBLE_NEXT_LINE);
        }

        string fileName = "";
        fileName = string.Format("{0}_{1}_{2}", DataManager.Inst.Player.Name, DataManager.Inst.CurrentLevel,DataManager.NowTimeToString2());

        SaveToCSV(fileName, sb.ToString());
    }

    public void GetAOIProcessing(ref Variable var, List<AOIValue> dataList)
    {        
        if (dataList.Count != 0)
        {   
            var.AOI_total_fixation_duration = DataManager.Inst.calculateAOI(dataList, DataManager.Static_0Sec, DataManager.Static_900Sec);                    
            var.AOI_first_fixation_duration = DataManager.Inst.calculateAOI(dataList, DataManager.Static_0Sec, DataManager.Static_300Sec);
            var.AOI_middle_fixation_duration = DataManager.Inst.calculateAOI(dataList, DataManager.Static_300Sec, DataManager.Static_600Sec);
            var.AOI_last_fixation_duration = DataManager.Inst.calculateAOI(dataList, DataManager.Static_600Sec, DataManager.Static_900Sec);
        }
    }

    public void GetFocusDataProcessing(ref Variable var, List<FocusValue> dataList, int Level)
    {
        List<FocusValue> focusList = dataList;

        if (focusList.Count > 0)
        {  
            List<float> standardDeviationList = new List<float>();

            float Corr = 0.0f;
            float CE = 0.0f;
            float OE = 0.0f;
            float RTCnt = 0.0f;
            float TotalRT = 0.0f;
            float meanRT = 0.0f;
            float RTV = 0.0f;
            float CV = 0.0f;

            for (int i = 0; i < focusList.Count; i++)
            {
                if (focusList[i].ACC == 1)
                {
                    Corr++;
                }
                // CE
                if (focusList[i].CRESP != 1 && focusList[i].RESP == 1)
                {
                    CE++;
                }
                // OE
                if (focusList[i].CRESP == 1 && focusList[i].RESP != 1)
                {
                    OE++;
                }

                // meanRT, RTV, CV 위한 데이터
                if (focusList[i].CRESP == 1 && focusList[i].RESP == 1)
                {
                    standardDeviationList.Add(focusList[i].reactionTime);
                    RTCnt++;
                    TotalRT += focusList[i].reactionTime;
                }
            }

            Debug.Log(string.Format("TotalRT : {0} / RTCnt : {1}", TotalRT, RTCnt));

            meanRT = TotalRT / RTCnt;

            float result = 0.0f;
            for (int k = 0; k < standardDeviationList.Count; k++)
            {
                float value = standardDeviationList[k] - meanRT;
                result += value * value;
            }
            Debug.Log("result / (RTCnt - 1.0f)" + result / (RTCnt - 1.0f));
            float rtvTemp = Mathf.Sqrt(result / (RTCnt - 1.0f));
            if (float.IsNaN(Mathf.Sqrt(result / (RTCnt - 1.0f))))
                RTV = 0;
            else
                RTV = Mathf.Sqrt(result / (RTCnt - 1.0f));
            if (float.IsNaN(RTV / meanRT))
                CV = 0;
            else
                CV = RTV / meanRT;

            switch (Level)
            {
                case 0:
                    var.total_Corr = Corr;
                    var.total_CE = CE;
                    var.total_OE = OE;
                    var.total_meanRT = meanRT;
                    var.total_RTV = RTV;
                    var.total_CV = CV;
                    var.EEG_total_meanTBR = 0;
                    break;
                case 1:
                    var.first_Corr = Corr;
                    var.first_CE = CE;
                    var.first_OE = OE;
                    var.first_meanRT = meanRT;
                    var.first_RTV = RTV;
                    var.first_CV = CV;
                    var.EEG_first_meanTBR = 0;
                    break;
                case 2:
                    var.middle_Corr = Corr;
                    var.middle_CE = CE;
                    var.middle_OE = OE;
                    var.middle_meanRT = meanRT;
                    var.middle_RTV = RTV;
                    var.middle_CV = CV;
                    var.EEG_middle_meanTBR = 0;
                    break;
                case 3:
                    var.last_Corr = Corr;
                    var.last_CE = CE;
                    var.last_OE = OE;
                    var.last_meanRT = meanRT;
                    var.last_RTV = RTV;
                    var.last_CV = CV;
                    var.EEG_last_meanTBR = 0;
                    break;
            }
            
        }
    }
    
    public void GetEEGProcessing(ref Variable var)
    {
        var.EEG_total_meanTBR = DataManager.Inst.calculateBetaThetaRatio(0, 900);
        var.EEG_first_meanTBR = DataManager.Inst.calculateBetaThetaRatio(0, 300);
        var.EEG_middle_meanTBR = DataManager.Inst.calculateBetaThetaRatio(300, 600);
        var.EEG_last_meanTBR = DataManager.Inst.calculateBetaThetaRatio(600, 900);
    }

    public void CreateBP_CTP(int level)
    {
        List<FocusData> focusList = Datas.FocusList;

        StringBuilder dataList = new StringBuilder();

        if (focusList.Count != 0)
        {   
            for (int i = 0; i < focusList.Count; i++)
            {
                dataList.AppendFormat("상황,{0}", DataManager.Inst.CurrentLevel + kNEXT_LINE);
                dataList.AppendFormat("StartTime,{0}", focusList[i].StartTime + kNEXT_LINE);
                dataList.Append(CPT_FirstColumns + kNEXT_LINE);

                List<float> standardDeviationList = new List<float>();

                float CE = 0.0f;
                float OE = 0.0f;
                float RTCnt = 0.0f;
                float TotalRT = 0.0f;
                float meanRT = 0.0f;
                float RTV = 0.0f;
                float CV = 0.0f;

                for (int j = 0; j < focusList[i].FocusValueList.Count; j++)
                {                    
                    dataList.AppendFormat("{0},", focusList[i].FocusValueList[j].ACC.ToString());
                    dataList.AppendFormat("{0},", focusList[i].FocusValueList[j].CRESP.ToString());
                    dataList.AppendFormat("{0},", focusList[i].FocusValueList[j].RESP.ToString());
                    dataList.AppendFormat("{0}{1}", focusList[i].FocusValueList[j].reactionTime.ToString(), kNEXT_LINE);

                    // CE
                    if(focusList[i].FocusValueList[j].CRESP != 1 && focusList[i].FocusValueList[j].RESP == 1)
                    {
                        CE++;
                    }
                    // OE
                    if (focusList[i].FocusValueList[j].CRESP == 1 && focusList[i].FocusValueList[j].RESP != 1)
                    {
                        OE++;
                    }

                    // meanRT, RTV, CV 위한 데이터
                    if (focusList[i].FocusValueList[j].CRESP == 1 && focusList[i].FocusValueList[j].RESP == 1)
                    {
                        standardDeviationList.Add(focusList[i].FocusValueList[j].reactionTime);
                        RTCnt++;                        
                        TotalRT += focusList[i].FocusValueList[j].reactionTime;
                    }
                }

                meanRT = TotalRT / RTCnt;
                float result = 0.0f;
                for(int k = 0; k < standardDeviationList.Count; k++)
                {                    
                    float value = standardDeviationList[k] - meanRT;                    
                    result += value * value;
                }                
                RTV = Mathf.Sqrt( result / (RTCnt - 1.0f) );
                CV = RTV / meanRT;

                float AnswerPercent = (RTCnt / focusList[i].FocusValueList.Count) * 100.0f;

                dataList.Append(kNEXT_LINE);
                dataList.Append(CPT_LastColumns+kNEXT_LINE);
                dataList.AppendFormat("{0},{1},{2},{3},{4},{5}{6}",AnswerPercent, CE, OE, meanRT, RTV, CV, kDOUBLE_NEXT_LINE);
            }
        }
        switch(level)
        {
            case 1:
                SaveToCSV(kCPT1, dataList.ToString());
                break;
            case 2:
                SaveToCSV(kCPT2, dataList.ToString());
                break;
            case 3:
                SaveToCSV(kCPT3, dataList.ToString());
                break;
            case 4:
                SaveToCSV(kCPT4, dataList.ToString());
                break;
        }  
    }

    public void SaveToCSV(string fileName, string data)
    {
        string path = string.Format("{0}/{1}",
#if UNITY_EDITOR
            string.Format("{0}/Data/CSV", Directory.GetParent(Application.dataPath)),
#else
            string.Format("{0}/Data/CSV", Application.persistentDataPath),
#endif
            DataManager.NowToString(1)            
            );
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filePath = string.Format("{0}/{1}.csv", path, fileName);
        if(File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        File.WriteAllText(filePath, data, System.Text.Encoding.UTF8);
        Debug.Log(fileName + " Save Done");
        //DataManager.Inst.data.DatasClear();
    }    
}

public struct Variable
{
    public float AOI_total_fixation_duration;

    public float total_Corr;
    public float total_CE;
    public float total_OE;
    public float total_meanRT;
    public float total_RTV;
    public float total_CV;
    public float EEG_total_meanTBR;
    public float AOI_first_fixation_duration;
	public float first_Corr;
    public float first_CE;
    public float first_OE;
    public float first_meanRT;
    public float first_RTV;
    public float first_CV;
    public float EEG_first_meanTBR;
    public float AOI_middle_fixation_duration;
	public float middle_Corr;
    public float middle_CE;
    public float middle_OE;
    public float middle_meanRT;
    public float middle_RTV;
    public float middle_CV;
    public float EEG_middle_meanTBR;
    public float AOI_last_fixation_duration;
	public float last_Corr;
    public float last_CE;
    public float last_OE;
    public float last_meanRT;
    public float last_RTV;
    public float last_CV;
    public float EEG_last_meanTBR;
    public float attention_SR;

    private string SelectLevelText(LevelType type, int Level)
    {
        string result = "";

        switch(type)
        {
            case LevelType.ROOM_LEVEL1:
            case LevelType.ROOM_LEVEL2:
            case LevelType.ROOM_LEVEL3:
                result += "R";                
                break;
            case LevelType.LIBRARY_LEVEL1:
            case LevelType.LIBRARY_LEVEL2:
            case LevelType.LIBRARY_LEVEL3:
                result += "L";
                break;
            case LevelType.STREET_LEVEL1:
            case LevelType.STREET_LEVEL2:
            case LevelType.STREET_LEVEL3:
                result += "S";
                break;

            case LevelType.CAFE_LEVEL1:
            case LevelType.CAFE_LEVEL2:
            case LevelType.CAFE_LEVEL3:
                result += "C";
                break;
        }

        switch (Level)
        {
            case 0:
                result += "_LV1_";
                break;
            case 1:
                result += "_LV2_";
                break;
            case 2:
                result += "_LV3_";
                break;

        }
        return result;
    }

    public string ToCSV(LevelType type, int Level)
    {
        return string.Format
        (
        //"{34}AOI_total_fixation_duration,{0}{33}" +
         "{34}total_Corr,{1}{33}" +
         "{34}total_CE,{2}{33}" +
         "{34}total_OE,{3}{33}" +
         "{34}total_meanRT,{4}{33}" +
         "{34}total_RTV,{5}{33}" +
         //"{34}total_CV,{6}{33}" +
         "{34}EEG_total_meanTBR,{7}{33}{33}{33}" +
         //"{34}AOI_first_fixation_duration,{8}{33}" +
         "{34}first_Corr,{9}{33}" +
         "{34}first_CE,{10}{33}" +
         "{34}first_OE,{11}{33}" +
         "{34}first_meanRT,{12}{33}" +
         "{34}first_RTV,{13}{33}" +
         //"{34}first_CV,{14}{33}" +
         "{34}EEG_first_meanTBR,{15}{33}{33}{33}" +
         //"{34}AOI_middle_fixation_duration,{16}{33}" +
         "{34}middle_Corr,{17}{33}" +
         "{34}middle_CE,{18}{33}" +
         "{34}middle_OE,{19}{33}" +
         "{34}middle_meanRT,{20}{33}" +
         "{34}middle_RTV,{21}{33}" +
         //"{34}middle_CV,{22}{33}" +
         "{34}EEG_middle_meanTBR,{23}{33}{33}{33}" +
         //"{34}AOI_last_fixation_duration,{24}{33}" +
         "{34}last_Corr,{25}{33}" +
         "{34}last_CE,{26}{33}" +
         "{34}last_OE,{27}{33}" +
         "{34}last_meanRT,{28}{33}" +
         "{34}last_RTV,{29}{33}" +
         //"{34}last_CV,{30}{33}" +
         "{34}EEG_last_meanTBR,{31}{33}{33}{33}" +
         "{34}attention_SR,{32}{33}"
          ,
          AOI_total_fixation_duration,
         total_Corr,
         total_CE,
         total_OE,
         total_meanRT,
         total_RTV,
         total_CV,
         EEG_total_meanTBR,
         AOI_first_fixation_duration,
         first_Corr,
         first_CE,
         first_OE,
         first_meanRT,
         first_RTV,
         first_CV,
         EEG_first_meanTBR,
         AOI_middle_fixation_duration,
         middle_Corr,
         middle_CE,
         middle_OE,
         middle_meanRT,
         middle_RTV,
         middle_CV,
         EEG_middle_meanTBR,
         AOI_last_fixation_duration,
         last_Corr,
         last_CE,
         last_OE,
         last_meanRT,
         last_RTV,
         last_CV,
         EEG_last_meanTBR,
         attention_SR,         
         "\r\n",
         SelectLevelText(type, Level)
        );
    }
}