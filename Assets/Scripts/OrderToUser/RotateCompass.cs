using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCompass : Singleton<RotateCompass>
{

    private bool isRotateStart = false;

    public float ViewAngle = 50.0f;
    public float ViewDistance = Mathf.Infinity;
    /// <summary>
    /// 지시방향 화살표 상위 객체
    /// </summary>
    private Transform CompassTR;
    /// <summary>
    /// 지시방향 회전하는 루틴
    /// </summary>
    private IEnumerator RotateCompassIE;

    private DirectionArrow dirArrow;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        gameObject.SetActive(false);
        ViewAngle = 5.0f;
    }

    // Use this for initialization
    void Start()
    {

        CompassTR = transform.Find("Compass").transform;
    }

    public void StartRotate(Transform targetVec)
    {
        CompassTR = transform.Find("Compass").transform;
        dirArrow = GetComponent<DirectionArrow>();
        if (RotateCompassIE != null)
        {
            StopCoroutine(RotateCompassIE);
        }
        RotateCompassIE = RotateRoutine(targetVec);
        isRotateStart = true;
        gameObject.SetActive(true);
        StartCoroutine(RotateCompassIE);
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        //탱크의 좌우 회전값 갱신
        angleInDegrees += Camera.main.transform.eulerAngles.y;
        //경계 벡터값 반환
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawView()
    {
        Transform camera = Camera.main.transform;
        Vector3 leftBoundary = DirFromAngle(-ViewAngle * 0.5f);
        Vector3 rightBoundary = DirFromAngle(ViewAngle * 0.5f);
        Debug.DrawLine(camera.position, camera.position + leftBoundary * ViewDistance, Color.blue);
        Debug.DrawLine(camera.position, camera.position + rightBoundary * ViewDistance, Color.blue);
    }

    private bool CheckAngle(Transform target)
    {
//        if (Camera.main == null) return false;
//        Transform CameraTR;
//#if UNITY_EDITOR
//        //CameraTR = FindObjectOfType<OVRCameraRig>().transform;
//#else
//        CameraTR = Camera.main.transform;
//#endif
//        //Debug.Log("dot : " + Vector3.Dot(CameraTR.forward, target.position));
//        //Debug.Log("Range : " + Mathf.Cos((ViewAngle / 2) * Mathf.Deg2Rad));

//        //Vector3 inverseCameraDir = CameraTR.InverseTransformDirection(CameraTR.forward);
//        //Vector3 inverseTargetDir = CameraTR.InverseTransformDirection(target.forward);

//        float dot = Vector3.Dot(inverseCameraDir, inverseTargetDir);
//        float angle = Mathf.Acos(dot);

//        //Debug.Log("dot : " + dot);
//        //Debug.Log("angle : " + angle * Mathf.Rad2Deg);
//        angle = angle * Mathf.Rad2Deg;
//        if (angle < ViewAngle)
//        {
//            return true;
//        }
        return true;
    }

    //private Vector3 TargetQu(Transform targetVec)
    //{
    //    Vector3 vec = targetVec - Camera.main.transform.position;
    //    //vec.Normalize();
    //    Quaternion q = Quaternion.LookRotation(vec);
    //    Vector3 up = new Vector3(0, 0, q.eulerAngles.z);
    //    up.Normalize();
    //    return up;
    //}

    private IEnumerator RotateRoutine(Transform targetTR)
    {
        // 메인 카메라가 Null인지 체크
        if (Camera.main == null) yield break;

        dirArrow.TargetObject = targetTR;
        //Transform camera = FindObjectOfType<OVRCameraRig>().transform;

#if UNITY_EDITOR
        //dirArrow.TheCamera = camera;
#else
        dirArrow.TheCamera = Camera.main.transform;
#endif

        dirArrow.RotateTarget = CompassTR;
        // 시야각 안에 있는지 체크
        bool Check = CheckAngle(targetTR);
        // 시야각 안
        if (Check)
        {

            Debug.Log("GetAngle(targetVec) : " + CheckAngle(targetTR));
            Debug.Log("Get Angle1");
        }
        // 시야각 밖
        else
        {
            DrawView();
            gameObject.SetActive(true);
            // 화살표 회전
            while (isRotateStart)
            {
                dirArrow.IsStart = true;
                bool NewCheck = CheckAngle(targetTR);
                if (NewCheck) break;
                //Debug.Log("NewAngle : " + NewCheck);
                //if (CompassTR == null) yield return null;
                //CompassTR.transform.rotation = TargetQu(targetVec);
                //CompassTR.transform.e (TargetQu(targetVec));
                yield return null;
            }
        }

        Debug.Log("Get Angle2");
        // 시야각 안에 도달

        // 종료
        isRotateStart = false;
        gameObject.SetActive(false);
        targetTR.gameObject.SetActive(true);
        yield return null;
    }
}
