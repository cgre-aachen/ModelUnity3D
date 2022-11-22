using System;
using System.Collections.Generic;
using System.Linq;
using GemPlay.Core.Data.LiquidEarth;
using UnityEngine;

namespace LiquidGemPy.Core.LiquidEarth
{
    /// <summary>
    /// This class contains Vertex and cells and may have Cell/Vertex attributes. Uvs and normals may be in the attributes
    /// but they will get extracted to the TexturedSurface object during the first parsing steps. Textures always have to
    /// be in a different file.
    /// </summary>
    public class Mesh //for Boreholes, Profile Meshes, etc. unstructured grids 
    {
        private bool _swapYZAxis = true;

        # region Data

        private Vector3[]                         _verticesWorld;
        private Vector3[]                         _verticesGame;
        public  int[]                             Cells;
        public  SortedDictionary<string, float[]> CellAttributes   = default;
        public  SortedDictionary<string, float[]> VertexAttributes = default;

        # endregion

        public List<string> CellAttributesNames => CellAttributes?.Keys.ToList();
        public List<string> VertexAttributesNames => VertexAttributes?.Keys.ToList();

        public Vector3[] Vertices => _verticesWorld;

        public Vector3[] VerticesWorld
        {
            get => _verticesWorld;
            private set
            {
                _verticesWorld = value;
                _verticesGame  = _swapYZAxis ? SwapYZAxis(ref _verticesWorld) : _verticesWorld;
            }
        }

        public Vector3[] VerticesGame
        {
            get => _verticesGame;
            set
            {
            //    _verticesGame  = ScaleAndShiftInverted(value);
                _verticesWorld = SwapYZAxis(ref _verticesGame);
            }
        }

        public int NumberVertex => VerticesWorld.Length;
        private readonly int _cellsDim;


        public Mesh(
            Vector3[] verticesWorld, int[] cells,
            SortedDictionary<string, float[]> cellAttributes = default,
            SortedDictionary<string, float[]> vertexAttributes = default,
            bool swapYZAxis = true)
        {
            _swapYZAxis = swapYZAxis;

            VerticesWorld = verticesWorld;
            Cells         = cells;

            CheckAndSetDefaultAttributesWhenNeeded(verticesWorld.Length, cellAttributes, vertexAttributes);
        }

        /// <summary>
        /// Parse Bytes Fortran order
        /// </summary>
        public Mesh(LiquidEarthMeshHeader header, byte[] bytes, bool swapYZAxis = true)
        {
            _swapYZAxis = swapYZAxis;
            _cellsDim   = header.CellShape[1];

            var offset = 0;
            offset = ParseVertices(header, bytes, offset);
            offset = ParseCells(header, bytes, offset);
            offset = ParseCellAttrToDict(header, bytes, offset); //This is for continuous arrays per attribute
            ParseVertexAttrToDict(header, bytes, offset);
            CheckAndSetDefaultAttributesWhenNeeded(NumberVertex, CellAttributes, VertexAttributes);
        }

        private void CheckAndSetDefaultAttributesWhenNeeded(int nVertex, SortedDictionary<string, float[]> cellAttributes,
            SortedDictionary<string, float[]> vertexAttributes)
        {
            var isVertexIdArray = vertexAttributes != default;
            var isCellsIdArray  = cellAttributes != default;
            var noAttributes    = !isVertexIdArray && !isCellsIdArray;

            if (noAttributes)
            {
                var defaultValues = new float[nVertex];
                for (var i = 0; i < defaultValues.Length; i++)
                {
                    defaultValues[i] = 1;
                }

                VertexAttributes = new SortedDictionary<string, float[]>() { { "Default", defaultValues } };
            }
            else if (isCellsIdArray && isVertexIdArray)
            {
                CellAttributes   = cellAttributes;
                VertexAttributes = vertexAttributes;
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            else if (!isCellsIdArray && isVertexIdArray)
            {
                VertexAttributes = vertexAttributes;
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            else if (isCellsIdArray && !isVertexIdArray)
            {
                VertexAttributes = new SortedDictionary<string, float[]>();
                foreach (var key in cellAttributes.Keys)
                {
                    var cellIdArray                = cellAttributes[key];
                    var generateVertexIdFromCellId = GenerateVertexIdFromCellId(NumberVertex, cellIdArray, Cells, _cellsDim);
                    VertexAttributes[key] = generateVertexIdFromCellId;
                }
            }

            static float[] GenerateVertexIdFromCellId(int vertexArrayLength, float[] cellIdArray, int[] cellArray,
                int cellsDim)
            {
                var vertexIdArray = new float[vertexArrayLength];
                for (var i = 0; i < cellArray.Length; i++)
                {
                    var cell = cellArray[i];
                    vertexIdArray[cell] = cellIdArray[i / cellsDim];
                }

                return vertexIdArray;
            }
        }


        // private Vector3[] ScaleAndShift(Vector3[] verticesGame)
        // {
        //     var modifierTransform = ContentMeta.GetCombinedDataModifier();
        //     var transformedVertices = ModifierBase.TranslateAndScale(
        //         vertices: verticesGame,
        //         shift:modifierTransform.Position,
        //         scale: modifierTransform.Scale.x);
        //     
        //     return transformedVertices;
        //
        // }
        //
        // private Vector3[] ScaleAndShiftInverted(Vector3[] verticesGame)
        // {
        //     var modifierTransform   = ContentMeta.GetCombinedDataModifier();
        //     var transformedVertices = ModifierBase.TranslateAndScaleInverted(
        //         vertices: verticesGame,
        //         shift:modifierTransform.Position,
        //         scale: modifierTransform.Scale.x);
        //     return transformedVertices;
        // }

        private static Vector3[] SwapYZAxis(ref Vector3[] verticesWorld)
        {
            var flipAxis = new Vector3(1, -1, 1);

            var rotation     = Quaternion.Euler(90, 0, 0);
            var verticesGame = new Vector3[verticesWorld.Length];
            for (int i = 0; i < verticesWorld.Length; i++)
            {
                //Apply rotation of parent
                var rotatedCoord = rotation * verticesWorld[i];
                verticesGame[i] = Vector3.Scale(rotatedCoord, flipAxis);
            }

            return verticesGame;
        }

        private static Vector3[] SwapYZAxis2(ref Vector3[] verticesWorld)
        {
            var verticesGame = new Vector3[verticesWorld.Length];
            for (int i = 0; i < verticesWorld.Length; i++)
            {
                verticesGame[i].x = verticesWorld[i].x;
                verticesGame[i].y = verticesWorld[i].z;
                verticesGame[i].z = verticesWorld[i].y;
            }

            return verticesGame;
        }


        private void ParseVertexAttrToDict(LiquidEarthMeshHeader header, byte[] bytes, int offset)
        {
            // NOTE: parse vertex attributes to Dictionary 

            if (header.VertexAttrShape == null || header.VertexAttrShape[0] == 0 || header.VertexAttrShape[1] == 0)
                return;

            VertexAttributes = new SortedDictionary<string, float[]>();
            for (int i = 0; i < header.VertexAttrShape[1]; i++)
            {
                var array = new float[header.VertexAttrShape[0]];

                for (int j = 0; j < header.VertexAttrShape[0]; j++)
                {
                    array[j] =  BitConverter.ToSingle(bytes, offset);
                    offset   += 4;
                }

                VertexAttributes.Add(header.VertexAttrNames[i], array);
            }
        }

        private int ParseCellAttrToDict(LiquidEarthMeshHeader header, byte[] bytes, int offset)
        {
            if (header.CellAttrShape == null || header.CellAttrShape[0] == 0 || header.CellAttrShape[1] == 0) return offset;

            CellAttributes = new SortedDictionary<string, float[]>();
            for (int i = 0; i < header.CellAttrShape[1]; i++)
            {
                var array = new float[header.CellAttrShape[0]];

                for (int j = 0; j < header.CellAttrShape[0]; j++)
                {
                    array[j] =  BitConverter.ToSingle(bytes, offset);
                    offset   += 4;
                }

                CellAttributes.Add(header.CellAttrNames[i], array);
            }

            return offset;
        }

        private int ParseCells(LiquidEarthMeshHeader header, byte[] bytes, int offset)
        {
            var dataWidth  = header.CellShape[0];
            var dataHeight = header.CellShape[1];
            var cellData   = new int[dataHeight, dataWidth];

            Buffer.BlockCopy(bytes, offset, cellData, 0, 4 * dataWidth * dataHeight);
            offset += 4 * dataWidth * dataHeight;

            Cells = new int[dataWidth * dataHeight];
            for (int i = 0; i < dataWidth; i++)
            {
                for (int j = 0; j < dataHeight; j++)
                    Cells[i * dataHeight + j] = cellData[j, i];
            }

            return offset;
        }

        private int ParseVertices(LiquidEarthMeshHeader header, byte[] bytes, int offset)
        {
            var dataWidth  = header.VertexShape[0];
            var dataHeight = header.VertexShape[1];
            var data       = new float[dataHeight, dataWidth];

            Buffer.BlockCopy(bytes, offset, data, 0, 4 * dataWidth * dataHeight);
            offset += 4 * dataWidth * dataHeight;

            var vertexParser = new Vector3[dataWidth];
            for (int i = 0; i < dataWidth; i++)
            {
                vertexParser[i].x = data[0, i];
                vertexParser[i].y = data[1, i];
                vertexParser[i].z = data[2, i];
            }

            VerticesWorld = vertexParser;
            return offset;
        }
    }
}