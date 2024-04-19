using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{

    /// <summary>
    /// Unity Player
    /// </summary>
    public VideoPlayer player;
    public VideoClip clip;
    /// <summary>
    /// 기본 auidoSoucre
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// 불러올 video Path 배열
    /// </summary>
    public VideoInfo[] videoInfoArray;
    /// <summary>
    /// Loop 판단
    /// </summary>
    public bool isLoopPlay = false;

    /// <summary>
    /// Player IEnumerator(영상 재생 관련)
    /// </summary>
    private IEnumerator playerIE;
    /// <summary>
    /// Loop IEnumerator(반복 재생 관련)
    /// </summary>
    private IEnumerator LoopPlayerIE;

    private IEnumerator PauseIE;
    /// <summary>
    /// 영상이 미리 로드되었는지 판단
    /// </summary>
    public bool isPrepared
    {
        get
        {
            return player.isPrepared;
        }
    }
    /// <summary>
    /// Volume 셋팅
    /// </summary>
    public float Volume
    {
        get
        {
            return audioSource.volume;
        }
        set
        {
            audioSource.volume = value;
        }
    }
    /// <summary>
    /// 영상이 준비되었는지 함수(함수포인터로 사용하기 위함)
    /// </summary>
    /// <returns></returns>
    public bool isPreparedFunc()
    {
        return player.isPrepared;
    }
    /// <summary>
    /// 영상이 재생중인지 체크
    /// </summary>
    /// <returns></returns>
    public bool isPlayingFunc()
    {
        return player.isPlaying;
    }
    /// <summary>
    /// 영상 재생
    /// </summary>
    public void VideoPlay()
    {
        Debug.Log("VideoPlay");
        //player.isLooping = isLoop;
        playerIE = PlayRoutine();
        StartCoroutine(playerIE);
        //player.Play();
    }
    public void VideoPlay2()
    {
        Debug.Log("VideoPlay");
        //player.isLooping = isLoop;
        playerIE = PlayRoutine(false);
        StartCoroutine(playerIE);
        //player.Play();
    }

    IEnumerator PlayRoutine(bool isFirstStart = true)
    {
        while (!isPrepared)
        {
            yield return null;
        }
        if (player.isPlaying)
        {
            player.Pause();
            if (isFirstStart)
            {
                player.time = 0;
            }
        }
        audioSource.volume = 1.0f;
        player.Play();
    }


    /// <summary>
    /// 영상 반복 재생 Coroutine
    /// </summary>
    /// <param name="StartFrame">시작 시간</param>
    /// <param name="EndFrame">끝나는 시간</param>
    public void VideoLoopPlay(float StartFrame, float EndFrame)
    {
        LoopPlayerIE = VideoLoopPlayRoutine(StartFrame, EndFrame);
        StartCoroutine(LoopPlayerIE);
    }
    /// <summary>
    /// VideoLoopPlay StopCoroutine
    /// </summary>
    public void StopVideoLoopPlay()
    {
        if (LoopPlayerIE != null)
            StopCoroutine(LoopPlayerIE);
        isLoopPlay = false;
    }
    private IEnumerator VideoLoopPlayRoutine(float StartFrame, float EndFrame)
    {
        WaitForSeconds TwoSec = new WaitForSeconds(1.0f);
        isLoopPlay = true;
        while (!isPrepared)
        {
            yield return null;
        }
        SetTime(StartFrame);
        player.Play();
        while (isLoopPlay)
        {
            if (player.time >= EndFrame)
            {
                SetTime(StartFrame);
                yield return TwoSec;
            }
            else
            {
                yield return null;
            }
        }
    }
    /// <summary>
    /// video Path Load
    /// </summary>
    public void LoadVideoInfo()
    {
        videoInfoArray = Resources.LoadAll<VideoInfo>("ScriptableData/Video");

    }

    public void StopVideo()
    {
        if (playerIE != null)
            StopCoroutine(playerIE);
    }
    IEnumerator StopRoutine()
    {
        while (!isPrepared)
        {
            yield return null;
        }
        player.Stop();
    }
    public void Seek(float time)
    {
        //player.time = time;
        StartCoroutine(SeekRoutine(time));
    }
    IEnumerator SeekRoutine(float time)
    {
        while (!isPrepared)
        {
            yield return null;
        }
        player.time = time;
    }

    private bool CheckFileExists(string path)
    {
        FileInfo file = new FileInfo(path);

        if (file.Exists)
            return true;
        else
            return false;
    }

    private void CreateVideoFolder(string path)
    {
        Directory.CreateDirectory(path);

        Debug.Log($"Video Folder 생성 : {path}");
    }

    public void SetVideoURL(string videoName)
    {
        /* ================================ App Store Upload Version ================================ */

        //if (player != null)
        //    if(player.clip != null)
        //        if (player.clip.name.Equals(videoName))
        //            return; 

        //        string filePath = string.Format("{0}/{1}",

        //#if UNITY_EDITOR
        //                                    Directory.GetParent(Application.dataPath),
        //#else
        //                                    Application.persistentDataPath,
        //#endif

        //                                    "bin/"
        //                                    );

        //        string name = "";

        //if (videoName.Contains("Cafe"))
        //{
        //    if(videoName.Contains("Level1"))
        //        name = "data0.bin";
        //    else if (videoName.Contains("Level2"))
        //        name = "data1.bin";
        //    else if (videoName.Contains("Level3"))
        //        name = "data2.bin";                        
        //}
        //else if(videoName.Contains("Street"))
        //{
        //    if(videoName.Contains("Level1"))
        //        name = "data3.bin";
        //    else if (videoName.Contains("Level2"))
        //        name = "data4.bin";
        //    else if (videoName.Contains("Level3"))
        //        name = "data5.bin";            
        //}
        //Debug.Log("videoContoller.name :" + name);

        //CachingDownload.Inst.LoadBundle(name, videoName, ref player, delegate { LoadComplete(); });

        /* ================================ App Store Upload Version ================================ */

        string path = string.Format("{0}/Video/{1}.mp4",

#if UNITY_EDITOR
                                    Directory.GetParent(Application.dataPath),
#else
                                    Application.persistentDataPath,
#endif

                                    videoName
                                    );

        if (CheckFileExists(path))
        {
            Debug.Log($"Video path : <color=cyan>{path}</color>");
            player.url = path;
            player.Prepare();
        }
        else
        {
            Debug.Log($"Video path Null : <color=red>{path}</color>");

            string videoFolderPath = "";
#if UNITY_EDITOR
            videoFolderPath = Directory.GetParent(Application.dataPath) + "/Video/";
#else
            videoFolderPath = Application.persistentDataPath + "/Video/";

#endif
            if (!CheckFileExists(videoFolderPath))
                CreateVideoFolder(videoFolderPath);
        }

    }
    public void LoadComplete()
    {
        Debug.Log("5");

        //player.clip = clip;
        //player.clip = clip;
        player.Prepare();
    }
    public void VideoPause()
    {
        PauseIE = PauseRoutine();
        StartCoroutine(PauseIE);
    }
    IEnumerator PauseRoutine()
    {
        while (!isPrepared)
        {
            yield return null;
        }
        player.Pause();
    }

    public void SetFrame(long t)
    {
        player.frame = t;
    }
    public void SetTime(float time)
    {
        player.time = time;
    }

}
