using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class light : MonoBehaviour
{
    private float duration = 5.0f;
    private float originalRange;
    private Light lightComp;
    // Start is called before the first frame update
    void Start()
    {
        lightComp = gameObject.AddComponent<Light>();
            
        lightComp.color = Color.magenta;

        originalRange = lightComp.range;
        
    }

    void Update()
    {
        var amplitude = Mathf.PingPong(Time.time, duration);

        amplitude = amplitude / duration * 0.5f + 0.5f;

        lightComp.range = originalRange * amplitude;

    }
}
