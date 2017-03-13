using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using MiniJSON;

public class JSONTest : MonoBehaviour {

    public TCPRequestHelperJSON requestHelper;
    public GridBuildingSpawner grid;

    private List<string> commentLines = new List<string>()
        {
            "This area has too many towers!",
            "Should consider a park here. ",
            "Nice design, please add trees. ",
            "Like this street!",
            "Would probably go shaded afternoon!",
            "Prefered the older design with lower buildings. ",
            "Will this area be open to everybody? ",
            "How would you solve parking in this street.",
            "This is a new skyline for the city, great!",
            "Nice!",
            "Seems not that accessible.",
            "We need more consistency here.",
            "Exciting new court!",
            "Will there be any new theaters here?",
            "Please add lower-income housing in this area. ",
            "Interesting design.",
            "Too tall for our city!  ",
            "Will this area be car free?",
            "We should have more of these in our city!"
        };

    public bool pushComments = false;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	    if(pushComments)
        {
            pushComments = false;
            PushFakeComments();
        }
	}

    public void PushFakeComments()
    {
        StartCoroutine(PushFakeComments(7, 8, 6, 600));
        StartCoroutine(PushFakeComments(9, 14, 4, 300));
        StartCoroutine(PushFakeComments(1, 1, 3, 100));
    }

    public IEnumerator PushFakeComments(int cx, int cy, int radius, int num)
    {
        int count = 0;
        for (int r = 1; r <= radius; r++)
        {
            for (var i = 0; i < r*num/(radius*radius); i++)
            {
                var v2 = r*UnityEngine.Random.insideUnitCircle;
                var x = cx + Mathf.RoundToInt(v2.x);
                var y = cy + Mathf.RoundToInt(v2.y);
                if(x >= 0 && x < 16 && y >= 0 && y < 16)
                {
                    var text = commentLines[UnityEngine.Random.Range(0, commentLines.Count)];
                    var posv2 = grid.GridToVector(x, y);
                    var pos = new Vector3(posv2.x, 0, posv2.y);
                    var geoloc = GeoCoordinatesConverter.Instance.WorldToLatLong(pos);
                    requestHelper.AddCommentPostRequest(text, new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(x, y) }, geoloc);
                }

                if (++count > 10)
                {
                    yield return new WaitForSeconds(0.5f);
                    count = 0;
                }
            }
        }
    }
}
