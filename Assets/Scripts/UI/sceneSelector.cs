using UnityEngine;
using System.Collections;

public class sceneSelector : MonoBehaviour {

	public string levelName; 

	public void LoadMenu(){
		Application.LoadLevel(0);
	}

	public void LoadAndorra(){
		Application.LoadLevel(1);
	}

	public void LoadHamburg(){
		Application.LoadLevel(2);
	}

	public void LoadKendall(){
		Application.LoadLevel(3);
	}
}