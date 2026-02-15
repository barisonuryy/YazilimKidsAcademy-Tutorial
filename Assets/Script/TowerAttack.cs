using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    public Transform spawnPoint;

    public GameObject bullet;
    public float attackCooldown=10f;
    private float currentCooldown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCooldown < attackCooldown)
        {
            currentCooldown+=Time.deltaTime;
        }
        else
        {
           GameObject bulletGameobject= Instantiate(bullet,transform);
           bulletGameobject.transform.Rotate(0,0,45);
           bulletGameobject.GetComponent<BulletMovement>().SetDirection(new Vector2(-1,-0.05f));
           currentCooldown=0f;
        }
    }
}
