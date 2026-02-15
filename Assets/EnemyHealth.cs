
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float startHealth=150;
    private float currentHealth;

    public Image healthBar;

    private bool isDead;
    void Start()
    {
        currentHealth=startHealth;
        SetHealthBar();
    }

    private void SetHealthBar()
    {
        healthBar.fillAmount=currentHealth/startHealth;
    }

    public void TakeDamage(float damage)
    {
        if(isDead) return;
        currentHealth=currentHealth-damage;
        SetHealthBar();
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    void OnDestroy()
    {
        FindAnyObjectByType<UIManager>().SetState("KAZANDIN!");
        isDead=true;
    }
}
