using System.Collections.Generic;
using System.Linq;
using GemPlay.Core.Helpers;
using LiquidGemPy.Core.LiquidEarth;
using UnityEngine;
using Mesh = LiquidGemPy.Core.LiquidEarth.Mesh;

namespace GemPlay.Core.Data.LiquidEarth
{ 
    public class AttributeVisualizationParam
    {
        public Gradient Colorbar = GemPlayColorbars.Viridis();
        public float ColorMin; //define the stretch of the colorbar over the attribute values
        public float ColorMax;
        
        
        public AttributeVisualizationParam(string attrName, Mesh mesh)
        {
            // create a default gradient
            
            // * Default min and max are stretched to the value Range of the selected Attribute
            // * attrName can either be in cell or vertex attributes 
            if (mesh.CellAttributes != null && mesh.CellAttributes.ContainsKey(attrName))
                GetColorMinMax(mesh.CellAttributes[attrName]);
            else if (mesh.VertexAttributes != null && mesh.VertexAttributes.ContainsKey(attrName))
                GetColorMinMax(mesh.VertexAttributes[attrName]);
            else Debug.Log("Key was not found in cell or vertex Attributes");

            
            void GetColorMinMax(float[] attributes)
            {
                // Remove NaN values efficiently
                var values = attributes.Where(x => !float.IsNaN(x)).ToArray();

                this.ColorMin = values.Min();
                this.ColorMax = values.Max();
            }
        }
        
        public Color32 EvaluateColor(float value) 
        {   
            // Make black if value is NaN
            if (float.IsNaN(value)) return Color.gray;
            Color32 color =this.Colorbar.Evaluate((value - this.ColorMin ) / (this.ColorMax -this.ColorMin));
            return color;
        }

        public Texture2D ConvertColorbarToTexture(int width = 512, int height = 2) 
        // Note: This method has to be tested in PlayMode
        {
            var gradTex = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var time =  (float)x / (float)width;
                    Color col = this.Colorbar.Evaluate(time);
                    gradTex.SetPixel(x, y, col);
                }
            }
            
            gradTex.Apply();
            return gradTex;
        }
    }
    public class VizParamCollection
    {
        public readonly Dictionary<string, AttributeVisualizationParam> Parameters = new Dictionary<string, AttributeVisualizationParam>();
        public string ActiveAttributeName;
        public AttributeVisualizationParam ActiveAttribute => Parameters[ActiveAttributeName];

        public VizParamCollection(Mesh mesh) 
        {
            if (mesh.VertexAttributes != null)
                foreach (var name in mesh.VertexAttributes.Keys)
                {
                    var value = new AttributeVisualizationParam(name, mesh);
                    Parameters.Add(name, value);
                }

            if (mesh.CellAttributes != null)
                foreach (var name in mesh.CellAttributes.Keys)
                {
                    if (Parameters.ContainsKey(name)) continue;
                    var value = new AttributeVisualizationParam(name, mesh);
                    Parameters.Add(name, value);
                }

            ActiveAttributeName = Parameters.Keys.FirstOrDefault(); //set first cell attribute as default for rendering
        }
    }
}