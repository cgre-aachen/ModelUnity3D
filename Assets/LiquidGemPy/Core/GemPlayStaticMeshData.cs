using System.Collections.Generic;
using System.Linq;
using GemPlay.Core.Data.LiquidEarth;
using GemPlay.Core.Helpers;
using UnityEngine;
using UnityMeshSimplifier;

namespace LiquidGemPy.Core
{
    public class GemPlayStaticMeshData
    {
        private          MeshSimplifier             _meshSimplifier;
        private          Mesh                       _unityMesh;
        private readonly LiquidEarthTexturedSurface _liquidEarthTexturedSurface;
        private readonly List<int[]>                _cellSubMeshes;
        private VizParamCollection VizParamCollection => _liquidEarthTexturedSurface.VisualizationParamCollection;

        public bool IsTexture => TextureDict.Count!=0 && (!TextureDict.ContainsKey("default") || TextureDict["default"] != null);

        // ReSharper disable once InconsistentNaming
        private Texture2D _texture
        {
            get => TextureDict.LastOrDefault().Value; // ! This can lead to unexpected behaviour. For example there was a nasty bug when we tried to color a textured mesh
            set => TextureDict["default"] = value;
        }

        public Texture2D Texture
        {
            get
            {
                return _texture == null ? TextureFromColorOrColorbar() : _texture;

                Texture2D TextureFromColorOrColorbar()
                {
                    return ColorBarsUtils.ConvertColorbarToTexture(VizParamCollection.ActiveAttribute.Colorbar);
                }
            }
            set => _texture = value;
        }
        public Dictionary<string, Texture2D> TextureDict => _liquidEarthTexturedSurface.TextureDict;



        public Mesh UnityMesh
        {
            get
            {
                if (_unityMesh == null) UpdateUnityMesh();
                return _unityMesh;
            }

            set => _unityMesh = value;
        }
        
        public GemPlayStaticMeshData(LiquidEarthTexturedSurface liquidEarthTexturedSurface, List<int[]> cellSubMeshes)
        {
            _liquidEarthTexturedSurface = liquidEarthTexturedSurface;
            this._cellSubMeshes         = cellSubMeshes;
        }
        
        public void UpdateUnityMesh()
        {
            _meshSimplifier = MeshSimplifier(_liquidEarthTexturedSurface, _cellSubMeshes, IsTexture);

            _unityMesh      = _meshSimplifier.ToMesh();
            _unityMesh.name = $"Mesh 1";
        }
        
        private static MeshSimplifier MeshSimplifier(LiquidEarthTexturedSurface leSurface, List<int[]> cellSubMesh, bool isTexture)
        {
            var liquidEarthMesh    = leSurface.Mesh;
            var vizParamCollection = leSurface.VisualizationParamCollection;

            MeshSimplifier meshSimplifier;
            {
                meshSimplifier = new MeshSimplifier
                {
                    Vertices = liquidEarthMesh.VerticesGame // ! Here gets applied the space extent scaling
                };

                // Optional fields
                {
                    if (leSurface.Uvs != null)
                    {
                        meshSimplifier.UV1 = leSurface.Uvs;
                    }
                    else if (isTexture)
                    {
                        meshSimplifier.UV1 = GenerateUvsToMapWholeTextureToMesh(leSurface);
                    }
                    else if (liquidEarthMesh.VertexAttributes != default)
                    {
                        var vertexIdArray = liquidEarthMesh.VertexAttributes.FirstOrDefault().Value;
                        meshSimplifier.UV1 = ColorBarsUtils.CreateGradientTextureUVs(vizParamCollection.ActiveAttribute,
                            liquidEarthMesh.NumberVertex, vertexIdArray);
                    }

                    if (leSurface.Normals != null)
                        meshSimplifier.Normals = leSurface.Normals;
                }

                // Reverse normals
                for (int j = 0; j < cellSubMesh.Count; j++) cellSubMesh[j] = cellSubMesh[j].Reverse().ToArray();

                meshSimplifier.AddSubMeshTriangles(cellSubMesh.ToArray());

                // Generate actual mesh
                meshSimplifier.SimplifyMesh(1);
            }
            
            return meshSimplifier;
            
            static Vector2[] GenerateUvsToMapWholeTextureToMesh(LiquidEarthTexturedSurface leSurface)
            {
                var liquidEarthMesh = leSurface.Mesh;
                var uvs             = new Vector2[liquidEarthMesh.NumberVertex];
                var verticesGame    = liquidEarthMesh.VerticesGame;
                var minX            = verticesGame.Min(v => v.x);
                var maxX            = verticesGame.Max(v => v.x);
                var minZ            = verticesGame.Min(v => v.z);
                var maxZ            = verticesGame.Max(v => v.z);
                for (int i = 0; i < uvs.Length; i++)
                {
                    var normalizedX = (verticesGame[i].x - minX) / (maxX - minX);
                    var normalizedZ = (verticesGame[i].z - minZ) / (maxZ - minZ);

                    uvs[i] = new Vector2(normalizedX, normalizedZ);
                }

                return uvs;
            }
        }
    }
}