using LiquidGemPy.Core;
using Newtonsoft.Json;

namespace Gempy
{
    public class OrientationPointSchema
    {
        private float[][] _orientationPointCoords = null;

        [JsonProperty("dip_positions")]
        public float[][] DipPositions
        {
            get => _orientationPointCoords ?? InterpolationInput.AllOrientationsCoordinates;
            set => _orientationPointCoords = value;
        }

        private float[][] _orientationPointDip = null;

        [JsonProperty("dip_gradients")]
        public float[][] DipGradients
        {
            get => _orientationPointDip ?? InterpolationInput.AllOrientationsGradients;
            set => _orientationPointDip = value;
        }
    }
}