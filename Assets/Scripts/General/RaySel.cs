using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RaySel : MonoBehaviour {

	//public Camera camera;
	//public Material materialAfterSelection; 
	//public Material materialBeforeSelection;

    public GridBuildingSpawner GridManager;

    public HashSet<GridBuildingController> SelectedBuildings;
    public List<MonoBehaviour> cameraMovementScripts;

    
    private bool IsOn = false;

    private bool eraserOn = false;
    private bool selected;

    public bool PaintModeOn
    {
        get
        {
            return IsOn;
        }
        set
        {
            IsOn = value;
            GridManager.UpdateOn = !value;
            ColorAll(value);
            eraserOn = false;
            foreach (var i in cameraMovementScripts)
                i.enabled = !value;
        }
    }

    public void SwitchPaintMode()
    {
        PaintModeOn = !PaintModeOn;
    }

    public void SwitchEraser()
    {
        eraserOn = !eraserOn;
    }
	// Use this for initialization
	void Start ()
    {
        SelectedBuildings = new HashSet<GridBuildingController>();
	
	}
	
	// Update is called once per frame

	void Update()
	{
        if (!IsOn)
            return;

		if (Input.GetMouseButton(0))
		{
			//Debug.Log("Mouse is down");

			RaycastHit hitInfo = new RaycastHit();
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo);
            // Debug.DrawRay(ray.origin, ray.direction, Color.red, 10);
            
			if (hit) 
			{
				//Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                
				GameObject designPieces = hitInfo.transform.gameObject;
                var controller = designPieces.GetComponent<GridBuildingController>();
                
                if (controller != null)
				{
                    selected = SelectedBuildings.Contains(controller);
                    if (!selected && !eraserOn)
                    {
                        SelectedBuildings.Add(controller);
                        controller.MarkObject(true);
                    } else if(selected && eraserOn)
                    {
                        SelectedBuildings.Remove(controller);
                        controller.MarkObject(false);
                    }
				}

			}
		} 
	}

    
    private void ColorAll(bool isOn)
    {
        foreach(var obj in SelectedBuildings)
        {
            obj.MarkObject(isOn);
        }
    }

    public List<GridBuildingController> GetSelected()
    {
        if (!IsOn)
            return new List<GridBuildingController>();
        return SelectedBuildings.ToList();
    }

    public void Reset()
    {
        foreach (var obj in SelectedBuildings)
            obj.MarkObject(false);

        SelectedBuildings.Clear();
    }
}