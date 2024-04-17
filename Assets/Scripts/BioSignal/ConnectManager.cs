using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO;
using System;

public class ConnectManager
{
    //Bluetooth state
    private int myState = 0;
    static readonly int STATE_NON = 0;
    static readonly int STATE_LISTEN = 1;
    static readonly int STATE_CONNECTING = 2;
    static readonly int STATE_CONNECTED = 3;
    //packet parsing and encoding thread
    private string portName = "";
    private bool isRun = false;
    //send buff
    private static Queue<double[]> eegOutBuff;
    private static Queue<float> gsrOutBuff;
    private static Queue<float> ppgOutBuff;
    //packet structure and variable
    readonly byte startByte = 0x81;
    readonly byte endByte = 0x82;
    private byte[] Packet;
    private int packetCount = 0;
    private readonly int packetSize = 16;
    //private ByteQueuing buffer;
    //android plugin tunnel
    private BluetoothModel model = null;
    //bluetooth searching result
    private List<string> btNameAddList;
    private bool isSearching = false;
    private bool isConnecting = false;
    private readonly string btAddKey = "btkey";
    //event 
    public UnityEngine.Events.UnityEvent onSearchingFinish;
    public UnityEngine.Events.UnityEvent onConnected;
    public UnityEngine.Events.UnityEvent onDisconnected;
    public UnityEngine.Events.UnityEvent onConnecting;


    public ConnectManager(Queue<double[]> _eegOutBuff, Queue<float> _gsrOutBuff, Queue<float> _ppgOutBuff)
    {
        eegOutBuff = _eegOutBuff;
        gsrOutBuff = _gsrOutBuff;
        ppgOutBuff = _ppgOutBuff;

        Packet = new byte[packetSize];

        if (model == null)
        {
            model = GameObject.Find("BluetoothModel").GetComponent<BluetoothModel>();
        }

        btNameAddList = new List<string>();
        btNameAddList.Clear();

        onSearchingFinish = new UnityEngine.Events.UnityEvent();
        onConnected = new UnityEngine.Events.UnityEvent();
        onDisconnected = new UnityEngine.Events.UnityEvent();
        onConnecting = new UnityEngine.Events.UnityEvent();

        model.onStateChange.AddListener((state) =>
        {
            myState = state;
            isConnecting = (myState == STATE_CONNECTING);
            if (myState == STATE_CONNECTED)
            {
                onConnected.Invoke();
                PlayerPrefs.SetString(btAddKey, portName);
                PlayerPrefs.Save();
            }
            else
            {
                onDisconnected.Invoke();
            }

            if (myState == STATE_CONNECTING)
            {
                onConnecting.Invoke();
            }

        });
        model.onFoundDevice.AddListener((name) =>
        {
            btNameAddList.Add(name);
        });
        model.onFoundNoDevice.AddListener(() =>
        {
            isSearching = false;
            btNameAddList.Clear();
            onSearchingFinish.Invoke();
        });
        model.onScanFinish.AddListener(() =>
        {
            isSearching = false;
            
            onSearchingFinish.Invoke();
        });
        model.onSendData.AddListener((data, len) =>
        {
            if (isRun)
            {
                PacketParsing(data, len);
            }
        });

        //AutoConnector();
    }

    private void AutoConnector()
    {
        if (PlayerPrefs.HasKey(btAddKey))
        {
            portName = PlayerPrefs.GetString(btAddKey);

            Open();
        }
    }

    public List<string> GetNames()
    {
        return btNameAddList;
    }

    public string[] GetName()
    {
        return btNameAddList.ToArray();
    }

    public void SetName(int index)
    {
        if (index < GetName().Length)
            SetName(GetName()[index]);
        else
            Debug.LogError("BT Selected index is out of length");
    }

    private bool SetName()
    {
        for (int i = 0; i < GetName().Length; i++)
        {
            Debug.Log(i + " : " + GetName()[i]);
            if (GetName()[i].Contains("BIOSIG"))
            {
                SetName(GetName()[i]);
                Debug.Log(GetName()[i] + " existes");
                return true;
            }
        }
        Debug.LogError("BIOSIG doesn't exist");
        return false;
    }

    public void SetName(string name)
    {
        portName = name;
    }

    public bool IsProcess()
    {
        return isConnecting || isSearching;
    }

    public bool IsConnected()
    {
        return myState == STATE_CONNECTED;
    }

    public string GetSelectedName()
    {
        return portName;
    }

    public bool Open()
    {
        if (IsConnected())
        {
            Debug.LogError("Can't Start!! Bluetooth is already connected");
            return false;
        }
        if (isSearching)
        {
            Debug.LogError("Can't Start!! Bluetooth is searching");
            return false;
        }
        if (isConnecting)
        {
            Debug.LogError("Can't Start!! Bluetooth is Not connecting");
            return false;
        }
        if (portName == "")
        {
            Debug.LogError("Device Not selected");
            return false;
        }
        model.Connect(portName);
        return true;
    }

    public void Close()
    {
        if (isRun)
            Stop();
        model.Stop();
    }

    public bool SearchingBT()
    {
        if (isSearching)
        {
            Debug.LogWarning("Bluetooth is already start scan mode");
            return false;
        }

        btNameAddList.Clear();
        model.GetPairedDevice();
        //model.SearchDevice();
        isSearching = true;
        return true;
    }

    public void Start()
    {
        if (!IsConnected())
        {
            Debug.LogError("Can't Start!! Bluetooth is Not connected");
            return;
        }

        //buffer.Clear();
        //model.Send("T");
        //model.Send("TR\n");

        isRun = true;
    }

    public void Stop()
    {
        if (!IsConnected())
        {
            Debug.LogError("Can't Stop!! port is NOT open");
            return;
        }

        //buffer.Clear();
        //model.Send("SR\n");

        isRun = false;
    }


    private void PacketParsing(byte[] data, int len)
    {
        for (int i = 0; i < len; i++)
        {
            if (packetCount > 0 && packetCount < packetSize - 1)
            {
                if ((Packet[packetCount] = data[i]) < 0x80)
                {
                    packetCount++;
                }
                else
                {
                    EnqueueDataWithPreData();
                    Debug.LogError(BitConverter.ToString(Packet, 0, packetCount + 1));
                    if (Packet[packetCount] != startByte)
                    {
                        packetCount = 0;
                    }
                    else
                    {
                        packetCount = 1;
                    }
                }
            }
            else if (packetCount == 0)
            {
                if ((Packet[0] = data[i]) == startByte)
                {
                    packetCount = 1;
                }
                else
                {
                    Debug.LogError(Packet[0].ToString("X2"));
                }
            }
            else if (packetCount == packetSize - 1)
            {
                if ((Packet[packetCount] = data[i]) == endByte)
                {
                    packetCount = 0;
                    GetDataFormPacket(Packet);
                }
                else
                {
                    EnqueueDataWithPreData();
                    Debug.LogError(BitConverter.ToString(Packet));
                    if (Packet[packetCount] != startByte)
                    {
                        packetCount = 0;
                    }
                    else
                    {
                        packetCount = 1;
                    }
                }
            }
        }
    }

    public void Updata()
    {

    }

    private float srrData;
    private double eegDataCh1;
    private double eegDataCh2;
    private float ppgData;

    private void GetDataFormPacket(byte[] mPacket)
    {
        int uint_24 = 0;

        uint_24 = 0;
        uint_24 = ((mPacket[1] << 14) | (mPacket[2] << 7) | (mPacket[3]));
        //uint_24 = (uint_24 & 0x7FFF) - (uint_24 & 0x8000);
        srrData = (float)uint_24;
        gsrOutBuff.Enqueue(srrData);

        uint_24 = (mPacket[4] << 21) | (mPacket[5] << 14) | (mPacket[6] << 7) | (mPacket[7]);
        uint_24 = (uint_24 & 0x7FFFFF) - (uint_24 & 0x800000);
        eegDataCh1 = (uint_24 * 3.83 * 1000) / (8388607d);

        uint_24 = 0;
        uint_24 = ((mPacket[8] << 21) | (mPacket[9] << 14) | (mPacket[10] << 7) | (mPacket[11]));
        uint_24 = (uint_24 & 0x7FFFFF) - (uint_24 & 0x800000);
        eegDataCh2 = (uint_24 * 3.83 * 1000) / (8388607d);
        double[] eegData = new double[2] { eegDataCh1, eegDataCh2 };
        eegOutBuff.Enqueue(eegData);

        uint_24 = 0;
        uint_24 = ((mPacket[12] << 14) | (mPacket[13] << 7) | (mPacket[14]));
        uint_24 = (uint_24 & 0x7FFF) - (uint_24 & 0x8000);
        ppgData = (float)uint_24;
        ppgOutBuff.Enqueue(ppgData);
    }

    private void EnqueueDataWithPreData()
    {
        gsrOutBuff.Enqueue(srrData);
        double[] eegData = new double[2] { eegDataCh1, eegDataCh2 };
        eegOutBuff.Enqueue(eegData);
        ppgOutBuff.Enqueue(ppgData);
    }



    public void Quit()
    {
        //if (readRoutine != null)
        //{
        //    readRoutineRun = false;
        //}
    }
}