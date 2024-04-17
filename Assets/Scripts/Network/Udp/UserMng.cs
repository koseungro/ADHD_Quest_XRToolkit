using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMng : Singleton<UserMng>
{

    private UdpUser user = new UdpUser();

    //private Dropdown dropDown;

    //private List<Dropdown.OptionData> optionDataList = new List<Dropdown.OptionData>();

    //public NetworkMng Mng;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        //Mng = FindObjectOfType<NetworkMng>();
        
    }

    // Use this for initialization
    void Start () {
        //if (Mng != null)
        //{
        //    Mng.StartGearVRServer(true,"SceneMain");
        //}
        //dropDown = GameObject.Find("Canvas/Dropdown").GetComponent<Dropdown>();
        //dropDown.options = optionDataList;
        //AddOption("None");

        user.CreateSocket();
        Debug.Log("Send Broadcast");
        user.SendIP();
    }

    public void PassConnect()
    {
        Debug.Log("UserMng::PassConnect");
        DataManager.Inst.isPassConnect = true;
    }

    public void ConnectToUser()
    {
        //Mng.SetIP("localhost");
        //Mng.StartGearVRServer(true, "SceneMain");

        //if(Mng.IsClientConnected())
        //{
        //    Mng.ServerChangeScene("SceneMain");
        //}
        //Mng.SetIP(dropDown.options[dropDown.value].text);
        //Mng.StartGearVRServer(false);
    }

    public void AddOption(string ipAddress)
    {
        //string[] splitStr = ipAddress.Split('/');
        //if(splitStr.Length > 2)
        //    ipAddress = splitStr[2];

        //Dropdown.OptionData data = new Dropdown.OptionData(ipAddress);

        //if (optionDataList.Find(x => x.text.Trim().Equals(ipAddress.Trim())) == null)
        //{
        //    optionDataList.Add(data);
        //}
    }

    public override void OnApplicationQuit()
    {
        try
        {
            user.ThreadStop();
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
        Application.Quit();
    }
}
