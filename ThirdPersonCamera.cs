using UnityEngine;
using System.Collections;
 
public class ThirdPersonCamera : MonoBehaviour 
{
    [SerializeField]private Transform CameraTarget;
	[SerializeField]private float maxViewDist = 15f;
    [SerializeField]private float minViewDist = 1f;
    [SerializeField]private int zoomRate = 20;
	[SerializeField]private float cameraTargetHeight = 1.0f;

    private float x = 0.0f;
    private float y = 0.0f;
 
    private int mouseXSpeedMod = 5;
    private int mouseYSpeedMod = 5;
 
    private int lerpRate = 5;
    private float distance = 3f;
    private float desireDistance;
    private float correctedDistance;
    private float currentDistance;
    private float curDist = 0;
 
    void Start () 
    {
        Vector3 Angles = transform.eulerAngles;
        x = Angles.x;
        y = Angles.y;
        currentDistance = distance;
        desireDistance = distance;
        correctedDistance = distance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
   
    void LateUpdate () 
    {
		//mouse movement
        x += Input.GetAxis("Mouse X") * mouseXSpeedMod;
        y -= Input.GetAxis("Mouse Y") * mouseYSpeedMod;

        y = ClampAngle (y, -90, 90);
		
		//track rotation of the camera
        Quaternion rotation = Quaternion.Euler (y, x, 0);

		//store desired scroll distance and clamp it between min and max values
        desireDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desireDistance);
        desireDistance = Mathf.Clamp (desireDistance, minViewDist, maxViewDist);
        correctedDistance = desireDistance;
 
		//set position as a function of the player's direction and desired zoom distance
        Vector3 position = CameraTarget.position - (rotation * Vector3.forward * desireDistance);
 
		//grab camera's target's position
        Vector3 cameraTargetPosition = new Vector3 (CameraTarget.position.x, CameraTarget.position.y + cameraTargetHeight, CameraTarget.position.z);
		
		//corrects distance from the player when camera hits terrain
		bool isCorrected = false;
		RaycastHit collisionHit;
        if (Physics.Linecast (cameraTargetPosition, position, out collisionHit)) 
		{
            position = collisionHit.point;
            correctedDistance = Vector3.Distance(cameraTargetPosition,position);
            isCorrected = true;
        }

		//sets new current distance to corrected distance through lerping if it didn't need correction, otherwise, just snap it in place
        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomRate) : correctedDistance;
 
        position = CameraTarget.position - (rotation * Vector3.forward * currentDistance + new Vector3 (0, -cameraTargetHeight, 0));
 
		//set new transform values
        transform.rotation = rotation;
        transform.position = position;

		//set the target's rotation to our own
        CameraTarget.eulerAngles = new Vector3(transform.rotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
    }
 
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) 
		{
            angle += 360;      
        }
        else if (angle > 360) 
		{
            angle -= 360;      
        }
        return Mathf.Clamp (angle, min, max);
    }
}