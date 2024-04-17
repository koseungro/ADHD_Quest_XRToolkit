using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CurvedUI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class Main : MonoBehaviour
{

    private SceneLoader sceneLoader;
    //private NetworkMng Mng;
    //public CanvasGroup Prograssbar;
    //public CanvasGroup Download;
    //public Text loadText;
    //public Text ProgressNumText;
    //public Image ProgressBarImg;
    //public GameObject ProgressBarBG;
    //public GameObject Slider;
    //public GameObject wifiText;
    //public GameObject AlertMessage;
    //public AudioSource audiosource;
    //public AudioClip completeEffect;
    //private bool IsDownloading = false;

    Canvas CanvasFixedUI;

    //[SerializeField]
    //private Button LowButton;
    //[SerializeField]
    //private Button HDButton;
    //[SerializeField]
    //private Button LowIconButton;
    //[SerializeField]
    //private Button HDIconButton;
    //[SerializeField]
    //private Sprite DownCompleteIconSprite;

    //private InAppSys IAPSys;

    private void Awake()
    {
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 2.0f;
        //Mng = FindObjectOfType<NetworkMng>();
        //if (GameObject.Find("Canvas_FixedUI") != null)
        //    CanvasFixedUI = GameObject.Find("Canvas_FixedUI").GetComponent<Canvas>();
        //SetCanvasActive(CanvasFixedUI, false);

        //IAPSys = GameObject.Find("InAppSystem").GetComponent<InAppSys>();

        //IAPSys.InitIAP(delegate 
        //                {
        //                    SetCanvasActive(CanvasFixedUI, true);
        //                });

        //Prograssbar.gameObject.SetActive(false);
        sceneLoader = SceneLoader.Inst;
    }

    private void SetCanvasActive(Canvas canvas, bool active)
    {
        canvas.gameObject.SetActive(active);        
    }

    // Use this for initialization
    void Start()
    {
        //Mng.SetIP("");
        //Mng.StartGearVRServer(false);
        //Mng.StartGearVRServer(true);

        //Mng.SetIP("localhost");
        //Mng.StartGearVRServer(true, "SceneMain");

        //        GameObject RightController;

        //        RightController = GameObject.Find("RightHandAnchor");
        //#if UNITY_EDITOR        
        //        RightController.SetActive(false);
        //#else        
        //#endif

        //#if UNITY_EDITOR
        //        if (CanvasFixedUI != null)
        //        {
        //            CurvedUISettings settings = CanvasFixedUI.GetComponent<CurvedUISettings>();
        //            settings.ControlMethod = CurvedUIInputModule.CUIControlMethod.MOUSE;
        //        }
        //#else
        //        if(CanvasFixedUI != null)
        //        {
        //            CurvedUISettings settings = CanvasFixedUI.GetComponent<CurvedUISettings>();
        //            settings.ControlMethod = CurvedUIInputModule.CUIControlMethod.OCULUSVR;
        //        }
        //#endif

        //        CachingDownload.Inst.SetProgressBarImg(ProgressBarImg);
        //        CachingDownload.Inst.SetProgressBarText(ProgressNumText);
        //        CachingDownload.Inst.SetLoadText(AlertMessage.GetComponent<Text>());
        //        CachingDownload.Inst.SetProgressBarBG(ProgressBarBG);
        //CachingDownload.Inst.SetLowButtonText(LowButton.transform.GetComponentInChildren<Text>());
        //CachingDownload.Inst.SetHDButtonText(HDButton.transform.GetComponentInChildren<Text>());


        //ShowDownload(true);
        //ShowLoadImg(true);
        //SetActive(Slider, false);

        //SetColor(loadText, new Color(0.848f, 0.848f, 0.848f));

        //CachingDownload.Inst.StartCheckProcess(
        //    delegate
        //    {
        //        //SetActive(Slider, true);
        //        ShowDownload(true);                
        //        ChangeSpriteIcon(true);                
        //        SetText(loadText, "더 선명한 화면으로 훈련을 진행하고 싶으시다면 고화질 버튼을 클릭하여 다운받아주세요." +
        //            "\n단, 고화질 버전은 휴대폰 발열에 대한 해결방안이 있으신 분들에게만 권장합니다." +
        //            "\n고화질 버전 다운로드시 20-30분 정도 소요되며 인터넷 환경에 따라 달라질 수 있습니다.");
        //        SetText(AlertMessage.GetComponent<Text>(), "현재 저화질 영상으로 이용중입니다.");
        //        SetActive(ProgressBarBG, true);
        //    },
        //    delegate
        //    {
        //        //SetActive(Slider, true);
        //        ShowDownload(true);

        //        ChangeSpriteIcon(false);
        //        SetText(loadText,"고화질 버전 진행시 발열 및 저장공간 등의 문제가 있다면\n저화질 버전으로 사용을 권장합니다." +
        //            "\n저화질 버전 다운로드시 10-15분 정도 소요되며 인터넷 환경에 따라 달라질 수 있습니다.");
        //        SetText(AlertMessage.GetComponent<Text>(), "현재 고화질 영상으로 이용중입니다.");
        //        SetActive(ProgressBarBG, true);
        //    },
        //    delegate
        //    {
        //        //SetActive(Slider, true);
        //        ShowDownload(true);

        //        SetText(AlertMessage.GetComponent<Text>(), "버전이 같지 않습니다. 영상을 다시 받아주세요");
        //        SetActive(ProgressBarBG, true);
        //    },
        //    delegate
        //    {
        //        //SetActive(Slider, true);
        //        ShowDownload(true);
        //        //SetText("훈련시작을 위해 동영상이 필요합니다.\n원하시는 화질을 선택하여 다운받아 주세요");
        //        SetActive(ProgressBarBG, true);
        //    }
        //);

        //if (!CachingDownload.Inst.CheckFile())
        //{

        //}
        //else
        //{
        //    Prograssbar.gameObject.SetActive(true);

        //    CachingDownload.Inst.AllLoadBundles(loadText,
        //        delegate { ShowLoadImg(true); },
        //        delegate { ShowLoadImg(false); },
        //        delegate { sceneLoader.StartMainScene(); }
        //        );
        //}

        sceneLoader.StartMainScene();
    }
    //private void SetTransform(Transform target, Vector3 pos)
    //{
    //    target.localPosition = pos;
    //}
    //private void SetTransform(Transform target, float x, float y, float z)
    //{
    //    target.localPosition = new Vector3(x,y,z);
    //}
    //private void ChangeSpriteIcon(bool IsLow)
    //{
    //    if(IsLow)
    //    {
    //        LowIconButton.image.sprite = DownCompleteIconSprite;
    //    }
    //    else
    //    {
    //        HDIconButton.image.sprite = DownCompleteIconSprite;
    //    }
    //}

    //private void SetActive(GameObject go, bool val)
    //{        
    //    go.SetActive(val);
    //}
    //private void CompleteSoundPlay()
    //{
    //    audiosource.clip = completeEffect;
    //    audiosource.Play();
    //}
    //private void ShowLoadImg(bool val)
    //{        
    //    loadText.gameObject.SetActive(val);
    //}
    //private void SetText(Text text, string txt)
    //{
    //    text.text = txt;
    //}
    //private void SetTextSize(Text text, int val)
    //{
    //    text.fontSize = val;
    //}
    //private void ShowDownload(bool val)
    //{
    //    Download.gameObject.SetActive(val);
    //}
    //private void SetColor(Text target, Color setColor)
    //{
    //    target.color = setColor;
    //}
    
    //public Button GetHDBuntton()
    //{
    //    return HDButton;
    //}

    //public void ChooseQuality(bool val)
    //{
    //    //if(!val)
    //    //{
    //    //    if(!IAPSys.GetIsPurchase)
    //    //    {
    //    //        IAPSys.BuyHDVideo();
    //    //        return;
    //    //    }
    //    //}


    //    LowButton.interactable = false;
    //    HDButton.interactable = false;

    //    SetTextSize(AlertMessage.GetComponent<Text>(), 60);
    //    SetTextSize(wifiText.GetComponent<Text>(), 45);

    //    SetTransform(AlertMessage.transform, new Vector3(0, 38.0f, 0));
    //    SetTransform(wifiText.transform, new Vector3(0, -58.5f, 0));

    //    SetColor(wifiText.GetComponent<Text>(), new Color(0.8125f, 0.8125f, 0.8125f));
        
    //    SetText(wifiText.GetComponent<Text>(), "다운로드 중에는 기어VR을 벗고 계셔도 됩니다.\n단, 휴대폰은 분리하지 마세요.");
        
    //    if (IsDownloading)
    //    {
    //        Debug.Log("Is Downloading");
    //        return;
    //    }

    //    if(val)
    //        IsDownloading = true;

    //    ShowDownload(false);
    //    SetActive(Slider, true);
    //    SetActive(loadText.gameObject, false);

    //    CachingDownload.Inst.ChooseQuality(AlertMessage.GetComponent<Text>(), val, 
    //        delegate { CompleteSoundPlay(); },
    //        delegate {
    //            Download.gameObject.SetActive(false);
    //            ShowLoadImg(false);
    //            CachingDownload.Inst.AllLoadBundles(loadText,                
    //            delegate {
    //                ShowLoadImg(false);
    //                ShowDownload(false);
    //                IsDownloading = false;
    //                SetActive(wifiText, false);
    //                SetActive(AlertMessage, false);
    //            },
    //            delegate { SetActive(ProgressBarBG, false); },
    //            delegate { sceneLoader.StartMainScene(); }
    //            );
    //        });
    //}
}
