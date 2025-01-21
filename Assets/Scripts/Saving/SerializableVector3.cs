using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    // Constructor to initialize from a Vector3
    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    // Convert back to a Vector3
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
