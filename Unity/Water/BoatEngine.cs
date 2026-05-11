using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatEngine : BuoyantEffector
{
    public float force = 10f;
    public float turnForce = 1f;
    [Range(0f, 1f)] public float turnSpeedBias = 0.25f;

    public Vector3 rotorPos = new Vector3(0, -0.5f, -0.5f);

    public override void PreBuoyancy(Rigidbody rb, WaterSurface water)
    {
        float forward = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        Vector3 worldPoint = transform.TransformPoint(rotorPos);
        float depth = water.GetHeightAtPos(worldPoint) - worldPoint.y;
        if (depth > 0)
        {
            rb.AddForceAtPosition(transform.forward * forward * force, worldPoint, ForceMode.Acceleration);
            Vector3 pointVelocity = rb.GetPointVelocity(worldPoint);

            float dot = Vector3.Dot(rb.velocity.normalized, transform.forward);
            float speedFactor = Mathf.Lerp(1f, rb.velocity.magnitude, turnSpeedBias);
            rb.AddForceAtPosition(transform.right * -(turn * speedFactor * dot) * turnForce, worldPoint, ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(rotorPos), 0.1f);
    }
}
