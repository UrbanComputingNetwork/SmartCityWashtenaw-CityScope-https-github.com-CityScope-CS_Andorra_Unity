using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class CameraSwap : MonoBehaviour

{


    public Transform objectContext;

    public Transform NormalHolder;
    public Transform ARHolder;

    public List<GameObject> UIOnlyObjects;
    public List<GameObject> AROnlyObjects;

    private bool UIMode = true;
//	public GameObject uiSlider;

	void Awake()
	{
        if (UIMode)
            ActivateUI();
        else
            ActivateAR();
	}

	public void Switch() 
	{
        UIMode = !UIMode;
        if (UIMode)
            ActivateUI();
        else
            ActivateAR();
    }

    public void ActivateUI()
    {
        AROnlyObjects.ForEach(obj => obj.SetActive(false));
        UIOnlyObjects.ForEach(obj => obj.SetActive(true));
        objectContext.SetParent(NormalHolder, false);
    }

    public void ActivateAR()
    {
        UIOnlyObjects.ForEach(obj => obj.SetActive(false));
        AROnlyObjects.ForEach(obj => obj.SetActive(true));
        objectContext.SetParent(ARHolder, false);
    }
}
	

