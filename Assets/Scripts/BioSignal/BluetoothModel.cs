using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class OnStateChnageEvent : UnityEvent<int> { };
public class OnSendDataEvent : UnityEvent<byte[], int> { };
public class OnFoundNoDeviceEvent : UnityEvent { };
public class OnScanFinishEvent : UnityEvent { };
public class OnFoundDeviceEvent : UnityEvent<string> { };

public class BluetoothModel : Singleton<BluetoothModel>
{
    private AndroidJavaClass pluginClass;
    private AndroidJavaObject mainActivity;
    private AndroidJavaObject pluginInstance;

    public OnStateChnageEvent onStateChange;
    public OnSendDataEvent onSendData;
    public OnFoundNoDeviceEvent onFoundNoDevice;
    public OnScanFinishEvent onScanFinish;
    public OnFoundDeviceEvent onFoundDevice;

    protected override void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        onStateChange = new OnStateChnageEvent();
        onSendData = new OnSendDataEvent();
        onFoundNoDevice = new OnFoundNoDeviceEvent();
        onScanFinish = new OnScanFinishEvent();
        onFoundDevice = new OnFoundDeviceEvent();

        //pluginClass = new AndroidJavaClass("com.fni.Panic.MainActivity");
        //pluginInstance = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
        pluginClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mainActivity = pluginClass.GetStatic<AndroidJavaObject>("currentActivity");
        pluginInstance = new AndroidJavaObject("com.fni.newbiosignal.MainActivity", mainActivity);

        Debug.Log("Pluagin Instance : " + pluginInstance);
        PluginStart();
    }

    void Start()
    {
        if (IsEnabled() == false)
        {
            bool reMess = EnableBluetooth();
            Debug.Log(reMess ? "Bluetooth enable Success" : "Bluetooth enable Faile");
        }
    }

    void Update()
    {

    }

    //--------------------------------------
    //      Unity -> Android Studio
    //--------------------------------------
    private void PluginStart()
    {
        pluginInstance.Call("StartPlugin");
    }

    public string Send(string message)
    {
        return pluginInstance.Call<string>("sendMessage", message);
    }

    public string Read()
    {
        return pluginInstance.Call<string>("readMessage");
    }

    public string SearchDevice()
    {
        return pluginInstance.Call<string>("ScanDevice");
    }

    public string GetPairedDevice()
    {
        return pluginInstance.Call<string>("GetPairedDevice");
    }

    public string GetDeviceConnectedName()
    {
        return pluginInstance.Call<string>("GetDeviceConnectedName");
    }

    public string Discoverable()
    {
        return pluginInstance.Call<string>("ensureDiscoverable");
    }

    public void Connect(string Address)
    {
        pluginInstance.Call("Connect", Address);
    }

    public bool EnableBluetooth()
    {
        return pluginInstance.Call<bool>("BluetoothEnable");
    }

    public string DisableBluetooth()
    {
        return pluginInstance.Call<string>("DisableBluetooth");
    }

    public string DeviceName()
    {
        return pluginInstance.Call<string>("DeviceName");
    }

    public bool IsEnabled()
    {
        return pluginInstance.Call<bool>("IsEnabled");
    }

    public bool IsConnected()
    {
        return pluginInstance.Call<bool>("IsConnected");
    }

    public void Stop()
    {
        pluginInstance.Call("stopThread");
    }

    public void showMessage(string mes)
    {
        pluginInstance.Call("showMessage", mes);
    }

    public void ReadMessage()
    {
        pluginInstance.Call("readMessage");
    }


    //--------------------------------------
    //      Android Studio -> Unity
    //--------------------------------------

    void OnStateChanged(string _State)
    {
        int state = int.Parse(_State);
        onStateChange.Invoke(state);
    }

    void OnSendData(string nothing)
    {
        byte[] buffer = pluginInstance.Call<byte[]>("GetBuffData");
        int len = int.Parse(nothing);

        Debug.LogFormat("{0} - {1}\t{2}", len, BitConverter.ToString(buffer, 0, len), (buffer.Length > len) ? BitConverter.ToString(buffer, len) : "");
        onSendData.Invoke(buffer, len);
    }

    void OnSendMessage(string _Message)
    {
        Debug.Log("On Send Message : " + _Message);
    }

    void OnReadMessage(string _Message)
    {
        Debug.Log("On Read Message : " + _Message);
    }


    void OnFoundNoDevice(string _s)
    {
        onFoundNoDevice.Invoke();
        Debug.Log("On Found No Device");
    }

    void OnScanFinish(string _s)
    {
        onScanFinish.Invoke();
        Debug.Log("On Scan Finish");
    }

    void OnFoundDevice(string _Device)
    {
        onFoundDevice.Invoke(_Device);
        Debug.Log("On Found Device : " + _Device);
    }
}
