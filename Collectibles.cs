using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectibles : MonoBehaviour {

	[SerializeField]private int commonFound;
	[SerializeField]private int rareFound;

	[SerializeField]private Text commonFoundText;
	[SerializeField]private Text rareFoundText;

	[SerializeField]private AudioSource commonPickupAudio;
	[SerializeField]private AudioSource rarePickupAudio;

	void OnTriggerEnter(Collider col) 
	{
		if (col.gameObject.CompareTag("CommonCollectible")) 
		{
			commonFound++;
			Destroy(col.gameObject);
			commonFoundText.text = commonFound.ToString();
			commonPickupAudio.Play();
		} 
		else if (col.gameObject.CompareTag("RareCollectible")) 
		{
			rareFound ++;
			Destroy(col.gameObject);
			rareFoundText.text = rareFound.ToString();
			rarePickupAudio.Play();
		}
	}
}
