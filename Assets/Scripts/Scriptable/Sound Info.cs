using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Sound Info", menuName ="Values/Sound Info", order = 0)]
[Serializable]
public class SoundInfo : ScriptableObject
{

    public string Key;
    public AudioClip[] Clips;

    public void SetUpValues(string Key, AudioClip[] clips)
    {
        this.Key = Key;
        this.Clips = clips;
    }

}
