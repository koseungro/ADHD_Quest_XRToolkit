using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;

[System.Serializable]
public class PlayerData{

    public int Age = 0;
    public int Year = 0;
    public int Month = 0;
    public int Day = 0;
    public int Gender = 0;
    public string Name = "TEST";
    
    public void PlayerDataClear()
    {        
        Age = 0;
        Year = 0;
        Month = 0;
        Day = 0;
        Gender = 0;
        Name = "";
    }

    /// <summary>
    /// NBack 설정값 읽어오기
    /// </summary>
    public void ReadInfoFile()
    {
        try
        {
            string path = string.Format("{0}/{1}",

#if UNITY_EDITOR
                                    Directory.GetParent(Application.dataPath),
#else
                                    Application.persistentDataPath,
#endif

                                    "Setting"
                                    );
            Debug.Log("Folder Path : " + path);

            // txt 파일 path
            string FullPath = string.Format("{0}/UserInfo.txt", path);

            // 디렉토리 체크하여 생성
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Name=Test");
                sb.AppendLine("Gender=0");
                Name = "Test";
                Gender = 0;
                File.WriteAllText(FullPath, sb.ToString());
            }
            else
            {
                if (!File.Exists(FullPath))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("Name=Test");
                    sb.AppendLine("Gender=0");
                    Name = "Test";
                    Gender = 0;
                    File.WriteAllText(FullPath, sb.ToString());
                }
                else
                {
                    string[] ReadText = File.ReadAllLines(FullPath);
                    for (int i = 0; i < ReadText.Length; i++)
                    {
                        string[] splitTxt = ReadText[i].Trim().Split('=');
                        if (splitTxt.Length > 1)
                        {
                            switch (splitTxt[0])
                            {
                                case "Name":
                                    Name = splitTxt[1];
                                    break;
                                case "Gender":
                                    Gender = int.Parse(splitTxt[1]);
                                    break;
                            }

                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }

    }
}
