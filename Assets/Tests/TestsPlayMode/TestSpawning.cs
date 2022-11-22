using System.IO;
using System.Threading.Tasks;
using GemPlay.Core.Data.LiquidEarth;
using GemPlay.Modules.TexturedMesh;
using LiquidGemPy.Modules.REST_API;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests.TestsPlayMode
{
    public class TestSpawning
    {
        [TestFixture]
        public class GeneralNUnitTests
        {
            [SetUp]
            public void Setup()
            {
                SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            }

            [TearDown]
            public void DestroyObjects()
            {
            }

            [Test]
            public async Task TestSpawning()
            {
                var                        json      = await File.ReadAllTextAsync(Application.dataPath + "/Jsons/example.json");
                var                        localHost = "http://localhost:8000";
                var                        bytes     = await RestClient.GetBytes(localHost, json);
                LiquidEarthUnstructRawData le        = RestClient.ParseLiquidEarth(bytes);
                LiquidEarthTexturedSurface surface   = new LiquidEarthTexturedSurface(le, "foo");

                var foo = TexturedMeshInterface.LiquidEarthToGemPlayStaticMesh(surface);
                foo.ForEach(data => TexturedMeshInterface.SpawnStaticMesh(data, 0));
                
                await Task.Delay(5000);
                Debug.Log("Done");
            }
        }
    }
}