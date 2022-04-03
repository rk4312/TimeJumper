using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private enum Direction { Left = -1, Right = 1 }

    // Audio
    private AudioMan audioMan;

    // Player fields
    public bool isGrounded;
    public float jumpForce;
    public float speed;
    private float maxSpeed;
    public bool isDead;
    public bool isDeadFromFall;
    public bool isDeadInsidePlatform;
    public bool isDeadfromVoid;
    private Rigidbody2D playerRigidBody;

    // Animation
    public Animator animator;
    public bool deathCodeRan;

    // Wall slide
    public bool isTouchingWall;
    private bool isWallSliding;
    public float wallSlideSpeed;

    // Wall jump
    private bool isWallJumping;
    public float xWallForce;
    public float yWallForce;
    public float wallJumpTime;
    private Direction direction;
    public GameObject platformManager;
    public bool isInWallJumpSection; //(stop platforms)

    // Coyote time
    public float coyoteTime;
    private float coyoteJumpTimer;
    private bool releasedJumpInAir;

    // Charge jump
    private float chargeTimer;
    public float maxChargeTimer;
    public float chargeJumpMultiplier;
    public float chargeJumpDrag;

    // Charge jump particles
    public ParticleSystem[] chargingParticles;
    private bool startedChargingParticles;
    public ParticleSystem[] chargeJumpParticles;

    // Distinguish charge jump from normal jump
    private bool jumpHeld;
    public float holdJumpDelay;
    private float holdJumpDelayTimer;

    // Coyote charge jump
    private bool pressedChargeJumpInAir;
    private float coyoteChargeJumpTimer;
    public bool shouldChargeJump;
    private bool isChargeJumping;

    // Time switch mask
    public float expandMultiplier;
    private bool shouldExpandMask;
    public int maxMaskSize;
    public float initialMaskSize;
    private GameObject sceneManager;

    // For resetting player position
    private Vector2 initialPos;

    // Bool for time switch
    private bool timeSwitchOnCooldown;
    private bool canTimeSwitch;

    // Start is called before the first frame update
    void Start()
    {
        // Initializes vars
        // General player vars
        speed = 0;
        maxSpeed = platformManager.GetComponent<PlatformManager>().speed;
        isDead = false;
        isDeadInsidePlatform = false;
        isDeadFromFall = false;
        isGrounded = false;
        direction = Direction.Right;
        playerRigidBody = gameObject.GetComponent<Rigidbody2D>();

        // Audio
        audioMan = FindObjectOfType<AudioMan>();

        // Animation
        deathCodeRan = false;

        // Wall jump/slide
        isTouchingWall = false;
        isWallSliding = false;
        isWallJumping = false;
        isInWallJumpSection = false; //(stop platforms)

        // Coyote time
        coyoteJumpTimer = 0;
        releasedJumpInAir = false;

        // Charge jump
        chargeTimer = 0;
        holdJumpDelayTimer = 0;
        jumpHeld = false;
        pressedChargeJumpInAir = false;
        shouldChargeJump = false;
        isChargeJumping = false;
        startedChargingParticles = false;

        // Time switch mask
        shouldExpandMask = false;
        sceneManager = GameObject.Find("SceneManager");

        // initial positions
        initialPos = transform.position;

        // Time Switch bool initialization
        timeSwitchOnCooldown = false;
        canTimeSwitch = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Gets user inputs
        if (isDead == false)
        {
            Inputs();
            WallSlide();
            CoyoteTimeJump();
            CoyoteTimeChargeJump();
        }
        else
        {
            if (deathCodeRan == false)
            {
                // Stops player from moving when dead (except from falling)
                if (isDeadFromFall == false)
                {
                    playerRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
                    playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;

                    // Animates player death
                    animator.SetTrigger("Death");
                    animator.SetBool("isDeadFromVoid", isDeadfromVoid);
                }

                deathCodeRan = true;
            }
        }
        ExpandTimeSwitchMask();
    }

    // For moving the player rigidbody
    private void FixedUpdate()
    {
        if (isDead == false)
        {
            MoveDuringWallJumpSections();
        }
    }

    // Key inputs
    private void Inputs()
    {
        if (GameObject.Find("SceneManager").GetComponent<SceneMan>().isPaused == false)
        {
            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Checks if player pressed space in air (used for charge jump, not normal jumps)
                if (!isGrounded && !isTouchingWall)
                {
                    pressedChargeJumpInAir = true;
                }
                else
                {
                    pressedChargeJumpInAir = false;
                }

                jumpHeld = false;
            }
            // Handles charge and normal jump (when let go of space)
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                // Stops charging sound
                audioMan.Stop("Charging");

                // Normal jump (no hold)
                if (!jumpHeld)
                {
                    // Handles normal and wall jumps
                    NormalAndWallJump();

                    if (!isGrounded && !isTouchingWall)
                    {
                        releasedJumpInAir = true;
                    }
                }
                // Charge jump
                else
                {
                    // Handles charge jump
                    if (coyoteChargeJumpTimer <= coyoteTime || shouldChargeJump)
                    {
                        ChargeJump();
                        shouldChargeJump = false;
                        isChargeJumping = true;
                        animator.SetBool("isCharging", shouldChargeJump);

                        // Stops the charging particle system
                        foreach (ParticleSystem particles in chargingParticles)
                        {
                            if (particles.isPlaying == true)
                            {
                                particles.Clear();
                                particles.Stop();
                            }
                        }
                        startedChargingParticles = false;

                        // Animates charge jump and starts particle system for charge jump if the charge is enough
                        if (chargeTimer >= .15f)
                        {
                            chargeJumpParticles[0].transform.up = new Vector3((int)direction * (chargeTimer * chargeJumpMultiplier + maxSpeed),
                                 chargeTimer * chargeJumpMultiplier, 0).normalized;
                            foreach (ParticleSystem particles in chargeJumpParticles)
                            {
                                particles.Clear();
                                particles.Play();
                            }

                            // Animates charge jump
                            animator.SetBool("isChargeJumping", isChargeJumping);
                        }
                    }
                }

                // Resets hold timer
                jumpHeld = false;
                holdJumpDelayTimer = 0;

                // Resets coyote charge jump
                coyoteChargeJumpTimer = 0;
                pressedChargeJumpInAir = false;

                // Resets charging timer
                chargeTimer = 0;
            }

            //  Charge jump (when holding)
            if (Input.GetKey(KeyCode.Space))
            {
                holdJumpDelayTimer += Time.deltaTime;

                // Only slows player down if they are going to charge jump
                if (holdJumpDelayTimer >= holdJumpDelay)
                {
                    // Slows player down
                    if (coyoteChargeJumpTimer <= coyoteTime || shouldChargeJump)
                    {
                        if (isGrounded == true)
                        {
                            // Plays charging sound
                            audioMan.PlaySingleInstance("Charging");

                            transform.Translate(Vector2.left * chargeJumpDrag * Time.deltaTime);
                            shouldChargeJump = true;

                            // Animates charging
                            animator.SetBool("isCharging", shouldChargeJump);

                            // Starts the charging particle system
                            if (startedChargingParticles == false)
                            {
                                foreach (ParticleSystem particles in chargingParticles)
                                {
                                    if (particles.isStopped == true)
                                    {
                                        particles.Clear();
                                        particles.Play();
                                    }
                                }
                                startedChargingParticles = true;
                            }
                        }
                        else if (isWallSliding)
                        {
                            // Plays charging sound
                            audioMan.PlaySingleInstance("Charging");

                            shouldChargeJump = true;

                            // Starts the charging particle system
                            if (startedChargingParticles == false)
                            {
                                foreach (ParticleSystem particles in chargingParticles)
                                {
                                    if (particles.isStopped == true)
                                    {
                                        particles.Clear();
                                        particles.Play();
                                    }
                                }
                                startedChargingParticles = true;
                            }
                        }
                    }

                    // Increments charge timer
                    if (chargeTimer <= maxChargeTimer)
                    {
                        chargeTimer += Time.deltaTime;
                    }
                    jumpHeld = true;
                }
            }

            // Time Switch
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (!timeSwitchOnCooldown && canTimeSwitch)
                {
                    audioMan.Play("Time Switch");
                    Invoke("ResetTimeSwitchCooldown", .35f);
                    shouldExpandMask = true;
                    sceneManager.GetComponent<SceneMan>().ChangeTimeState();
                    timeSwitchOnCooldown = true;
                }
            }
        }
    }

    // Normal and wall jump
    private void NormalAndWallJump()
    {
        // Normal jump
        if (isGrounded == true)
        {
            // Plays jump sound
            audioMan.Play("Jump");

            // Jump
            playerRigidBody.AddForce(new Vector2(0, jumpForce));

            // Resets coyote jump timer
            releasedJumpInAir = false;
            coyoteJumpTimer = 0;
        }
        // Wall jump
        else if (isWallSliding == true)
        {
            // Plays jump sound
            audioMan.Play("Jump");

            isWallJumping = true;
            animator.SetBool("isWallJumping", isWallJumping);

            // Switches direction
            if (direction == Direction.Right)
            {
                direction = Direction.Left;
                animator.SetBool("isRightTrue", false);
            }
            else
            {
                direction = Direction.Right;
                animator.SetBool("isRightTrue", true);
            }

            // Resets player velocity and wall jumps
            playerRigidBody.velocity = Vector2.zero;
              playerRigidBody.AddForce(
                new Vector2(xWallForce * (int)direction -
                platformManager.GetComponent<PlatformManager>().speed,
                yWallForce));

            // Resets coyote jump timer
            releasedJumpInAir = false;
            coyoteJumpTimer = 0;
        }
    }

    // Charge jump
    private void ChargeJump()
    {
        if (isGrounded == true)
        {
            if (chargeTimer >= .15)
            {
                // Plays random charge jump sound
                audioMan.sounds[Random.Range(1, 3)].source.Play();
            }
            else
            {
                // Plays jump sound
                audioMan.Play("Jump");
            }

            // Charge jump
            playerRigidBody.AddForce(new Vector2(chargeTimer * chargeJumpMultiplier,
                jumpForce + chargeTimer * chargeJumpMultiplier));

            // Resets coyote charge jump timer
            pressedChargeJumpInAir = false;
            coyoteChargeJumpTimer = 0;
        }
        else if (isWallSliding == true)
        {
            if (chargeTimer >= .15)
            {
                // Plays random charge jump sound
                audioMan.sounds[Random.Range(1, 3)].source.Play();
            }
            else
            {
                // Plays jump sound
                audioMan.Play("Jump");
            }

            isWallJumping = true;
            animator.SetBool("isWallJumping", isWallJumping);

            // Switches direction
            if (direction == Direction.Right)
            {
                direction = Direction.Left;
                animator.SetBool("isRightTrue", false);
            }
            else
            {
                direction = Direction.Right;
                animator.SetBool("isRightTrue", true);
            }

            // Resets player velocity and wall jumps
            playerRigidBody.velocity = Vector2.zero;

            // Wall jump (changes x speed depending on the platform speed)
            // Platform speed will usually be 0, but player can wall jump while it's 
            // slowing/speeding up to a stop/movespeed
            playerRigidBody.AddForce(
                new Vector2(xWallForce * (int)direction -
                platformManager.GetComponent<PlatformManager>().speed,
                yWallForce + chargeTimer * chargeJumpMultiplier * 4));

            // Resets coyote charge jump timer
            pressedChargeJumpInAir = false;
            coyoteChargeJumpTimer = 0;
        }
    }

    // Wall slide
    private void WallSlide()
    {
        // Checks if player is wall sliding
        if (isTouchingWall && !isGrounded)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        // Slides player down
        if (isWallSliding)
        {
            // Clamps the player's y velocity
            // Only do this if player's falling to prevent it from dragging on jump
            if (playerRigidBody.velocity.y <= 0)
            {
                // Slides down at slower speed when charge jumping
                if (jumpHeld)
                {
                    playerRigidBody.velocity = new Vector2(
                        playerRigidBody.velocity.x,
                        Mathf.Clamp(playerRigidBody.velocity.y, -wallSlideSpeed / 10, float.MaxValue));
                }
                // Slides down at normal speed
                else
                {
                    if (Input.GetKey(KeyCode.S))
                    {
                        playerRigidBody.velocity = new Vector2(
                        playerRigidBody.velocity.x,
                        Mathf.Clamp(playerRigidBody.velocity.y, -wallSlideSpeed * 5, float.MaxValue));
                    }
                    else
                    {
                        playerRigidBody.velocity = new Vector2(
                        playerRigidBody.velocity.x,
                        Mathf.Clamp(playerRigidBody.velocity.y, -wallSlideSpeed, float.MaxValue));
                    }
                }
            }
        }
    }

    // Moves player during wall jump sections when grounded
    private void MoveDuringWallJumpSections()
    {
        if (isInWallJumpSection == true && isWallJumping == false && isTouchingWall == false)
        {
            // Calculates correct speed for player to be at
            speed = maxSpeed - platformManager.GetComponent<PlatformManager>().speed;

            if (speed + playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed >= maxSpeed &&
                playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed < maxSpeed &&
                playerRigidBody.velocity.x > 0)
            {
                speed = maxSpeed - playerRigidBody.velocity.x - platformManager.GetComponent<PlatformManager>().speed;
            }

            // Moves player to the right
            if (speed + playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed <= maxSpeed)
            {
                Vector3 newVelocity = playerRigidBody.velocity;
                newVelocity.x += speed;
                playerRigidBody.velocity = newVelocity;
            }
        }
        else if (isInWallJumpSection == false && playerRigidBody.velocity.x >= 0 && speed != 0)
        {
            // Calculates correct speed for player to be at
            speed = maxSpeed - platformManager.GetComponent<PlatformManager>().speed;

            if (speed + playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed >= maxSpeed &&
                playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed < maxSpeed &&
                playerRigidBody.velocity.x > 0)
            {
                speed = maxSpeed - playerRigidBody.velocity.x - platformManager.GetComponent<PlatformManager>().speed;
            }

            // Moves player to the right
            if (speed + playerRigidBody.velocity.x + platformManager.GetComponent<PlatformManager>().speed <= maxSpeed)
            {
                Vector3 newVelocity = playerRigidBody.velocity;
                newVelocity.x += speed;
                playerRigidBody.velocity = newVelocity;
            }
        }
    }

    // Coyote time for jumping
    private void CoyoteTimeJump()
    {
        if (releasedJumpInAir)
        {
            coyoteJumpTimer += Time.deltaTime;

            // Resets timer
            if (coyoteJumpTimer >= coyoteTime)
            {
                releasedJumpInAir = false;
                coyoteJumpTimer = 0;
            }
            else
            {
                // Handles normal and wall jumps
                NormalAndWallJump();
            }
        }
    }

    // Coyote time for charge jumping
    private void CoyoteTimeChargeJump()
    {
        if (pressedChargeJumpInAir)
        {
            coyoteChargeJumpTimer += Time.deltaTime;
        }
    }

    // Expands time switch mask
    private void ExpandTimeSwitchMask()
    {
        if (shouldExpandMask == true)
        {
            gameObject.transform.Find("TimeSwitchMask").transform.localScale +=
                new Vector3(expandMultiplier * Time.deltaTime, expandMultiplier * Time.deltaTime, 0);

            // Resets variables after finished expanding
            if (gameObject.transform.Find("TimeSwitchMask").transform.localScale.x >= maxMaskSize)
            {
                shouldExpandMask = false;

                // Switches which platforms should be rendered inside and outside mask
                sceneManager.GetComponent<SceneMan>().SwapTimeMasks();

                // Resets mask
                gameObject.transform.Find("TimeSwitchMask").transform.localScale = new Vector2(initialMaskSize, initialMaskSize);
            }
        }
    }

    // Collision enter listener
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision with floor
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            direction = Direction.Right;
            isChargeJumping = false;
            isWallJumping = false;
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isChargeJumping", isChargeJumping);
            animator.SetBool("isWallJumping", isWallJumping);
            animator.SetBool("isRightTrue", true);

            // Plays landing sound
            if (sceneManager.GetComponent<SceneMan>().currentLevelTimeState == TimeState.Past)
            {
                audioMan.Play("Land Past");
            }
            else
            {
                audioMan.Play("Land Future");
            }
        }
        // Collision with wall
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
            animator.SetBool("isWallSliding", isTouchingWall);
            isWallJumping = false;
            animator.SetBool("isWallJumping", isWallJumping);

            // Plays landing sound
            if (isInWallJumpSection)
            {
                if (sceneManager.GetComponent<SceneMan>().currentLevelTimeState == TimeState.Past)
                {
                    audioMan.Play("Land Past");
                }
                else
                {
                    audioMan.Play("Land Future");
                }
            }
        }
    }

    // Collision stay listener
    void OnCollisionStay2D(Collision2D collision)
    {
        // Collision with floor
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            direction = Direction.Right;
            isWallJumping = false;
        }
        // Collision with wall
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }
    }

    // Collision exit listener
    void OnCollisionExit2D(Collision2D collision)
    {
        // Collision with floor
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
            animator.SetBool("isGrounded", isGrounded);
        }
        // Collision with wall
        else if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = false;
            animator.SetBool("isWallSliding", isTouchingWall);
        }
    }

    // Enters but don't collide
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Collision with collider indicating end of level. We switch the scene here.
        if (collision.gameObject.CompareTag("EndLevelZone"))
        {
            // Saves the latest level player is at
            if (SceneManager.GetActiveScene().buildIndex + 1 > PlayerPrefs.GetInt("latestLevel", 1))
            {
                PlayerPrefs.SetInt("latestLevel", SceneManager.GetActiveScene().buildIndex + 1);
            }

            GameObject.Find("UIManager").GetComponent<UIManager>().RunLevelCompleteScene(
                sceneManager.GetComponent<SceneMan>().currentLevelTimeState, SceneManager.GetActiveScene().buildIndex + 1);
        }

        // Collision with stop camera (wall jumping)
        else if (collision.gameObject.CompareTag("StopCamera"))
        {
            isInWallJumpSection = true;
        }

        // Entering time switch zone. Should enable the ability to time switch
        else if (collision.gameObject.CompareTag("TimeSwitchZone"))
        {
            canTimeSwitch = true;
        }

        // Entering death zone. Should kill the player and trigger reset method
        else if (collision.gameObject.CompareTag("DeathZone"))
        {
            // Plays transition sound
            if (isDead == false)
            {
                audioMan.Play("Transition");
            }

            isDead = true;
            isDeadFromFall = true;
        }

        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Plays transition sound
            if (isDead == false)
            {
                audioMan.Play("Transition");
            }

            isDead = true;
            isDeadfromVoid = true;
        }
        else if (collision.gameObject.CompareTag("InsidePlatformKill"))
        {
            // Plays transition sound
            if (isDead == false)
            {
                audioMan.Play("Transition");
            }

            isDead = true;
            isDeadInsidePlatform = true;
        }
    }

    // Exits trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Collision with stop camera (wall jumping)
        if (collision.gameObject.CompareTag("StopCamera"))
        {
            isInWallJumpSection = false;
        }

        // Exiting time switch zone. Should disable the ability to time switch
        else if (collision.gameObject.CompareTag("TimeSwitchZone"))
        {
            canTimeSwitch = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Entering time switch zone. Should enable the ability to time switch
        if (collision.gameObject.CompareTag("TimeSwitchZone"))
        {
            canTimeSwitch = true;
        }
    }

    // Resets cooldown for Time Switch
    private void ResetTimeSwitchCooldown()
    {
        timeSwitchOnCooldown = false;
    }

    // Resets player positions
    public void Reset()
    {
        speed = 0;
        isDead = false;
        isDeadFromFall = false;
        isGrounded = true;
        isInWallJumpSection = false;
        isTouchingWall = false;
        isWallSliding = false;
        isWallJumping = false;
        direction = Direction.Right;
        coyoteJumpTimer = 0;
        releasedJumpInAir = false;
        chargeTimer = 0;
        holdJumpDelayTimer = 0;
        jumpHeld = false;
        pressedChargeJumpInAir = false;
        coyoteChargeJumpTimer = 0;
        shouldChargeJump = false;
        isChargeJumping = false;
        transform.position = initialPos;
        timeSwitchOnCooldown = false;
        canTimeSwitch = false;
        shouldExpandMask = false;
        isDeadInsidePlatform = false;
        playerRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        deathCodeRan = false;

        // Resets the animation variables
        animator.SetBool("isGrounded", true);
        animator.SetBool("isChargeJumping", false);
        animator.SetBool("isCharging", false);
        animator.SetBool("isWallSliding", false);
        animator.SetBool("isWallJumping", false);
        animator.SetBool("isRightTrue", true);
        animator.ResetTrigger("Death");
        animator.Play("Run");

        // Stops charging sound
        audioMan.Stop("Charging");

        // Stops the charging particle system
        foreach (ParticleSystem particles in chargingParticles)
        {
            if (particles.isPlaying == true)
            {
                particles.Clear();
                particles.Stop();
            }
        }
        startedChargingParticles = false;
        foreach (ParticleSystem particles in chargeJumpParticles)
        {
            if (particles.isPlaying == true)
            {
                particles.Clear();
                particles.Stop();
            }
        }
    }
}