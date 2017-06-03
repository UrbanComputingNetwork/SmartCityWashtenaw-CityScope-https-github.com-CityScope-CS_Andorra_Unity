using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;


public class AsciiParser : MonoBehaviour
{
	/// <summary>
	/// The ASCII txt files.
	/// </summary>
	public TextAsset asciiTypes;
	public TextAsset asciiHeights;

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
	private int _counter = 0;


	private float _rangeOfTypes;
	private float _rangeOfFloors;

	List<int> _listTypes = new List<int> ();
	List<int> _listFloors = new List<int> ();
	public List<Color> _randomColors = new List<Color> ();

	/// <summary>
	/// Parse a txt File from sting to int.
	/// </summary>
	/// <param name="txtFile">Text file.</param>
	private List<int> Parser (TextAsset txtFile)
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

	private void AsciiViz ()
	{
		for (int c = 0; c < _rangeOfTypes + 1; c++) { //!!! Must add one to count since _numberoftypes is 12, not 13!
			var _rnd = Random.Range (0f, 1f);
			_randomColors.Add (Color.HSVToRGB (0.25f, 1, _rnd));

		}
		for (int i = 0; i < _gridX - 1; i++) {
			for (int n = 0; n < _gridY; n++) {
				//count the loops 
				_counter = _counter + 1; 
				var _shiftTypeListAboveZero = _listTypes [_counter] + Mathf.Abs (_listTypes.Min ()); // move list item from subzero 
				var _shiftFloorListAboveZero = _listFloors [_counter] + Mathf.Abs (_listTypes.Min ()); // move list item from subzero 

				if (_listTypes [_counter] != _outOfBoundsType) { // if not on the area which is out of the model space

					//make the height map 
					_floorsGeometry = GameObject.CreatePrimitive (PrimitiveType.Cube); //make cell cube
					_floorsGeometry.name = ("Floors " + _listFloors [_counter].ToString ()); 
					_floorsGeometry.transform.parent = transform; //put into parent object for later control 
					//move and rotate 
					_floorsGeometry.transform.localScale = new Vector3 (_cellShrink * _cellSize, 
						_shiftFloorListAboveZero * _zAxisMultiplier, 
						_cellShrink * _cellSize);
					_floorsGeometry.transform.localPosition = new Vector3 (i * _cellSize, 
						_shiftFloorListAboveZero * (_zAxisMultiplier / 2), 
						n * _cellSize); //compensate for scale shift due to height
					//color the thing
					_floorsGeometry.transform.GetComponent<Renderer> ().material.color = 
						Color.HSVToRGB (.1f, 1, (_listFloors [_counter]) / _rangeOfFloors);// this creates color based on value of cell! 

					// create types map //
					_typesGeometry = GameObject.CreatePrimitive (PrimitiveType.Plane); //make cell cube
					_typesGeometry.name = ("Types " + _listTypes [_counter].ToString ());  
					_typesGeometry.transform.localPosition = 
						new Vector3 (i * _cellSize, 300, n * _cellSize); //compensate for scale shift due to height
					_typesGeometry.transform.localScale = new Vector3 (0.1f * _cellShrink * _cellSize, 1, 0.1f * _cellShrink * _cellSize);
					_typesGeometry.transform.parent = transform; //put into parent object for later control 
					_typesGeometry.transform.GetComponent<Renderer> ().material.color = 
						_randomColors [_shiftTypeListAboveZero]; 			


				}
			}
		}
	}


	void Start ()
	{
		_listTypes = Parser (asciiTypes);
		_listFloors = Parser (asciiHeights); 
		_rangeOfTypes = (Mathf.Abs (_listTypes.Max ()) + Mathf.Abs (_listTypes.Min ()));
		_rangeOfFloors = (Mathf.Abs (_listFloors.Max ()) + Mathf.Abs (_listFloors.Min ()));

		AsciiViz ();
	}

}