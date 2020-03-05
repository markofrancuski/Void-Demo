using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _soundPlayer;
    [SerializeField] private AudioSource _musicPlayer;

    [SerializeField] public Dictionary<string, AudioClip[]> dictionary = new Dictionary<string, AudioClip[]>();

    public SoundInfo[] scriptableInfos;

    public void Awake()
    {
        Instance = this;      
    }

    private void Start()
    {
        LoadFromScriptable();

        foreach (var item in dictionary)
        {
            Debug.Log($"Key: {item.Key}, Clips: {item.Value.Length}");
        }
    }
    public void PlaySFX(string name)
    {
        //_soundPlayer.clip = dictionary[name].GetRandomOneClip();
    }

    private void LoadFromScriptable()
    {

        scriptableInfos = Resources.LoadAll<SoundInfo>("/Scriptable");

        dictionary = new Dictionary<string, AudioClip[]>();

        foreach (var info in scriptableInfos)
        {
            dictionary.Add(info.Key, info.Clips);
        }
    }
}

