using UnityEngine;
using System.Collections;

public class HolderOffsetPersistanceScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //var name = gameObject.name;
	    if(PlayerPrefs.HasKey(name + "pos") && PlayerPrefs.HasKey(name + "rot"))
        {
            float x, y, z, rx, ry, rz;
            x = PlayerPrefs.GetFloat(name + "posX");
            y = PlayerPrefs.GetFloat(name + "posY");
            z = PlayerPrefs.GetFloat(name + "posZ");
            rx = PlayerPrefs.GetFloat(name + "rotX");
            ry = PlayerPrefs.GetFloat(name + "rotY");
            rz = PlayerPrefs.GetFloat(name + "rotZ");
            transform.localPosition = new Vector3(x, y, z);
            transform.localRotation = Quaternion.Euler(rx, ry, rz);
        }
        else
        {
            SaveCurrent();
        }
	}
	
    public void SaveCurrent()
    {
        PlayerPrefs.SetInt(name + "pos", 1);
        PlayerPrefs.SetInt(name + "rot", 1);
        var pos = transform.localPosition;
        var rot = transform.localRotation.eulerAngles;
        PlayerPrefs.SetFloat(name + "posX", pos.x);
        PlayerPrefs.SetFloat(name + "posY", pos.y);
        PlayerPrefs.SetFloat(name + "posZ", pos.z);
        PlayerPrefs.SetFloat(name + "rotX", rot.x);
        PlayerPrefs.SetFloat(name + "rotY", rot.y);
        PlayerPrefs.SetFloat(name + "rotZ", rot.z);
        PlayerPrefs.Save();
    }

	// Update is called once per frame
	void Update () {
	
	}
}
