using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 16f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 0.75f;
    [SerializeField] private TrailRenderer dashTrail;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;

    [Header("References")]
    public GameObject blackSmoke;
    public GameObject bullet;
    public Transform bulletSpawnPoint;

    [Header("Animation")]
    public AnimationClip attackAnim;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Transform blackSmokeTransform;
    private Animator animator;

    private Vector2 lookDirection = Vector2.right;
    private bool isAttacking;
    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;

    private PlayerSoundsProvider playerSoundsProvider;
    private Coroutine dashCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerSoundsProvider = GetComponent<PlayerSoundsProvider>();

        if (dashTrail == null)
            dashTrail = GetComponentInChildren<TrailRenderer>();

        if (dashTrail != null)
            dashTrail.emitting = false;
    }

    private void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Enable();

        if (attackAction != null && attackAction.action != null)
            attackAction.action.Enable();

        if (jumpAction != null && jumpAction.action != null)
            jumpAction.action.Enable();

        if (dashAction != null && dashAction.action != null)
            dashAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Disable();

        if (attackAction != null && attackAction.action != null)
            attackAction.action.Disable();

        if (jumpAction != null && jumpAction.action != null)
            jumpAction.action.Disable();

        if (dashAction != null && dashAction.action != null)
            dashAction.action.Disable();
    }

    private void Update()
    {
        CheckGrounded();
        ReadMoveInput();
        ReadAttackInput();
        ReadJumpInput();
        ReadDashInput();
    }

    private void CheckGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundMask
        ) != null;

        if (animator != null)
        {
            animator.SetBool("isGrounded", isGrounded);
        }
    }

    private void ReadMoveInput()
    {
        if (moveAction == null || moveAction.action == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = moveAction.action.ReadValue<Vector2>();

        SetFlip(moveInput.x);

        bool isWalked = Mathf.Abs(moveInput.x) > 0.01f;

        if (animator != null)
        {
            animator.SetBool("speed", isWalked && !isDashing);
        }
    }

    private void ReadAttackInput()
    {
        if (attackAction == null || attackAction.action == null)
            return;

        if (isDashing)
            return;

        if (attackAction.action.WasPressedThisFrame() && !isAttacking)
        {
            StartCoroutine(MakeBullet());
        }
    }

    private void ReadJumpInput()
    {
        if (jumpAction == null || jumpAction.action == null)
            return;

        if (isDashing)
            return;

        if (jumpAction.action.WasPressedThisFrame() && isGrounded)
        {
            Jump();
        }
    }

    private void ReadDashInput()
    {
        if (dashAction == null || dashAction.action == null)
            return;

        if (dashAction.action.WasPressedThisFrame() && canDash && !isDashing)
        {
            if (dashCoroutine != null)
                StopCoroutine(dashCoroutine);

            dashCoroutine = StartCoroutine(DoDash());
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;

        if (animator != null)
        {
            animator.SetTrigger("jump");
        }
    }

    private void SetFlip(float input)
    {
        if (input > 0)
        {
            spriteRenderer.flipX = false;
            lookDirection = Vector2.right;
        }
        else if (input < 0)
        {
            spriteRenderer.flipX = true;
            lookDirection = Vector2.left;
        }
    }

    private IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;
        isAttacking = false;

        Vector2 dashDirection = GetDashDirection();

        if (animator != null)
        {
            animator.SetBool("isDash", true);
            animator.SetBool("speed", false);
            animator.SetBool("isAttack", false);
        }

        if (dashTrail != null)
        {
            dashTrail.Clear();
            dashTrail.emitting = true;
        }

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dashDirection.x * dashSpeed, 0f);

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            rb.linearVelocity = new Vector2(dashDirection.x * dashSpeed, 0f);
            yield return null;
        }

        rb.gravityScale = originalGravity;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        isDashing = false;

        if (animator != null)
        {
            animator.SetBool("isDash", false);
        }

        if (dashTrail != null)
        {
            dashTrail.emitting = false;
        }

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        dashCoroutine = null;
    }

    private Vector2 GetDashDirection()
    {
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            float dirX = Mathf.Sign(moveInput.x);
            return new Vector2(dirX, 0f);
        }

        return new Vector2(lookDirection.x, 0f).normalized;
    }

    private IEnumerator MakeBullet()
    {
        isAttacking = true;

        if (animator != null)
        {
            if (attackAnim != null)
                animator.SetBool("isAttack", true);
        }

        if (playerSoundsProvider != null)
        {
            playerSoundsProvider.PlayAttackSound();
        }

        yield return new WaitForSeconds(0.2f);

        GameObject bulletGO = Instantiate(
            bullet,
            bulletSpawnPoint.position,
            Quaternion.identity
        );

        BulletMovement bulletMovement = bulletGO.GetComponent<BulletMovement>();
        if (bulletMovement != null)
        {
            bulletMovement.SetDirection(lookDirection);
        }

        Debug.Log("Attack yapıldı " + gameObject.name);

        yield return new WaitForSeconds(0.5f);

        if (attackAnim != null)
            ResetAttack();
        else
            isAttacking = false;
    }

    private void ResetAttack()
    {
        if (animator != null)
        {
            animator.SetBool("isAttack", false);
            Debug.Log("Attack reset " + animator.gameObject.name);
        }

        isAttacking = false;
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;

        Vector2 v = moveInput;

        if (v.sqrMagnitude > 1f)
            v = v.normalized;

        rb.linearVelocity = new Vector2(v.x * speed, rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}