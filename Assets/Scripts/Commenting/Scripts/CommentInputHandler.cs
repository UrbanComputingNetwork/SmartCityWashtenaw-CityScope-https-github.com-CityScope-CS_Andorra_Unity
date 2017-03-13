using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CommentInputHandler : MonoBehaviour {

    public CommentCollection commentManager;
    public RaySel paintingManager;

    public InputField commentInputField;
    
	void Awake(){
		commentInputField.gameObject.SetActive(false);
	}

    // Use this for initialization
    void Start()
    {
        commentInputField.onEndEdit.AddListener(SubmitComment);
    }

	// Update is called once per frame
	void Update ()
    {
	
	}

    public void ToggleInputField()
    {
        commentInputField.gameObject.SetActive(!commentInputField.gameObject.activeSelf);
    }

    public void SubmitComment(string text)
    {
		if (commentInputField.text == "")
		{
			return;
		}
			
		ToggleInputField(); 

        var result = paintingManager.GetSelected();
        Vector3 average = new Vector3(0, 0, 0);
        if (result.Count == 0)
        {
            average = new Vector3(0, 0, 0);
        }
        else
        {
            foreach (var obj in result)
                average += obj.transform.localPosition;
            average /= result.Count;
        }
        paintingManager.Reset();
        paintingManager.PaintModeOn = false;
        commentManager.AddComment(text, average, result);

		//clear 
		commentInputField.text = "";
	}

	public void SubmitComment()
	{
		SubmitComment (commentInputField.text);

	}
}
