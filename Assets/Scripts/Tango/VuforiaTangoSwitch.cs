using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class VuforiaTangoSwitch : MonoBehaviour {

    public ARType ARMode;

    /// </summary>
    public enum ARType
    {
        NONE, 
        VUFORIA,
        TANGO
    }

    public List<GameObject> vuforiaComponents;
    public List<GameObject> tangoComponents;

	// Use this for initialization
	void Awake () {
        bool vuforia = ARMode == ARType.VUFORIA;
        vuforiaComponents.ForEach(c => ActivateOrDestroy(c, ARMode == ARType.VUFORIA));
        tangoComponents.ForEach(c => ActivateOrDestroy(c, ARMode == ARType.TANGO));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void ActivateOrDestroy(GameObject obj, bool value)
    {
        if (value)
            obj.SetActive(true);
        else
            Destroy(obj);
    }
}
