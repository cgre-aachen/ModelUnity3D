using LiquidGemPy.Core;
using Newtonsoft.Json;

namespace Gempy
{
    public class SurfacePointsSchema
    {
        private float[][] _surfacePointsCoords = null;
        [JsonProperty("sp_coords")] public float[][] SurfacePointsCoords
        {
            get => _surfacePointsCoords ?? InterpolationInput.AllSurfacePointsCoordinates;
            set => _surfacePointsCoords = value;
        }
    }
}