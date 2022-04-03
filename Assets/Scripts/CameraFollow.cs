
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Fields
    private Player player;
    private PlatformManager platformManager;

    // Following player
    private Vector3 initialPos;
    public Vector3 offset;
    private Vector3 desiredPos;
    private Vector3 smoothedPos;
    public float smoothSpeed = .125f;

    // Death
    public float deathMoveSpeed;
    public float cameraOffsetDeath;
    public bool stoppedMovingCamera;

    // Box collider recentering
    public bool shouldVerticalRecenter;

    private void Start()
    {
        // References
        platformManager = GameObject.Find("PlatformManager").GetComponent<PlatformManager>();
        player = GameObject.Find("Player").GetComponent<Player>();

        // For following player
        initialPos = transform.position;
        desiredPos = Vector3.zero;
        smoothedPos = Vector3.zero;
        shouldVerticalRecenter = false;
    }

    private void Update()
    {
        StopCameraOnDeath();
    }

    // To make camera follow smooth
    private void FixedUpdate()
    {
        FollowPlayer();
    }

    // Follows player
    private void FollowPlayer()
    {
        if (player.isDead == false)
        {
            // Follows player both horizontally and vertically
            if (shouldVerticalRecenter == true)
            {
                desiredPos = player.transform.position + offset;
                smoothedPos = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
                transform.position = smoothedPos;
            }
            // Only follows player horizontally
            else
            {
                desiredPos = player.transform.position + offset;
                desiredPos.y = transform.position.y;
                smoothedPos = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
                transform.position = smoothedPos;
            }  
        }
    }

    // Stops camera when player dies
    void StopCameraOnDeath()
    {
        // Recenters camera on player death
        if (player.isDead == true)
        {
            if (platformManager.stoppedMovingPlatforms)
            {
                // Moves camera towards player if they died (not from fall)
                if (player.isDeadFromFall == false)
                {
                    transform.position = Vector3.MoveTowards(transform.position,
                        new Vector3(player.transform.position.x + cameraOffsetDeath, player.transform.position.y - cameraOffsetDeath * 1.5f, initialPos.z),
                        deathMoveSpeed * Time.deltaTime);

                    // Match x and y pos
                    if (player.isDeadfromVoid == false)
                    {
                        if (Mathf.Abs(transform.position.x - (player.transform.position.x + cameraOffsetDeath)) <= .1 &&
                            Mathf.Abs(transform.position.y - (player.transform.position.y - cameraOffsetDeath * 1.5f)) <= .1)
                        {
                            stoppedMovingCamera = true;
                        }
                    }
                    // Match just x pos
                    else
                    {
                        if (Mathf.Abs(transform.position.x - (player.transform.position.x + cameraOffsetDeath)) <= .1)
                        {
                            stoppedMovingCamera = true;
                        }
                    }
                }
                // Doesn't move camera towards player if they died from fall
                else
                {
                    stoppedMovingCamera = true;
                }
            }
        }
    }

    // Toggles bool that allows vertical recentering
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && player.isGrounded == true)
        {
            shouldVerticalRecenter = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            shouldVerticalRecenter = true;
        }
    }

    // Resets camera
    public void Reset()
    {
        desiredPos = Vector3.zero;
        smoothedPos = Vector3.zero;
        stoppedMovingCamera = false;
        transform.position = initialPos;
        shouldVerticalRecenter = false;
    }
}
