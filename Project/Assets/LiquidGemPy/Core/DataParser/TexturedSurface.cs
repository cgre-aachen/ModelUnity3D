using System;
using System.Collections.Generic;
using System.Linq;
using LiquidGemPy.Core.LiquidEarth;
using UnityEngine;
using Mesh = LiquidGemPy.Core.LiquidEarth.Mesh;

namespace GemPlay.Core.Data.LiquidEarth
{
    [System.Serializable]
    public class LiquidEarthTextureHeader
    {
        public int[] data_shape;
        public float[] texture_origin;
        public float[] texture_point_u;
        public float[] texture_point_v;
        
        public static LiquidEarthTextureHeader CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<LiquidEarthTextureHeader>(jsonString);
        }
    }
    
    public class LiquidEarthTexture
    {
        public LiquidEarthTextureHeader Header;
        public Texture2D Texture;

        public LiquidEarthTexture(LiquidEarthTextureHeader header, byte[] bytes)
        {
            Texture = new Texture2D(header.data_shape[1], header.data_shape[0], TextureFormat.RGBA32,
                false);

            var width = header.data_shape[0]; //flip texture to match with flipped y/z axis in unity
            var height = header.data_shape[1];

            var data = new float[header.data_shape[0], header.data_shape[1], header.data_shape[2]];
            Buffer.BlockCopy(bytes, 0, data, 0, 4 * width * height * 4);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //normalize color and populate Texture
                    Texture.SetPixel(j, width - i,
                        new Color(data[i, j, 0] / 255.0f, data[i, j, 1] / 255.0f, data[i, j, 2] / 255.0f,
                            data[i, j, 3] / 255.0f));
                }
            }
        }
    }
    
    public class LiquidEarthTexturedSurface: LiquidEarthBase // * formerly known as LiquidEarthTexturedMesh
    {
        public readonly LiquidEarthMeshHeader MeshHeader;
        public readonly Mesh Mesh;
        public LiquidEarthTextureHeader TextureHeader;

        public readonly List<int[]> CellSubMesh;
        
        public Texture2D Texture
        {
            get => TextureDict.LastOrDefault().Value;
            set => TextureDict["default"] = value;
        }

        public Dictionary<string, Texture2D> TextureDict = new Dictionary<string, Texture2D>();
        
        public Vector2[] Uvs;
        public Vector3[] Normals;
        public VizParamCollection VisualizationParamCollection;
        public override LiquidEarthDataType DataType => LiquidEarthDataType.StaticMesh;

        //Todo:

        // - if no texture is given, create a colorbar texture and set uvs accordingly
        //- create uvs: [time normalize to 0-1, 0] 
        // - create texture from vertex (or cell?) attributes
        //danger: if we do it like that, the color between two vertices will interpolate bewtween the two values on the
        //colorbar! the  colorchanges  will not be discrete
        public LiquidEarthTexturedSurface(LiquidEarthUnstructRawData liquidEarthUnstructRawData, string dataId) : base(dataId)
        {
            MeshHeader = liquidEarthUnstructRawData.Header;
            Mesh = liquidEarthUnstructRawData.Mesh;
            CheckUVsAsAttributeOrDefault(Mesh);
            SetDefaultViz(Mesh);
        }

        /*Constructor for multiple materials*/
        public LiquidEarthTexturedSurface(
            LiquidEarthUnstructRawData liquidEarthUnstructRawDataMaster,
            List<LiquidEarthUnstructRawData> liquidEarthUnstructRawDataPerSubMesh,
            LiquidEarthTextureHeader textureHeader,
            Dictionary<string, Texture2D> textureDict, string dataId) : base(dataId)
        {
            MeshHeader = liquidEarthUnstructRawDataMaster.Header;
            Mesh = liquidEarthUnstructRawDataMaster.Mesh;
            CheckUVsAsAttributeOrDefault(Mesh);
            
            TextureHeader = textureHeader;
            TextureDict = textureDict;
            
            SetDefaultViz(Mesh);

            // Extract submesh cells
            CellSubMesh = new List<int[]>();
            foreach (var le in liquidEarthUnstructRawDataPerSubMesh)
            {
                CellSubMesh.Add(le.Mesh.Cells);
            }
            
        }


        /*check if UVs are sent, if not populate UVs with [0,0]*/
        private void CheckUVsAsAttributeOrDefault(Mesh mesh)
        {
            if (mesh.VertexAttributes != null && mesh.VertexAttributes.ContainsKey("u") & mesh.VertexAttributes.ContainsKey("v"))
            {
                Uvs = new Vector2[mesh.Vertices.Length];
                for (int i = 0; i < mesh.Vertices.Length; i++)
                {
                    Uvs[i] = new Vector2(mesh.VertexAttributes["u"][i], mesh.VertexAttributes["v"][i]);
                }
            }
            // else /*Necessary!*/
            // {
            //     for (int i = 0; i < mesh.Vertices.Length; i++) Uvs[i] = new Vector2(0.0f, 0.0f);
            //     Debug.Log("no UV data found, creating default UVs");
            // }
        }
        

        private void SetDefaultViz(Mesh mesh)
        {
            this.VisualizationParamCollection = new VizParamCollection(mesh);
        }

    }
}