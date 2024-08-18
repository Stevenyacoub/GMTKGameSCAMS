using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// An object that creates interactive shadows that the player can run on
/// A ShadowableObject requires a mesh object to render and a parent with ShadowPoint children.
/// Adjust ShadowPoints to define the outline of the shadow's shape.
/// This generates a mesh of the shadow using ExtrudedMesh2D.
public class ShadowableObject : MonoBehaviour
{
    [SerializeField] private GameObject pointsParent;
    [SerializeField] private Material shadowMaterial;

    private GameObject _mainLight;
    private VertexPoints[] _points;
    private Vector2[] _shadowPointsXY;
    private ExtrudedMesh2D _shadowMesh;
    
    public bool flipShadowMeshDepth = true;
    
    /// Z-axis offset to give shadow depth for player to walk on.
    public const float SHADOW_MESH_DEPTH_OFFSET = 0.05f;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainLight = GameObject.FindWithTag("MainLight");
        _points = pointsParent.GetComponentsInChildren<VertexPoints>();
        _shadowPointsXY = new Vector2[_points.Length];

        // Set up the shadow mesh and its properties
        GameObject obj = new GameObject();
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<MeshFilter>();
        obj.GetComponent<MeshRenderer>().material = shadowMaterial;
        _shadowMesh = obj.AddComponent<ExtrudedMesh2D>();  // Must be added after Mesh Renderer and Filter

        _shadowMesh.flipShadowMeshDepth = flipShadowMeshDepth;
        _shadowMesh.GenerateMesh();

    }

    void FixedUpdate()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = LayerMask.GetMask("ReceivesShadows");

        for (int i = 0; i < _points.Length; i++)
        {
            RaycastHit hit;
            
            Vector3 origin = _mainLight.transform.position;
            Vector3 direction = _points[i].transform.position - origin;
            if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
                
                _shadowPointsXY[i] = new Vector2(hit.point.x, hit.point.y);
                _shadowMesh.shape2D = _shadowPointsXY;
                _shadowMesh.GenerateMesh();

                // Set the z position of the shadow.
                _shadowMesh.transform.position = new Vector3(_shadowMesh.transform.position.x, 
                    _shadowMesh.transform.position.y, 
                    hit.point.z - SHADOW_MESH_DEPTH_OFFSET); 
            }
            else
            {
                Debug.DrawRay(origin, direction * 1000, Color.red);
            }
            
        }
    }
}

