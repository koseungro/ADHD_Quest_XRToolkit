using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrow : MonoBehaviour {

    [SerializeField]
    private Transform rotateTarget;
    [SerializeField]
    private Transform targetObject;
    [SerializeField]
    private Transform theCamera;
    private Vector3 targetVec;
    [SerializeField]
    private bool isStart = false;
    
    public bool IsStart
    {
        get
        {
            return isStart;
        }

        set
        {
            isStart = value;
        }
    }

    public Transform TargetObject
    {
        get
        {
            
            return targetObject;
        }

        set
        {
            Debug.Log("TargetObject localPos :" + value.localPosition);
            Debug.Log("TargetObject worldPos :" + value.position);
            targetObject = value;
            targetVec = value.position;
        }
    }

    public Transform TheCamera
    {
        get
        {
            return theCamera;
        }

        set
        {
            theCamera = value;
        }
    }

    public Transform RotateTarget
    {
        get
        {
            return rotateTarget;
        }

        set
        {
            rotateTarget = value;
        }
    }

    // Update is called once per frame
    void Update () {

        if (!IsStart) return;

        Vector3 targetPosition = TheCamera.transform.InverseTransformPoint(targetVec);
		Vector3 targetDirection = new Vector3(targetPosition.x, targetPosition.y, 0);
		targetDirection.Normalize();
		Vector3 right = TheCamera.transform.TransformDirection(targetDirection);
		Vector3 forward = TheCamera.transform.forward;
		Vector3 up = Vector3.Cross(forward, right);
        RotateTarget.rotation = Quaternion.LookRotation(forward, up);

	}
}
