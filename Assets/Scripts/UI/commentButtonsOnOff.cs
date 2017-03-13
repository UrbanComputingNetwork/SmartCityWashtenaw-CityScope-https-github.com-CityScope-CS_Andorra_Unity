using UnityEngine;
using System.Collections;

public class commentButtonsOnOff : MonoBehaviour {

	public GameObject commentsButtons; 
	private bool onoff; 

	/*
 * __________________________________________________________________________________________________________________________
 * 
 * These functions control what is being shown on each model
 * 
 * __________________________________________________________________________________________________________________________
*/


	void Awake (){

		commentsButtons.GetComponent<CanvasGroup>().alpha = 0f; 
		commentsButtons.GetComponent<CanvasGroup>().blocksRaycasts = false; 
	}

	public void commentsButtonsAppear () {

		onoff = !onoff; // toggles onoff at each click

		if (onoff){
			commentsButtons.GetComponent<CanvasGroup>().alpha = 1f; 
			commentsButtons.GetComponent<CanvasGroup>().blocksRaycasts = true; //this prevents the UI element to receive input events

		} else {

			commentsButtons.GetComponent<CanvasGroup>().alpha = 0f; 
			commentsButtons.GetComponent<CanvasGroup>().blocksRaycasts = false; //this prevents the UI element to receive input events
		}
	}		
}
