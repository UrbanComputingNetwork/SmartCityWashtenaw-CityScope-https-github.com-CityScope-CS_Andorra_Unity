using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stateManager : MonoBehaviour
{

    public GameObject _contextHolder;
    public GameObject _heatmapHolder;
    public cityIO _cityIOscript;
    public HeatMaps _heatmapsScript;
    public GameObject _andorraCityScope;
    public GameObject _andorraOld;

    public int _sliderState = 0;
    private int _oldState;

    void Awake()
    {
        _sliderState = _cityIOscript._table.objects.toggle1;
        _oldState = _sliderState;
        StateControl(_sliderState);
    }

    void Update()
    {
        if (_sliderState != _oldState)
        {
            StateControl(_sliderState);
            _oldState = _sliderState;
        }
    }

    void StateControl(int _sliderState)
    {
        switch (_sliderState)
        {
            default:
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraOld);
                print("Default: Basic Sat view and cityIO grid" + '\n');
                break;
            case 0: //grid
                CleanOldViz(_contextHolder, _heatmapHolder); ShowContext(_andorraCityScope);
                print("State 0: Basic Sat view and cityIO grid" + '\n');
                break;
            case 1:// land use
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraCityScope);
                _heatmapsScript.FloorsViz();
                print("State 1: Land use map" + '\n');

                break;
            case 2: // density 
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraCityScope);
                _heatmapsScript.TypesViz();

                print("State 2: Density (floors) map" + '\n');

                break;
            case 3: // heatmap 
                CleanOldViz(_contextHolder, _heatmapHolder);
                ShowContext(_andorraCityScope);
                _heatmapsScript.SearchNeighbors();

                print("State 3: Proximity HeatMap" + '\n');
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


