using System;
using System.Collections.Generic;
using System.Linq;
using Gempy;
using LiquidGemPy.Core;
using LiquidGemPy.Core.Schemas;

using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Windows;

namespace LiquidGemPy.Modules.DataAssembler
{
    public static class InterpolationInputSpawner
    {
        private static readonly GameObject RootSurfacePointsGameObject = new("SurfacePoints");
        private static readonly GameObject RootOrientationGameObject   = new("Orientation");
        private static readonly GameObject RootStackGameObject         = new("Stack");
        private static readonly int        BaseColor                   = Shader.PropertyToID("_BaseColor");
        private static readonly List<int> _surfaceIDs                  = new List<int>();
        private static readonly List<int> _stackIDs                    = new List<int>();
        private static readonly List<string> _units                    = new List<string>();
        private static readonly Dictionary<string, int> _unitID        = new Dictionary<string, int>();
        private static Vector3             mpos                        = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        private static readonly List<Vector3> _origPos                 = new List<Vector3>();
        private static bool ToggleExplode                     = false;
        
        [SerializeField]
        private static Dictionary<Lithology, Vector3> _lithcolordict   = new Dictionary<Lithology, Vector3>()
        {
            {Lithology.Sandstone, new Vector3(205f/255f, 255f/255f, 217f/255f)},
            {Lithology.Shale, new Vector3(172f/255f, 228f/255f,200f/255f)},
            {Lithology.Limestone, new Vector3(67f/255f, 175f/255f, 249f/255f)},
            {Lithology.Evaporite, new Vector3(1f/255f, 156f/255f, 205f/255f)},
            {Lithology.Coal, new Vector3(130f/255f, 0f/255f, 65f/255f)},
            {Lithology.Granite, new Vector3(249f/255f, 181f/255f,187f/255f)},
            {Lithology.Basalt, new Vector3(221f/255f,179f/255f,151f/255f)}
        };

        public static void SurfacePointSpawner(Vector3 spawnCoordinates, int surfaceId, int seriesId, string formation, Color color)
        {
            // Option to provide IDs by the user and choose a color
            
            var loaderGameObject = GameObject.Find("Loader");
            var loader = loaderGameObject.GetComponent<Loader>();
            var surfacePointPrefab = loader.SurfacePointPrefab;
            
            var surfacePointGo = GameObject.Instantiate(surfacePointPrefab, spawnCoordinates, Quaternion.identity);
            surfacePointGo.transform.parent = RootSurfacePointsGameObject.transform;
            var material = surfacePointGo.GetComponentInChildren<MeshRenderer>().material;
            material.SetColor(BaseColor, color);
        
            var surfacePoint   = new SurfacePoint
            {
                SurfaceId  = surfaceId,
                GameObject = surfacePointGo,
                Series     = seriesId, 
                Formation  = formation,
                Color      = color
            };
            
            InterpolationInput.AddSurfacePoint(surfacePoint);
            switch (_surfaceIDs.Contains(surfaceId), _stackIDs.Contains(seriesId))
            {
                case (true, true):
                    break;
                case (false, true):
                    _surfaceIDs.Add(surfaceId);
                    UnitStackSpawner(surfaceId, seriesId, color);
                    break;
                case (false, false):
                    _stackIDs.Add(seriesId);
                    _surfaceIDs.Add(surfaceId);
                    UnitStackSpawner(surfaceId, seriesId, color);
                    break;
            }
     
        }
        
        public static void SurfacePointSpawner(Vector3 spawnCoordinates, int surfaceId, int seriesId, string formation, Lithology enumColor)
        {
            // Option to provide IDs by the User, color is determined by enumerator
            
            var loaderGameObject = GameObject.Find("Loader");
            var loader = loaderGameObject.GetComponent<Loader>();
            var surfacePointPrefab = loader.SurfacePointPrefab;
            
            var surfacePointGo = GameObject.Instantiate(surfacePointPrefab, spawnCoordinates, Quaternion.identity);
            surfacePointGo.transform.parent = RootSurfacePointsGameObject.transform;
            var material = surfacePointGo.GetComponentInChildren<MeshRenderer>().material;
            
            var colorCode = _lithcolordict[enumColor];
            var color = new Color(colorCode[0], colorCode[1], colorCode[2], 0.0f);
            material.SetColor(BaseColor, color);
        
            var surfacePoint   = new SurfacePoint
            {
                SurfaceId  = surfaceId,
                GameObject = surfacePointGo,
                Series     = seriesId,
                Formation  = formation,
                Color      = color
            };
            
            InterpolationInput.AddSurfacePoint(surfacePoint);
            switch (_surfaceIDs.Contains(surfaceId), _stackIDs.Contains(seriesId))
            {
                case (true, true):
                    break;
                case (false, true):
                    _surfaceIDs.Add(surfaceId);
                    UnitStackSpawner(surfaceId, seriesId, color);
                    break;
                case (false, false):
                    _stackIDs.Add(seriesId);
                    _surfaceIDs.Add(surfaceId);
                    UnitStackSpawner(surfaceId, seriesId, color);
                    break;
            }
        }
        
        public static void SurfacePointSpawner(Vector3 spawnCoordinates, string formation, Color color)
        {
            // Option to only provide Formation string and Color. will be determined from Stack geometry <- not yet implemented
            
            var loaderGameObject = GameObject.Find("Loader");
            var loader = loaderGameObject.GetComponent<Loader>();
            var surfacePointPrefab = loader.SurfacePointPrefab;
            
            var surfacePointGo = GameObject.Instantiate(surfacePointPrefab, spawnCoordinates, Quaternion.identity);
            surfacePointGo.transform.parent = RootSurfacePointsGameObject.transform;
            var material = surfacePointGo.GetComponentInChildren<MeshRenderer>().material;
            
            material.SetColor(BaseColor, color);
            
            if (_units.Contains(formation))
            {
                
            }
            else
            {
                _units.Add(formation);
                _surfaceIDs.Add(_units.Count);
                _unitID[formation] = _units.Count;
                UnitStackSpawner(formation, color);
            }
            
            var surfacePoint   = new SurfacePoint
            {
                SurfaceId  = _units.Count,
                GameObject = surfacePointGo,
                // Series     = seriesId, 
                Formation  = formation,
                Color      = color
            };
            
            InterpolationInput.AddSurfacePoint(surfacePoint);
            
        }
        
        public static void OrientationSpawner(Vector3 spawnCoordinates, int surfaceId, int seriesId, string formation, Color color)
        {
            // Orientation without azimuth or inclination 
            var loaderGameObject = GameObject.Find("Loader");
            var loader = loaderGameObject.GetComponent<Loader>();
            var orientationPrefab = loader.OrientationPrefab;

            var initialGradient    = Quaternion.identity; // ? Make this a variable
            var orientationGo = GameObject.Instantiate(orientationPrefab, spawnCoordinates, initialGradient);
            orientationGo.transform.parent = RootOrientationGameObject.transform;
            orientationGo.GetComponentInChildren<MeshRenderer>().material.SetColor(BaseColor, color);
            
            var orientation   = new Orientation
            {
                SurfaceId  = surfaceId,
                GameObject = orientationGo,
                Series     = seriesId, 
                Formation  = formation,
                Color      = color
            };
            
            InterpolationInput.AddOrientation(orientation);
        }
        
        public static void OrientationSpawner(Vector3 spawnCoordinates, float azimuth, float dip, int surfaceId, int seriesId, Color color)
        {
            // Orientation with azimuth and inclination (dip)
            var loaderGameObject = GameObject.Find("Loader");
            var loader = loaderGameObject.GetComponent<Loader>();
            var orientationPrefab = loader.OrientationPrefab;
            
            var rotation = Quaternion.AngleAxis(azimuth + 180f, Vector3.up) * Quaternion.AngleAxis(dip, Vector3.left);
            var orientationGo = GameObject.Instantiate(orientationPrefab, spawnCoordinates, rotation);
            orientationGo.transform.parent = RootOrientationGameObject.transform;
            orientationGo.GetComponentInChildren<MeshRenderer>().material.SetColor(BaseColor, color);
            
            var orientation   = new Orientation
            {
                SurfaceId  = surfaceId,
                GameObject = orientationGo,
                Series     = seriesId, 
                Color      = color,
            };
            
            InterpolationInput.AddOrientation(orientation);
        }

        public static void UnitStackSpawner(string formation, Color color)
        {
            // Spawner of the Stack, not 100 % working yet, missing logic for new series
            
            var loaderGameObject = GameObject.Find("Loader");
            var loader = loaderGameObject.GetComponent<Loader>();
            var UnitPrefab = loader.StackPrefab;
            var screenBottomCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.2f, 20.0f));
            RootStackGameObject.transform.position = screenBottomCenter;
            Debug.Log($"Formation {formation}");
            var surfaceId = _unitID[formation];
            
            if (_surfaceIDs is {Count: > 0})
            {
                var r = UnitPrefab.GetComponent<Renderer>();
                
                if (r == null)
                {
                    return;
                }
                var relX = 0; // relX has to come from the seriesID
                var relY = -surfaceId;
                _origPos.Add(new Vector3(relX, relY, 0));
                var UnitGo = GameObject.Instantiate(UnitPrefab); 
                UnitGo.transform.parent = RootStackGameObject.transform;
                UnitGo.transform.localPosition = new Vector3(relX, relY, 0);
                UnitGo.GetComponentInChildren<MeshRenderer>().material.SetColor(BaseColor, color);
                var unit = new SurfaceStack
                {
                    GameObject = UnitGo,
                    SurfaceId = surfaceId,
                    Color = color,
                    Formation = formation
        
                };
                InterpolationInput.AddSurfaceToStack(unit);
            }
            else
            {
                _origPos.Add(new Vector3(0, 0, 0));
                var UnitGo = GameObject.Instantiate(UnitPrefab);
                UnitGo.transform.parent = RootStackGameObject.transform;
                UnitGo.GetComponentInChildren<MeshRenderer>().material.SetColor(BaseColor, color);
                var unit = new SurfaceStack
                {
                    GameObject = UnitGo,
                    SurfaceId = surfaceId,
                    // Series = seriesId,
                    Color = color,
                    Formation = formation
        
                };
                InterpolationInput.AddSurfaceToStack(unit);
            }
        }
        public static void UnitStackSpawner(int surfaceId, int seriesId, Color color)
        {
            // Spawner of stack, if IDs are provided by user
            
            var loaderGameObject = GameObject.Find("Loader");
            var loader = loaderGameObject.GetComponent<Loader>();
            var UnitPrefab = loader.StackPrefab;
            var screenBottomCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0.2f, 20.0f));
            RootStackGameObject.transform.position = screenBottomCenter;
            if (_surfaceIDs is {Count: > 0})
            {
                var r = UnitPrefab.GetComponent<Renderer>();
                
                if (r == null)
                {
                    return;
                }

                var relX = -seriesId; 
                var relY = -surfaceId;

                _origPos.Add(new Vector3(relX, relY, 0));
                var UnitGo = GameObject.Instantiate(UnitPrefab);
                UnitGo.transform.parent = RootStackGameObject.transform;
                UnitGo.transform.localPosition = new Vector3(relX, relY, 0);
                UnitGo.GetComponentInChildren<MeshRenderer>().material.SetColor(BaseColor, color);
                var unit = new SurfaceStack
                {
                    GameObject = UnitGo,
                    SurfaceId = surfaceId,
                    Series = seriesId,
                    Color = color,

                };
                InterpolationInput.AddSurfaceToStack(unit);
            }
            else
            {
                _origPos.Add(new Vector3(0, 0, 0));
                var UnitGo = GameObject.Instantiate(UnitPrefab, screenBottomCenter, Quaternion.identity);
                UnitGo.transform.parent = RootStackGameObject.transform;
                UnitGo.GetComponentInChildren<MeshRenderer>().material.SetColor(BaseColor, color);
                var unit = new SurfaceStack
                {
                    GameObject = UnitGo,
                    SurfaceId = surfaceId,
                    Series = seriesId,
                    Color = color,

                };
                InterpolationInput.AddSurfaceToStack(unit);
            }
        }

        public static void ReArrangeStack()
        {
            var stack = InterpolationInput.SurfaceStack;
            List<float> xpos = new();
            List<float> ypos = new();
            
            foreach (var surface in stack)
            {
                var xses = surface.GameObject.transform.localPosition.x;
                var yses = surface.GameObject.transform.localPosition.y;
                xpos.Add(xses);
                ypos.Add(yses);
            }

            var xmin = xpos.Min();
            var ymin = ypos.Min();
            var counter = 0;
            foreach (var surface in stack)
            {
                var transformLocalPosition = surface.GameObject.transform.localPosition;
                var nXpos= transformLocalPosition.x + Mathf.Abs(xmin);
                var nYpos = transformLocalPosition.y + Mathf.Abs(ymin);
                _origPos[counter] = new Vector3(nXpos, nYpos, 0);
                surface.GameObject.transform.localPosition = new Vector3(nXpos, nYpos, 0);
                counter += 1; 
            }
        }

        public static void UnitStackExploder()
        {
            var stack = InterpolationInput.SurfaceStack;
            List<float> xpos = new();
            
            foreach (var surface in stack)
            {
                var xses = surface.GameObject.transform.localPosition.x;
                xpos.Add(xses);
            }

            var xmin = xpos.Min();

            if (ToggleExplode == false)
            {
                foreach (var surface in stack)
                {
                    var locPos = surface.GameObject.transform.localPosition;
                    surface.GameObject.transform.localPosition = new Vector3(xmin, locPos.y, locPos.z);
                }

                ToggleExplode = true;
            }
            else
            {
                for (int i = 0; i < stack.Count; i++)
                {
                    stack[i].GameObject.transform.localPosition = _origPos[i]; 
                }

                ToggleExplode = false;
            }
            
            

        }
        
        public static void SpawnData(UserInputSchema loadedSchema)
        {
            if (loadedSchema != null)
            {
                // var loadedSchema = JsonParser.loadedInputSchema;
                var surfacePointSchema = loadedSchema.SurfacePoints;
                foreach (var surfacepoint in surfacePointSchema)
                {
                    var spawnCoordinates = new Vector3(surfacepoint.X, surfacepoint.Y, surfacepoint.Z);
                    SurfacePointSpawner(
                        spawnCoordinates: spawnCoordinates,
                        surfaceId: surfacepoint.Id,
                        seriesId : surfacepoint.Stack, 
                        formation: surfacepoint.Formation,
                        color    : new Color(surfacepoint.Color[0], surfacepoint.Color[1], surfacepoint.Color[2], surfacepoint.Color[3]));
                }

                var orientationPointSchema = loadedSchema.Orientations;
                foreach (var orientationPoint in orientationPointSchema)
                {
                    var spawnCoordinates = new Vector3(orientationPoint.X, orientationPoint.Y, orientationPoint.Z);
                    var azimuth = orientationPoint.Azimuth;
                    var dip     = orientationPoint.Inclination;
                    
                    OrientationSpawner(
                        spawnCoordinates: spawnCoordinates,
                        azimuth: azimuth,
                        dip: dip,
                        surfaceId:orientationPoint.Id,
                        seriesId: orientationPoint.Stack, 
                        color: new Color(orientationPoint.Color[0], orientationPoint.Color[1], orientationPoint.Color[2], orientationPoint.Color[3]));
                }
                ReArrangeStack();
            }
        }
    }
}