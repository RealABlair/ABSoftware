using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransformReference;

    [Header("Axes")]
    public bool XAxis = false;
    public bool YAxis = false;
    public bool ZAxis = true;

    [Header("Rotation")]
    public Vector3 eulerRotation = Vector3.zero;
    
    void Update()
    {
        Vector3 diff = cameraTransformReference.position - transform.position;

        float xRot = XAxis ? -Mathf.Atan2(diff.y, diff.z) * Mathf.Rad2Deg + 90f : 0f;
        float yRot = YAxis ? Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg : 0f;
        float zRot = ZAxis ? Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90f : 0f;
        transform.eulerAngles = eulerRotation + new Vector3(xRot, yRot, zRot);
    }
}
