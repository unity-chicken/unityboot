using UnityEngine;
using System.Collections;

public enum WheelAxis {
    Z,
    Y,
    X
}

public class Wheel : MonoBehaviour {
    public float speed = 180;
    public WheelAxis axis;
    
    void Start () {
    
    }
    
    void Update () {
        float degree = Time.deltaTime * speed;

        switch (axis) {
            case WheelAxis.Z:
                transform.Rotate(0, 0, degree);
                break;
            case WheelAxis.Y:
                transform.Rotate(0, degree, 0);
                break;
            case WheelAxis.X:
                transform.Rotate(degree, 0, 0);
                break;
        }
    }
}
