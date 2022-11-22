using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Numerics;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Gempy
{
    public class Stack
    {
        [SerializeField]
        public Dictionary<int, List<int>> SeriesDict;

        public Stack()
        {
            SeriesDict = new Dictionary<int, List<int>>();
        }
        
    }
    
    public class Surface 
    {
        public int Id;
        public Color Color = Color.white;
        public Interface[] InterfacePoints;
        public Orientation[] OrientationPoints;
        
        public Surface(int id, Color color, Interface[] inter, Orientation[] orient)
        {
            Id = id;
            Color = color;
            InterfacePoints = inter;
            OrientationPoints = orient;
        }
        
    }
    public class Interface // ? DEP
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
        public int ObjId;

        public Interface(float x, float y, float z, int id, int objId)
        {
            X = x;
            Y = y;
            Z = z;
            Id = id;
            ObjId = objId;
        }

    }
    
    public class Orientation
    {
        public float X;
        public float Y;
        public float Z;
        public float Azimuth;
        public float Inclination;
        public int Polarity;
        public string Name;
        public int Id;
        
        public Orientation(float x, float y, float z, float az, 
            float inc, int polarity, int id)
        {
            X = x;
            Y = y;
            Z = z;
            Azimuth = az;
            Inclination = inc;
            Polarity = polarity;
            Id = id;
        }
    }

    public class InputVertical
    {
        [SerializeField]
        public List<Interface> Interfaces;
        
        [SerializeField]
        public List<Orientation> Orientations;

        public InputVertical()
        {
            Interfaces = new List<Interface>();
            Orientations = new List<Orientation>();
        }

        public InputVertical(List<Interface> interfaces, List<Orientation> orientations)
        {
            Interfaces = interfaces;
            Orientations = orientations;
        }

        public void Add(Interface item)
        {
            Interfaces.Add(item);
        }

        public void Add(Orientation item)
        {
            Orientations.Add(item);
        }
        
    }
}

