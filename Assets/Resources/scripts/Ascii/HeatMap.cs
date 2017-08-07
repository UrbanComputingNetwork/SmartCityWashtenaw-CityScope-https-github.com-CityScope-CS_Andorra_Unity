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

	float gradientScale = 0.4f;

	public HeatMap(int sizeX, int sizeY, int searchDimension, float _cellSize, float _addToYHeight, string name) {
		this.heatmapGeos = new GameObject[sizeX * sizeY];
		this.gridX = sizeX;
		this.gridY = sizeY;
		this.searchDim = searchDimension;

		this._typesArray = new int[sizeX,sizeY];
		this.cellSize = _cellSize;
		this.yOffset = _addToYHeight;

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

	/// <summary>
	/// Searches the neighbors // brute force
	/// </summary>
	public void UpdateHeatmap(int x, int y, int type, int index) 
	{
		_typesArray[x, y] = type;

		// for the brick at (x, y), 
		// check if it's one of the origin types that needs a score
		// if so, check if its neighbors are in the search type array & increment score if so
		if (this.originTypes.Contains((Brick)_typesArray[x, y]))
		{
			heatmapGeos[index].transform.GetComponent<Renderer>().material.color = Color.green;
			heatmapGeos[index].transform.localScale = new Vector3(cellSize, cellSize, cellSize);
			int _cellScoreCount = 0; //decalre a tmp counter  

			for (int _windowX = x - searchDim; _windowX < x + searchDim; _windowX++)
			{
				for (int _windowY = y - searchDim; _windowY < y + searchDim; _windowY++)
				{
					if (_windowX > 0 && _windowY > 0 && _windowX < gridX && _windowY < gridY)
					{ // make sure window area is not outside grid bounds 
						if (this.searchTypes.Contains((Brick)_typesArray[_windowX, _windowY]))
						{
							_cellScoreCount = _cellScoreCount + 1;
							heatmapGeos[index].transform.localPosition =
								new Vector3(x * cellSize, yOffset + (_cellScoreCount * 2), y * cellSize);
							heatmapGeos[index].name = ("Results count: " + _cellScoreCount.ToString());
							var _tmpColor = (_cellScoreCount / Mathf.Pow(2 * searchDim, 2)) * gradientScale; // color color spectrum based on cell score/max potential score 
							heatmapGeos [index].transform.GetComponent<Renderer> ().material.color =
								Color.HSVToRGB (_tmpColor, 1, 1);
						}
					}
				}
			}
		}
		else
		{
			heatmapGeos[index].transform.GetComponent<Renderer>().material.color = Color.HSVToRGB(0, 0, 0);
			heatmapGeos[index].transform.localScale = new Vector3(cellSize * 0.9f, cellSize * 0.9f, cellSize * 0.9f);
		}
	}
}
