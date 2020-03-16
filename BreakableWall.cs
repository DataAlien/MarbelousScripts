using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BreakableWall : MonoBehaviour {

	private enum AXIS {x, y, z};
	
	[SerializeField]
	private float requiredVelocity;

	[SerializeField]
	private AXIS axis;

	[SerializeField]
	private GameObject[] breakables;

	[SerializeField]
	private AudioSource audio;

	[SerializeField]
	private GameObject impactEffect;

	[SerializeField]
	private Transform playerTransform;


	void OnTriggerEnter(Collider col) 
	{
		if (col.gameObject.CompareTag("Player")) 
		{
			Vector3 playerVelocity = col.gameObject.GetComponent<Rigidbody>().velocity;
			switch (axis) 
			{
				case AXIS.x:
					if (Math.Abs(playerVelocity.x) > requiredVelocity) 
					{
						Break();
					}
					break;
				case AXIS.y:
					if (Math.Abs(playerVelocity.y) > requiredVelocity) 
					{
						Break();
					}
					break;
				case AXIS.z:
					Debug.Log("Player velocity on hit: " + playerVelocity.z);
					if (Math.Abs(playerVelocity.z) > requiredVelocity) 
					{
						Break();
					}
					break;
			}
		}
	}

	void Break() 
	{
		foreach(GameObject obj in breakables) 
		{
			obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
		}
		//stop the game and play the break sound
		StartCoroutine(HitFeedback(.25f));

		//remove the collider, we are done here.
		Destroy(GetComponent<BoxCollider>());

	}

	IEnumerator HitFeedback(float pauseTime) 
	{
		//play audio, create impact effect at the player
		audio.Play();
		GameObject clone = Instantiate(impactEffect, playerTransform.position, Quaternion.identity);

		//rotate the impact effect based on the wall's break axis and the position of the player relative to the wall
		//then, rotate it to a random value on that axis to look different every time

		float rand = UnityEngine.Random.Range(0f, 360f);

		switch (axis) 
		{
			case AXIS.x:
				if (playerTransform.position.x < transform.position.x) 
				{
					clone.transform.Rotate(new Vector3(0f, 180f, 0f));
				}
				clone.transform.Rotate(new Vector3(rand, 0f, 0f), Space.World);
				break;
			case AXIS.y:
				if (playerTransform.position.y > transform.position.y) 
				{
					clone.transform.Rotate(new Vector3(0f, 0f, 90f));
				}
				else 
				{
				}
				break;
			case AXIS.z:
				if (playerTransform.position.z > transform.position.z) 
				{
					clone.transform.Rotate(new Vector3(0f, -90f, 0f));
				}
				else 
				{
					clone.transform.Rotate(new Vector3(0f, 90f, 0f));
				}

				clone.transform.Rotate(new Vector3(0f, 0f, rand), Space.World);
				break;
		}

		//stop time for dramatic effect
		Time.timeScale = 0f;
     	float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
      	while (Time.realtimeSinceStartup < pauseEndTime)
    	{
        	yield return 0;
    	}

		//resume, get rid of the impact effect
		Destroy(clone);
		Time.timeScale = 1f;
	}
}
