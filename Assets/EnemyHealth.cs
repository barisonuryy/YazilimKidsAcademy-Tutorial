
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float startHealth=150;
    private float currentHealth;

    public Image healthBar;

    private bool isDead;

    public PlayerDissolve playerDisslv;
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
            Debug.Log("Son Cana ulaşıldı");
            if(playerDisslv!=null) StartCoroutine(playerDisslv.PlayDissolve());
        }
    }
    void ondisa()
    {
       
       // isDead=true;
    }
     void OnDisable()
    {
         FindAnyObjectByType<UIManager>().SetState("KAZANDIN!");
    }
}
