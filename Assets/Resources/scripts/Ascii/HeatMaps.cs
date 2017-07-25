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
    private GameObject _typesGeometry;
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
    private GameObject _neighborsGeometry;
    private int _cellScoreCount = 0;

    void Awake()
    {
        _floorsList = AsciiParser.AsciiParserMethod(_asciiFloors);
        _typesList = AsciiParser.AsciiParserMethod(_asciiTypes);
        // _masksList = AsciiParser.AsciiParserMethod(_asciiMasks);
    }

	/// <summary>
	/// Initializes the floor geometries
	/// </summary>
	private bool SetupFloors() {
		int index = 0;
		_floorsGeometries = new GameObject[(_gridX-1) * _gridY];

		for (int x = 0; x < _gridX - 1; x++) {
			for (int y = 0; y < _gridY; y++) {
				if (_typesList[index] != _outOfBoundsType && _floorsList[index] > 0)
				{ 
					// if not on the area which is out of the physical model space
					_floorsGeometries[index] = GameObject.CreatePrimitive(PrimitiveType.Cube); //make cell cube
					_floorsGeometries[index].name = (_floorsList[index].ToString() + "Floors ");
					_floorsGeometries[index].transform.parent = transform; //put into parent object for later control
				}
				index++;
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
		
        _rangeOfFloors = (Mathf.Abs(_floorsList.Max()) + Mathf.Abs(_floorsList.Min()));

		int index = 0;
		int _floorHeight = 0;

        for (int x = 0; x < _gridX - 1; x++) {
            for (int y = 0; y < _gridY; y++) {
				var _shiftFloorsHeightAboveZero = _floorsList[index] + Mathf.Abs(_floorsList.Min()); // move list item from subzero
				if (_typesList[index] != _outOfBoundsType && _floorsList[index] > 0)
                { // if not on the area which is out of the physical model space
					if (_floorsGeometries [index] == null) {
						Debug.Log ("index " + index + " not found in floor viz list");
						continue;
					}

					_floorsGeometries[index].transform.localPosition = new Vector3(x * _cellSize, _shiftFloorsHeightAboveZero * (_zAxisMultiplier / 2) + _addToYHeight, y * _cellSize); //compensate for scale shift due to height                                                                                                                                                    //color the thing
					_floorsGeometries[index].transform.GetComponent<Renderer>().material.color = Color.HSVToRGB(1, 1, (_floorsList[index]) / _rangeOfFloors);// this creates color based on value of cell!
                    _floorHeight = _shiftFloorsHeightAboveZero * _zAxisMultiplier;
					_floorsGeometries[index].transform.localScale = new Vector3(_cellShrink * _cellSize, _floorHeight, _cellShrink * _cellSize);
                }

				index++;
            }
        }
    }

    /// <summary>
    /// Viz of different landuse types 
    /// </summary>
    public void TypesViz() // create types map //
    {
        _loopsCounter = 0;
        _rangeOfTypes = (Mathf.Abs(_typesList.Max()) + Mathf.Abs(_typesList.Min()));

        for (int x = 0; x < _gridX - 1; x++)
        {
            for (int y = 0; y < _gridY; y++)
            {
                var _shiftTypeListAboveZero = _typesList[_loopsCounter] + Mathf.Abs(_typesList.Min()); // move list item from subzero
                                                                                                       // var _shiftFloorListAboveZero = _floorsList[_loopsCounter] + Mathf.Abs(_floorsList.Min()); // move list item from subzero

                if (_typesList[_loopsCounter] != _outOfBoundsType)
                { // if not on the area which is out of the physical model space
                    _typesGeometry = GameObject.CreatePrimitive(PrimitiveType.Quad); //make cell cube
                    _typesGeometry.name = ("Types " + _typesList[_loopsCounter].ToString());
                    /*_typesGeometry.transform.localPosition = new Vector3(x * _cellSize,
                       _shiftFloorListAboveZero * _zAxisMultiplier + _addToYHeight,
                      y * _cellSize);   //move and rotate */
                    _typesGeometry.transform.localPosition = new Vector3(x * _cellSize, _addToYHeight, y * _cellSize);   //move and rotate
                    Quaternion _tmpRot = transform.localRotation;
                    _tmpRot.eulerAngles = new Vector3(90, 0, 0.0f);
                    _typesGeometry.transform.localRotation = _tmpRot;
                    _typesGeometry.transform.localScale = new Vector3(_cellShrink * _cellSize,
                        _cellShrink * _cellSize,
                        _cellShrink * _cellSize);
                    _typesGeometry.transform.parent = transform; //put into parent object for later control
                    for (int c = 0; c < _rangeOfTypes + 1; c++)
                    { //!!! Must add one to count since _numberoftypes is 12, not 13!
                        var _rnd = Random.Range(0f, 1f);
                        _randomColors.Add(Color.HSVToRGB(1 - _rnd, 1, 1));
                    }
                    if (_typesList[_loopsCounter] == -1)
                    {
                        _typesGeometry.transform.localScale = new Vector3(0.25f * _cellSize, 0.25f * _cellSize, 0.25f * _cellSize);
                        _typesGeometry.transform.GetComponent<Renderer>().material.color = Color.black;
                    }
                    else
                    {
                        _typesGeometry.transform.GetComponent<Renderer>().material.color = _randomColors[_shiftTypeListAboveZero];
                    }
                    _typesGeometry.transform.GetComponent<Renderer>().receiveShadows = false;
                    _typesGeometry.transform.GetComponent<Renderer>().shadowCastingMode =
                        UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                _loopsCounter = _loopsCounter + 1;//count the loops
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
        int[,] _typesArray = new int[_gridX, _gridY];
        _loopsCounter = 0;

        for (int x = 0; x < _gridX - 1; x++)
        {
            for (int y = 0; y < _gridY; y++)
            {
                _loopsCounter = _loopsCounter + 1;
                if (_typesList[_loopsCounter] != _outOfBoundsType)
                { // if not on the area which is out of the physical model space

                    _neighborsGeometry = GameObject.CreatePrimitive(PrimitiveType.Quad); //make cell cube
                    _neighborsGeometry.name = ("Type: " + _typesList[_loopsCounter]);
                    _neighborsGeometry.transform.localPosition =
                        new Vector3(x * _cellSize, _addToYHeight, y * _cellSize);
                    Quaternion _tmpRot = transform.localRotation;
                    _tmpRot.eulerAngles = new Vector3(90, 0, 0.0f);
                    _neighborsGeometry.transform.localRotation = _tmpRot;
                    _neighborsGeometry.transform.localScale = new Vector3(_cellShrink * _cellSize, _cellShrink * _cellSize, _cellShrink * _cellSize);
                    _neighborsGeometry.transform.GetComponent<Renderer>().receiveShadows = false;
                    _neighborsGeometry.transform.GetComponent<Renderer>().shadowCastingMode =
                        UnityEngine.Rendering.ShadowCastingMode.Off;
                    _neighborsGeometry.transform.parent = transform; //put into parent object for later control
                    _typesArray[x, y] = _typesList[_loopsCounter];

                    if (_typesArray[x, y] > 0 && _typesArray[x, y] < 3) // what is the cells type we're searching for? 
                    {
                        _neighborsGeometry.transform.GetComponent<Renderer>().material.color = Color.green;
                        _neighborsGeometry.transform.localScale = new Vector3(_cellSize, _cellSize, _cellSize);
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
                                        _neighborsGeometry.transform.localPosition =
                                      new Vector3(x * _cellSize, _addToYHeight + (_cellScoreCount * 2), y * _cellSize);
                                        _neighborsGeometry.name = ("Results count: " + _cellScoreCount.ToString());
                                        var _tmpColor = _cellScoreCount / Mathf.Pow(2 * _winodwSearchDim, 2); // color color spectrum based on cell score/max potential score 
                                        _neighborsGeometry.transform.GetComponent<Renderer>().material.color =
                                         Color.HSVToRGB(_tmpColor, 1, 1);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _neighborsGeometry.transform.GetComponent<Renderer>().material.color =
                                           Color.HSVToRGB(0, 0, 0);
                        _neighborsGeometry.transform.localScale =
                        new Vector3(_cellShrink * _cellSize, _cellShrink * _cellSize, _cellShrink * _cellSize);
                    }
                }
            }
        }
    }
}