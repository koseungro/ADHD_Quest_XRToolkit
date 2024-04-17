using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public Slider slider;
    public Text percentText;

    /// <summary>
    /// 현재 씬 로딩 중인지 체크
    /// </summary>
    public bool isStart = false;
    public Text LoadingText;

    private AsyncOperation async;
    private CurvedUI.CurvedUISettings Setting;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private float speed = 1;
    private float WaitPercent = 0.0f;

    private GameObject LaserBeam;
    public Light _light; // Quest2 콘트롤러가 잘 보이지 않아서 추가 => 대기 씬에서만 활성화 됨 [2024.04.05]


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(_light);
        canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
    }

    public void StartMainScene()
    {
        if (canvas == null)
        {
            Setting = GetComponent<CurvedUI.CurvedUISettings>();
            canvas = transform.GetComponent<Canvas>();
            canvas.worldCamera = GameObject.Find("XR Origin/Camera Offset/Main Camera/Camera_UI").GetComponent<Camera>();
            canvasGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
        }
        //StartCoroutine(StartScene());
        //LoadScene(2);
        LoadScene("SceneMain");
    }


    void Update()
    {
        if (canvas != null)
        {
            if (canvas.worldCamera == null)
                canvas.worldCamera = GameObject.Find("Camera_UI").GetComponent<Camera>();
        }
        if (isStart)
        {            
            if (WaitPercent < 0.9f)
            {
                //slider.value = async.progress;
                //slider.value += Time.deltaTime / 6;
                WaitPercent += Time.deltaTime / 5;
                //if (async.progress >= 0.9f)
                //    WaitPercent = 0.9f;
                
                //percentText.text = string.Format("{0}%", (int)(slider.value * 100f));
            }            
            else if(async.progress >= 1)
            {   
                //slider.value = 1;
                //percentText.text = "100%";
                if(canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                }
                speed = 1;
                //async.allowSceneActivation = true;                
                isStart = false;
                if(Setting != null)
                    Setting.BlocksRaycasts = false;                
            }
            else
            {                
                if (WaitPercent >= 0.9f && async.allowSceneActivation == false)
                {
                    //if (LaserBeam != null)
                        //LaserBeam.SetActive(true);

                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                    //LoadingText.gameObject.SetActive(false);
                    speed = 1;
                    //async.allowSceneActivation = true;                
                    isStart = false;
                    if(Setting != null)
                        Setting.BlocksRaycasts = false;
                    async.allowSceneActivation = true;
                    if (LaserBeam != null)
                        LaserBeam.SetActive(true);
                    Debug.Log($"<color=yellow> 씬 로드 : {async.allowSceneActivation}</color>");
                }
                //slider.value += Time.deltaTime / speed;                
                //percentText.text = string.Format("{0}%", (int)(slider.value * 100f));
            }
        }
    }

    public bool IsLoadingTextActive()
    {
        return LoadingText.gameObject.activeInHierarchy;
    }
    public void SetLoadingTextActive(bool val)
    {
        LoadingText.gameObject.SetActive(val);
    }
    
    public void LoadMain()
    {
        DataManager.Inst.MissionLevel = 0;
        percentText.text = "0%";
        slider.value = 0;
        WaitPercent = 0.0f;

        //canvasGroup.alpha = 0;

        if (GameObject.Find("CurvedUILaserBeam") != null)
        {
            LaserBeam = GameObject.Find("CurvedUILaserBeam");
            LaserBeam.SetActive(false);
        }

        async = SceneManager.LoadSceneAsync("SceneMain");
        async.allowSceneActivation = false;
        //transform.GetChild(0).gameObject.SetActive(true);

        if (!_light.gameObject.activeSelf) 
            _light.gameObject.SetActive(true);
                       
        canvasGroup.alpha = 1;
        if (DataManager.Inst.isServer)
            LoadingText.gameObject.SetActive(true);
        StartCoroutine(LoadMainRoutine());
        //isStart = true;

    }
    public void AddScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
    }
    public void UnLoadScene(string SceneName)
    {
        SceneManager.UnloadSceneAsync(SceneName);
    }
    public void LoadScene(string SceneName)
    {
        //Debug.Log(System.DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.ff"));
        if (SceneName == "-1")
            return;

        switch (SceneName)
        {
            case "ClientInitScene":
                speed = 0.1f;
                DataManager.Inst.CurrentLevel = "";
                DataManager.Inst.CurrentLevelType = LevelType.None;
                break;
            case "Loading":
                speed = 0.1f;                
                DataManager.Inst.CurrentLevel = "";
                DataManager.Inst.CurrentLevelType = LevelType.None;
                break;
            case "SceneMain":
                speed = 0.1f;
                DataManager.Inst.SceneName = "훈련 선택 중";
                break;
            case "TestRoom":
                speed = 0.3f;
                DataManager.Inst.SceneName = "방안에서";
                DataManager.Inst.CurrentLevel = "SceneRoom0";
                DataManager.Inst.CurrentLevelType = LevelType.ROOM_LEVEL0;
                break;
            case "SceneStreet360Degree":
                speed = 0.1f;
                DataManager.Inst.SceneName = "길거리에서";
                DataManager.Inst.CurrentLevel = "SceneStreet0";
                DataManager.Inst.CurrentLevelType = LevelType.STREET_LEVEL0;
                break;
            case "SceneCafe360Degree":
                speed = 0.1f;
                DataManager.Inst.SceneName = "카페에서";
                DataManager.Inst.CurrentLevel = "SceneCafe0";
                DataManager.Inst.CurrentLevelType = LevelType.CAFE_LEVEL0;
                break;
            case "SceneLibrary":
                speed = 7.0f;
                DataManager.Inst.SceneName = "도서관에서";
                DataManager.Inst.CurrentLevel = "SceneLibrary";
                DataManager.Inst.CurrentLevelType = LevelType.LIBRARY_LEVEL0;
                break;
            case "SceneReference":
                speed = 7.0f;
                DataManager.Inst.SceneName = "자료실에서";
                DataManager.Inst.CurrentLevel = "SceneReference";
                DataManager.Inst.CurrentLevelType = LevelType.LIBRARY_LEVEL2;
                break;
            case "SceneLounge":
                speed = 7.0f;
                DataManager.Inst.SceneName = "휴게실에서";
                DataManager.Inst.CurrentLevel = "SceneLounge";
                DataManager.Inst.CurrentLevelType = LevelType.LIBRARY_LEVEL3;
                break;
        }

        if (GameObject.Find("CurvedUILaserBeam") != null)
        {
            LaserBeam = GameObject.Find("CurvedUILaserBeam");
            LaserBeam.SetActive(false);
        }

        //percentText.text = "0%";
        //slider.value = 0;
        //canvasGroup.alpha = 0;
        WaitPercent = 0.0f;
        Debug.Log($"<color=yellow>[{SceneName}]</color> 씬을 로드합니다.");
        async = SceneManager.LoadSceneAsync(SceneName);
        async.allowSceneActivation = false;
        //transform.GetChild(0).gameObject.SetActive(true);

        bool isLightOn = false;
        if(SceneName == "ClientInitScene" || SceneName == "Loading" || SceneName == "SceneMain")
        {
            isLightOn = true;
        }
        _light.gameObject.SetActive(isLightOn);

        if(canvasGroup != null)
            canvasGroup.alpha = 1;
        if(Setting != null)
            Setting.BlocksRaycasts = true;
        if (DataManager.Inst.isServer)
        {
            Debug.Log("isServer And Show LoadingText");
            if (LoadingText != null)
            {
                LoadingText.gameObject.SetActive(true);
            }
        }   
        else if(DataManager.Inst.isPassConnect)
        {
            Debug.Log("SceneLoader::Connect Pass");
            if (LoadingText != null)
            {
                LoadingText.gameObject.SetActive(true);
            }
        }
        if (LoadingText != null)
        {
            Debug.Log("just Show LoadingText");
            LoadingText.gameObject.SetActive(true);
        }
        isStart = true;
    }


    IEnumerator LoadMainRoutine()
    {
        while(true)
        {
            if (async.progress >= 0.9f)
            {
                //slider.value = 1;
                //percentText.text = "100%";
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                }
                speed = 1;
                //async.allowSceneActivation = true;                
                isStart = false;
                LoadingText.gameObject.SetActive(false);
                if (LaserBeam != null)
                    LaserBeam.SetActive(true);
                break;
            }
            else
            {
                //slider.value = async.progress;
                yield return null;
            }
        }
        if(LaserBeam != null)
            LaserBeam.SetActive(true);
        async.allowSceneActivation = true;
    }
}
