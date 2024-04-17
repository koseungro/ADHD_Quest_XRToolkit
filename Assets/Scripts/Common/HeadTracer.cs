using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTracer : Singleton<HeadTracer>
{

    /// <summary>
    /// 1초 이상 바라보고 있는 AOI 대상
    /// </summary>
    public AOIType type;
    public float rayDist = 100.0f;
    /// <summary>
    /// 지속시간
    /// </summary>
    public const float kLookTerm = 1.0f;

    /// <summary>
    /// 바라본 시간
    /// </summary>
    public float LookingTimedelta = 0.0f;

    /// <summary>
    /// AOI 물체를 바라본 시간
    /// </summary>
    public DateTime LookingTime;

    public bool isLooking = false;    
    public bool isStart = false;
    
    private float TotalTime = 0.0f;

    private IEnumerator AOICheckIE;

    public string ToLookingTime
    {
        get
        {
            return LookingTime.ToString("yyyy-MM-dd_HH:mm:ss.ff");
        }
    }

    public bool Start
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

    public void StartAOI()
    {
        type = AOIType.NONE;
        LookingTimedelta = 0.0f;
        LookingTime = new DateTime();
        isLooking = false;
        isStart = true;
        TotalTime = 0.0f;

        CheckAOI();

        //if(line != null)
        //{
        //    line = GetComponent<LineRenderer>();
        //    line.startColor = Color.red;
        //    line.startWidth = 0.05f;
        //    line.positionCount = 2;
        //    line.useWorldSpace = false;
        //    line.SetPosition(0, new Vector3(-0.1f, 0, 0.5f));
        //    line.SetPosition(1, new Vector3(0, 0, 50));
        //}
    }
    public void EndAOI()
    {
        isStart = false;
        // AOI가 NONE이 아니면 보고있는데 종료 된 것이므로
        if (type != AOIType.NONE)
        {
            AddAOI(type, DataManager.Inst.CurrentLevel, LookingTimedelta, ToLookingTime, DataManager.Inst.GetTotalTime);
            type = AOIType.NONE;
        }
        isLooking = false;
        LookingTimedelta = 0.0f;
        TotalTime = 0.0f;
    }

    private void CheckAOI()
    {
        if (!gameObject.activeInHierarchy) return;

        AOICheckIE = AOICheckRoutine();
        StartCoroutine(AOICheckIE);
    }

    private IEnumerator AOICheckRoutine()
    {
        WaitForEndOfFrame wfs = new WaitForEndOfFrame();

        while (isStart)
        {
            //TotalTime += Time.deltaTime;
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            Debug.DrawRay(transform.position, transform.forward, Color.red);

            //UIManager.Inst.ShowAndHideTestText("", false);        
            // Ray에 뭔가 닿았다.
            if (Physics.Raycast(ray, out hit, rayDist))
            {
                //UIManager.Inst.ShowAndHideTestText(hit.collider.tag, true);
                if (!isLooking)
                {
                    isLooking = true;
                    LookingTime = DateTime.Now;
                }
                // 1초 이상 바라봤나?
                if (LookingTimedelta > kLookTerm)
                {
                    // 1초이상 바라보면 그 바라본 녀석에 대해 바라본 시간 증가
                    //LookingTimedelta += Time.deltaTime;
                    // 1초이상 봤는데 그 바라본 대상의 AOI_Type은 뭔가?
                    if (hit.collider.CompareTag("AOI_1"))
                    {
                        type = AOIType.AOI1;
                    }
                    else if (hit.collider.CompareTag("AOI_2"))
                    {
                        type = AOIType.AOI2;
                    }
                }
                // 아니면 바라본 시간 증가
                else
                {

                }
                //LookingTimedelta += 1.0f;
                LookingTimedelta += Time.deltaTime;
            }
            // Ray에 닿지 않는다면
            else
            {
                if (type != AOIType.NONE)
                {
                    AddAOI(type, DataManager.Inst.CurrentLevel, LookingTimedelta, ToLookingTime, DataManager.Inst.GetTotalTime);
                    type = AOIType.NONE;
                }
                isLooking = false;
                LookingTimedelta = 0.0f;
            }

            yield return wfs;
        }
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
        if (!isStart)
            return;

        //TotalTime += Time.deltaTime;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        //UIManager.Inst.ShowAndHideTestText("", false);        
        // Ray에 뭔가 닿았다.
        if (Physics.Raycast(ray, out hit, rayDist))
        {   
            //UIManager.Inst.ShowAndHideTestText(hit.collider.tag, true);
            if (!isLooking)
            {
                isLooking = true;
                LookingTime = DateTime.Now;
            }
            // 1초 이상 바라봤나?
            if (LookingTimedelta > kLookTerm)
            {
                // 1초이상 바라보면 그 바라본 녀석에 대해 바라본 시간 증가
                //LookingTimedelta += Time.deltaTime;
                // 1초이상 봤는데 그 바라본 대상의 AOI_Type은 뭔가?
                if(hit.collider.CompareTag("AOI_1"))
                {
                    type = AOIType.AOI1;
                }
                else if (hit.collider.CompareTag("AOI_2"))
                {
                    type = AOIType.AOI2;
                }                    
            }
            // 아니면 바라본 시간 증가
            else
            {                
                
            }
            LookingTimedelta += Time.deltaTime;
        }
        // Ray에 닿지 않는다면
        else 
        {
            if(type != AOIType.NONE)
            {
                AddAOI(type, DataManager.Inst.CurrentLevel, LookingTimedelta, ToLookingTime, DataManager.Inst.GetTotalTime);
                type = AOIType.NONE;
            }
            isLooking = false;
            LookingTimedelta = 0.0f;
        }
    }
    */
    public void AddAOI(AOIType type, string id, float LookingTimedelta, string startTime, float totalTime)
    {
        DataManager.Inst.AddAOIValue(id, type, LookingTimedelta, startTime, totalTime);
    }


    // Use this for initialization
    //void Start () {
		
	//}
	
	
}
