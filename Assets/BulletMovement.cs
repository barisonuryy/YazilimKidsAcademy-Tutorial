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

    // Uçuş yönü (normalize tutulacak)
    private Vector2 moveDir = Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Bullet'i hedef/yöne göre ateşlemek için çağır.
    /// </summary>
    public void SetDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector2.right;

        moveDir = dir.normalized;

        // İstersen mermiyi anında yönüne döndür
        if (rotateToVelocity)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void FixedUpdate()
    {
        // Unity sürümüne göre velocity/linearVelocity farkı olabilir.
        // Sende Rigidbody2D.linearVelocity çalışıyorsa onu bırakabilirsin.
        rb.linearVelocity = moveDir * speed;
        // rb.linearVelocity = moveDir * speed; // Unity 6+ kullanıyorsan bunu da kullanabilirsin.
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyTower"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null) enemyHealth.TakeDamage(damage);
            return;
        }

        if (collision.CompareTag("Player"))
        {
          Debug.Log("Player bulundu");
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);
            return;
        }
    }
}