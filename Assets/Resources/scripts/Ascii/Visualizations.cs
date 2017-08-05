using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class Visualizations : MonoBehaviour
{
    public cityIO _city_IO_script;
    /// <summary>
    /// The ASCII types txt files.
    /// </summary>
    public TextAsset _asciiTypes;
    /// <summary>
    /// The ASCII floors txt files.
    /// </summary>
    public TextAsset _asciiFloors;
    /// <summary>
    /// The ASCII masks txt files.
    /// </summary>
    public TextAsset _asciiMasks;
    private List<int> _typesList = new List<int>();
    private List<int> _floorsList = new List<int>();
    private List<int> _masksList = new List<int>();

    /// <summary>
    /// to be replaced with x,y dim from ascii parsing
    /// </summary>
    public int _gridX;
    public int _gridY;
    /// <summary>
    /// counter for the double loop
    /// </summary>
    private int _loopsCounter = 0;
    /// <summary>
    /// The GO to show the grid
    /// </summary>
    private GameObject[] _floorsGeometries;
	private GameObject floorsParent;

    private GameObject[] _typesGeometries;
	private GameObject typesParent;

    [Range(0.1f, 1)]
    public float _cellShrink;
    public float _cellSize;
    /// <summary>
    /// the type that is out of the table are for matters of calc
    /// </summary>
    public int _outOfBoundsType = -2; // make sure this is indeed the type 
    public List<Color> _randomColors = new List<Color>();
    [Range(1f, 100)]
    public int _zAxisMultiplier;
    public int _addToYHeight = 450;

    /// <summary>
    /// get the # range of types
    /// </summary>
   // private float _rangeOfTypes;
    private float _rangeOfFloors;

    /// <summary>
    /// vars for neighbor searching 
    /// </summary>
    public int _windowSearchDim = 10;


	// Heatmaps
	private const int NUM_HEATMAPS = 3;
	public enum HeatmapType { RES = 0, OFFICE = 1, PARK = 2 };
	private HeatMap[] heatmaps;
	private GameObject heatmapsParent;

    private int _cellScoreCount = 0;

	private enum Mask { INTERACTIVE = 0, GRID = 1, FULL_SITE = 2, OUTSIDE = 3 };

	List<Brick> officeTypes = new List<Brick> { Brick.OL, Brick.OM, Brick.OS };
	List<Brick> resTypes = new List<Brick> { Brick.RL, Brick.RM, Brick.RS };

	private int interactiveIndex;
	private Vector2 interactiveGridLocation;
	private Vector2 interactiveGridDim;

	private cityIO cityIO;

	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	/// 
	/// SETUP
	/// 
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////

    void Awake()
    {
		cityIO = GameObject.Find ("cityIO").GetComponent<cityIO> ();

        _floorsList = AsciiParser.AsciiParserMethod(_asciiFloors);
        _typesList = AsciiParser.AsciiParserMethod(_asciiTypes);
        _masksList = AsciiParser.AsciiParserMethod(_asciiMasks);

		SetupFloors ();
		SetupHeatmaps ();
		SetupTypesViz ();

		EventManager.StartListening ("updateData", OnUpdateData);
		EventManager.StartListening ("scannersInitialized", FindInteractiveZone);

    }

	private void FindInteractiveZone() {
		interactiveIndex = -1;
		int index = 0;
		interactiveGridLocation = new Vector2 (0, 0);
		interactiveGridDim = new Vector2 (0, 0);

		// Find location of interactive grid
		for (int i = 0; i < _gridX; i++) {
			for (int j = 0; j < _gridY; j++) {
				if (_masksList[index] == (int) Mask.GRID && interactiveIndex < 0) {
					interactiveIndex = index;
					interactiveGridLocation.x = j;
					interactiveGridLocation.y = i;
					Debug.Log ("Mask index is " + interactiveIndex);
				}
				index++;
			}
		}

		interactiveGridDim = GameObject.Find ("ScannersParent").GetComponent<Scanners> ().GetGridDimensions();

		Debug.Log ("Interactive grid starts at " + interactiveGridLocation + "   and has dimensions:   " + interactiveGridDim + " with index " + interactiveIndex);
	}

	private void CreateParent(ref GameObject parent) {
		parent = new GameObject ();
		parent.transform.parent = this.transform;
		parent.SetActive (false);
	}

	/// <summary>
	/// Initializes the floor geometries
	/// </summary>
	private bool SetupFloors() {
		int index = 0;
		int _floorHeight = 0;

		if (floorsParent == null) {
			CreateParent (ref floorsParent);
			floorsParent.name = "Floors";
		}

		_floorsGeometries = new GameObject[(_gridX-1) * _gridY];
		_rangeOfFloors = (Mathf.Abs(_floorsList.Max()) + Mathf.Abs(_floorsList.Min()));

		for (int x = 0; x < _gridX - 1; x++) {
			for (int y = 0; y < _gridY; y++) {
				var _shiftFloorsHeightAboveZero = _floorsList[index] + Mathf.Abs(_floorsList.Min()); // move list item from subzero

				if (_typesList[index] != _outOfBoundsType && _floorsList[index] > 0)
				{ 
					// if not on the area which is out of the physical model space
					_floorsGeometries[index] = GameObject.CreatePrimitive(PrimitiveType.Cube); //make cell cube
					_floorsGeometries[index].name = (_floorsList[index].ToString() + "Floors ");
					_floorsGeometries[index].transform.parent = floorsParent.transform;
					_floorsGeometries[index].transform.localPosition = new Vector3(x * _cellSize, _shiftFloorsHeightAboveZero * (_zAxisMultiplier / 2) + _addToYHeight, y * _cellSize); //compensate for scale shift due to height                                                                                                                                                    
					//color the thing
					_floorsGeometries[index].transform.GetComponent<Renderer>().material.color = Color.HSVToRGB(1, 1, (_floorsList[index]) / _rangeOfFloors);// this creates color based on value of cell!
					_floorHeight = _shiftFloorsHeightAboveZero * _zAxisMultiplier;
					_floorsGeometries[index].transform.localScale = new Vector3(_cellShrink * _cellSize, _floorHeight, _cellShrink * _cellSize);
				}
				index++;
			}
		}
		return true;
	}


	private void UpdateFloor(int index) {
		
		if (_floorsGeometries [index] == null)
			return;

		// update range with new item
		_rangeOfFloors = (Mathf.Abs(_floorsList.Max()) + Mathf.Abs(_floorsList.Min()));
		
		var _shiftFloorsHeightAboveZero = _floorsList [index] + Mathf.Abs(_floorsList.Min());

		//_floorsGeometries[index].transform.localPosition = new Vector3(_floorsGeometries[index].transform.position.x, _shiftFloorsHeightAboveZero * (_zAxisMultiplier / 2) + _addToYHeight, _floorsGeometries[index].transform.position.y); //compensate for scale shift due to height                                                                                                                                                    
		//color the thing
		_floorsGeometries[index].transform.GetComponent<Renderer>().material.color = Color.HSVToRGB(1, 1, (_floorsList[index]) / _rangeOfFloors);// this creates color based on value of cell!
		float _floorHeight = _shiftFloorsHeightAboveZero * _zAxisMultiplier;
		_floorsGeometries[index].transform.localScale = new Vector3(_cellShrink * _cellSize, _floorHeight, _cellShrink * _cellSize);
	}

	/// <summary>
	/// Initializes the types visualization.
	/// </summary>
	/// <returns><c>true</c>, if types viz was setuped, <c>false</c> otherwise.</returns>
	private bool SetupTypesViz() {
		_loopsCounter = 0;
		//_rangeOfTypes = (Mathf.Abs(_typesList.Max()) + Mathf.Abs(_typesList.Min()));

		if (typesParent == null) {
			CreateParent (ref typesParent);
			typesParent.name = "Types";
		}

		if (_typesGeometries == null)
			_typesGeometries = new GameObject[(_gridX - 1) * _gridY];

		for (int x = 0; x < _gridX - 1; x++)
		{
			for (int y = 0; y < _gridY; y++)
			{
				var _shiftTypeListAboveZero = _typesList [_loopsCounter];
					//+ Mathf.Abs(_typesList.Min()); // move list item from subzero
				// var _shiftFloorListAboveZero = _floorsList[_loopsCounter] + Mathf.Abs(_floorsList.Min()); // move list item from subzero

				if (_typesList[_loopsCounter] != _outOfBoundsType)
				{ // if not on the area which is out of the physical model space
					_typesGeometries[_loopsCounter] = GameObject.CreatePrimitive(PrimitiveType.Quad); //make cell cube
					_typesGeometries[_loopsCounter].name = ("Types " + _typesList[_loopsCounter].ToString() + " " + _loopsCounter.ToString());
					/*_typesGeometries.transform.localPosition = new Vector3(x * _cellSize,
                       _shiftFloorListAboveZero * _zAxisMultiplier + _addToYHeight,
                      y * _cellSize);   //move and rotate */
					_typesGeometries[_loopsCounter].transform.localPosition = new Vector3(x * _cellSize, _addToYHeight, y * _cellSize);   //move and rotate
					Quaternion _tmpRot = transform.localRotation;
					_tmpRot.eulerAngles = new Vector3(90, 0, 0.0f);
					_typesGeometries[_loopsCounter].transform.localRotation = _tmpRot;
					_typesGeometries[_loopsCounter].transform.localScale = new Vector3(_cellShrink * _cellSize,
						_cellShrink * _cellSize,
						_cellShrink * _cellSize);
					_typesGeometries[_loopsCounter].transform.parent = typesParent.transform; //put into parent object for later control

					if (_typesList[_loopsCounter] == -1)
					{
						_typesGeometries[_loopsCounter].transform.localScale = new Vector3(0.25f * _cellSize, 0.25f * _cellSize, 0.25f * _cellSize);
						_typesGeometries[_loopsCounter].transform.GetComponent<Renderer>().material.color = Color.black;
					}
					else
					{
						Color currColor = cityIO.GetColor (_shiftTypeListAboveZero);
						_typesGeometries [_loopsCounter].transform.GetComponent<Renderer> ().material.color = currColor;
					}

					_typesGeometries[_loopsCounter].transform.GetComponent<Renderer>().receiveShadows = false;
					_typesGeometries[_loopsCounter].transform.GetComponent<Renderer>().shadowCastingMode =
						UnityEngine.Rendering.ShadowCastingMode.Off;
				}

				_loopsCounter++;
			}
		}

		return true;
	}

	private void UpdateType(int index) {
		if (_typesGeometries [index] == null)
			return;
		
		var _shiftTypeListAboveZero = _typesList[index]; 

		_typesGeometries[index].name = ("Types " + _typesList[_loopsCounter].ToString());

		if (_typesList[index] == -1)
		{
			//_typesGeometries[index].transform.localScale = new Vector3(0.25f * _cellSize, 0.25f * _cellSize, 0.25f * _cellSize);
			_typesGeometries[index].transform.GetComponent<Renderer>().material.color = Color.black;
		}
		else
		{
			_typesGeometries[index].transform.localScale = new Vector3(_cellShrink * _cellSize, _cellShrink * _cellSize, _cellShrink * _cellSize);
			Color currColor = cityIO.GetColor (_shiftTypeListAboveZero);
			_typesGeometries [index].transform.GetComponent<Renderer> ().material.color = currColor;
		}
	}


	private void SetupHeatmaps() {
		_loopsCounter = 0;

		// Initialize parent object
		if (heatmapsParent == null) {
			CreateParent (ref heatmapsParent);
			heatmapsParent.name = "Heatmaps";
			heatmaps = new HeatMap[NUM_HEATMAPS];
		}

		float cellSize = _cellShrink * _cellSize;
		for (int i = 0; i < NUM_HEATMAPS; i++) {
			heatmaps[i] = new HeatMap(_gridX-1, _gridY, _windowSearchDim, cellSize, _addToYHeight, ((HeatmapType)i).ToString());
			heatmaps [i].SetParent (heatmapsParent);

			// Set up search types & origin types for each heatmap
			switch (i) 
			{
				case (int) HeatmapType.OFFICE:
					heatmaps [i].SetOriginTypes (resTypes);
					heatmaps [i].SetSearchTypes (officeTypes);
					break;
				case (int) HeatmapType.RES:
					heatmaps [i].SetOriginTypes (officeTypes);
					heatmaps [i].SetSearchTypes (resTypes);
					break;
				case (int) HeatmapType.PARK:
					// TEMP
					heatmaps [i].SetOriginTypes (resTypes);
					heatmaps [i].SetSearchTypes (officeTypes);
					break;
			}
		}

		// Initialize geos
		for (int x = 0; x < _gridX - 1; x++) {
			for (int y = 0; y < _gridY; y++) {
				_loopsCounter++;
				if (_typesList [_loopsCounter] != _outOfBoundsType) { // if not on the area which is out of the physical model space
					// Init heatmap geometries for each heatmap object
					foreach (HeatMap hm in heatmaps) {
						hm.CreateHeatmapGeo (x, y, _loopsCounter, _typesList [_loopsCounter]);
					}
				}
			}
		}

		UpdateHeatmaps ();
	}

	private void UpdateHeatmaps() {
		int index = 0;
		for (int x = 0; x < _gridX-1; x++) {
			for (int y = 0; y < _gridY; y++) {
				if (_typesList [index] != _outOfBoundsType) {
					foreach (HeatMap hm in heatmaps) {
						hm.UpdateHeatmap (x, y, _typesList [index], index);
					}
				}
				index++;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	/// 
	/// UPDATES
	/// 
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////


	/// 
    /// <summary>
    /// Viz of floor heights 
    /// </summary>
    public void FloorsViz() // make the height map //
    {
		if (_typesList.Count < 0)
			return;
		
		if (_floorsGeometries == null || _floorsGeometries.Length <= 0)
			SetupFloors ();

		floorsParent.SetActive (true);
    }

    /// <summary>
    /// Viz of different landuse types 
    /// </summary>
    public void TypesViz() // create types map //
    {
		if (_typesGeometries == null)
			SetupTypesViz ();

		typesParent.SetActive (true);
    }

    /// <summary>
    /// ------------
    /// PSEUDO CODE:
    /// ------------
    /// create array from data 
    /// run 2xloops of x, y
    /// find location of item x,y
    /// store its location in new array
    /// create search 'window' around it:
    /// [x-n, x+n, y-n, y+n]
    /// if found Target item, measure Manhatten distance to it
    /// add distances to _varDist and create new array of [x,y,_varDist]
    /// loop through array, look for min, max of _varDist
    /// assign color/Y axis/other viz based on value
    ///
    /// </summary>

    public void HeatmapViz(HeatmapType heatmapType)
    {
		if (heatmapsParent == null)
			SetupHeatmaps ();

		heatmapsParent.SetActive (true);

		for (int i = 0; i < heatmaps.Length; i++) {
			if (i == (int) heatmapType)
				heatmaps[(int) heatmapType].SetParentActive (true);
			else
				heatmaps[(int) heatmapType].SetParentActive (false);
		}
    }
		
	/// <summary>
	/// ~~~~~ COULD THREAD ~~~~~
	/// Raises the update data event.
	/// </summary>
	public void OnUpdateData() {
		UpdateFloorsAndTypes ();
		UpdateHeatmaps ();
	}

	/// <summary>
	/// Updates the types // only update interactive part
	/// </summary>
	private void UpdateFloorsAndTypes() {
		Debug.Log ("Update floors & types in HeatMaps.");

		int index = interactiveIndex;
		int gridIndex = 0;

		///
		/// The interactive grid is indexed the other way! 
		/// so have to iterate r-l up-down
		/// 
		for (int j = (int)(interactiveGridLocation.x + interactiveGridDim.x); j > (int)interactiveGridLocation.x ; j--) {
			for (int i = (int)interactiveGridLocation.y; i < (int)(interactiveGridLocation.y + interactiveGridDim.y); i++) {
				index = i * (int)(_gridY) + j;
				// Update interactive part only
				if (cityIO.ShouldUpdateGrid(gridIndex) && _masksList[index] == (int)Mask.INTERACTIVE) {
					_typesList [index] = cityIO.GetGridType (gridIndex);
					_floorsList [index] = cityIO.GetFloorHeight (gridIndex);
					UpdateType (index);
					UpdateFloor (index);
				}
				gridIndex++;
			}
		}
	}

	/// <summary>
	/// Returns the value of the mask (from the ASCII file) at the given location in the Table grid.
	/// </summary>
	/// <returns>The mask.</returns>
	/// <param name="index">Index.</param>
	public int GetMask(int index) {
		int currJ = index % (int)interactiveGridDim.y;
		int currI = (int) (index / interactiveGridDim.y);
		int i = (int) (interactiveGridLocation.x + interactiveGridDim.y) - currI;
		int j = (int) interactiveGridLocation.y + currJ;
		int remappedIndex = j * (int)(_gridY) + i;

		return _masksList[remappedIndex];
	}

	/// <summary>
	/// Determines whether the mask shows that the module is interactive at the specified index.
	/// </summary>
	/// <returns><c>true</c> if this instance is interactive the specified index; otherwise, <c>false</c>.</returns>
	/// <param name="index">Index.</param>
	public bool IsInteractive(int index) {
		return (GetMask (index) == (int)Mask.INTERACTIVE);
	}

}