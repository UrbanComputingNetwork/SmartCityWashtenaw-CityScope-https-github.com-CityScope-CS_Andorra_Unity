using UnityEngine;
using System.Collections;

public class RotateTowardsCamera : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		transform.localScale = new Vector3 (1, 1, 1);
		if (Camera.main == null)
			return;
		var rot = Quaternion.LookRotation (Camera.main.transform.forward, Camera.main.transform.up);
		//Debug.LogFormat("Calculated rotation: {0}", rot.eulerAngles.ToString());
		transform.rotation = rot;

	}
}
