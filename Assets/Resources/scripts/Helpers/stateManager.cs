using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stateManager : MonoBehaviour
{

    cityIO _script;
    public GameObject _cityIOgameObj;
    public GameObject _state1;
    public GameObject _state2;
    public GameObject _state3;

    void Start()
    {
        _script = _cityIOgameObj.transform.GetComponent<cityIO>();
    }
    void Update()
    {
        if (_script._flag == true)
        { // data IS flowing from cityIO 

            if (_script._Cells.objects.toggle1 == 0)
            {
                cleanOthers(gameObject);
                showState(_state1);

            }
            else if (_script._Cells.objects.toggle1 == 1)
            {
                cleanOthers(gameObject);
                showState(_state2);


            }
            else if (_script._Cells.objects.toggle1 >= 2)
            {
                cleanOthers(gameObject);
                showState(_state3);


            }
            else
            {

                cleanOthers(gameObject);
                showState(_state1);
            }

        }
    }

    void cleanOthers(GameObject t)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void showState(GameObject t)
    {
        t.transform.gameObject.SetActive(true);
    }
}

