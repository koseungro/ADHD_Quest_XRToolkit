using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnvironmentSound : Singleton<EnvironmentSound> {

    [SerializeField]
    private AudioMixerGroup mixer;    
    public List<AudioSource> audioSource = new List<AudioSource>();
    public List<AudioSource> LoopAudioSource = new List<AudioSource>();
    public List<AudioClip> clipList;

    private IEnumerator LoopPlayerIE;
    private IEnumerator ElapsePlayerIE;

    public void SetEnvironmentVolume(float val)
    {
        
        //audioSource.volume = val;
    }

    public void AllLoopStop()
    {
        StopAllCoroutines();
    }    

    public void AllPlayStop()
    {
        for(int i = 0; i < audioSource.Count; i++)
        {
            audioSource[i].Stop();
        }
        for (int i = 0; i < LoopAudioSource.Count; i++)
        {
            LoopAudioSource[i].Stop();
        }
    }

    /// <summary>
    /// 일반적으로 Term 없이 계속 루프 할 수 있는 재생 기능
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="vol"></param>
    /// <param name="isLoop"></param>
    public void EnvironmentSoundPlay(AudioClip audioClip, float vol = 1, bool isLoop = false)
    {
        for(int j = 0; j < audioSource.Count; j++)
        {
            if(audioSource[j].clip.name.Equals(audioClip.name))
            {
                audioSource[j].Stop();
                audioSource[j].clip = null;
            }
        }

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
        if( vol != 1)
        {
            audioSource[i].volume = vol;
        }
        audioSource[i].loop = isLoop;
        audioSource[i].clip = audioClip;
        audioSource[i].Play();
    }
    /// <summary>
    /// Term이 있는 반복 재생 기능
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="term"></param>
    /// <param name="volume"></param>
    public void LoopDiscouragementSoundPlay(AudioClip audioClip, float term, float volume = 1)
    {
        for (int j = 0; j < audioSource.Count; j++)
        {
            if (audioSource[j].clip.name.Equals(audioClip.name))
            {
                audioSource[j].Stop();
                audioSource[j].clip = null;
            }
        }

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

        if(volume != 1)
        {
            LoopAudioSource[i].volume = volume;
        }
        LoopAudioSource[i].outputAudioMixerGroup = mixer;
        LoopAudioSource[i].clip = audioClip;
        LoopAudioSource[i].Play();

        LoopPlayerIE = LoopDiscouragementSoundPlayRoutine(LoopAudioSource[i], audioClip, term);
        StartCoroutine(LoopPlayerIE);
    }
    /// <summary>
    /// 일정시간 동안 재생되는 기능
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="term"></param>
    public void ElapseDiscouragementSoundPlay(AudioClip audioClip, float term)
    {
        ElapsePlayerIE = ElapsePlayerRoutine(FindSource(), audioClip, term);
        StartCoroutine(ElapsePlayerIE);
    }
    public void StopElapseDiscouragementSoundPlay()
    {
        if (ElapsePlayerIE != null)
            StopCoroutine(ElapsePlayerIE);

    }

    private AudioSource FindSource()
    {
        AudioSource temp = new AudioSource();
        int i = 0;
        for (; i < audioSource.Count; i++)
        {
            if (audioSource[i].isPlaying)
            {
                continue;
            }
            else
            {
                temp = audioSource[i];
                break;
            }
        }

        if (i == audioSource.Count)
        {
            audioSource.Add(gameObject.AddComponent<AudioSource>());
            temp = audioSource[i];
        }
        Debug.Log("i : " + i);
        return temp;
    }

    private int FindSourceIndex()
    {
        AudioSource temp = new AudioSource();
        int i = 0;
        for (; i < audioSource.Count; i++)
        {
            if (audioSource[i].isPlaying)
            {
                continue;
            }
            else
            {
                temp = audioSource[i];
                break;
            }
        }

        if (i == audioSource.Count)
        {
            audioSource.Add(gameObject.AddComponent<AudioSource>());
            temp = audioSource[i];
        }
        Debug.Log("i : " + i);
        return i;
    }

    IEnumerator LoopDiscouragementSoundPlayRoutine(AudioSource audioSource, AudioClip clip, float term)
    {
        //if (audioSource.isPlaying)
        //    yield return null;

        float time = 0.0f;

        while (true)
        {
            time += Time.deltaTime;

            if (term <= time)
            {
                time = 0;
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
                yield return null;
        }
    }
    IEnumerator ElapsePlayerRoutine(AudioSource audioSource, AudioClip clip, float term)
    {
        for (int j = 0; j < this.audioSource.Count; j++)
        {
            if (this.audioSource[j].clip.name.Equals(clip.name))
            {
                this.audioSource[j].Stop();
                this.audioSource[j].clip = null;
            }
        }

        if (audioSource.isPlaying)
            audioSource.Stop();

        float time = 0.0f;

        while (true)
        {
            time += Time.deltaTime;

            if (term >= time)
            {
                if (audioSource.isPlaying)
                    yield return null;
                else
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
            else
                break;
        }


        audioSource.clip = clip;
        audioSource.Play();
    }
}
