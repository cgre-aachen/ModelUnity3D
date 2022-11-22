using System;
using System.Collections;
using System.Collections.Generic;
using Gempy;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Gempy.DataAssembler))]
public class AssemblerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Gempy.DataAssembler assemble = (Gempy.DataAssembler) target;

        if (GUILayout.Button("Spawn Surface Point"))
        {
            assemble.AddNewSurfacePointInstanceToScene();
            assemble.UpdateSurfacePoints();
        }

        if (GUILayout.Button("Spawn Orientation Point"))
        {
            assemble.AddNewOrientationPointInstanceToScene();
            assemble.UpdateSurfacePoints();
        }

        if (GUILayout.Button("Save Points to JSON"))
        {
            assemble.UpdateSurfacePoints();
            // assemble.SortSurfacePoints();
            // assemble.CountPoints();
            var json = assemble.SurfaceToJson(assemble.input_schema);
            assemble.SaveData(json, "InterfacePoints");

            var json2 = assemble.NSurfacePointsPerSurface(assemble._nSurfacePoints);
            assemble.SaveData(json2, "n_InterfacePoints_per_surface");

            var json3 = assemble.NSurfacePointsPerSurface(assemble._nSurfacePointsPerSeries);
            assemble.SaveData(json3, "n_InterfacePoints_per_series");

            // var json4 = assemble.NSurfacePointsPerSurface(assemble._nSurfacePointsPerSeries);

        }

    }
    
}
