using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public enum UICanvas
{
    MainCanvas, LevelSelectCanvas, CreditsCanvas, ControlsCanvas
}

public class UIManager : MonoBehaviour
{
    // Fields
    public GameObject mainCanvas;
    public GameObject levelSelectCanvas;
    public GameObject controlsCanvas;
    public GameObject creditsCanvas;
    public GameObject pauseCanvas;

    // Mask
    private UICanvas UIToDisplay;
    public float expandMultiplier;
    private bool shouldExpandMask;
    public int maxMaskSize;
    private Vector3 initialMaskSize;
    public GameObject mask;
    private MaskUI[] objectsToMask;

    // Text fonts in level
    public TMP_Text[] menuTexts;
    public TMP_FontAsset pastFont;
    public Material pastFontMat;
    public TMP_FontAsset futureFont;
    public Material futureFontMat;

    // Moving to next level
    public static int nextLevelIndex = 2;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes variables in menu scene
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            // Gets all the necessary masking components
            UIToDisplay = UICanvas.LevelSelectCanvas;
            initialMaskSize = mask.transform.localScale;
            shouldExpandMask = false;
            objectsToMask = Resources.FindObjectsOfTypeAll<MaskUI>();

            // Disables clicking on level select
            if (levelSelectCanvas != null)
            {
                levelSelectCanvas.GetComponent<GraphicRaycaster>().enabled = false;
            }
            if (creditsCanvas != null)
            {
                creditsCanvas.GetComponent<GraphicRaycaster>().enabled = false;
            }

            // Starts playing menu music if it isn't already
            FindObjectOfType<AudioMan>().PlaySingleInstance("Cave Ambience 2");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu" &&
            SceneManager.GetActiveScene().name != "LevelCompleteScreen")
        {
            UIKeyboardInputs();
        }
        else
        {
            ExpandMask();
        }
    }

    private void PlayClickSound()
    {
        FindObjectOfType<AudioMan>().Play("Click");
    }

    public void OpenLevelSelect()
    {
        PlayClickSound();
        StartExpandMask(UICanvas.LevelSelectCanvas);
    }

    public void OpenControls()
    {
        PlayClickSound();
        StartExpandMask(UICanvas.ControlsCanvas);
    }

    public void OpenCredits()
    {
        PlayClickSound();
        StartExpandMask(UICanvas.CreditsCanvas);
    }

    public void OpenMenu()
    {
        PlayClickSound();
        StartExpandMask(UICanvas.MainCanvas);
    }

    public void RunLevel(int contentID)
    {
        PlayClickSound();
        if (levelSelectCanvas != null)
        {
            levelSelectCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
        GameObject.Find("MenuTransitionCanvas/MenuTransition").GetComponent<MenuTransitions>().StartCloseCircleTransition(contentID + 1);
    }

    public void RunMenuScene()
    {
        PlayClickSound();
        if (pauseCanvas != null)
        {
            pauseCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
        GameObject.Find("MenuTransitionCanvas/MenuTransition").GetComponent<MenuTransitions>().StartCloseCircleTransition(0);
    }

    public void RunLevelCompleteScene(TimeState endingTimeState, int nextLevel)
    {
        if (mainCanvas != null)
        {
            mainCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
        nextLevelIndex = nextLevel;
        MenuTransitions.endingTimeState = endingTimeState;
        GameObject.Find("MenuTransitionCanvas/MenuTransition").GetComponent<MenuTransitions>().StartCloseCircleTransition(4);
    }

    public void RunNextLevel()
    {
        // Makes sure we don't load in the level complete scene
        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            RunLevel(nextLevelIndex - 1);
        }
    }

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    public void Pause()
    {
        PlayClickSound();
        GameObject.Find("SceneManager").GetComponent<SceneMan>().isPaused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;

        if (pauseCanvas != null && pauseCanvas.activeSelf == false)
        {
            pauseCanvas.SetActive(true);
        }
        if (mainCanvas != null && mainCanvas.activeSelf == true)
        {
            mainCanvas.SetActive(false);
        }
    }

    public void UnPause()
    {
        PlayClickSound();
        GameObject.Find("SceneManager").GetComponent<SceneMan>().isPaused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;

        if (pauseCanvas != null && pauseCanvas.activeSelf == true)
        {
            pauseCanvas.SetActive(false);
        }
        if (mainCanvas != null && mainCanvas.activeSelf == false)
        {
            mainCanvas.SetActive(true);
        }
    }

    // Keyboard UI Inputs
    void UIKeyboardInputs()
    {
        // Pauses the game when user presses escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameObject.Find("SceneManager").GetComponent<SceneMan>().isPaused == false)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }
    }

    // Swaps the fonts of menu texts in the level
    public void SwapMenuFonts()
    {
        foreach (TMP_Text text in menuTexts)
        {
            if (text.font == pastFont)
            {
                text.font = futureFont;
                text.fontMaterial = futureFontMat;
            }
            else
            {
                text.font = pastFont;
                text.fontMaterial = pastFontMat;
            }
        }
    }

    // ----------------------------------EXPAND MENU UI MASKS (ONLY USE IN MENUS)--------------------------------------
    public void StartExpandMask(UICanvas displayCanvas)
    {
        UIToDisplay = displayCanvas;
        shouldExpandMask = true;

        // Disables raycasters on all canvases during transition
        if (mainCanvas != null)
        {
            mainCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
        if (levelSelectCanvas != null)
        {
            levelSelectCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
        if (controlsCanvas != null)
        {
            controlsCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }
        if (creditsCanvas != null)
        {
            creditsCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        }

        // ONLY enables the canvas to display
        switch (UIToDisplay)
        {
            case UICanvas.MainCanvas:
                if (mainCanvas != null)
                {
                    mainCanvas.SetActive(true);
                }
                break;
            case UICanvas.LevelSelectCanvas:
                if (levelSelectCanvas != null)
                {
                    levelSelectCanvas.SetActive(true);
                }
                break;
            case UICanvas.ControlsCanvas:
                if (controlsCanvas != null)
                {
                    controlsCanvas.SetActive(true);
                }
                break;
            case UICanvas.CreditsCanvas:
                if (creditsCanvas != null)
                {
                    creditsCanvas.SetActive(true);
                }
                break;
        }
    }

    private void ExpandMask()
    {
        if (shouldExpandMask == true)
        {
            mask.transform.localScale +=
                new Vector3(expandMultiplier * Time.deltaTime, expandMultiplier * Time.deltaTime, 0);

            // Resets variables after finished expanding
            if (mask.transform.localScale.x >= maxMaskSize)
            {
                shouldExpandMask = false;

                // Swaps render queues
                foreach (MaskUI maskUI in objectsToMask)
                {
                    maskUI.SwapRenderQueue();
                }

                // Disables all but the displayed canvas
                // ONLY enables the canvas to display
                switch (UIToDisplay)
                {
                    case UICanvas.MainCanvas:
                        if (mainCanvas != null)
                        {
                            mainCanvas.GetComponent<GraphicRaycaster>().enabled = true;
                        }
                        if (levelSelectCanvas != null)
                        {
                            levelSelectCanvas.SetActive(false);
                        }
                        if (controlsCanvas != null)
                        {
                            controlsCanvas.SetActive(false);
                        }
                        if (creditsCanvas != null)
                        {
                            creditsCanvas.SetActive(false);
                        }
                        break;
                    case UICanvas.LevelSelectCanvas:
                        if (levelSelectCanvas != null)
                        {
                            levelSelectCanvas.GetComponent<GraphicRaycaster>().enabled = true;
                        }
                        if (mainCanvas != null)
                        {
                            mainCanvas.SetActive(false);
                        }
                        if (controlsCanvas != null)
                        {
                            controlsCanvas.SetActive(false);
                        }
                        if (creditsCanvas != null)
                        {
                            creditsCanvas.SetActive(false);
                        }
                        break;
                    case UICanvas.ControlsCanvas:
                        if (controlsCanvas != null)
                        {
                            controlsCanvas.GetComponent<GraphicRaycaster>().enabled = true;
                        }
                        if (mainCanvas != null)
                        {
                            mainCanvas.SetActive(false);
                        }
                        if (levelSelectCanvas != null)
                        {
                            levelSelectCanvas.SetActive(false);
                        }
                        if (creditsCanvas != null)
                        {
                            creditsCanvas.SetActive(false);
                        }
                        break;
                    case UICanvas.CreditsCanvas:
                        if (creditsCanvas != null)
                        {
                            creditsCanvas.GetComponent<GraphicRaycaster>().enabled = true;
                        }
                        if (mainCanvas != null)
                        {
                            mainCanvas.SetActive(false);
                        }
                        if (levelSelectCanvas != null)
                        {
                            levelSelectCanvas.SetActive(false);
                        }
                        if (controlsCanvas != null)
                        {
                            controlsCanvas.SetActive(false);
                        }
                        break;
                }

                // Resets mask
                mask.transform.localScale = initialMaskSize;
            }
        }
    }
}
