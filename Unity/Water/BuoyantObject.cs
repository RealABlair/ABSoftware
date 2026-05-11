using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuoyantObject : MonoBehaviour
{
    [Header("Buoyancy Properties")]
    public float buoyancy = 1f;
    public float drag = 0.1f;

    public Vector3[] buoyancyPoints;

    WaterSurface currentWaterSurface;

    Rigidbody rb;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    public void OnDrawGizmosSelected()
    {
        if(buoyancyPoints != null)
        {
            Gizmos.color = Color.blue;
            for(int i = 0; i < buoyancyPoints.Length; i++)
            {
                Gizmos.DrawSphere(transform.TransformPoint(buoyancyPoints[i]), 0.1f);
            }
        }
    }

    private void FixedUpdate()
    {
        if (buoyancyPoints == null || currentWaterSurface == null)
            return;

        for(int i = 0; i < buoyancyPoints.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(buoyancyPoints[i]);
            float waterHeight = currentWaterSurface.GetHeightAtPos(worldPos);

            if(worldPos.y <= waterHeight)
            {
                float depth = waterHeight - worldPos.y;

                Vector3 force = Vector3.up * depth * buoyancy;

                rb.AddForceAtPosition(force, worldPos, ForceMode.Acceleration);

                Vector3 pointForce = rb.GetPointVelocity(worldPos);

                rb.AddForceAtPosition(-pointForce * drag, worldPos, ForceMode.Acceleration);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WaterSurface water))
            currentWaterSurface = water;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentWaterSurface.gameObject)
            currentWaterSurface = null;
    }
}
