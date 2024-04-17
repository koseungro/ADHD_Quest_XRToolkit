using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainBioSignal : Singleton<MainBioSignal>
{
    float recStartTime;
    bool isPlay = false;
    bool isRecord = false;
    bool isActiveOnGui = true;
    bool isComputHrv = false;
    bool isConnect = false;

    private string filterPath;
    private string savePath;
    private string menuPortKey = "port";
    private string menuFilterEegKey = "eeg filter";
    private string menuFilterPpgKey = "ppg filter";
    private string menuFilterGsrKey = "gsr filter";
    private string menuPathFilterKey = "filter path";
    private string menuPathSaveKey = "save path";

    ConnectManager port;
    public ConnectManager GetConnectMng { get { return port; } }
    DataWriter rec;
    public DataWriter GetRec { get { return rec; } }
    Queue<float> ppgBuffer;
    Filter PpgFilter;
    PeakDetector PpgPeak;
    HRV PpgHrv;
    Queue<float> LinearPeakBuff;
    Queue<KeyValuePair<uint, float>> PpgPeakBuff;

    Queue<float> gsrBuffer;
    Filter GsrFilter;

    Queue<double[]> eegBuffer;

    SignalMaker Sinusoidal;
    Queue<float> SinBuffer;

    Filter EegFilter;

    Window<double>[] EegWindow;

    EEGPower[] bandPw;

    int windowSize = 1000;
    int chNum = 2;
    int sampleFreq = 250;
    int stftOverlap = 90;//90%

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {        
        DataManager.Inst.CreateEEGList();
        filterPath = Application.persistentDataPath + "/filters.txt";
        savePath = Application.persistentDataPath + "/UserDataFiles/";

        ppgBuffer = new Queue<float>();
        //PpgFilter = new Filter(filterPath, 1);
        LinearPeakBuff = new Queue<float>();
        PpgPeakBuff = new Queue<KeyValuePair<uint, float>>();

        PpgInit();
        GsrInit();
        EegInit();
        StftInit();
        BandpwInit();
            
        port = new ConnectManager(eegBuffer, gsrBuffer, ppgBuffer);
        rec = new DataWriter(savePath);
        rec.AddDataKey("rawEeg");
        //rec.AddDataKey("rawPpg");
        //rec.AddDataKey("LinearBpm");
        //rec.AddDataKey("PpgPeak");
        //rec.AddDataKey("rawGsr");
        for (int i = 0; i < chNum; i++)
        {
            rec.AddDataKey("stftCh" + i);
            rec.AddDataKey("eegBand" + i);
        }

        rec.SetSavePath(savePath);

        portSearching();

        StartCoroutine(SetBluetoothName());
    }
    public void ConnectBluetooth()
    {
        StartCoroutine(SetBluetoothName());
    }
    IEnumerator SetBluetoothName()
    {
        bool isFind = false;
        int count = 0;
        WaitForSeconds sec = new WaitForSeconds(2.0f);        
        while(true)
        {
            if(count > 2)
            {
                Debug.Log("Not Found Bluetooth");
                break;
            }
            
            List<string> names = port.GetNames();
            Debug.Log("port Count : " + names.Count);
            for (int i = 0; i < names.Count; i++)
            {
                Debug.Log(string.Format("names[{0}] : {1}", i, names[i]));
                if (names[i].Trim().Contains("BIOSIG"))
                {
                    Debug.Log("Find!!!");
                    port.SetName(i);
                    isFind = true;
                    break;
                }
            }

            if (isFind)
                break;
            count++;
            yield return sec;
        }
        if(isFind)
        {
            port.Open();
            //while (!port.IsConnected())
            //{
            //    yield return null;
            //}
        }

        
        //port.Start();
        //rec.RecordStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!port.IsConnected())
            return;

        port.Updata();

        for (int i = 0; i < eegBuffer.Count; i++)
        {
            double[] data = eegBuffer.Dequeue();    //data = {EegCh1, EegCh2, ...}
            rec.Record(data, "rawEeg");
            
            EegFilter.filter(data);
            
            for (int j = 0; j < chNum; j++)
            {
                EegWindow[j].InputData(data[j]);
            }
        }

        //process each chanel
        for (int ch = 0; ch < chNum; ch++)
        {
            //get windows data
            double[][] stftData = EegWindow[ch].GetWindows();   //stftData = {time(n)window, time(n+1)window, ...}
            for (int i = 0; i < stftData.Length; i++)
            {
                //fft
                double[] inputreal = stftData[i];
                double[] inputimag = new double[windowSize];
                Fft.Transform(inputreal, inputimag);

                //get fft power
                double[] power = Magnitude(inputreal, inputimag);

                rec.Record(power, "stftCh" + ch);
                //push fft power to get band power
                float[] band = bandPw[ch].GetBandPw(power);

                if (ch == 0)
                {
                    DataManager.Inst.Add_BetaCh1(band[1]);
                    DataManager.Inst.Add_ThetaCh1(band[3]);
                }
                else
                {
                    DataManager.Inst.Add_BetaCh2(band[1]);
                    DataManager.Inst.Add_ThetaCh2(band[3]);
                }
                rec.Record(band, "eegBand" + ch);
            }
        }
    }

    //fft power
    double[] Magnitude(double[] real, double[] img)
    {
        double[] result = new double[real.Length];
        for (int i = 0; i < real.Length; i++)
        {
            result[i] = System.Math.Sqrt(real[i] * real[i] + img[i] * img[i]);
        }

        return result;
    }

    /// <summary>
    /// Filter 텍스트 파일 생성
    /// </summary>
    void MakeFilterTxt()
    {
        TextAsset filters = Resources.Load<TextAsset>("filters");
        if (!File.Exists(filterPath))
        {
            File.WriteAllText(filterPath, filters.text);
        }
    }

    private void PpgInit()
    {
        PpgPeak = new PeakDetector(LinearPeakBuff, PpgPeakBuff, sampleFreq);

        PpgHrv = new HRV(sampleFreq);
        //PpgFilter.SetFilter(3);
    }

    private void GsrInit()
    {
        gsrBuffer = new Queue<float>();
        GsrFilter = new Filter(filterPath, 1);
        GsrFilter.SetFilter(4);
    }

    private void EegInit()
    {
        eegBuffer = new Queue<double[]>();
        EegFilter = new Filter(filterPath, chNum);
        EegFilter.SetFilter(1);
    }
    void StftInit()
    {
        EegWindow = new Window<double>[chNum];
        for (int i = 0; i < chNum; i++)
        {
            EegWindow[i] = new Window<double>(windowSize, stftOverlap);
        }
    }
    void BandpwInit()
    {
        bandPw = new EEGPower[chNum];

        for (int i = 0; i < chNum; i++)
        {
            bandPw[i] = new EEGPower(sampleFreq, windowSize);
            bandPw[i].AddBand("alpha", 8f, 12.99f);
            bandPw[i].AddBand("beta", 13f, 30f);
            bandPw[i].AddBand("gamma", 30f, 40f);
            bandPw[i].AddBand("theta", 4f, 7.99f);
            bandPw[i].AddBand("delta", 0.2f, 3.99f);
        }
    }

    void portSearching()
    {
        if (port.SearchingBT())
            Debug.Log("Buluetooth Searching");        
    }

    public override void OnApplicationQuit()
    {
        Debug.Log("============== OnApplicationQuit ==============");
        rec.RecordStop();
        //you must close serialport. if didnt, you can't open this port again
        port.Close();
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.SuppressFinalize(this);
    }
}
