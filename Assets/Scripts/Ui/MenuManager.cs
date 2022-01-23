using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }
    
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private CanvasGroup buttons = null;
    [SerializeField] private CanvasGroup options = null;
    [SerializeField] private CanvasGroup credits = null;
    [SerializeField] private CanvasGroup popUpCg = null;
    [SerializeField] private Slider masterVolumeSlider = null;
    [SerializeField] private Slider musicVolumeSlider = null;
    [SerializeField] private Slider sfxVolumeSlider = null;
    [SerializeField] private Button popUpAcceptButton = null;
    [SerializeField] private TMP_Text popUpText = null;
    [SerializeField] private bool isStart = false;

    public bool IsOpen => canvasGroup.alpha > 0;
    
    private float _prevTimeScale;
    
    private void Awake()
    {
        Instance = this;
        
        if (!isStart) canvasGroup.Deactivate();
    }

    private void Start()
    {
        masterVolumeSlider.value = LoaderManager.Data.masterVolume;
        musicVolumeSlider.value = LoaderManager.Data.musicVolume;
        sfxVolumeSlider.value = LoaderManager.Data.sfxVolume;
    }

    public void ToggleMenu()
    {
        if (isStart) return;

        if (IsOpen)
        {
            ContinueGame();
            return;
        }

        canvasGroup.Activate();
        _prevTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        canvasGroup.Deactivate();
        Time.timeScale = _prevTimeScale;
    }

    public void NewGame()
    {
        if (isStart)
        {
            LoaderManager.Instance.StartNewGame();
            return;
        }

        popUpCg.Activate();
        popUpText.text = "Si empiezas una nueva partida perderás el progreso. ¿Seguro que quieres eso?";
        popUpAcceptButton.onClick.AddListener(() =>
        {
            LoaderManager.Instance.StartNewGame();
        });
    }
    
    public void CloseGame()
    {
        if (isStart)
        {
            Application.Quit();
            return;
        }
        
        popUpCg.Activate();
        popUpAcceptButton.onClick.AddListener(LoaderManager.Instance.LoadMenu);
    }

    public void HidePopUp()
    {
        popUpCg.Deactivate();
        popUpText.text = "Si sales ahora perderás el progreso. ¿Seguro que quieres eso?";
        popUpAcceptButton.onClick.RemoveAllListeners();
    }

    public void ShowButtons()
    {
        buttons.Activate();
        options.Deactivate();
        credits.Deactivate();
    }

    public void ShowOptions()
    {
        buttons.Deactivate();
        options.Activate();
        credits.Deactivate();
    }

    public void ShowCredits()
    {
        buttons.Deactivate();
        options.Deactivate();
        credits.Activate();
    }

    public void ChangeLanguage(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        LoaderManager.Instance.SaveLanguage(index);
    }

    public void PlayButtonSound()
    {
        AudioManager.instance.PlayButton();
    }

    public void ChangeVolume(int sliderNumber)
    {
        switch (sliderNumber)
        {
            case 0:
                AudioManager.instance.ChangeVolume(masterVolumeSlider.value, "Master Volume");
                LoaderManager.Instance.SaveVolume(masterVolumeSlider.value, sliderNumber);
                break;
            case 1:
                AudioManager.instance.ChangeVolume(musicVolumeSlider.value, "Music Volume");
                LoaderManager.Instance.SaveVolume(musicVolumeSlider.value, sliderNumber);
                break;
            case 2:
                AudioManager.instance.ChangeVolume(sfxVolumeSlider.value, "Fx Volume");
                LoaderManager.Instance.SaveVolume(sfxVolumeSlider.value, sliderNumber);
                break;
        }
        
    }
}
