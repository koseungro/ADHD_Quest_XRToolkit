using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using CurvedUI;

public class UIManager : Singleton<UIManager>
{

    /// <summary>
    /// 이전 UI CanvasGroup
    /// </summary>
    private CanvasGroup PreCanvasGroup;
    private int UILevel;
    public int GetSetUILevel
    {
        get { return UILevel; }
        set { UILevel = value; }
    }
    /// <summary>
    /// 반투명한 검정색 바탕의 캔버스
    /// </summary>
    private CanvasGroup HalfFade;
    /// <summary>
    /// 불투명 검정색 바탕의 캔버스( UI BG를 검은색으로 하기 위해 )
    /// </summary>
    private CanvasGroup FullFade2;
    /// <summary>
    /// 완전 검정색 바탕의 캔버스
    /// </summary>
    private CanvasGroup FullFade;

    private Canvas CanvasFixedUI;

    private AudioSource Narration;
    private AudioSource EffectSource;
    private Text NarrationText;
    private Transform NarrationBG;

    /// <summary>
    /// 버튼 데이터 Dictionary 해당 Scene의 버튼 데이터(버튼 텍스트, 이미지 등)을 가지고 있음
    /// 해당 Scene의 게임매니저 이름이 메인 폴더이고 진행 순서의 Canvas 이름으로 하위 폴더 구분
    /// </summary>
    private Dictionary<string, CreateButtonData[]> ButtonData;
    private List<CreateButtonData> KeyData;
    private List<AudioClip> narrations = new List<AudioClip>();
    private List<AudioClip> CommonNarration = new List<AudioClip>();
    private List<AudioClip> EffectList = new List<AudioClip>();
    private UnityAction SceneExitAction;

    private IEnumerator WaitForNarrationAndFuncIE;
    private IEnumerator WaitSecondsIE;
    private IEnumerator CompleteAndCallbackIE;
    private bool IsWaitForSecondsStop = false;

    /// <summary>
    /// 화면에 지시점 표시 객체
    /// </summary>
    private DrawOrder m_Director;
    /// <summary>
    /// 지시 캔버스
    /// </summary>
    private RotateCompass RotateCompass;

    protected override void Awake()
    {
        base.Awake();
        InitDrawOrder();
        Transform Parent = GameObject.Find("XR Origin/Camera Offset/Main Camera/Camera_UI").transform;
        HalfFade = Parent.Find("Canvas_Head/HalfFade").GetComponent<CanvasGroup>();
        FullFade2 = Parent.Find("Canvas_Head/FullFade2").GetComponent<CanvasGroup>();
        FullFade = Parent.Find("Canvas_Fade/FullFade").GetComponent<CanvasGroup>();

        if (GameObject.Find("Canvas_FixedUI") != null)
            CanvasFixedUI = GameObject.Find("Canvas_FixedUI").GetComponent<Canvas>();

        Narration = GetComponent<AudioSource>();
        EffectSource = gameObject.AddComponent<AudioSource>();
        EffectSource.transform.SetParent(transform);
        LoadEffectSoundClip();

        if (GameObject.Find("NarrationText") != null)
            NarrationText = GameObject.Find("NarrationText").GetComponent<Text>() == null ? null : GameObject.Find("NarrationText").GetComponent<Text>();
        if (NarrationText != null)
            if (NarrationText.transform.parent != null)
            NarrationBG = NarrationText.transform.parent;

        ButtonData = new Dictionary<string, CreateButtonData[]>();
        KeyData = new List<CreateButtonData>();
        LoadCommonNarrationClip();
        LoadKeyBoardData();
        FindRotateCompass();
        //var client = FindObjectOfType<AsyncClient>();

        //if(client.isServer)
        //{
        //    RotateCompass = FindObjectOfType<RotateCompass>();
        //    RotateCompass.gameObject.SetActive(false);
        //}

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //if (CanvasFixedUI != null)
        //{
        //    CurvedUISettings settings = CanvasFixedUI.GetComponent<CurvedUISettings>();
        //    settings.ControlMethod = CurvedUIInputModule.CUIControlMethod.MOUSE;
        //}
#else
        if(CanvasFixedUI != null)
        {
            CurvedUISettings settings = CanvasFixedUI.GetComponent<CurvedUISettings>();
            settings.ControlMethod = CurvedUIInputModule.CUIControlMethod.OCULUSVR;
        }
#endif
    }
    
    /// <summary>
    /// 지시 화살표 컴포넌트 찾기
    /// </summary>
    public void FindRotateCompass()
    {
        //var client = FindObjectOfType<AsyncClient>();
        //if (client == null) return;
        
        RotateCompass = RotateCompass = RotateCompass.Inst;
        if (RotateCompass != null)
        {
            RotateCompass.gameObject.SetActive(false);
        }  
    }

    /// <summary>
    /// 지시 화살표 회전 시작
    /// </summary>
    /// <param name="targetVec"></param>
    public void StartRotateCompass(Transform targetVec)
    {
        if (RotateCompass == null)
        {
            Debug.Log("is RotateCompass null");
            return;
        }
        else
        {
            RotateCompass.gameObject.SetActive(true);
            RotateCompass.StartRotate(targetVec);
        }
    }

    /// <summary>
    /// 지시점 표시하는 클래스 할당 및 프리팹 할당
    /// </summary>
    private void InitDrawOrder()
    {
        m_Director = GetComponent<DrawOrder>();
        //m_oriDirectionEffectPrefab = Resources.Load<DirectionEffect>("Prefabs/Effect/DirectionEffectPrefab");
    }


    /// <summary>
    /// 버튼에 대한 데이터 로드
    /// </summary>
    /// <param name="key">해당 씬의 캔버스 오브젝트 이름</param>
    /// <param name="path">해당 씬의 게임 매니저 이름 + 하위 캔버스 이름</param>
    public void LoadButtonData(string key, string path)
    {
        if (!ButtonData.ContainsKey(key))
            ButtonData.Add(key, Resources.LoadAll<CreateButtonData>(string.Format("ScriptableData/Button/{0}", path)));
        //AddRange(Resources.LoadAll<CreateButtonData>( string.Format("ScripableData/Button/{0}", path)));
    }
    public void LoadKeyBoardData()
    {
        CreateButtonData[] array = Resources.LoadAll<CreateButtonData>("ScriptableData/Button/SceneMainManager/KeyBoard/");
        for (int i = 0; i < array.Length; i++)
            KeyData.Add(array[i]);        
    }
    public void GetKeyData(ref ButtonFunc btn, string name)
    {
        CreateButtonData temp = KeyData.Find(x => x.name.Equals(name));
        btn.IsFunctionKey = temp.isFunctionKey;
        btn.GetSetImage.sprite = temp.DefaultImage;
        btn.DefaultSprite = temp.DefaultImage;
        btn.PressedSprite = temp.PressedImage;
        btn.HoverSprite = temp.HoverImage;
    }

    public void GetKorean(ref ButtonFunc btn, string name)
    {
        CreateButtonData temp = KeyData.Find(x => x.name.Equals(name));
        btn.GetSetText = temp.Korean;
    }
    public void GetEnglish(ref ButtonFunc btn, string name)
    {
        CreateButtonData temp = KeyData.Find(x => x.name.Equals(name));
        btn.GetSetText = temp.English;
    }
    public void GetShiftWord(ref ButtonFunc btn, string name, bool isKor = true)
    {
        CreateButtonData temp = KeyData.Find(x => x.name.Equals(name));
        if (isKor)
        {
            btn.GetSetText = temp.ShiftKOR;
        }
        else
        {
            btn.GetSetText = temp.ShiftENG;
        }
    }
    public void LoadCommonNarrationClip()
    {
        AudioClip[] temp = Resources.LoadAll<AudioClip>("Sound/Common/");
        for (int i = 0; i < temp.Length; i++)
            CommonNarration.Add(temp[i]);
    }
    public void LoadNarrationClip(string path)
    {
        AudioClip[] temp = Resources.LoadAll<AudioClip>(string.Format("Sound/{0}/", path));
        for (int i = 0; i < temp.Length; i++)
            narrations.Add(temp[i]);
    }
    public void LoadEffectSoundClip()
    {
        AudioClip[] temp = Resources.LoadAll<AudioClip>("Sound/Effect/");
        for (int i = 0; i < temp.Length; i++)
            EffectList.Add(temp[i]);
    }
    public AudioClip FindEffectSound(string name)
    {
        AudioClip clip = EffectList.Find(x => x.name == name);

        return clip;
    }
    public AudioClip[] FindAllEffectSound(string name)
    {
        AudioClip[] clip = EffectList.FindAll(x => x.name.Contains(name)).ToArray();

        return clip;
    }
    public void PlayEffectSound(AudioClip clip)
    {
        if (EffectSource.isPlaying)
            EffectSource.Stop();
        EffectSource.clip = clip;
        EffectSource.Play();
    }
    /// <summary>
    /// 로드된 버튼 데이터를 key를 가져와 버튼 데이터 셋팅
    /// </summary>
    /// <param name="key">해당 씬에 자기 상위 캔버스 이름</param>
    /// <param name="InputButtons">매개변수의 ButtonFunc를 params으로 넣어줌</param>
    public void SettingButtonData(string key, params ButtonFunc[] InputButtons)
    {
        // 버튼데이터 안에 해당 키가 존재하는지 확인
        if (ButtonData.ContainsKey(key))
        {
            CreateButtonData[] Temp = ButtonData[key];

            // 존재하면 for문을 돌면서 데이터 셋팅
            for (int i = 0; i < InputButtons.Length; i++)
            {
                for (int j = 0; j < Temp.Length; j++)
                {
                    if (InputButtons[i].name.Equals(Temp[j].name))
                    {
                        InputButtons[i].buttonType = Temp[j].buttonType;
                        InputButtons[i].NewDefaultColor = Temp[j].NewDefaultColor;
                        InputButtons[i].NewEnterColor = Temp[j].NewEnterColor;
                        InputButtons[i].NewPressedColor = Temp[j].NewPressedColor;
                        InputButtons[i].NewTextPressedColor = Temp[j].NewTextPressedColor;
                        InputButtons[i].NewTextDefaultColor = Temp[j].NewTextDefaultColor;
                        InputButtons[i].NewTextEnterColor = Temp[j].NewTextEnterColor;

                        InputButtons[i].NewButtonText = Temp[j].ButtonText;
                        InputButtons[i].SceneNumber = Temp[j].SceneNumber;
                        InputButtons[i].SceneName = Temp[j].SceneName;
                        InputButtons[i].isScaleChange = Temp[j].isScaleChange;

                        InputButtons[i].Allocate();

                        //if (Temp[j].isScaleChange)
                        //{
                        //    InputButtons[i].mScaleImage.sprite = Temp[j].DefaultImage;
                        //    InputButtons[i].PressedSprite = Temp[j].PressedImage;
                        //    InputButtons[i].HoverSprite = Temp[j].HoverImage;
                        //    InputButtons[i].ChangeAlpha(InputButtons[i].mScaleImage, 0.8f);
                        //    InputButtons[i].ChangeAlpha(InputButtons[i].mText, 0.8f);
                        //    InputButtons[i].ChangeScale(InputButtons[i].mScaleImage.transform, 0.8f);
                        //    InputButtons[i].ChangeScale(InputButtons[i].mText.transform, 0.8f);
                        //}
                        //else
                        //{
                        //    InputButtons[i].GetSetImage.sprite = Temp[j].DefaultImage;
                        //    InputButtons[i].DefaultSprite = Temp[j].DefaultImage;
                        //    InputButtons[i].PressedSprite = Temp[j].PressedImage;
                        //    InputButtons[i].HoverSprite = Temp[j].HoverImage;
                        //}
                    }
                }
            }
        }
    }

    /// <summary>
    /// 특정 UI를 FadeIn 하기 위한 함수
    /// </summary>
    /// <param name="Target">타겟 캔버스</param>
    /// <param name="totalTime">총 걸리는 시간</param>
    /// <param name="receiver">FadeIn이 끝난 후 Callback함수</param>
    public void FadeIn(CanvasGroup Target, float totalTime = 1, params UnityAction[] receiver)
    {
        StartCoroutine(FadeInRoutine(Target, totalTime, receiver));
    }
    /// <summary>
    /// 특정 UI를 FadeIn 하기 위한 함수
    /// </summary>
    /// <param name="Target">타겟 캔버스</param>
    /// <param name="totalTime">총 걸리는 시간</param>
    /// <param name="receiver">FadeOut이 끝난 후 Callback함수</param>
    public void FadeOut(CanvasGroup Target, float totalTime = 1, params UnityAction[] receiver)
    {
        StartCoroutine(FadeOutRoutine(Target, totalTime, receiver));
    }
    /// <summary>
    /// HalfFade 캔버스를 FadeIn 하는 함수
    /// </summary>
    /// <param name="receiver">FadeIn이 끝난 후 실행 할 CallBack함수</param>
    public void HalfFadeIn(params UnityAction[] receiver)
    {
        StartCoroutine(HalfOrFullFadeInRoutine(HalfFade, receiver));
    }
    /// <summary>
    /// HalfFade 캔버스를 FadeOut 하는 함수
    /// </summary>
    /// <param name="receiver">FadeOut이 끝난 후 실행 할 CallBack함수</param>
    public void HalfFadeOut(params UnityAction[] receiver)
    {
        StartCoroutine(HalfOrFullFadeOutRoutine(HalfFade, receiver));
    }
    /// <summary>
    /// FullFade 캔버스를 FadeIn 하는 함수
    /// </summary>
    /// <param name="receiver">FadeIn이 끝난 후 실행 할 CallBack함수</param>
    public void FullFadeIn(params UnityAction[] receiver)
    {
        StartCoroutine(HalfOrFullFadeInRoutine(FullFade, receiver));
    }
    /// <summary>
    /// FullFade 캔버스를 FadeOut 하는 함수
    /// </summary>
    /// <param name="receiver">FadeIn이 끝난 후 실행 할 CallBack함수</param>
    public void FullFadeOut(params UnityAction[] receiver)
    {

        StartCoroutine(HalfOrFullFadeOutRoutine(FullFade, receiver));
    }

    /// <summary>
    /// HalfFade 캔버스를 FadeIn 하는 함수
    /// </summary>
    /// <param name="receiver">FadeIn이 끝난 후 실행 할 CallBack함수</param>
    public void HalfFadeIn(bool isQuick, params UnityAction[] receiver)
    {
        if (isQuick)
        {
            StartCoroutine(QuickHalfOrFullFadeInRoutine(HalfFade, receiver));
        }
        else
        {
            StartCoroutine(HalfOrFullFadeInRoutine(HalfFade, receiver));
        }

    }
    /// <summary>
    /// HalfFade 캔버스를 FadeOut 하는 함수
    /// </summary>
    /// <param name="receiver">FadeOut이 끝난 후 실행 할 CallBack함수</param>
    public void HalfFadeOut(bool isQuick, params UnityAction[] receiver)
    {
        if (isQuick)
        {
            StartCoroutine(QuickHalfOrFullFadeOutRoutine(HalfFade, receiver));
        }
        else
        {
            StartCoroutine(HalfOrFullFadeOutRoutine(HalfFade, receiver));
        }

    }
    /// <summary>
    /// FullFade 캔버스를 FadeIn 하는 함수
    /// </summary>
    /// <param name="receiver">FadeIn이 끝난 후 실행 할 CallBack함수</param>
    public void FullFadeIn(bool isQuick, params UnityAction[] receiver)
    {
        if (isQuick)
        {
            StartCoroutine(QuickHalfOrFullFadeInRoutine(FullFade, receiver));
        }
        else
        {
            StartCoroutine(HalfOrFullFadeInRoutine(FullFade, receiver));
        }

    }
    /// <summary>
    /// FullFade 캔버스를 FadeOut 하는 함수
    /// </summary>
    /// <param name="receiver">FadeIn이 끝난 후 실행 할 CallBack함수</param>
    public void FullFadeOut(bool isQuick, params UnityAction[] receiver)
    {
        if (isQuick)
        {
            StartCoroutine(QuickHalfOrFullFadeOutRoutine(FullFade, receiver));
        }
        else
        {
            StartCoroutine(HalfOrFullFadeOutRoutine(FullFade, receiver));
        }

    }

    /// <summary>
    /// 일정 시간 지난 후 CallBack함수 실행
    /// </summary>
    /// <param name="time">기다리는 시간</param>
    /// <param name="receiver">기다린 후 실행 할 CallBack함수</param>
    public void WaitSeconds(float time, params UnityAction[] receiver)
    {
        if (WaitSecondsIE != null)
            WaitSecondsIE = null;
        IsWaitForSecondsStop = false;
        WaitSecondsIE = WaitSecondsRoutine(time, receiver);
        StartCoroutine(WaitSecondsIE);
    }
    public void StopWaitSeconds()
    {
        if (WaitSecondsIE != null)
        {
            IsWaitForSecondsStop = true;
            Debug.Log("UIManager.StopWaitSeconds()");
            StopCoroutine(WaitSecondsIE);
        }
    }
    /// <summary>
    /// CanvasGroup 매개변수의 Alpha =1, Interactable = true, SetActive(true) 함수
    /// </summary>
    /// <param name="target">타겟 CanvasGroup</param>
    public void ShowAndInteractableCanvasGroup(CanvasGroup target)
    {
        target.alpha = 1.0f;
        target.interactable = true;
        target.blocksRaycasts = true;
        target.gameObject.SetActive(true);
    }
    /// <summary>
    /// CanvasGroup 매개변수의 Alpha =0, Interactable = false, SetActive(false) 함수
    /// </summary>
    /// <param name="target">타겟 CanvasGroup</param>
    public void HideAndInteractableCanvasGroup(params CanvasGroup[] target)
    {
        if (SceneExitAction != null)
            SceneExitAction.Invoke();

        for (int i = 0; i < target.Length; i++)
        {
            target[i].alpha = 0;
            target[i].interactable = false;
            target[i].blocksRaycasts = false;
            target[i].gameObject.SetActive(false);
        }
    }
    public void SetFullFade2(bool active, float value)
    {
        FullFade2.gameObject.SetActive(active);
        FullFade2.interactable = active;
        FullFade2.alpha = value;
    }
    public void DontTouch(params CanvasGroup[] target)
    {
        for (int i = 0; i < target.Length; i++)
        {
            if (target[i] != null)
            {
                target[i].interactable = false;
                target[i].blocksRaycasts = false;
            }
        }
    }
    /// <summary>
    /// 나레이션과 자막이 나온 뒤 Callback함수 실행
    /// </summary>
    /// <param name="clip">나레이션</param>
    /// <param name="subTitle">자막</param>
    /// <param name="receiver">Callback</param>
    public void WaitForNarrationAndFunc(AudioClip clip, string subTitle, bool isSkip, params UnityAction[] receiver)
    {
        WaitForNarrationAndFuncIE = WaitForNarrationAndFuncRoutine(clip, subTitle, isSkip, receiver);
        StartCoroutine(WaitForNarrationAndFuncIE);
    }
    public void StopWaitForNarrationAndFunc()
    {
        if (WaitForNarrationAndFuncIE == null) return;
        //while (WaitForNarrationAndFuncIE.MoveNext())
        //{            

        //}

        if (WaitForNarrationAndFuncIE != null)
        {
            StopCoroutine(WaitForNarrationAndFuncIE);
        }
    }

    /// <summary>
    /// 나레이션 찾기
    /// </summary>
    /// <param name="name">AudioClip 이름</param>
    /// <returns></returns>
    public AudioClip FindNarration(string name)
    {
        AudioClip clip = narrations.Find(x => x.name == name);

        if (clip == null)
        {
            clip = CommonNarration.Find(x => x.name == name);
        }
        return clip;
    }

    public void CompleteAndCallback(Func<bool> isEnd, params UnityAction[] receiver)
    {
        if (CompleteAndCallbackIE != null)
            CompleteAndCallbackIE = null;
        CompleteAndCallbackIE = CompleteAndCallbackRoutine(isEnd, receiver);
        StartCoroutine(CompleteAndCallbackIE);
    }
    public void StopCompleteAndCallback()
    {
        if (CompleteAndCallbackIE != null)
        {
            StopCoroutine(CompleteAndCallbackIE);
            CompleteAndCallbackIE = null;
        }
    }

    public void SkipNarration()
    {
        if (Narration.isPlaying)
            Narration.Stop();
        NarrationText.text = "";
        NarrationText.gameObject.SetActive(false);
    }
    public void SkipNarrationSound()
    {
        if (Narration.isPlaying)
            Narration.Stop();
    }
    public void StopNarration()
    {
        if (Narration.isPlaying)
            Narration.Stop();
        Narration.Stop();
    }
    public void ShowNarrationText(string text)
    {
        NarrationText.text = "";
        if (NarrationBG != null)
            NarrationBG.gameObject.SetActive(true);
        NarrationText.gameObject.SetActive(true);
        NarrationText.text = text;
    }
    public void ShowNarrationText(string text, float x = 0, float y = 0, float z = 0)
    {
        if (NarrationBG != null)
        {
            NarrationBG.gameObject.SetActive(true);
            NarrationText.gameObject.SetActive(true);
        }
        else
            NarrationText.gameObject.SetActive(true);

        NarrationText.text = "";
        if (NarrationBG != null)
        {
            NarrationBG.localPosition = new Vector3(x, y, z);
        }
        else
        {
            NarrationText.transform.localPosition = new Vector3(x, y, z);
        }
        NarrationText.text = text;
    }

    public void HidenarrationBG()
    {
        if (NarrationBG != null)
            NarrationBG.gameObject.SetActive(false);
    }
    public void HideNarrationText()
    {
        NarrationText.text = "";
        NarrationText.gameObject.SetActive(false);
        if (NarrationBG != null)
            NarrationBG.gameObject.SetActive(false);
    }
    public void SetNarrationPosition(Vector3 position)
    {
        if (NarrationBG != null)
        {
            NarrationBG.localPosition = position;
        }
        else
        {
            NarrationText.transform.localPosition = position;
        }

    }

    public void SetNarrationPosition(float x, float y, float z)
    {
        if (NarrationBG != null)
        {
            NarrationBG.localPosition = new Vector3(x, y, z);
        }
        else
        {
            NarrationText.transform.localPosition = new Vector3(x, y, z);
        }
    }

    public void ShowAndHideCanvasFixedUI(bool val)
    {
        CanvasFixedUI.gameObject.SetActive(val);
    }
    public void ExitPoint(UnityAction action)
    {
        SceneExitAction = action;
    }
    public Sprite GetImgResource(string Path)
    {
        Sprite find = null;

        return find = Resources.Load<Sprite>(Path);
    }
    public void PlayNarration(AudioClip clip)
    {
        if (Narration.isPlaying)
            Narration.Stop();
        Narration.clip = clip;
        if (clip != null)
            Narration.Play();
    }

    public void SetResolutionScale(float value)
    {
        if (value == 1)
        {
            UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1;
            Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

            //OVRManager.cpuLevel = 0;
            //OVRManager.gpuLevel = 0;
            //OVRPlugin.vsyncCount = 2;
        }
        else if (value == 2)
        {
            UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 2;
            Debug.Log("<color=yellow> 2024.04.16 주석 처리</color>");

            //OVRManager.cpuLevel = 1;
            //OVRManager.gpuLevel = 1;
            //OVRPlugin.vsyncCount = 1;
        }

    }

    /// <summary>
    /// Ray에 부딛힌 곳에 지시점 표시하기
    /// </summary>
    public void ShootRay()
    {
        Debug.Log("UIManager::ShootRay");        
        DrawOrder Director = FindObjectOfType<DrawOrder>();
        if(Director != null)
        {
            //RaycastHit target = Director.ShootRay();
            Vector3 dirVec = Director.ShootRay();

            //AsyncClient asyncClient = FindObjectOfType<AsyncClient>();
            //if (asyncClient != null)
            //{
            //    System.Text.StringBuilder ParentPath = new System.Text.StringBuilder();
            //    Component[] parents = gameObject.transform.GetComponentsInParent(typeof(Transform), true);
            //    for (int i = parents.Length - 1; i >= 0; i--)
            //    {
            //        if (i == 0)
            //            ParentPath.Append(string.Format("{0}", parents[i].name));
            //        else
            //            ParentPath.Append(string.Format("{0}/", parents[i].name));
            //    }

            //    Debug.Log("Total Path : " + ParentPath.ToString());
            //    asyncClient.CmdOrderButtonPressed(
            //        GetType().Name,
            //        ParentPath.ToString(),
            //        "ServerShootRay",
            //        dirVec.x,
            //        dirVec.y,
            //        dirVec.z);
            //}

            //if (target.transform != null)
            //{                
            //    AsyncClient asyncClient = FindObjectOfType<AsyncClient>();
            //    if (asyncClient != null)
            //    {
            //        System.Text.StringBuilder ParentPath = new System.Text.StringBuilder();
            //        Component[] parents = gameObject.transform.GetComponentsInParent(typeof(Transform), true);
            //        for (int i = parents.Length - 1; i >= 0; i--)
            //        {
            //            if (i == 0)
            //                ParentPath.Append(string.Format("{0}", parents[i].name));
            //            else
            //                ParentPath.Append(string.Format("{0}/", parents[i].name));
            //        }

            //        Debug.Log("Total Path : " + ParentPath.ToString());
            //        asyncClient.CmdOrderButtonPressed(
            //            GetType().Name,
            //            ParentPath.ToString(),
            //            "ServerShootRay",
            //            target.point.x,
            //            target.point.y,
            //            target.point.z);
            //    }
            //}
        }        
    }

    public void ServerShootRay(float x, float y, float z)
    {        
        DrawOrder Director = FindObjectOfType<DrawOrder>();
        DirectionEffect effect;
        Transform target;
        if (Director != null)
        {
            effect = Instantiate(DataManager.Inst.DirectionEffectPrefab, Vector3.zero, Quaternion.identity);
            SetParentToTarget(null, effect.transform, new Vector3(x, y, z), false);
            effect.gameObject.SetActive(false);
            //GameObject canvas = GameObject.Find("Canvas_FixedUI");

            //if (canvas != null)
            //{
            //    effect = Instantiate(DataManager.Inst.DirectionEffectPrefab, Vector3.zero, Quaternion.identity);
            //    SetParentToTarget(canvas.transform, effect.transform, new Vector3(x, y, z), true);
            //    target = effect.transform;
            //    effect.gameObject.SetActive(false);
            //}
            //else
            //{
            //    effect = Instantiate(DataManager.Inst.DirectionEffectPrefab, Vector3.zero, Quaternion.identity);
            //    SetParentToTarget(null, effect.transform, new Vector3(x, y, z), false);
            //    target = effect.transform;
            //    effect.gameObject.SetActive(false);
            //}
            StartRotateCompass(effect.transform);

        }
    }

    /// <summary>
    /// Target오브젝트로 부모 지정하기
    /// </summary>
    /// <param name="target"></param>
    /// <param name="child"></param>
    private void SetParentToTarget(Transform target, Transform child)
    {
        child.SetParent(target);
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
    }
    private Transform SetParentToTarget(Transform target, Transform child, Vector3 ToPos, bool isCanvas = true)
    {
        if (target != null)
            child.SetParent(target);
        
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
        return child;
    }

    //public void SetTestText()
    //{
    //    if (GameObject.Find("TestText") != null)
    //    {
    //        TestText = GameObject.Find("TestText").GetComponent<Text>();
    //        TestText.gameObject.SetActive(false);
    //    }
    //}

    //public void ShowAndHideTestText(string text, bool check)
    //{
    //    if (TestText != null)
    //    {
    //        TestText.gameObject.SetActive(check);
    //        TestText.text = text;
    //    }
    //}

    /// <summary>
    /// Set Narration pitch
    /// </summary>
    /// <param name="val"></param>
    public void SetNarrationPitch(float val)
    {
        if(Narration != null)
            Narration.pitch = val;
    }

    private IEnumerator FadeInRoutine(CanvasGroup Target, float totalTime = 1, params UnityAction[] receiver)
    {
        if (Target == null) yield break;

        Target.alpha = 0;
        Target.gameObject.SetActive(true);

        while (Target.alpha < 1)
        {
            Target.alpha += Time.deltaTime / totalTime;
            yield return null;
        }
        if (receiver != null)
        {
            if (receiver != null)
            {
                for (int i = 0; i < receiver.Length; i++)
                {
                    receiver[i].Invoke();
                }
            }
        }
    }
    private IEnumerator FadeOutRoutine(CanvasGroup Target, float totalTime = 1, params UnityAction[] receiver)
    {
        if (Target == null) yield break;

        Target.alpha = 1;
        Target.gameObject.SetActive(true);

        while (Target.alpha > 0)
        {
            Target.alpha -= Time.deltaTime / totalTime;
            yield return null;
        }
        if (receiver != null)
        {
            if (receiver != null)
            {
                for (int i = 0; i < receiver.Length; i++)
                {
                    receiver[i].Invoke();
                }
            }
        }
    }

    private IEnumerator HalfOrFullFadeInRoutine(CanvasGroup target, params UnityAction[] receiver)
    {
        if (target == null) yield break;

        Debug.Log("HalfOrFullFadeInRoutine");
        target.alpha = 0;
        float alpha = 0.0f;
        target.gameObject.SetActive(true);
        while (target.alpha < 1)
        {
            if (alpha >= 1)
                break;

            alpha += 0.01f;
            target.alpha += alpha;
            yield return null;
        }

        if (receiver != null)
        {
            for (int i = 0; i < receiver.Length; i++)
            {
                receiver[i].Invoke();
            }
        }
    }

    private IEnumerator QuickHalfOrFullFadeInRoutine(CanvasGroup target, params UnityAction[] receiver)
    {
        if (target == null) yield break;
        target.alpha = 0;
        target.gameObject.SetActive(true);

        while (target.alpha < 1)
        {
            target.alpha += 0.1f;
            yield return null;
        }
        if (receiver != null)
        {
            for (int i = 0; i < receiver.Length; i++)
            {
                receiver[i].Invoke();
            }
        }
    }


    private IEnumerator HalfOrFullFadeOutRoutine(CanvasGroup target, params UnityAction[] receiver)
    {
        if (target == null) yield break;
        target.gameObject.SetActive(true);

        while (target.alpha > 0)
        {
            target.alpha -= Time.deltaTime / 0.5f;
            yield return null;
        }

        if (receiver != null)
        {
            if (receiver != null)
            {
                for (int i = 0; i < receiver.Length; i++)
                {
                    receiver[i].Invoke();
                }
            }
        }
    }

    private IEnumerator QuickHalfOrFullFadeOutRoutine(CanvasGroup target, params UnityAction[] receiver)
    {
        target.alpha = 1;
        target.gameObject.SetActive(true);

        while (target.alpha > 0)
        {
            target.alpha -= 0.1f;
            yield return null;
        }

        if (receiver != null)
        {
            if (receiver != null)
            {
                for (int i = 0; i < receiver.Length; i++)
                {
                    receiver[i].Invoke();
                }
            }
        }
    }

    private IEnumerator WaitSecondsRoutine(float time, UnityAction[] receiver)
    {
        yield return new WaitForSeconds(time);

        for (int i = 0; i < receiver.Length; i++)
        {
            receiver[i].Invoke();
        }
    }
    private IEnumerator WaitForNarrationAndFuncRoutine(AudioClip clip, string subTitleText, bool isSkip, params UnityAction[] receiver)
    {
        if (Narration.isPlaying)
        {
            Narration.Stop();
        }

        if (clip != null)
        {
            Narration.clip = clip;
            Narration.Play();
        }

        NarrationText.text = subTitleText;
        if (NarrationText.text != "")
        {
            if (NarrationBG != null)
            {
                NarrationBG.gameObject.SetActive(true);
            }
            else
            {
                NarrationText.gameObject.SetActive(true);
            }
        }


        if (isSkip)
        {
            for (int i = 0; i < receiver.Length; i++)
            {
                receiver[i].Invoke();
            }

            while (Narration.isPlaying)
            {
                yield return null;
            }

            NarrationText.text = "";
            if (NarrationBG != null)
            {
                NarrationBG.gameObject.SetActive(false);
            }
            else
            {
                NarrationText.gameObject.SetActive(false);
            }
        }
        else
        {
            while (Narration.isPlaying)
            {
                yield return null;
            }

            NarrationText.text = "";
            if (NarrationBG != null)
            {
                NarrationBG.gameObject.SetActive(false);
            }
            else
            {
                NarrationText.gameObject.SetActive(false);
            }

            for (int i = 0; i < receiver.Length; i++)
            {
                receiver[i].Invoke();
            }
        }
    }

    private IEnumerator CompleteAndCallbackRoutine(Func<bool> CheckFunc, params UnityAction[] receiver)
    {

        Debug.Log("==================" + receiver[0].Method.Name + " CompleteAndCallbackRoutine ==================");
        while (CheckFunc() == false)
        {

            yield return null;
        }

        Debug.Log("==================" + receiver[0].Method.Name + " CompleteAndCallbackRoutine2 ==================");

        for (int i = 0; i < receiver.Length; i++)
        {
            receiver[i].Invoke();
        }
    }
}
