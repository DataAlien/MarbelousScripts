using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionOnPlayerDeath : MonoBehaviour {

	private Vector3 startingPosition;
	private Quaternion startingRotation;
	private Rigidbody rb;

	void Start() 
	{
		startingPosition = transform.position;
		startingRotation = transform.rotation;
		rb = GetComponent<Rigidbody>();
		MarbleMovement.onPlayerRespawn += Reset;
	}

	void Reset() 
	{
		transform.position = startingPosition;
		transform.rotation = startingRotation;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		Debug.Log("Environment Reset has been fired on " + gameObject.name);
	}
}
