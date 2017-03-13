using UnityEngine;
using System.Collections;


public class showCommentWindow : MonoBehaviour {


	public GameObject commentWindow;
	private bool onoff; 

	void Awake (){

		commentWindow.GetComponent<CanvasGroup>().alpha = 0f; 
		commentWindow.GetComponent<CanvasGroup>().blocksRaycasts = false; 
	}

	public void CommentWindowAppear () {

		onoff = !onoff; // toggles onoff at each click

		if (onoff){
		commentWindow.GetComponent<CanvasGroup>().alpha = 1f; 
		commentWindow.GetComponent<CanvasGroup>().blocksRaycasts = true; //this prevents the UI element to receive input events

		} else {
		
		commentWindow.GetComponent<CanvasGroup>().alpha = 0f; 
		commentWindow.GetComponent<CanvasGroup>().blocksRaycasts = false; //this prevents the UI element to receive input events
		}
	}

}
