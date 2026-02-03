using UnityEngine;
public class playermoment : MonoBehaviour
{
    public float speed = 5f;

    public Transform kulebir;
    public Transform kuleiki;
    private Transform player;

    private Rigidbody2D rb;
    
    
    private SpriteRenderer tankSprite;
    private int direction = 1; 
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        player = GetComponent<Transform>();
        tankSprite=GetComponent<SpriteRenderer>();
    }
    void Update()
    {
       // player.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
       //Hareket kontrol kodu
        if (player.position.x >= kuleiki.position.x)
        {
            direction = -1; 
            tankSprite.flipX=true;
        }
            
        else if (player.position.x <= kulebir.position.x)
        {
             direction = 1;  
             tankSprite.flipX=false;
        }
           
    }
    
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity= new Vector3(direction * speed, 0, 0);
    }
}




