using UnityEngine;
using System.Collections;
using System;

public class GeoCoordinatesConverter : Singleton<GeoCoordinatesConverter> {


    public GeoPoint ReferenceObject1;
    public GeoPoint ReferenceObject2;

    public double ratioX;
    public double ratioY;

    public double factor;

    //public double a, b, c, d;
    public Vector4 abcd;

    private Vector3 lastPos1;
    private Vector3 lastPos2;

    public static double TILE_SIZE { get { return 256; } }

    // Use this for initialization
    void Awake ()
    {

        lastPos1 = ReferenceObject1.transform.position;
        lastPos2 = ReferenceObject2.transform.position;
        UpdateRatio(lastPos1, lastPos2);
    }

    private void UpdateRatio(Vector3 pos1, Vector3 pos2)
    {
        //ratioX = (pos2.x - pos1.x) / (ReferenceObject2.Longitude - ReferenceObject1.Longitude);
        //ratioY = (pos2.z - pos1.z) / (ReferenceObject2.Latitude - ReferenceObject1.Latitude);

        var p1 = latLngWorld(ReferenceObject1.latLong);
        var p2 = latLngWorld(ReferenceObject2.latLong);

        double latdiff = p2.x - p1.x;
        double longdiff = p2.y - p1.y;

        //factor = Math.log;
        factor = 1;
        while (latdiff * factor < 1 && longdiff * factor < 1)
            factor *= 10;


        float x1 = 0f;
        float y1 = 0f;
        float x2 = (float)(longdiff * factor);
        float y2 = (float)(factor*latdiff);

        var M = new Matrix4x4();
        M.SetColumn(0, new Vector4(x1, y1, x2, y2));
        M.SetColumn(1, new Vector4(y1, -x1, y2, -x2));
        M.SetColumn(2, new Vector4(1, 0, 1, 0));
        M.SetColumn(3, new Vector4(0, 1, 0, 1));
        var MI = M.inverse;
        abcd = MI * new Vector4(pos1.x, pos1.z, pos2.x, pos2.z);

    }

    // Update is called once per frame
    void Update () {
        var pos1 = ReferenceObject1.transform.position;
        var pos2 = ReferenceObject2.transform.position;
        if (pos1 != lastPos1 || pos2 != lastPos2)
        {
            lastPos1 = ReferenceObject1.transform.position;
            lastPos2 = ReferenceObject2.transform.position;
            UpdateRatio(lastPos1, lastPos2);
        }
	}

    public Vector2 LatLongToWorld(LatLong point)
    {
        //var pos1 = ReferenceObject1.transform.position;
        //var pos2 = ReferenceObject2.transform.position;
        //var x = (point.longitude-ReferenceObject1.Longitude) * ratioX + pos1.x;
        //var y = (point.latitude - ReferenceObject1.Latitude) * ratioY + pos1.z;

        var p1 = latLngWorld(ReferenceObject1.latLong);
        var p = latLngWorld(point);

        var lo = (p.y - p1.y) * factor;
        var la = (p.x - p1.x) * factor;

        var x = +abcd[0] * lo + abcd[1] * la + abcd[2];
        var y = -abcd[1] * lo + abcd[0] * la + abcd[3];

        //Debug.LogFormat("{0} {1}", x, y);
        //Debug.LogFormat("{0} {1}", x * factor, y * factor);
        var v = new Vector2((float)x, (float)y);
        // LL -> RW -> RW_Normalized -> World
        //Debug.LogFormat("LL->W: {0} -> {1} -> {2} -> {3}", point, p, new Vector2d(la, lo), v);

        return v;
    }

    public LatLong WorldToLatLong(Vector2 point)
    {
        var a = (double)abcd[0];
        var b = (double)abcd[1];
        var c = (double)abcd[2];
        var d = (double)abcd[3];

        var tmp_x = (b * point.x + a * point.y - b * c - a * d) / (a*a+b*b);
        var tmp_y = (a * point.x - b * point.y + b * d - a * c) / (a*a+b*b);
        

        var p_0 = latLngWorld(ReferenceObject1.latLong);

        var x = tmp_x / factor + p_0.x;
        var y = tmp_y / factor + p_0.y;

        var ll = worldLatLng(new Vector2d(x, y));
        //World -> RW_N -> RW -> LL
        //Debug.LogFormat("W->LL: {0} -> {1} -> {2} -> {3}", point, new Vector2d(tmp_x, tmp_y), new Vector2d(x, y), ll);
        return ll;
    }

    public LatLong WorldToLatLong(Vector3 point)
    {
        return WorldToLatLong(new Vector2(point.x, point.z));
    }

    private static Vector2d latLngWorld(LatLong latLng)
    {

        double siny = Math.Sin(latLng.latitude * Math.PI / 180.0);

        // clip clip! mercator is not gonna work for high latitudes
        siny = Math.Min(Math.Max(siny, -0.99999999), 0.99999999);

        Vector2d worldCoordinate = new Vector2d();
        worldCoordinate.x = TILE_SIZE * (0.5 + latLng.longitude / 360);
        worldCoordinate.y = TILE_SIZE * (0.5 - Math.Log((1 + siny) / (1 - siny)) / (4 * Math.PI));

        return worldCoordinate;
    }

    private static LatLong worldLatLng(Vector2d worldCoordinate)
    {
        double rady = Math.Exp((worldCoordinate.y / TILE_SIZE - 0.5) * -(4.0 * Math.PI));

        LatLong latLng = new LatLong(
                            Math.Asin((rady - 1) / (rady + 1)) * 180 / Math.PI,
                           (worldCoordinate.x / TILE_SIZE - 0.5) * 360
                           );

        return latLng;
    }

    //
    //
    //


    public struct Vector2d
    {
        public double x;
        public double y;

        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2((float)x, (float)y);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }

        public static Vector2d one = new Vector2d(1.0, 1.0);

        public static Vector2d operator +(Vector2d one, Vector2d two)
        {
            return new Vector2d(one.x + two.x, one.y + two.y);
        }

        public static Vector2d operator -(Vector2d one, Vector2d two)
        {
            return new Vector2d(one.x - two.x, one.y - two.y);
        }

        // this is no a dot or cross product 
        public static Vector2d operator *(Vector2d one, Vector2d second)
        {
            return new Vector2d(one.x * second.x, one.y * second.y);
        }

        // scale
        public static Vector2d operator *(Vector2d one, double scaler)
        {
            return new Vector2d(one.x * scaler, one.y * scaler);
        }

        public static Vector2d operator /(Vector2d one, double divider)
        {
            return new Vector2d(one.x / divider, one.y / divider);
        }

    }
}
