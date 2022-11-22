using System;
using GemPlay.Core.Data.LiquidEarth;
using UnityEngine;

namespace GemPlay.Core.Helpers
{
    public static class ColorBarsUtils
    {
        public static Texture2D ConvertColorbarToTexture(Gradient colorbar, int width = 512, int height = 2) 
            // Note: This method has to be tested in PlayMode
        {
            var gradTex = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var time =  x / (float) width;
                    Color col = colorbar.Evaluate(time);
                    gradTex.SetPixel(x, y, col);
                }
            }
            gradTex.Apply();
            return gradTex;
        }
        
        public static Texture2D ConvertColorToTexture(Color color, int width = 512, int height = 2) 
            // Note: This method has to be tested in PlayMode
        {
            var gradTex = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    gradTex.SetPixel(x, y, color);
                }
            }
            gradTex.Apply();
            return gradTex;
        }
        
        public static Vector2[] CreateGradientTextureUVs(AttributeVisualizationParam visAttrParams,
            int verticesLenght, float propertyValue)
        {
            var uvArray = new Vector2[verticesLenght];
            for (int i = 0; i < verticesLenght; i++)
            {
                var normalizePropertyValue = (propertyValue - visAttrParams.ColorMin) / 
                                             (visAttrParams.ColorMax - visAttrParams.ColorMin); 
                uvArray[i] = new Vector2(normalizePropertyValue, 0);
            }
            return uvArray;
        }
        
        public static Vector2[] CreateGradientTextureUVs(AttributeVisualizationParam visAttrParams,
            int verticesLenght, float[] propertyValue)
        {
            var uvArray = new Vector2[verticesLenght];
            for (int i = 0; i < verticesLenght; i++)
            {
                var normalizePropertyValue = (propertyValue[i] - visAttrParams.ColorMin) / 
                                             (visAttrParams.ColorMax - visAttrParams.ColorMin);
                if (float.IsNaN(normalizePropertyValue)) normalizePropertyValue = 0;
                uvArray[i] = new Vector2(normalizePropertyValue, 0);
            }
            return uvArray;
        }
        
        
        
        
        public static Color32 EvaluateColor(Gradient colorbar, float value, float colorMin, float colorMax) 
        {
           Color32 color = colorbar.Evaluate((value - colorMin ) / (colorMax - colorMin));
            return color;
        }
    }

    public static class GemPlayColorbars
    {
        public static Gradient Jet()
        {
            var cb= new Gradient();
            
            // Populate the color keys at the relative time 0 and 1 (0 and 100%)
            var colorKey = new GradientColorKey[6];
            colorKey[0].color = Color.red;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.yellow;
            colorKey[1].time = 0.2f;
            colorKey[2].color = Color.green;
            colorKey[2].time = 0.4f;
            colorKey[3].color = Color.cyan;
            colorKey[3].time = 0.6f;
            colorKey[4].color = Color.blue;
            colorKey[4].time = 0.8f;
            colorKey[5].color = Color.magenta;
            colorKey[5].time = 1.0f;

            // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
            var alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1.0f;

            cb.SetKeys(colorKey, alphaKey);

            return cb;
        }

        public static Gradient Viridis()
        {

            var colorKey = new GradientColorKey[8];
            colorKey[0].color = new Color(0.267004f, 0.004874f, 0.329415f);       
            colorKey[1].color = new Color( 0.275191f, 0.194905f, 0.496005f);  
            colorKey[2].color = new Color(0.212395f, 0.359683f, 0.55171f);  
            colorKey[3].color = new Color(0.153364f, 0.497f,    0.557724f);  
            colorKey[4].color = new Color(0.122312f, 0.633153f, 0.530398f);  
            colorKey[5].color = new Color(0.288921f, 0.758394f, 0.428426f);  
            colorKey[6].color = new Color(0.626579f, 0.854645f, 0.223353f);  
            colorKey[7].color = new Color(0.993248f, 0.906157f, 0.143936f);

            for (int i = 0; i < colorKey.Length; i++) //set 8 equally spread keys
            {
                colorKey[i].time =(float) i / 7f;
            }
            
            var alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1.0f;
            
            var gradient= new Gradient();
            gradient.SetKeys(colorKey,alphaKey);

            return gradient;
        }
    }
    
}