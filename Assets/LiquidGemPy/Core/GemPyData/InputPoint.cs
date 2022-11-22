using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace LiquidGemPy.Core
{
    
    public class InputPoint: IComparable<InputPoint>
    {
        public int SurfaceId;
        // {
        //     get => Stack.Surfaces.orderID;
        //     set => Stack.Surfaces.orderID = value;
        // }

        public int        Series;
        public string     Formation;
        
        [JsonIgnore]
        public GameObject GameObject;
        
        [JsonIgnore]
        public Vector3    Position => GameObject.transform.position;

        [JsonProperty("position")]
        public float[] PositionStore
        {
            get
            {
                return new[] { Position.x, Position.y, Position.z };
            }
            set
            {
                var position = Position;
                position.x = value[0];
                position.y = value[1];
                position.z = value[2];
            }
        }
        
        [JsonIgnore]
        public Color      Color;
        [JsonProperty("color")] public float[] ColorStore
        {
            get
            {
                return new[] {Color.r, Color.g, Color.b, Color.a};
            }
            set
            {
                Color.r = value[0];
                Color.g = value[1];
                Color.b = value[2];
                Color.a = value[3];
            }
        }

        public int CompareTo(InputPoint other)
        {
            int compareTo = SurfaceId.CompareTo(other.SurfaceId);
            return compareTo;
        }
    }
}