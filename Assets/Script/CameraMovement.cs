
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    private Transform cameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
    {
        cameraTransform=GetComponent<Transform>();
        if(cameraTransform!=null) Debug.Log("Camera alındı");
    }
 

  private void LateUpdate()
  {
    cameraTransform.position=new Vector3(player.transform.position.x,cameraTransform.position.y,cameraTransform.position.z);
  }
}
