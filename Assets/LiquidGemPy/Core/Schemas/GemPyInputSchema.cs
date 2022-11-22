using Newtonsoft.Json;

namespace Gempy
{
    public class GemPyInputSchema
    {
        [JsonProperty("interpolation_input")] public InterpolatorInputSchema InterpolatorInputSchema = new();
        [JsonProperty("input_data_descriptor")] public InputDataDescriptorSchema InputDataDescriptorSchema = new ();
    }
    
}