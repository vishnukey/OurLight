using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector3 FromTo(this Vector3 from, Vector3 to){
        return to - from;
    }

    public static void SmoothLookAt(this Transform transform, Vector3 point, float speed)
    {
        transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(
                    transform.position.FromTo(point),
                    Vector3.up
                ),
                Time.deltaTime * speed
            );
    }
}
