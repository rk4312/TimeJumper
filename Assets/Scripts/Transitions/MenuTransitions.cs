using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuTransitions : MonoBehaviour
{
    // Fields
    private Image transitionImage;
    public float transitionSpeed = 2f;
    private int sceneToLoad;
    static private float transitionCutoff = 1.1f;

    // Closes vs opens circle
    private bool startedCloseTransition;
    static private bool startedOpenTransition = true;

    // FOR LEVEL COMPLETE SCENE ONLY
    public static TimeState endingTimeState;
    public GameObject levelCompletePastBackground;
    public GameObject levelCompleteFutureBackground;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes variables
        transitionImage = GetComponent<Image>();
        startedCloseTransition = true;
        transitionImage.material.SetFloat("_Cutoff", transitionCutoff);
        transitionImage.material.SetTexture("_TransitionEffect", transitionImage.sprite.texture);

        // Sets the background for level complete scene
        if (SceneManager.GetActiveScene().name == "LevelComplete")
        {
            // Toggles the future and past backgrounds
            if (endingTimeState == TimeState.Future)
            {
                levelCompletePastBackground.SetActive(false);
                levelCompleteFutureBackground.SetActive(true);
            }

            // Disables next level button text if on last level
            if (UIManager.nextLevelIndex >= SceneManager.sceneCountInBuildSettings - 1)
            {
                GameObject nextLevelButton = GameObject.Find("NextLevelButton");
                nextLevelButton.GetComponent<Button>().interactable = false;
                Vector2 newSize = nextLevelButton.GetComponent<RectTransform>().sizeDelta;
                newSize.x = 650;
                nextLevelButton.GetComponent<RectTransform>().sizeDelta = newSize;
                GameObject.Find("NextLevelButton/Next Level Text").GetComponent<TMP_Text>().text = "NEXT LEVEL COMING SOON";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        OpenCircleTransition();
        CloseCircleTransition();
    }

    private void OpenCircleTransition()
    {
        // Checks if the transition has already started
        if (startedOpenTransition == false)
        {
            // Gradually opens circle death transition (move to 1.1 just to be safe b/c 1 might have artifacts)
            transitionCutoff =
                Mathf.MoveTowards(transitionImage.material.GetFloat("_Cutoff"), 1.1f, transitionSpeed * Time.unscaledDeltaTime);
            transitionImage.material.SetFloat("_Cutoff", transitionCutoff);

            // Checks if circle is fully opened
            if (transitionImage.material.GetFloat("_Cutoff") == 1.1f)
            {
                // Sets circle up to close upon next death
                startedOpenTransition = true;
            }
        }
    }

    private void CloseCircleTransition()
    {
        // Checks if the transition has already started
        if (startedCloseTransition == false)
        {
            // Gradually closes circle death transition
            transitionCutoff = Mathf.MoveTowards(transitionImage.material.GetFloat("_Cutoff"),
                -.1f - transitionImage.material.GetFloat("_Smoothing"),transitionSpeed * Time.unscaledDeltaTime);
            transitionImage.material.SetFloat("_Cutoff", transitionCutoff);

            // Checks if circle is fully closed
            if (transitionImage.material.GetFloat("_Cutoff") == -.1f - transitionImage.material.GetFloat("_Smoothing"))
            {
                // Opens circle after fully closed
                startedCloseTransition = true;
                startedOpenTransition = false;

                /*if (GameObject.Find("AudioManager").GetComponent<AudioMan>().mainTheme.source.isPlaying == true)
                {
                    GameObject.Find("AudioManager").GetComponent<AudioMan>().mainTheme.source.Stop();
                }*/

                // Loads in new scene depending on scene name/index
                if (Application.CanStreamedLevelBeLoaded(sceneToLoad))
                {
                    SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
                    Time.timeScale = 1;
                    AudioListener.pause = false;
                }
            }
        }
    }

    public void StartCloseCircleTransition(int sceneIndex)
    {
        sceneToLoad = sceneIndex;
        transitionCutoff = 1.1f;
        transitionImage.material.SetFloat("_Cutoff", transitionCutoff);
        startedCloseTransition = false;
        startedOpenTransition = true;
    }
}
