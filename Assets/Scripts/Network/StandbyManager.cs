using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandbyManager : MonoBehaviour {

    //public NetworkMng NetMng;
    
    // Use this for initialization
    void Start () {
        
    }
	
	IEnumerator FindNetworkMng()
    {
        Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");
        yield return null;
        //while(NetMng == null)
        //{
        //    NetMng = FindObjectOfType<NetworkMng>();

        //    if (NetMng == null)
        //        yield return null;
        //    else
        //        break;
        //}

    }
}
