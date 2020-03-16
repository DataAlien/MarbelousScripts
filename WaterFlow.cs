using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFlow : MonoBehaviour {
	
	[SerializeField]
	private float speed;

	void OnTriggerStay(Collider col) {
		if (col.gameObject.CompareTag("Player")) {
			Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();

			Vector3 pushDirection = speed * transform.right;
			if (rb.velocity.magnitude < 30f) {
				rb.AddForce(pushDirection);
			} 
		}
	}
}
