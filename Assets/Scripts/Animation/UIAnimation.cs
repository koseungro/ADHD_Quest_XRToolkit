using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIAnimation : MonoBehaviour {

    private bool isStart;
    private bool isPlay;
    public bool setPlay { set { isPlay = value; } }
    //private const float kMillisecToSec = 0.001f;
    private DateTime mDateTime;
    public double GetElapsedTime
    {
        get
        {
            TimeSpan span = DateTime.Now - mDateTime;
            return span.TotalMilliseconds;
        }
    }
    private IEnumerator mMoveIE;    
    private IEnumerator mFadeIE;
    private IEnumerator mBlinkIE;
    private IEnumerator mTransToTransIE;
    private WaitForEndOfFrame EOF = new WaitForEndOfFrame();

    //private void Start()
    //{
        
    //}

    //private void OnEnable()
    //{
    //    Image image = GetComponent<Image>();
    //    Vector3 pos = image.transform.localPosition;
    //    UIMove(image.transform, pos, new Vector3(pos.x - 100, pos.y, pos.z), 1000);
    //    UIFadeOut(image, 1000);
    //}

    #region UIMove
    /// <summary>
    /// UI Transform Move
    /// </summary>
    /// <param name="target"></param>
    /// <param name="StartVec"></param>
    /// <param name="EndVec"></param>
    /// <param name="duration"></param>
    public void UIMove(Transform target, Vector3 StartVec, Vector3 EndVec, float duration)
    {
        if(mMoveIE != null)
        {
            isStart = false;
            StopCoroutine(mMoveIE);
        }
        //mMoveIE = UIMoveRoutine(target, StartVec, EndVec, duration);
        if(gameObject.activeSelf)
            StartCoroutine(UIMoveRoutine(target, StartVec, EndVec, duration));
    }

    private IEnumerator UIMoveRoutine(Transform target, Vector3 StartVec, Vector3 EndVec, double MillisecDuration)
    {
        //Debug.Log("Start Move");
        isStart = true;
        mDateTime = DateTime.Now;    
        while(GetElapsedTime < MillisecDuration)
        {
            float ratio = (float)(GetElapsedTime / MillisecDuration);
            target.localPosition = Vector3.Lerp(StartVec, EndVec, ratio);
            yield return EOF;
        }        
        target.localPosition = EndVec;        
        isStart = false;
    }
    #endregion

    #region FadeIn
    /// <summary>
    /// UI Fade In Function
    /// </summary>
    /// <param name="target"></param>
    /// <param name="MilliSecDuration"></param>
    public void UIFadeIn(Graphic target, double MilliSecDuration)
    {
        if(mFadeIE != null)
        {
            isStart = false;
            StopCoroutine(mFadeIE);
        }

        mFadeIE = UIFadeRoutine(target, target.color, new Color(1,1,1,1), MilliSecDuration);
        StartCoroutine(mFadeIE);
    }

    /// <summary>
    /// UI Fade Out Function
    /// </summary>
    /// <param name="target"></param>
    /// <param name="MilliSecDuration"></param>
    public void UIFadeOut(Graphic target, double MilliSecDuration, params UnityAction[] callback)
    {
        if(mFadeIE != null)
        {
            isStart = false;
            StopCoroutine(mFadeIE);
        }
        mFadeIE = UIFadeRoutine(target, target.color, new Color(1, 1, 1, 0), MilliSecDuration, callback);
        StartCoroutine(mFadeIE);
    }

    private IEnumerator UIFadeRoutine(Graphic target, Color StartColor, Color EndColor,  double MilliSecDuration, params UnityAction[] callback)
    {
        //Debug.Log("Start Fade");
        isStart = true;

        mDateTime = DateTime.Now;
        yield return EOF;
        while(GetElapsedTime < MilliSecDuration)
        {
            float ratio = (float)(GetElapsedTime / MilliSecDuration);
            target.color = Color.Lerp(target.color, EndColor, ratio);
            yield return EOF;
        }
        target.color = EndColor;
        isStart = false;
        for (int i = 0; i < callback.Length; i++)
        {
            if(callback[i] != null)
                callback[i]();
        }   
    }

    #endregion

    #region Blink
    public void UIBlink(List<Graphic> target, List<Color> targetColor)
    {
        if(mBlinkIE != null)
        {
            isStart = false;
            StopCoroutine(mBlinkIE);
        }
        mBlinkIE = UIBlinkRoutine(target, targetColor, 500f);
        StartCoroutine(mBlinkIE);
    }
    public void UIStopBlink(Graphic target)
    {
        if (mBlinkIE != null)
        {
            isStart = false;
            StopCoroutine(mBlinkIE);
        }
        target.color = new Color(1, 1, 1, 0);
        target.gameObject.SetActive(false);
    }
    private IEnumerator UIBlinkRoutine(List<Graphic> target, List<Color> targetColorList, float Term)
    {
        //Debug.Log("Start Fade");
        isStart = true;
        bool switcher = true;
        mDateTime = DateTime.Now;

        List<Color> ShowColorList = new List<Color>();
        List<Color> HideColorList = new List<Color>();

        for (int i = 0; i < targetColorList.Count; i++)
        {
            ShowColorList.Add(new Color(targetColorList[i].r, targetColorList[i].g, targetColorList[i].b, 1));
        }

        for (int i = 0; i < targetColorList.Count; i++)
        {
            HideColorList.Add(new Color(targetColorList[i].r, targetColorList[i].g, targetColorList[i].b, 0));
        }
        

        while (true)
        {
            float ratio = (float)(GetElapsedTime / Term);
            if (switcher)
            {                
                for(int i = 0; i < target.Count; i++)
                {
                    target[i].color = Color.Lerp(target[i].color, HideColorList[i], ratio);
                }
            }
            else
            {                
                for(int i = 0; i < target.Count; i++)
                {
                    //Debug.Log( i + " : "+ShowColorList[i]);
                    target[i].color = Color.Lerp(target[i].color, ShowColorList[i], ratio);
                }
            }

            if (ratio >= 1)
            {
                ratio = 0;
                switcher = !switcher;
                mDateTime = DateTime.Now;
            }
            yield return EOF;
        }
        for(int i = 0; i < target.Count; i++)
        {
            target[i].color = HideColorList[i];
            isStart = false;
        }
    }
    #endregion

    #region TransToTrans
    /// <summary>
    /// 왕복운동하는 애니메이션
    /// </summary>
    /// <param name="target"></param>
    /// <param name="startVec"></param>
    /// <param name="endVec"></param>
    /// <param name="Term"></param>
    public void UITransToTrans(Transform target, Vector3 startVec, Vector3 endVec, double Term)
    {
        if (isPlay) return;
        if (mTransToTransIE != null)
        {
            isStart = false;
            StopCoroutine(mTransToTransIE);
        }
        //mTransToTransIE = TransToTransRoutine(target, startVec, endVec, Term);
        StartCoroutine(TransToTransRoutine(target, startVec, endVec, Term));
    }
    public void UIStopTransToTrans(Transform target, Vector3 initPos)
    {
        if (mTransToTransIE != null)
        {
            isStart = false;
            isPlay = false;
            StopCoroutine(mTransToTransIE);
        }
        target.localPosition = initPos;

    }
    IEnumerator TransToTransRoutine(Transform target, Vector3 startVec, Vector3 endVec, double Term)
    {
        bool switcher = true;
        mDateTime = DateTime.Now;
        isPlay = true;
        while (isPlay)
        {                
            float ratio = (float)(GetElapsedTime / Term);
            if (switcher)
            {
                target.localPosition = Vector3.Lerp(startVec, endVec, ratio);
            }
            else
            {
                target.localPosition = Vector3.Lerp(endVec, startVec, ratio);
            }

            if (ratio >= 1)
            {
                ratio = 0;
                switcher = !switcher;
                mDateTime = DateTime.Now;
            }
            yield return EOF;
        }
        target.localPosition = startVec;
    }

    #endregion
}
