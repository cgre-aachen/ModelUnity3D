using System.Collections.Generic;
using System.Linq;
using LiquidGemPy.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace Gempy
{
    public class InputDataDescriptorSchema
    {
        private List<SurfacePoint> AllSurfacePoints => InterpolationInput.SurfacePoints;
        private List<Orientation> AllOrientations => InterpolationInput.Orientations;

        [JsonProperty("number_of_points_per_surface")]
        public int[] NumberOfPointsPerSurface => CountElementsInGroups(AllSurfacePoints.GroupBy(j => j.SurfaceId));

        [JsonProperty("number_of_points_per_stack")]
        public int[] NumberOfPointsPerStack => CountElementsInGroups(AllSurfacePoints.GroupBy(j => j.Series));

        [JsonProperty("number_of_surfaces_per_stack")]
        public int[] NumberOfSurfacesPerStack => CountNumberOfSurfacesInGroups(AllSurfacePoints.GroupBy(j => j.Series));

        [JsonProperty("number_of_orientations_per_stack")]
        public int[] NumberOfOrientationsPerStack => CountElementsInGroups(AllOrientations.GroupBy(j => j.Series));

        [JsonProperty("masking_descriptor")] public int[] MaskingDescriptor = {1, 1};


        private static int[] CountNumberOfSurfacesInGroups(IEnumerable<IGrouping<int, SurfacePoint>> group)
        {
            var result = new int[group.Count()];
            var i      = 0;
            foreach (var g in group)
            {
                result[i] = g.Select(j => j.SurfaceId).Distinct().Count();
                i++;
            }

            return result;
        }

        private static int[] CountElementsInGroups(IEnumerable<IGrouping<int, InputPoint>> groups)
        {
            var i      = 0;
            var result = new int[groups.Count()];

            foreach (var group in groups)
            {
                result[i] = group.Count();
                i++;
                // Debug.Log($"Series {group.Key}: {group.Count()} times");
            }

            return result;
        }
    }
}