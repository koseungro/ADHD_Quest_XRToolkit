using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawOrder : Singleton<DrawOrder> {

    
    
    //private Canvas m_Canvas;
    //EventTrigger trigger;

    protected override void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        //    m_Canvas = GetComponent<Canvas>();
        //    trigger = GetComponent<EventTrigger>();

        //InitEventTrigger();
    }

    private void InitEventTrigger()
    {
        //trigger.triggers.Clear();

        //EventTrigger.Entry entry = new EventTrigger.Entry();
        //entry.eventID = EventTriggerType.PointerUp;
        //entry.callback.AddListener((data) => { DrawOrderEvent((PointerEventData)data); });
        //trigger.triggers.Add(entry);
    }

    public void DrawOrderEvent(PointerEventData data)
    {
        Debug.Log("PointerUp");
    }
    
    public Vector3 ShootRay()
    {
        
        Vector3 StartVec = Vector3.forward;
        Vector3 dirVec = Vector3.forward;
        if (Camera.main == null)
        {
            Debug.Log("Camera Main is null");
        }
        if (Camera.main != null)
        {
            //Debug.Log("Camera.main : " + Camera.main.name);
            StartVec = Camera.main.transform.position;
            dirVec = Camera.main.transform.forward;
            //dirVec = Camera.main.transform.InverseTransformDirection(Camera.main.transform.forward);
        }

        Debug.Log("dirVec : " + dirVec);

        DirectionEffect effect = Instantiate(DataManager.Inst.DirectionEffectPrefab, Vector3.zero, Quaternion.identity);
        SetParentToTarget(null, effect.transform, dirVec, true);
        //Ray ray = new Ray(StartVec, transform.TransformDirection(dirVec));

        //RaycastHit hit;

        //if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        //{
        //    Debug.DrawRay(StartVec, dirVec, Color.yellow, Mathf.Infinity);

        //    if (hit.transform != null)
        //    {
        //        Debug.Log(hit.collider.name);
        //        GameObject canvas = GameObject.Find("Canvas_FixedUI");

        //        if (canvas != null)
        //        {
        //            DirectionEffect effect = Instantiate(DataManager.Inst.DirectionEffectPrefab, Vector3.zero, Quaternion.identity);
        //            SetParentToTarget(null, effect.transform, hit.point, true);
        //        }
        //        else
        //        {
        //            DirectionEffect effect = Instantiate(DataManager.Inst.DirectionEffectPrefab, Vector3.zero, Quaternion.identity);
        //            SetParentToTarget(null, effect.transform, hit.point, false);
        //        }
        //        return hit;
        //    }
        //}
        //Debug.Log("hit is Null");
        return dirVec;
    }

    private void SetParentToTarget(Transform target, Transform child, Vector3 ToPos, bool isCanvas = true)
    {
        if(target != null)
            child.SetParent(target);

        Debug.Log("ToPos : " + ToPos);

        if (GameObject.Find("Player") != null)
        {
            Transform playerGO = GameObject.Find("Player").transform;
            child.SetParent(playerGO);
            //child.localPosition = Vector3.zero;
            //child.localScale = Vector3.one;
            //child.rotation = Quaternion.identity;

            Debug.Log("playerGO.rotation : " + playerGO.eulerAngles);
            Debug.Log("playerGO.localrotation : " + playerGO.localEulerAngles);

            if (playerGO.eulerAngles.y >= 180)
            {
                Debug.Log("180 이상");
                child.localPosition = new Vector3(ToPos.x * -1, ToPos.y, ToPos.z * -1);
                //child.localRotation = ToPos * Quaternion.Euler(-1, -1, -1);
            }
            else
            {
                Debug.Log("180 이하");
                if (DataManager.Inst.CurrentLevel.Equals("LibraryLevel3") || DataManager.Inst.CurrentLevel.Equals("SceneLounge"))
                {
                    Debug.Log("is SceneLounge");
                    child.localPosition = ToPos;// new Vector3(ToPos.x * -1, ToPos.y, ToPos.z * -1);
                }
                else
                    child.position = ToPos;
            }
        }
        else
        {
            child.position = ToPos;
        }

        Vector3 vec = child.transform.position - Camera.main.transform.position;
        vec.Normalize();
        Quaternion q = Quaternion.LookRotation(vec);
        child.transform.rotation = q;

        //var client = FindObjectOfType<AsyncClient>();

        //if (client.isServer)
        //{
        //    child.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //    //if (isCanvas)
        //    //    child.localScale = new Vector3(100, 100, 1);
        //    //else
        //    //{
        //    //    if(ToPos.z < 10)
        //    //    {
        //    //        child.localScale = new Vector3(0.5f, 0.5f, 1);
        //    //    }
        //    //    else
        //    //    {
        //    //        child.localScale = new Vector3(4, 4, 1);
        //    //    }

        //    //}

        //}
        //else
        //{
        //    child.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //    //if (isCanvas)
        //    //    child.localScale = new Vector3(0.7f, 0.7f, 1);
        //    //else
        //    //{
        //    //    if(ToPos.z < 10)
        //    //    {
        //    //        child.localScale = new Vector3(0.5f, 0.5f, 1);
        //    //    }
        //    //    else
        //    //        child.localScale = new Vector3(5, 5, 1);
        //    //}
        //}
    }

}
