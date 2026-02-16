
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float startHealth=150;
    private float currentHealth;

    public int generalHealth=3;

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
            generalHealth--;
            FindAnyObjectByType<UIManager>().SetHealths(generalHealth);
            if (generalHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    void OnDestroy()
    {
        FindAnyObjectByType<UIManager>().SetState("KAYBETTİN!");
        FindAnyObjectByType<UIManager>().SetRestart();
        isDead=true;
    }
}
