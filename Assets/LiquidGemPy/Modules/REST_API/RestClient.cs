using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GemPlay.Core.Data.LiquidEarth;
using LiquidGemPy.Core.LiquidEarth;
using Newtonsoft.Json;
using UnityEngine;
using Mesh = LiquidGemPy.Core.LiquidEarth.Mesh;

namespace LiquidGemPy.Modules.REST_API
{
    public static class RestClient
    {
        private static readonly HttpClient Client = new();
        
        static RestClient()
        {
        } 
        
        public static async Task<byte[]> GetBytes(string url, string jsonPayLoad)
        {
            var content = new StringContent(jsonPayLoad, Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(url, content);
            var responseByteArray = await response.Content.ReadAsByteArrayAsync();
            
            Debug.Log("Response: " + responseByteArray.Length);
            return responseByteArray; 
        }

        public static LiquidEarthUnstructRawData ParseLiquidEarth(byte[] data, bool swapYZAxis=false)
        {
            var (jsonHeader, headerLenght) = ExtractHeaderFromByteArray(data);
            LiquidEarthMeshHeader liquidEarthMeshHeader = JsonConvert.DeserializeObject<LiquidEarthMeshHeader>(jsonHeader);
            byte[] body = ExtractBodyFromByteArray(data, headerLenght);
            Mesh mesh = new Mesh(liquidEarthMeshHeader, body, swapYZAxis);
            LiquidEarthUnstructRawData liquidEarthUnstructRawData = new LiquidEarthUnstructRawData(liquidEarthMeshHeader, mesh);
            return liquidEarthUnstructRawData;
        }

        
        private static (string, int) ExtractHeaderFromByteArray(byte[] data)
        {
            /*Extract json header. The encoding was the following:
                header_json = json.dumps(header)
                header_json_bytes = header_json.encode('utf-8')
                header_json_length = len(header_json_bytes)
                header_json_length_bytes = header_json_length.to_bytes(4, byteorder='little')
                body = header_json_length_bytes + header_json_bytes + body
             */
            
            // Read first 4 bytes to get the length of the header
            var headerLength = BitConverter.ToInt32(data, 0); // ! This only works for little endian  
            var headerJson = Encoding.UTF8.GetString(data, 4, headerLength);
            return (headerJson, headerLength);
        }
        
        private static byte[] ExtractBodyFromByteArray(byte[] data, int headerLenght)
        {
            byte[] body = new byte[data.Length - headerLenght - 4];
            Array.Copy(data, headerLenght + 4, body, 0, body.Length);
            return body;
        }
    }
}