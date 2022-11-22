using System.Threading.Tasks;
using GemPlay.Core.Data.LiquidEarth;
using GemPlay.Modules.TexturedMesh;
using Gempy;
using LiquidGemPy.Modules.REST_API;

namespace LiquidGemPy.API
{
    public static class ComputeModel
    {
        public static async Task SendDataAndSpawn(GemPyInputSchema inputSchema)
        {
            // Serialize inputSchema
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(inputSchema);

            var                        localHost = "http://localhost:8000";
            var                        bytes     = await RestClient.GetBytes(localHost, json);
            
            LiquidEarthUnstructRawData le        = RestClient.ParseLiquidEarth(bytes, true);
            LiquidEarthTexturedSurface surface   = new LiquidEarthTexturedSurface(le, "foo");
            var                        foo       = TexturedMeshInterface.LiquidEarthToGemPlayStaticMesh(surface);
            
            for (var i = 0; i < foo.Count; i++) TexturedMeshInterface.SpawnStaticMesh(foo[i], i);
        }
    }
}