using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceManager : MonoBehaviour
{
   [SerializeField] private AudioSource audioSource;
   private AudioClipData lastaudioClip = null;

    public void playAudioClip(AudioClipData clipData)
    {   
        
        if(clipData == lastaudioClip)   //If it's same do not change the settings just play it.
        {
            audioSource.Play();
            return;
        }

        //Else 
        audioSource.clip = clipData.audioclip;
        audioSource.loop = clipData.loopOn;
        audioSource.volume = clipData.volume;
        audioSource.pitch = clipData.pitchValue;

        audioSource.Play();
    }
}
