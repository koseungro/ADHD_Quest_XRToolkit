using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct UserData
{
    public string ipAddress;
    public string Name;
    public string Sex;
    public string Percent;
    public string Explain;
    public string Level;
    public bool isMissionStart;
    public string EegValue;

    public UserData(string _ipAddress, string _name, string _sex, string _percent, string _explain, string _level, string _missionStart, string _eegValue)
    {
        ipAddress = _ipAddress;
        Name = _name;
        Sex = _sex;
        Percent = _percent;
        Explain = _explain;
        Level = _level;
        if(_missionStart.Equals("0"))
        {
            isMissionStart = false;
        }
        else
        {
            isMissionStart = true;
        }
        EegValue = _eegValue;
    }
}
public class ListenerMng : Singleton<ListenerMng> {

    /// <summary>
    /// 프로그램 종료 유무
    /// </summary>
    private bool isQuit = false;

    private Dropdown dropDown;

    private List<Dropdown.OptionData> optionDataList = new List<Dropdown.OptionData>();

    private UdpListener listener = new UdpListener();

    //public NetworkMng Mng;

    private List<UserData> AddUserList = new List<UserData>();

    

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
        //Mng = FindObjectOfType<NetworkMng>();
    }

    // Use this for initialization
    void Start () {
        listener.Run_Thread();
        StartCoroutine(CheckUserList());
    }

    public void AddIP(string ipAddress)
    {
        //ReceivedIP = ipAddress;
        Debug.Log("AddIP : " + ipAddress);

        string[] splitStr = ipAddress.Split('/');
        if (splitStr.Length > 2)
            ipAddress = splitStr[2];

        //Mng.SetIP(ipAddress);
    }

    public void AddOption(string infomation)
    {
        //Debug.Log(infomation);
        string[] splitStr = infomation.Split('/');
        if (splitStr.Length < 8) return;
        
        AddUserList.Add(
            new UserData(
                splitStr[0],
                splitStr[1],
                splitStr[2],
                splitStr[3],
                splitStr[4],
                splitStr[5],
                splitStr[6],
                splitStr[7]
                ));
        
    }
    IEnumerator CheckUserList()
    {
        while(!isQuit)
        {
            if (AddUserList.Count > 0)
            {
                List<ButtonNetwork> UserList = ClientManager.Inst.GetUserList();                
                ButtonNetwork button = UserList.Find(x => x.ipAddress.Equals(AddUserList[0].ipAddress));

                if (button != null)
                {
                    float ProgressVal = float.Parse(AddUserList[0].Percent);
                    button.PercentVal = (ProgressVal/100.0f);
                    button.PercentText.text = string.Format("{0:F0}%", ProgressVal);
                    button.ExplainText.text = AddUserList[0].Explain;
                    button.LevelText.text = AddUserList[0].Level;
                    button.isMissionStart = AddUserList[0].isMissionStart;
                    button.LifeTime = 0;
                    string truncateVal = "";
                    try
                    {
                        truncateVal = string.Format("{0:F2}", float.Parse(AddUserList[0].EegValue));
                    } catch(System.Exception e)
                    {
                        Debug.Log("ListenerMng.CheckUserList :: "+e.Message);
                        truncateVal = "0";
                    }
                    
                    button.EegValueStr = truncateVal;
                    AddUserList.RemoveAt(0);
                }
                else
                {

                    ClientManager.Inst.AddStandbyUser(
                        AddUserList[0].ipAddress,
                        AddUserList[0].Name,
                        AddUserList[0].Sex,
                        AddUserList[0].Percent,
                        AddUserList[0].Explain,
                        AddUserList[0].Level,
                        AddUserList[0].isMissionStart);
                    AddUserList.RemoveAt(0);
                }
            }
            yield return null;
        }
    }


    public override void OnApplicationQuit()
    {
        isQuit = true;
        try
        {
            listener.StopThread();
            base.OnApplicationQuit();
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        //Application.Quit();

    }
}
