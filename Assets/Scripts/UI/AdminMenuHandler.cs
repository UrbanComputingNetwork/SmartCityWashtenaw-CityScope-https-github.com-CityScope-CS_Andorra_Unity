using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdminMenuHandler : MonoBehaviour {

    public TCPRequestHelperJSON RequestHelper;
    public Text statusText;
    public Text posText;
    public Text rotText;

    private int currentIndex = 0;
    public GameObject[] contextObject;
	public Console console;

    private bool active = false;
    //private RectTransform obj;
    private RectTransform rectTransform;

    
    //private delegate

    private bool isActive
    {
        get
        {
            return active;
        }
        set
        {
            active = value;
            if (value)
                OnActivate();
            else
                OnDeactivate();
        }
    }

	// Use this for initialization
	void Start () {
        //obj = this as RectTransform;
        rectTransform = GetComponent<RectTransform>();
        Application.logMessageReceived += Application_logMessageReceived;

		console.show = false; //set console GameObj false
	}

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void OnActivate()
    {
        statusText.text = "";
		console.show = true;
        rectTransform.anchoredPosition = new Vector2(-rectTransform.sizeDelta.x, 0);
        StartCoroutine(StatusUpdate());
    }

    public void OnDeactivate()
    {
        console.show = false;
        rectTransform.anchoredPosition = new Vector2(0, 0);
        StopAllCoroutines();
    }

    public void ButtonClicked()
    {
        isActive = !isActive;
    }

    private IEnumerator StatusUpdate()
    {
        while (true)
        {
            bool? status = null;
            RequestHelper.AddStatusRequest((s) => status = s);
            yield return new WaitWhile(() => !status.HasValue);
            statusText.text = status.Value ? "<color=green>Online</color>" : "<color=red>Offline</color>";
        }

    }



    private int selectionGridInt = 0;
    private int selectionOppId = -1;

    void OnGUI()
    {
        

        if (active)
        {
            if (GUI.Button(new Rect(640, 100, 60, 60),  contextObject[currentIndex].name))
            {
                currentIndex = (currentIndex + 1) % contextObject.Length;
            }
            string[] selectionStrings = { "x", "y", "z", "rx", "ry", "rz" };

            var contextTransform = contextObject[currentIndex].transform;

            // Action<float>[] setters = { (f) => contextTransform.localPosition.x = f, (f) => contextTransform.localPosition.y = f, (f) => contextTransform.localPosition.z = f, (f) => contextTransform.localRotation.x = f, (f) => contextTransform.localRotation.y = f, (f) => contextTransform.localRotation.z = f };
            // Func<float>[] getters = { () => contextTransform.localPosition.x, () => contextTransform.localPosition.y, () => contextTransform.localPosition.z, () => contextTransform.localRotation.x, () => contextTransform.localRotation.y, () => contextTransform.localRotation.z };

            selectionGridInt = GUI.SelectionGrid(new Rect(300, 100, 300, 180), selectionGridInt, selectionStrings, 3);
            //var g = getters[selectionGridInt];
            //var s = setters[selectionGridInt];

            var opperations = new List<float>{ -0.001f, 0.001f, -0.01f, 0.01f, -0.1f, 0.1f, -1f, 1f, -10f, 10f };
            var oppStrings = opperations.Select(o => o.ToString()).ToArray();
            selectionOppId = GUI.SelectionGrid(new Rect(800, 100, 250, 320), -1, oppStrings, 2);
            if(selectionOppId != -1)
            {
                if(selectionGridInt >= 3)
                {
                    var v = contextTransform.localEulerAngles;
                    v[selectionGridInt % 3] += opperations[selectionOppId] * 10;
                    contextTransform.localEulerAngles = v;
                }
                else
                {

                    var v = contextTransform.localPosition;
                    v[selectionGridInt] += opperations[selectionOppId];
                    contextTransform.localPosition = v;
                }

                var comp = contextObject[currentIndex].GetComponent<HolderOffsetPersistanceScript>();
                if(comp != null)
                {
                    comp.SaveCurrent();
                }
                
            }
            posText.text = contextTransform.localPosition.ToString("F3");
            rotText.text = contextTransform.localEulerAngles.ToString("F3");

        }
    }

}
