using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]// have to have this in every JSON class!
public class Table
{
	public List<Grid> grid;
	public Objects objects;
	public string id;
	public long timestamp;

	public static Table CreateFromJSON(string jsonString)
	{ // static function that returns Table which holds Class objects 
		return JsonUtility.FromJson<Table>(jsonString);
	}

	/// <summary>
	/// Creates Table object from GridDecoder.
	/// Returns true if there are changes to the grid.
	/// </summary>
	/// <param name="table">Table.</param>
	public bool CreateFromDecoder(string scannersParentName)
	{
		bool needsUpdate = false;
		CreateGrid(scannersParentName, ref needsUpdate);

		CreateObjects (scannersParentName, ref needsUpdate);

		return needsUpdate;
	}

	/// <summary>
	/// Creates the grid with data passed from the Scanners class.
	/// </summary>
	/// <returns><c>true</c>, if grid was created, <c>false</c> otherwise.</returns>
	/// <param name="table">Table.</param>
	/// <param name="scannersParentName">Scanners parent name.</param>
	private bool CreateGrid(string scannersParentName, ref bool needsUpdate) {
		int[,] currIds = GameObject.Find (scannersParentName).GetComponent<Scanners> ().GetCurrentIds();
		if (currIds == null)
			return false;

		if (this.grid != null) {
			for (int i = 0; i < currIds.GetLength (0); i++) {
				for (int j = 0; j < currIds.GetLength (1); j++) {
					int currType = this.grid [i * currIds.GetLength (1) + j].type;

					if (currType != currIds [i, j]) {
						this.grid [i * currIds.GetLength (1) + j].type = currIds [i, j];
						needsUpdate = true;
						this.grid [i * currIds.GetLength (1) + j].update = true;
					}
					else
						this.grid [i * currIds.GetLength (1) + j].update = false;
				}
			}
		}
		else {
			needsUpdate = true;
			Debug.Log ("Creating new table grid list");
			this.grid = new List<Grid> ();
			for (int i = 0; i < currIds.GetLength (0); i++) {
				for (int j = 0; j < currIds.GetLength (1); j++) {
					Grid currGrid = new Grid ();
					currGrid.type = currIds [i, j];
					currGrid.x = i;
					currGrid.y = j;
					currGrid.rot = 180;
					currGrid.update = true;
					this.grid.Add (currGrid);
					currGrid = null;
				}
			}
		}

		return needsUpdate;
	}

	/// <summary>
	/// Populates the Table class' Objects with dock ID, slider values, etc from the Scanners class.
	/// </summary>
	/// <returns><c>true</c>, if objects was created, <c>false</c> otherwise.</returns>
	/// <param name="table">Table.</param>
	/// <param name="scannerParentName">Scanner parent name.</param>
	/// <param name="needsUpdate">Needs update.</param>
	private bool CreateObjects(string scannerParentName, ref bool needsUpdate) {
		return false;
	}
}
