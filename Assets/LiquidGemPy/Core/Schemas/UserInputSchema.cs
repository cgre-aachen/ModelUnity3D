using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LiquidGemPy.Core
{
    public class Interface
    {
        [SerializeField]
        public float X;
        
        [SerializeField]
        public float Y;
        
        [SerializeField]
        public float Z;
        
        public string Name;
        
        [SerializeField]
        public int Id;
    
        [SerializeField] 
        public int Stack;

        [SerializeField] 
        public string Formation;
    
        [SerializeField] 
        public float[] Color;
    
        public Interface(float x, float y, float z, int id, int stack, string formation, float[] color)
        {
            X = x;
            Y = y;
            Z = z;
            Id = id;
            Stack = stack;
            Formation = formation;
            Color = color;
        }
    
    }
    
    public class OrientationPoint
    {
        public float X;
        public float Y;
        public float Z;
        public float Azimuth;
        public float Inclination;
        public float Polarity;
        public string Formation;
        public int Id;
        public readonly int Stack;
        public float[] Color;
        
        public OrientationPoint(float x, float y, float z, float az, 
            float inc, float polarity, int id, int stack, string formation, float[] color)
        {
            X = x;
            Y = y;
            Z = z;
            Azimuth = az;
            Inclination = inc;
            Polarity = polarity;
            Id = id;
            Stack = stack;
            Formation = formation;
            Color = color;
        }
    }
    
    public class UserInputSchema
    {
        [SerializeField]
        [JsonProperty("Interface_points")]   public List<Interface> SurfacePoints;
        [SerializeField]
        [JsonProperty("Orientation_points")] public List<OrientationPoint> Orientations;
        
        public UserInputSchema()
        {
            SurfacePoints = new List<Interface>();
            Orientations = new List<OrientationPoint>();
        }
        
        public UserInputSchema(List<Interface> surfacePoints, List<OrientationPoint> orientations)
        {
            SurfacePoints = surfacePoints;
            Orientations = orientations;
        }
        public void Add(Interface item)
        {
            SurfacePoints.Add(item);
        }

        public void Add(OrientationPoint item)
        {
            Orientations.Add(item);
        }

        
    }
}