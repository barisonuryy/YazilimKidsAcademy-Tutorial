using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    public GameObject blackSmoke;

    private SpriteRenderer spriteRenderer;

    public GameObject bullet;
    public Transform bulletSpawnPoint;
    private Transform blackSmokeTransform;

    private Animator animator;

    public AnimationClip attackAnim;

    // Karakterin baktığı yön
    private Vector2 lookDirection = Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        SetFlip(moveInput.x);

        bool isWalked = moveInput.x > 0 || moveInput.x < 0;
        Debug.Log("isWalked " + isWalked);

        if (animator != null)
        {
            Debug.Log("animator boş değil");
            animator.SetBool("speed", isWalked);
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

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        GameObject bulletGO = Instantiate(
            bullet,
            bulletSpawnPoint.position,
            Quaternion.identity
        );

        animator.SetBool("isAttack", true);
        Invoke(nameof(ResetAttack), attackAnim.length);

        BulletMovement bulletMovement = bulletGO.GetComponent<BulletMovement>();
        if (bulletMovement != null)
        {
            bulletMovement.SetDirection(lookDirection);
        }
    }

    private void ResetAttack()
    {
        animator.SetBool("isAttack", false);
    }

    private void FixedUpdate()
    {
        Vector2 v = moveInput;
        if (v.sqrMagnitude > 1f) v = v.normalized;

        rb.linearVelocity = v * speed;
    }
}