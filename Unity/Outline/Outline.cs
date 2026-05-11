using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Outline : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color outlineColor = Color.green;
    
    [SerializeField] private float outlineWidth = 1.15f;

    [SerializeField] private UnityEngine.Rendering.CompareFunction outlineZTest = UnityEngine.Rendering.CompareFunction.NotEqual;
    [SerializeField] private UnityEngine.Rendering.CompareFunction maskZTest = UnityEngine.Rendering.CompareFunction.Always;

    [Header("Shader Names")]
    private string outlineShaderName = "Unlit/OutlineShader";
    private string maskShaderName = "Unlit/OutlineMaskShader";

    private Renderer objectRenderer;
    private Material outlineMaterial;
    private Material maskMaterial;
    private MaterialPropertyBlock propertyBlock;
    private bool isDirty = true;

    public Color Color { get { return outlineColor; } set { outlineColor = value; isDirty = true; } }
    public float Width { get { return outlineWidth; } set { outlineWidth = value; isDirty = true; } }
    public UnityEngine.Rendering.CompareFunction OutlineZTest { get { return outlineZTest; } set { outlineZTest = value; isDirty = true; } }
    public UnityEngine.Rendering.CompareFunction MaskZTest { get { return maskZTest; } set { maskZTest = value; isDirty = true; } }

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        BakeNormals();
    }

    void OnEnable()
    {
        CreateMaterials();
        ApplyMaterials();
        isDirty = true;
    }

    void OnDisable()
    {
        RemoveMaterials();
    }

    void OnValidate()
    {
        isDirty = true;
    }

    void Update()
    {
        if (isDirty && outlineMaterial != null)
        {
            UpdateProperties();
        }
    }

    private void CreateMaterials()
    {
        if (outlineMaterial == null)
        {
            Shader outlineShader = Shader.Find(outlineShaderName);
            if (outlineShader)
            {
                outlineMaterial = new Material(outlineShader);
                outlineMaterial.hideFlags = HideFlags.HideAndDontSave;
                outlineMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.NotEqual);
            }
        }

        if (maskMaterial == null)
        {
            Shader maskShader = Shader.Find(maskShaderName);
            if (maskShader)
            {
                maskMaterial = new Material(maskShader);
                maskMaterial.hideFlags = HideFlags.HideAndDontSave;
                maskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
            }
        }
    }

    private void ApplyMaterials()
    {
        if (objectRenderer == null) return;

        List<Material> mats = new List<Material>(objectRenderer.sharedMaterials);
        if (maskMaterial != null && !mats.Contains(maskMaterial)) mats.Add(maskMaterial);
        if (outlineMaterial != null && !mats.Contains(outlineMaterial)) mats.Add(outlineMaterial);

        objectRenderer.sharedMaterials = mats.ToArray();
    }

    private void RemoveMaterials()
    {
        if (objectRenderer == null) return;

        List<Material> mats = new List<Material>(objectRenderer.sharedMaterials);
        mats.Remove(maskMaterial);
        mats.Remove(outlineMaterial);

        objectRenderer.sharedMaterials = mats.ToArray();
    }

    private void UpdateProperties()
    {
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();

        outlineMaterial.SetFloat("_ZTest", (float)outlineZTest);
        maskMaterial.SetFloat("_ZTest", (float)maskZTest);

        objectRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_OutlineColor", outlineColor);
        propertyBlock.SetFloat("_OutlineWidth", outlineWidth);
        objectRenderer.SetPropertyBlock(propertyBlock);

        isDirty = false;
    }

    [ContextMenu("Bake Smooth Normals")]
    public void BakeNormals()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;

        Mesh mesh = mf.sharedMesh;
        if (!mesh.isReadable)
        {
            Debug.LogError("Mesh is not readable! Change import settings.");
            return;
        }

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Dictionary<Vector3, Vector3> averageNormals = new Dictionary<Vector3, Vector3>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (!averageNormals.ContainsKey(vertices[i]))
                averageNormals[vertices[i]] = Vector3.zero;
            averageNormals[vertices[i]] += normals[i];
        }

        Vector3[] smoothNormals = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            smoothNormals[i] = averageNormals[vertices[i]].normalized;
        }

        mesh.SetUVs(1, smoothNormals);
    }

    private void OnDestroy()
    {
        if (outlineMaterial) DestroyImmediate(outlineMaterial);
        if (maskMaterial) DestroyImmediate(maskMaterial);
    }
}