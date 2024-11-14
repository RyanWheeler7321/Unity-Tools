using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;
using StarterAssets;

public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu instance;

    [Header("UI References")]
    public GameObject optionsMenuMain;
    public HorizontalMenuSelector resSelector;
    public HorizontalMenuSelector qualitySelector;
    public HorizontalMenuSelector musicVolSelector;
    public HorizontalMenuSelector soundVolSelector;
    public HorizontalMenuSelector vsyncSelector;
    public HorizontalMenuSelector fullscreenSelector;
    public HorizontalMenuSelector languageSelector;

    [Header("Audio Settings")]
    public AudioMixer mixer;

    [Header("Other Settings")]
    public string tag = "";

    private Resolution[] resolutions;
    private List<Resolution> realResolutions;
    private bool initialized;
    private bool prevOptions;
    private bool inOptions;
    private float deleteFileTimer;
    private bool quitting;

    private void Awake()
    {
        // Ensure Singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Initialize UI Visual Starting Values
        StartCoroutine(Util.DoNextFrame(() =>
        {
            SetMenuVisualStartingValues();
            ApplySettings();
            optionsMenuMain.SetActive(false);
        }));
    }

    private void Update()
    {
        HandleInput();
    }

    private void LateUpdate()
    {
        prevOptions = inOptions;
    }

    // Enables Options Menu and sets initial values for UI
    public void EnableOptionsMenu()
    {
        inOptions = true;
        optionsMenuMain.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        resSelector.Focus();
        SetMenuVisualStartingValues();
    }

    // Handles disabling the options menu through input
    public void InputDisableOptionsMenu()
    {
        Sound.x.PlayUIBankSound(9, 0.5f, 0.03f, 0.9f, 0.02f, 80, 0);
        DisableOptionsMenu();
    }

    // Disables Options Menu and clears focus from all selectors
    public void DisableOptionsMenu()
    {
        inOptions = false;
        optionsMenuMain.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        UnfocusAllSelectors();
    }

    // Unfocus all horizontal selectors
    private void UnfocusAllSelectors()
    {
        resSelector.UnFocus();
        qualitySelector.UnFocus();
        musicVolSelector.UnFocus();
        soundVolSelector.UnFocus();
        vsyncSelector.UnFocus();
        fullscreenSelector.UnFocus();
        languageSelector.UnFocus();
    }

    // Apply all the currently selected settings
    public void ApplySettings()
    {
        SetQuality(qualitySelector.currentValue);
        SetMusicVolume(musicVolSelector.currentValue);
        SetSoundVolume(soundVolSelector.currentValue);
        SetFullScreen(fullscreenSelector.currentValue);
        SetVsync(vsyncSelector.currentValue);
        SetRes(resSelector.currentValue);
        SetLanguage(languageSelector.currentValue);
    }

    // Sets menu visual values based on PlayerPrefs or defaults
    public void SetMenuVisualStartingValues()
    {
        InitializeResolutions();
        InitializeSelectors();
    }

    // Initialize screen resolutions and selector options
    private void InitializeResolutions()
    {
        realResolutions = new List<Resolution>();
        resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        foreach (var res in resolutions)
        {
            if (TestAspectRatio(res))
            {
                string option = res.width + " x " + res.height + " " + res.refreshRate + "Hz";
                options.Add(option);
                realResolutions.Add(res);
            }
        }

        if (options.Count > 0)
        {
            resSelector.options = options;
        }
        resSelector.UpdateValue(PlayerPrefs.GetInt(tag + "Resolution", realResolutions.Count - 1));
    }

    // Initialize various settings selectors with saved values or defaults
    private void InitializeSelectors()
    {
        qualitySelector.options = new List<string> { Localize.x.Code("LOWUI"), Localize.x.Code("MEDIUMUI"), Localize.x.Code("HIGHUI") };
        qualitySelector.UpdateValue(PlayerPrefs.GetInt(tag + "Quality", 2));

        musicVolSelector.options = soundVolSelector.options = new List<string> { "0", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" };
        musicVolSelector.UpdateValue(PlayerPrefs.GetInt(tag + "MusicVol", 10));
        soundVolSelector.UpdateValue(PlayerPrefs.GetInt(tag + "SoundVol", 10));

        List<string> offOnList = new List<string> { Localize.x.Code("OFFUI"), Localize.x.Code("ONUI") };
        fullscreenSelector.options = vsyncSelector.options = offOnList;
        fullscreenSelector.UpdateValue(PlayerPrefs.GetInt(tag + "FullScreen", 1));
        vsyncSelector.UpdateValue(PlayerPrefs.GetInt(tag + "VSYNC", 1));

        languageSelector.options = new List<string> { "L1", "L2", "L3", "L4", "L5", "L6", "L7", "L8", "L9", "L10", "L11" };
        languageSelector.UpdateValue(PlayerPrefs.GetInt(tag + "Language", 0));
    }

    // Test if a resolution matches acceptable aspect ratios (e.g., 16:9 or 16:10)
    public bool TestAspectRatio(Resolution r)
    {
        float epsilon = 0.0001f;
        float aspectRatio = (float)r.width / r.height;
        float targetAspectRatio = 16f / 9f;
        float targetAspectRatio2 = 16f / 10f;

        return Math.Abs(aspectRatio - targetAspectRatio) < epsilon || Math.Abs(aspectRatio - targetAspectRatio2) < epsilon;
    }

    // Handle input during options menu
    private void HandleInput()
    {
        if (inOptions)
        {
            if (Inputs.x.startPress)
            {
                InputDisableOptionsMenu();
            }
            else if (Inputs.x.interactPress)
            {
                BorderUI.x.borderUIMain.SetBool("Enabled", true);
                BorderUI.x.pauseMenu.SetBool("Enabled", true);
                InputDisableOptionsMenu();
            }
            else if (EventSystem.current.currentSelectedGameObject == null)
            {
                // Placeholder for reselecting first option if needed
            }

            HandleQuitInput();
        }
    }

    // Handle quit logic when holding the start button
    private void HandleQuitInput()
    {
        if (Inputs.x.start)
        {
            deleteFileTimer += Time.unscaledDeltaTime;
            if (deleteFileTimer > 5f && !quitting)
            {
                quitting = true;
                Save.x.DeleteFile();
                // PauseMenu.x.QuitButton();
            }
        }
        else
        {
            deleteFileTimer = 0f;
        }
    }

    // Setting Methods
    public void SetRes(int value)
    {
        Resolution res = realResolutions[value];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt(tag + "Resolution", value);
    }

    public void SetQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
        PlayerPrefs.SetInt(tag + "Quality", value);
    }

    public void SetMusicVolume(int value)
    {
        mixer.SetFloat("MusicVol", value == 0 ? -1000000 : GetVolFromNormal(value / 10f));
        PlayerPrefs.SetInt(tag + "MusicVol", value);
    }

    public void SetSoundVolume(int value)
    {
        float volume = value == 0 ? -1000000 : GetVolFromNormal(value / 10f);
        mixer.SetFloat("SoundVol", volume);
        mixer.SetFloat("UISoundVol", volume);
        PlayerPrefs.SetInt(tag + "SoundVol", value);
    }

    public void SetFullScreen(int value)
    {
        Screen.fullScreen = value == 1;
        PlayerPrefs.SetInt(tag + "FullScreen", value);
    }

    public void SetVsync(int value)
    {
        QualitySettings.vSyncCount = value;
        PlayerPrefs.SetInt(tag + "VSYNC", value);
    }

    public void SetLanguage(int value)
    {
        Localize.x.UpdateLanguageIndex(value + 1);
        SetMenuVisualStartingValues();
    }

    // Converts normalized volume (0-1) to decibel value
    public float GetVolFromNormal(float normalizedVolume)
    {
        return 20f * Mathf.Log10(normalizedVolume);
    }
}
