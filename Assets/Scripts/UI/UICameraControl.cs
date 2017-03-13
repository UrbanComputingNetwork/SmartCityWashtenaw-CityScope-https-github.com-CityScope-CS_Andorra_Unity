using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Lean;
using System;

public class UICameraControl : MonoBehaviour

{
	public float MinSize = 25.0f;
	public float MaxSize = 1000.0f;
	public GameObject UICamera;
    public LayerMask interactableLayer;
	public float rotationangle = 15;
	private float angle = 45;

    private HashSet<LeanFinger> fingersBusy;

    protected virtual void OnEnable()
    {
        fingersBusy = new HashSet<LeanFinger>();
        // Hook into the OnFingerDown event
        Lean.LeanTouch.OnFingerDown += OnFingerDown;

        // Hook into the OnFingerUp event
        Lean.LeanTouch.OnFingerUp += OnFingerUp;
    }

    private void OnFingerUp(LeanFinger finger)
    {
        fingersBusy.Remove(finger);
    }

    private void OnFingerDown(LeanFinger finger)
    {
        // Raycast information
        var ray = finger.GetRay();
        var hit = default(RaycastHit);

        // Was this finger pressed down on a collider?
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, interactableLayer) == true)
        {
            // Was that collider this one?
            fingersBusy.Add(finger);
        }
    }

    protected virtual void OnDisable()
    {
        // Unhook the OnFingerDown event
        Lean.LeanTouch.OnFingerDown -= OnFingerDown;

        // Unhook the OnFingerUp event
        Lean.LeanTouch.OnFingerUp -= OnFingerUp;
    }

    protected virtual void LateUpdate()
	{
        if (fingersBusy.Count > 0)
            return;
		// Does the main camera exist?
		if (Camera.main != null)
		{
			// Make sure the pinch scale is valid
			if (Lean.LeanTouch.PinchScale > 0.0f)
			{
				// Scale the FOV based on the pinch scale
				Camera.main.orthographicSize /= Lean.LeanTouch.PinchScale;

				// Make sure the new FOV is within our min/max
				Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MinSize, MaxSize);
			}
		}
			
		// This will move the current transform based on a finger drag gesture
		Lean.LeanTouch.MoveObject(transform, Lean.LeanTouch.DragDelta * -1, Camera.main);

		// This will rotate the current transform based on a multi finger twist gesture
		angle = angle + ((Lean.LeanTouch.MultiDragDelta.x)/5f);
		UICamera.transform.rotation = Quaternion.Euler(rotationangle, angle, 0);
		//Debug.Log (angle);

	}
}
	