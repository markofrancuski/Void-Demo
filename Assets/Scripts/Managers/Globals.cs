using UnityEngine;
using Pixelplacement;
using System.Collections.Generic;
using MEC;
using System;

public class Globals : Singleton<Globals>
{
    [Header("Sound Settings")]
    public bool SoundOn;

    [Header("Movement Values")]
    /// <summary>
    /// Value that shows how much you move on Vertical Axis.
    /// Gets changed based on screen size.
    /// </summary>
    public float movePaceVertical;

    /// <summary>
    /// Value that shows how much you move on Horizontal Axis.
    /// Gets changed based on screen size.
    /// </summary>
    public float movePaceHorizontal;

    /// <summary>
    /// Cached Vector value for moving Up.
    /// </summary>
    public Vector3 upVector;
    /// <summary>
    /// Cached Vector value for moving Down.
    /// </summary>
    public Vector3 downVector;
    /// <summary>
    /// Cached Vector value for moving Right.
    /// </summary>
    public Vector3 rightVector;
    /// <summary>
    /// Cached Vector value for moving Left.
    /// </summary>
    public Vector3 leftVector;

    public Vector2 scaleSize;

    public float horizontalBoundary;
    public float verticalBoundary;

    public float tweenDuration = 0.5f;
    private float _bossTweenDuration = 0.1f;
    private float _normalTweenDuration = 0.5f;

    [Header("Level/Chapter Transition Values")]
    public float LevelMoveSpeed = .1f;
    public int currentChapter;
    public int currentLevel;

    public GameObject levelGO;

    public bool isSceneReady;

    [Header("Chapter Info")]
    [Tooltip("Information about chapters and level( chapter info size means the number of the chapters and for each chapter you can define level prefabs)")]
    [SerializeField] private ChapterInfo[] chapterInfo;
    /// <summary>
    /// 
    /// </summary>
    /// <returns> Sum of the integer value of all levels in current Chapter. </returns>
    public int GetNumberOfLevelsInCurrentChapter()
    {
       
        if(currentChapter != -1) return chapterInfo[currentChapter - 1].levels.Count; // return currentChapter + 1 || currentChapter
        return -1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns> Integer value the number of Chapters. </returns>
    public int GetNumberOfChapters()
    {
        return chapterInfo.Length;
    }

    /// <summary>
    /// Switches to the next scene in chapter or if there is no more levels in current chapter. Back to Main Menu.
    /// </summary>
    public void GoToNextLevelInChapter()
    {
        if(currentChapter == 5) SceneSwitcher.Instance.LoadMainMenu(1f);
        int val = chapterInfo[currentChapter-1].levels.Count;
        //There are more levels in Current Chapter
        if (val > currentLevel)
        {
            currentLevel++;
            SceneSwitcher.Instance.LoadLevel(2f, GetCurrentLevelSceneName());
            //Destroy(levelGO);            
        }
        //No more levels in Current Chapter
        else
        {
            //There are more Chapters
            if(currentChapter + 1 <= chapterInfo.Length)
            {
                currentLevel = 1;
                currentChapter++;
                //SceneSwitcher.Instance.LoadMainMenu(2f);
                SceneSwitcher.Instance.LoadLevel(2f, GetCurrentLevelSceneName());
                //Load Next Chapter
            }
            //No more Chapters
            //End of the game
            /*else
            {
                SceneSwitcher.Instance.LoadMainMenu(2f);
            }*/
        }
        
    }

    /// <summary>
    /// Spawns the current level based on which chapter and level we choose.
    /// </summary>
    public void SpawnCurrentLevel()
    {
        Instantiate(chapterInfo[currentChapter-1].levels[currentLevel - 1], Vector2.zero, Quaternion.identity);
        /*switch (currentChapter)
        {
            case 1: if (chapter1[currentLevel - 1] != null) Instantiate(chapter1[currentLevel - 1], Vector2.zero, Quaternion.identity); break;
            case 2: if (chapter2[currentLevel - 1] != null) Instantiate(chapter2[currentLevel - 1], Vector2.zero, Quaternion.identity); break;
            case 3: if (chapter3[currentLevel - 1] != null) Instantiate(chapter3[currentLevel - 1], Vector2.zero, Quaternion.identity); break;
            case 4: if (chapter4[currentLevel - 1] != null) Instantiate(chapter4[currentLevel - 1], Vector2.zero, Quaternion.identity); break;
            case 5: if (chapter5[currentLevel - 1] != null) Instantiate(chapter5[currentLevel - 1], Vector2.zero, Quaternion.identity); break;

            default: break;
        }*/
    }

    /// <summary>
    /// In case you need to reset current level.
    /// </summary>
    /// <returns> Returns the name of the current level scene for reloading that scene.</returns> 
    public string GetCurrentLevelSceneName()
    {
        return  "Level_" + currentChapter + "_" + currentLevel;
    }

    /// <summary>
    /// Function to Check if there are more chapters.
    /// </summary>
    /// <returns> True if there are more chapters to be played, false for no more chapters.</returns>
    public bool IsEnd()
    {
        if (currentChapter + 1 > chapterInfo.Length) return true;
        return false;
    }

    public void ChangeTweenSpeed(bool isBossLevel)
    {
        if (isBossLevel) tweenDuration = _bossTweenDuration;
        else tweenDuration = _normalTweenDuration;
    }

    [Serializable]
    class ChapterInfo
    {
        public List<GameObject> levels;
    }
}
