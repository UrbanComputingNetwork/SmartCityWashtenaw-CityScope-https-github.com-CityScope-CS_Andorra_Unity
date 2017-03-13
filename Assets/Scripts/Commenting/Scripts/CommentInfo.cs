using UnityEngine;

public class CommentInfo
{
    public int id;
    public LatLong location;
    public string text;

    public CommentInfo(int id, string text, LatLong location)
    {
        this.id = id;
        this.location = location;
        this.text = text;
    }
}
