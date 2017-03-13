using UnityEngine;
using System.Collections;
using MiniJSON;
using System.Collections.Generic;
using System.Linq;

public class UrbanProcessingAPI : MonoBehaviour {

    public static float baseHeight = 40;

    public double lastRatio;

    private class Question
    {
        private static Color transparency = new Color(1f, 1f, 1f, 0.8f);
        private static Color emptyColor = Color.grey * transparency;
        private static Color noColor = Color.red * transparency;
        private static Color intermediateColor = Color.yellow * transparency;
        private static Color yesColor = Color.green * transparency;

        public string id;
        public string text;
        public List<LatLong> polygon;

        public List<Vector3> prismQuads;
        public List<Vector3> prismTriangleStripes;

        public int yesCount;
        public int noCount;

        public Color color;

        public static Question FromJsonDictionary(Dictionary<string, object> dict)
        {
            var q = new Question();
            q.text = (string)dict["text"];
            q.id = (string)dict["id"];

            //Debug.Log(q.text);

            q.polygon = new List<LatLong>();
            var locs = dict["location"] as List<object>;
            foreach (var l_dict in locs)
            {
                var l = l_dict as Dictionary<string, object>;
                var latitude = (double)(l["lat"]);
                var longitude = (double)(l["long"]);
                q.polygon.Add(new LatLong(latitude, longitude));
                // Debug.LogFormat("Lat {0} Long {1}", latitude, longitude);
            }

            var yesno = dict["results"] as List<object>;
            q.yesCount = (int)(long)yesno[0];
            q.noCount = (int)(long)yesno[1];

            if (q.yesCount + q.noCount == 0)
            {
                q.color = emptyColor;
            }
            else
            {
                var t = (float)q.yesCount / (q.noCount + q.yesCount);
                q.color = t < 0.5 ? Color.Lerp(noColor, intermediateColor, t*2) : Color.Lerp(intermediateColor, yesColor, (t-0.5f)*2);
                // Debug.LogFormat("{0} v. {1} = {2}", q.yesCount, q.noCount, t);
            }

            q.BuildPrism();

            return q;
        }

        public void BuildPrism()
        {
            prismQuads = new List<Vector3>();

            //Calculate height of current prism
            var height = baseHeight + (yesCount + noCount) * 10;

            var converter = GeoCoordinatesConverter.Instance;

            //Convert all geopoints to in-game locations
            var points2d = polygon.ConvertAll(p => converter.LatLongToWorld(p)).ToArray();

            // Create numbering for vertices for debug purposes.
            /*int idx = 0;
            foreach (var p in points2d)
            {
                Debug.LogFormat("({0}, {1})", p.x, p.y);
                
                var obj = new GameObject("obj", typeof(TextMesh));
                var tm = obj.GetComponent<TextMesh>();
                tm.text = (idx++).ToString();
                tm.characterSize = 3;
                tm.fontSize = 40;
                tm.color = Color.red;
                tm.alignment = TextAlignment.Center;
                tm.anchor = TextAnchor.LowerCenter;
                obj.transform.position = new Vector3(p.x, height+2, p.y);
            }*/

            var p0 = points2d[0] ;
            for (int i = 0; i < points2d.Length-1; i++)
            {
                var i_next = (i + 1) % points2d.Length;
                var p1 = points2d[i];
                var p2 = points2d[i_next];

                //TODO: Convert LatLong to In-Game coords
                prismQuads.Add(new Vector3(p1.x, 0, p1.y));
                prismQuads.Add(new Vector3(p2.x, 0, p2.y));
                prismQuads.Add(new Vector3(p2.x, height, p2.y));

                //prismQuads.Add(new Vector3(p1.x, 0, p1.y));
                //prismQuads.Add(new Vector3(p2.x, height, p2.y));
                prismQuads.Add(new Vector3(p1.x, height, p1.y));

                /*if(i != 0 && i_next != 0)
                {
                    prism.Add(new Vector3(p0.x, height, p0.y));
                    prism.Add(new Vector3(p1.x, height, p1.y));
                    prism.Add(new Vector3(p2.x, height, p2.y));
                }*/
            }

            prismTriangleStripes = new List<Vector3>();
            //foreach (var i in indices)
            //{

            prismTriangleStripes.Add(new Vector3(p0.x, height, p0.y));
            var i1 = 1;
            var i2 = points2d.Length - 1;
            bool b = true;
            while (i1 != i2)
            {
                Vector2 p;
                if (b)
                {
                    p = points2d[i1++];
                }
                else
                {
                    p = points2d[i2--];
                }
                //b = !b;
                prismTriangleStripes.Add(new Vector3(p.x, height, p.y));

            }
            var pl = points2d[i1];
            prismTriangleStripes.Add(new Vector3(pl.x, height, pl.y));
            //prismTriangleStripes.Add(new Vector3(p0.x, height, p0.y));
            //}
        }


    }

    public string URL = "http://urbanprocessing.yasushisakai.com/response-json/";
    private List<Question> questions;


	// Use this for initialization
	IEnumerator Start () {
        questions = new List<Question>();

        lastRatio = GeoCoordinatesConverter.Instance.ratioX / GeoCoordinatesConverter.Instance.ratioY;

        WWW www = new WWW(URL);
        yield return www;
        var questions_json = www.text;
        var dict = Json.Deserialize(questions_json) as Dictionary<string, object>;
        var questions_dict = dict["questions"] as List<object>;
        foreach(var q_d in questions_dict)
        {
            var q_dict = q_d as Dictionary<string, object>;
            var q = Question.FromJsonDictionary(q_dict);
            questions.Add(q);
        }
        //Debug.LogFormat("Loaded {0} questions", questions.Count);

        //Test the transformation accuaracy
        var la = 42.367144;
        var lo = -71.077463;
        var ll = new LatLong(la, lo);
        var res = GeoCoordinatesConverter.Instance.LatLongToWorld(ll);
        var res2 = GeoCoordinatesConverter.Instance.WorldToLatLong(res);
        Debug.LogFormat("{0} -> {1} -> {2}", ll, res.ToString("F3"), res2);

    }
	
	// Update is called once per frame
	void Update () {

        /*var r = GeoCoordinatesConverter.Instance.ratioX / GeoCoordinatesConverter.Instance.ratioY;
        if(r != lastRatio)
        {
            foreach(var q in questions)
            {
                q.BuildPrism();
            }
            lastRatio = r;
        }*/
    }

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (questions == null)
            return;

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        foreach (var q in questions)
        {

            GL.Begin(GL.QUADS);
            GL.Color(q.color);
            foreach (var v in q.prismQuads)
                GL.Vertex(v);
            GL.End();
            /*GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(q.color * new Color(0.5f, 0.5f, 0.5f, 1f));
            foreach (var v in q.prismTriangleStripes)
                GL.Vertex(v);
            GL.End();*/
            GL.Begin(GL.LINES);
            GL.Color(new Color(1f, 0.5f, 0.5f, 1f));
            for(var i = 0; i < q.prismTriangleStripes.Count-1; i++)
            {
                GL.Vertex(q.prismTriangleStripes[i]);
                GL.Vertex(q.prismTriangleStripes[i + 1]);
            }
            GL.End();
        }
        //questions = null;
        GL.PopMatrix();
    }
}
