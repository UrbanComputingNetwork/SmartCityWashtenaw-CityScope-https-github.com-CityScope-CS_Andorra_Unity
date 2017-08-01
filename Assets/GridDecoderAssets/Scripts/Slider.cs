using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoSlider : LegoUI {
	private GameObject sliderStartObject;
	private GameObject sliderEndObject;

	private GameObject[] sliderScanners;
	private const int NUM_SCANNERS = 30;

	private float length;

	public float value;

	/// <summary>
	/// Creates the slider.
	/// Compute distance to two endpoints from a given color slider object.
	/// </summary>
	public LegoSlider(GameObject parentObject, float _scannerScale) {
		CreateScannerParent ("Slider parent", parentObject);

		CreateSlider (_scannerScale);
	}

	/// <summary>
	/// Updates the slider.
	/// </summary>
	public void UpdateSlider() {

	}

	private void CreateSlider(float _scannerScale) {
		if (sliderScanners == null) {
			sliderScanners = new GameObject[NUM_SCANNERS];

			sliderStartObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
			sliderStartObject.transform.parent = this.uiParent.transform;
			sliderStartObject.transform.localPosition = new Vector3 (0, 0.2f, 0);
			sliderStartObject.transform.localScale = new Vector3 (_scannerScale, _scannerScale, _scannerScale);  
			sliderStartObject.transform.Rotate (90, 0, 0);

			sliderEndObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
			sliderEndObject.transform.parent = this.uiParent.transform;
			sliderEndObject.transform.localPosition = new Vector3 (0, 0.2f, 1);
			sliderEndObject.transform.localScale = new Vector3 (_scannerScale, _scannerScale, _scannerScale);  
			sliderEndObject.transform.Rotate (90, 0, 0);
		}

		length = Vector3.Distance (sliderStartObject.transform.localPosition, sliderEndObject.transform.localPosition);
		float currentPosition = 0f;

		this.value = (currentPosition / length);

		float offsetDist = length / NUM_SCANNERS;

		for (int i = 0; i < NUM_SCANNERS; i++) {
			sliderScanners [i] = new GameObject ();

			sliderScanners[i].name = "slider_" + i;
			sliderScanners[i].transform.parent = this.uiParent.transform;
			sliderScanners[i].transform.localPosition = new Vector3 (0, 0.2f, i * offsetDist);
			sliderScanners[i].transform.Rotate (90, 0, 0); 
		}
	}



	/////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////
	/// 
	/// GETTERS & SETTERS
	/// 
	/////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////

	public int GetSliderValue() {
		return (int) this.value;
	}

	public void SetSliderPosition(Vector3 start, Vector3 end) {
		this.sliderStartObject.transform.localPosition = start;
		this.sliderEndObject.transform.localPosition = end;

		// update slider
	}
}
