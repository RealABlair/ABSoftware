using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDepth : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;    
    }
}
