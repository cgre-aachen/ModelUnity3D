using System.Collections;
using System.Collections.Generic;
using Gempy;
using UnityEngine;

public class OrientationPoint : SurfacePoint
{
    public Vector3 AzimuthDipVector;
    
    public float Azimuth => AzimuthDipVector.x;
    public float Dip => AzimuthDipVector.y;
    public float Polarity => AzimuthDipVector.z;
}
