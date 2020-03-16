using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEventManager : MonoBehaviour {

	public delegate void ResetEnvironment();
	public static event ResetEnvironment onPlayerRespawn;

	void Start() {
		onPlayerRespawn += WriteMessage;
	}

	void WriteMessage () {
		Debug.Log("Environment Objects Reset");
	}
}
