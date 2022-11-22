using System;
using Newtonsoft.Json;
using UnityEngine;

namespace LiquidGemPy.Core
{
    public class Orientation: InputPoint
    {
        [JsonIgnore]
        private Vector3 _eulerAngles => GameObject.transform.localEulerAngles;
        [JsonIgnore]
        public Vector3 GradientVector
        {
            get
            {
                var gx = Mathf.Sin(Mathf.Deg2Rad * Dip) * Mathf.Sin(Mathf.Deg2Rad * Azimuth) * Polarity;
                var gy = Mathf.Sin(Mathf.Deg2Rad * Dip) * Mathf.Cos(Mathf.Deg2Rad * Azimuth) * Polarity;
                var gz = Mathf.Cos(Mathf.Deg2Rad * Dip) * Polarity;

                return new Vector3(gx, gy, gz);
            }
        }
        
        [JsonIgnore]
        public Vector3 AzimuthDipVector
        {
            get
            {
                var normal = GameObject.transform.up;
                var strikeVector = Vector3.Cross(normal, Vector3.up);
                var dipVector = Vector3.Cross(normal, strikeVector);

                var azimuth = 90f - (Mathf.Rad2Deg * Mathf.Atan2(dipVector.z, dipVector.x));
                var Azimuth = azimuth < 0f ? azimuth + 360f : azimuth;
                var Dip = Vector3.Angle(Vector3.up, dipVector) - 90;
                var polarity = normal.y > 0.0f ? 1 : -1;

                if (Dip == -90)
                {
                    Dip = 0;
                }

                return new Vector3(Azimuth, Dip, polarity);
            }
        }
        [JsonIgnore]
        public Vector3 EulerFromAzimuth // Might be erroneous, as only counts for 'Proper Euler angles', not yaw, pitch and roll 
        {
            get
            {
                var phi = -Mathf.Sin(Mathf.Deg2Rad * Dip) * Mathf.Cos(Mathf.Deg2Rad * (Azimuth + 90f));
                var theta = Mathf.Cos(Mathf.Deg2Rad * Dip);
                var psi = 0f;
                return new Vector3(theta, phi, psi);
            }
        }

        public float Azimuth => AzimuthDipVector.x;

        public float Dip => AzimuthDipVector.y;
        public float Polarity => AzimuthDipVector.z;
    }
}