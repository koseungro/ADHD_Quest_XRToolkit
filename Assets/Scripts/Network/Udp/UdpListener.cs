using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

public class UdpListener{

    /// <summary>
    /// 포트번호
    /// </summary>
    private int port = 9090;
    /// <summary>
    /// Thread 종료 체크함수
    /// </summary>
    private bool done = false;
    public bool isDone { get { return done; } set { done = value; } }

    /// <summary>
    /// Udp Listener Thread
    /// </summary>
    private Thread listener_thread;

    UdpClient listener;

    /// <summary>
    /// Thread Fucntion
    /// </summary>
    public void Run_Thread()
    {
        listener_thread = new Thread(StartListener);
        //listener_thread.IsBackground = true;
        listener_thread.Start();
    }

    /// <summary>
    /// Start Udp Listen
    /// </summary>
    public void StartListener()
    {
        done = false;
        listener = new UdpClient(port);

        IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, port);
        try
        {
            while (!done)
            {
                //Debug.Log("StartListener");
                // UDP 브로드캐스트 Package Receive
                byte[] bytes = listener.Receive(ref groupEP);

                string User = Encoding.UTF8.GetString(bytes);

                ListenerMng.Inst.AddOption(User);

                Socket receive = listener.Client;

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

                byte[] ipbytes = Encoding.UTF8.GetBytes(localIP);
                //listener.Send(ipbytes, 0);
                receive.SendTo(ipbytes, groupEP);
                Thread.Sleep(100);
            }            
        }
        catch (ThreadAbortException e)
        {   
            done = true;
            listener_thread.Join();
            //listener_thread.Join();            
        }
    }

    public void StopThread()
    {
        Debug.Log("UdpListener StopThread");        
        listener.Close();        
        listener_thread = null;

    }

}
