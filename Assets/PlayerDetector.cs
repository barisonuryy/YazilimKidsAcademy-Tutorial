using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDetector : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float[] rotationAngles = { -45f, 0f, 45f, 0f };
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float waitAtAngle = 0.8f;

    [Header("Ray Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayDistance = 8f;
    [SerializeField] private LayerMask detectionMask;

    [Header("Tags")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string boxTag = "Box";

    [Header("Shoot / Restart")]
    [SerializeField] private float restartDelay = 1f;

    [Header("Gizmo Settings")]
    [SerializeField] private bool drawAllAngles = true;
    [SerializeField] private bool drawCurrentAngle = true;
    [SerializeField] private float gizmoSphereRadius = 0.08f;

    private int currentAngleIndex = 0;
    private bool playerDetected = false;

    private void Start()
    {
        if (rayOrigin == null)
            rayOrigin = transform;

        StartCoroutine(RotateAndDetectRoutine());
    }

    private IEnumerator RotateAndDetectRoutine()
    {
        while (!playerDetected)
        {
            float targetAngle = rotationAngles[currentAngleIndex];

            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle)) > 0.1f)
            {
                float newAngle = Mathf.MoveTowardsAngle(
                    transform.eulerAngles.z,
                    targetAngle,
                    rotationSpeed * Time.deltaTime
                );

                transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
                yield return null;
            }

            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);

            yield return new WaitForSeconds(waitAtAngle);

            DetectTarget();

            if (playerDetected)
                yield break;

            currentAngleIndex++;
            if (currentAngleIndex >= rotationAngles.Length)
                currentAngleIndex = 0;
        }
    }

    private void DetectTarget()
    {
        Vector2 direction = rayOrigin.right;

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin.position,
            direction,
            rayDistance,
            detectionMask
        );

        if (hit.collider != null)
        {
            Debug.DrawRay(rayOrigin.position, direction * rayDistance, Color.red, 1f);

            if (hit.collider.CompareTag(playerTag))
            {
                Debug.Log("Oyuncu tespit edildi, ateş ediliyor!");
                FireAtPlayer(hit.collider.transform);
            }
            else if (hit.collider.CompareTag(boxTag))
            {
                Debug.Log("Kutu tespit edildi, rotasyon devam ediyor.");
            }
            else
            {
                Debug.Log("Başka bir nesne tespit edildi: " + hit.collider.name);
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin.position, direction * rayDistance, Color.green, 1f);
            Debug.Log("Hiçbir şey tespit edilmedi.");
        }
    }

    private void FireAtPlayer(Transform player)
    {
        playerDetected = true;

        // Buraya ateş animasyonu / mermi / efekt ekleyebilirsin
        Debug.Log("ATEŞ!");

        StartCoroutine(RestartSceneRoutine());
    }

    private IEnumerator RestartSceneRoutine()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDrawGizmos()
    {
        DrawDetectionGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        DrawDetectionGizmos();
    }

    private void DrawDetectionGizmos()
    {
        Transform origin = rayOrigin != null ? rayOrigin : transform;
        if (origin == null) return;

        Vector3 startPos = origin.position;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(startPos, gizmoSphereRadius);

        if (drawAllAngles && rotationAngles != null && rotationAngles.Length > 0)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.7f);

            for (int i = 0; i < rotationAngles.Length; i++)
            {
                Vector3 dir = GetDirectionFromAngle(rotationAngles[i]);
                Gizmos.DrawLine(startPos, startPos + dir * rayDistance);
                Gizmos.DrawWireSphere(startPos + dir * rayDistance, gizmoSphereRadius * 0.75f);
            }
        }

        if (drawCurrentAngle && rotationAngles != null && rotationAngles.Length > 0)
        {
            int safeIndex = Mathf.Clamp(currentAngleIndex, 0, rotationAngles.Length - 1);

            Gizmos.color = Color.cyan;
            Vector3 currentDir = GetDirectionFromAngle(rotationAngles[safeIndex]);
            Gizmos.DrawLine(startPos, startPos + currentDir * rayDistance);
            Gizmos.DrawWireSphere(startPos + currentDir * rayDistance, gizmoSphereRadius);
        }
    }

    private Vector3 GetDirectionFromAngle(float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0f).normalized;
    }
}