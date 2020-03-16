using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertPlayerMomentum : MonoBehaviour {

	void OnTriggerEnter(Collider col) 
	{
		if (col.gameObject.CompareTag("Player")) {
			Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
			rb.velocity *= -1;
			rb.angularVelocity *= -1;
		}
	}
}
