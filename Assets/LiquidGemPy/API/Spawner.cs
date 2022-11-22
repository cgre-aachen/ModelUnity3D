using System;
using System.Collections.Generic;
using System.IO;
using Codice.Client.Common;
using Gempy;
using LiquidGemPy.API;
using LiquidGemPy.Core;
using LiquidGemPy.Core.Schemas;
using LiquidGemPy.Modules.DataAssembler;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor.Graphs;

namespace LiquidGemPy.API
{
    public class Series
    {
        public List<Lithology> Surfaces;
        public string BottomRelation;
        // ....
    }
    
    public class Spawner : MonoBehaviour
    {
        public Vector3 SpawnCoordinates; 
        public int     SurfaceId;           // identifier, unique per surface
        public int     Series;              //  identifier, unique per Series/Stack
        public string  Formation;
        public Color   Color;

        // [Serializable]
        public Lithology Lithology;
        public List<Lithology> Surfaces;
        public List<Series> SeriesPile;
        
    }
    
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Spawner spawner = (Spawner)target; // ? Do we need this

            if (GUILayout.Button("Spawn Surface Point"))
            {
                // There are three possibilities to spawn input points. Two are commented out...but ideally, the last one is the best, where IDs are not
                // specified by the user, but by giving only the formation name.
                
                // Give a color and IDs
                InterpolationInputSpawner.SurfacePointSpawner(spawner.SpawnCoordinates, spawner.SurfaceId, spawner.Series, spawner.Formation, spawner.Color);
                
                // Specify a lithology and IDs
                // InterpolationInputSpawner.SurfacePointSpawner(spawner.SpawnCoordinates, spawner.SurfaceId, spawner.Series, spawner.Formation, spawner.Lithology);
                
                // Specify a Formation
                // InterpolationInputSpawner.SurfacePointSpawner(spawner.SpawnCoordinates, spawner.Formation, spawner.Color);
            }

            if (GUILayout.Button("Spawn Orientation Point")) 
                InterpolationInputSpawner.OrientationSpawner(spawner.SpawnCoordinates, spawner.SurfaceId, spawner.Series, spawner.Formation, spawner.Color);
            
            if (GUILayout.Button("Compute Model"))
            {
                var gempyInput = new GemPyInputSchema();
                JsonParser.GemPyInputToJson(gempyInput); // * This is just for debugging
                ComputeModel.SendDataAndSpawn(gempyInput);
            }

            if (GUILayout.Button("Save Points to JSON"))
            {
                JsonParser.SaveModelToJson();
            }

            if (GUILayout.Button("Load Points from JSON"))
            {
                JsonParser.LoadJson(Application.dataPath + "/Jsons/UserInput.json");
            }

            if (GUILayout.Button("Explode / Contract Pile"))
            {
                InterpolationInputSpawner.UnitStackExploder();
            }
        }
    }
}