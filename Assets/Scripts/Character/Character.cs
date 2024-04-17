using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RogoDigital.Lipsync;
using UnityEngine.Events;

public class Character : MonoBehaviour {

    private Animator animator;
    private LipSync lipSync;
    private List<LipSyncData> LipDataList = new List<LipSyncData>();
    private AudioSource audioSource;
    private List<AudioClip> LipClipList = new List<AudioClip>();
    private LipSyncData CurrentData;
    private Vector3 InitPos;
    private MeshRenderer[] renderers;
    public SkinnedMeshRenderer[] skinnedMeshRenderer;

    private IEnumerator WaitForSecAndAniPlayIE;
    private IEnumerator WaitForSecAndActionIE;


    private void Awake()
    {        
        
    }
    // Use this for initialization
    void Start()
    {
        InitFunc();
    }

    public void InitFunc()
    {
        InitPos = transform.localPosition;
        animator = GetComponent<Animator>();
        lipSync = GetComponent<LipSync>();
        renderers = GetComponentsInChildren<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        //CurrentData = new LipSyncData();
        if (audioSource != null)
            lipSync.audioSource = audioSource;
        skinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();

        //ActiveRenderer(false);
    }

    public void LoadLipSyncData(string path)
    {
        LipSyncData[] data = Resources.LoadAll<LipSyncData>("Sound/LipSyncData/" + path);
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sound/LipSyncData/" + path);
        LipDataList.Clear();
        LipClipList.Clear();
        for (int i = 0; i < data.Length; i++)
        {
            LipDataList.Add(data[i]);
            
        }
        for (int i = 0; i < clips.Length; i++)
        {
            LipClipList.Add(clips[i]);
        }
                
    }    
    public void SetTrigger(string name)
    {
        animator.ResetTrigger(name);
        animator.SetTrigger(name);                
    }

    public void Testing()
    {        
        AnimatorClipInfo[] temp = animator.GetCurrentAnimatorClipInfo(0);
        var temp2 = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log(temp2.normalizedTime);        
        Debug.Log(temp[0].clip.length);
        Debug.Log(temp[0].clip.length * temp2.normalizedTime);
    }

    public void SetAnimation(string aniName, int layerIndex = 0, float start = 0.0f)
    {
        animator.Play(aniName, layerIndex, start);
    }

    public void SetVoice(string fileName)
    {
        AudioClip clip;

        CurrentData = LipDataList.Find(x => x.name == fileName);
        clip = LipClipList.Find(x => x.name == fileName);

        //lipSync.audioSource.clip = clip;
    }

    public void PlayLipSyncAndSound()
    {
        if(CurrentData != null)
            lipSync.Play(CurrentData);        
    }

    public void StopAnimation()
    {
        //animator.
    }

    public void SetActive(bool val)
    {
        gameObject.SetActive(val);
    }
    public void ActiveRenderer(bool val)
    {
        for(int i = 0; i < renderers.Length; i++)
        {            
            renderers[i].enabled = val;            
        }
        for(int i = 0; i < skinnedMeshRenderer.Length; i++)
        {
            skinnedMeshRenderer[i].enabled = val;            
        }
    }
    public void SetPosition(float x , float y, float z)        
    {
        transform.localPosition = new Vector3(x, y, z);
    }
    public void SetPosition(Vector3 vec)
    {
        transform.localPosition = vec;
    }
    public void WaitForSecAndAniPlay(string aniName, float term = 0)
    {
        WaitForSecAndAniPlayIE = WaitForSecAndAniPlayRoutine(aniName, term);
        StartCoroutine(WaitForSecAndAniPlayIE);
    }
    public void StopWaitForSecAndAniPlay()
    {
        if (WaitForSecAndAniPlayIE != null)
            StopCoroutine(WaitForSecAndAniPlayIE);
    }
    public void WaitForSecAndAction(float[] terms, params UnityAction[] actions)
    {
        WaitForSecAndActionIE = WaitForSecAndActionRoutine(terms, actions);
        StartCoroutine(WaitForSecAndActionIE);

    }
    IEnumerator WaitForSecAndAniPlayRoutine(string aniName, float term)
    {
        yield return new WaitForSeconds(term);

        animator.SetTrigger(aniName);
    }
    private IEnumerator WaitForSecAndActionRoutine(float[] terms, params UnityAction[] actions)
    {
        WaitForSeconds[] tSecs = new WaitForSeconds[terms.Length];
        for (int i = 0; i < terms.Length; i++)
        {
            tSecs[i] = new WaitForSeconds(terms[i]);
        }

        for (int i = 0; i < terms.Length; i++)
        {
            yield return tSecs[i];

            actions[i].Invoke();
        }

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            SetTrigger("hair_motion");
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Testing();
        }
    }
}
