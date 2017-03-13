using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommentCollection : MonoBehaviour {

    public GameObject commentPrefab;
    public TCPRequestHelperJSON NetworkManager;
    public GameObject commentHolder;

	void Awake(){
        transform.position = new Vector3 (0,0,0);
	}

	void Start(){
        StartCoroutine(InitComments());	
	}

    private IEnumerator InitComments()
    {
        yield return new WaitForSeconds(5);
        StartCoroutine(HandleComment(0));
        yield return new WaitForSeconds(3);
        StartCoroutine(HandleComment(10));
        yield return new WaitForSeconds(1);
        StartCoroutine(HandleComment(20));
    }

    private IEnumerator HandleComment(int minCount)
    {
        while(true)
        {
            //var comment = GetRandomComment();
            CommentInfo comment = null;
            int commentsTotal = 0;
            NetworkManager.AddGetRandomCommentRequest((com, total) => {
                comment = com;
                commentsTotal = total;
            });
            yield return new WaitWhile(() => comment == null);
            if (commentsTotal >= minCount)
            {
                var pos2d = GeoCoordinatesConverter.Instance.LatLongToWorld(comment.location);
                var position = new Vector3(pos2d.x, 0, pos2d.y);
                var obj = Instantiate(commentPrefab, position, Quaternion.identity) as GameObject;
                obj.transform.SetParent(commentHolder.transform, true);
                var c = obj.GetComponentInChildren<CommentUIHandler>(); //get the text from the child
                c.text = comment.text;
            }
            yield return new WaitForSeconds(12);
        }
    }

    public void AddComment(string text, Vector3 pos, List<GridBuildingController> buildings){
        //comments.Add (new CommentInfo(text, pos));
        var idList = buildings.ConvertAll(b => new KeyValuePair<int, int>(b.buildingInfo.x, b.buildingInfo.y));
        var geoloc = GeoCoordinatesConverter.Instance.WorldToLatLong(pos);
        NetworkManager.AddCommentPostRequest(text, idList, geoloc);
	}

}
