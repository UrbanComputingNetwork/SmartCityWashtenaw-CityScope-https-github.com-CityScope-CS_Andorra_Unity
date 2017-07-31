using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoSlider : LegoUI {
	public Vector3 sliderStart;
	public Vector3 sliderEnd;

	public float value;

	/// <summary>
	/// Creates the slider.
	/// Compute distance to two endpoints from a given color slider object.
	/// </summary>
	public LegoSlider(GameObject parentObject) {
		CreateScannerParent ("Slider parent", parentObject);

	}

	/// <summary>
	/// Updates the slider.
	/// </summary>
	public void UpdateSlider() {

	}

	private void LoadSlider() {

	}
}
