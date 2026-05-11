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
        float f = material.GetFloat("_WaveFrequency");
        float s = material.GetFloat("_WaveSpeed");

        float time = Time.time * s;

        Vector3 localPos = transform.InverseTransformPoint(worldPos);

        float w1 = Mathf.Sin(localPos.x * f + time);
        float w2 = Mathf.Sin((localPos.x + localPos.z) * f * 0.7f + time * 1.3f);
        float w3 = Mathf.Cos((localPos.z - localPos.x) * f * 2.5f + time * 2.0f);

        float wave = (w1 * 0.5f + w2 * 0.3f + w3 * 0.2f) * h;
        return transform.position.y + (wave * transform.lossyScale.y);
    }
}
