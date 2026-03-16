using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private bool rotateToVelocity = true;

    [Header("Damage")]
    [SerializeField] private float damage = 50f;

    private Rigidbody2D rb;

    private Vector2 moveDir = Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        if (dir == Vector2.zero)
            dir = Vector2.right;

        moveDir = dir.normalized;

        if (rotateToVelocity)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDir * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyTower") && !gameObject.CompareTag("EnemyBullet"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null) enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Player") && !gameObject.CompareTag("PlayerBullet"))
        {
            Debug.Log("Player bulundu");
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }
}