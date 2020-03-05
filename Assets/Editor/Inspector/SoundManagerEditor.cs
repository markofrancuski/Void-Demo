using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{

    public const string SAVE_PATH = "/Scriptable";

    List<SoundInfo> infos = new List<SoundInfo>();

    public string Key;
    public List<AudioClip> audioClips = new List<AudioClip>();
    public AudioClip clip;

    private void Awake()
    {
        //dictionary = (target as SoundManager).dictionary;
        Debug.Log("Awake SoundManagerEditor!");
        LoadScriptables();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Dictionary Key");
        GUILayout.Label("Dictionary Value");
        GUILayout.EndHorizontal();

        foreach (var item in infos)
        {
            GUILayout.BeginHorizontal();
            //Key
            GUILayout.Label(item.Key);

            //Array Of Clips
            GUILayout.BeginVertical();
            for (int i = 0; i < item.Clips.Length; i++)
            {
                GUILayout.Label(item.Clips[i].name);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }
        
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Sound Information"))
        {
            if (Key == string.Empty) return;

            CreateNewScriptable(Key, audioClips.ToArray());
            //dictionary.Add();
            audioClips.Clear();
        }
        if (GUILayout.Button("Add Audio Clip"))
        {
            if (clip == null) return;

            audioClips.Add(clip);
            clip = null;
        }

        GUILayout.EndHorizontal();

        Key = EditorGUILayout.TextField("Choose your key",Key);

        clip = EditorGUILayout.ObjectField("Choose Prefab Level to Edit", clip, typeof(AudioClip), false) as AudioClip;

        /*if (GUILayout.Button("Expload"))
        {
            ExplosiveEnemy enemy = target as ExplosiveEnemy;
            enemy.StartTicking();
        }*/
    }

    private void LoadScriptables()
    {
        SoundInfo[] sounds = Resources.LoadAll<SoundInfo>(SAVE_PATH);
        Debug.Log(sounds.Length);

        for (int i = 0; i < sounds.Length; i++)
        {
            infos.Add(sounds[i]);
        }
        

    }

    private void CreateNewScriptable(string Key, AudioClip[] clips)
    {

        SoundInfo sound = CreateInstance<SoundInfo>();
        sound.SetUpValues(Key, clips);

        AssetDatabase.CreateAsset(sound, "Assets/Resources/Scriptable/"+SAVE_PATH);

    }
}


