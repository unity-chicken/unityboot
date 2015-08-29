using UnityEngine;

public static class Vector3Extensions {

    public static Vector3 RotateX(this Vector3 v, float angle) {
        float sin = Mathf.Sin( angle );
        float cos = Mathf.Cos( angle );
        float ty = v.y;
        float tz = v.z;

        Vector3 result = v;
        result.y = (cos * ty) - (sin * tz);
        result.z = (cos * tz) + (sin * ty);
        return result;
    }

    public static Vector3 RotateY(this Vector3 v, float angle) {
        float sin = Mathf.Sin( angle );
        float cos = Mathf.Cos( angle );
        float tx = v.x;
        float tz = v.z;

        Vector3 result = v;
        result.x = (cos * tx) + (sin * tz);
        result.z = (cos * tz) - (sin * tx);
        return result;
    }
    
    public static Vector3 RotateZ(this Vector3 v, float angle) {
        float sin = Mathf.Sin( angle );
        float cos = Mathf.Cos( angle );
        float tx = v.x;
        float ty = v.y;

        Vector3 result = v;
        result.x = (cos * tx) - (sin * ty);
        result.y = (cos * ty) + (sin * tx);
        return result;
    }
}

