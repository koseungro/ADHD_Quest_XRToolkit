using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test : MonoBehaviour {

    public Transform Camera;
    
	// Use this for initialization
	void Start () {

        StartCoroutine(Temperature());
	}
	
    IEnumerator Temperature()
    {
        WaitForSeconds sec = new WaitForSeconds(1.0f);
        while(true)
        {
            //Debug.Log(OVRManager.batteryTemperature);            
            Debug.Log(DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss"));
            yield return sec;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {

            float speed = 2.0f;

            Quaternion t = new Quaternion(Camera.localRotation.x + -speed * Time.deltaTime,
                                           Camera.localRotation.y,
                                           Camera.localRotation.z,
                                           Camera.localRotation.w);
            Camera.localRotation = t;
        }
        if (Input.GetKey(KeyCode.S))
        {

            float speed = 2.0f;

            Quaternion t = new Quaternion(Camera.localRotation.x + speed * Time.deltaTime,
                                           Camera.localRotation.y,
                                           Camera.localRotation.z,
                                           Camera.localRotation.w);
            Camera.localRotation = t;
        }
        if (Input.GetKey(KeyCode.D))
        {

            float speed = 2.0f;

            Quaternion t = new Quaternion(Camera.localRotation.x,
                                           Camera.localRotation.y + speed * Time.deltaTime,
                                           Camera.localRotation.z,
                                           Camera.localRotation.w);
            Camera.localRotation = t;
        }
        if (Input.GetKey(KeyCode.A))
        {

            float speed = 2.0f;

            Quaternion t = new Quaternion(Camera.localRotation.x,
                                           Camera.localRotation.y + -speed * Time.deltaTime,
                                           Camera.localRotation.z,
                                           Camera.localRotation.w);
            Camera.localRotation = t;
        }
    }
}
