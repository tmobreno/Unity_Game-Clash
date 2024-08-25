using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class PlayerController : Entity
{
    public bool CanMove { get; private set; } = true;

    [Header("Functional Options")]
    [SerializeField] private bool canJump;
    [SerializeField] private bool canInteract;

    [Header("Controls")]
    [SerializeField] private PlayerControls controls;
    private KeyCode moveLeftKey;
    private KeyCode moveRightKey;
    private KeyCode jumpKey;

    [Header("Jump")]
    [SerializeField] private bool airControl;
    [SerializeField] private LayerMask whatIsGround;
    const float groundedRadius = 0.2f;
    private bool grounded;
    private bool justJumped = false;

    [Header("Movement")]
    private bool facingRight = true;
    private Vector3 velocity = Vector3.zero;
    public float currentMovementSpeed { get; private set; }
    public float currentAirModifier { get; private set; }

    private Rigidbody2D rb;
    [SerializeField] BoxCollider2D fullBodyCol, jumpBodyCol;
    private Animator anim;
    private PlayerStats stats;

    public static PlayerController instance;

    private void Awake()
    {
        instance = this;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        LoadControls();
    }

    protected override void Start()
    {
        base.Start();
        stats = PlayerStats.instance;
        currentMovementSpeed = stats.baseMovementSpeed;
    }

    private void LoadControls()
    {
        moveLeftKey = controls.MoveLeft;
        moveRightKey = controls.MoveRight;
        jumpKey = controls.Jump;
    }

    private void FixedUpdate()
    {
        HandleLanding();
    }

    void Update()
    {
        if (CanMove || grounded || airControl) HandleMovement();
        if (canJump) HandleJump();
    }

    private void HandleLanding()
    {
        if (justJumped) return;
        grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(botCol.transform.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                currentAirModifier = stats.baseAirModifier;
                stats.ResetScoreMultiplier();
                if (anim.GetBool("Jump")) anim.SetBool("Jump", false);
                AdjustCollider(0);
                FlipColliders(false);
                grounded = true;
            }
        }
    }

    private void HandleMovement()
    {
        if (grounded) currentMovementSpeed = stats.baseMovementSpeed;
        else currentMovementSpeed = stats.baseMovementSpeed * currentAirModifier;

        if (Input.GetKey(moveLeftKey))
        {
            Move(-currentMovementSpeed);
            if (!anim.GetBool("Walk") && !anim.GetBool("Jump"))
            {
                AudioManager.instance.PlaySound("Player_Walk");
                anim.SetBool("Walk", true);
            }
        }
        else if (Input.GetKey(moveRightKey))
        {
            Move(currentMovementSpeed);
            if (!anim.GetBool("Walk") && !anim.GetBool("Jump"))
            {
                AudioManager.instance.PlaySound("Player_Walk");
                anim.SetBool("Walk", true);
            }
        }
        else
        {
            if (anim.GetBool("Walk"))
            {
                AudioManager.instance.StopSound("Player_Walk");
                anim.SetBool("Walk", false);
            }
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            AudioManager.instance.PlaySound("Player_Jump");
            AudioManager.instance.StopSound("Player_Walk");
            Jump();
            AdjustCollider(0.75f);
            FlipColliders(true);
            if (!anim.GetBool("Jump"))
            {
                anim.SetBool("Jump", true);
                anim.SetBool("Walk", false);
            }
        }
    }

    private void AdjustCollider(float pos)
    {
        botCol.transform.localPosition = new Vector2(0, pos);
    }

    private void FlipColliders(bool jump)
    {
        fullBodyCol.enabled = !jump;
        jumpBodyCol.enabled = jump;
    }

    private void Move(float move)
    {
        Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, stats.movementSmoothing);
        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();
    }

    private void Jump()
    {
        StartCoroutine(ResetGroundCheck());
        if (currentAirModifier < 10.0f) currentAirModifier += stats.baseAirModifier / 3;
        grounded = false;
        rb.velocity = new Vector2(rb.velocity.x, stats.jumpForce);
    }

    private IEnumerator ResetGroundCheck()
    {
        justJumped = true;
        yield return new WaitForSeconds(0.1f);
        justJumped = false;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canInteract && collision.GetComponent<Collidable>()) collision.GetComponent<Collidable>().OnTriggerInteraction(this.gameObject);
    }

    public override void OnTakeHit()
    {
        AudioManager.instance.PlaySound("Enemy_Hits_Player");
        StartCoroutine(FlashOnHit());
        stats.ChangeHealth(-1);
        stats.ResetScoreMultiplier();
        if (stats.health <= 0) Death();
    }

    public virtual IEnumerator FlashOnHit()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.15f);
        this.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void Death()
    {
        AudioManager.instance.StopSound("Player_Walk");
        AudioManager.instance.PlaySound("Game_Over");
        StoredData.instance.UpdateHighScores(stats.score, LayoutManager.instance.GetLoopNumber(), LayoutManager.instance.GetCurrentLevel());
        PlayerUI.instance.UpdateHighScores();
        PlayerUI.instance.ActivatePostText();
        Destroy(this.gameObject);
    }

    public override void OnHitOther()
    {
        AudioManager.instance.PlaySound("Player_Hits_Anything");
        Jump();
    }
}
