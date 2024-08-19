using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ExtrudedMesh2D : MonoBehaviour
{
    public Vector2[] shape2D; // 2D vertices
    public float depth = .1f; // Depth for extrusion
    
    /// If true, extrudes the shape along the positive Z-axis. Otherwise, extrudes along negative Z-axis.
    /// Adjust this depending on the direction of the scene. 
    public bool flipShadowMeshDepth = true;

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    void Awake()
    {
        _mesh = new Mesh();
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateMesh();
        
        if (flipShadowMeshDepth)
        {
            depth *= -1.0f;
        }
    }
    
    
    public void GenerateMesh() {
        if (shape2D == null || shape2D.Length == 0)
        {
            return;
        }
        
        int numVertices = shape2D.Length;
        Vector3[] vertices = new Vector3[numVertices * 2];
        int[] triangles = new int[(numVertices - 2) * 6 + numVertices * 6];
        
        // Create vertices
        for (int i = 0; i < shape2D.Length; i++)
        {
            vertices[i] = new Vector3(shape2D[i].x, shape2D[i].y, 0); // Front face
            vertices[i + shape2D.Length] = new Vector3(shape2D[i].x, shape2D[i].y, depth); // Back face
        }
        
        // Create front face triangles
        int triIndex = 0;
        for (int i = 0; i < numVertices - 2; i++)
        {
            triangles[triIndex++] = 0;
            triangles[triIndex++] = i + 1;
            triangles[triIndex++] = i + 2;
        }
        
        // Create back face triangles
        for (int i = 0; i < numVertices - 2; i++)
        {
            triangles[triIndex++] = numVertices;
            triangles[triIndex++] = numVertices + i + 2;
            triangles[triIndex++] = numVertices + i + 1;
        }
        
        // Create side face triangles
        for (int i = 0; i < numVertices; i++)
        {
            int nextIndex = (i + 1) % numVertices;
            triangles[triIndex++] = i;
            triangles[triIndex++] = nextIndex + numVertices;
            triangles[triIndex++] = nextIndex;

            triangles[triIndex++] = i;
            triangles[triIndex++] = i + numVertices;
            triangles[triIndex++] = nextIndex + numVertices;
        }
        
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();

        _meshFilter.mesh = _mesh;
        _meshCollider.sharedMesh = _mesh;
    }
}
