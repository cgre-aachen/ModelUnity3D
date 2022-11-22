using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LiquidGemPy.Core
{
    // * This class may become non-static eventually 
    public static class InterpolationInput
    {
        public static readonly List<SurfacePoint> SurfacePoints = new();
        public static readonly List<Orientation>  Orientations  = new();
        public static readonly List<SurfaceStack> SurfaceStack  = new();

        public static float[][] AllSurfacePointsCoordinates => GrabCoords(SurfacePoints);
        public static float[][] AllOrientationsCoordinates  => GrabCoords(Orientations, true);
        public static float[][] AllOrientationsGradients  => GrabGrad(Orientations, false);

        private static float[][] GrabGrad(List<Orientation> allOrientations, bool swapYZ = true)
        {
            var coords = new float[allOrientations.Count][];
            for (var i = 0; i < allOrientations.Count; i++)
            {
                Orientation point    = allOrientations[i];
                var azimuthDipVector = point.GradientVector;
                coords[i] = swapYZ ? new[] {azimuthDipVector.x, azimuthDipVector.z, azimuthDipVector.y} : new[] {azimuthDipVector.x, azimuthDipVector.y, azimuthDipVector.z};
            }
            return coords;
        }

        private static float[][] GrabCoords<T>(List<T> allInputPoints, bool swapYZ = true ) where T : InputPoint
        {
            var coords = new float[allInputPoints.Count][];
            for (var i = 0; i < allInputPoints.Count; i++)
            {
                var point    = allInputPoints[i];
                var position = point.Position;
                coords[i] = swapYZ ? new[] { position.x, position.z, position.y } : new[] { position.x, position.y, position.z };
            }

            return coords;
        }

        public static void AddSurfacePoint(SurfacePoint surfacePoint)
        {
            SurfacePoints.Add(surfacePoint);
            SurfacePoints.Sort();
        }

        public static void AddOrientation(Orientation orientation)
        {
            Orientations.Add(orientation);
            Orientations.Sort();
        }

        public static void AddSurfaceToStack(SurfaceStack surfaceStack)
        {
            SurfaceStack.Add(surfaceStack);
            SurfaceStack.Sort();
            AssignSeriesIds();
        }

        public static void AssignSeriesIds()
        {
            SurfaceStack.OrderBy(y => y.Position.y);
        }
    }
}