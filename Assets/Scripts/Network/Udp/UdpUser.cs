using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;

using System.Threading;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

public class UdpUser{

    private Socket socket;

    private int port = 9090;

    public byte[] Receivedbytes = new byte[1024];
    public bool isReceived = false;

    private Thread SendIP_thread;

    public bool isDone = false;
    private bool isSendReceive = false;

    public void CreateSocket()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.EnableBroadcast = true;
    }

    public void SendIP()
    {
        SendIP_thread = new Thread(SendIP_Thread);
        SendIP_thread.Start();
    }
    public void ThreadStop()
    {
        Debug.Log("UdpUser StopThread");
        socket.Close();
        //SendIP_thread.Abort();               
        SendIP_thread = null;
    }
    public void SendIP_Thread()
    {
        try
        {
            while (!isDone)
            {
                string localIP = "";
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
                
                byte[] sendbuf = Encoding.ASCII.GetBytes(localIP);

                EndPoint targetEndPoint = new IPEndPoint(IPAddress.Broadcast, port);
                //Debug.Log("SendIP");
                string sendString = "";
                try
                {
                    if (DataManager.Inst == null) return;
                    string MissionLevelStr = "";
                    switch (DataManager.Inst.MissionLevel)
                    {
                        case 0:
                        case 1:
                            MissionLevelStr = "초급";
                            break;
                        case 2:
                            MissionLevelStr = "중급";
                            break;
                        case 3:
                            MissionLevelStr = "고급";
                            break;
                    }
                    sendString = string.Format(
                        "{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}",
                        localIP,
                        DataManager.Inst.Player.Name == null ? " " : DataManager.Inst.Player.Name,
                        DataManager.Inst.Player.Gender == 0 ? "남" : "여",
                        DataManager.Inst.MissionProgressValue,
                        DataManager.Inst.SceneName,
                        MissionLevelStr,
                        DataManager.Inst.MissionLevel == 0 ? "0" : "1",
#if UNITY_EDITOR
                        "0.0"
#else
                        DataManager.Inst.calculateLastBetaThetaRatio()
#endif

                        );
                }
                catch(System.Exception e)
                {
                    return;
                }

                //Debug.Log("UdpListener sendString : " + sendString);
                byte[] ipbytes = Encoding.UTF8.GetBytes(sendString);

                // Send IPAddress
                //socket.SendTo(sendbuf, targetEndPoint);
                socket.SendTo(ipbytes, targetEndPoint);

                //if (!isSendReceive)
                //{
                //    isSendReceive = true;
                //    Run_Receive_Thread();
                //}

                Thread.Sleep(100);
            }
        }
        catch(ThreadAbortException e)
        {
            isDone = true;
            SendIP_thread.Join();
        }
    }

    //private bool done = false;
    //public bool isDone { get { return done; } set { done = value; } }
    private Thread listener_thread;

    private void Run_Receive_Thread()
    {
        listener_thread = new Thread(ReceiveThread);
        listener_thread.Start();
    }

    private void ReceiveThread()
    {   
        socket.Receive(Receivedbytes);
        Debug.Log("Received IP");
        string ReceivedIP = Encoding.UTF8.GetString(Receivedbytes);
        UserMng.Inst.AddOption(ReceivedIP);
        //Array.Clear(Receivedbytes, 0, Receivedbytes.Length);
        isReceived = false;
        //isDone = true;
        //listener_thread.Abort();        
    }
}
