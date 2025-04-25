using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshCollider))]
public class CombineHouseMeshesForCollider : MonoBehaviour
{
    void Start()
    {
        GenerateCombinedMeshCollider();
    }

    void GenerateCombinedMeshCollider()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (MeshFilter mf in meshFilters)
        {
            if (mf.sharedMesh == null || mf.gameObject == this.gameObject)
                continue;

            CombineInstance ci = new CombineInstance();
            ci.mesh = mf.sharedMesh;
            ci.transform = mf.transform.localToWorldMatrix;
            combineInstances.Add(ci);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // for large meshes
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);

        MeshCollider collider = GetComponent<MeshCollider>();
        collider.sharedMesh = combinedMesh;
        collider.convex = false; // important for walking through interiors
    }
}
