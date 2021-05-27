using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Public objects
    public CharacterController2D controller;
    public AudioSource dieSound;
    public AudioSource scoreSound;
    public AudioSource jumpPowerSound;
    public AudioSource speedPowerSound;
    public float runSpeedSet = 40f;
    public Vector3 startPos;

    // Private variables
    private float speedMultiplier = 2f;
    private float jumpMultiplier = 1.2f;
    private float runSpeed;
    private int levelupscore = 5;
    float horizontalMovement = 0f;
    bool jump = false;
    private Rigidbody2D rigidBody;
    private float ogJumpForce;
    GameObject santahat;
    GameObject tilak;
    CharacterController2D charController;
    GameManager game;
    Animator animator;

    // Events
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;
    public static event PlayerDelegate OnJumpPowerUp;
    public static event PlayerDelegate OnSpeedPowerUp;
    public static event PlayerDelegate OnLevelUp;

    // Initialize
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.simulated = false;
        runSpeed = runSpeedSet;
        animator = GetComponent<Animator>();
        santahat = GameObject.Find("santahat");
        santahat.SetActive(false);
        tilak = GameObject.Find("BigT");
        tilak.SetActive(false);
    }

    void Start()
    {
        charController = CharacterController2D.Instance;
        ogJumpForce = charController.JumpForce;
        game = GameManager.Instance;

        // Get santahat
    }

    // Move on update
    void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("Jump", true);
        }

        // Set yVelocity in the animator
        animator.SetFloat("yVelocity", rigidBody.velocity.y);
        animator.SetFloat("xVelocity", rigidBody.velocity.x);

        // Toggle Santa Hat
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

            if (santahat.active)
            {
                santahat.SetActive(false);
            }
            else 
            {
                santahat.SetActive(true);
            }
        }

        // Toggle Tilak
        if (Input.GetKeyDown(KeyCode.T) & Input.GetKeyDown(KeyCode.I) & Input.GetKeyDown(KeyCode.L) & Input.GetKeyDown(KeyCode.Alpha4))
        {

            if (tilak.active)
            {
                tilak.SetActive(false);
            }
            else
            {
                tilak.SetActive(true);
            }
        }
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;  // Player clicked start button
        GameManager.OnRestart += OnRestart; // Player clicked restart
    }
    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnRestart -= OnRestart;
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMovement * runSpeed * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    // Register collisions
    void OnTriggerEnter2D(Collider2D col)
    {
        // Mithai points
        if (col.gameObject.tag == "scoreZone")
        {
            OnPlayerScored();
            col.gameObject.SetActive(false);
            scoreSound.Play();

            CheckScore();
        }

        // Ras Malai points
        //if (col.gameObject.tag == "RasMalai")
        //{
        //    OnPlayerScored();
        //    OnPlayerScored();
        //    OnPlayerScored();

        //    col.gameObject.SetActive(false);
        //    scoreSound.Play();

        //    CheckScore();
        //}

        // Kulfi speed up
        if (col.gameObject.tag == "powerZone")
        {
            col.gameObject.SetActive(false);
            StartCoroutine(SpeedUp()); 
            speedPowerSound.Play();

        }

        // Kulfi speed up
        if (col.gameObject.tag == "jumpPower")
        {
            col.gameObject.SetActive(false);
            StartCoroutine(JumpUp()); 
            jumpPowerSound.Play();
        }

        // Die from being eaten by Lips
        if (col.gameObject.tag == "deadZone")
        {
            OnPlayerDied();
            dieSound.Play();

            rigidBody.velocity = Vector3.zero;
            rigidBody.velocity = new Vector3(-15, 15, 0);
            runSpeed = runSpeedSet;

            foreach (var c in GetComponents<CircleCollider2D>())
            {
                c.enabled = false;
            }

        }
    }

    // Player clicks start button
    void OnGameStarted()
    {
        rigidBody.simulated = true;
    }

    void OnRestart()
    {
        foreach (var c in GetComponents<CircleCollider2D>())
        {
            c.enabled = true;
        }
        rigidBody.simulated = false;
        transform.localPosition = startPos;
        rigidBody.velocity = Vector3.zero;
        jump = false;
        runSpeed = runSpeedSet;
        controller.Move(0, false, jump);

    }

    void CheckScore()
    {
        // Level up every n points
        int score = game.GetScore;
        if (score % levelupscore == 0)
        {
            OnLevelUp();
        }
    }
    // Increase speed for t seconds
    IEnumerator SpeedUp()
    {
        OnSpeedPowerUp();
        runSpeed = runSpeedSet * speedMultiplier;
        yield return new WaitForSeconds(2.0f);
        runSpeed = runSpeedSet;
    }

    // Increase jump for t seconds
    IEnumerator JumpUp()
    {
        OnJumpPowerUp();
        charController.JumpForce = ogJumpForce * jumpMultiplier;
        yield return new WaitForSeconds(4.0f);
        charController.JumpForce = ogJumpForce;
    }
}
