using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(GridBuildingSpawner))]
public class GridMeshAnimator : MonoBehaviour {

    public float BaseHeight = 0f;

    private GridBuildingSpawner grid;
    public Terrain terrain;

    public Transform Vertices;

    Vector3[] tri;

    public class Triangulation
    {

        public class Vertex
        {

            public Vector3 pos;
            public Vector3 normal;
            public Vector2 uv;
            public int type;


            public Vertex(Vector3 p, int t)
            {

                this.pos = p;
                this.type = t;

            }

            public Vertex(Vector3 p, int t, Vector3 n, Vector2 u)
            {

                this.pos = p;
                this.type = t;
                this.normal = n;
                this.uv = u;

            }

        }

        List<Vertex> vertices;
        public float lowerAngle;


        public Triangulation()
        {

            this.vertices = new List<Vertex>();
            this.lowerAngle = 3f;

        }

        public Triangulation(float lower_Angle)
        {

            this.vertices = new List<Vertex>();
            this.lowerAngle = lower_Angle;

        }

        public void Add(Vector3 p, int t)
        {

            this.vertices.Add(new Vertex(p, t));

        }

        public Vector3[] Calculate()
        {

            if (this.vertices.Count == 0) return new Vector3[1];

            Vector3[] result;
            int i;

            if (this.vertices.Count <= 3)
            {

                result = new Vector3[vertices.Count];
                for (i = 0; i < vertices.Count; i++) result[i] = vertices[i].pos;
                return result;

            }

            List<int[]> triangles = new List<int[]>();
            int[] triangle;
            int w, x;
            float circumsphereRadius;
            Vector3 a, b, c, ac, ab, abXac, toCircumsphereCenter, ccs;

            //All Combinations without repetition, some vertice of different type, only one vertice different of type 0
            for (i = 0; i < vertices.Count - 2; i++)
            {
                for (w = i + 1; w < vertices.Count - 1; w++)
                {
                    for (x = w + 1; x < vertices.Count; x++)
                    {

                        if (vertices[i].type == vertices[w].type && vertices[i].type == vertices[x].type) continue; // Same type
                        if (Vector3.Angle(vertices[w].pos - vertices[i].pos, vertices[x].pos - vertices[i].pos) < this.lowerAngle) continue; // Remove triangles with angle near to 180º
                        triangle = new int[3] { i, w, x };
                        triangles.Add(triangle);

                    }
                }
            }

            //Delaunay Condition
            for (i = triangles.Count - 1; i >= 0; i--)
            {

                //Points
                triangle = triangles[i];
                a = vertices[triangle[0]].pos;
                b = vertices[triangle[1]].pos;
                c = vertices[triangle[2]].pos;

                //Circumcenter 3Dpoints
                //http://gamedev.stackexchange.com/questions/60630/how-do-i-find-the-circumcenter-of-a-triangle-in-3d
                ac = c - a;
                ab = b - a;
                abXac = Vector3.Cross(ab, ac);
                // this is the vector from a TO the circumsphere center
                toCircumsphereCenter = (Vector3.Cross(abXac, ab) * ac.sqrMagnitude + Vector3.Cross(ac, abXac) * ab.sqrMagnitude) / (2f * abXac.sqrMagnitude);
                // The 3 space coords of the circumsphere center then:
                ccs = a + toCircumsphereCenter; // now this is the actual 3space location
                                                // The three vertices A, B, C of the triangle ABC are the same distance from the circumcenter ccs.
                circumsphereRadius = toCircumsphereCenter.magnitude;
                // As defined by the Delaunay condition, circumcircle is empty if it contains no other vertices besides the three that define.
                for (w = 0; w < vertices.Count; w++) if (w != triangle[0] && w != triangle[1] && w != triangle[2]) if (Vector3.Distance(vertices[w].pos, ccs) <= circumsphereRadius) break;
                // If it's not empty, remove.
                if (w != vertices.Count) triangles.RemoveAt(i);

            }

            result = new Vector3[triangles.Count * 3];
            for (i = 0; i < triangles.Count; i++)
            {

                triangle = triangles[i];
                result[i * 3] = vertices[triangle[0]].pos;
                result[i * 3 + 1] = vertices[triangle[1]].pos;
                result[i * 3 + 2] = vertices[triangle[2]].pos;

            }

            return result;

        }

    }

    // Use this for initialization
    void Start () {

        grid = GetComponent<GridBuildingSpawner>();
        int gridSize = 16;


        var data = terrain.terrainData;
        float scale = (float)data.heightmapResolution / gridSize;
        //Debug.LogFormat("SCALE = {0}", scale);
        var heights = new float[data.heightmapResolution,data.heightmapResolution];

        Triangulation meshTriangle = new Triangulation();

        var list2d = new List<Vector2>();
        var list3d = new List<Vector3>();
        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                var v2 = grid.GridToVector(j, i);
                list2d.Add(v2);
                
                var h = 2*scale*(BaseHeight + Random.value)/(1f+BaseHeight);
                Debug.LogFormat("HEIGHT = {0}", h);
                list3d.Add(new Vector3(v2.x, h, v2.y));
                heights[(int)(i * scale), (int)(j * scale)] = h;

            }
        }
        for(int i = 0; i < 2*scale; i++)
        {
            SmoothHeights(ref heights);
        }
        data.SetHeights(0, 0, heights);
        terrain.drawHeightmap = true;        
    }
	
    private void SmoothHeights(ref float[,] heights)
    {
        int x_res = heights.GetLength(0), y_res = heights.GetLength(1);
        for (int i = 0; i < x_res; i++)
        {
            for (int j = 0; j < y_res; j++)
            {
                var height = 0f;
                int count = 0;
                for(int y = i-1; y <= i+1; y++)
                {
                    for(int x = j-1; x <= j+1; x++)
                    {
                        if (y >= 0 && x >= 0 && x < x_res && y < y_res)
                        {
                            height += heights[x, y];
                            count++;
                        }
                    }
                }
                heights[i, j] = height / count;
            }
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
