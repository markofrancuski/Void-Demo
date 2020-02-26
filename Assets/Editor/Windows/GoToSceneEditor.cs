using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GoToSceneEditor : EditorWindow
{

    [MenuItem("Window/Custom Windows/Scene Picker")]
    public static void ShowWindow()
    {
        GetWindow<GoToSceneEditor>("Scene Picker");
    }

    private void OnGUI()
    {
        if(GUILayout.Button("Splash Scene"))
        {
            //Load Splash Scene
            EditorSceneManager.OpenScene("Assets/Scenes/Splash/SplashScene.unity");
        }

        if(GUILayout.Button("Main Menu"))
        {
            //Load Main Menu
            EditorSceneManager.OpenScene("Assets/Scenes/Main/MainMenu.unity");
        }

        if (GUILayout.Button("Chapter 6 Test Scene"))
        {
            //Load Main Menu
            EditorSceneManager.OpenScene("Assets/Scenes/Chapter_6/Level_6_1.unity");
        }
        if (GUILayout.Button("Double Prefab"))
        {
            //Load Main Menu
            EditorSceneManager.OpenScene("Assets/Scenes/Chapter_6/Level_6_1.unity");
        }

        if (GUILayout.Button("Boss Scene"))
        {
            //Load Main Menu
            EditorSceneManager.OpenScene("Assets/Scenes/Chapter_5/Level_5_1.unity");
        }

    }
}
