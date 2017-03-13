using UnityEngine;
using System.Collections;

public class HamburgContextOnOff : MonoBehaviour {


	public GameObject contextObj; 
	private bool onoff; 


	/*
 * __________________________________________________________________________________________________________________________
 * 
 * These functions control what is being shown on each model
 * 
 * __________________________________________________________________________________________________________________________
*/

	void Awake(){

		Renderer[] renderersAwake = contextObj.GetComponentsInChildren<Renderer> (); // renderer[] is now getting all render comp. in 'fatherObj'
		foreach (var r in renderersAwake) { //go through all childrens and ..
			r.enabled = true; // set the renderer to OFF state 
		}

			
	}

	public void clickbeforeAfterButton(){

		onoff = !onoff; // toggles onoff at each click

		if (onoff){

			Renderer[] renderersAwake = contextObj.GetComponentsInChildren<Renderer> (); 
			foreach (var r in renderersAwake) { //go through all childrens and ..
				r.enabled = false; // set the renderer to OFF state 
			}
				

		}else {


			Renderer[] renderersAwake = contextObj.GetComponentsInChildren<Renderer> (); 
			foreach (var r in renderersAwake) { //go through all childrens and ..
				r.enabled = true; // set the renderer to OFF state 
			}


		}
	}

}
