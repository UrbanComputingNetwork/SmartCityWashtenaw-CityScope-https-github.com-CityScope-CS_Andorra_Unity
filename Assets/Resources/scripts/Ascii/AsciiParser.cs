using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;


public class AsciiParser : MonoBehaviour
{
	/// <summary>
	/// The ASCII types txt files.
	/// </summary>
	public TextAsset _asciiTypes;
	/// <summary>
	/// The ASCII floors txt files.
	/// </summary>
	public TextAsset _asciiFloors;

	public int _outOfBoundsType = -2;
	// the type that is out of the table are for matters of calc
	/// <summary>
	/// to be replaced with x,y dim from ascii parsing
	/// </summary>
	public int _gridX;
	public int _gridY;
	/// <summary>
	/// The GO to show the grid
	/// </summary>
	private GameObject _floorsGeometry;
	private GameObject _typesGeometry;

	public float _cellSize;
	[Range (0.1f, 1)]
	public float _cellShrink;

	[Range (1f, 100)]
	public int _zAxisMultiplier;
	/// <summary>
	/// counter for the double loop
	/// </summary>
	private int _counter = -1;

	/// <summary>
	/// get the # range of types
	/// </summary>
	private float _rangeOfTypes;
	private float _rangeOfFloors;

	List<int> _listTypes = new List<int> ();
	List<int> _listFloors = new List<int> ();
	public List<Color> _randomColors = new List<Color> ();

	void Start ()
	{

		_listTypes = AsciiParserMethod (_asciiTypes);
		_listFloors = AsciiParserMethod (_asciiFloors);
		_rangeOfTypes = (Mathf.Abs (_listTypes.Max ()) + Mathf.Abs (_listTypes.Min ()));
		_rangeOfFloors = (Mathf.Abs (_listFloors.Max ()) + Mathf.Abs (_listFloors.Min ()));

		DataViz ();
		HeatMap ();
	}



	private void DataViz ()
	{
		for (int x = 0; x < _gridX - 1; x++) {
			for (int y = 0; y < _gridY; y++) {
				//count the loops
				_counter = _counter + 1;
				var _shiftTypeListAboveZero = _listTypes [_counter] + Mathf.Abs (_listTypes.Min ()); // move list item from subzero
				var _shiftFloorListAboveZero = _listFloors [_counter] + Mathf.Abs (_listTypes.Min ()); // move list item from subzero

				if (_listTypes [_counter] != _outOfBoundsType) { // if not on the area which is out of the physical model space

					//make the height map //

					_floorsGeometry = GameObject.CreatePrimitive (PrimitiveType.Cube); //make cell cube
					_floorsGeometry.name = ("Floors " + _listFloors [_counter].ToString ());
					_floorsGeometry.transform.parent = transform; //put into parent object for later control
					_floorsGeometry.transform.localScale = new Vector3 (_cellShrink * _cellSize,
						_shiftFloorListAboveZero * _zAxisMultiplier,
						_cellShrink * _cellSize); 	//move and rotate
					_floorsGeometry.transform.localPosition = new Vector3 (x * _cellSize,
						_shiftFloorListAboveZero * (_zAxisMultiplier / 2),
						y * _cellSize); //compensate for scale shift due to height
					//color the thing
					_floorsGeometry.transform.GetComponent<Renderer> ().material.color =
						Color.HSVToRGB (.1f, 1, (_listFloors [_counter]) / _rangeOfFloors);// this creates color based on value of cell!

					// create types map //

					_typesGeometry = GameObject.CreatePrimitive (PrimitiveType.Quad); //make cell cube
					_typesGeometry.name = ("Types " + _listTypes [_counter].ToString ());
					_typesGeometry.transform.localPosition =
						new Vector3 (x * _cellSize, 200, y * _cellSize); //compensate for scale shift due to height
					Quaternion _tmpRot = transform.localRotation;
					_tmpRot.eulerAngles = new Vector3 (90, 0, 0.0f);
					_typesGeometry.transform.localRotation = _tmpRot;
					_typesGeometry.transform.localScale = new Vector3 (_cellShrink * _cellSize,
						_cellShrink * _cellSize,
						_cellShrink * _cellSize);
					_typesGeometry.transform.parent = transform; //put into parent object for later control
					for (int c = 0; c < _rangeOfTypes + 1; c++) { //!!! Must add one to count since _numberoftypes is 12, not 13!
						var _rnd = Random.Range (0f, 1f);
						_randomColors.Add (Color.HSVToRGB (0.25f, 1, _rnd));
					}
					_typesGeometry.transform.GetComponent<Renderer> ().material.color =
						_randomColors [_shiftTypeListAboveZero];
					
				}
			}
		}
	}

	/// <summary>
	/// ------------
	/// PSAUDO CODE:
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


	private void HeatMap ()
	{
		int[,] _typesArray = new int[_gridX, _gridY];
		_counter = 0; 
		for (int x = 0; x < _gridX - 1; x++) {
			for (int y = 0; y < _gridY; y++) {
				_counter = _counter + 1;
				_typesArray [x, y] = _listTypes [_counter];
				if (_typesArray [x, y] == 2) { //or whatever
				
					var _tmpCount = 0; //decalre a tmp counter  
						
					for (int _windowX = x - 10; _windowX < x + 10; _windowX++) {
						for (int _windowY = y - 10; _windowY < y + 10; _windowY++) {
							
							if (_windowX > 0
							    && _windowY > 0
							    && _windowX < _gridX
							    && _windowY < _gridY) {
								if (_typesArray [_windowX, _windowY] == 4) {
									_tmpCount += 1; 
									print (" Landuse type: " + _typesArray [x, y] +
									" in X Loc: " + x + " Y Loc: " + y +
									" has searched type " + _typesArray [_windowX, _windowY] +
									"  around it in X:" + _windowX + " Y: " + _windowY + '\n');
								}
							}
						}
					}
					if (_tmpCount > 0) {
						print ("how many found for this cell? >>>>>> " + _tmpCount + '\n'); 

					}
				}

			}
		}
	}

	/// <summary>
	/// Parse a txt File from sting to int.
	/// </summary>
	/// <param name="txtFile">Text file.</param>
	private List<int> AsciiParserMethod (TextAsset txtFile)
	{
		List<int> _tmpList = new List<int> ();
		string _txtRemovedNewlines = txtFile.text.Replace ('\n', ' ');
		string[] _strValues = _txtRemovedNewlines.Split (' ');
		foreach (string str in _strValues) {
			var _outVal = 0;
			if (int.TryParse (str, out _outVal)) {
				_tmpList.Add (System.Int32.Parse (str));
			}
		}
		return _tmpList; // the temp list that this method returns
	}
}