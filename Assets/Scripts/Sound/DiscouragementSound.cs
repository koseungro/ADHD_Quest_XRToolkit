using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DiscouragementSound : Singleton<DiscouragementSound>
{
    [SerializeField]
    private AudioMixerGroup mixer;
    [SerializeField]
    private AudioMixerGroup mixer2;

    public List<AudioSource> audioSource = new List<AudioSource>();
    public List<AudioSource> LoopAudioSource = new List<AudioSource>();
    public List<AudioClip> clipList = new List<AudioClip>();

    private IEnumerator LoopPlayerIE;

    private IEnumerator WaitAndFuncIE;
    private IEnumerator WaitForSecAndPlayIE;
    private IEnumerator WaitForSecAndPlayIE2;
    private IEnumerator WaitForSecAndPlayIE3;


    public void DiscouragementSoundPlay(AudioClip audioClip, int index = 0)
    {
        int i = 0;
        for (; i < audioSource.Count; i++)
        {
            if (audioSource[i].isPlaying)
            {
                continue;
            }
            else
            {
                break;
            }
        }

        if (i == audioSource.Count)
        {
            audioSource.Add(gameObject.AddComponent<AudioSource>());

        }
        //if(index == 0)
        //{
        //    audioSource[i].outputAudioMixerGroup = mixer;
        //}
        //else
        //    audioSource[i].outputAudioMixerGroup = mixer2;

        audioSource[i].clip = audioClip;        
        audioSource[i].Play();
    }

    public void LoopDiscouragementSoundPlay(AudioClip audioClip, float term, float volume = 1, int index = 0)
    {
        int i = 0;
        for (; i < LoopAudioSource.Count; i++)
        {
            if (LoopAudioSource[i].isPlaying)
            {
                continue;
            }
            else
            {
                break;
            }
        }

        if (i == LoopAudioSource.Count)
        {
            LoopAudioSource.Add(gameObject.AddComponent<AudioSource>());
        }

        LoopAudioSource[i].clip = audioClip;
        //if (index == 0)
        //{
        //    LoopAudioSource[i].outputAudioMixerGroup = mixer;
        //}
        //else
        //    LoopAudioSource[i].outputAudioMixerGroup = mixer2;

        LoopAudioSource[i].volume = volume;
        LoopAudioSource[i].Play();

        LoopPlayerIE = LoopDiscouragementSoundPlayRoutine(LoopAudioSource[i], audioClip, term);
        StartCoroutine(LoopPlayerIE);
    }

    IEnumerator LoopDiscouragementSoundPlayRoutine(AudioSource audioSource, AudioClip clip, float term)
    {
        //if (audioSource.isPlaying)
        //    yield return null;

        float time = 0.0f;

        while(true)
        {
            if(!audioSource.isPlaying)
                time += Time.deltaTime;

            if(term <= time)
            {
                time = 0;
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
                yield return null;            
        }     
    }

    public void AllStop()
    {
        StopAllCoroutines();

        for (int i = 0; i < LoopAudioSource.Count; i++)
            LoopAudioSource[i].Stop();
        for (int i = 0; i < audioSource.Count; i++)
            audioSource[i].Stop();
    }
    
    private AudioSource FindSource(int index = 0)
    {
        AudioSource temp = new AudioSource();
        int i = 0;

        for (; i < audioSource.Count; i++)
        {
            if (audioSource[i].isPlaying)
            {
                Debug.Log("audioSource_" + i + "is Playing");
                continue;
            }
            else
            {
                Debug.Log("audioSource_" + i + "is not Playing");
                temp = audioSource[i];
                break;
            }
        }

        if (i == audioSource.Count)
        {
            Debug.Log("Last AudioSource");
            audioSource.Add(gameObject.AddComponent<AudioSource>());
            //if(index == 0)
            //    audioSource[i].outputAudioMixerGroup = mixer;
            //else
            //    audioSource[i].outputAudioMixerGroup = mixer2;

            temp = audioSource[i];
        }
        Debug.Log("i : " + i);
        return temp;
    }

    public void WaitAndFunc(int index = 0, params AudioClip[] clips)
    {
        WaitAndFuncIE = WaitAndFuncRoutine(FindSource(index), clips);
        StartCoroutine(WaitAndFuncIE);
    }

    private IEnumerator WaitAndFuncRoutine(AudioSource audioSource, params AudioClip[] clips)
    {
        WaitForSeconds tSec = new WaitForSeconds(0.3f);
        for(int i = 0; i < clips.Length; i++)
        {
            Debug.Log("audiosource : " + audioSource.name);
            Debug.Log("clips[" + i + "] :" + clips[i].name);
            audioSource.clip = clips[i];
            audioSource.Play();
            yield return tSec;
            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }
    }

    /// <summary>
    /// 일정시간 지난 후 오디오 재생
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="sec"></param>
    /// <param name="volume"></param>
    /// <param name="index"></param>
    public void WaitForSecAndPlay(AudioClip clip, float sec, float volume = 1, int index = 0)
    {        
        Debug.Log("WaitForSecondAndPlay1");
        WaitForSecAndPlayIE = WaitForSecAndPlayRoutine(clip, sec, volume, index);
        StartCoroutine(WaitForSecAndPlayIE);
    }
    /// <summary>
    /// 일정 시간 지난 후 여러개의 클립을 연속 재생 사이 간격 0.5초 텀 있음
    /// </summary>
    /// <param name="sec"></param>
    /// <param name="volume"></param>
    /// <param name="clips"></param>
    public void WaitForSecAndPlay(float sec, float volume = 1, params AudioClip[] clips)
    {
        Debug.Log("WaitForSecondAndPlay2");
        WaitForSecAndPlayIE2 = WaitForSecAndPlayRoutine2(sec, volume, clips);
        StartCoroutine(WaitForSecAndPlayIE2);
    }
    // 일정시간 지난 후 오디오 재생 하는데 지속시간 동안 반복재생
    public void WaitForSecAndPlay(AudioClip clip, float sec, float duration, int index, float volume = 1)
    {
        Debug.Log("WaitForSecondAndPlay3");
        WaitForSecAndPlayIE3 = WaitForSecAndPlayRoutine3(clip, sec, duration, volume, index);
        StartCoroutine(WaitForSecAndPlayIE3);
    }
    public void StopWaitForSecAndPlay()
    {
        if (WaitForSecAndPlayIE != null)
            StopCoroutine(WaitForSecAndPlayIE);
    }
    public void StopWaitForSecAndPlay2()
    {
        if (WaitForSecAndPlayIE2 != null)
            StopCoroutine(WaitForSecAndPlayIE2);
    }

    public void StopWaitForSecAndPlay3()
    {
        if (WaitForSecAndPlayIE3 != null)
            StopCoroutine(WaitForSecAndPlayIE3);
    }

    private IEnumerator WaitForSecAndPlayRoutine(AudioClip clip, float sec, float volume = 1, int index = 0)
    {
        WaitForSeconds tSec = new WaitForSeconds(sec);

        yield return tSec;
        AudioSource audioSource = FindSource(index);
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.Play();
    }
    private IEnumerator WaitForSecAndPlayRoutine2(float sec, float volume, params AudioClip[] clips)
    {
        WaitForSeconds tSec = new WaitForSeconds(sec);
        WaitForSeconds tSec2 = new WaitForSeconds(0.3f);
        yield return tSec;
        
        for(int i = 0; i < clips.Length; i++)
        {
            AudioSource audioSource = FindSource();
            audioSource.volume = volume;
            audioSource.clip = clips[i];
            audioSource.Play();
            yield return tSec2;
            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }
    }
    
    private IEnumerator WaitForSecAndPlayRoutine3(AudioClip clip, float sec, float Duration, float volume = 1 , int index =0)
    {
        WaitForSeconds tSec = new WaitForSeconds(sec);
        float time = 0.0f;
        yield return tSec;
        AudioSource audioSource = FindSource(index);
        audioSource.volume = volume;
        audioSource.clip = clip;        
        audioSource.Play();

        while(time <= Duration)
        {
            time += Time.deltaTime;

            if(audioSource.isPlaying)
            {
                yield return null;
            }
            else
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
        audioSource.Stop();
    }
}
