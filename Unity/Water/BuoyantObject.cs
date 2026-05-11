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

    public Rigidbody rb { get; private set; }

    List<IBuoyantEffector> effectors = new List<IBuoyantEffector>();

    public bool AddEffector(IBuoyantEffector effector)
    {
        if(this.effectors.Contains(effector))
            return false;

        this.effectors.Add(effector);
        return true;
    }

    public bool RemoveEffector(IBuoyantEffector effector)
    {
        return this.effectors.Remove(effector);
    }

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

        for (int e = 0; e < effectors.Count; e++)
            effectors[e].PreBuoyancy(rb, currentWaterSurface);

        float perPointForce = buoyancy / buoyancyPoints.Length;
        for (int i = 0; i < buoyancyPoints.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(buoyancyPoints[i]);
            float waterHeight = currentWaterSurface.GetHeightAtPos(worldPos);

            if(worldPos.y <= waterHeight)
            {
                float depth = waterHeight - worldPos.y;

                Vector3 force = Vector3.up * depth * perPointForce;

                rb.AddForceAtPosition(force, worldPos, ForceMode.Acceleration);

                Vector3 pointForce = rb.GetPointVelocity(worldPos);

                for (int e = 0; e < effectors.Count; e++)
                    effectors[e].EffectSubmerged(rb, worldPos, depth, pointForce);

                rb.AddForceAtPosition(-pointForce * drag, worldPos, ForceMode.Acceleration);
            }
            else
            {
                Vector3 pointForce = rb.GetPointVelocity(worldPos);

                for (int e = 0; e < effectors.Count; e++)
                    effectors[e].EffectSurfaced(rb, worldPos, pointForce);
            }
        }

        for (int e = 0; e < effectors.Count; e++)
            effectors[e].PostBuoyancy(rb, currentWaterSurface);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out WaterSurface water))
        {
            for (int e = 0; e < effectors.Count; e++)
                effectors[e].OnWaterSurfaceEnter(currentWaterSurface);
            currentWaterSurface = water;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentWaterSurface.gameObject)
        {
            for (int e = 0; e < effectors.Count; e++)
                effectors[e].OnWaterSurfaceExit(currentWaterSurface);
            currentWaterSurface = null;
        }
    }
}

public interface IBuoyantEffector
{
    public void PreBuoyancy(Rigidbody rb, WaterSurface water);
    public void EffectSubmerged(Rigidbody rb, Vector3 worldPoint, float depth, Vector3 velocity);
    public void EffectSurfaced(Rigidbody rb, Vector3 worldPoint, Vector3 velocity);
    public void PostBuoyancy(Rigidbody rb, WaterSurface water);

    public void OnWaterSurfaceEnter(WaterSurface water);
    public void OnWaterSurfaceExit(WaterSurface water);
}

public abstract class BuoyantEffector : MonoBehaviour, IBuoyantEffector
{
    protected BuoyantObject obj;

    void OnEnable()
    {
        obj = GetComponent<BuoyantObject>();
        if (obj != null)
            obj.AddEffector(this);
    }

    void OnDisable()
    {
        if (obj != null)
            obj.RemoveEffector(this);
    }

    public virtual void EffectSubmerged(Rigidbody rb, Vector3 worldPoint, float depth, Vector3 velocity)
    {
        
    }

    public virtual void EffectSurfaced(Rigidbody rb, Vector3 worldPoint, Vector3 velocity)
    {
        
    }

    public virtual void OnWaterSurfaceEnter(WaterSurface water)
    {
        
    }

    public virtual void OnWaterSurfaceExit(WaterSurface water)
    {
        
    }

    public virtual void PostBuoyancy(Rigidbody rb, WaterSurface water)
    {
        
    }

    public virtual void PreBuoyancy(Rigidbody rb, WaterSurface water)
    {
        
    }
}