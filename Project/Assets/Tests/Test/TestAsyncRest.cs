using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using GemPlay.Core.Data.LiquidEarth;
using LiquidGemPy.Modules.REST_API;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    [TestFixture]
    public class TestAsyncRest
    {
        /// <summary>
        /// Hack to run async tests
        /// </summary>
        private static T RunAsyncMethodSync<T>(Func<Task<T>> asyncFunc) {
            return Task.Run(async () => await asyncFunc()).GetAwaiter().GetResult();
        }
        
        private static void RunAsyncMethodSync(Func<Task> asyncFunc) {
             Task.Run(async () => await asyncFunc()).GetAwaiter().GetResult();
        }

        #region User Management
        [SetUp]
        public void SetUp()
        {
        }
        #endregion
        
        [Test]
        public void TestGetLiquidEarthAsync()
        {
            // Read Json in Assets/Jsons/GemPyInput.json
            var json = File.ReadAllText(Application.dataPath + "/Jsons/example.json");
            var localHost    = "http://localhost:8000";
            var bytes = RunAsyncMethodSync(() => RestClient.GetBytes(localHost, json));
            LiquidEarthUnstructRawData le = RestClient.ParseLiquidEarth(bytes);
            LiquidEarthTexturedSurface surface = new LiquidEarthTexturedSurface(le, "foo");
            // TODO: Replace Log by assert
            Debug.Log(le.Mesh.Vertices[0]);
        }
    }
    
}