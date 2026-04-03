using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDetector : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float leftAngle = -45f;
    [SerializeField] private float rightAngle = 45f;
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Ray Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayDistance = 8f;
    [SerializeField] private LayerMask detectionMask;

    [Header("Tags")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string boxTag = "Box";

    [Header("Shoot / Restart")]
    [SerializeField] private float restartDelay = 0.25f;
    [SerializeField] private bool restartSceneAfterDissolve = true;

    [Header("Gizmo Settings")]
    [SerializeField] private bool drawCenterRay = true;
    [SerializeField] private float gizmoSphereRadius = 0.08f;

    private bool playerDetected = false;
    private bool rotationLocked = false;
    private float currentTargetAngle;
    private Coroutine sweepRoutine;

    private void Start()
    {
        if (rayOrigin == null)
            rayOrigin = transform;

        rayDistance = Mathf.Abs(rayDistance);
        currentTargetAngle = rightAngle;

        transform.rotation = Quaternion.Euler(0f, 0f, leftAngle);

        sweepRoutine = StartCoroutine(SweepRoutine());
    }

    private IEnumerator SweepRoutine()
    {
        while (!playerDetected && !rotationLocked)
        {
            yield return RotateTowardsTarget(currentTargetAngle);

            if (playerDetected || rotationLocked)
                yield break;

            currentTargetAngle = Mathf.Approximately(currentTargetAngle, rightAngle)
                ? leftAngle
                : rightAngle;
        }
    }

    private IEnumerator RotateTowardsTarget(float targetAngle)
    {
        while (!playerDetected && !rotationLocked &&
               Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle)) > 0.1f)
        {
            float newAngle = Mathf.MoveTowardsAngle(
                transform.eulerAngles.z,
                targetAngle,
                rotationSpeed * Time.deltaTime
            );

            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

            DetectTargetFromOriginDirection();

            yield return null;
        }

        if (!rotationLocked)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
            DetectTargetFromOriginDirection();
        }
    }

    private void DetectTargetFromOriginDirection()
    {
        if (playerDetected || rotationLocked)
            return;

        Vector2 direction = (-rayOrigin.up).normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin.position,
            direction,
            rayDistance,
            detectionMask
        );

        Debug.DrawRay(
            rayOrigin.position,
            direction * rayDistance,
            hit.collider != null ? Color.red : Color.green,
            0.02f
        );

        if (hit.collider == null)
            return;

        Debug.Log(
            "Hit successful: " + hit.collider.name +
            " | Tag: " + hit.collider.tag +
            " | Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer)
        );

        if (hit.collider.CompareTag(playerTag))
        {
            Debug.Log("Oyuncu tespit edildi, ateş ediliyor!");
            FireAtPlayer(hit.collider.transform);
        }
        else if (hit.collider.CompareTag(boxTag))
        {
            Debug.Log("Kutu tespit edildi, dönüş devam ediyor.");
        }
    }

    private void FireAtPlayer(Transform player)
    {
        if (playerDetected)
            return;

        playerDetected = true;
        rotationLocked = true;

        if (sweepRoutine != null)
        {
            StopCoroutine(sweepRoutine);
            sweepRoutine = null;
        }

        Debug.Log("ATEŞ! " + player.name);
        StartCoroutine(HandlePlayerCaught(player));
    }

    private IEnumerator HandlePlayerCaught(Transform player)
    {
        PlayerDissolve dissolve = player.GetComponent<PlayerDissolve>();

        if (dissolve == null)
            dissolve = player.GetComponentInChildren<PlayerDissolve>();

        if (dissolve != null)
        {
            dissolve.ResetDissolve();
            yield return StartCoroutine(dissolve.PlayDissolve());
        }
        else
        {
            Debug.LogWarning("PlayerDissolve componenti player üzerinde bulunamadı.", player);
        }

        if (restartDelay > 0f)
            yield return new WaitForSeconds(restartDelay);

        if (restartSceneAfterDissolve)
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
        if (origin == null || !drawCenterRay)
            return;

        float safeDistance = Mathf.Abs(rayDistance);
        Vector3 startPos = origin.position;
        Vector3 dir = (-origin.up).normalized;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(startPos, gizmoSphereRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(startPos, startPos + dir * safeDistance);
        Gizmos.DrawWireSphere(startPos + dir * safeDistance, gizmoSphereRadius);
    }
}