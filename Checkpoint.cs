using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	[SerializeField]private GameObject activatedFire;
	private bool activated = false;

	void Start() 
	{
		//when new checkpoint is discovered, we want to deactivate the others.
		MarbleMovement.onNewCheckpointDiscovered += Deactivate;
	}

	public bool isActivated() {
		return activated;
	}

	public void Activate () 
	{
		activated = false;
		activatedFire.SetActive(true);
	}

	void Deactivate () 
	{
		activated = false;
		activatedFire.SetActive(false);
		//Debug.Log("Checkpoint has been visually deactivated: " + gameObject.name);
	}
}
