using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] private GameObject attackUI;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            attackUI.gameObject.SetActive(true);
        }
    }
}
