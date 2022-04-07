using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="AudioClipData",menuName ="AudioClipData/AudioClipData",order = 1)]
public class AudioClipData : ScriptableObject
{
    public AudioClip audioclip;
    [Range(0,1)]
    public float volume = 1;
    [Range(-3,3)]
    public float pitchValue = 1;
    public bool loopOn = false;
}
