using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SceneSwitchButtonHandler : MonoBehaviour {

    public List<GameObject> sceneList;

    private int currentIndex = 0;

	// Use this for initialization
	void Start () {
        
        ActivateObject(currentIndex);
	}

    private void ActivateObject(int index)
    {
        //Deactivate all objects
        sceneList.ForEach(o => o.SetActive(false));
        sceneList[index].SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void NextScene()
    {
        currentIndex = (currentIndex + 1) % sceneList.Count;
        ActivateObject(currentIndex);
    }
}
