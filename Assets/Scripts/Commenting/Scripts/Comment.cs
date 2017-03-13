using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Comment : MonoBehaviour {

    public string text;
    public Vector3 pos;

	private TextMesh txt;
	//private Image img;
	private float born_time;

	// Use this for initialization

	void Start () {

		//uicam = GameObject.Find ("UICamera");
		//arcam = GameObject.Find ("Camera"); // want to change name to be consistant....

		txt = gameObject.GetComponentInChildren<TextMesh> ();
		txt.text = text; // gets a random comment from the Collection
        
		//world_transform = CommentCollection.GetRandomPosition(); // gets a random position from the Collection

		born_time = Time.time;
		//img = gameObject.GetComponent<Image> ();

		//Destroy(gameObject, 10.0f);
        Destroy(gameObject, 10.0f);
    }

	void Update () {


		// switch to the right camera, want this event driven??
		//if (CameraSwap.isUIMode ()) 

		var cam = Camera.main;

		// gets the screen coordinate of the 3d 'comment_object' 


		float age = Time.time - born_time;
		Color c; 

		// float??
		transform.localPosition += (Vector3.up*(Mathf.Sin(age*2.0f))*0.5f);
        //transform.LookAt(cam.transform);
        var rot = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
        //Debug.LogFormat("Calculated rotation: {0}", rot.eulerAngles.ToString());
        transform.rotation = rot;

        // start to die
        if (age >10f)
        {
			c = txt.color;
			c.a = c.a*(1-(age - 10f));
			//txt.color = c;
			//img.color = c;

			//c = img.color;
			//c.a = txt.color.a*0.5f;
			//img.color = c;

		}
			
	}
}
