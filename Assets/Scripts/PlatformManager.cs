using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    // Fields
    // For moving camera
    public float speed;
    public float maxSpeed;
    public float DeathSpeedMultiplier;
    public float WallJumpSpeedMultiplier;
    public GameObject player;
    public bool stoppedMovingPlatforms;

    // For resetting platform position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes variables
        maxSpeed = speed;
        stoppedMovingPlatforms = false;
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlatforms();
    }

    // Moves platforms (normal and ondeath)
    private void MovePlatforms()
    {
        if (player.GetComponent<Player>().isDead)
        {
            // Slows platforms
            speed = Mathf.MoveTowards(speed, 0, DeathSpeedMultiplier * Time.deltaTime);

            transform.Translate(Vector2.left * speed * Time.deltaTime);

            if (speed == 0)
            {
                stoppedMovingPlatforms = true;
            }
        }
        else
        {
            // Checks to stop platforms when player enters wall jump area
            if (player.GetComponent<Player>().isInWallJumpSection)
            {
                speed = Mathf.MoveTowards(speed, 0, WallJumpSpeedMultiplier * Time.deltaTime);
            }
            else
            {
                speed = Mathf.MoveTowards(speed, maxSpeed, WallJumpSpeedMultiplier * Time.deltaTime);
            }

            // Moves platforms to the left
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
    }

    // Resets platform positions
    public void Reset()
    {
        speed = maxSpeed;
        stoppedMovingPlatforms = false;
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
    }
}
