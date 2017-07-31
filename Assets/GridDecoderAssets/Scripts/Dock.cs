using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : LegoUI {
	public GameObject[] dockScanners;

	public Vector3 dockPosition; 

	public int dockId;

	/// <summary>
	/// Creates the dock scanner.
	/// </summary>
	public Dock(GameObject parentObject, int gridSize, float _scannerScale) {
		CreateScannerParent ("Dock parent", parentObject);

		dockScanners = new GameObject[gridSize * gridSize];
		int index = 0;

		for (int x = 0; x < gridSize; x++) {
			for (int y = 0; y < gridSize; y++) {
				dockScanners[index] = GameObject.CreatePrimitive (PrimitiveType.Quad);
				dockScanners[index].name = "dock_" + y + x;
				dockScanners[index].transform.parent = this.uiParent.transform;
				dockScanners[index].transform.localScale = new Vector3 (_scannerScale, _scannerScale, _scannerScale);  
				dockScanners[index].transform.localPosition = new Vector3 (x * _scannerScale * 2, 0.2f, y * _scannerScale * 2);
				dockScanners[index].transform.Rotate (90, 0, 0); 
				index++;
			}
		}
	}

	/// <summary>
	/// Updates the dock ID scanner.
	/// </summary>
	public void UpdateDock() {

	}
}
