using UnityEngine;

public class TankMovement : MonoBehaviour
{
    private Transform tankTransform;
    public float speed;
    void Start()
    {
       tankTransform=GetComponent<Transform>(); 
    }

    // Update is called once per frame
    void Update()
    {
        tankTransform.position+=new Vector3(speed,0,0)*Time.deltaTime;
    }
}
