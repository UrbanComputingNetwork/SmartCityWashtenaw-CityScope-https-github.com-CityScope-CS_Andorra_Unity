using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class HeatMaps : MonoBehaviour
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
    // private List<int> _masksList = new List<int>();

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
    private float _rangeOfTypes;
    private float _rangeOfFloors;

    /// <summary>
    /// vars for neighbor searching 
    /// </summary>
    public int _winodwSearchDim = 10;
    private GameObject[] _neighborsGeometries;
	private GameObject neighborSearchParent;
    private int _cellScoreCount = 0;

    void Awake()
    {
        _floorsList = AsciiParser.AsciiParserMethod(_asciiFloors);
        _typesList = AsciiParser.AsciiParserMethod(_asciiTypes);
        // _masksList = AsciiParser.AsciiParserMethod(_asciiMasks);

		SetupFloors ();
		SetupNeighborSearch ();
		SetupTypesViz ();
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
					_floorsGeometries[index].transform.localPosition = new Vector3(x * _cellSize, _shiftFloorsHeightAboveZero * (_zAxisMultiplier / 2) + _addToYHeight, y * _cellSize); //compensate for scale shift due to height                                                                                                                                                    //color the thing
					_floorsGeometries[index].transform.GetComponent<Renderer>().material.color = Color.HSVToRGB(1, 1, (_floorsList[index]) / _rangeOfFloors);// this creates color based on value of cell!
					_floorHeight = _shiftFloorsHeightAboveZero * _zAxisMultiplier;
					_floorsGeometries[index].transform.localScale = new Vector3(_cellShrink * _cellSize, _floorHeight, _cellShrink * _cellSize);
				}
				index++;
			}
		}
		return true;
	}

	/// <summary>
	/// Initializes the types visualization.
	/// </summary>
	/// <returns><c>true</c>, if types viz was setuped, <c>false</c> otherwise.</returns>
	private bool SetupTypesViz() {
		_loopsCounter = 0;
		_rangeOfTypes = (Mathf.Abs(_typesList.Max()) + Mathf.Abs(_typesList.Min()));

		if (typesParent == null) {
			CreateParent (ref typesParent);
		}

		if (_typesGeometries == null)
			_typesGeometries = new GameObject[(_gridX - 1) * _gridY];

		for (int x = 0; x < _gridX - 1; x++)
		{
			for (int y = 0; y < _gridY; y++)
			{
				var _shiftTypeListAboveZero = _typesList[_loopsCounter] + Mathf.Abs(_typesList.Min()); // move list item from subzero
				// var _shiftFloorListAboveZero = _floorsList[_loopsCounter] + Mathf.Abs(_floorsList.Min()); // move list item from subzero

				if (_typesList[_loopsCounter] != _outOfBoundsType)
				{ // if not on the area which is out of the physical model space
					_typesGeometries[_loopsCounter] = GameObject.CreatePrimitive(PrimitiveType.Quad); //make cell cube
					_typesGeometries[_loopsCounter].name = ("Types " + _typesList[_loopsCounter].ToString());
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
						Color currColor = GameObject.Find ("cityIO").GetComponent<cityIO> ().GetColor (_shiftTypeListAboveZero);
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

    /// <summary>
    /// Viz of floor heights 
    /// </summary>
    public void FloorsViz() // make the height map //
    {
		if (_typesList.Count < 0)
			return;
		
		if (_floorsGeometries == null || _floorsGeometries.Length <= 0)
			SetupFloors ();

		typesParent.SetActive (false);
		neighborSearchParent.SetActive (false);
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
		neighborSearchParent.SetActive (false);
		floorsParent.SetActive (false);
		
    }

	private void CreateNeighborGeo(int x, int y, int index) {
		if (_neighborsGeometries == null)
			_neighborsGeometries = new GameObject[(_gridX - 1) * _gridY];
		
		_neighborsGeometries[index] = GameObject.CreatePrimitive(PrimitiveType.Quad); //make cell cube
		_neighborsGeometries[index].name = ("Type: " + _typesList[_loopsCounter]);
		_neighborsGeometries[index].transform.localPosition =
			new Vector3(x * _cellSize, _addToYHeight, y * _cellSize);
		Quaternion _tmpRot = transform.localRotation;
		_tmpRot.eulerAngles = new Vector3(90, 0, 0.0f);
		_neighborsGeometries[index].transform.localRotation = _tmpRot;
		_neighborsGeometries[index].transform.localScale = new Vector3(_cellShrink * _cellSize, _cellShrink * _cellSize, _cellShrink * _cellSize);
		_neighborsGeometries[index].transform.GetComponent<Renderer>().receiveShadows = false;
		_neighborsGeometries[index].transform.GetComponent<Renderer>().shadowCastingMode =
			UnityEngine.Rendering.ShadowCastingMode.Off;
		_neighborsGeometries[index].transform.parent = neighborSearchParent.transform; //put into parent object for later control
	}

	private void SetupNeighborSearch() {
		int[,] _typesArray = new int[_gridX, _gridY];
		_loopsCounter = 0;

		if (neighborSearchParent == null) {
			CreateParent (ref neighborSearchParent);
		}

		for (int x = 0; x < _gridX - 1; x++)
		{
			for (int y = 0; y < _gridY; y++)
			{
				_loopsCounter = _loopsCounter + 1;
				if (_typesList[_loopsCounter] != _outOfBoundsType)
				{ // if not on the area which is out of the physical model space

					CreateNeighborGeo (x, y, _loopsCounter);
					_typesArray[x, y] = _typesList[_loopsCounter];

					if (_typesArray[x, y] > 0 && _typesArray[x, y] < 3) // what is the cells type we're searching for? 
					{
						_neighborsGeometries[_loopsCounter].transform.GetComponent<Renderer>().material.color = Color.green;
						_neighborsGeometries[_loopsCounter].transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
						_cellScoreCount = 0; //decalre a tmp counter  
						for (int _windowX = x - _winodwSearchDim; _windowX < x + _winodwSearchDim; _windowX++)
						{
							for (int _windowY = y - _winodwSearchDim; _windowY < y + _winodwSearchDim; _windowY++)
							{
								if (_windowX > 0
									&& _windowY > 0
									&& _windowX < _gridX
									&& _windowY < _gridY)
								{ // make sure window area is not outside grid bounds 
									if (_typesArray[_windowX, _windowY] > 6 && _typesArray[_windowX, _windowY] < 9)
									{
										_cellScoreCount = _cellScoreCount + 1;
										_neighborsGeometries[_loopsCounter].transform.localPosition =
											new Vector3(x * _cellSize, _addToYHeight + (_cellScoreCount * 2), y * _cellSize);
										_neighborsGeometries[_loopsCounter].name = ("Results count: " + _cellScoreCount.ToString());
										var _tmpColor = _cellScoreCount / Mathf.Pow(2 * _winodwSearchDim, 2); // color color spectrum based on cell score/max potential score 
										_neighborsGeometries[_loopsCounter].transform.GetComponent<Renderer>().material.color =
											Color.HSVToRGB(_tmpColor, 1, 1);
									}
								}
							}
						}
					}
					else
					{
						_neighborsGeometries[_loopsCounter].transform.GetComponent<Renderer>().material.color =
							Color.HSVToRGB(0, 0, 0);
						_neighborsGeometries[_loopsCounter].transform.localScale =
							new Vector3(_cellShrink * _cellSize, _cellShrink * _cellSize, _cellShrink * _cellSize);
					}
				}
			}
		}
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

    public void SearchNeighbors()
    {
		if (_neighborsGeometries == null)
			SetupNeighborSearch ();
		
		neighborSearchParent.SetActive (true);
		typesParent.SetActive (false);
		floorsParent.SetActive (false);
    }
}