using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using UnityEditor.Experimental;

namespace Gempy
{
    
    public class DataAssembler : MonoBehaviour
    {
        public List<SurfacePoint> AllSurfacePoints; // All surface points
        public List<OrientationPoint> AllOrientationPoints; // All orientations
        
        public List<int> _id;
        public List<int> _oid;
        private List<int> _series;
        
        public GameObject Sp; // Surface point Dummy
        public GameObject Op; // Orientation point Dummy
        public Vector3 Spawnpoint; // Spawnpoint
        public int ID; // Surface ID - identifier, unique per surface
        public int Series; // Stack ID - identifier, unique per Series/Stack

        private string path = ""; // Saving path
        private string persistentPath = "";
        
        public SortedDictionary<int, int> _nSurfacePoints; // Key: Surface ID - Values: number of Surface Points 
        public SortedDictionary<int, int> _nSurfacePointsPerSeries; // Key: Series ID - Values: number of Surface Points
        
        public InputVertical inputs = new InputVertical(); // Old way of writing the JSON
        public GemPyInputSchema input_schema = new GemPyInputSchema(); // New way of writing the JSON
        public Stack Stack= new Stack(); // Stack / Series Object
        
        [HideInInspector] 
        public bool UpdateInputdata; // Switch to update the Surface Point Lists
        
        private int _sum; // Sum operator to count the number of Surface Points per Series / Stack
        
        void Start()
        {
            SetPaths();
        }
        
        void Update()
        {
            if (UpdateInputdata)
            {
                UpdateSurfacePoints();
                UpdateInputdata = false;
            }
        }
        public void AddNewSurfacePointInstanceToScene()
        {
            var surfpoint_go = Instantiate(Sp, Spawnpoint, Quaternion.identity); // Spawn a Surface Point
            var surfpoint = surfpoint_go.GetComponent<SurfacePoint>(); // Get the SurfacePoint Coordinate
            _id.Add(ID);
            // Assign ID, Series to SufacePoint                
            surfpoint.SurfaceId = ID;
            surfpoint.Series = Series;
            surfpoint.dataAssembler = this;
            AllSurfacePoints.Add(surfpoint); // Add SurfacePoint to the List
            AllSurfacePoints.Sort();
            // Create an Interface Object for the old way of writing the JSON
            var surfInstance = new Interface(Spawnpoint.x,Spawnpoint.y, Spawnpoint.z, surfpoint.SurfaceId, surfpoint.GetInstanceID());
            inputs.Add(surfInstance);
            AssignSurfaceToSeries(AllSurfacePoints);
            
            // UpdateSurfacePoints();
            // SortSurfacePoints();
            CountPoints(AllSurfacePoints);
        }

        public void AddNewOrientationPointInstanceToScene()
        {
            var orientation_go = Instantiate(Op, Spawnpoint, Quaternion.identity); // Spawn an Orientation
            var orientpoint = orientation_go.GetComponent<OrientationPoint>();
            
            _oid.Add(ID);

            orientpoint.SurfaceId = ID;
            orientpoint.Series = Series;
            orientpoint.dataAssembler = this;
            DipAzimuth(orientpoint);
            AllOrientationPoints.Add(orientpoint);
            AllOrientationPoints.Sort();
            
            var orientInstance = new Orientation(Spawnpoint.x, Spawnpoint.y, Spawnpoint.z, orientpoint.Azimuth,
                orientpoint.Dip, orientpoint.Polarity, orientpoint.SurfaceId);
            
            inputs.Add(orientInstance);

            CountPoints(AllOrientationPoints);
            // AssignSurfaceToSeries(AllOrientationPoints);
        }
        
        public void DipAzimuth(OrientationPoint orientation)
        {
            var Normal = orientation.transform.up;
        
            var StrikeVector = Vector3.Cross( Normal, Vector3.up);
            var DipVector = Vector3.Cross(Normal,StrikeVector); //can be upfacing or downfacing. how to determine?
            
            var azimuth =  90f - (Mathf.Rad2Deg * Mathf.Atan2(DipVector.z, DipVector.x)); //90f because the north is at the top of the screen. substract the value because we want to go clockwise
            var Azimuth = azimuth < 0f ? azimuth + 360f : azimuth; //if the azimuth is negative, add 360 to it.
            orientation.Polarity = Normal.y > 0.0f ? 1 : -1;
            var Dip = Vector3.Angle(Vector3.up,DipVector)-90;
            print($"Dip is {DipVector}");
            orientation.Dip = Dip;
            orientation.Azimuth = Azimuth;
        }

        public void UpdateSurfacePoints(string old)
        {
            // Old method of writing the JSON
            inputs = new InputVertical();
            AllSurfacePoints.Sort();
            foreach (var point in AllSurfacePoints)
            {
                var surfInstance = new Interface(point.transform.position.x, point.transform.position.y, 
                    point.transform.position.z, point.SurfaceId, point.GetInstanceID());
                inputs.Add(surfInstance);
            }

            foreach (var point in AllOrientationPoints)
            {
                DipAzimuth(point);
                var orientInstance = new Orientation(point.transform.position.x, point.transform.position.y, 
                    point.transform.position.z, point.Azimuth, point.Dip, point.Polarity,
                    point.SurfaceId);
                inputs.Add(orientInstance);
            }
            
            AssignSurfaceToSeries(AllSurfacePoints);
            // AssignSurfaceToSeries(AllOrientationPoints);
            // SortSurfacePoints();
            // CountPoints();
            
        }
        public void UpdateSurfacePoints()
        {
            AllSurfacePoints.Sort();
            // print("Surfpoints are updated!");
            var surfacePointsSchema = input_schema.InterpolatorInputSchema._surfacePointsSchema;
            var len = AllSurfacePoints.Count;
            // !DEP surfacePointsSchema.SurfacePointsCoords = new float[len][];
            
            
            for (var i = 0; i < AllSurfacePoints.Count; i++)
            {
                var point = AllSurfacePoints[i];
                var position = point.transform.position;
                surfacePointsSchema.SurfacePointsCoords[i] = new[] {position.x, position.y, position.z};

            }
            
            var orientationPointSchema = input_schema.InterpolatorInputSchema._orientationPointSchema;
            var lenOri = AllOrientationPoints.Count;
            orientationPointSchema.DipPositionsSchema.dip_positions = new float[lenOri][];
            orientationPointSchema.DipGradientsSchema.dip_gradients = new float[lenOri][];
            
            for (var i = 0; i < AllOrientationPoints.Count; i++)
            {
                var point = AllOrientationPoints[i];
                DipAzimuth(point);
                var position = point.transform.position;
                var orientation = point.transform.rotation;
                orientationPointSchema.DipPositionsSchema.dip_positions[i] = new[] {position.x, position.y, position.z};
                orientationPointSchema.DipGradientsSchema.dip_gradients[i] = new[] {orientation.x, orientation.y, orientation.z}; // PLACE HOLDER: REPLACE BY GRADIENTS

            }

            AssignSurfaceToSeries(AllSurfacePoints);
            AssignSurfaceToSeries(AllOrientationPoints);
            // SortSurfacePoints();
            CountPoints(AllSurfacePoints);
            
            var inputDataDescriptorSchema = input_schema.InputDataDescriptorSchema;
            
            var n_points = _nSurfacePoints.Count;
            //inputDataDescriptorSchema.NumberOfPointsPerSurface = new int[n_points];
            for (int i = 0; i < _nSurfacePoints.Count; i++)
            {
                inputDataDescriptorSchema.NumberOfPointsPerSurface[i] = _nSurfacePoints[i];
            }

            var n_series = _nSurfacePointsPerSeries.Count;
            //inputDataDescriptorSchema.NumberOfPointsPerStack = new int[n_series];
            for (int i = 0; i < n_series; i++)
            {
                inputDataDescriptorSchema.NumberOfPointsPerStack[i] = _nSurfacePointsPerSeries[i]; // TODO Error when saving stuff > Series 2 
            }

        }

        void AssignSurfaceToSeries(List<SurfacePoint> allPoints)
        {
            foreach (var point in allPoints)
            {
                var sid = point.SurfaceId;
                var ser = point.Series;
                var isinseries = Stack.SeriesDict.ContainsKey(ser);
                if (isinseries == false)
                {
                    Stack.SeriesDict[ser] = new List<int>{sid};
                }
                else
                {
                    var isinsurfaces = Stack.SeriesDict[ser].Contains(sid);
                    if (isinsurfaces == false)
                    {
                        Stack.SeriesDict[ser].Add(sid);
                    }
                }
            }

            foreach (var kvp in Stack.SeriesDict)
            {
                Debug.Log($"{allPoints.Count.ToString()}, Key = {kvp.Key}, Value = {string.Join(", ", kvp.Value)}");
            }
        }
        
        void AssignSurfaceToSeries(List<OrientationPoint> allPoints)
        {
            foreach (var point in allPoints)
            {
                var sid = point.SurfaceId;
                var ser = point.Series;
                var isinseries = Stack.SeriesDict.ContainsKey(ser);
                if (isinseries == false)
                {
                    Stack.SeriesDict[ser] = new List<int>{sid};
                }
                else
                {
                    var isinsurfaces = Stack.SeriesDict[ser].Contains(sid);
                    if (isinsurfaces == false)
                    {
                        Stack.SeriesDict[ser].Add(sid);
                    }
                }
            }

            foreach (var kvp in Stack.SeriesDict)
            {
                Debug.Log($"{allPoints.Count.ToString()}, Key = {kvp.Key}, Value = {string.Join(", ", kvp.Value)}");
            }
        }

        public void CountPoints(List<SurfacePoint> surfacePoints)
        {
            _nSurfacePoints = new SortedDictionary<int, int>();
            surfacePoints.Sort();
            foreach (var group in surfacePoints.GroupBy(i => i.SurfaceId)) // TODO Change _id to a more robust iterator...surfaceID 
            {
                var c = group.Count();
                _nSurfacePoints[group.Key] = group.Count();
                Debug.Log(string.Format("ID {0}: {1} times", group.Key, group.Count()));
            }
            _nSurfacePointsPerSeries = new SortedDictionary<int, int>();
            foreach (var series in Stack.SeriesDict)
            {
                _sum = 0;
                foreach (var surface in series.Value)
                {
                    _sum += _nSurfacePoints[surface];
                }

                _nSurfacePointsPerSeries[series.Key] = _sum;
                Debug.Log(string.Format("series {0}: {1} interface points", series.Key, _sum));
            }
        }
        
        public void CountPoints(List<OrientationPoint> surfacePoints)
        {
            _nSurfacePoints = new SortedDictionary<int, int>();
            surfacePoints.Sort();
            foreach (var group in surfacePoints.GroupBy(i => i.SurfaceId)) // TODO Change _id to a more robust iterator...surfaceID 
            {
                var c = group.Count();
                _nSurfacePoints[group.Key] = group.Count();
                Debug.Log(string.Format("ID {0}: {1} times", group.Key, group.Count()));
            }
            _nSurfacePointsPerSeries = new SortedDictionary<int, int>();
            foreach (var series in Stack.SeriesDict)
            {
                _sum = 0;
                foreach (var surface in series.Value)
                {
                    _sum += _nSurfacePoints[surface];
                }

                _nSurfacePointsPerSeries[series.Key] = _sum;
                Debug.Log(string.Format("series {0}: {1} interface points", series.Key, _sum));
            }
        }
        
        private void SetPaths()
        {
            path = Application.dataPath + Path.AltDirectorySeparatorChar;
            persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SavedPoints.json";
        }

        public string SurfaceToJson(InputVertical inInputs)
        {
            var json = JsonConvert.SerializeObject(inInputs, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return json;
        }
        
        public string SurfaceToJson(GemPyInputSchema inInputs)
        {
            var json = JsonConvert.SerializeObject(inInputs, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return json;
        }

        public string SurfaceToJson(Interface inInterface)
        {
            // SortSurfacePoints();
            var json = JsonConvert.SerializeObject(inInterface, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Debug.Log(json);
            return json;
        }

        public string NSurfacePointsPerSurface(SortedDictionary<int, int> inSurfaces)
        {
            var json = JsonConvert.SerializeObject(inSurfaces, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return json;
        }

        public void SaveData(string json, string arraytype)
        {
            SetPaths();
            var savePath = path;
            savePath = savePath + arraytype + ".json";
            using StreamWriter writer = new StreamWriter(savePath);
            writer.Write(json);
        }

    }
}