using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Enemy : Entity
{
    private Rigidbody2D rb;
    private bool facingRight = true;

    protected bool canBeHit;
    protected bool inFleeState;
    protected bool canJumpWhenPlayerIsClose;
    protected float initialMovementSpeed;

    [Header("Stats")]
    [SerializeField] private string name;
    [SerializeField] private string description;
    [Range(1, 10)][SerializeField] protected int health = 2;
    [SerializeField] protected bool canJump = true;
    [SerializeField] protected bool canFlip = true;

    [Header("Movement")]
    [Range(3f, 20f)][SerializeField] protected float jumpForce = 10;
    [Range(0f, 2f)][SerializeField] protected float movementSpeed = 0.05f;
    [Range(0f, 0.3f)][SerializeField] protected float movementSmoothing = 0.05f;
    [Range(1f, 15f)][SerializeField] protected float fleeStateSpeedMultiplier = 2;
    [SerializeField] private LayerMask whatIsGround;
    const float groundedRadius = 0.2f;
    private bool grounded;
    private Vector3 velocity = Vector3.zero;

    [Header("Timers")]
    [Range(1f, 20f)][SerializeField] protected float jumpCycleTimer = 7;
    [Range(1f, 20f)][SerializeField] protected float flipCycleTimer = 11;
    private bool justJumped = false;

    [Header("Rewards")]
    [Range(10, 1000)][SerializeField] protected int scoreValue = 50;
    [Range(1, 5)][SerializeField] protected int scoreMultiplierValue = 1;

    [Header("Component References")]
    [SerializeField] private BoxCollider2D fullBodyCol;
    [SerializeField] private BoxCollider2D jumpBodyCol;
    [SerializeField] private GameObject floatingText;
    private Animator anim;

    protected IEnumerator jumpCycle;
    protected IEnumerator flipCycle;

    protected virtual void Awake()
    {
        initialMovementSpeed = movementSpeed;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected override void Start()
    {
        base.Start();

        canBeHit = true;
        canJumpWhenPlayerIsClose = true;
        inFleeState = false;

        AdjustStats();

        StartTimers();
    }

    protected virtual void AdjustStats()
    {
        int adj = LayoutManager.instance.GetLoopNumber();
        health += (health + 2) * adj;
        jumpForce += adj;
        movementSpeed += adj * 0.02f;
        jumpCycleTimer -= adj * 0.2f;
        flipCycleTimer -= adj * 0.2f;
        fleeStateSpeedMultiplier += adj * 0.75f;
    }

    protected virtual void StartTimers()
    {
        jumpCycle = JumpCycle();
        flipCycle = FlipCycle();

        StartCoroutine(jumpCycle);
        StartCoroutine(flipCycle);
    }

    protected virtual void FixedUpdate()
    {
        HandleLanding();
        if (canJumpWhenPlayerIsClose) PlayerGetsClose();
    }

    protected virtual void Update()
    {
        WalkCycle();
    }

    protected virtual void HandleLanding()
    {
        if (justJumped) return;
        grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(botCol.transform.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                if (anim.GetBool("Jump"))
                {
                    anim.SetBool("Jump", false);
                }
                AdjustCollider(0);
                FlipColliders(false);
                grounded = true;
            }
        }
    }

    protected virtual void WalkCycle()
    {
        Move(movementSpeed);
        if (!anim.GetBool("Jump"))
        {
            anim.SetBool("Walk", true);
        }
        else anim.SetBool("Walk", false);
    }

    protected virtual void Move(float move)
    {
        if (!facingRight) move = -move;
        Vector3 targetVelocity = new Vector2(move * 10f, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);
        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();
    }

    public virtual void Jump(float power)
    {
        if (!canJump) return;
        AudioManager.instance.PlaySound("Enemy_Jump");
        StartCoroutine(ResetGroundCheck());
        if (!anim.GetBool("Jump"))
        {
            anim.SetBool("Jump", true);
            anim.SetBool("Walk", false);
        }
        AdjustCollider(0.4f);
        FlipColliders(true);
        if (movementSpeed < 2) movementSpeed *= 1.1f;
        rb.velocity = new Vector2(rb.velocity.x, power);
    }

    private IEnumerator ResetGroundCheck()
    {
        justJumped = true;
        yield return new WaitForSeconds(0.1f);
        justJumped = false;
    }

    public virtual IEnumerator DoubleJump(float power)
    {
        Jump(power);
        yield return new WaitForSeconds(0.5f);
        Jump(power);
    }

    public virtual IEnumerator TripleJump(float power)
    {
        Jump(power);
        yield return new WaitForSeconds(0.5f);
        Jump(power / 1.5f);
        yield return new WaitForSeconds(0.5f);
        Jump(power / 1.5f);
    }

    public virtual void Flip()
    {
        if (!canFlip) return;
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    protected virtual void AdjustCollider(float pos)
    {
        if (botCol == null) return;
        botCol.transform.localPosition = new Vector2(0, pos);
    }

    protected virtual void FlipColliders(bool jump)
    {
        fullBodyCol.enabled = !jump;
        jumpBodyCol.enabled = jump;
    }

    protected virtual void PlayerGetsClose()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, 2);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.GetComponent<PlayerController>())
            {
                int chanceToJump = Random.Range(1, 5);
                if (chanceToJump == 1) Jump(jumpForce);
                StartCoroutine(ResetProximityCheck());
            }
        }
    }

    protected virtual IEnumerator FleeState()
    {
        inFleeState = true;

        //canBeHit = false;
        Flip();
        movementSpeed = initialMovementSpeed;
        movementSpeed *= fleeStateSpeedMultiplier;
        StopCoroutine(flipCycle);

        yield return new WaitForSeconds(1.5f);

        canBeHit = true;

        yield return new WaitForSeconds(5f);

        StartCoroutine(flipCycle);
        movementSpeed = initialMovementSpeed;

        inFleeState = false;
    }

    protected virtual IEnumerator ResetProximityCheck()
    {
        canJumpWhenPlayerIsClose = false;
        yield return new WaitForSeconds(5f);
        canJumpWhenPlayerIsClose = true;
    }

    protected virtual IEnumerator JumpCycle()
    {
        while (true)
        {
            float randTimer = Random.Range(jumpCycleTimer - 1, jumpCycleTimer + 1);
            yield return new WaitForSeconds(randTimer);
            Jump(jumpForce);
        }
    }

    protected virtual IEnumerator FlipCycle()
    {
        while (true)
        {
            float randTimer = Random.Range(flipCycleTimer - 1, flipCycleTimer + 1);
            yield return new WaitForSeconds(randTimer);
            Flip();
        }
    }

    public override void OnHitOther()
    {
        Jump(jumpForce);
    }

    public override void OnTakeHit()
    {
        if (!canBeHit) return;

        health -= PlayerStats.instance.attack;
        AudioManager.instance.PlaySound("Player_Hits_Enemy");
        StartCoroutine(FlashOnHit());

        if (health <= 0)
        {
            Death();
        }
        else
        {
            int scoreChange = scoreValue / 5;
            PlayerStats.instance.ChangeScore(scoreChange);
            ShowFloatingText(1f, scoreChange * PlayerStats.instance.scoreMultiplier);

            PlayerStats.instance.ChangeScoreMultiplier(scoreMultiplierValue);
            if (!inFleeState) StartCoroutine(FleeState());
            else Flip();
        }
    }

    protected virtual IEnumerator FlashOnHit()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.15f);
        this.GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.15f);
        this.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.15f);
        this.GetComponent<SpriteRenderer>().enabled = true;
    }

    protected virtual void ShowFloatingText(float offset, float score)
    {
        GameObject g = Instantiate(floatingText, this.transform.position, this.transform.rotation);
        g.transform.position = new Vector2(g.transform.position.x, g.transform.position.y + offset);
        g.GetComponent<TextMesh>().text = score.ToString();
    }

    protected virtual void Death()
    {
        AudioManager.instance.PlaySound("Enemy_Death");

        int scoreChange = scoreValue;
        PlayerStats.instance.ChangeScore(scoreChange);
        ShowFloatingText(0.5f, scoreChange * PlayerStats.instance.scoreMultiplier);

        PlayerStats.instance.ChangeScoreMultiplier(scoreMultiplierValue);
        LayoutManager.instance.ChangeEnemyAmount(-1);

        Destroy(this.gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Collidable>()) collision.GetComponent<Collidable>().OnTriggerInteraction(this.gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>()) Flip();
        if (collision.gameObject.GetComponent<UpgradeItem>()) Flip();
        if (collision.gameObject.GetComponent<PlayerController>()) Flip();
    }
}
