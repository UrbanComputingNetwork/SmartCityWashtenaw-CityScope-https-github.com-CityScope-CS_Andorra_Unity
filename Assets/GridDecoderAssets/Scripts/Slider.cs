using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LegoSlider : LegoUI {
	private GameObject sliderStartObject;
	private GameObject sliderEndObject;

	private GameObject[,] sliderScanners;
	private const int NUM_SCANNERS = 30;

	private float length;

	public int value;

	// Number of ints the slider can return as valid values 
	// i.e. 30 for 30 floors in building height calculation
	public int range;

	private bool debug = false;
	private bool needsUpdate;

	// Current reference value the slider should track
	private int REFERENCE_COLOR = (int) ColorClassifier.SampleColor.RED;

	// Stores indices of scanner objects that have the slider's reference color
	List<int> refColorIndex;

	// Stores currently scanned color IDs (from ColorClassifier.SampleColors)
	int[] currIds;

	/// <summary>
	/// Creates the slider.
	/// Compute distance to two endpoints from a given color slider object.
	/// </summary>
	public LegoSlider(GameObject parentObject, float _scannerScale, int range) {
		this.range = range;
		this.value = 0; // for init value

		CreateScannerParent ("Slider parent", parentObject);
		CreateSlider (_scannerScale);

		refColorIndex = new List<int> ();
		currIds = new int[sliderScanners.GetLength(1)];
	}

	/// <summary>
	/// Updates the slider.
	/// </summary>
	public void UpdateSlider() {
		if (sliderScanners.GetLength (1) == 0)
			return;

		needsUpdate = false;
		refColorIndex.Clear ();

		for (int i = 0; i < sliderScanners.GetLength(1); i++) {
			int currId = GameObject.Find ("ScannersParent").GetComponent<Scanners> ().FindColor (0, i, ref sliderScanners, false);
			if (currIds[i] != currId) {
				currIds [i] = currId;
				needsUpdate = true;
			}

			// Using RED as reference color, but could change to slider's own reference
			if (currIds[i] == REFERENCE_COLOR) {
				refColorIndex.Add (i);
			}
		}

		if (needsUpdate && refColorIndex.Count > 0) {
			RecomputeSliderValue (ref refColorIndex);
		}
	}

	/// <summary>
	/// Recomputes the slider value based on the current reading & notifies 
	/// CityIO if there is a change
	/// </summary>
	/// <param name="refColorIndex">Reference color index.</param>
	private void RecomputeSliderValue (ref List<int> refColorIndex) {
		int refIndex = refColorIndex.Sum () / refColorIndex.Count;
		if (debug)
			sliderScanners [0, refIndex].GetComponent<Renderer> ().material.color = Color.cyan;

		int newValue = (int) (((float) refIndex / (float) NUM_SCANNERS) * (float)this.range);
		Debug.Log ("Current value is: " + newValue + " with range " + this.range);

		if (this.value != newValue) {
			this.value = newValue;

			// Notify CityIO
			EventManager.TriggerEvent("sliderChange");
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
