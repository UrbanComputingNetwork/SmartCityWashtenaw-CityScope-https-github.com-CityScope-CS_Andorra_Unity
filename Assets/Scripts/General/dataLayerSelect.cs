using UnityEngine;
using System.Collections;

public class dataLayerSelect : MonoBehaviour {

	public GameObject firstObj; 
	public GameObject secondObj; 
	private bool onoff; 
 
/*
 * __________________________________________________________________________________________________________________________
 * 
 * These functions control what is being shown on each model
 * 
 * __________________________________________________________________________________________________________________________
*/

	void Awake(){

		Renderer[] renderersAwake = firstObj.GetComponentsInChildren<Renderer> (); // renderer[] is now getting all render comp. in 'fatherObj'
		foreach (var r in renderersAwake) { //go through all childrens and ..
			r.enabled = true; // set the renderer to OFF state 
		}

		Renderer[] renderers2 = secondObj.GetComponentsInChildren<Renderer> (); // renderer[] is now getting all render comp. in 'fatherObj'
		foreach (var r in renderers2) { //go through all childrens and ..
			r.enabled = false; // set the renderer to ON state 

		}

	}

// intactableModel model layer
	 public void onOffObjects(){

		onoff = !onoff; // toggles onoff at each click


		if (onoff){

			Renderer[] renderers = firstObj.GetComponentsInChildren<Renderer> (); // renderer[] is now getting all render comp. in 'fatherObj'
			foreach (var r in renderers) { //go through all childrens and ..
				r.enabled = true; // set the renderer to OFF state 
			}

			Renderer[] renderers2 = secondObj.GetComponentsInChildren<Renderer> (); // renderer[] is now getting all render comp. in 'fatherObj'
			foreach (var r in renderers2) { //go through all childrens and ..
				r.enabled = false; // set the renderer to ON state 

			}

		} else {

			Renderer[] renderers = firstObj.GetComponentsInChildren<Renderer> (); // renderer[] is now getting all render comp. in 'fatherObj'
			foreach (var r in renderers) { //go through all childrens and ..
				r.enabled = false; // set the renderer to OFF state 
			}

			Renderer[] renderers2 = secondObj.GetComponentsInChildren<Renderer> (); // renderer[] is now getting all render comp. in 'fatherObj'
			foreach (var r in renderers2) { //go through all childrens and ..
				r.enabled = true; // set the renderer to ON state 

			}
		}
	}			
}
	