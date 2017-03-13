using System;
using UnityEngine;

[Serializable]
public class LatLong
{
    public double latitude;
    public double longitude;

    public LatLong(double lat, double @long)
    {
        latitude = lat;
        longitude = @long;
    }

    public override string ToString()
    {
        return string.Format("{0},{1}", latitude, longitude);
    }
}