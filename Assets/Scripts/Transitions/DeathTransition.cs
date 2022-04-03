using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathTransition : MonoBehaviour
{
    // Fields
    public Image currentTransitionImage;
    public float transitionSpeed = 2f;
    public GameObject mainCamera;
    public GameObject player;
    public Sprite[] transitionImages;

    // Closes vs opens circle
    private bool shouldOpen;
    private bool startedCloseTransition;
    private bool startedOpenTransition;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes variables
        currentTransitionImage = GetComponent<Image>();
        currentTransitionImage.material.SetFloat("_Cutoff", 1.1f);
        shouldOpen = false;
        startedCloseTransition = true;
        startedOpenTransition = true;
        SwitchTransition();
    }

    // Update is called once per frame
    void Update()
    {
        // Starts the circle transition after the camera has stopped moving after death
        if (mainCamera.GetComponent<CameraFollow>().stoppedMovingCamera)
        {
            startedCloseTransition = false;
        }

        TransitionDeathCircle();
    }

    private void TransitionDeathCircle()
    {
        // Checks if circle should open or close
        if (shouldOpen)
        {
            // Checks if the transition has already started
            if (startedOpenTransition == false)
            {
                // Gradually opens circle death transition (move to 1.1 just to be safe b/c 1 might have artifacts)
                currentTransitionImage.material.SetFloat("_Cutoff",
                    Mathf.MoveTowards(currentTransitionImage.material.GetFloat("_Cutoff"), 1.1f, transitionSpeed * Time.deltaTime));

                // Checks if circle is fully opened
                if (currentTransitionImage.material.GetFloat("_Cutoff") == 1.1f)
                {
                    // Sets circle up to close upon next death
                    startedOpenTransition = false;
                    shouldOpen = false;

                    // Changes transition image
                    SwitchTransition();
                }
            }
        }
        else
        {
            // Checks if the transition has already started
            if (startedCloseTransition == false)
            {
                // Gradually closes circle death transition
                currentTransitionImage.material.SetFloat("_Cutoff",
                    Mathf.MoveTowards(currentTransitionImage.material.GetFloat("_Cutoff"), -.1f - currentTransitionImage.material.GetFloat("_Smoothing"),
                    transitionSpeed * Time.deltaTime));


                // Checks if circle is fully closed
                if (currentTransitionImage.material.GetFloat("_Cutoff") == -.1f - currentTransitionImage.material.GetFloat("_Smoothing"))
                {
                    // Opens circle after fully closed
                    startedOpenTransition = false;
                    startedCloseTransition = true;
                    shouldOpen = true;

                    // Resets the level
                    GameObject.Find("SceneManager").GetComponent<SceneMan>().ResetLevel();
                }
            }
        }
    }

    // Changes death transition
    private void SwitchTransition()
    {
        int newTransition = Random.Range(0, transitionImages.Length - 1);

        currentTransitionImage.material.SetTexture("_TransitionEffect", transitionImages[newTransition].texture);
    }
}
