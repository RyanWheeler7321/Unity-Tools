using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MAIN : MonoBehaviour
{
    public bool canPause = false;

    public bool usingController = false;

    public bool debugCommands = false;

    public bool cursorActive = false;

    public AudioMixer mixer;

    public static MAIN x;

    private void Awake()
    {
        if (x)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            x = this;
        }

        DontDestroyOnLoad(gameObject);

        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        if (x == this)
        {
            x = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (debugCommands)
        {
            DebugCommands();
        }
    }

    #region  Debug

    void DebugCommands()
    {
        if (!Keyboard.current.leftShiftKey.IsActuated())
            return;

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            Debug.Log("Timescale 1");

            Time.timeScale = 1f;
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            Debug.Log("Timescale 16");

            Time.timeScale = 16f;
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            Debug.Log("Timescale 4");

            Time.timeScale = 4f;
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            Debug.Log("Timescale 0.1");

            Time.timeScale = 0.1f;
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("Reloading current scene");

            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(index);
        }
        if (Keyboard.current.leftBracketKey.wasPressedThisFrame)
        {
            Debug.Log("Development Build Error Console set to Visible");

            Debug.developerConsoleVisible = true;
        }
    }

    #endregion

    #region UI and Cursor States

    public void SetCursor(bool active)
    {
        cursorActive = active;

        if (!usingController)
        {
            Cursor.lockState = active ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = active;
        }
    }

    #endregion

    #region  Level Loading

    public bool loadingNewLevel;

    public void LoadLevel(string name, Color startFadeColor)
    {
        if (loadingNewLevel)
            return;

        loadingNewLevel = true;
        StartCoroutine(LoadSceneAsync(name, startFadeColor));
    }

    public void LoadLevelInstant(string name)
    {
        if (loadingNewLevel)
            return;

        loadingNewLevel = true;
        StartCoroutine(LoadSceneInstantAsync(name));
    }

    AsyncOperation LoadMyScene(string name)
    {
        return SceneManager.LoadSceneAsync(name);
    }

    public Image loadingBar;


    private IEnumerator LoadSceneAsync(string name, Color startFadeColor)
    {
        Fade(startFadeColor, Color.black, 0.5f);

        yield return new WaitForSeconds(0.5f);

        AsyncOperation asyncOperation = LoadMyScene(name);
        asyncOperation.allowSceneActivation = false;

        float fillAmount = 0f;

        // Loading progress is reported between 0 and 0.9
        while (asyncOperation.progress < 0.9f)
        {
            // Lerp the fill amount for smooth transition
            fillAmount = asyncOperation.progress;
            loadingBar.fillAmount = fillAmount;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        asyncOperation.allowSceneActivation = true;
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);


        loadingBar.fillAmount = 1f;

        yield return new WaitForSeconds(0.2f);


        loadingBar.fillAmount = 0f;

        Fade(Color.black, Color.clear, 0.5f);

        loadingNewLevel = false;

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator LoadSceneInstantAsync(string name)
    {
        AsyncOperation asyncOperation = LoadMyScene(name);
        asyncOperation.allowSceneActivation = false; // Prevent Unity from loading the scene when it's ready

        // Wait for the scene to be almost loaded (90% done in Unity)
        while (asyncOperation.progress < 0.9f)
        {
            yield return null;
        }

        // To control when the scene is actually activated, we set allowSceneActivation to true only after the new scene is ready.
        asyncOperation.allowSceneActivation = true;

        // Wait until the scene is fully active
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        loadingNewLevel = false;
    }

    public void LoadLevel(int index, Color startFadeColor)
    {
        if (loadingNewLevel)
            return;

        loadingNewLevel = true;

        // Get the scene path by the build index
        string scenePath = SceneUtility.GetScenePathByBuildIndex(index);

        // Extract the scene name from the path (last part after the last '/')
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        StartCoroutine(LoadSceneAsync(sceneName, startFadeColor));
    }

    #endregion

    #region  Fading

    public Image faderImage;

    private Coroutine currentFadeCoroutine;

    public void Fade(Color startColor, Color fadeColor, float time)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeCor(startColor, fadeColor, time));
    }

    public IEnumerator FadeCor(Color startColor, Color fadeColor, float time)
    {
        float timer = 0;
        while (timer < time)
        {
            faderImage.color = Color.Lerp(startColor, fadeColor, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        faderImage.color = fadeColor;

        currentFadeCoroutine = null;
    }


    #endregion

}
