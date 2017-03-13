using UnityEngine;
using System.Collections;
using System;

public class ScaleSliderHandler : MonoBehaviour {

    public GameObject scalableContext;
    public float initialValue = 0.01f;

	// Use this for initialization
	void Start () {
        SliderUpdated(initialValue);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SliderUpdated(float value)
    {
        var scale = GetScaleFromValue(value);
        scalableContext.transform.localScale = new Vector3(scale, scale, scale);
    }

    private float GetScaleFromValue(float value)
    {

		return (value * value); //x^2 non linear
    }
}   
