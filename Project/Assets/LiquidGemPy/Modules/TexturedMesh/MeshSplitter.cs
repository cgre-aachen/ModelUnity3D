using System;
using System.Collections.Generic;
using GemPlay.Core.Data.LiquidEarth;
using LiquidGemPy.Core;

namespace GemPlay.Modules.TexturedMesh
{
    internal static class MeshSplitter
    {
        private const int SubMeshTri = 1_000_000 * 3; // TODO: Move this to a config file
        
        // ReSharper disable once InconsistentNaming
        internal static List<GemPlayStaticMeshData> SplitLiquidEarthMeshesIntoMultipleGO(
            LiquidEarthTexturedSurface liquidEarthTexturedSurface,  List<int> idxCells)
        {
            var idxCellsCount = idxCells.Count;

            var listGemPlayStaticMeshes = new List<GemPlayStaticMeshData>();

            for (var i = 0; i < idxCellsCount - 1; i++)
            {
                var gemPlayStaticMeshData = ExtractMeshItemFromCellsIds(i, idxCells, liquidEarthTexturedSurface);
                listGemPlayStaticMeshes.Add(gemPlayStaticMeshData);
            }

            return listGemPlayStaticMeshes;
        }

        private static GemPlayStaticMeshData ExtractMeshItemFromCellsIds(int i, List<int> idxCells,
            LiquidEarthTexturedSurface liquidEarthTexturedSurface)
        {
            
            // Extract cell
            List<int[]> cellSubMeshes = GetCellSubmMesh(i, idxCells, liquidEarthTexturedSurface);
            
            var gemplayStaticMesh = new GemPlayStaticMeshData(liquidEarthTexturedSurface, cellSubMeshes);

            return gemplayStaticMesh;
        }

        private static List<int[]> GetCellSubmMesh(int i, List<int> idxCells, LiquidEarthTexturedSurface liquidEarthTexturedSurface)
        {
            var liquidEarthMesh = liquidEarthTexturedSurface.Mesh;
            
            List<int[]> cellSubMesh;
            {
                if (liquidEarthTexturedSurface.CellSubMesh != null)
                    cellSubMesh = liquidEarthTexturedSurface.CellSubMesh;
                else
                {
                    var cellArraySingleMesh = new int[(idxCells[i + 1] - idxCells[i])];
                    var cellArray = liquidEarthMesh.Cells;

                    var nCells = idxCells[i + 1] - idxCells[i];
                    Array.Copy(cellArray, idxCells[i], cellArraySingleMesh, 0, nCells);

                    // Create submesh
                    var subMeshIndexRange = SubMeshIndexRange(nCells);
                    cellSubMesh = SubMeshCellsArray(subMeshIndexRange, cellArraySingleMesh);
                }
            }
            
            return cellSubMesh;
            
            
            static List<int> SubMeshIndexRange(int nCells)
            {
                var subMesh = new List<int>();
                var totalTri = nCells;
                var nSubMesh = totalTri / SubMeshTri + 1;
                for (int z = 0; z < nSubMesh; z++)
                {
                    subMesh.Add(SubMeshTri * z);
                }

                subMesh.Add(totalTri);
                return subMesh;
            }

            static List<int[]> SubMeshCellsArray(List<int> subMeshIndexRangeInner, int[] cellArrayFullMesh)
            {
                var subMeshCellsArray = new List<int[]>();

                for (int j = 0; j < subMeshIndexRangeInner.Count - 1; j++)
                {
                    var nSubCells = subMeshIndexRangeInner[j + 1] - subMeshIndexRangeInner[j];
                    var subcellArraySingleMesh = new int[nSubCells];
                    Array.Copy(cellArrayFullMesh, subMeshIndexRangeInner[j], subcellArraySingleMesh, 0, nSubCells);
                    subMeshCellsArray.Add(subcellArraySingleMesh);
                }

                return subMeshCellsArray;
            }
        }
    }
}