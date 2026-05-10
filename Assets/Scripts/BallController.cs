using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    private TrailRenderer trail;

    [Header("Base Movement")]
    public float jumpForce = 12f;
    public float moveForce = 8f;

    [Header("Power-Up Durations")]
    public float boostDuration = 3f;
    public float shieldDuration = 5f;
    public float superBounceDuration = 4f;

    [Header("Power-Up Multipliers")]
    public float boostMultiplier = 2.2f;       // Boost: speed & jump multiplier
    public float superBounceMultiplier = 2.5f; // SuperBounce: jump height multiplier

    // State
    private bool fallingDown;
    private float xinp;
    private int jumpCount = 0;
    private int maxJumps = 1; // becomes 2 with Double Jump

    // Power-up flags & timers
    private bool isBoosted;
    private bool isDoubleJumpActive;
    private bool isShielded;
    private bool isSuperBouncing;

    private float boostTimer;
    private float shieldTimer;
    private float superBounceTimer;

    // Visual feedback (assign in Inspector or leave null)
    [Header("Visual FX (optional)")]
    public GameObject shieldVFX;
    public TrailRenderer boostTrail;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        xinp = Input.acceleration.x;

        // Tick power-up timers
        TickTimers();

        // Keyboard fallback for editor testing
#if UNITY_EDITOR
        xinp = Input.GetAxis("Horizontal");
#endif
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocityY <= 0f)
            fallingDown = true;

        float currentMoveForce = isBoosted ? moveForce * boostMultiplier : moveForce;
        rb.AddForce(new Vector2(xinp, 0f) * currentMoveForce * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") && fallingDown)
        {
            PerformBounce();
        }
    }

    // ─── Core Bounce ────────────────────────────────────────────────
    private void PerformBounce()
    {
        float force = jumpForce;

        if (isBoosted)       force *= boostMultiplier;
        if (isSuperBouncing) force *= superBounceMultiplier;

        // Zero out downward velocity before impulse for consistent feel
        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        fallingDown = false;
        jumpCount = 1; // one jump used (the bounce)
    }

    // ─── BUTTON: Boost ──────────────────────────────────────────────
    /// Call from UI Button OnClick()
    public void ActivateBoost()
    {
        isBoosted = true;
        boostTimer = boostDuration;

        if (boostTrail != null) boostTrail.emitting = true;
        Debug.Log("[PowerUp] Boost activated!");
    }

    // ─── BUTTON: Double Jump ────────────────────────────────────────
    /// Call from UI Button OnClick()
    public void ActivateDoubleJump()
    {
        isDoubleJumpActive = true;
        maxJumps = 2;

        // Immediately perform the mid-air extra jump if already airborne
        if (fallingDown && jumpCount < maxJumps)
        {
            jumpCount++;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            fallingDown = false;
            Debug.Log("[PowerUp] Double Jump used mid-air!");
        }
        else
        {
            Debug.Log("[PowerUp] Double Jump ready!");
        }
    }

    // ─── BUTTON: Shield ─────────────────────────────────────────────
    /// Call from UI Button OnClick()
    public void ActivateShield()
    {
        isShielded = true;
        shieldTimer = shieldDuration;

        if (shieldVFX != null) shieldVFX.SetActive(true);
        Debug.Log("[PowerUp] Shield activated!");
    }

    // ─── BUTTON: Super Bounce ───────────────────────────────────────
    /// Call from UI Button OnClick()
    public void ActivateSuperBounce()
    {
        isSuperBouncing = true;
        superBounceTimer = superBounceDuration;
        Debug.Log("[PowerUp] Super Bounce activated!");
    }

    // ─── Shield: absorb a bad collision (extend here as needed) ────
    /// Returns true if shield blocked the event, then consumes it.
    public bool TryBlockWithShield()
    {
        if (!isShielded) return false;
        isShielded = false;
        shieldTimer = 0f;
        if (shieldVFX != null) shieldVFX.SetActive(false);
        Debug.Log("[PowerUp] Shield absorbed hit!");
        return true;
    }

    // ─── Timer Ticking ──────────────────────────────────────────────
    private void TickTimers()
    {
        if (isBoosted)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosted = false;
                if (boostTrail != null) boostTrail.emitting = false;
                Debug.Log("[PowerUp] Boost expired.");
            }
        }

        if (isShielded)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0f)
            {
                isShielded = false;
                if (shieldVFX != null) shieldVFX.SetActive(false);
                Debug.Log("[PowerUp] Shield expired.");
            }
        }

        if (isSuperBouncing)
        {
            superBounceTimer -= Time.deltaTime;
            if (superBounceTimer <= 0f)
            {
                isSuperBouncing = false;
                Debug.Log("[PowerUp] Super Bounce expired.");
            }
        }
    }

    // ─── Public State Getters (for UI) ──────────────────────────────
    public bool IsBoosted        => isBoosted;
    public bool IsDoubleJumping  => isDoubleJumpActive;
    public bool IsShielded       => isShielded;
    public bool IsSuperBouncing  => isSuperBouncing;

    public float BoostTimeLeft       => boostTimer;
    public float ShieldTimeLeft      => shieldTimer;
    public float SuperBounceTimeLeft => superBounceTimer;
}