using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public struct VersionInfo
{
    public int QualityLV;
    public string Version;
    public ulong[] Size;
}
[System.Serializable]
public struct ServerInfo
{
    public VersionInfo[] ServerInfos;
    
    public bool IsNull()
    {
        bool result = false;

        if (ServerInfos == null)
            return true;
        if (ServerInfos.Length == 0)
        {
            result = true;
            return result;
        }
        return result;
    }
}

/*
 

[System.Serializable]
public class CachingDownload : Singleton<CachingDownload> {

    [SerializeField]
    private ServerInfo ServerInfo;
    private ServerInfo MyVersionInfo;
    [SerializeField]
    public float percent = 0;
    // URL
    public string BundleURL = "";
    public string AssetName = "";
    public int version = 0;
    public bool IsVersionSame = false;
    private bool IsExists = false;    
    private bool IsDownloadStart = false;
    private bool IsCheckOver = false;
    private bool IsLoad = false;
    private bool IsVersionFileExists = false;
    private const string kMainURL = "http://1.214.193.90/FlagShip/";
    private const string kLow = "Low/";
    private const string kHD = "HD/";
    private const long kMBunit = 1048576;
    private string SubURL = "";

    private string FilePath = "";

    private string[] LowfileName = new string[]
                                {
                                    "data0.bin",
                                    "data1.bin",
                                    "data2.bin",
                                    "data3.bin",
                                    "data4.bin",
                                    "data5.bin"
                                };
    
    private string myVersion = "";
    private string VersionTxt = "";
    private string VersionPath = "";

    private int fileCount = 0;
    private int CheckIndex = 0;
    private bool[] CheckSize;
    
    [SerializeField]
    public AssetBundle bundle;
    public List<AssetBundle> LoadBundles = new List<AssetBundle>();
    public VideoClip clip;
    
    public WWW www;
    private UnityWebRequest reqFile;

    AssetBundleCreateRequest createreq;
    AssetBundleRequest abreq;
    private Text LoadText;

    private Text ProgressBarText;
    private Image ProgressBarImg;
    private GameObject ProgressBarBG;

    public string GetFilePath
    {
        get
        {
            if(FilePath == "")
            {
                FilePath = string.Format("{0}/{1}",
#if UNITY_EDITOR            
            Directory.GetParent(Application.dataPath),
#else
            Application.persistentDataPath,
#endif
            "bin/"
                );
            }
            
            return FilePath;
        }
    }

    // Use this for initialization
    void Start() {
        
    }
    public void SetProgressBarText(Text text)
    {
        ProgressBarText = text;
    }
    public void SetProgressBarImg(Image img)
    {
        ProgressBarImg = img;
    }
    public void SetLoadText(Text text)
    {
        LoadText = text;
    }
    public void SetProgressBarBG(GameObject go)
    {
        ProgressBarBG = go;
    }
    public void SetActive(GameObject go, bool val)
    {
        go.SetActive(val);
    }
    /// <summary>
    /// 해당경로의 파일을 삭제함
    /// </summary>
    /// <param name="fileName"></param>
    public void DeleteDir(string fileName)
    {
        if(File.Exists(GetFilePath + fileName))
            File.Delete(GetFilePath + fileName);
    }

    /// <summary>
    /// 시작 함수(파일 검사)
    /// </summary>
    /// <param name="actions"></param>
    public void StartCheckProcess(params UnityAction[] actions)
    {
        StartCoroutine(Check(actions));
    }

    IEnumerator Check(params UnityAction[] actions)
    {
        StartCoroutine(CheckFileRoutine());

        while (true)
        {
            // 파일 검사가 종료 되었는지 체크
            if (!IsCheckOver)
            {
                yield return null;
            }
            else
            {
                IsCheckOver = true;
                break;
            }
        }

        // 버전이 같냐
        if (IsVersionSame)
        {
            // 버전에 따라 콜백함수 실행
            if(MyVersionInfo.ServerInfos[0].QualityLV == 0)            
                actions[0].Invoke();
            else
                actions[1].Invoke();
        }
        else
        {
            // 파일 갯수랑 파일이름 배열이랑 같냐
            //if (fileCount == fileName.Length)
            //{
            //    bool result = false;
            //    for (int i = 0; i < CheckSize.Length; i++)
            //    {
            //        if (!CheckSize[i])
            //        {
            //            result = false;
            //        }
            //        else
            //            result = true;
            //    }
            //    if (result && IsVersionSame)
            //        IsExists = true;
            //    else
            //    {
            //        IsExists = false;
            //        Debug.Log("File is not Complete");
            //    }
            //}
            //else
            //{
            //    Debug.Log("File Count don't match");
            //    IsExists = false;
            //}

            // 파일이 존재 하냐
            if(IsExists)
            {
                actions[2].Invoke();
            }
            else
            {
                actions[3].Invoke();
            }
        }

        //if(IsExists)
        //{
            
        //}
        //else
        //{
            
        //}
    }

    IEnumerator CheckFileRoutine()
    {
        CheckSize = new bool[LowfileName.Length];
        Debug.Log(GetFilePath);

        // 디렉토리가 존재 하냐?
        if (!Directory.Exists(GetFilePath))
        {
            Debug.Log("Directory is not Exists");
            Directory.CreateDirectory(GetFilePath);
            IsCheckOver = true;            
            CheckVersion();
        }
        else
        {
            if (!File.Exists(GetFilePath + LowfileName[0]))
            {
                IsCheckOver = true;
                // 버전 확인
                CheckVersion();
                yield break;
            }
            IsExists = true;
            CheckVersion();
            // 버전이 같냐
            if (IsVersionSame)
            {
                IsCheckOver = true;
                yield break;
            }
            else
            {
                
                IsCheckOver = true;
                yield break;
            }
            #region Delete
            //// 파일이 존재하면 개수++
            //for (int i = 0; i < fileName.Length; i++)
            //{
            //    Debug.Log(i);
            //    if (File.Exists(GetFilePath + fileName[i]))
            //    {
            //        Debug.Log("fileName : " + fileName[i]);

            //        SetActive(ProgressBarBG, true);

            //        using (WWW request = new WWW(kMainURL + fileName[i]))
            //        {
            //            FileInfo finfo = new FileInfo(GetFilePath + fileName[i]);
            //            LoadText.text = "파일 검사 중..";
            //            while (true)
            //            {
            //                if (request.isDone)
            //                {
            //                    SetActive(ProgressBarBG, false);
            //                    Debug.Log("www is Done");
            //                    break;
            //                }
            //                else
            //                {
            //                    if(request.bytesDownloaded > finfo.Length)
            //                    {
            //                        Debug.Log("File is not match");
            //                        ProgressBarImg.fillAmount = 0;
            //                        ProgressBarText.text = "";
            //                        LoadText.text = "파일이 맞지 않습니다.";
            //                        break;
            //                    }
            //                    ProgressBarImg.fillAmount = request.progress;
            //                    ProgressBarText.text = System.Math.Truncate(request.progress * 100.0f).ToString();
            //                    //Debug.Log(request.progress);
            //                    yield return null;
            //                }
            //                //yield return request;
            //            }
            //            if (request.error != null)
            //                throw new System.Exception(www.error);

            //            long wwwFileSize = request.bytesDownloaded;
            //            long myFileSize = 0;


            //            if (finfo != null)
            //                myFileSize = finfo.Length;
            //            else
            //                Debug.Log("FileStream is Null");

            //            Debug.Log("wwwFileSize : " + wwwFileSize);
            //            Debug.Log("myFileSize  : " + myFileSize);

            //            if (wwwFileSize == myFileSize)
            //            {
            //                CheckSize[CheckIndex] = true;
            //            }
            //            else
            //            {
            //                CheckSize[CheckIndex] = false;
            //                DeleteDir(fileName[i]);
            //            }

            //        }
            //        CheckIndex++;        
            //    }
            //    fileCount++;
            //}         
            #endregion
        }
        IsCheckOver = true;
    }

    public void CheckVersion()
    {
        Debug.Log("3");
        using (UnityWebRequest VersionReq = UnityWebRequest.Get(kMainURL + "Version.txt"))
        {
            VersionReq.SendWebRequest();
            while (true)
            {
                if (VersionReq.isDone)
                {
                    Debug.Log("www is Done");
                    break;
                }
                else
                {
                    continue;
                }
            }

            // 서버 파일 정보
            VersionTxt = System.Text.Encoding.UTF8.GetString(VersionReq.downloadHandler.data);
            Debug.Log("Version : " + VersionTxt);

            ServerInfo = JsonUtility.FromJson<ServerInfo>(VersionTxt);

            VersionPath = GetFilePath + "Version.txt";

            if (!IsExists)
                return;
            Debug.Log("4");
            // 내 파일 버전 정보
            MyVersionInfo = new ServerInfo();
            ulong mySize = 0;
            ulong ServerSize = 0;
            try
            {
                GetMyVersion();

                if (!IsVersionFileExists)
                    return;

                if (MyVersionInfo.ServerInfos[0].QualityLV == 0)
                {
                    for (int i = 0; i < ServerInfo.ServerInfos[0].Size.Length; i++)
                    {
                        ServerSize += ServerInfo.ServerInfos[0].Size[i];
                    }
                }
                else
                {
                    for (int i = 0; i < ServerInfo.ServerInfos[1].Size.Length; i++)
                    {
                        ServerSize += ServerInfo.ServerInfos[1].Size[i];
                    }
                }

                myVersion = MyVersionInfo.ServerInfos[0].Version;

                for (int i = 0; i < LowfileName.Length; i++)
                {
                    FileInfo finfo = new FileInfo(GetFilePath + LowfileName[i]);
                    if (finfo.Exists)
                        mySize += ulong.Parse(finfo.Length.ToString(), System.Globalization.NumberStyles.Integer);
                }
                Debug.Log("MyVersion : " + myVersion);
                Debug.Log("mySize : " + mySize);

                Debug.Log("5");
            }
            catch (System.Exception ex)
            {
                throw ex;// System.Exception();
                Debug.Log("ex.error : " + ex.Message);
            }
            finally
            {
                if (MyVersionInfo.ServerInfos[0].QualityLV == 0)
                {
                    if (ServerInfo.ServerInfos[0].Version.Equals(MyVersionInfo.ServerInfos[0].Version) &&
                   ServerSize == mySize)
                    {
                        Debug.Log("Version is Same");
                        IsVersionSame = true;
                    }
                    else
                    {
                        Debug.Log("Version is not Same");
                        IsVersionSame = false;
                    }
                }
                else
                {
                    if (ServerInfo.ServerInfos[1].Version.Equals(MyVersionInfo.ServerInfos[0].Version) &&
                   ServerSize == mySize)
                    {
                        Debug.Log("Version is Same");
                        IsVersionSame = true;
                    }
                    else
                    {
                        Debug.Log("Version is not Same");
                        IsVersionSame = false;
                    }
                }

            }
            Debug.Log("6");
        }
    }
    /// <summary>
    /// 내 버전 파일 확인
    /// </summary>
    private void GetMyVersion()
    {        
        if(MyVersionInfo.IsNull())
        {
            MyVersionInfo = new ServerInfo();

            if (!File.Exists(VersionPath))
            {
                File.Create(VersionPath);
                MyVersionInfo = new ServerInfo();
                MyVersionInfo.ServerInfos = new VersionInfo[1];
                MyVersionInfo.ServerInfos[0].Size = new ulong[LowfileName.Length];
                MyVersionInfo.ServerInfos[0].Version = "";
                MyVersionInfo.ServerInfos[0].QualityLV = 0;
                IsVersionFileExists = false;
                return;
            }

            string fileText = File.ReadAllText(GetFilePath+"Version.txt", System.Text.Encoding.UTF8);
            //Debug.Log(fileText);
            if (fileText != string.Empty)
            {
                MyVersionInfo = JsonUtility.FromJson<ServerInfo>(fileText);
                IsVersionFileExists = true;
                if (MyVersionInfo.GetType() != typeof(ServerInfo))
                {
                    Debug.Log("Is not match File");
                    MyVersionInfo = new ServerInfo();
                    MyVersionInfo.ServerInfos = new VersionInfo[1];
                    MyVersionInfo.ServerInfos[0].Size = new ulong[LowfileName.Length];
                    MyVersionInfo.ServerInfos[0].Version = "";
                    MyVersionInfo.ServerInfos[0].QualityLV = 0;
                    IsVersionFileExists = false;
                }
            }
            else
            {
                Debug.Log("File is Empty");
                MyVersionInfo.ServerInfos = new VersionInfo[1];
                MyVersionInfo.ServerInfos[0].Size = new ulong[LowfileName.Length];
                MyVersionInfo.ServerInfos[0].Version = "";
                MyVersionInfo.ServerInfos[0].QualityLV = 0;
                IsVersionFileExists = false;
            }
        }
        
    }
    /*
    public void CheckFile()
    {
        IsCheckOver = false;

        CheckSize = new bool[fileName.Length];
        Debug.Log(GetFilePath);

        // 디렉토리가 존재 하냐?
        if (!Directory.Exists(GetFilePath))
        {
            Debug.Log("Directory is not Exists");
            Directory.CreateDirectory(GetFilePath);

            return;
        }
        else
        {
            // 파일이 존재하면 개수++
            for (int i = 0; i < fileName.Length; i++)
            {
                if (File.Exists(GetFilePath + fileName[i]))
                {
                    Debug.Log("fileName : " + fileName[i]);
                    StartCoroutine(CheckFileSize(fileName[i]));
                    fileCount++;
                }
            }

            // 파일 갯수랑 파일이름 배열이랑 같냐
            if (fileCount == fileName.Length)
            {
                bool result = false;
                for (int i = 0; i < CheckSize.Length; i++)
                {
                    if (!CheckSize[i])
                    {
                        result = false;
                    }
                    else
                        result = true;
                }
                if (result && IsVersionSame)
                    IsExists = true;
                else
                {
                    IsExists = false;
                    Debug.Log("File is not Complete");
                }
            }
            else
            {
                Debug.Log("File Count don't match");
                IsExists = false;
            }
        }
    }
  

    /// <summary>
    /// 저화질 고화질 선택
    /// </summary>
    /// <param name="text"></param>
    /// <param name="IsLowQuality"></param>
    /// <param name="callback"></param>
    public void ChooseQuality(Text text, bool IsLowQuality, params UnityAction[] callback)
    {        
        LoadText = text;

        GetMyVersion();
               
        // 저화질
        if(IsLowQuality)
        {
            if (MyVersionInfo.ServerInfos == null)
                return;
            if(MyVersionInfo.ServerInfos[0].QualityLV == 0 && IsVersionSame)
            {
                callback[1].Invoke();
                return;
            }
            else
            {
                for(int i = 0; i < LowfileName.Length; i++)
                {
                    DeleteDir(LowfileName[i]);
                }
            }
        }
        //고화질
        else
        {
            if (MyVersionInfo.ServerInfos[0].QualityLV == 1 && IsVersionSame)
            {
                callback[1].Invoke();
                return;
            }
            else
            {
                for (int i = 0; i < LowfileName.Length; i++)
                {
                    DeleteDir(LowfileName[i]);
                }
            }
        }

        // 저화질
        if (IsLowQuality)
        {            
            StartCoroutine(StartDownloadRoutine(IsLowQuality, callback));

        }
        // 고화질
        else
        {            
            StartCoroutine(StartDownloadRoutine(IsLowQuality, callback));
        }
    }

    IEnumerator StartDownloadRoutine(bool IsLow, params UnityAction[] callback)
    {
        int i = 0;
        //percent = 0;
        

        Debug.Log("StartDownloadRoutine");
        //WaitForSeconds tsec = new WaitForSeconds(2.0f);

        while (true)
        {
            if (IsDownloadStart)
            {
                if (reqFile.isDone)
                {                    
                    continue;
                }
                float fileSize = 0.0f;

                if (IsLow)
                {
                    fileSize = float.Parse(ServerInfo.ServerInfos[0].Size[i - 1].ToString());
                }
                else
                {
                    fileSize = float.Parse(ServerInfo.ServerInfos[1].Size[i - 1].ToString());
                }
                
                //Debug.Log(reqFile.downloadedBytes);
                //Debug.Log(ulong.Parse(ServerInfo.Size));
                percent = ((float)reqFile.downloadedBytes / fileSize);
                //Debug.Log("percent : " + percent.ToString());
                ProgressBarImg.rectTransform.sizeDelta = new Vector2((297.5f*percent),11.0f); //reqFile.downloadProgress;
                ProgressBarText.text = string.Format("{0}MB / {1}MB", 
                    System.Math.Truncate((float)reqFile.downloadedBytes/kMBunit),
                    System.Math.Truncate(fileSize/kMBunit));
                //Debug.Log(reqFile.downloadProgress);
                //Debug.Log("Download Percent : " + www.progress * 100.0f);
                yield return null;                
            }
            else
            {
                if (i >= LowfileName.Length)
                    break;

                LoadText.text = string.Format("Download({0}/{1})", (i + 1), LowfileName.Length);
                if (IsLow)
                {   
                    BundleURL = string.Format("{0}{1}{2}", kMainURL, kLow, LowfileName[i]);
                    Debug.Log("BundleURL : " + BundleURL);
                    StartCoroutine(DownloadRoutine(GetFilePath, LowfileName[i]));
                    i++;
                }
                else
                {   
                    BundleURL = string.Format("{0}{1}{2}", kMainURL, kHD, LowfileName[i]);
                    Debug.Log("BundleURL : " + BundleURL);
                    StartCoroutine(DownloadRoutine(GetFilePath, LowfileName[i]));
                    i++;
                }
                
            }
        }

        // 저화질 고화질에 따라서 다운로드 완료 되면 서버 파일 정보를 내 파일 정보에 덮어쓰기
        Debug.Log("VersionPath : " + VersionPath + " / VersionTxt : " + VersionTxt);
        ServerInfo SaveInfo = new ServerInfo();
        if (IsLow)
        {
            SaveInfo.ServerInfos = new VersionInfo[1];
            SaveInfo.ServerInfos[0] = ServerInfo.ServerInfos[0];
            SaveInfo.ServerInfos[0].QualityLV = 0;
        }
        else
        {
            SaveInfo.ServerInfos = new VersionInfo[1];
            SaveInfo.ServerInfos[0] = ServerInfo.ServerInfos[1];
            SaveInfo.ServerInfos[0].QualityLV = 1;
        }        

        // 파일 쓰기
        File.WriteAllText(VersionPath, JsonUtility.ToJson(SaveInfo), System.Text.Encoding.UTF8);
        callback[0].Invoke();
        yield return new WaitForSeconds(5.0f);
        // 파일 다운로드 완료 시 완료음 실행
        callback[1].Invoke();
    }

    /// <summary>
    /// 다운로드 받는 코루틴
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    IEnumerator DownloadRoutine(string filePath, string fileName)
    {

        IsDownloadStart = true;

        //while(!Caching.ready)
        //{
        //    yield return null;
        //}

        //www = WWW.LoadFromCacheOrDownload(BundleURL, version);

        //yield return www;

        //if(bundle == null)
        //    bundle = www.assetBundle;
        //else
        //{
        //    string[] names = bundle.GetAllAssetNames();
        //    for(int i = 0; i < names.Length; i++)
        //    {
        //        Debug.Log("names : " + names[i]);
        //    }
        //}
        
        DownloadHandlerFile dh = new DownloadHandlerFile(filePath + fileName);        
        reqFile = UnityWebRequestAssetBundle.GetAssetBundle(BundleURL);
        
        reqFile.downloadHandler = dh;
        
        //StartCoroutine(ShowDownloadPercent());
        yield return reqFile.SendWebRequest();

        if (!reqFile.isHttpError && !reqFile.isNetworkError)
        {
            //LoadBundles.Add(AssetBundle.LoadFromFile(filePath + fileName));
            //bundle = AssetBundle.LoadFromFile(filePath + fileName);            
        }
        else
        {
            throw new System.Exception("WWW download had an error:" + reqFile.error);
        }
        dh.Dispose();
        IsDownloadStart = false;
    }
   
    /// <summary>
    /// 번들 로드
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="AssetName"></param>
    /// <param name="player">비디오 플레이어</param>
    /// <param name="callback">비디오 Prepare() 함수</param>
    public void LoadBundle(string fileName, string AssetName, ref VideoPlayer player, UnityAction callback)
    {
        // 번들 null 체크
        if (bundle != null)
        {
            // 번들안에 기존에 로드한 데이터가 있는지 체크
            // 있으면 해당 에셋 로드 및 종료
            if (bundle.name.Equals(fileName))
            {
                VideoClip clip = bundle.LoadAsset(AssetName) as VideoClip;
                player.clip = clip;
                player.Prepare();
                return;
            }
        }

        //bundle = null;        
        CoroutineWithData coData = new CoroutineWithData(this, LoadBundleRoutine(fileName, AssetName, player, callback));
        
        //return coData.result;
    }
    public void AllLoadBundles(Text text, params UnityAction[] callbacks)
    {        
        LoadText = text;
        StartCoroutine(AllLoadBundlesRoutine(callbacks));
    }

    public IEnumerator GetAsset(string fileName, string AssetName,VideoPlayer player, UnityAction callback)
    {
        Debug.Log(AssetName);
        //bundle = null;
        this.AssetName = AssetName;
        //AssetBundleRequest req2 = new AssetBundleRequest();
        //AssetBundleCreateRequest creatreq = new 
        //if (AssetName.Contains("Cafe"))
        //{
        //    req2 =  bundle.LoadAsset(AssetName);

        //    while (true)
        //    {
        //        if (req2.isDone)
        //            break;
        //        else
        //        {

        //        }
        //        yield return req2;
        //    }
        //}
        //else
        //{
        //    req2 = bundle.LoadAssetAsync(AssetName);

        //    while (true)
        //    {
        //        if (req2.isDone)
        //            break;
        //        else
        //        {

        //        }
        //        yield return req2;
        //    }
        //}        
        //object obj = bundle.LoadAsset(AssetName);
        yield return bundle;
        VideoClip clip = bundle.LoadAsset(AssetName) as VideoClip;
        //VideoClip clip = abreq.asset as VideoClip;
        //yield return req2.asset;
        Debug.Log("Load Pre");
        //VideoClip clip = bundle.LoadAsset(AssetName) as VideoClip;
        Debug.Log(clip.name);        
        player.clip = clip;
        Debug.Log("Load Pre");
        player.Prepare();
        //bundle.Unload(false);
        //callback.Invoke();
    }
    

    private IEnumerator AllLoadBundlesRoutine(params UnityAction[] callbacks)
    {
        callbacks[0].Invoke();
        yield return null;
        Debug.Log("AllLoadBundles");
        

        callbacks[1].Invoke();
        if(callbacks[2] != null)
            callbacks[2].Invoke();
        
    }
    /// <summary>
    /// 번들 로드 코루틴
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="AssetName"></param>
    /// <param name="player"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator LoadBundleRoutine(string fileName, string AssetName, VideoPlayer player, UnityAction callback)
    {        
        // 에셋번들에서 모든 번들 로드
        IEnumerable bundles = AssetBundle.GetAllLoadedAssetBundles();
        IEnumerator ie = bundles.GetEnumerator();
        
        bool IsLoaded = false;

        Debug.Log("fileName : " + fileName);
        //int i = 0;
        while (ie.MoveNext())
        {
            // 번들 이름에 ()가 있어서 제거, 공백 제거(Trim) 후 비교
            string tempLoadedName = ie.Current.ToString().Remove(ie.Current.ToString().IndexOf("("));
            tempLoadedName = tempLoadedName.Trim();
            Debug.Log("bundle.name : "+ tempLoadedName);
            if(fileName.Equals(tempLoadedName))
            {
                Debug.Log(fileName + " Bundle is Loaded");
                IsLoaded = true;
                bundle = ie.Current as AssetBundle;
                break;
            }
            //i++;
        }

        if(IsLoaded)
        {
            //AssetBundle.UnloadAllAssetBundles(false);
            Debug.Log(fileName + " Bundle is Loaded so Bundle.Unload and Load");
            Debug.Log(bundle.name);
            //bundle = AssetBundle.
        }
        else
        {
            Debug.Log(fileName + " Bundle is not Loaded");
            bundle = AssetBundle.LoadFromFile(GetFilePath + fileName);
        }
        IsLoad = false;
        //bundle.Unload(false);
        //createreq = AssetBundle.LoadFromFileAsync(GetFilePath + fileName);
        
        //abreq = bundle.LoadAsset(AssetName);
        yield return bundle;

       

        //bundle = null;
        //bundle = createreq.assetBundle;

        CoroutineWithData co = new CoroutineWithData(this, GetAsset(fileName, AssetName,player, callback));
        yield return co.coroutine;
        //bundle.Unload(false);


        //VideoClip clip = GetAsset(fileName, AssetName) as VideoClip;
        //VideoClip clip = co.result as VideoClip;
        //Debug.Log(clip.name);
        //player.clip = clip;
        //callback.Invoke();
        //bundle = AssetBundle.LoadFromFile(filePath);
        //yield return bundle;
    }
    private IEnumerator ShowDownloadPercent()
    {
        while (reqFile.downloadProgress < 1)
        {
            Debug.Log("Download Percent : " + reqFile.downloadProgress * 100.0f);
            yield return null;
        }
    }
}


*/