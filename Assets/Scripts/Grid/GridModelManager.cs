#if UNITY_EDITOR
// Editor specific code here
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class GridModelManager : MonoBehaviour {
    
    [Serializable]
    public struct Threshhold
    {
        public int height;
        public GameObject gameObject;
    }

    [Serializable]
    public class BuildingInfo
    {
        public int type;
        public List<Threshhold> info;

        public BuildingInfo()
        {
            type = 0;
            info = new List<Threshhold>();
            info.Add(new Threshhold());
        }
    }


    private Dictionary<int, List<Threshhold>> dict;
    
    public List<BuildingInfo> Structures;

    public List<int> Population;
    public List<int> Density;

    public HideBuildingsUI buildingMapUI;

    private List<int> buildingDensity;

	// Use this for initialization
	void Start ()
    {
        dict = new Dictionary<int, List<Threshhold>>();
        foreach(var bi in Structures)
        {
            dict[bi.type] = bi.info.OrderByDescending(o => o.height).ToList();
        }

        buildingMapUI.LoadGrid(16, 16);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Func<GridInfo.CellData, bool> UpdateData(GridInfo newGrid)
    {
        var changedDensitiesSet = new HashSet<int>();
        if(newGrid.BuildingDensity != null)
        {
            if (buildingDensity != null)
            {
                for (int i = 0; i < buildingDensity.Count && i < newGrid.BuildingDensity.Count; i++)
                {
                    var density1 = TypeHeightToModel(i, buildingDensity[i]);
                    var density2 = TypeHeightToModel(i, newGrid.BuildingDensity[i]);
                    if (density1 != density2)
                        changedDensitiesSet.Add(i);
                }
            }
            buildingDensity = newGrid.BuildingDensity;
        }

        return (b) => changedDensitiesSet.Contains(b.type);
    }

    public GameObject GetModel(GridInfo.CellData building)
    {
        var type = building.type;// != -1 ? building.type : 0;

        var height = type >= 0 && type < buildingDensity.Count ? buildingDensity[type] : 0;

        if (!buildingMapUI.getVisibility(building.x, building.y))
            return null;

        var obj = TypeHeightToModel(type, height);
        if(obj == null)
        {
            Debug.LogFormat("Type {0} was not found.", type);
            //return dict.FirstOrDefault().Value[0].gameObject;
        }

        //if (building.type == -1)
        //    return null;
        return obj;
    }

    private GameObject TypeHeightToModel(int type, int height)
    {
        if (!dict.ContainsKey(type))
            return null;

        return dict[type].FirstOrDefault(o => height >= o.height).gameObject;
    }
}
