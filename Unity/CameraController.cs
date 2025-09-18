using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform transformToTrack;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public Vector3 lookAtOffset;
    public float movementSpeed = 10f;

    void FixedUpdate()
    {
        Vector3 pos = transformToTrack.position + GetLocalOffset(transformToTrack, offset);
        Vector3 interpolatedPos = Vector3.Lerp(transform.position, pos, movementSpeed * Time.fixedDeltaTime);

        Vector3 deltaPos = (transformToTrack.position + lookAtOffset) - transform.position;
        float dist = Mathf.Sqrt(deltaPos.x * deltaPos.x + deltaPos.y * deltaPos.y + deltaPos.z * deltaPos.z);
        float yaw = Mathf.Atan2(deltaPos.x, deltaPos.z) * Mathf.Rad2Deg;
        float pitch = -Mathf.Asin(deltaPos.y / dist) * Mathf.Rad2Deg;

        transform.position = interpolatedPos;
        transform.rotation = Quaternion.Euler(new Vector3(pitch, yaw, 0f));
    }

    Vector3 GetLocalOffset(Transform local, Vector3 offset)
    {
        return local.right * offset.x + local.up * offset.y + local.forward * offset.z;
    }
}
