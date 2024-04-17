using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLog : MonoBehaviour {

    public string output = "";
    public string stack = "";

    private StreamWriter writer;
    private string path = "";
    private List<string> TextList = new List<string>();

    private string line1 = "-------------------------";
    private string line2 = "-------------------------\n";
    private string line3 = "-----------------------------------------------------\n\n";
    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);

        Application.logMessageReceived += HandleLog;

        path = string.Format("{0}/log/{1}_log.txt",
#if UNITY_EDITOR
                                    Directory.GetParent(Application.dataPath),
#else
                                    Application.persistentDataPath,
#endif
                                    NowToString()
                                     );
        
    }

    private void OnDisable()
    {
        string temp = "";
        for (int i = 0; i < TextList.Count; i++)
            temp += TextList[i].ToString();

        if(!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        File.AppendAllText(path, temp);

        Application.logMessageReceived -= HandleLog;
    }
    private string NowToString()
    {
        return DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss");
    }
    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        string text = "";
        if (type == LogType.Error)
        {
            text += "-------------------------Error-------------------------\n";
            text += string.Format("{0} : {1}-{2}\n", NowToString(), stackTrace, logString);
            text += line3;
        }
        else
        {
            text += string.Format("{0}{1}{2}", line1, type.ToString(), line2);
            text += string.Format("{0} : {1}\n", NowToString(), logString);
            text += line3;
        }
        
        AddText(text);
        //output = logString;
        //stack = stackTrace;
    }

    public void AddText(string text)
    {
        TextList.Add(text);
    }

    public void SetText(string add)
    {
        
    }
}
