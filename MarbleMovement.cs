using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System;

public class MarbleMovement : MonoBehaviour {
	
	[SerializeField]private float speed;
	[SerializeField]private float aerialSpeedMultiplier;
	[SerializeField]private float jumpForce;
	[SerializeField]private float bonusUpwardForce;

	[SerializeField]private GameObject smokeParticleEffect;
	[SerializeField]private GameObject speedParticleEffect;

	[SerializeField]private KeyCode joystickJump;

	private Rigidbody rb;
	private Transform cam;

	[SerializeField] private List<Vector3> jumpableNormals;

	private float viewAngle;

	private bool isTouchingJumpableSurface = false;
	private bool isBraking = false;
	private bool inRushingWater = false;
	private bool inOuterSpace = false;
	private bool startJump = false;

	public delegate void ResetEnvironment();
	public delegate void DeactivateAllCheckpoints();
	
	public static event ResetEnvironment onPlayerRespawn;
	public static event DeactivateAllCheckpoints onNewCheckpointDiscovered;

	private Vector3 lastCheckpointLocation;

	private float tempDrag;
	private float tempAngularDrag;

	private float jumpCoolDown = .15f;
	private float lastJumpTimeStamp;



	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		rb.maxAngularVelocity = 10000f;
		cam = GameObject.Find("Main Camera").transform;
		jumpableNormals = new List<Vector3>();

		//Default checkpoint location. TODO: Change after demo.
		lastCheckpointLocation = new Vector3 (33.84f, 2.7f, -39.27f);
	}

	void FixedUpdate ()
	{
		//get input axes and keep camera 
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		//obtain the camera's forward values
		Vector3 forward = cam.forward;
		Vector3 right = cam.right;

		//zero out their y values (they would interfere with our x/z movement otherwise)
		forward.y = 0f;
		right.y = 0f;

		//normalize to keep values constant for movement
		forward.Normalize();
		right.Normalize();

		//apply movement 
		Vector3 directionVector = forward * moveVertical + right * moveHorizontal;

		//Debug.Log("DirectionVector: " + directionVector);

		// if (rb.velocity.magnitude >= 16) {
		// 	speedParticleEffect.SetActive(true);
		// }
		// else
		// {
		// 	speedParticleEffect.SetActive(false);
		// }

		//apply movement, but slow aerial movement if in the air

		if (rb.IsSleeping()) 
		{
        	rb.WakeUp();
    	}

		//allow movement when on a jumpable surface, ie wall or floor
		if (isTouchingJumpableSurface) 
		{
			if (!isBraking) 
			{
				rb.AddForce (directionVector * speed, ForceMode.Force);
			}
		}
		else if (!inOuterSpace)
		{
			//if we aren't in Outer Space or trying to air-brake, then perform aerial movement
			if (!Input.GetKey(KeyCode.LeftControl) )
			{
				rb.AddForce (directionVector * speed * aerialSpeedMultiplier, ForceMode.Force);
			}
			Vector3 airborneTorqueVector = new Vector3(directionVector.z, 0f, directionVector.x * -1f);

			//torque the marble in the air
			rb.AddTorque(airborneTorqueVector * 4f, ForceMode.Force); 
		}

		//brake if we're hitting the brake key
		if (Input.GetKey(KeyCode.LeftControl)) 
		{
			if (isTouchingJumpableSurface) 
			{
				isBraking = true;
			}
		} 
		else 
		{
			isBraking = false;
		}

		//perform jump if jump was initiated from FixedUpdate
		if (startJump) 
		{
			Debug.Log("Startjump happened.");
			startJump = false;
			foreach (Vector3 normal in jumpableNormals ) 
			{
				rb.AddForce(normal * jumpForce / jumpableNormals.Count, ForceMode.Impulse);
			}
			rb.AddForce(bonusUpwardForce * Vector3.up, ForceMode.Impulse);
		}

		//clear tracked normals at the end of a physics frame, so that we don't kick off of phantom surfaces
		jumpableNormals.Clear();
	}

	void Update () 
	{
		//trigger jump in FixedUpdate if we are touching surfaces and we hit jump key
		if ((Input.GetKeyDown(KeyCode.Space) 
			|| Input.GetKeyDown(joystickJump)) 
			&& jumpableNormals.Count != 0) 
		{
			startJump = true;
			lastJumpTimeStamp = Time.time;
		}
		else if (Input.GetKey(KeyCode.Space) && jumpableNormals.Count != 0 && isTouchingJumpableSurface)
		//trigger jump in FixedUpdate if we are touching surfaces and we are holding jump key
		{
			if (Time.time > lastJumpTimeStamp + jumpCoolDown){
				startJump = true;
				isTouchingJumpableSurface = false;
				lastJumpTimeStamp = Time.time;
			}
		}
	}

	private void ReturnToCheckpoint() 
	{
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.position = lastCheckpointLocation;

		//reset the environment
		if (onPlayerRespawn != null) 
		{
			onPlayerRespawn();
		} 
		else 
		{
			Debug.Log("OnPlayerRespawn event is null. Error.");
		}
	}

	private void MarkNewCheckpointAsActive(GameObject go)
	{
		Checkpoint cp = go.GetComponent<Checkpoint>();

			if (!cp.isActivated()) {
				onNewCheckpointDiscovered();
				cp.Activate();
				lastCheckpointLocation = go.transform.position;
			}
	}

	//TODO:Remove these methods and lerp the camera instead of translating it
	private void LockCameraToCameraZone(Transform zone)
	{
		cam.transform.position = zone.transform.position;
		cam.GetComponent<ThirdPersonCamera>().enabled = false;
		cam.GetComponent<LockedCamera>().enabled = true;
	}

	private void UnlockCameraFromCameraZone(Transform zone)
	{
		cam.GetComponent<ThirdPersonCamera>().enabled = true;
		cam.GetComponent<LockedCamera>().enabled = false;
	}

	void OnCollisionEnter(Collision col) 
	{
		//keep track of all collision normals so that we can kick off of multiple surfaces
		if (col.gameObject.CompareTag ("Jumpable")) 
		{
			Vector3 thisNormal = col.contacts[0].normal;

			if (jumpableNormals.Count == 0) 
			{
				jumpableNormals.Add(new Vector3((float)Math.Round(thisNormal.x, 1), (float)Math.Round(thisNormal.y, 1), (float)Math.Round(thisNormal.z, 1)));
			} 
			else 
			{
				Vector3[] temp = new Vector3[jumpableNormals.Count];
				jumpableNormals.CopyTo(temp);
				bool canBeAdded = true;

				//don't add normals that are too similar to the ones already in the list
				//TODO: make this less ugly
				for (int i = 0; i < temp.Length; i++) 
				{
					if ((Math.Round(thisNormal.x, 1) == Math.Round(temp[i].x, 1)
						&& Math.Round(thisNormal.y, 1) == Math.Round(temp[i].y, 1)
						&& Math.Round(thisNormal.z, 1) == Math.Round(temp[i].z, 1))) 
					{
							canBeAdded = false;
					}
				}
				if (canBeAdded) 
				{
					jumpableNormals.Add(new Vector3((float)Math.Round(thisNormal.x, 1), (float)Math.Round(thisNormal.y, 1), (float)Math.Round(thisNormal.z, 1)));
					//Debug.Log ("Normal count: " + jumpableNormals.Count); 
				}
			}
		}
	}
	void OnCollisionExit(Collision col) 
	{
		//clear jump flags
		if (col.gameObject.CompareTag ("Jumpable")) 
		{
			isTouchingJumpableSurface = false;
			startJump = false;
		}
	}

	void OnCollisionStay(Collision col)  
	{
		Debug.Log("Stay");
		OnCollisionEnter(col);

		if (col.gameObject.CompareTag ("Jumpable")) 
		{
			isTouchingJumpableSurface = true;

			//perform brake by stopping angular velocity
			if (Input.GetKey(KeyCode.LeftControl)) 
			{
				rb.angularVelocity = Vector3.zero;

				if (rb.velocity.magnitude > 17 && !inRushingWater) 
				{
					Instantiate(smokeParticleEffect, col.contacts[0].point, Quaternion.identity);
				}
			}
		}
	}

	void OnTriggerStay (Collider col) 
	{
		if (col.gameObject.CompareTag("RushingWater")) 
		{
			inRushingWater = true;
		}

		if (col.gameObject.CompareTag("OuterSpace")) 
		{	
			inOuterSpace = true;
			rb.useGravity = false;
			
			rb.drag = 0f;
			rb.angularDrag = 0f;
		}
	}

	void OnTriggerEnter (Collider col) 
	{
		if (col.gameObject.CompareTag("Checkpoint")) 
		{	
			MarkNewCheckpointAsActive(col.gameObject);
		} 
		else if (col.gameObject.CompareTag("EnvironmentReset"))  
		{
			//when we hit environment reset points (usually falling off the map) return to checkpoint
			ReturnToCheckpoint();
		}

		//do outer space things if we are in outer space
		if (col.gameObject.CompareTag("OuterSpace"))
		{
			tempDrag = rb.drag;
			tempAngularDrag = rb.angularDrag;
		}

		//TODO: do this by lerping the camera instead?
		if (col.gameObject.CompareTag("CameraZone"))
		{
			LockCameraToCameraZone(col.gameObject.transform);
		}
	}

	//clears relevant flags
	void OnTriggerExit (Collider col) 
	{
		if (col.gameObject.CompareTag("RushingWater")) 
		{
			inRushingWater = false;
		}
		if (col.gameObject.CompareTag("OuterSpace")) 
		{
			inOuterSpace = false;
			rb.useGravity = true;
			rb.drag = tempDrag;
			rb.angularDrag = tempAngularDrag;
		}
		if (col.gameObject.CompareTag("CameraZone"))
		{
			UnlockCameraFromCameraZone(col.gameObject.transform);
		}
	}
}

