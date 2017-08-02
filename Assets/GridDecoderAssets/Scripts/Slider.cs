using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoSlider : LegoUI {
	private GameObject sliderStartObject;
	private GameObject sliderEndObject;

	private GameObject[,] sliderScanners;
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
		if (sliderScanners.GetLength (1) == 0)
			return;

		Debug.Log ("Slider scanner length: " + sliderScanners.GetLength (1));

		int[] currIds = new int[sliderScanners.GetLength(1)];

		for (int i = 0; i < sliderScanners.GetLength(1); i++) {
			currIds[i] = GameObject.Find ("ScannersParent").GetComponent<Scanners> ().FindColor (0, i, ref sliderScanners, false);
			
		}
	}

	private void CreateSlider(float _scannerScale) {
		if (sliderScanners == null) {
			sliderScanners = new GameObject[1, NUM_SCANNERS];

			Vector3 startPos = new Vector3(0, 0, 0);
			Vector3 endPos = new Vector3 (0, 0, 0.5f);

			CreateEndObject (ref sliderStartObject, startPos, _scannerScale);
			CreateEndObject (ref sliderEndObject, endPos, _scannerScale);
		}
			
		Vector3 dir = sliderEndObject.transform.position - sliderStartObject.transform.position;
		Vector3 offsetVector = dir / NUM_SCANNERS;

		for (int i = 0; i < NUM_SCANNERS; i++) {
			CreateEndObject (ref sliderScanners [0,i], offsetVector * i, _scannerScale);
		}
	}

	private void CreateEndObject(ref GameObject currObject, Vector3 pos, float _scannerScale) {
		currObject = GameObject.CreatePrimitive (PrimitiveType.Quad);
		currObject.transform.parent = this.uiParent.transform;
		currObject.transform.localPosition = new Vector3 (pos.x, 0.2f, pos.z);
		currObject.transform.localScale = new Vector3 (_scannerScale, _scannerScale, _scannerScale);  
		currObject.transform.Rotate (90, 0, 0);
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
