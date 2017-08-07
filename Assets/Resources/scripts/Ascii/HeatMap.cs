using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMap {
	GameObject heatmapParent; 
	GameObject[] heatmapGeos;
	private GameObject currentHeatmapParent;

	List<Brick> searchTypes;
	List<Brick> originTypes;

	int gridX;
	int gridY;
	float cellSize;
	float yOffset;
	int searchDim;
	int[,] _typesArray;

	float [] scores;
	bool [] update;

	float maxScore;

	float gradientScale = 0.4f;

	public HeatMap(int sizeX, int sizeY, int searchDimension, float _cellSize, float _addToYHeight, string name) {
		this.heatmapGeos = new GameObject[sizeX * sizeY];
		this.gridX = sizeX;
		this.gridY = sizeY;
		this.searchDim = searchDimension;

		this._typesArray = new int[sizeX,sizeY];
		this.scores = new float[sizeX * sizeY];
		this.update = new bool[sizeX * sizeY];

		this.cellSize = _cellSize;
		this.yOffset = _addToYHeight;

		maxScore = -2f;

		this.currentHeatmapParent = new GameObject ();
		this.currentHeatmapParent.SetActive (false);
		this.currentHeatmapParent.name = name;
	}

	public void SetSearchTypes(List<Brick> searched) {
		this.searchTypes = searched;
	}

	public void SetOriginTypes(List<Brick> origins) {
		this.originTypes = origins;
	}

	public void SetParent(GameObject parentObject) {
		this.heatmapParent = parentObject;
		this.currentHeatmapParent.transform.parent = heatmapParent.transform;
	}

	public GameObject GetParentObject() {
		return currentHeatmapParent;
	}

	public void SetParentActive(bool isActive) {
		this.currentHeatmapParent.SetActive (isActive);
	}

	public void CreateHeatmapGeo(int x, int y, int index, int type) {
		heatmapGeos[index] = GameObject.CreatePrimitive(PrimitiveType.Quad); //make cell cube
		heatmapGeos[index].name = (currentHeatmapParent.name + " Type: " + type);

		heatmapGeos[index].transform.localPosition = new Vector3(x * cellSize, yOffset, y * cellSize);
		Quaternion _tmpRot = heatmapParent.transform.localRotation;
		_tmpRot.eulerAngles = new Vector3(90, 0, 0.0f);
		heatmapGeos[index].transform.localRotation = _tmpRot;

		heatmapGeos[index].transform.localScale = new Vector3(cellSize, cellSize, cellSize);
		heatmapGeos[index].transform.GetComponent<Renderer>().receiveShadows = false;
		heatmapGeos[index].transform.GetComponent<Renderer>().shadowCastingMode =
			UnityEngine.Rendering.ShadowCastingMode.Off;
		heatmapGeos[index].transform.parent = currentHeatmapParent.transform; //put into parent object for later control
	}

	// Update types separately from score compute loop!
	public void UpdateType(int x, int y, int type) {
		if (_typesArray != null)
			_typesArray[x, y] = type;
	}

	/// <summary>
	/// Searches the neighbors // brute force
	/// </summary>
	public void UpdateHeatmapItem(int x, int y, int type, int index) 
	{
		int newScore = -1;

		// for the brick at (x, y), 
		// check if it's one of the origin types that needs a score
		// if so, check if its neighbors are in the search type array & increment score if so
		if (this.originTypes.Contains((Brick)_typesArray[x, y])) {
			ComputeScore (x, y, ref newScore);
			if (newScore > maxScore)
				maxScore = newScore;
		}

		if (newScore != scores [index]) {
			update [index] = true;
			scores [index] = newScore;
		} else
			update [index] = false;
	}

	public void ApplyScore(int index) {
		if (heatmapGeos [index] == null)
			return;
		
		if (scores[index] >= 0) {
			heatmapGeos[index].transform.localPosition =
				new Vector3(heatmapGeos[index].transform.localPosition.x, yOffset + (scores[index] * 2), heatmapGeos[index].transform.localPosition.z);
			heatmapGeos[index].name = ("Results count: " + scores[index].ToString());
			var _tmpColor = scores[index] * gradientScale; // color color spectrum based on cell score/max potential score 
			heatmapGeos [index].transform.GetComponent<Renderer> ().material.color =
				Color.HSVToRGB (_tmpColor, 1, 1);
		}
		else
		{
			heatmapGeos[index].transform.GetComponent<Renderer>().material.color = Color.HSVToRGB(0, 0, 0);
			heatmapGeos[index].transform.localScale = new Vector3(cellSize * 0.9f, cellSize * 0.9f, cellSize * 0.9f);
		}

	}

	public void UpdateHeatmap() {
		int index = 0;
		for (int x = 0; x < gridX; x++) {
			for (int y = 0; y < gridY; y++) {
				UpdateHeatmapItem (x, y, _typesArray [x, y], index);
				index++;
			}
		}

		NormalizeScores ();

		for (int i = 0; i < scores.Length; i++) {
			if (update[i])
				ApplyScore (i);
		}
	}

	private void NormalizeScores() {
		for (int i = 0; i < scores.Length; i++) {
			if (scores[i] >= 0)
				scores [i] = Remap (scores [i], 0, maxScore, 0, 1f);
		}
	}

	//
	// From https://forum.unity3d.com/threads/re-map-a-number-from-one-range-to-another.119437/
	// Remap
	private float Remap (float value, float from1, float to1, float from2, float to2) {
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	/// <summary>
	/// Computes the score within the current search window 
	/// by checking each other object if they belong in the search list
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="_cellScoreCount">Cell score count.</param>
	private void ComputeScore(int x, int y, ref int _cellScoreCount) {
		for (int _windowX = x - searchDim; _windowX < x + searchDim; _windowX++)
		{
			for (int _windowY = y - searchDim; _windowY < y + searchDim; _windowY++)
			{
				if (_windowX > 0 && _windowY > 0 && _windowX < gridX && _windowY < gridY)
				{ // make sure window area is not outside grid bounds 
					if (this.searchTypes.Contains((Brick)_typesArray[_windowX, _windowY]))
					{
						_cellScoreCount++;
					}
				}
			}
		}
	}
}
