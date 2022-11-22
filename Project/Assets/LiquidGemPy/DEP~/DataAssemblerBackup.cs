using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Gempy;
using UnityEngine.Rendering;

[ExecuteAlways]
public class DataAssembler : MonoBehaviour
{
    public Vector3[] Interfacevector;
    public GameObject[] Interfaces;
    public GameObject[] Orientations;
    private float pointsum;
    private float threshold = 0.05f;

    // public DataAssembler(Vector3[] interfacevector)
    // {
    //     Interfacevector = interfacevector;
    // }

    // Start is called before the first frame update
    void Start()
    {
        Interfaces = GameObject.FindGameObjectsWithTag("Interface");
        
        Interfacevector = new Vector3[Interfaces.Length];
        for (int i = 0; i < Interfaces.Length; i++)
        {
            Interfacevector[i] = new Vector3(Interfaces[i].transform.position.x,
                Interfaces[i].transform.position.y, Interfaces[i].transform.position.z);
        }

        Debug.Log($"{Interfaces.Length.ToString()} interface(s) found.");

        // new Gempy.Interfaces(1.0f, 1.0f, 1.0f, "F1");

        foreach (var point in Interfaces)
        {
            pointsum = point.transform.position.magnitude;
        }
        // Debug.Log($"The sum of Interfaces is {pointsum}");

        Orientations = GameObject.FindGameObjectsWithTag("Orientation");
        
        Debug.Log($"{Orientations.Length} orientation(s) found.");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Gatherpoints();

        }
    }

    void Gatherpoints()
    {
        var vSum = 0.0f;
        foreach (var v in Interfaces)
        {
            vSum = v.transform.position.magnitude;
        }
        
        var diff = pointsum - vSum;
        // Debug.Log($"vSum is {vSum}, pointsum is {pointsum}, diff is {diff}");
        if (Mathf.Abs(diff) > threshold)
        {
            Interfacevector = new Vector3[Interfaces.Length];
            for (int i = 0; i < Interfaces.Length; i++)
            {
                Interfacevector[i] = new Vector3(Interfaces[i].transform.position.x,
                    Interfaces[i].transform.position.y, Interfaces[i].transform.position.z);
            }

            pointsum = vSum;
            // Debug.Log($"The sum of Interfaces is {pointsum}");
            // Debug.Log(Interfaces[i].transform.position);
            // Debug.Log(Interfacevector[i]);
        }
        
    }
}

