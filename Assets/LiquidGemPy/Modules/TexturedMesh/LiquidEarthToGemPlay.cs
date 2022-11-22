using System.Collections.Generic;
using System.Linq;
using GemPlay.Core.Data.LiquidEarth;
using LiquidGemPy.Core;
using LiquidGemPy.Core.LiquidEarth;
using static GemPlay.Modules.TexturedMeshSpawner.MeshSpawnerUtils;

namespace GemPlay.Modules.TexturedMesh
{
    struct MeshProperties
    {
        public bool IsVertexIdArray;
        public bool IsCellsIdArray;
        public List<int> IdxVertex;
        public bool SameNumberOfGroupsForCellsAndVertex;
        public List<int> IdxCells;
    }
    
    
    internal static class LiquidEarthToGemPlay
    {
        // TODO: Move this to a config file
        private const int MaxNumberOfTriangles = 2_500_000 * 3; // * This is to split meshes into multiple gameobjects. We also have a split of mesh into multiple submeshes
        public static List<GemPlayStaticMeshData> ToTexturedMeshItem(LiquidEarthTexturedSurface liquidEarthTexturedSurface)
        {
            // Decide if split the mesh?
            var meshAnalysis = MeshAnalysis(liquidEarthTexturedSurface.Mesh);
            
            var idxCells = meshAnalysis.IdxCells;
            if (idxCells.Last() > MaxNumberOfTriangles)
                idxCells = new List<int>() {0, MaxNumberOfTriangles};
            
            var listGemPlayStaticMeshes = MeshSplitter.SplitLiquidEarthMeshesIntoMultipleGO(liquidEarthTexturedSurface, idxCells);
            
            return listGemPlayStaticMeshes;
        }

        private static MeshProperties MeshAnalysis(Mesh mesh)
        {
            // Check Attributes configuration (attr are optional and therefore each data set may come a bit different)
            float[] vertexIdArray;
            float[] cellIdArray;
            
            var isVertexIdArray =  mesh.VertexAttributes != default;
            var isCellsIdArray = mesh.CellAttributes != default;

            List<int> idxVertex = new List<int> {0, mesh.NumberVertex};;
            List<int> idxCells = new List<int> {0, mesh.Cells.Length};
            bool sameNumberOfGroupsForCellsAndVertex = false;
            
            if (!isVertexIdArray & isCellsIdArray)
            {
                // TODO: Now we do this in LiquidEarthMesh So I guess we never get into this branch.
                //Generate VertexAttributes From Cell
                {
                    cellIdArray = mesh.CellAttributes.FirstOrDefault().Value;
                    vertexIdArray = GenerateVertexIdFromCellId(mesh.NumberVertex, cellIdArray, mesh.Cells);
                    mesh.VertexAttributes = new SortedDictionary<string, float[]>() { { "Generated", vertexIdArray } };
                }
                
                // Try to split the mesh into multiple meshes by ID
                {
                    var _idxVertex = FindIndexEachLayerFromValues(vertexIdArray);
                    var _idxCells = FindIndexEachLayerFromValues(cellIdArray, multiplier: 3);
                    sameNumberOfGroupsForCellsAndVertex = _idxCells.Count != _idxVertex.Count;

                    if (sameNumberOfGroupsForCellsAndVertex & _idxCells.Count < 50)
                    {
                        idxVertex = _idxVertex;
                        idxCells = _idxCells;
                    }
                }
            }
            else if (isVertexIdArray & isCellsIdArray)
            {
                // * Try to split the mesh into multiple meshes by ID
                vertexIdArray = mesh.VertexAttributes.FirstOrDefault().Value;
                cellIdArray = mesh.CellAttributes.FirstOrDefault().Value;
                var _idxVertex = FindIndexEachLayerFromValues(vertexIdArray);
                var _idxCells = FindIndexEachLayerFromValues(cellIdArray, multiplier: 3);
                sameNumberOfGroupsForCellsAndVertex = _idxCells.Count == _idxVertex.Count;
                if (sameNumberOfGroupsForCellsAndVertex)
                {
                    idxVertex = _idxVertex;
                    idxCells = _idxCells;
                }
            }
            
            var meshProperties = new MeshProperties
            {
                IsVertexIdArray = isVertexIdArray,
                IsCellsIdArray = isCellsIdArray,
                IdxVertex = idxVertex,
                IdxCells = idxCells,
                SameNumberOfGroupsForCellsAndVertex = sameNumberOfGroupsForCellsAndVertex,
            };
            return meshProperties;
        }
    }
}