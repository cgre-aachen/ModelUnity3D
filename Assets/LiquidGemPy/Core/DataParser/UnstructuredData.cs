using System.Collections.Generic;
using LiquidGemPy.Core.LiquidEarth;
using Newtonsoft.Json;

namespace GemPlay.Core.Data.LiquidEarth
{
    public class LiquidEarthUnstructRawData
    {
        public readonly LiquidEarthMeshHeader Header;
        public readonly Mesh Mesh;

        public LiquidEarthUnstructRawData(LiquidEarthMeshHeader header, Mesh mesh)
        {
            Mesh = mesh;
            Header = header;
        }
    }

    public class LiquidEarthMeshHeader
    {
        [JsonProperty("vertex_shape")] public int[] VertexShape;
        [JsonProperty("cell_shape")] public int[] CellShape;
        [JsonProperty("cell_attr_shape")] public int[] CellAttrShape;
        [JsonProperty("vertex_attr_shape")] public int[] VertexAttrShape;
        [JsonProperty("cell_attr_names")] public string[] CellAttrNames = {};
        [JsonProperty("vertex_attr_names")] public string[] VertexAttrNames = {"Default"};
        [JsonProperty("xarray_attrs")] public Dictionary<string, object> MetaData = new Dictionary<string, object>();
    }
}