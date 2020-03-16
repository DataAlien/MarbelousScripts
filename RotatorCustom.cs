using UnityEngine;
using System.Collections;

public class RotatorCustom : MonoBehaviour {

	[SerializeField]private Vector3 rotate;
	[SerializeField]private float speed;

	void FixedUpdate () 
	{
		transform.Rotate (rotate * speed);
	}
}	