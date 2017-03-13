using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEditor;

public class CommentUIHandler : MonoBehaviour {

    public string text;
    public Image image;

	// Use this for initialization
	void Start () {
        image = gameObject.GetComponentInChildren<Image>();
        var txt = gameObject.GetComponentInChildren<Text>();
        txt.text = text; // gets a random comment from the Collection
        Destroy(gameObject, 10.0f);
    }
	
	/*// Update is called once per frame
	void Update () {

       // var r = GUIRectWithObject(this.gameObject);
      //  var m = Mathf.Max(r.height / Screen.height, r.width / Screen.width);
        if (Camera.main != null)
        {
            var d = Vector3.Distance(Camera.main.transform.position, transform.position) / transform.lossyScale.y;
            var m1 = Mathf.InverseLerp(1, 100, d);
			var color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), m1 - 0.3f);
            image.color = color;
        }
	}

    public static Rect GUIRectWithObject(GameObject go)
    {
        var renderer = go.GetComponent<CanvasRenderer>();
        Vector3 cen = renderer.bounds.center;
        Vector3 ext = renderer.bounds.extents;
        Vector2[] extentPoints = new Vector2[8]
        {
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
         HandleUtility.WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
        };
        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];
        foreach (Vector2 v in extentPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }
        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    } */
}
