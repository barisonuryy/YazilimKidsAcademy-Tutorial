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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer=GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        SetFlip(moveInput.x);
    }
    private void SetFlip(float input)
    {
        blackSmoke.SetActive(true);
        if (input > 0)
        {
            spriteRenderer.flipX=false;
            
        }
        else if (input < 0)
        {
            spriteRenderer.flipX=true;
        }
        else
        {
            blackSmoke.SetActive(false);
        }
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return; // sadece basıldığı an

        GameObject bulletGO=Instantiate(
            bullet,
            bulletSpawnPoint.position,
            bulletSpawnPoint.rotation
        );
        bulletGO.GetComponent<BulletMovement>().SetDirection(new Vector2(1,0));
    }

    private void FixedUpdate()
    {
        Vector2 v = moveInput;
        if (v.sqrMagnitude > 1f) v = v.normalized;

        rb.linearVelocity = v * speed;
    }
}
