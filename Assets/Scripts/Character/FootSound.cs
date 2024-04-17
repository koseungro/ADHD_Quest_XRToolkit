using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class FootSound : MonoBehaviour {

    private AudioSource source;
    private AudioClip[] clips;
    [SerializeField]
    private bool IsFootLeft = false;
    private bool IsStep = false;
    private bool IsFirst = true;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        clips = UIManager.Inst.FindAllEffectSound("Footstep");
	}
    private void OnTriggerEnter(Collider col)
    {        
        if(IsFirst)
        {
            IsFirst = false;
            return;
        }

        if (!IsStep)
        {            
            AudioClip clip = clips[IsFootLeft ? 0 : 1];
            source.clip = clip;
            source.Play();
        }
        IsStep = true;

    }

    private void OnTriggerExit(Collider col)
    {        
        IsStep = false;
    }
}
