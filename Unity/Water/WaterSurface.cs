using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSurface : MonoBehaviour
{
    Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    public float GetHeightAtPos(Vector3 worldPos)
    {
        float h = material.GetFloat("_WaveHeight");
        float f = material.GetFloat("_WaveFrequency") * material.GetFloat("_WaveScale");
        float s = material.GetFloat("_WaveSpeed");

        float time = Time.time * s;

        float wave = (Mathf.Sin(worldPos.x * f + time) +
                      Mathf.Cos(worldPos.z * (f * 0.8f) + time * 1.5f)) * h;

        return transform.position.y + wave;
    }
}
