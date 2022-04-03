using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Enum declaration for past and future elements
public enum TimeState
{
    Past,
    Future
}

public class SceneMan : MonoBehaviour
{
    // Fields
    // Determines which time state the level is in
    public TimeState currentLevelTimeState;
    public TimeState defaultLevelTimeState;

    // Main objects
    public GameObject player;
    public GameObject enemy;
    public GameObject mainCamera;
    public GameObject platformManager;
    public GameObject[] arrayOfParallax;
    private GameObject pastManager;
    private GameObject futureManager;
    private GameObject pastParallax;
    private GameObject futureParallax;
    private GameObject pastParticles;
    private GameObject futureParticles;
    public bool isPaused;

    // Child components of those objects
    private SpriteRenderer[] pastSrArray;
    private SpriteRenderer[] futureSrArray;
    private TilemapRenderer[] futureTrArray;
    private SpriteRenderer[] pastParallaxArray;
    private SpriteRenderer[] futureParallaxArray;
    private BoxCollider2D[] pastBoxColliders;
    private PolygonCollider2D[] pastPolyColliders;
    private EdgeCollider2D[] pastEdgeColliders;
    private BoxCollider2D[] futureBoxColliders;
    private PolygonCollider2D[] futurePolyColliders;
    private EdgeCollider2D[] futureEdgeColliders;

    // Start is called before the first frame update
    void Start()
    {
        // Finds all the objects
        pastManager = GameObject.Find("PastManager");
        futureManager = GameObject.Find("FutureManager");
        pastParallax = GameObject.Find("PastBackgroundManager");
        futureParallax = GameObject.Find("FutureBackgroundManager");
        pastParticles = GameObject.Find("PastParticles");
        futureParticles = GameObject.Find("FutureParticles");
        isPaused = false;

        // Finds all the child components of those objects
        pastSrArray = pastManager.GetComponentsInChildren<SpriteRenderer>();
        futureSrArray = futureManager.GetComponentsInChildren<SpriteRenderer>();
        futureTrArray = futureManager.GetComponentsInChildren<TilemapRenderer>();
        pastParallaxArray = pastParallax.GetComponentsInChildren<SpriteRenderer>();
        futureParallaxArray = futureParallax.GetComponentsInChildren<SpriteRenderer>();
        pastBoxColliders = pastManager.GetComponentsInChildren<BoxCollider2D>();
        pastPolyColliders = pastManager.GetComponentsInChildren<PolygonCollider2D>();
        pastEdgeColliders = pastManager.GetComponentsInChildren<EdgeCollider2D>();
        futureBoxColliders = futureManager.GetComponentsInChildren<BoxCollider2D>();
        futurePolyColliders = futureManager.GetComponentsInChildren<PolygonCollider2D>();
        futureEdgeColliders = futureManager.GetComponentsInChildren<EdgeCollider2D>();

        // Sets the appropriate time state
        if (defaultLevelTimeState == TimeState.Past)
        {
            futureManager.SetActive(false);
            futureParallax.SetActive(false);
            futureParticles.SetActive(false);
        }
        else
        {
            pastManager.SetActive(false);
            pastParallax.SetActive(false);
            pastParticles.SetActive(false);
        }
    }

    // Resets the game on death
    public void ResetLevel()
    {
        // Plays fade animation
        GameObject.Find("Panel").GetComponent<Animator>().SetTrigger("Start Fade");

        // Gets all the parallax layers
        arrayOfParallax = GameObject.FindGameObjectsWithTag("Parallax");

        // Resets scene
        mainCamera.GetComponent<CameraFollow>().Reset();
        player.GetComponent<Player>().Reset();
        platformManager.GetComponent<PlatformManager>().Reset();
        enemy.GetComponent<Enemy>().Reset();
        // Checks if masks should be switched
        if (currentLevelTimeState != defaultLevelTimeState)
        {
            SwapTimeMasks();
            ChangeTimeState();
        }

        // Looping through all the parallax layers and calling reset on them
        for (int i = 0; i < arrayOfParallax.Length; i++)
        {
            arrayOfParallax[i].GetComponent<ParallaxEffect>().Reset();
        }
    }

    // Changes time state
    public void ChangeTimeState()
    {
        switch (currentLevelTimeState)
        {
            case TimeState.Future:
                // Changes time state
                currentLevelTimeState = TimeState.Past;

                // Turns off future colliders and enables past colliders
                foreach (BoxCollider2D boxCollider in futureBoxColliders)
                {
                    boxCollider.enabled = false;
                }
                foreach (PolygonCollider2D polyCollider in futurePolyColliders)
                {
                    polyCollider.enabled = false;
                }
                foreach (EdgeCollider2D edgeCollider in futureEdgeColliders)
                {
                    edgeCollider.enabled = false;
                }
                foreach (BoxCollider2D boxCollider in pastBoxColliders)
                {
                    boxCollider.enabled = true;
                }
                foreach (PolygonCollider2D polyCollider in pastPolyColliders)
                {
                    polyCollider.enabled = true;
                }
                foreach (EdgeCollider2D edgeCollider in pastEdgeColliders)
                {
                    edgeCollider.enabled = true;
                }

                // Activates the new platforms
                pastManager.SetActive(true);
                pastParallax.SetActive(true);
                pastParticles.SetActive(true);
                break;

            case TimeState.Past:
                // Changes time state
                currentLevelTimeState = TimeState.Future;

                // Turns off future colliders and enables past colliders
                foreach (BoxCollider2D boxCollider in pastBoxColliders)
                {
                    boxCollider.enabled = false;
                }
                foreach (PolygonCollider2D polyCollider in pastPolyColliders)
                {
                    polyCollider.enabled = false;
                }
                foreach (EdgeCollider2D edgeCollider in pastEdgeColliders)
                {
                    edgeCollider.enabled = false;
                }
                foreach (BoxCollider2D boxCollider in futureBoxColliders)
                {
                    boxCollider.enabled = true;
                }
                foreach (PolygonCollider2D polyCollider in futurePolyColliders)
                {
                    polyCollider.enabled = true;
                }
                foreach (EdgeCollider2D edgeCollider in futureEdgeColliders)
                {
                    edgeCollider.enabled = true;
                }

                // Activates the new platforms
                futureManager.SetActive(true);
                futureParallax.SetActive(true);
                futureParticles.SetActive(true);
                break;
        }

        // Swaps fonts
        GameObject.Find("UIManager").GetComponent<UIManager>().SwapMenuFonts();
    }

    // Swaps time state masks upon death if necessary
    // Switches which textures should be rendered inside and outside mask
    public void SwapTimeMasks()
    {
        // Disables the correct manager/parallax/particles
        switch (currentLevelTimeState)
        {
            case TimeState.Future:
                pastManager.SetActive(false);
                pastParallax.SetActive(false);
                pastParticles.SetActive(false);
                break;
            case TimeState.Past:
                futureManager.SetActive(false);
                futureParallax.SetActive(false);
                futureParticles.SetActive(false);
                break;
        }

        // -------------------------PAST PLATFORMS--------------------------
        foreach (SpriteRenderer Sr in pastSrArray)
        {
            if (Sr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        // -------------------------FUTURE PLATFORMS--------------------------
        foreach (SpriteRenderer Sr in futureSrArray)
        {
            if (Sr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        foreach (TilemapRenderer Tr in futureTrArray)
        {
            if (Tr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Tr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Tr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        // -------------------------PAST PARALLAX--------------------------
        foreach (SpriteRenderer Sr in pastParallaxArray)
        {
            if (Sr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        // -------------------------FUTURE PARALLAX--------------------------
        foreach (SpriteRenderer Sr in futureParallaxArray)
        {
            if (Sr.maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                Sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }

        // -------------------------PAST PARTICLES--------------------------
        if (pastParticles.GetComponent<ParticleSystemRenderer>().maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
        {
            pastParticles.GetComponent<ParticleSystemRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
        else
        {
            pastParticles.GetComponent<ParticleSystemRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        // -------------------------FUTURE PARTICLES--------------------------
        if (futureParticles.GetComponent<ParticleSystemRenderer>().maskInteraction == SpriteMaskInteraction.VisibleInsideMask)
        {
            futureParticles.GetComponent<ParticleSystemRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
        else
        {
            futureParticles.GetComponent<ParticleSystemRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }
}
