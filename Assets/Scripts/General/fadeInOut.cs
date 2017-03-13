/* 


using UnityEngine;
using System.Collections;
using UnityEngine.UI; // to be able to control text 

public class fadeInOut : MonoBehaviour {

	public GameObject textObject; // insert text object here
	public GameObject gameObject; // insert text object here
	
	// Update is called once per frame
	void Update () {

		// get text object and switch Alpha channel in between time and 0.9
		textObject.GetComponentInChildren<Text>().color = new Color (1, 1 ,1 ,Mathf.PingPong(Time.time, 0.9F)); 

		// Debug.Log (Mathf.PingPong(Time.time, 0.9F)); // if you want to debug the change over time

	}
}


*/