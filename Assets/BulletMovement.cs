using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 2f;

    private Rigidbody2D rb;
   
    void Start()
    {
      rb=GetComponent<Rigidbody2D>();
      Destroy(gameObject,lifetime);  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       rb.linearVelocity= new Vector3(speed, 0, 0); 
    }
}
