using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpwardLaunch : MonoBehaviour {

	[SerializeField]private float launchForce;

	void OnValidate() 
	{
		//Debug.DrawRay(transform.position, transform.up, Color.yellow, 20f);
	}

	void OnTriggerEnter (Collider col) 
	{
		if (col.gameObject.tag == "Player") 
		{
			Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();

			//zero out the y velocity (stop it from falling)
			Vector3 newVelocity = rb.velocity;
			newVelocity.y = 0f;
			rb.velocity = newVelocity;

			//add the upward force
			rb.AddForce(transform.up * launchForce, ForceMode.Impulse);
		}
	}
}
