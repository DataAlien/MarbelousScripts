using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCancelUpwardLaunch : MonoBehaviour {

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

			//add the upward force
			rb.AddForce(transform.up * launchForce, ForceMode.Impulse);
		}
	}
}
