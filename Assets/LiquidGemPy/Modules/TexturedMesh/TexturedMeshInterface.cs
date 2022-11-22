using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GemPlay.Core.Data.LiquidEarth;
using GemPlay.Modules.TexturedMeshSpawner;
using LiquidGemPy.Core;
using Unity.Jobs;
using UnityEngine;

namespace GemPlay.Modules.TexturedMesh
{
    public static class TexturedMeshInterface
    {
        private static readonly int Basemap = Shader.PropertyToID("_Basemap");
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        public static List<GemPlayStaticMeshData> LiquidEarthToGemPlayStaticMesh(LiquidEarthTexturedSurface liquidEarthTexturedSurface) 
            => LiquidEarthToGemPlay.ToTexturedMeshItem(liquidEarthTexturedSurface);


        public static async void SpawnStaticMesh(GemPlayStaticMeshData gemPlayMesh, int meshNumber = default)
        {
            gemPlayMesh.UpdateUnityMesh(); // * This still have 0.4s hiccups in museum dataset 
            await Task.Yield();
            var gameObject = TexturedMeshSpawner.SpawnTexturedMesh(gemPlayMesh, meshNumber);
        }
    }
}