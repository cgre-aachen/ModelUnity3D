using System.Collections.Generic;

namespace GemPlay.Modules.TexturedMeshSpawner
{
    public static class MeshSpawnerUtils
    {
        internal static List<int> FindIndexEachLayerFromValues(IReadOnlyList<float> idArray,  int multiplier = 1)
        {
            var previousIdInit = 1;
            var argsList = new List<int>() {0};
            
            for (var index = 0; index < idArray.Count; index++)
            {
                var currentId = (int) idArray[index];
                if (currentId != previousIdInit)
                {
                    argsList.Add( (index) * multiplier);
                }

                previousIdInit = currentId;
            }

            argsList.Add(idArray.Count * multiplier ); // Add Last one
            return argsList;
        }
        
        internal static float[] GenerateVertexIdFromCellId(int vertexArrayLength, float[] cellIdArray, int[] cellArray)
        {
            var vertexIdArray = new float[vertexArrayLength];
            for (var i = 0; i < cellArray.Length; i++)
            {
                var cell = cellArray[i];
                vertexIdArray[cell] = cellIdArray[i/3];
            }

            return vertexIdArray;
        }
        
        
    }
}