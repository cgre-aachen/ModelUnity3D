using LiquidGemPy.Core;
using Newtonsoft.Json;

namespace Gempy
{
    public class InterpolatorInputSchema
    {
        [JsonProperty("surface_points")] public SurfacePointsSchema    SurfacePointsSchema    = new ();
        [JsonProperty("orientations")]   public OrientationPointSchema OrientationPointSchema = new ();
        [JsonProperty("grid")]           public GridSchema             GridSchema              = null;
    }
}