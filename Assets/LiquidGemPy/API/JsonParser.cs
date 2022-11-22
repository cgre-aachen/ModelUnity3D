using System.IO;
using Gempy;
using LiquidGemPy.Core;
using LiquidGemPy.Modules.DataAssembler;
using Newtonsoft.Json;
using UnityEngine;

namespace LiquidGemPy.API
{
    public static class JsonParser
    {
        public static string GemPyInputToJson(GemPyInputSchema inInputs)
        {
            var json = JsonConvert.SerializeObject(inInputs, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            SaveData(json, "GemPyInput");
            return json;
        }

        public static string GemPyInputToJson(UserInputSchema inInputs)
        {
            var json = JsonConvert.SerializeObject(inInputs, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});
            SaveData(json, "UserInput");
            return json;
        }

        private static void SaveData(string json, string name)
        {
            var                savePath = $"{Application.dataPath}\\Jsons{Path.AltDirectorySeparatorChar}{name}.json";
            using StreamWriter writer   = new StreamWriter(savePath);
            writer.Write(json);
        }
        
        public static void LoadJson(string path)
        {
            if (File.Exists(path))
            {
                UserInputSchema loadedInputSchema = new UserInputSchema();
                var fileContents = File.ReadAllText(path);
                loadedInputSchema = JsonConvert.DeserializeObject<UserInputSchema>(fileContents);
                InterpolationInputSpawner.SpawnData(loadedInputSchema);
            }
        }

        public static void SaveModelToJson()
        {
            var userInput         = new UserInputSchema();
            var surfacePoints     = InterpolationInput.SurfacePoints;
            var orientationPoints = InterpolationInput.Orientations;
            surfacePoints.Sort();
            foreach (var surfacePoint in surfacePoints)
            {
                var surfInstance = new Interface(surfacePoint.Position.x, surfacePoint.Position.y,
                    surfacePoint.Position.z, surfacePoint.SurfaceId, surfacePoint.Series, surfacePoint.Formation, surfacePoint.ColorStore);
                userInput.Add(surfInstance);
            }

            foreach (var orientationPoint in orientationPoints)
            {
                Debug.Log($"Surface ID {orientationPoint.SurfaceId}, Azimuth is {orientationPoint.Azimuth}, Dip is {orientationPoint.Dip}");
                var oriInstance = new OrientationPoint(orientationPoint.Position.x, orientationPoint.Position.y,
                    orientationPoint.Position.z,
                    orientationPoint.Azimuth, orientationPoint.Dip, orientationPoint.Polarity,
                    orientationPoint.SurfaceId, orientationPoint.Series, orientationPoint.Formation, orientationPoint.ColorStore);
                userInput.Add(oriInstance);
            }
                
                
            JsonParser.GemPyInputToJson(userInput);
        }
    }
}