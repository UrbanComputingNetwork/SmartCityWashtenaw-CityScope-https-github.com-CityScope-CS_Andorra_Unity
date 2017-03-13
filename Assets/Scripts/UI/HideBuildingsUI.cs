using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;

public class HideBuildingsUI : MonoBehaviour {

    public GridBuildingSpawner buildingSpawner;

    private class Cell
    {
        private bool shown;
        private Image image;
        private HideBuildingsUI parent;

        private int x;
        private int y;

        public bool isShown
        {
            get
            {
                return shown;
            }
            set
            {
                shown = value;
                image.color = value ? Color.green : Color.red;
            }
        }

        public Cell(HideBuildingsUI parent, int x, int y)
        {
            this.parent = parent;
            this.x = x;
            this.y = y;

            var obj = new GameObject();
            image = obj.AddComponent<Image>();

            var trigger = obj.AddComponent<EventTrigger>();
            EventTrigger.Entry onEnter = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerEnter,
            };
            onEnter.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
            trigger.triggers.Add(onEnter);

            EventTrigger.Entry onClick = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerClick,
            };
            onClick.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
            trigger.triggers.Add(onClick);

            obj.transform.SetParent(parent.transform, false);
        }

        private void OnPointerEnterDelegate(PointerEventData data)
        {
            if (data.pointerPress != null)
            {
                data.Use();
                isShown = !isShown;
                parent.buildingSpawner.RefreshCell(x, y);
            }
        }
    }
    


    private Cell[,] cellGrid;

    private float lastSaveTime; 

	// Use this for initialization
	void Start () {
        //LoadGrid();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (cellGrid != null && Time.time-lastSaveTime > 10)
        {
            lastSaveTime = Time.time;
            SaveGrid();
        }
	
	}

    public void LoadGrid(int width, int height)
    {
        cellGrid = new Cell[height, width];
        lastSaveTime = Time.time;

        for (var i = 0; i < cellGrid.GetLength(0); i++)
            for (var j = 0; j < cellGrid.GetLength(1); j++)
            {
                var cell = new Cell(this, j, i);
                cell.isShown = PlayerPrefs.GetInt(string.Format("showCell[{0},{1}]", i,j), 1) == 1;
                cellGrid[i, j] = cell;
            }

        //return cellGrid;
    }

    private void SaveGrid()
    {
        for (var i = 0; i < cellGrid.GetLength(0); i++)
            for (var j = 0; j < cellGrid.GetLength(1); j++)
            {
                PlayerPrefs.SetInt(string.Format("showCell[{0},{1}]", i, j), cellGrid[i, j].isShown ? 1 : 0);
            }

        PlayerPrefs.Save();
    }

    public bool getVisibility(int x, int y)
    {
        return cellGrid[y, x].isShown;
    }
}
