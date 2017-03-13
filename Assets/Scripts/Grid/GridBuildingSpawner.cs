using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class GridBuildingSpawner : MonoBehaviour {

    public class MapInfo
    {
        public GameObject gameObject = null;
        public bool isFantom = false;
        public GridBuildingController controller;

        public int NumberOfChanges;

        public bool isEmpty
        {
            get
            {
                return gameObject == null;
            }
        }

        public MapInfo(GameObject gameObject = null, GridBuildingController component = null, bool isFantom = false)
        {
            this.NumberOfChanges = 0;
            this.gameObject = gameObject;
            this.controller = component;
            this.isFantom = isFantom;
        }

        public void Update(GameObject gameObject, GridBuildingController component = null)
        {
            this.gameObject = gameObject;
            this.controller = component; 
        }


    }

    public GridModelManager modelManager;
    public TCPRequestHelperJSON RequestHelper;
    public GameObject GridContainer;
    public float gridSpacing = 0.01f;
    public float blockSize = 35f;

    public int width = 16;
    public int height = 16;


    public GridInfo gridInfo;
    public MapInfo[,] mapInfo;
    public Material creationMaterial;
    public Material destroyingMaterial;
    public Material whiteMaterial;

    private bool texturesAllowed = true;

    [NonSerialized]
    public bool UpdateOn = true;

    public void RefreshCell(int x, int y)
    {
        CompareBuildings(gridInfo, (b) => b.x == x && b.y == y);
    }

    // Use this for initialization
    void Start () {
        mapInfo = new MapInfo[height, width];
        gridInfo = new GridInfo(width, height);
        StartCoroutine(ManageUpdates());
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
    }

    private IEnumerator ManageUpdates()
    {
        yield return new WaitWhile(() => RequestHelper == null);
        while (true)
        {
            GridInfo grid = null;
            RequestHelper.AddUpdateRequest(width, height, (g) => grid = g);
            yield return new WaitWhile(() => grid == null);
            UpdateGrid(grid);
        }
    }

    private void UpdateGrid(GridInfo grid)
    {
        var forceChange = modelManager.UpdateData(grid) ?? ((b) => { return false; });
        CompareBuildings(grid, forceChange);
    }

    private void CompareBuildings(GridInfo newGrid, Func<GridInfo.CellData, bool> forceChangeCheck)
    {
        var gridData = newGrid.gridData;
        foreach(var kv in gridData)
        {
            GridInfo.CellData old_value = gridInfo.gridData[kv.y, kv.x];
            if (kv.type != -2 && (!old_value.Equals(kv) || forceChangeCheck(kv)))
            {
                OnBuildingPotentialChanged(old_value, kv);
            }
            gridInfo.UpdateCell(kv);
        }
        if(newGrid.BuildingDensity != null)
            gridInfo.BuildingDensity = newGrid.BuildingDensity;
        //gridInfo = newGrid;
    }

    private void OnBuildingAdded(GridInfo.CellData newBuilding, string viaPotential = "true")
    {
        //if (newBuilding.type == -1)
        //    return;
        var building = GetBuildingInfo(newBuilding);

        //Force to stop animation
        if (building.gameObject != null)
            DestroyBuilding(newBuilding);

        Debug.Assert(building.gameObject == null);
        //if(newBuilding.type != -1)
        SpawnBuilding(newBuilding);
    }

    private void OnBuildingRemoved(GridInfo.CellData oldBuilding, string viaPotential = "true")
    {
        var building = GetBuildingInfo(oldBuilding);
        Debug.Assert(building != null);
        if (building.gameObject == null)
            print(oldBuilding + " d " + viaPotential);
        Debug.Assert(building.gameObject != null);

        DestroyBuilding(oldBuilding);
    }

    private void OnBuildingChanged(GridInfo.CellData oldBuilding, GridInfo.CellData newBuilding)
    {
        Debug.Assert(oldBuilding.x == newBuilding.x && oldBuilding.y == newBuilding.y);
        //Debug.Assert(oldBuilding.type != -1 && newBuilding.type != -1);
        
       
        var oBuilding = GetBuildingInfo(oldBuilding);
        var nBuilding = GetBuildingInfo(newBuilding);
        Debug.Assert(oBuilding.gameObject != null);
        // Debug.Log("RECREATING");
        DestroyBuilding(oldBuilding);
        SpawnBuilding(newBuilding);
    }

    private void OnBuildingPotentialChanged(GridInfo.CellData oldBuilding, GridInfo.CellData newBuilding)
    {
     
        if (GetBuildingInfo(oldBuilding).isEmpty)
            OnBuildingAdded(newBuilding);
        else
            OnBuildingChanged(oldBuilding, newBuilding);
    }

    private void SpawnBuilding(GridInfo.CellData building)
    {
        

        //Ensure there is no animation game object left
        var oldBuilding = GetBuildingInfo(building);
        if (!oldBuilding.isEmpty)
            Destroy(oldBuilding.gameObject);

        oldBuilding.NumberOfChanges++;
        //If no model was found, leave an empty space
        var model = modelManager.GetModel(building);
        if (model == null)
        {
            Debug.LogFormat("MODEL NOT FOUND AT {0}, {1}", building.x, building.y);
            mapInfo[building.y, building.x].gameObject = null;
            return;
        }

        //Creating the game object
        var obj = Instantiate(model);
        obj.transform.SetParent(GridContainer.transform, false);
        obj.transform.localPosition = CalculateLocation(building);
        obj.transform.localRotation = CalculateRotation(building);
        

        //Adding the controller
        var controller = obj.AddComponent<GridBuildingController>();
        controller.buildingInfo = building;
        controller.creationMaterial = creationMaterial;
        controller.destroyingMaterial = destroyingMaterial;
        controller.whiteMaterial = whiteMaterial;
        controller.AllowTextures(texturesAllowed);
        controller.StartCreationAnimation();

        mapInfo[building.y, building.x].Update(obj, controller);

    }


    private void DestroyBuilding(GridInfo.CellData building)
    {
        var info = GetBuildingInfo(building);
        var obj = info.gameObject;
        Action completeDestroying = () =>
        {
            //Debug.Log(mapInfo[building.id].controller != null);
            DestroyObject(obj);
        };

        info.controller.StartDestroyingAnimation(completeDestroying);
    }

   

    private Vector3 CalculateLocation(GridInfo.CellData building)
    {
        int x = building.x, y = building.y;
        var v2d = GridToVector(x, y);
        return new Vector3(v2d.x, 0, v2d.y);
    }

    private Quaternion CalculateRotation(GridInfo.CellData building)
    {
        return Quaternion.Euler(0, building.rotation, 0);
    }
    
    private MapInfo GetBuildingInfo(GridInfo.CellData building)
    {
        return GetBuildingInfo(building.x, building.y);
    }

    public MapInfo GetBuildingInfo(int x, int y)
    {
        if (mapInfo[y, x] == null)
            mapInfo[y, x] = new MapInfo(null);
        return mapInfo[y, x];
    }

    public void AllowTextures(bool value)
    {
        Debug.LogFormat("AllowTexture({0})", value);
        texturesAllowed = value;
        foreach(var kv in mapInfo)
        {
            var controller = kv.controller;
            controller.AllowTextures(value);
        }
    }

    public Vector2 GridToVector(int x, int y)
    {
        var offset_x = (width - 1) * (blockSize + gridSpacing) * 0.5f;
        var offset_y = (height - 1) * (blockSize + gridSpacing) * 0.5f;
        return new Vector2(x * (blockSize + gridSpacing) - offset_x, -(y * (blockSize + gridSpacing) - offset_y));
    }

}
