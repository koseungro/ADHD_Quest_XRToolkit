using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;


public class HRV
{
    public struct FrqRang
    {
        public float start;
        public float end;
        public double pow;

        public int startIndex;
        public int endIndex;

        public FrqRang(float _start, float _end)
        {
            start = _start;
            end = _end;
            pow = 0d;
            startIndex = 0;
            endIndex = 0;
        }

        public void SetIndex(float multyVal)
        {
            startIndex = (int)(start * multyVal);
            endIndex = (int)(end * multyVal);
        }
    }

    public bool hrvCheck = false;

    private string savePath;
    private Thread computHrv;
    private float processPercent;
    private int sampleFrq;

    private FrqRang hf;
    private FrqRang lf;
    private FrqRang ulf;

    public HRV(int _sampleFrq)
    {
        sampleFrq = _sampleFrq;

        hf = new FrqRang(0.15f, 0.4f);
        lf = new FrqRang(0.04f, 0.15f);
        ulf = new FrqRang(0.0033f, 0.04f);
    }

    public bool Start(string path)
    {

        savePath = path;

        if (computHrv != null)
            computHrv.Abort();
        computHrv = new Thread(RunEachHrv);
        computHrv.Start();
        computHrv.IsBackground = false;

        return true;
    }

    private void RunEachHrv()
    {
        string hrvResultPath = savePath + @"Hrv.csv";
        string[] HrvFile;
        if (File.Exists(hrvResultPath))
        {
            HrvFile = File.ReadAllLines(hrvResultPath);
            File.Delete(hrvResultPath);
        }
        else
        {
            HrvFile = new String[] { "" };
        }

        Debug.LogFormat("HRV started this path : {0}", hrvResultPath);
        StreamWriter hrvSW = File.CreateText(hrvResultPath);
        hrvSW.WriteLine("name,SDNN,RMSSD,High Freq,Low Freq,Ultra Low Freq");

        string[] eachFile = Directory.GetDirectories(savePath);
        for (int i = 0; i < eachFile.Length; i++)
        {
            eachFile[i] += "/LinearBpm.csv";
            if (File.Exists(eachFile[i]))
            {
                string name = Directory.GetParent(eachFile[i]).Name;
                if (name != "Temp")
                {
                    int checkNum = IsComputedHRV(HrvFile, name);
                    if (checkNum != 0)
                    {
                        Debug.LogFormat("HRV Skip arleady computed : {0}", name);
                        hrvSW.WriteLine(HrvFile[checkNum]);
                    }
                    else
                    {
                        Debug.LogFormat("HRV computing... : {0}", name);
                        string data = HrvRoutine(eachFile[i]);
                        hrvSW.WriteLine(name + "," + data);
                    }
                }
            }
            else
            {
                Debug.LogWarning("This linear file not exist path : " + eachFile[0]);
                eachFile[i] = "Error";
            }
        }

        hrvSW.Flush();
        hrvSW.Close();
        hrvCheck = true;
    }

    public int IsComputedHRV(string[] HrvFile, string name)
    {
        for (int i = 0; i < HrvFile.Length; i++)
        {
            if (HrvFile[i].Split(',')[0] == name)
                return i;
        }
        return 0;
    }

    public float getProcessPercent()
    {
        return processPercent;
    }

    private string HrvRoutine(string bpmFilePath)
    {
        try
        {
            processPercent = 0f;

            //----------------Read txt------------------------------------
            string[] lines = File.ReadAllLines(bpmFilePath);
            lines[0] = lines[1];
            uint len = (uint)lines.Length;

            if (len < 2)
                return "Error";


            //----------------Frequwncy domain method------------------------
            double[] inputreal = new double[len];
            double[] inputimag = new double[len];

            for (uint i = 0; i < len; i++)
            {
                inputreal[i] = double.Parse(lines[i]);
                processPercent = (i * 25f) / len;
            }

            Fft.Transform(inputreal, inputimag);

            for (int i = 0; i < len; i++)
            {
                inputreal[i] = System.Math.Sqrt(inputreal[i] * inputreal[i] + inputimag[i] * inputimag[i]);
                processPercent = (i * 25f) / len + 25f;
            }

            //memory save
            inputimag = null;

            float temp = len / sampleFrq;

            hf.SetIndex(temp);
            hf.pow = 0d;
            lf.SetIndex(temp);
            lf.pow = 0d;
            ulf.SetIndex(temp);
            ulf.pow = 0d;

            double sum = 0d;
            for (int i = 0; i < len; i++)
            {
                sum += inputreal[i];
            }
            for (int i = hf.startIndex; i < hf.endIndex; i++)
            {
                hf.pow += inputreal[i];
            }
            for (int i = lf.startIndex; i < lf.endIndex; i++)
            {
                lf.pow += inputreal[i];
            }
            for (int i = ulf.startIndex; i < ulf.endIndex; i++)
            {
                ulf.pow += inputreal[i];
            }


            hf.pow /= sum;
            lf.pow /= sum;
            ulf.pow /= sum;

            Debug.LogFormat("{0} - {1} - {2}", hf.pow, lf.pow, ulf.pow);

            processPercent = 50f;

            //memory save
            inputreal = null;

            //--------------------Time domain method-----------------------
            Statistics nnInterval = new Statistics();
            float preBpm = float.Parse(lines[0]);
            float bpm = float.Parse(lines[1]);
            float preSlope = bpm - preBpm;
            preBpm = bpm;


            for (int i = 2; i < len; i++)
            {
                bpm = float.Parse(lines[i]);
                float slope = bpm - preBpm;
                if (slope != preSlope)
                {
                    nnInterval.AddData(60 / preBpm);
                }
                preBpm = bpm;
                preSlope = slope;

                processPercent = (i * 40f) / len + 50f;
            }

            float SDNN = nnInterval.GetStd();
            processPercent = 95f;
            float RMSSD = nnInterval.GetRmssd();
            processPercent = 100f;
            Debug.LogFormat("{0} - {1}", SDNN, RMSSD);

            return string.Format("{0},{1},{2},{3},{4}", SDNN, RMSSD, hf.pow, lf.pow, ulf.pow);

        }
        catch (ThreadAbortException ex)
        {
            Debug.LogError(ex.Message);
            return ex.Message;
        }
        catch (IOException ex)
        {
            Debug.LogError(ex.Message);
            return ex.Message;
        }
        catch (FormatException ex)
        {
            Debug.LogError(ex.Message);
            return ex.Message;
        }
    }

    public void Dispose()
    {
        if (computHrv != null)
        {
            if (computHrv.ThreadState == ThreadState.Running)
            {
                computHrv.Abort();
            }
        }
    }
}