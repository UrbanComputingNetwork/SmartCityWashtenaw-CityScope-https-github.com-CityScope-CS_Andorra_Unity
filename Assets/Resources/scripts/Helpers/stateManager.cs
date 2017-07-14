using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class stateManager : MonoBehaviour
{
    public GameObject _contextHolder;
    public GameObject _cellTowers;
    public GameObject _heatmapHolder;
    public cityIO _cityIOscript;
    public HeatMaps _heatmapsScript;
    public GameObject _andorraCityScope;
    public GameObject _andorraHeatmap;
    public GameObject _floorsUI;
    private int _sliderState = 1;
    private int _oldState;
    public bool _demoModeBool = true;
    public int _changeModeEverySeconds = 60;
    void Awake()
    {
        if (_demoModeBool != true)
        {
            _sliderState = _cityIOscript._table.objects.toggle1; //gets the slider 
            _oldState = _sliderState;
            StateControl(_sliderState);
        }
        else
        {
            StartCoroutine(DemoMode());
        }
    }
    void Update()
    {
        _sliderState = _cityIOscript._table.objects.toggle1; //gets the slider 
        if (_sliderState != _oldState)
        {
            StateControl(_sliderState);
            _oldState = _sliderState;
        }
    }
    IEnumerator DemoMode()
    {
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForEndOfFrame();
                StateControl(i);
                yield return new WaitForSeconds(_changeModeEverySeconds);
            }
        }
    }
    void StateControl(int _sliderState)
    {
        switch (_sliderState)
        {
            default:
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraCityScope);
                print("Default: Basic Sat view and cityIO grid" + '\n');
                _floorsUI.SetActive(false);

                break;
            case 0: //CITYIO
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraCityScope);
                print("State 0: Basic Sat view and cityIO grid" + '\n');
                _floorsUI.SetActive(false);
                break;
            case 1: // LANDUSE 
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraHeatmap);
                _heatmapsScript.TypesViz();
                print("State 2: Land use map" + '\n');
                _floorsUI.SetActive(false);
                break;
            case 2:// FLOORS
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraHeatmap);
                _heatmapsScript.FloorsViz();
                _floorsUI.SetActive(true);

                print("State 1: Floors map" + '\n');
                break;
            case 3: // HEATMAP

                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraHeatmap);
                _heatmapsScript.SearchNeighbors();
                print("State 3: Proximity HeatMap" + '\n');
                _floorsUI.SetActive(false);
                break;
            case 4: // Cell towers
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraHeatmap);
                ShowContext(_cellTowers);
                print("State 4: Celltowers heatmap" + '\n');
                _floorsUI.SetActive(false);
                break;
        }
    }
    void CleanOldViz(GameObject _contextHolder, GameObject _heatmapHolder)
    {
        foreach (Transform child in _contextHolder.transform)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in _heatmapHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }
    void ShowContext(GameObject t)
    {
        t.transform.gameObject.SetActive(true);
    }
}


