using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget: MonoBehaviour {

	[SerializeField]private Transform playerTransform;
	
	//track the player to offer focal point for camera
	void FixedUpdate () {
		transform.position = playerTransform.position;
	}
}
