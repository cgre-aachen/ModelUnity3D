using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiquidGemPy.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace GemPlay.Modules.TexturedMesh
{
    internal static class TexturedMeshSpawner
    {
        private static readonly Material   TexturedMeshMaterial;
        private static readonly GameObject TexturedMeshPrefab;
        private static readonly int        BaseMap = Shader.PropertyToID("_BaseMap");
        private static readonly int        MainTex = Shader.PropertyToID("_MainTex");

        static TexturedMeshSpawner()
        {
            TexturedMeshMaterial = GemPlayLoader.Addressable<Material>("TexturedMeshMaterial.mat");
            TexturedMeshPrefab   = GemPlayLoader.Addressable<GameObject>("TexturedMesh.prefab");
        }

        public static async Task SpawnTexturedMesh(GemPlayStaticMeshData gemPlayMesh, int meshNumber = default)
        {
            var makeDoubleSided = false;

            var gameObject = Object.Instantiate(TexturedMeshPrefab);
            gameObject.name = $"TexturedMesh_{meshNumber}";

            var helper       = gameObject.GetComponent<GameObjects.TexturedMeshHelper>();
            var meshFilter   = helper.meshFilter;
            var meshRenderer = helper.meshRenderer;
            var meshCollider = helper.meshCollider;


            var doubleSided = false;
            if (makeDoubleSided)
            {
                gemPlayMesh.UnityMesh = DoubleSidedMesh(gemPlayMesh.UnityMesh);
                doubleSided           = true;
            }

            var unityMesh = gemPlayMesh.UnityMesh;

            await Task.Yield();

            meshFilter.sharedMesh = unityMesh;

            {
                var meshMaterials = new List<Material>();

                var textureKeys             = gemPlayMesh.TextureDict.Keys.ToList();
                var nMaterials              = unityMesh.subMeshCount;
                //if (doubleSided) nMaterials -= 1; // * We don't want to put a material on the double face mesh

                for (int i = 0; i < nMaterials; i++)
                {
                    var texture = GetTextureFromGemPlayContent(gemPlayMesh, textureKeys, i);
                    if (texture == null) continue;

                    var textureName = texture.name.Split('/').Last();
                    var material = new Material(TexturedMeshMaterial)
                    {
                        name = "TexturedMeshMaterial_" + textureName,
                    };
                    material.SetTexture(BaseMap, texture);
                    material.SetTexture(MainTex, texture);
                    meshMaterials.Add(new Material(material));

                    await Task.Yield();
                }

                meshRenderer.sharedMaterials = meshMaterials.ToArray();
            }

            meshFilter.sharedMesh.RecalculateNormals();
            await Task.Yield();

            static Texture2D GetTextureFromGemPlayContent(GemPlayStaticMeshData gemPlayMesh, List<string> textureKeys, int i)
            {
                Texture2D texture;
                var       unityMesh = gemPlayMesh.UnityMesh;

                if (gemPlayMesh.IsTexture)
                    if (unityMesh.subMeshCount == gemPlayMesh.TextureDict.Count)
                    {
                        texture = gemPlayMesh.TextureDict[textureKeys[i]];
                    }
                    else if (gemPlayMesh.TextureDict.Count == 1)
                    {
                        texture = gemPlayMesh.TextureDict[textureKeys[0]];
                    }
                    else
                    {
                        Debug.LogError("Texture Dictionary does not match mesh submesh count");
                        texture = null;
                    }
                else
                    texture = gemPlayMesh.Texture;

                return texture;
            }
        }

        private static Mesh DoubleSidedMesh(Mesh mesh)
        {
            // See https://storyprogramming.com/2019/04/23/double-sided-mesh-generator/
            var invertedMesh = InvertMesh(mesh); // This merege all submeshes in one Inverted
            return CombineMeshes(mesh, invertedMesh);

            static Mesh CombineMeshes(Mesh mesh0, Mesh mesh1)
            {
                var combineInstances = new CombineInstance[mesh0.subMeshCount + mesh1.subMeshCount];
                // TODO: Mesh 0 has to be combined for each submesh
                for (var i = 0; i < mesh0.subMeshCount; i++)
                {
                    combineInstances[i].mesh         = mesh0;
                    combineInstances[i].subMeshIndex = i;
                    combineInstances[i].transform    = Matrix4x4.identity;
                }

                for (var i = 0; i < mesh1.subMeshCount; i++)
                {
                    combineInstances[i + mesh0.subMeshCount].mesh         = mesh1;
                    combineInstances[i + mesh0.subMeshCount].subMeshIndex = i;
                    combineInstances[i + mesh0.subMeshCount].transform    = Matrix4x4.identity;
                }

                var combinedMesh = new Mesh() { indexFormat = IndexFormat.UInt32 };
                combinedMesh.CombineMeshes(combineInstances, false);

                return combinedMesh;
            }

            static Mesh InvertMesh(Mesh mesh)
            {
                var newNormals          = new Vector3[mesh.normals.Length];
                var previousMeshNormals = mesh.normals;
                for (var i = 0; i < previousMeshNormals.Length; i++)
                {
                    newNormals[i] = -previousMeshNormals[i];
                }

                var newTangents          = new Vector4[mesh.tangents.Length];
                var previousMeshTangents = mesh.tangents;
                for (var i = 0; i < previousMeshTangents.Length; i++)
                {
                    //  newTangents[i] = previousMeshTangents[i];
                    newTangents[i].w = -previousMeshTangents[i].w;
                }

                var invertedMesh = new Mesh
                {
                    indexFormat = mesh.indexFormat,
                    name        = "Inverted " + mesh.name,
                    vertices    = mesh.vertices,
                    triangles   = mesh.triangles.Reverse().ToArray(),
                    normals     = newNormals,
                    tangents    = newTangents,
                    uv          = mesh.uv
                };

                return invertedMesh;
            }
        }
    }
}