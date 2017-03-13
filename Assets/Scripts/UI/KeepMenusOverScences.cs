using UnityEngine;
using System.Collections;

public class KeepMenusOverScences : MonoBehaviour {

	void Awake () {
		DontDestroyOnLoad (transform.gameObject);
	}

}