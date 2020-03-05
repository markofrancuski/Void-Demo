using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelManager : MonoBehaviour
{

    [SerializeField] private Image _buttonImage;
    [SerializeField] private Sprite[] _soundSprites;

    private void OnEnable()
    {
        //Check sound settings
        UpdateSoundImage();
    }

    public void OnHomeButtonClick()
    {
        InputManager.Instance.UnControlPlayer();

        SceneSwitcher.Instance.LoadMainMenu(0.5f);
    }

    public void OnResetButtonClick()
    {
        InputManager.Instance.UnControlPlayer();
        SceneSwitcher.Instance.LoadLevel(0.5f, Globals.Instance.GetCurrentLevelSceneName());
    }

    public void OnSoundButtonClick()
    {
        Globals.Instance.SoundOn = !Globals.Instance.SoundOn;
        UpdateSoundImage();
    }

    private void UpdateSoundImage()
    {
        if (Globals.Instance.SoundOn)
        {
            _buttonImage.sprite = _soundSprites[0];
        }
        else
        {
            _buttonImage.sprite = _soundSprites[1];
        }
    }

}
