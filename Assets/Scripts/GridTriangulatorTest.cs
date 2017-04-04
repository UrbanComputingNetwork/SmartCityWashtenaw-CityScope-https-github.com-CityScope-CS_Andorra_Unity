using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class GridTriangulatorTest : MonoBehaviour {

    public class GraphObject
    {
        private GridTriangulatorTest controller;
        public GameObject gameObject;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private Vector3[,] vertices;
        private GridBuildingSpawner grid;
        private BoxCollider collider;

        private Func<int, int, float> dataSource;

        public GraphObject(GridTriangulatorTest controller, string label, Func<int, int, float> dataSource)
        {
            this.controller = controller;
            this.dataSource = dataSource;
            this.grid = controller.grid;

            vertices = new Vector3[grid.height, grid.width];
            for (int i = 0; i < grid.width; i++)
            {
                for (int j = 0; j < grid.height; j++)
                {
                    var v2 = grid.GridToVector(j, i);

                    var v3 = new Vector3(v2.x, 0, v2.y);
                    vertices[j, i] = v3;
                }
            }
            gameObject = Instantiate(controller.meshPrefab);
            gameObject.name = label;
            TriangulatorDelaunay.CreateInfluencePolygon(vertices.Cast<Vector3>().ToArray(), gameObject);
            gameObject.transform.SetParent(controller.meshContainer.transform, false);
            //meshObject.SetActive(true);

            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            meshRenderer.material = controller.meshMaterial;

            var textMesh = gameObject.GetComponentInChildren<TextMesh>() as TextMesh;
            textMesh.text = label;

            collider = gameObject.GetComponent<BoxCollider>();
            var dim = grid.GridToVector(grid.width - 1, grid.height - 1) - grid.GridToVector(0, 0);
            collider.size = new Vector3(dim.x, controller.maxMagnitude, dim.y);
            //collider.center = new Vector3(dim.x / 2, 0, dim.y / 2);
        }

        void UpdateMesh()
        {
            var mesh = meshFilter.mesh;
            mesh.vertices = vertices.Cast<Vector3>().ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        void UpdateHeight(int x, int y, float height)
        {
            var v = vertices[y, x];
            v.y = height;
            vertices[y, x] = v;
        }

        void NormalizeHeights()
        {
            var maxHeight = vertices.Cast<Vector3>().Max(v => v.y);
            var minHeight = vertices.Cast<Vector3>().Min(v => v.y);
            //Check if there is no data, so no need to normalize
            var amplitude = maxHeight - minHeight;
            if (amplitude == 0)
                amplitude = 1;

            for (int i = 0; i < grid.width; i++)
            {
                for (int j = 0; j < grid.height; j++)
                {
                    UpdateHeight(i, j, controller.maxMagnitude * (vertices[j, i].y-minHeight) / (amplitude));
                }
            }
        }

        public void setDataSource(Func<int, int, float> source)
        {
            dataSource = source;
            gameObject.SetActive(true);
            UpdateGrid();
        }

        public void UpdateGrid()
        {

            for (int i = 0; i < grid.width; i++)
            {
                for (int j = 0; j < grid.height; j++)
                {
                    var data = grid.GetBuildingInfo(i, j);

                    //var h = data != null && data.controller != null ? data.controller.buildingInfo.heat : 0;
                    var h = dataSource(i, j);
                    UpdateHeight(i, j, h);
                }
            }
            NormalizeHeights();
            UpdateMesh();
        }
    }


    public GameObject meshPrefab;

    private GridBuildingSpawner grid;
    private List<GraphObject> graphs;

    public GameObject meshContainer;
    public Material meshMaterial;

    public float maxMagnitude = 100f;

    // Use this for initialization
    void Start() {
        grid = GetComponent<GridBuildingSpawner>();
        graphs = new List<GraphObject>();
        //AddGraph((x, y) => { return 1f; });
        var g1 = AddGraph("Comment heatmap", (x, y) =>
        {
            var data = grid.GetBuildingInfo(x, y);
            return data != null && data.controller != null ? data.controller.buildingInfo.heat : 0;
        });
        var g2 = AddGraph("Distribution of changes", (x, y) =>
        {
            var data = grid.GetBuildingInfo(x, y);
            return data.NumberOfChanges;
        });
        var g3 = AddGraph("Density", (x, y) =>
        {
            var data = grid.GetBuildingInfo(x, y);

            if(data == null || data.controller == null)
                return 0;
            var b = data.controller.buildingInfo;

            var height = b.type >= 0 && b.type < grid.gridInfo.BuildingDensity.Count ? grid.gridInfo.BuildingDensity[b.type] : 0;
            return height;
        });
        g1.gameObject.transform.localPosition = new Vector3(0, 0, 0);
        g2.gameObject.transform.localPosition = new Vector3(0, 250, 0);
        g3.gameObject.transform.localPosition = new Vector3(0, 500, 0);

    }

    // Update is called once per frame
    void Update () {
        if(meshContainer.activeSelf && Time.frameCount % 30 == 0)
        {
            foreach(var graph in graphs)
                graph.UpdateGrid();
        }
	}

    public GraphObject AddGraph(string label, Func<int, int, float> dataSource)
    {
        var graph = new GraphObject(this, label, dataSource);
        //graph.gameObject.transform.name = "graph " + graphs.Count.ToString();
        graphs.Add(graph);
        return graph;
    }

    

}
