using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // For moving enemy when platforms stop moving due to wall jumps
    public GameObject player;
    public GameObject platformManager;
    public Transform cameraPos;
    public float playerOffset;
    private float speed;
    public float maxSpeed;
    [HideInInspector]public float wallJumpSpeed;

    // For resetting enemy position
    private float xInitialPos;
    private float yInitialPos;
    private float zInitialPos;

    // Start is called before the first frame update
    void Start()
    {
        wallJumpSpeed = maxSpeed;
        speed = 0;
        xInitialPos = transform.position.x;
        yInitialPos = transform.position.y;
        zInitialPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
        MatchCameraYPos();
    }

    // Starts moving towards player if platforms have stopped
    private void MoveTowardsPlayer()
    {
        if (player.GetComponent<Player>().isInWallJumpSection == true)
        {
            speed = Mathf.MoveTowards(speed,
                wallJumpSpeed,
                platformManager.GetComponent<PlatformManager>().WallJumpSpeedMultiplier * Time.deltaTime);
        }
        else
        {
            speed = Mathf.MoveTowards(speed,
                player.GetComponent<Player>().speed,
                platformManager.GetComponent<PlatformManager>().WallJumpSpeedMultiplier * Time.deltaTime);
        }

        // Moves enemy to the right
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Moves back towards the camera if it's too far away from player
        if (speed == 0 && transform.position.x <= player.transform.position.x - playerOffset)
        {
            // Moves enemy to the right
            transform.Translate(Vector2.right * maxSpeed * 1.5f * Time.deltaTime);
        }
    }

    // Matches the camera's y position
    private void MatchCameraYPos()
    {
        Vector3 targetPos = transform.position;
        targetPos.y = cameraPos.position.y;
        transform.position = targetPos;
    }

    public void Reset()
    {
        transform.position = new Vector3(xInitialPos, yInitialPos, zInitialPos);
        speed = 0;
    }
}
