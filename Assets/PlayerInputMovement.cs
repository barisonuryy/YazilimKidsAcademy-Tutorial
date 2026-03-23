using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference attackAction;

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
    private PlayerSoundsProvider playerSoundsProvider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerSoundsProvider=GetComponent<PlayerSoundsProvider>();
    }

    private void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Enable();
        }

        if (attackAction != null && attackAction.action != null)
        {
            attackAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Disable();
        }

        if (attackAction != null && attackAction.action != null)
        {
            attackAction.action.Disable();
        }
    }

    private void Update()
    {
        ReadMoveInput();
        ReadAttackInput();
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

        bool isWalked = moveInput.x > 0 || moveInput.x < 0;

        if (animator != null)
        {
            animator.SetBool("speed", isWalked);
        }
    }

    private void ReadAttackInput()
    {
        if (attackAction == null || attackAction.action == null)
        {
            return;
        }

        if (attackAction.action.WasPressedThisFrame() && !isAttacking)
        {
            StartCoroutine(MakeBullet());
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

    private IEnumerator MakeBullet()
    {
        isAttacking = true;

        if (animator != null)
        {
            if(attackAnim!=null)
            animator.SetBool("isAttack", true);
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

        Debug.Log("Attack yapıldı"+gameObject.name);

        yield return new WaitForSeconds(0.5f);
        if(attackAnim!=null)
        ResetAttack();
    }

    private void ResetAttack()
    {
        if (animator != null)
        {
            animator.SetBool("isAttack", false);
            Debug.Log("Attack yapıldı"+animator.gameObject.name);
        }

        isAttacking = false;
    }

    private void FixedUpdate()
    {
        Vector2 v = moveInput;

        if (v.sqrMagnitude > 1f)
        {
            v = v.normalized;
        }

        rb.linearVelocity = v * speed;
    }
}